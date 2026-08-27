namespace ConfigurableMoreEmployees
{
    public static class ConfigurableMoreEmployeesApi
    {
        public static bool RegisterProperty(PropertyDefinition definition)
        {
            if (PropertyDefinitionRegistry.TryRegister(definition, out var errorMessage))
            {
                MainMod.Instance?.LoggerInstance.Msg(
                    $"Registered external property definition: {definition.DisplayName} ({definition.GameObjectName}).");
                return true;
            }

            MainMod.Instance?.LoggerInstance.Warning(
                $"Could not register external property definition: {errorMessage}");
            return false;
        }
    }
}
