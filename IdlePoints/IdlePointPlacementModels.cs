using System;
using System.Linq;
using UnityEngine;

namespace ConfigurableMoreEmployees
{
    /// <summary>
    /// Position and yaw rotation for a generated employee idle point.
    /// </summary>
    public readonly struct IdlePointPlacement
    {
        /// <summary>
        /// Creates a generated idle point placement.
        /// </summary>
        /// <param name="position">World position for the idle point.</param>
        /// <param name="yRotation">Yaw rotation in degrees.</param>
        public IdlePointPlacement(Vector3 position, float yRotation)
        {
            Position = position;
            YRotation = yRotation;
        }

        /// <summary>
        /// World position for the idle point.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Yaw rotation in degrees.
        /// </summary>
        public float YRotation { get; }
    }

    /// <summary>
    /// Direction used when filling an axis-aligned placement grid.
    /// </summary>
    public enum GridAxisDirection
    {
        /// <summary>
        /// Fill from the high coordinate toward the low coordinate.
        /// </summary>
        Negative = -1,

        /// <summary>
        /// Fill from the low coordinate toward the high coordinate.
        /// </summary>
        Positive = 1
    }

    /// <summary>
    /// Order used when filling an axis-aligned placement grid.
    /// </summary>
    public enum GridFillOrder
    {
        /// <summary>
        /// Advance along X before moving to the next Z row.
        /// </summary>
        XMajor,

        /// <summary>
        /// Advance along Z before moving to the next X column.
        /// </summary>
        ZMajor
    }

    /// <summary>
    /// Direction used for the column axis of a four-point oriented grid.
    /// </summary>
    public enum OrientedGridColumnDirection
    {
        PointAToPointB,
        PointBToPointA,
        PointCToPointD,
        PointDToPointC
    }

    /// <summary>
    /// Direction used for the row axis of a four-point oriented grid.
    /// </summary>
    public enum OrientedGridRowDirection
    {
        PointAToPointD,
        PointDToPointA,
        PointBToPointC,
        PointCToPointB
    }

    /// <summary>
    /// Order used when filling an oriented placement grid.
    /// </summary>
    public enum OrientedGridFillOrder
    {
        /// <summary>
        /// Advance along columns before moving to the next row.
        /// </summary>
        ColumnMajor,

        /// <summary>
        /// Advance along rows before moving to the next column.
        /// </summary>
        RowMajor
    }

    /// <summary>
    /// Base type for placement strategies that produce generated idle point positions.
    /// </summary>
    public abstract class IdlePointPlacementStrategy
    {
        /// <summary>
        /// Gets whether this strategy needs bounds from its placement area.
        /// </summary>
        public virtual bool RequiresBounds => false;

        /// <summary>
        /// Gets the number of placement attempts this strategy can provide.
        /// </summary>
        /// <param name="ruleMaxAttempts">The maximum attempts requested by the placement rule.</param>
        /// <returns>The number of placements available from this strategy.</returns>
        public abstract int GetAttemptCount(int ruleMaxAttempts);

        /// <summary>
        /// Gets a placement for the requested index.
        /// </summary>
        /// <param name="startLocation">The original idle point position used as a fallback reference.</param>
        /// <param name="index">Zero-based placement index.</param>
        /// <returns>The generated idle point placement.</returns>
        public virtual IdlePointPlacement GetPlacement(Vector3 startLocation, int index)
        {
            throw new NotSupportedException($"{GetType().Name} requires placement bounds.");
        }

        /// <summary>
        /// Gets a placement for the requested index using bounds supplied by the placement area.
        /// </summary>
        /// <param name="startLocation">The original idle point position used as a fallback reference.</param>
        /// <param name="bounds">The four bounds points supplied by the placement area.</param>
        /// <param name="index">Zero-based placement index.</param>
        /// <returns>The generated idle point placement.</returns>
        public virtual IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index)
        {
            return GetPlacement(startLocation, index);
        }
    }

    /// <summary>
    /// Places idle points in an axis-aligned grid inside a bounds rectangle.
    /// </summary>
    public sealed class GridIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
        /// <summary>
        /// Creates an axis-aligned grid placement strategy.
        /// </summary>
        /// <param name="height">Optional fixed world Y height. When null, the source idle point height is reused.</param>
        /// <param name="xCount">Number of grid positions on the X axis.</param>
        /// <param name="zCount">Number of grid positions on the Z axis.</param>
        /// <param name="margin">Inset applied to each side of the bounds before placing points.</param>
        /// <param name="yRotation">Yaw rotation in degrees for generated points.</param>
        /// <param name="xDirection">Direction used to fill the X axis.</param>
        /// <param name="zDirection">Direction used to fill the Z axis.</param>
        /// <param name="fillOrder">Whether X or Z advances first.</param>
        public GridIdlePointPlacementStrategy(
            float? height,
            int xCount,
            int zCount,
            float margin,
            float yRotation,
            GridAxisDirection xDirection,
            GridAxisDirection zDirection,
            GridFillOrder fillOrder)
        {
            Height = height;
            XCount = xCount;
            ZCount = zCount;
            Margin = margin;
            YRotation = yRotation;
            XDirection = xDirection;
            ZDirection = zDirection;
            FillOrder = fillOrder;
        }

        /// <summary>
        /// Optional fixed world Y height. When null, the source idle point height is reused.
        /// </summary>
        public float? Height { get; }

        /// <summary>
        /// Number of grid positions on the X axis.
        /// </summary>
        public int XCount { get; }

        /// <summary>
        /// Number of grid positions on the Z axis.
        /// </summary>
        public int ZCount { get; }

        /// <summary>
        /// Inset applied to each side of the bounds before placing points.
        /// </summary>
        public float Margin { get; }

        /// <summary>
        /// Yaw rotation in degrees for generated points.
        /// </summary>
        public float YRotation { get; }

        /// <summary>
        /// Direction used to fill the X axis.
        /// </summary>
        public GridAxisDirection XDirection { get; }

        /// <summary>
        /// Direction used to fill the Z axis.
        /// </summary>
        public GridAxisDirection ZDirection { get; }

        /// <summary>
        /// Whether X or Z advances first.
        /// </summary>
        public GridFillOrder FillOrder { get; }

        public override bool RequiresBounds => true;

        public override int GetAttemptCount(int ruleMaxAttempts)
        {
            return XCount * ZCount;
        }

        public override IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index)
        {
            var minX = bounds.Min(point => point.x);
            var maxX = bounds.Max(point => point.x);
            var minZ = bounds.Min(point => point.y);
            var maxZ = bounds.Max(point => point.y);
            var usableMinX = minX + Margin;
            var usableMaxX = maxX - Margin;
            var usableMinZ = minZ + Margin;
            var usableMaxZ = maxZ - Margin;

            var xIndex = FillOrder == GridFillOrder.XMajor ? index % XCount : index / ZCount;
            var zIndex = FillOrder == GridFillOrder.XMajor ? index / XCount : index % ZCount;
            var xStep = XCount <= 1 ? 0f : (usableMaxX - usableMinX) / (XCount - 1);
            var zStep = ZCount <= 1 ? 0f : (usableMaxZ - usableMinZ) / (ZCount - 1);
            var xStart = XDirection == GridAxisDirection.Positive ? usableMinX : usableMaxX;
            var zStart = ZDirection == GridAxisDirection.Positive ? usableMinZ : usableMaxZ;
            var x = XCount <= 1 ? (usableMinX + usableMaxX) / 2f : xStart + xStep * xIndex * (int)XDirection;
            var z = ZCount <= 1 ? (usableMinZ + usableMaxZ) / 2f : zStart + zStep * zIndex * (int)ZDirection;

            return new IdlePointPlacement(
                new Vector3(x, Height ?? startLocation.y, z),
                YRotation);
        }
    }

    /// <summary>
    /// Places idle points in a grid whose axes are derived from four supplied bounds points.
    /// </summary>
    public sealed class OrientedGridIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
        /// <summary>
        /// Creates an oriented grid placement strategy.
        /// </summary>
        /// <param name="height">Optional fixed world Y height. When null, the source idle point height is reused.</param>
        /// <param name="columnCount">Number of positions on the column axis.</param>
        /// <param name="rowCount">Number of positions on the row axis.</param>
        /// <param name="margin">Inset applied to each axis before placing points.</param>
        /// <param name="yRotation">Yaw rotation in degrees for generated points.</param>
        /// <param name="columnDirection">Which bounds edge and direction define the column axis.</param>
        /// <param name="rowDirection">Which bounds edge and direction define the row axis.</param>
        /// <param name="fillOrder">Whether columns or rows advance first.</param>
        public OrientedGridIdlePointPlacementStrategy(
            float? height,
            int columnCount,
            int rowCount,
            float margin,
            float yRotation,
            OrientedGridColumnDirection columnDirection,
            OrientedGridRowDirection rowDirection,
            OrientedGridFillOrder fillOrder)
        {
            Height = height;
            ColumnCount = columnCount;
            RowCount = rowCount;
            Margin = margin;
            YRotation = yRotation;
            ColumnDirection = columnDirection;
            RowDirection = rowDirection;
            FillOrder = fillOrder;
        }

        /// <summary>
        /// Optional fixed world Y height. When null, the source idle point height is reused.
        /// </summary>
        public float? Height { get; }

        /// <summary>
        /// Number of positions on the column axis.
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// Number of positions on the row axis.
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// Inset applied to each axis before placing points.
        /// </summary>
        public float Margin { get; }

        /// <summary>
        /// Yaw rotation in degrees for generated points.
        /// </summary>
        public float YRotation { get; }

        /// <summary>
        /// Which bounds edge and direction define the column axis.
        /// </summary>
        public OrientedGridColumnDirection ColumnDirection { get; }

        /// <summary>
        /// Which bounds edge and direction define the row axis.
        /// </summary>
        public OrientedGridRowDirection RowDirection { get; }

        /// <summary>
        /// Whether columns or rows advance first.
        /// </summary>
        public OrientedGridFillOrder FillOrder { get; }

        public override bool RequiresBounds => true;

        public override int GetAttemptCount(int ruleMaxAttempts)
        {
            return ColumnCount * RowCount;
        }

        public override IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index)
        {
            GetColumnEndpoints(bounds, out var columnStart, out var columnEnd);
            GetRowEndpoints(bounds, out var rowStart, out var rowEnd);

            var columnIndex = FillOrder == OrientedGridFillOrder.ColumnMajor ? index % ColumnCount : index / RowCount;
            var rowIndex = FillOrder == OrientedGridFillOrder.ColumnMajor ? index / ColumnCount : index % RowCount;
            var columnOffset = GetAxisOffset((columnEnd - columnStart).magnitude, ColumnCount, columnIndex);
            var rowOffset = GetAxisOffset((rowEnd - rowStart).magnitude, RowCount, rowIndex);
            var columnUnit = (columnEnd - columnStart).normalized;
            var rowUnit = (rowEnd - rowStart).normalized;
            var position = columnStart + columnUnit * columnOffset + rowUnit * rowOffset;

            return new IdlePointPlacement(
                new Vector3(position.x, Height ?? startLocation.y, position.y),
                YRotation);
        }

        private float GetAxisOffset(float length, int count, int index)
        {
            if (count <= 1)
            {
                return length / 2f;
            }

            var usableLength = Mathf.Max(0f, length - Margin * 2f);
            return Margin + usableLength / (count - 1) * index;
        }

        private void GetColumnEndpoints(Vector2[] bounds, out Vector2 start, out Vector2 end)
        {
            switch (ColumnDirection)
            {
                case OrientedGridColumnDirection.PointBToPointA:
                    start = bounds[1];
                    end = bounds[0];
                    return;
                case OrientedGridColumnDirection.PointCToPointD:
                    start = bounds[2];
                    end = bounds[3];
                    return;
                case OrientedGridColumnDirection.PointDToPointC:
                    start = bounds[3];
                    end = bounds[2];
                    return;
                default:
                    start = bounds[0];
                    end = bounds[1];
                    return;
            }
        }

        private void GetRowEndpoints(Vector2[] bounds, out Vector2 start, out Vector2 end)
        {
            switch (RowDirection)
            {
                case OrientedGridRowDirection.PointDToPointA:
                    start = bounds[3];
                    end = bounds[0];
                    return;
                case OrientedGridRowDirection.PointBToPointC:
                    start = bounds[1];
                    end = bounds[2];
                    return;
                case OrientedGridRowDirection.PointCToPointB:
                    start = bounds[2];
                    end = bounds[1];
                    return;
                default:
                    start = bounds[0];
                    end = bounds[3];
                    return;
            }
        }
    }

    /// <summary>
    /// Places idle points from an explicit list of world positions.
    /// </summary>
    public sealed class ExplicitIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
        /// <summary>
        /// Creates an explicit placement strategy.
        /// </summary>
        /// <param name="placements">The ordered placements to return.</param>
        public ExplicitIdlePointPlacementStrategy(IdlePointPlacement[] placements)
        {
            Placements = placements ?? Array.Empty<IdlePointPlacement>();
        }

        /// <summary>
        /// The ordered placements to return.
        /// </summary>
        public IdlePointPlacement[] Placements { get; }

        public override int GetAttemptCount(int ruleMaxAttempts)
        {
            return Placements.Length;
        }

        public override IdlePointPlacement GetPlacement(Vector3 startLocation, int index)
        {
            return Placements[index];
        }
    }

    /// <summary>
    /// Defines an area and strategy for generating extra idle points.
    /// </summary>
    public sealed class IdlePointPlacementArea
    {
        /// <summary>
        /// Creates an unbounded idle point placement area.
        /// </summary>
        /// <param name="strategy">Strategy that generates placements without bounds.</param>
        public IdlePointPlacementArea(IdlePointPlacementStrategy strategy)
            : this(null, strategy, false)
        {
        }

        /// <summary>
        /// Creates a bounded idle point placement area.
        /// </summary>
        /// <param name="boundsProvider">Returns four bounds points for the area, using the source idle point as context.</param>
        /// <param name="strategy">Strategy that generates placements inside or from the area.</param>
        /// <param name="validateBounds">Whether generated points should be checked against the area bounds.</param>
        public IdlePointPlacementArea(
            Func<Vector3, Vector2[]> boundsProvider,
            IdlePointPlacementStrategy strategy,
            bool validateBounds = true)
        {
            BoundsProvider = boundsProvider;
            Strategy = strategy;
            ValidateBounds = validateBounds;
        }

        /// <summary>
        /// Returns four bounds points for the area, using the source idle point as context.
        /// </summary>
        public Func<Vector3, Vector2[]> BoundsProvider { get; }

        /// <summary>
        /// Strategy that generates placements inside or from the area.
        /// </summary>
        public IdlePointPlacementStrategy Strategy { get; }

        /// <summary>
        /// Whether generated points should be checked against the area bounds.
        /// </summary>
        public bool ValidateBounds { get; }
    }
}
