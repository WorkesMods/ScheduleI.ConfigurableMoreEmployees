using UnityEngine;

namespace ConfigurableMoreEmployees
{
    /// <summary>
    /// Convenience helpers for authoring idle point placement definitions.
    /// </summary>
    public static class IdlePointPlacementDsl
    {
        /// <summary>
        /// Creates an idle point placement from world coordinates and yaw rotation.
        /// </summary>
        public static IdlePointPlacement Point(float x, float y, float z, float yRotation)
        {
            return new IdlePointPlacement(new Vector3(x, y, z), yRotation);
        }

        /// <summary>
        /// Creates a two-dimensional X/Z point.
        /// </summary>
        public static Vector2 Point2(float x, float z)
        {
            return new Vector2(x, z);
        }

        /// <summary>
        /// Creates axis-aligned bounds from two opposite corners.
        /// </summary>
        public static IdlePointBounds Box(Vector2 pointA, Vector2 pointC)
        {
            return new AxisAlignedIdlePointBounds(pointA, pointC);
        }

        /// <summary>
        /// Creates oriented bounds from three corners. Corner D is derived as A + (C - B).
        /// </summary>
        public static IdlePointBounds OrientedBox(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            return new OrientedIdlePointBounds(pointA, pointB, pointC);
        }

        /// <summary>
        /// Creates an unbounded placement area from explicit world positions.
        /// </summary>
        public static IdlePointPlacementArea Explicit(params IdlePointPlacement[] placements)
        {
            return new IdlePointPlacementArea(new ExplicitIdlePointPlacementStrategy(placements));
        }

        /// <summary>
        /// Creates an axis-aligned grid placement area.
        /// </summary>
        public static IdlePointPlacementArea Grid(
            IdlePointBounds bounds,
            float? height,
            int xCount,
            int zCount,
            float yRotation,
            GridAxisDirection xDirection = GridAxisDirection.Positive,
            GridAxisDirection zDirection = GridAxisDirection.Positive,
            GridFillOrder fillOrder = GridFillOrder.XMajor,
            float margin = 0.2f)
        {
            return new IdlePointPlacementArea(
                bounds,
                new GridIdlePointPlacementStrategy(
                    height,
                    xCount,
                    zCount,
                    margin,
                    yRotation,
                    xDirection,
                    zDirection,
                    fillOrder));
        }

        /// <summary>
        /// Creates an oriented grid placement area.
        /// </summary>
        public static IdlePointPlacementArea OrientedGrid(
            IdlePointBounds bounds,
            float? height,
            int columnCount,
            int rowCount,
            float yRotation,
            OrientedGridColumnDirection columnDirection,
            OrientedGridRowDirection rowDirection,
            OrientedGridFillOrder fillOrder = OrientedGridFillOrder.ColumnMajor,
            float margin = 0.2f)
        {
            return new IdlePointPlacementArea(
                bounds,
                new OrientedGridIdlePointPlacementStrategy(
                    height,
                    columnCount,
                    rowCount,
                    margin,
                    yRotation,
                    columnDirection,
                    rowDirection,
                    fillOrder));
        }
    }
}
