using UnityEngine;

namespace ConfigurableMoreEmployees
{
    internal static class IdlePointDebugVisualizer
    {
        private const float MarkerSize = 0.25f;

        internal static void CreateMarker(Transform parent, Vector3 position, Color color, string name)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = $"ConfigurableMoreEmployees_IdlePointMarker_{name}";
            marker.transform.SetParent(parent, true);
            marker.transform.position = position;
            marker.transform.localScale = new Vector3(MarkerSize, MarkerSize, MarkerSize);

            var collider = marker.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }

            var renderer = marker.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                return;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = shader != null ? new Material(shader) : new Material(renderer.material);
            material.color = color;
            renderer.material = material;
        }
    }
}
