using System;
using System.Linq;
using UnityEngine;

namespace ConfigurableMoreEmployees
{
    public readonly struct IdlePointPlacement
    {
        public IdlePointPlacement(Vector3 position, float yRotation)
        {
            Position = position;
            YRotation = yRotation;
        }

        public Vector3 Position { get; }
        public float YRotation { get; }
    }

    public enum GridAxisDirection
    {
        Negative = -1,
        Positive = 1
    }

    public enum GridFillOrder
    {
        XMajor,
        ZMajor
    }

    public enum OrientedGridColumnDirection
    {
        PointAToPointB,
        PointBToPointA,
        PointCToPointD,
        PointDToPointC
    }

    public enum OrientedGridRowDirection
    {
        PointAToPointD,
        PointDToPointA,
        PointBToPointC,
        PointCToPointB
    }

    public enum OrientedGridFillOrder
    {
        ColumnMajor,
        RowMajor
    }

    public abstract class IdlePointPlacementStrategy
    {
        public abstract int GetAttemptCount(int ruleMaxAttempts);
        public abstract IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index);
    }

    public sealed class GridIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
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

        public float? Height { get; }
        public int XCount { get; }
        public int ZCount { get; }
        public float Margin { get; }
        public float YRotation { get; }
        public GridAxisDirection XDirection { get; }
        public GridAxisDirection ZDirection { get; }
        public GridFillOrder FillOrder { get; }

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

    public sealed class OrientedGridIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
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

        public float? Height { get; }
        public int ColumnCount { get; }
        public int RowCount { get; }
        public float Margin { get; }
        public float YRotation { get; }
        public OrientedGridColumnDirection ColumnDirection { get; }
        public OrientedGridRowDirection RowDirection { get; }
        public OrientedGridFillOrder FillOrder { get; }

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

    public sealed class ExplicitIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
        public ExplicitIdlePointPlacementStrategy(IdlePointPlacement[] placements)
        {
            Placements = placements ?? Array.Empty<IdlePointPlacement>();
        }

        public IdlePointPlacement[] Placements { get; }

        public override int GetAttemptCount(int ruleMaxAttempts)
        {
            return Placements.Length;
        }

        public override IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index)
        {
            return Placements[index];
        }
    }

    public sealed class IdlePointPlacementArea
    {
        public IdlePointPlacementArea(
            Func<Vector3, Vector2[]> boundsProvider,
            IdlePointPlacementStrategy strategy,
            bool validateBounds = true)
        {
            BoundsProvider = boundsProvider;
            Strategy = strategy;
            ValidateBounds = validateBounds;
        }

        public Func<Vector3, Vector2[]> BoundsProvider { get; }
        public IdlePointPlacementStrategy Strategy { get; }
        public bool ValidateBounds { get; }
    }
}
