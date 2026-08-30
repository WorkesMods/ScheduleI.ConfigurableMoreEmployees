using static ConfigurableMoreEmployees.GridAxisDirection;
using static ConfigurableMoreEmployees.GridFillOrder;
using static ConfigurableMoreEmployees.IdlePointPlacementDsl;

namespace ConfigurableMoreEmployees
{
    internal static class BungalowDefinition
    {
        internal static PropertyDefinition Create()
        {
            return new PropertyDefinition(
                key: "Bungalow",
                gameObjectName: "Bungalow",
                displayName: "Bungalow",
                propertyCode: "bungalow",
                vanillaIdlePointCount: 5,
                defaultMaxEmployees: 7,
                addMannyDialogueChoice: false,
                placementAreas: new[]
                {
                    Explicit(
                        Point(-167.4f, -4f, 112f, 310f),
                        Point(-165.9f, -4f, 112f, 350f),
                        Point(-164.4f, -4f, 112f, 350f)),
                    Grid(
                        Box(
                            Point2(-168f, 114.6f),
                            Point2(-164.8f, 115.2f)),
                        height: -3.7f,
                        xCount: 4,
                        zCount: 1,
                        yRotation: 350f,
                        xDirection: Negative,
                        zDirection: Positive,
                        fillOrder: XMajor),
                    Grid(
                        Box(
                            Point2(-163.5f, 115.4f),
                            Point2(-162.5f, 118.4f)),
                        height: -3.9f,
                        xCount: 1,
                        zCount: 3,
                        yRotation: 260f,
                        xDirection: Positive,
                        zDirection: Positive,
                        fillOrder: ZMajor),
                    Grid(
                        Box(
                            Point2(-173.3f, 107.4f),
                            Point2(-169.9f, 109f)),
                        height: -3.9f,
                        xCount: 4,
                        zCount: 2,
                        yRotation: 350f,
                        xDirection: Negative,
                        zDirection: Negative,
                        fillOrder: XMajor)
                },
                preferenceKey: "BungalowMaxEmployees",
                preferenceDescription: "Maximum employees allowed at the Bungalow.",
                required: true);
        }
    }
}
