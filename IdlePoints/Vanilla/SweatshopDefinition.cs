using static ConfigurableMoreEmployees.GridAxisDirection;
using static ConfigurableMoreEmployees.GridFillOrder;
using static ConfigurableMoreEmployees.IdlePointPlacementDsl;

namespace ConfigurableMoreEmployees
{
    internal static class SweatshopDefinition
    {
        internal static PropertyDefinition Create()
        {
            return new PropertyDefinition(
                key: "Sweatshop",
                gameObjectName: "Sweatshop",
                displayName: "Sweatshop",
                propertyCode: "sweatshop",
                vanillaIdlePointCount: 1,
                defaultMaxEmployees: 3,
                addMannyDialogueChoice: false,
                placementAreas: new[]
                {
                    Explicit(
                        Point(-63.6f, -4f, 143.43f, 330f),
                        Point(-64.6f, -4f, 143.43f, 330f),
                        Point(-62.6f, -4f, 144.43f, 330f),
                        Point(-63.6f, -4f, 144.43f, 330f),
                        Point(-64.6f, -4f, 144.43f, 330f),
                        Point(-67.6f, -4f, 143.93f, 60f),
                        Point(-67.6f, -4f, 142.03f, 60f)),
                    Grid(
                        Box(
                            Point2(-65.1f, 146.7f),
                            Point2(-64.3f, 151f)),
                        height: -4f,
                        xCount: 1,
                        zCount: 5,
                        yRotation: 60f,
                        xDirection: Positive,
                        zDirection: Positive,
                        fillOrder: ZMajor)
                },
                preferenceKey: "SweatshopMaxEmployees",
                preferenceDescription: "Maximum employees allowed at the Sweatshop.",
                required: true);
        }
    }
}
