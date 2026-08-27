using UnityEngine;

namespace ConfigurableMoreEmployees
{
    internal sealed class IdlePointFactory
    {
        private readonly Transform template;

        internal IdlePointFactory(Transform template)
        {
            this.template = template;
        }

        internal Transform Create(PropertyHandler handler, IdlePointPlacement placement, int index)
        {
            var clone = Object.Instantiate(template.gameObject);
            clone.name = $"ConfigurableMoreEmployees_IdlePoint_{handler.Definition.Key}_{index}";

            var parent = handler.GetIdlePointParent();
            clone.transform.SetParent(parent, true);
            clone.transform.position = placement.Position;
            clone.transform.localRotation = Quaternion.Euler(0f, placement.YRotation, 0f);

            return clone.transform;
        }
    }
}
