using System;
using UnityEngine;

namespace ConfigurableMoreEmployees
{
    internal static class VanillaPropertyDefinitions
    {
        private const float DefaultGridMargin = 0.2f;

        internal static PropertyDefinition[] Create()
        {
            return new[]
            {
                Define(
                    "Barn",
                    "Barn",
                    "Barn",
                    "barn",
                    10,
                    13,
                    false,
                    "BarnMaxEmployees",
                    "Maximum employees allowed at the Barn.",
                    Grid(Bounds(V2(182.3f, -14.4f), V2(182.3f, -12.6f), V2(180.7f, -12.6f), V2(180.7f, -14.4f)), 0.1f, 2, 2, 270f, GridAxisDirection.Negative, GridAxisDirection.Positive, GridFillOrder.ZMajor),
                    Grid(Bounds(V2(180.7f, -14.8f), V2(180.7f, -18.4f), V2(180.1f, -18.4f), V2(180.1f, -14.8f)), 0.1f, 1, 3, 270f, GridAxisDirection.Negative, GridAxisDirection.Negative, GridFillOrder.ZMajor),
                    Grid(Bounds(V2(175.6f, -15.9f), V2(175.6f, -12.7f), V2(176.3f, -12.7f), V2(176.3f, -15.9f)), 0f, 1, 3, 90f, GridAxisDirection.Negative, GridAxisDirection.Positive, GridFillOrder.ZMajor),
                    Grid(Bounds(V2(175.6f, -9.3f), V2(175.6f, -6f), V2(176.3f, -6f), V2(176.3f, -9.3f)), 0f, 1, 3, 90f, GridAxisDirection.Negative, GridAxisDirection.Negative, GridFillOrder.ZMajor),
                    Grid(Bounds(V2(183.1f, -3.3f), V2(183.1f, -2.6f), V2(189.3f, -2.6f), V2(189.3f, -3.3f)), 0f, 6, 1, 0f, GridAxisDirection.Positive, GridAxisDirection.Negative, GridFillOrder.XMajor),
                    Grid(Bounds(V2(183.1f, -18.7f), V2(183.1f, -19.4f), V2(189.3f, -19.4f), V2(189.3f, -18.7f)), 0f, 6, 1, 180f, GridAxisDirection.Positive, GridAxisDirection.Negative, GridFillOrder.XMajor)),
                Define(
                    "Manor",
                    "Manor",
                    "Manor",
                    "manor",
                    12,
                    17,
                    false,
                    "ManorMaxEmployees",
                    "Maximum employees allowed at the Manor.",
                    Explicit(
                        Point(167.8f, 10f, -67.1f, 160f),
                        Point(168.6f, 10f, -68.2f, 175f),
                        Point(167.6f, 10f, -69.3f, 190f),
                        Point(168.5f, 10f, -70.3f, 200f),
                        Point(169.6f, 10f, -69.2f, 165f),
                        Point(170.6f, 10f, -69.5f, 185f),
                        Point(169.6f, 10f, -70.3f, 195f),
                        Point(171.8f, 10f, -70.3f, 170f)),
                    Grid(Bounds(V2(159.8f, -65.1f), V2(154f, -65.1f), V2(154f, -65.8f), V2(159.8f, -65.8f)), 10f, 5, 1, 180f, GridAxisDirection.Negative, GridAxisDirection.Negative, GridFillOrder.ZMajor),
                    Grid(Bounds(V2(160.6f, -74.3f), V2(160.6f, -66.1f), V2(161.3f, -66.1f), V2(161.3f, -74.3f)), 10f, 1, 6, 90f, GridAxisDirection.Negative, GridAxisDirection.Positive, GridFillOrder.XMajor),
                    Grid(Bounds(V2(176.4f, -60.1f), V2(176.4f, -65.2f), V2(177.1f, -65.2f), V2(177.1f, -60.1f)), 10f, 1, 4, 90f, GridAxisDirection.Negative, GridAxisDirection.Negative, GridFillOrder.XMajor)),
                Define(
                    "Bungalow",
                    "Bungalow",
                    "Bungalow",
                    "bungalow",
                    5,
                    7,
                    false,
                    "BungalowMaxEmployees",
                    "Maximum employees allowed at the Bungalow.",
                    Explicit(
                        Point(-167.4f, -4f, 112f, 310f),
                        Point(-165.9f, -4f, 112f, 350f),
                        Point(-164.4f, -4f, 112f, 350f)),
                    Grid(Bounds(V2(-164.8f, 115.2f), V2(-164.8f, 114.6f), V2(-168f, 114.6f), V2(-168f, 115.2f)), -3.7f, 4, 1, 350f, GridAxisDirection.Negative, GridAxisDirection.Positive, GridFillOrder.XMajor),
                    Grid(Bounds(V2(-163.5f, 115.4f), V2(-162.5f, 115.4f), V2(-162.5f, 118.4f), V2(-163.5f, 118.4f)), -3.9f, 1, 3, 260f, GridAxisDirection.Positive, GridAxisDirection.Positive, GridFillOrder.ZMajor),
                    Grid(Bounds(V2(-169.9f, 109f), V2(-169.9f, 107.4f), V2(-173.3f, 107.4f), V2(-173.3f, 109f)), -3.9f, 4, 2, 350f, GridAxisDirection.Negative, GridAxisDirection.Negative, GridFillOrder.XMajor)),
                Define(
                    "DocksWarehouse",
                    "DocksWarehouse",
                    "Docks Warehouse",
                    "dockswarehouse",
                    12,
                    17,
                    false,
                    "DocksWarehouseMaxEmployees",
                    "Maximum employees allowed at the Docks Warehouse.",
                    OrientedGrid(Bounds(V2(-87.0f, -61.8f), V2(-83.3f, -59.9f), V2(-82.1f, -62f), V2(-85.6f, -64.2f)), -2.4f, 4, 3, 60f, OrientedGridColumnDirection.PointBToPointA, OrientedGridRowDirection.PointBToPointC, OrientedGridFillOrder.ColumnMajor),
                    OrientedGrid(Bounds(V2(-78.6f, -57f), V2(-74.6f, -54.7f), V2(-73.2f, -57.1f), V2(-77.2f, -59.4f)), -2.4f, 4, 3, 60f, OrientedGridColumnDirection.PointAToPointB, OrientedGridRowDirection.PointBToPointC, OrientedGridFillOrder.ColumnMajor)),
                Define(
                    "Sweatshop",
                    "Sweatshop",
                    "Sweatshop",
                    "sweatshop",
                    1,
                    3,
                    false,
                    "SweatshopMaxEmployees",
                    "Maximum employees allowed at the Sweatshop.",
                    Explicit(
                        Point(-63.6f, -4f, 143.43f, 330f),
                        Point(-64.6f, -4f, 143.43f, 330f),
                        Point(-62.6f, -4f, 144.43f, 330f),
                        Point(-63.6f, -4f, 144.43f, 330f),
                        Point(-64.6f, -4f, 144.43f, 330f),
                        Point(-67.6f, -4f, 143.93f, 60f),
                        Point(-67.6f, -4f, 142.03f, 60f)),
                    Grid(Bounds(V2(-65.1f, 146.7f), V2(-64.3f, 146.7f), V2(-64.3f, 151f), V2(-65.1f, 151f)), -4f, 1, 5, 60f, GridAxisDirection.Positive, GridAxisDirection.Positive, GridFillOrder.ZMajor)),
                Define(
                    "MotelRoom",
                    "MotelRoom",
                    "Motel Room",
                    "motelroom",
                    0,
                    0,
                    true,
                    "MotelMaxEmployees",
                    "Maximum employees allowed at the Motel.",
                    Explicit(
                        Point(-65.9f, 0f, 80.2f, 30f),
                        Point(-65.2f, 0f, 81.5f, 0f)),
                    Grid(Bounds(V2(-65.5f, 83.8f), V2(-65.5f, 87.6f), V2(-64.8f, 87.6f), V2(-64.8f, 83.8f)), 0.1f, 1, 4, 0f, GridAxisDirection.Negative, GridAxisDirection.Positive, GridFillOrder.ZMajor),
                    Grid(Bounds(V2(-69.3f, 78.9f), V2(-72.8f, 78.9f), V2(-72.8f, 78.2f), V2(-69.3f, 78.2f)), 0f, 3, 1, 90f, GridAxisDirection.Negative, GridAxisDirection.Positive, GridFillOrder.XMajor)),
                Define(
                    "StorageUnit",
                    "StorageUnit",
                    "Storage Unit",
                    "storageunit",
                    3,
                    5,
                    false,
                    "StorageUnitMaxEmployees",
                    "Maximum employees allowed at the Storage Unit.",
                    Explicit(
                        Point(-7.5f, 0.1f, 102.9f, 180f),
                        Point(-6.7f, 0.1f, 102.9f, 180f),
                        Point(-4.5f, 0.1f, 103.2f, 180f),
                        Point(-1.8f, 0.1f, 103.2f, 270f)),
                    Grid(Bounds(V2(-3.8f, 101.3f), V2(-3.8f, 97.6f), V2(-2.2f, 97.6f), V2(-2.2f, 101.3f)), 0.1f, 2, 4, 270f, GridAxisDirection.Positive, GridAxisDirection.Positive, GridFillOrder.ZMajor)),
                Define(
                    "SewerOffice",
                    "Sewer office",
                    "Sewer Office",
                    "seweroffice",
                    0,
                    0,
                    true,
                    "SewerMaxEmployees",
                    "Maximum employees allowed at the Sewer Office.",
                    Grid(Bounds(V2(47.4f, 66.8f), V2(42.1f, 66.8f), V2(42.1f, 66.2f), V2(47.4f, 66.2f)), -9f, 5, 1, 180f, GridAxisDirection.Negative, GridAxisDirection.Positive, GridFillOrder.XMajor),
                    Grid(Bounds(V2(53.6f, 66.8f), V2(49.9f, 66.8f), V2(49.9f, 66.2f), V2(53.6f, 66.2f)), -9f, 5, 1, 180f, GridAxisDirection.Positive, GridAxisDirection.Positive, GridFillOrder.XMajor))
            };
        }

        private static PropertyDefinition Define(
            string key,
            string gameObjectName,
            string displayName,
            string propertyCode,
            int vanillaIdlePointCount,
            int defaultMaxEmployees,
            bool addMannyDialogueChoice,
            string preferenceKey,
            string preferenceDescription,
            params IdlePointPlacementArea[] areas)
        {
            return new PropertyDefinition(
                key,
                gameObjectName,
                displayName,
                propertyCode,
                vanillaIdlePointCount,
                defaultMaxEmployees,
                addMannyDialogueChoice,
                areas,
                preferenceKey,
                preferenceDescription,
                true);
        }

        private static IdlePointPlacement Point(float x, float y, float z, float yRotation)
        {
            return new IdlePointPlacement(new Vector3(x, y, z), yRotation);
        }

        private static Vector2 V2(float x, float z)
        {
            return new Vector2(x, z);
        }

        private static Func<Vector3, Vector2[]> Bounds(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
        {
            return startLocation => new[] { pointA, pointB, pointC, pointD };
        }

        private static IdlePointPlacementArea Explicit(params IdlePointPlacement[] placements)
        {
            return new IdlePointPlacementArea(new ExplicitIdlePointPlacementStrategy(placements));
        }

        private static IdlePointPlacementArea Grid(
            Func<Vector3, Vector2[]> bounds,
            float height,
            int xCount,
            int zCount,
            float yRotation,
            GridAxisDirection xDirection,
            GridAxisDirection zDirection,
            GridFillOrder fillOrder)
        {
            return new IdlePointPlacementArea(
                bounds,
                new GridIdlePointPlacementStrategy(
                    height,
                    xCount,
                    zCount,
                    DefaultGridMargin,
                    yRotation,
                    xDirection,
                    zDirection,
                    fillOrder));
        }

        private static IdlePointPlacementArea OrientedGrid(
            Func<Vector3, Vector2[]> bounds,
            float height,
            int columnCount,
            int rowCount,
            float yRotation,
            OrientedGridColumnDirection columnDirection,
            OrientedGridRowDirection rowDirection,
            OrientedGridFillOrder fillOrder)
        {
            return new IdlePointPlacementArea(
                bounds,
                new OrientedGridIdlePointPlacementStrategy(
                    height,
                    columnCount,
                    rowCount,
                    DefaultGridMargin,
                    yRotation,
                    columnDirection,
                    rowDirection,
                    fillOrder));
        }
    }
}
