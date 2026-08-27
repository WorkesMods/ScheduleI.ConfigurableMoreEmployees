using System;
using System.Linq;
using UnityEngine;

namespace ConfigurableMoreEmployees
{
    internal delegate IdlePointPlacement IdlePointAdvancementStrategy(Vector3 startLocation, int index);

    internal readonly struct IdlePointPlacement
    {
        internal IdlePointPlacement(Vector3 position, float yRotation)
        {
            Position = position;
            YRotation = yRotation;
        }

        internal Vector3 Position { get; }
        internal float YRotation { get; }
    }

    internal enum GridAxisDirection
    {
        Negative = -1,
        Positive = 1
    }

    internal enum GridFillOrder
    {
        XMajor,
        ZMajor
    }

    internal enum OrientedGridColumnDirection
    {
        PointAToPointB,
        PointBToPointA,
        PointCToPointD,
        PointDToPointC
    }

    internal enum OrientedGridRowDirection
    {
        PointAToPointD,
        PointDToPointA,
        PointBToPointC,
        PointCToPointB
    }

    internal enum OrientedGridFillOrder
    {
        ColumnMajor,
        RowMajor
    }

    internal abstract class IdlePointPlacementStrategy
    {
        internal abstract int GetAttemptCount(int ruleMaxAttempts);
        internal abstract IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index);
    }

    internal sealed class GridIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
        internal GridIdlePointPlacementStrategy(
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

        internal float? Height { get; }
        internal int XCount { get; }
        internal int ZCount { get; }
        internal float Margin { get; }
        internal float YRotation { get; }
        internal GridAxisDirection XDirection { get; }
        internal GridAxisDirection ZDirection { get; }
        internal GridFillOrder FillOrder { get; }

        internal override int GetAttemptCount(int ruleMaxAttempts)
        {
            return XCount * ZCount;
        }

        internal override IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index)
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

    internal sealed class OrientedGridIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
        internal OrientedGridIdlePointPlacementStrategy(
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

        internal float? Height { get; }
        internal int ColumnCount { get; }
        internal int RowCount { get; }
        internal float Margin { get; }
        internal float YRotation { get; }
        internal OrientedGridColumnDirection ColumnDirection { get; }
        internal OrientedGridRowDirection RowDirection { get; }
        internal OrientedGridFillOrder FillOrder { get; }

        internal override int GetAttemptCount(int ruleMaxAttempts)
        {
            return ColumnCount * RowCount;
        }

        internal override IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index)
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

    internal sealed class CustomIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
        internal CustomIdlePointPlacementStrategy(IdlePointAdvancementStrategy advancementStrategy)
        {
            AdvancementStrategy = advancementStrategy;
        }

        internal IdlePointAdvancementStrategy AdvancementStrategy { get; }

        internal override int GetAttemptCount(int ruleMaxAttempts)
        {
            return ruleMaxAttempts;
        }

        internal override IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index)
        {
            return AdvancementStrategy(startLocation, index);
        }
    }

    internal sealed class ExplicitIdlePointPlacementStrategy : IdlePointPlacementStrategy
    {
        internal ExplicitIdlePointPlacementStrategy(IdlePointPlacement[] placements)
        {
            Placements = placements;
        }

        internal IdlePointPlacement[] Placements { get; }

        internal override int GetAttemptCount(int ruleMaxAttempts)
        {
            return Placements.Length;
        }

        internal override IdlePointPlacement GetPlacement(Vector3 startLocation, Vector2[] bounds, int index)
        {
            return Placements[index];
        }
    }

    internal sealed class IdlePointPlacementArea
    {
        internal IdlePointPlacementArea(
            Func<PropertyHandler, Vector3> startLocationProvider,
            Func<Vector3, Vector2[]> boundsProvider,
            IdlePointPlacementStrategy strategy,
            bool validateBounds = true)
        {
            StartLocationProvider = startLocationProvider;
            BoundsProvider = boundsProvider;
            Strategy = strategy;
            ValidateBounds = validateBounds;
        }

        internal Func<PropertyHandler, Vector3> StartLocationProvider { get; }
        internal Func<Vector3, Vector2[]> BoundsProvider { get; }
        internal IdlePointPlacementStrategy Strategy { get; }
        internal bool ValidateBounds { get; }
    }

    internal sealed class IdlePointPlacementRule
    {
        internal IdlePointPlacementRule(
            PropertyBindingKey key,
            int vanillaIdlePointCount,
            int maxAttempts,
            IdlePointPlacementArea[] areas)
        {
            Key = key;
            VanillaIdlePointCount = vanillaIdlePointCount;
            MaxAttempts = maxAttempts;
            Areas = areas;
        }

        internal PropertyBindingKey Key { get; }
        internal int VanillaIdlePointCount { get; }
        internal int MaxAttempts { get; }
        internal IdlePointPlacementArea[] Areas { get; }
    }

    internal static class IdlePointPlacementConstants
    {
        private const int DefaultMaxAttempts = 128;
        private const float DefaultBoundsHalfSize = 6f;
        private const float DefaultGridMargin = 0.2f;
        private const int DefaultGridXCount = 4;
        private const int DefaultGridZCount = 32;

        internal static IdlePointPlacementRule GetRule(PropertyBindingKey key)
        {
            switch (key)
            {
                case PropertyBindingKey.StorageUnit:
                    return CreateStorageUnitRule();
                case PropertyBindingKey.Sweatshop:
                    return CreateSweatshopRule();
                case PropertyBindingKey.Bungalow:
                    return CreateBungalowRule();
                case PropertyBindingKey.Barn:
                    return CreateBarnRule();
                case PropertyBindingKey.Manor:
                    return CreateManorRule();
                case PropertyBindingKey.DocksWarehouse:
                    return CreateDocksWarehouseRule();
                case PropertyBindingKey.MotelRoom:
                    return CreateMotelRoomRule();
                case PropertyBindingKey.SewerOffice:
                    return CreateSewerOfficeRule();
                default:
                    return CreateDefaultRule(key, 0);
            }
        }

        private static IdlePointPlacementRule CreateStorageUnitRule()
        {
            return new IdlePointPlacementRule(
                PropertyBindingKey.StorageUnit,
                3,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        CreateDefaultBounds,
                        new ExplicitIdlePointPlacementStrategy(new[]
                        {
                            new IdlePointPlacement(new Vector3(-7.5f, 0.1f, 102.9f), 180f),
                            new IdlePointPlacement(new Vector3(-6.7f, 0.1f, 102.9f), 180f),
                            new IdlePointPlacement(new Vector3(-4.5f, 0.1f, 103.2f), 180f),
                            new IdlePointPlacement(new Vector3(-1.8f, 0.1f, 103.2f), 270f)
                        }),
                        false),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(-3.8f, 101.3f),
                            new Vector2(-3.8f, 97.6f),
                            new Vector2(-2.2f, 97.6f),
                            new Vector2(-2.2f, 101.3f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0.1f,
                            2,
                            4,
                            DefaultGridMargin,
                            270f,
                            GridAxisDirection.Positive,
                            GridAxisDirection.Positive,
                            GridFillOrder.ZMajor))
                });
        }

        private static IdlePointPlacementRule CreateSweatshopRule()
        {
            return new IdlePointPlacementRule(
                PropertyBindingKey.Sweatshop,
                1,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        CreateDefaultBounds,
                        new ExplicitIdlePointPlacementStrategy(new[]
                        {
                            new IdlePointPlacement(new Vector3(-63.6f, -4f, 143.43f), 330f),
                            new IdlePointPlacement(new Vector3(-64.6f, -4f, 143.43f), 330f),
                            new IdlePointPlacement(new Vector3(-62.6f, -4f, 144.43f), 330f),
                            new IdlePointPlacement(new Vector3(-63.6f, -4f, 144.43f), 330f),
                            new IdlePointPlacement(new Vector3(-64.6f, -4f, 144.43f), 330f),
                            new IdlePointPlacement(new Vector3(-67.6f, -4f, 143.93f), 60f),
                            new IdlePointPlacement(new Vector3(-67.6f, -4f, 142.03f), 60f)
                        }),
                        false),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(-65.1f, 146.7f),
                            new Vector2(-64.3f, 146.7f),
                            new Vector2(-64.3f, 151f),
                            new Vector2(-65.1f, 151f)
                        },
                        new GridIdlePointPlacementStrategy(
                            -4f,
                            1,
                            5,
                            DefaultGridMargin,
                            60f,
                            GridAxisDirection.Positive,
                            GridAxisDirection.Positive,
                            GridFillOrder.ZMajor))
                });
        }

        private static IdlePointPlacementRule CreateBungalowRule()
        {
            return new IdlePointPlacementRule(
                PropertyBindingKey.Bungalow,
                5,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        CreateDefaultBounds,
                        new ExplicitIdlePointPlacementStrategy(new[]
                        {
                            new IdlePointPlacement(new Vector3(-167.4f, -4f, 112f), 310f),
                            new IdlePointPlacement(new Vector3(-165.9f, -4f, 112f), 350f),
                            new IdlePointPlacement(new Vector3(-164.4f, -4f, 112f), 350f)
                        }),
                        false),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(-164.8f, 115.2f),
                            new Vector2(-164.8f, 114.6f),
                            new Vector2(-168f, 114.6f),
                            new Vector2(-168f, 115.2f)
                        },
                        new GridIdlePointPlacementStrategy(
                            -3.7f,
                            4,
                            1,
                            DefaultGridMargin,
                            350f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Positive,
                            GridFillOrder.XMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(-163.5f, 115.4f),
                            new Vector2(-162.5f, 115.4f),
                            new Vector2(-162.5f, 118.4f),
                            new Vector2(-163.5f, 118.4f)
                        },
                        new GridIdlePointPlacementStrategy(
                            -3.9f,
                            1,
                            3,
                            DefaultGridMargin,
                            260f,
                            GridAxisDirection.Positive,
                            GridAxisDirection.Positive,
                            GridFillOrder.ZMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(-169.9f, 109f),
                            new Vector2(-169.9f, 107.4f),
                            new Vector2(-173.3f, 107.4f),
                            new Vector2(-173.3f, 109f)
                        },
                        new GridIdlePointPlacementStrategy(
                            -3.9f,
                            4,
                            2,
                            DefaultGridMargin,
                            350f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Negative,
                            GridFillOrder.XMajor))
                });
        }

        private static IdlePointPlacementRule CreateBarnRule()
        {
            return new IdlePointPlacementRule(
                PropertyBindingKey.Barn,
                10,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(182.3f, -14.4f),
                            new Vector2(182.3f, -12.6f),
                            new Vector2(180.7f, -12.6f),
                            new Vector2(180.7f, -14.4f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0.1f,
                            2,
                            2,
                            DefaultGridMargin,
                            270f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Positive,
                            GridFillOrder.ZMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(180.7f, -14.8f),
                            new Vector2(180.7f, -18.4f),
                            new Vector2(180.1f, -18.4f),
                            new Vector2(180.1f, -14.8f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0.1f,
                            1,
                            3,
                            DefaultGridMargin,
                            270f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Negative,
                            GridFillOrder.ZMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(175.6f, -15.9f),
                            new Vector2(175.6f, -12.7f),
                            new Vector2(176.3f, -12.7f),
                            new Vector2(176.3f, -15.9f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0f,
                            1,
                            3,
                            DefaultGridMargin,
                            90f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Positive,
                            GridFillOrder.ZMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(175.6f, -9.3f),
                            new Vector2(175.6f, -6f),
                            new Vector2(176.3f, -6f),
                            new Vector2(176.3f, -9.3f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0f,
                            1,
                            3,
                            DefaultGridMargin,
                            90f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Negative,
                            GridFillOrder.ZMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(183.1f, -3.3f),
                            new Vector2(183.1f, -2.6f),
                            new Vector2(189.3f, -2.6f),
                            new Vector2(189.3f, -3.3f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0f,
                            6,
                            1,
                            DefaultGridMargin,
                            0f,
                            GridAxisDirection.Positive,
                            GridAxisDirection.Negative,
                            GridFillOrder.XMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(183.1f, -18.7f),
                            new Vector2(183.1f, -19.4f),
                            new Vector2(189.3f, -19.4f),
                            new Vector2(189.3f, -18.7f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0f,
                            6,
                            1,
                            DefaultGridMargin,
                            180f,
                            GridAxisDirection.Positive,
                            GridAxisDirection.Negative,
                            GridFillOrder.XMajor))
                });
        }

        private static IdlePointPlacementRule CreateManorRule()
        {
            return new IdlePointPlacementRule(
                PropertyBindingKey.Manor,
                12,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        CreateDefaultBounds,
                        new ExplicitIdlePointPlacementStrategy(new[]
                        {
                            new IdlePointPlacement(new Vector3(167.8f, 10f, -67.1f), 160f),
                            new IdlePointPlacement(new Vector3(168.6f, 10f, -68.2f), 175f),
                            new IdlePointPlacement(new Vector3(167.6f, 10f, -69.3f), 190f),
                            new IdlePointPlacement(new Vector3(168.5f, 10f, -70.3f), 200f),
                            new IdlePointPlacement(new Vector3(169.6f, 10f, -69.2f), 165f),
                            new IdlePointPlacement(new Vector3(170.6f, 10f, -69.5f), 185f),
                            new IdlePointPlacement(new Vector3(169.6f, 10f, -70.3f), 195f),
                            new IdlePointPlacement(new Vector3(171.8f, 10f, -70.3f), 170f)
                        }),
                        false),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(159.8f, -65.1f),
                            new Vector2(154f, -65.1f),
                            new Vector2(154f, -65.8f),
                            new Vector2(159.8f, -65.8f)
                        },
                        new GridIdlePointPlacementStrategy(
                            10f,
                            5,
                            1,
                            DefaultGridMargin,
                            180f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Negative,
                            GridFillOrder.ZMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(160.6f, -74.3f),
                            new Vector2(160.6f, -66.1f),
                            new Vector2(161.3f, -66.1f),
                            new Vector2(161.3f, -74.3f)
                        },
                        new GridIdlePointPlacementStrategy(
                            10f,
                            1,
                            6,
                            DefaultGridMargin,
                            90f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Positive,
                            GridFillOrder.XMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(176.4f, -60.1f),
                            new Vector2(176.4f, -65.2f),
                            new Vector2(177.1f, -65.2f),
                            new Vector2(177.1f, -60.1f)
                        },
                        new GridIdlePointPlacementStrategy(
                            10f,
                            1,
                            4,
                            DefaultGridMargin,
                            90f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Negative,
                            GridFillOrder.XMajor))
                });
        }

        private static IdlePointPlacementRule CreateDocksWarehouseRule()
        {
            return new IdlePointPlacementRule(
                PropertyBindingKey.DocksWarehouse,
                12,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        CreateStaticBounds(
                            new Vector2(-87.0f, -61.8f),
                            new Vector2(-83.3f, -59.9f),
                            new Vector2(-82.1f, -62f),
                            new Vector2(-85.6f, -64.2f)),
                        new OrientedGridIdlePointPlacementStrategy(
                            -2.4f,
                            4,
                            3,
                            DefaultGridMargin,
                            60f,
                            OrientedGridColumnDirection.PointBToPointA,
                            OrientedGridRowDirection.PointBToPointC,
                            OrientedGridFillOrder.ColumnMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        CreateStaticBounds(
                            new Vector2(-78.6f, -57f),
                            new Vector2(-74.6f, -54.7f),
                            new Vector2(-73.2f, -57.1f),
                            new Vector2(-77.2f, -59.4f)),
                        new OrientedGridIdlePointPlacementStrategy(
                            -2.4f,
                            4,
                            3,
                            DefaultGridMargin,
                            60f,
                            OrientedGridColumnDirection.PointAToPointB,
                            OrientedGridRowDirection.PointBToPointC,
                            OrientedGridFillOrder.ColumnMajor))
                });
        }

        private static IdlePointPlacementRule CreateMotelRoomRule()
        {
            return new IdlePointPlacementRule(
                PropertyBindingKey.MotelRoom,
                0,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        CreateDefaultBounds,
                        new ExplicitIdlePointPlacementStrategy(new[]
                        {
                            new IdlePointPlacement(new Vector3(-65.9f, 0f, 80.2f), 30f),
                            new IdlePointPlacement(new Vector3(-65.2f, 0f, 81.5f), 0f)
                        }),
                        false),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(-65.5f, 83.8f),
                            new Vector2(-65.5f, 87.6f),
                            new Vector2(-64.8f, 87.6f),
                            new Vector2(-64.8f, 83.8f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0.1f,
                            1,
                            4,
                            DefaultGridMargin,
                            0f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Positive,
                            GridFillOrder.ZMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(-69.3f, 78.9f),
                            new Vector2(-72.8f, 78.9f),
                            new Vector2(-72.8f, 78.2f),
                            new Vector2(-69.3f, 78.2f)
                        },
                        new GridIdlePointPlacementStrategy(
                            0f,
                            3,
                            1,
                            DefaultGridMargin,
                            90f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Positive,
                            GridFillOrder.XMajor))
                });
        }

        private static IdlePointPlacementRule CreateSewerOfficeRule()
        {
            return new IdlePointPlacementRule(
                PropertyBindingKey.SewerOffice,
                0,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(47.4f, 66.8f),
                            new Vector2(42.1f, 66.8f),
                            new Vector2(42.1f, 66.2f),
                            new Vector2(47.4f, 66.2f)
                        },
                        new GridIdlePointPlacementStrategy(
                            -9f,
                            5,
                            1,
                            DefaultGridMargin,
                            180f,
                            GridAxisDirection.Negative,
                            GridAxisDirection.Positive,
                            GridFillOrder.XMajor)),
                    new IdlePointPlacementArea(
                        handler => Vector3.zero,
                        startLocation => new[]
                        {
                            new Vector2(53.6f, 66.8f),
                            new Vector2(49.9f, 66.8f),
                            new Vector2(49.9f, 66.2f),
                            new Vector2(53.6f, 66.2f)
                        },
                        new GridIdlePointPlacementStrategy(
                            -9f,
                            5,
                            1,
                            DefaultGridMargin,
                            180f,
                            GridAxisDirection.Positive,
                            GridAxisDirection.Positive,
                            GridFillOrder.XMajor))
                });
        }

        private static Func<Vector3, Vector2[]> CreateStaticBounds(
            Vector2 pointA,
            Vector2 pointB,
            Vector2 pointC,
            Vector2 pointD)
        {
            return startLocation => new[] { pointA, pointB, pointC, pointD };
        }

        private static IdlePointPlacementRule CreateDefaultRule(PropertyBindingKey key, int vanillaIdlePointCount)
        {
            return new IdlePointPlacementRule(
                key,
                vanillaIdlePointCount,
                DefaultMaxAttempts,
                new[]
                {
                    new IdlePointPlacementArea(
                        handler => handler.GetIdlePointStartLocation(),
                        CreateDefaultBounds,
                        CreateDefaultGridStrategy())
                });
        }

        private static GridIdlePointPlacementStrategy CreateDefaultGridStrategy()
        {
            return new GridIdlePointPlacementStrategy(
                null,
                DefaultGridXCount,
                DefaultGridZCount,
                DefaultGridMargin,
                0f,
                GridAxisDirection.Positive,
                GridAxisDirection.Positive,
                GridFillOrder.XMajor);
        }

        private static Vector2[] CreateDefaultBounds(Vector3 startLocation)
        {
            return new[]
            {
                new Vector2(startLocation.x - DefaultBoundsHalfSize, startLocation.z - DefaultBoundsHalfSize),
                new Vector2(startLocation.x + DefaultBoundsHalfSize, startLocation.z - DefaultBoundsHalfSize),
                new Vector2(startLocation.x + DefaultBoundsHalfSize, startLocation.z + DefaultBoundsHalfSize),
                new Vector2(startLocation.x - DefaultBoundsHalfSize, startLocation.z + DefaultBoundsHalfSize)
            };
        }
    }
}
