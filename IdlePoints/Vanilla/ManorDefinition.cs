using static ConfigurableMoreEmployees.GridAxisDirection;
using static ConfigurableMoreEmployees.GridFillOrder;
using static ConfigurableMoreEmployees.IdlePointPlacementDsl;

namespace ConfigurableMoreEmployees
{
    internal static class ManorDefinition
    {
        internal static PropertyDefinition Create()
        {
            return new PropertyDefinition(
                key: "Manor",
                gameObjectName: "Manor",
                displayName: "Manor",
                propertyCode: "manor",
                vanillaIdlePointCount: 12,
                defaultMaxEmployees: 17,
                addMannyDialogueChoice: false,
                placementAreas: new[]
                {
                    Explicit(
                        Point(167.8f, 10f, -67.1f, 160f),
                        Point(168.6f, 10f, -68.2f, 175f),
                        Point(167.6f, 10f, -69.3f, 190f),
                        Point(168.5f, 10f, -70.3f, 200f),
                        Point(169.6f, 10f, -69.2f, 165f),
                        Point(170.6f, 10f, -69.5f, 185f),
                        Point(169.6f, 10f, -70.3f, 195f),
                        Point(171.8f, 10f, -70.3f, 170f)),
                    Grid(
                        Box(
                            Point2(154f, -65.8f),
                            Point2(159.8f, -65.1f)),
                        height: 10f,
                        xCount: 5,
                        zCount: 1,
                        yRotation: 180f,
                        xDirection: Negative,
                        zDirection: Negative,
                        fillOrder: ZMajor),
                    Grid(
                        Box(
                            Point2(160.6f, -74.3f),
                            Point2(161.3f, -66.1f)),
                        height: 10f,
                        xCount: 1,
                        zCount: 6,
                        yRotation: 90f,
                        xDirection: Negative,
                        zDirection: Positive,
                        fillOrder: XMajor),
                    Grid(
                        Box(
                            Point2(176.4f, -65.2f),
                            Point2(177.1f, -60.1f)),
                        height: 10f,
                        xCount: 1,
                        zCount: 4,
                        yRotation: 90f,
                        xDirection: Negative,
                        zDirection: Negative,
                        fillOrder: XMajor)
                },
                preferenceKey: "ManorMaxEmployees",
                preferenceDescription: "Maximum employees allowed at the Manor.",
                required: true);
        }
    }
}
