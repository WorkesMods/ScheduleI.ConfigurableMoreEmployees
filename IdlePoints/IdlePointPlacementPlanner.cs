using System.Collections.Generic;
using UnityEngine;

namespace ConfigurableMoreEmployees
{
    internal sealed class IdlePointPlacementPlan
    {
        private IdlePointPlacementPlan(bool success, IdlePointPlacement[] placements, int maxEmployees, string errorMessage)
        {
            Success = success;
            Placements = placements;
            MaxEmployees = maxEmployees;
            ErrorMessage = errorMessage;
        }

        internal bool Success { get; }
        internal IdlePointPlacement[] Placements { get; }
        internal int MaxEmployees { get; }
        internal string ErrorMessage { get; }

        internal static IdlePointPlacementPlan Ok(IdlePointPlacement[] placements, int maxEmployees)
        {
            return new IdlePointPlacementPlan(true, placements, maxEmployees, string.Empty);
        }

        internal static IdlePointPlacementPlan Fail(string errorMessage)
        {
            return new IdlePointPlacementPlan(false, new IdlePointPlacement[0], 0, errorMessage);
        }
    }

    internal static class IdlePointPlacementPlanner
    {
        private const float BoundsEpsilon = 0.001f;

        internal static IdlePointPlacementPlan Plan(PropertyHandler handler, int targetMaxEmployees)
        {
            var currentIdlePoints = handler.GetCurrentIdlePointPositions();
            var currentCount = currentIdlePoints.Length;
            var definition = handler.Definition;
            var vanillaIdlePoints = handler.GetVanillaIdlePointPositions();
            var vanillaCount = vanillaIdlePoints.Length;
            if (vanillaCount != definition.VanillaIdlePointCount)
            {
                MainMod.Instance.LoggerInstance.Warning(
                    $"{definition.DisplayName}: expected {definition.VanillaIdlePointCount} vanilla idle points, found {vanillaCount}.");
            }

            var allGeneratedPlacements = GetAllGeneratedPlacements(handler, definition);
            var supportedMaxEmployees = vanillaCount + allGeneratedPlacements.Count;
            var maxEmployees = Mathf.Min(targetMaxEmployees, supportedMaxEmployees);

            if (targetMaxEmployees > supportedMaxEmployees)
            {
                MainMod.Instance.LoggerInstance.Warning(
                    $"{definition.DisplayName}: attempted to configure {targetMaxEmployees} employees, " +
                    $"but placement rules support {supportedMaxEmployees}. Clamping to {supportedMaxEmployees}.");
            }

            var generatedAlreadyPresent = Mathf.Max(0, currentCount - vanillaCount);
            var generatedNeeded = Mathf.Max(0, maxEmployees - vanillaCount);
            if (generatedAlreadyPresent >= generatedNeeded)
            {
                return IdlePointPlacementPlan.Ok(new IdlePointPlacement[0], maxEmployees);
            }

            var missingPlacements = new IdlePointPlacement[generatedNeeded - generatedAlreadyPresent];
            for (var i = 0; i < missingPlacements.Length; i++)
            {
                missingPlacements[i] = allGeneratedPlacements[generatedAlreadyPresent + i];
            }

            return IdlePointPlacementPlan.Ok(missingPlacements, maxEmployees);
        }

        internal static IdlePointPlacement[] GetSupportedGeneratedPlacements(PropertyHandler handler)
        {
            return GetAllGeneratedPlacements(handler, handler.Definition).ToArray();
        }

        private static List<IdlePointPlacement> GetAllGeneratedPlacements(
            PropertyHandler handler,
            PropertyDefinition definition)
        {
            var generatedPlacements = new List<IdlePointPlacement>();
            TryGeneratePlacements(handler, definition, generatedPlacements, int.MaxValue);
            return generatedPlacements;
        }

        private static void TryGeneratePlacements(
            PropertyHandler handler,
            PropertyDefinition definition,
            List<IdlePointPlacement> generatedPlacements,
            int requestedGeneratedCount)
        {
            foreach (var area in definition.PlacementAreas)
            {
                if (generatedPlacements.Count >= requestedGeneratedCount)
                {
                    return;
                }

                var startLocation = handler.GetIdlePointStartLocation();
                var bounds = area.Bounds?.GetPoints(startLocation);
                if (bounds == null && area.Strategy.RequiresBounds)
                {
                    MainMod.Instance.LoggerInstance.Warning(
                        $"{definition.DisplayName}: skipped placement area because {area.Strategy.GetType().Name} requires bounds.");
                    continue;
                }

                var attemptCount = area.Strategy.GetAttemptCount();

                for (var attempt = 0; attempt < attemptCount && generatedPlacements.Count < requestedGeneratedCount; attempt++)
                {
                    var placement = bounds == null
                        ? area.Strategy.GetPlacement(startLocation, attempt)
                        : area.Strategy.GetPlacement(startLocation, bounds, attempt);
                    if (area.ValidateBounds && bounds != null && !IsInsideBounds(placement.Position, bounds))
                    {
                        MainMod.Instance.VerboseLog($"{definition.DisplayName}: rejected idle point outside bounds at {FormatPosition(placement.Position)}.");
                        continue;
                    }

                    generatedPlacements.Add(placement);
                    MainMod.Instance.VerboseLog($"{definition.DisplayName}: accepted generated idle point at {FormatPosition(placement.Position)}, y rotation {placement.YRotation:0.##}.");
                }
            }
        }

        private static string FormatPosition(Vector3 position)
        {
            return $"({position.x:0.00}, {position.y:0.00}, {position.z:0.00})";
        }

        private static bool IsInsideBounds(Vector3 position, Vector2[] bounds)
        {
            var point = new Vector2(position.x, position.z);
            if (IsOnBoundsEdge(point, bounds))
            {
                return true;
            }

            var inside = false;

            for (int i = 0, j = bounds.Length - 1; i < bounds.Length; j = i++)
            {
                var current = bounds[i];
                var previous = bounds[j];

                if ((current.y > point.y) != (previous.y > point.y) &&
                    point.x < (previous.x - current.x) * (point.y - current.y) / (previous.y - current.y) + current.x)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        private static bool IsOnBoundsEdge(Vector2 point, Vector2[] bounds)
        {
            for (int i = 0, j = bounds.Length - 1; i < bounds.Length; j = i++)
            {
                if (IsPointOnLineSegment(point, bounds[j], bounds[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            var segmentX = end.x - start.x;
            var segmentY = end.y - start.y;
            var pointX = point.x - start.x;
            var pointY = point.y - start.y;
            var cross = segmentX * pointY - segmentY * pointX;
            if (Mathf.Abs(cross) > BoundsEpsilon)
            {
                return false;
            }

            var dot = pointX * segmentX + pointY * segmentY;
            if (dot < -BoundsEpsilon)
            {
                return false;
            }

            var segmentLengthSquared = segmentX * segmentX + segmentY * segmentY;
            return dot <= segmentLengthSquared + BoundsEpsilon;
        }
    }
}
