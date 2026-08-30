using static ConfigurableMoreEmployees.GridAxisDirection;
using static ConfigurableMoreEmployees.GridFillOrder;
using static ConfigurableMoreEmployees.IdlePointPlacementDsl;

namespace ConfigurableMoreEmployees
{
    internal static class SewerOfficeDefinition
    {
        internal static PropertyDefinition Create()
        {
            return new PropertyDefinition(
                key: "SewerOffice",
                gameObjectName: "Sewer office",
                displayName: "Sewer Office",
                propertyCode: "seweroffice",
                vanillaIdlePointCount: 0,
                defaultMaxEmployees: 0,
                addMannyDialogueChoice: true,
                placementAreas: new[]
                {
                    Grid(
                        Box(
                            Point2(42.1f, 66.2f),
                            Point2(47.4f, 66.8f)),
                        height: -9f,
                        xCount: 5,
                        zCount: 1,
                        yRotation: 180f,
                        xDirection: Negative,
                        zDirection: Positive,
                        fillOrder: XMajor),
                    Grid(
                        Box(
                            Point2(49.9f, 66.2f),
                            Point2(53.6f, 66.8f)),
                        height: -9f,
                        xCount: 5,
                        zCount: 1,
                        yRotation: 180f,
                        xDirection: Positive,
                        zDirection: Positive,
                        fillOrder: XMajor)
                },
                preferenceKey: "SewerMaxEmployees",
                preferenceDescription: "Maximum employees allowed at the Sewer Office.",
                required: true);
        }
    }
}
