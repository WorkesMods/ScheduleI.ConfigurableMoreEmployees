namespace ConfigurableMoreEmployees
{
    /// <summary>
    /// Public extension points for adding employee capacity support to additional properties.
    /// </summary>
    public static class ConfigurableMoreEmployeesApi
    {
        /// <summary>
        /// Registers a property definition so Configurable More Employees can create preferences,
        /// generate idle points, raise employee capacity, and optionally add Manny dialogue for it.
        /// </summary>
        /// <param name="definition">The property metadata and idle point placement rules to register.</param>
        /// <returns><c>true</c> when the definition was accepted; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Register custom properties before properties are bound during world loading. Each definition
        /// must have a unique key and game object name.
        /// </remarks>
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
