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
        private int markerCount;

        internal IdlePointMarkerController(
            PropertyDefinition definition,
            Transform propertyTransform,
            int originalIdlePointCount)
        {
            this.definition = definition;
            this.propertyTransform = propertyTransform;
            this.originalIdlePointCount = originalIdlePointCount;
        }

        internal void SetVisible(Il2CppReferenceArray<Transform> idlePoints, bool visible)
        {
            if (!visible)
            {
                Clear();
                return;
            }

            if (idlePoints == null || idlePoints.Length == 0)
            {
                return;
            }

            if (markerRoot != null && markerCount == idlePoints.Length)
            {
                Update(idlePoints);
                return;
            }

            Clear();
            markerRoot = new GameObject($"ConfigurableMoreEmployees_DebugMarkers_{definition.Key}");
            markerRoot.transform.SetParent(propertyTransform, true);

            for (var i = 0; i < idlePoints.Length; i++)
            {
                var idlePoint = idlePoints[i];
                if (idlePoint == null)
                {
                    continue;
                }

                var color = i < originalIdlePointCount ? Color.green : Color.cyan;
                IdlePointDebugVisualizer.CreateMarker(markerRoot.transform, idlePoint.position, color, $"{definition.Key}_{i}");
            }

            markerCount = idlePoints.Length;
        }

        internal void Clear()
        {
            if (markerRoot == null)
            {
                return;
            }

            Object.Destroy(markerRoot);
            markerRoot = null;
            markerCount = 0;
        }

        private void Update(Il2CppReferenceArray<Transform> idlePoints)
        {
            for (var i = 0; i < idlePoints.Length && i < markerRoot.transform.childCount; i++)
            {
                var idlePoint = idlePoints[i];
                if (idlePoint == null)
                {
                    continue;
                }

                markerRoot.transform.GetChild(i).position = idlePoint.position;
            }
        }
    }
}
