using Il2CppScheduleOne.Property;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace ConfigurableMoreEmployees
{
    internal sealed class PropertyHandler
    {
        private readonly Property property;
        private readonly int originalEmployeeCapacity;
        private readonly int originalIdlePointCount;
        private GameObject debugMarkerRoot;
        private int debugMarkerCount;

        internal PropertyHandler(PropertyBinding binding, Property property)
        {
            Binding = binding;
            this.property = property;
            originalEmployeeCapacity = property.EmployeeCapacity;
            originalIdlePointCount = property.EmployeeIdlePoints?.Length ?? 0;
        }

        internal PropertyBinding Binding { get; }

        internal void ApplyMaxEmployees(int maxEmployees)
        {
            if (maxEmployees < 0)
            {
                MainMod.Instance.LoggerInstance.Error(
                    $"{Binding.DisplayName}: configured max employees cannot be negative ({maxEmployees}). Skipping this property.");
                return;
            }

            var placementPlan = IdlePointPlacementPlanner.Plan(this, maxEmployees);
            if (!placementPlan.Success)
            {
                MainMod.Instance.LoggerInstance.Error(placementPlan.ErrorMessage);
                return;
            }

            if (placementPlan.Placements.Length > 0)
            {
                if (!CreateMissingIdlePoints(placementPlan.Placements))
                {
                    return;
                }
            }

            property.EmployeeCapacity = maxEmployees;
            MainMod.Instance.LoggerInstance.Msg(
                $"{Binding.DisplayName}: employee capacity {originalEmployeeCapacity} -> {property.EmployeeCapacity}, idle points {GetIdlePointCount()}");
        }

        internal Vector3 GetIdlePointStartLocation()
        {
            var idlePoints = property.EmployeeIdlePoints;
            if (idlePoints != null && idlePoints.Length > 0 && idlePoints[0] != null)
            {
                return idlePoints[0].position;
            }

            if (property.InteriorSpawnPoint != null)
            {
                return property.InteriorSpawnPoint.position;
            }

            if (property.SpawnPoint != null)
            {
                return property.SpawnPoint.position;
            }

            return property.transform.position;
        }

        internal Vector3[] GetCurrentIdlePointPositions()
        {
            var idlePoints = property.EmployeeIdlePoints;
            if (idlePoints == null || idlePoints.Length == 0)
            {
                return new Vector3[0];
            }

            var positions = new Vector3[idlePoints.Length];
            for (var i = 0; i < idlePoints.Length; i++)
            {
                positions[i] = idlePoints[i] != null ? idlePoints[i].position : GetIdlePointStartLocation();
            }

            return positions;
        }

        internal Vector3[] GetVanillaIdlePointPositions()
        {
            var idlePoints = property.EmployeeIdlePoints;
            if (idlePoints == null || idlePoints.Length == 0 || originalIdlePointCount == 0)
            {
                return new Vector3[0];
            }

            var vanillaCount = Mathf.Min(originalIdlePointCount, idlePoints.Length);
            var positions = new Vector3[vanillaCount];
            for (var i = 0; i < vanillaCount; i++)
            {
                positions[i] = idlePoints[i] != null ? idlePoints[i].position : GetIdlePointStartLocation();
            }

            return positions;
        }

        internal Transform GetIdlePointParent()
        {
            var existingParent = FindExistingIdlePointParent();
            if (existingParent != null)
            {
                return existingParent;
            }

            var parent = new GameObject("EmployeeIdlePoints");
            parent.transform.SetParent(property.transform, false);
            return parent.transform;
        }

        internal Transform GetFirstExistingIdlePoint()
        {
            var idlePoints = property.EmployeeIdlePoints;
            if (idlePoints == null)
            {
                return null;
            }

            for (var i = 0; i < idlePoints.Length; i++)
            {
                if (idlePoints[i] != null)
                {
                    return idlePoints[i];
                }
            }

            return null;
        }

        internal void SetIdlePointMarkersVisible(bool visible)
        {
            if (!visible)
            {
                ClearIdlePointMarkers();
                return;
            }

            var idlePoints = property.EmployeeIdlePoints;
            if (idlePoints == null || idlePoints.Length == 0)
            {
                return;
            }

            if (debugMarkerRoot != null && debugMarkerCount == idlePoints.Length)
            {
                UpdateIdlePointMarkers(idlePoints);
                return;
            }

            ClearIdlePointMarkers();
            debugMarkerRoot = new GameObject($"ConfigurableMoreEmployees_DebugMarkers_{Binding.Key}");
            debugMarkerRoot.transform.SetParent(property.transform, true);

            for (var i = 0; i < idlePoints.Length; i++)
            {
                var idlePoint = idlePoints[i];
                if (idlePoint == null)
                {
                    continue;
                }

                var color = i < originalIdlePointCount ? Color.green : Color.cyan;
                IdlePointDebugVisualizer.CreateMarker(debugMarkerRoot.transform, idlePoint.position, color, $"{Binding.Key}_{i}");
            }

            debugMarkerCount = idlePoints.Length;
        }

        internal void ClearIdlePointMarkers()
        {
            if (debugMarkerRoot == null)
            {
                return;
            }

            Object.Destroy(debugMarkerRoot);
            debugMarkerRoot = null;
            debugMarkerCount = 0;
        }

        private void UpdateIdlePointMarkers(Il2CppReferenceArray<Transform> idlePoints)
        {
            for (var i = 0; i < idlePoints.Length && i < debugMarkerRoot.transform.childCount; i++)
            {
                var idlePoint = idlePoints[i];
                if (idlePoint == null)
                {
                    continue;
                }

                debugMarkerRoot.transform.GetChild(i).position = idlePoint.position;
            }
        }

        private int GetIdlePointCount()
        {
            return property.EmployeeIdlePoints?.Length ?? 0;
        }

        private bool CreateMissingIdlePoints(IdlePointPlacement[] placements)
        {
            var template = MainMod.Instance.GetIdlePointTemplate();
            if (template == null)
            {
                MainMod.Instance.LoggerInstance.Error(
                    $"{Binding.DisplayName}: no idle point template was available. Skipping this property.");
                return false;
            }

            var idlePoints = property.EmployeeIdlePoints;
            var currentCount = idlePoints?.Length ?? 0;
            var expandedIdlePoints = new Il2CppReferenceArray<Transform>(currentCount + placements.Length);

            for (var i = 0; i < currentCount; i++)
            {
                expandedIdlePoints[i] = idlePoints[i];
            }

            var factory = new IdlePointFactory(template);
            for (var i = 0; i < placements.Length; i++)
            {
                expandedIdlePoints[currentCount + i] = factory.Create(this, placements[i], currentCount + i);
            }

            property.EmployeeIdlePoints = expandedIdlePoints;
            MainMod.Instance.LoggerInstance.Msg(
                $"{Binding.DisplayName}: expanded idle points {currentCount} -> {property.EmployeeIdlePoints.Length}");
            return true;
        }

        private Transform FindExistingIdlePointParent()
        {
            var directChild = property.transform.Find("EmployeeIdlePoints");
            if (directChild != null)
            {
                return directChild;
            }

            var idlePoints = property.EmployeeIdlePoints;
            if (idlePoints != null && idlePoints.Length > 0 && idlePoints[0] != null && idlePoints[0].parent != null)
            {
                return idlePoints[0].parent;
            }

            if (property.EmployeeContainer != null)
            {
                var employeeContainerChild = property.EmployeeContainer.Find("EmployeeIdlePoints");
                if (employeeContainerChild != null)
                {
                    return employeeContainerChild;
                }
            }

            return null;
        }
    }
}
