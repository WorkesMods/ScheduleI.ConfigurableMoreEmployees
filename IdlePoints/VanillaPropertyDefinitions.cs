namespace ConfigurableMoreEmployees
{
    internal static class VanillaPropertyDefinitions
    {
        internal static PropertyDefinition[] Create()
        {
            return new[]
            {
                BarnDefinition.Create(),
                BungalowDefinition.Create(),
                DocksWarehouseDefinition.Create(),
                ManorDefinition.Create(),
                MotelRoomDefinition.Create(),
                SewerOfficeDefinition.Create(),
                StorageUnitDefinition.Create(),
                SweatshopDefinition.Create()
            };
        }
    }
}
