using static ConfigurableMoreEmployees.IdlePointPlacementDsl;
using static ConfigurableMoreEmployees.OrientedGridColumnDirection;
using static ConfigurableMoreEmployees.OrientedGridFillOrder;
using static ConfigurableMoreEmployees.OrientedGridRowDirection;

namespace ConfigurableMoreEmployees
{
    internal static class DocksWarehouseDefinition
    {
        internal static PropertyDefinition Create()
        {
            return new PropertyDefinition(
                key: "DocksWarehouse",
                gameObjectName: "DocksWarehouse",
                displayName: "Docks Warehouse",
                propertyCode: "dockswarehouse",
                vanillaIdlePointCount: 12,
                defaultMaxEmployees: 17,
                addMannyDialogueChoice: false,
                placementAreas: new[]
                {
                    OrientedGrid(
                        OrientedBox(
                            pointA: Point2(-87.0f, -61.8f),
                            pointB: Point2(-83.3f, -59.9f),
                            pointC: Point2(-82.1f, -62f)),
                        height: -2.4f,
                        columnCount: 4,
                        rowCount: 3,
                        yRotation: 60f,
                        columnDirection: PointBToPointA,
                        rowDirection: PointBToPointC,
                        fillOrder: ColumnMajor),
                    OrientedGrid(
                        OrientedBox(
                            pointA: Point2(-78.6f, -57f),
                            pointB: Point2(-74.6f, -54.7f),
                            pointC: Point2(-73.2f, -57.1f)),
                        height: -2.4f,
                        columnCount: 4,
                        rowCount: 3,
                        yRotation: 60f,
                        columnDirection: PointAToPointB,
                        rowDirection: PointBToPointC,
                        fillOrder: ColumnMajor)
                },
                preferenceKey: "DocksWarehouseMaxEmployees",
                preferenceDescription: "Maximum employees allowed at the Docks Warehouse.",
                required: true);
        }
    }
}
