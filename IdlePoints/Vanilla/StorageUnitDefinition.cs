using static ConfigurableMoreEmployees.GridAxisDirection;
using static ConfigurableMoreEmployees.GridFillOrder;
using static ConfigurableMoreEmployees.IdlePointPlacementDsl;

namespace ConfigurableMoreEmployees
{
    internal static class StorageUnitDefinition
    {
        internal static PropertyDefinition Create()
        {
            return new PropertyDefinition(
                key: "StorageUnit",
                gameObjectName: "StorageUnit",
                displayName: "Storage Unit",
                propertyCode: "storageunit",
                vanillaIdlePointCount: 3,
                defaultMaxEmployees: 5,
                addMannyDialogueChoice: false,
                placementAreas: new[]
                {
                    Explicit(
                        Point(-7.5f, 0.1f, 102.9f, 180f),
                        Point(-6.7f, 0.1f, 102.9f, 180f),
                        Point(-4.5f, 0.1f, 103.2f, 180f),
                        Point(-1.8f, 0.1f, 103.2f, 270f)),
                    Grid(
                        Box(
                            Point2(-3.8f, 97.6f),
                            Point2(-2.2f, 101.3f)),
                        height: 0.1f,
                        xCount: 2,
                        zCount: 4,
                        yRotation: 270f,
                        xDirection: Positive,
                        zDirection: Positive,
                        fillOrder: ZMajor)
                },
                preferenceKey: "StorageUnitMaxEmployees",
                preferenceDescription: "Maximum employees allowed at the Storage Unit.",
                required: true);
        }
    }
}
