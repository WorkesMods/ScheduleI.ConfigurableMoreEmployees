using static ConfigurableMoreEmployees.GridAxisDirection;
using static ConfigurableMoreEmployees.GridFillOrder;
using static ConfigurableMoreEmployees.IdlePointPlacementDsl;

namespace ConfigurableMoreEmployees
{
    internal static class BarnDefinition
    {
        internal static PropertyDefinition Create()
        {
            return new PropertyDefinition(
                key: "Barn",
                gameObjectName: "Barn",
                displayName: "Barn",
                propertyCode: "barn",
                vanillaIdlePointCount: 10,
                defaultMaxEmployees: 13,
                addMannyDialogueChoice: false,
                placementAreas: new[]
                {
                    Grid(
                        Box(
                            Point2(180.7f, -14.4f),
                            Point2(182.3f, -12.6f)),
                        height: 0.1f,
                        xCount: 2,
                        zCount: 2,
                        yRotation: 270f,
                        xDirection: Negative,
                        zDirection: Positive,
                        fillOrder: ZMajor),
                    Grid(
                        Box(
                            Point2(180.1f, -18.4f),
                            Point2(180.7f, -14.8f)),
                        height: 0.1f,
                        xCount: 1,
                        zCount: 3,
                        yRotation: 270f,
                        xDirection: Negative,
                        zDirection: Negative,
                        fillOrder: ZMajor),
                    Grid(
                        Box(
                            Point2(175.6f, -15.9f),
                            Point2(176.3f, -12.7f)),
                        height: 0f,
                        xCount: 1,
                        zCount: 3,
                        yRotation: 90f,
                        xDirection: Negative,
                        zDirection: Positive,
                        fillOrder: ZMajor),
                    Grid(
                        Box(
                            Point2(175.6f, -9.3f),
                            Point2(176.3f, -6f)),
                        height: 0f,
                        xCount: 1,
                        zCount: 3,
                        yRotation: 90f,
                        xDirection: Negative,
                        zDirection: Negative,
                        fillOrder: ZMajor),
                    Grid(
                        Box(
                            Point2(183.1f, -3.3f),
                            Point2(189.3f, -2.6f)),
                        height: 0f,
                        xCount: 6,
                        zCount: 1,
                        yRotation: 0f,
                        xDirection: Positive,
                        zDirection: Negative,
                        fillOrder: XMajor),
                    Grid(
                        Box(
                            Point2(183.1f, -19.4f),
                            Point2(189.3f, -18.7f)),
                        height: 0f,
                        xCount: 6,
                        zCount: 1,
                        yRotation: 180f,
                        xDirection: Positive,
                        zDirection: Negative,
                        fillOrder: XMajor)
                },
                preferenceKey: "BarnMaxEmployees",
                preferenceDescription: "Maximum employees allowed at the Barn.",
                required: true);
        }
    }
}
