using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace ConfigurableMoreEmployees
{
    internal sealed class IdlePointMarkerController
    {
        private readonly PropertyDefinition definition;
        private readonly Transform propertyTransform;
        private readonly int originalIdlePointCount;
        private GameObject markerRoot;

        internal IdlePointMarkerController(
            PropertyDefinition definition,
            Transform propertyTransform,
            int originalIdlePointCount)
        {
            this.definition = definition;
            this.propertyTransform = propertyTransform;
            this.originalIdlePointCount = originalIdlePointCount;
        }

        internal void SetVisible(
            Il2CppReferenceArray<Transform> idlePoints,
            IdlePointPlacement[] supportedGeneratedPlacements,
            int configuredGeneratedIdlePointCount,
            bool visible)
        {
            if (!visible)
            {
                Clear();
                return;
            }

            if ((idlePoints == null || idlePoints.Length == 0) &&
                (supportedGeneratedPlacements == null || supportedGeneratedPlacements.Length == 0))
            {
                return;
            }

            Clear();
            markerRoot = new GameObject($"ConfigurableMoreEmployees_DebugMarkers_{definition.Key}");
            markerRoot.transform.SetParent(propertyTransform, true);

            if (idlePoints != null)
            {
                for (var i = 0; i < idlePoints.Length; i++)
                {
                    var idlePoint = idlePoints[i];
                    if (idlePoint == null)
                    {
                        continue;
                    }

                    var color = GetExistingIdlePointMarkerColor(i, configuredGeneratedIdlePointCount);
                    IdlePointDebugVisualizer.CreateMarker(markerRoot.transform, idlePoint.position, color, $"{definition.Key}_{i}");
                }
            }

            var existingGeneratedIdlePointCount = idlePoints != null
                ? Mathf.Max(0, idlePoints.Length - originalIdlePointCount)
                : 0;
            CreateUnusedPlacementMarkers(
                supportedGeneratedPlacements,
                configuredGeneratedIdlePointCount,
                existingGeneratedIdlePointCount);
        }

        internal void Clear()
        {
            if (markerRoot == null)
            {
                return;
            }

            Object.Destroy(markerRoot);
            markerRoot = null;
        }

        private Color GetExistingIdlePointMarkerColor(int idlePointIndex, int configuredGeneratedIdlePointCount)
        {
            if (idlePointIndex < originalIdlePointCount)
            {
                return Color.green;
            }

            var generatedIndex = idlePointIndex - originalIdlePointCount;
            return generatedIndex < configuredGeneratedIdlePointCount
                ? Color.cyan
                : new Color(0.6f, 0f, 1f);
        }

        private void CreateUnusedPlacementMarkers(
            IdlePointPlacement[] supportedGeneratedPlacements,
            int configuredGeneratedIdlePointCount,
            int existingGeneratedIdlePointCount)
        {
            if (supportedGeneratedPlacements == null)
            {
                return;
            }

            var purple = new Color(0.6f, 0f, 1f);
            var firstPreviewIndex = Mathf.Max(configuredGeneratedIdlePointCount, existingGeneratedIdlePointCount);
            for (var i = firstPreviewIndex; i < supportedGeneratedPlacements.Length; i++)
            {
                IdlePointDebugVisualizer.CreateMarker(
                    markerRoot.transform,
                    supportedGeneratedPlacements[i].Position,
                    purple,
                    $"{definition.Key}_Unused_{i}");
            }
        }
    }
}
