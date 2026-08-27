namespace ConfigurableMoreEmployees
{
    internal enum PropertyBindingKey
    {
        Barn,
        Manor,
        Bungalow,
        DocksWarehouse,
        Sweatshop,
        MotelRoom,
        StorageUnit,
        SewerOffice
    }

    internal sealed class PropertyBinding
    {
        internal PropertyBinding(PropertyBindingKey key, string gameObjectName, string displayName)
        {
            Key = key;
            GameObjectName = gameObjectName;
            DisplayName = displayName;
        }

        internal PropertyBindingKey Key { get; }
        internal string GameObjectName { get; }
        internal string DisplayName { get; }
    }

    internal static class PropertyBindingConstants
    {
        internal const int ExpectedPropertyStartCount = 13;
        internal const string IgnoredRvParentGameObjectName = "RV";

        internal static readonly PropertyBinding[] RequiredBindings =
        {
            new PropertyBinding(PropertyBindingKey.Barn, "Barn", "Barn"),
            new PropertyBinding(PropertyBindingKey.Manor, "Manor", "Manor"),
            new PropertyBinding(PropertyBindingKey.Bungalow, "Bungalow", "Bungalow"),
            new PropertyBinding(PropertyBindingKey.DocksWarehouse, "DocksWarehouse", "Docks Warehouse"),
            new PropertyBinding(PropertyBindingKey.Sweatshop, "Sweatshop", "Sweatshop"),
            new PropertyBinding(PropertyBindingKey.MotelRoom, "MotelRoom", "Motel Room"),
            new PropertyBinding(PropertyBindingKey.StorageUnit, "StorageUnit", "Storage Unit"),
            new PropertyBinding(PropertyBindingKey.SewerOffice, "Sewer office", "Sewer Office")
        };
    }
}
