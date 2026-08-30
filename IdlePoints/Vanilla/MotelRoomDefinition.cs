using static ConfigurableMoreEmployees.GridAxisDirection;
using static ConfigurableMoreEmployees.GridFillOrder;
using static ConfigurableMoreEmployees.IdlePointPlacementDsl;

namespace ConfigurableMoreEmployees
{
    internal static class MotelRoomDefinition
    {
        internal static PropertyDefinition Create()
        {
            return new PropertyDefinition(
                key: "MotelRoom",
                gameObjectName: "MotelRoom",
                displayName: "Motel Room",
                propertyCode: "motelroom",
                vanillaIdlePointCount: 0,
                defaultMaxEmployees: 0,
                addMannyDialogueChoice: true,
                placementAreas: new[]
                {
                    Explicit(
                        Point(-65.9f, 0f, 80.2f, 30f),
                        Point(-65.2f, 0f, 81.5f, 0f)),
                    Grid(
                        Box(
                            Point2(-65.5f, 83.8f),
                            Point2(-64.8f, 87.6f)),
                        height: 0.1f,
                        xCount: 1,
                        zCount: 4,
                        yRotation: 0f,
                        xDirection: Negative,
                        zDirection: Positive,
                        fillOrder: ZMajor),
                    Grid(
                        Box(
                            Point2(-72.8f, 78.2f),
                            Point2(-69.3f, 78.9f)),
                        height: 0f,
                        xCount: 3,
                        zCount: 1,
                        yRotation: 90f,
                        xDirection: Negative,
                        zDirection: Positive,
                        fillOrder: XMajor)
                },
                preferenceKey: "MotelMaxEmployees",
                preferenceDescription: "Maximum employees allowed at the Motel.",
                required: true);
        }
    }
}
