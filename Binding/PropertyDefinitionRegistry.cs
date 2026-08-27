using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigurableMoreEmployees
{
    internal static class PropertyDefinitionRegistry
    {
        private static readonly List<PropertyDefinition> definitions =
            new List<PropertyDefinition>(VanillaPropertyDefinitions.Create());

        internal static IReadOnlyList<PropertyDefinition> Definitions => definitions;

        internal static bool TryRegister(PropertyDefinition definition, out string errorMessage)
        {
            errorMessage = Validate(definition);
            if (errorMessage != null)
            {
                return false;
            }

            definitions.Add(definition);
            MainMod.Instance?.EnsurePreferencesForDefinition(definition);
            return true;
        }

        private static string Validate(PropertyDefinition definition)
        {
            if (definition == null)
            {
                return "Property definition was null.";
            }

            if (string.IsNullOrWhiteSpace(definition.Key))
            {
                return "Property definition key cannot be empty.";
            }

            if (string.IsNullOrWhiteSpace(definition.GameObjectName))
            {
                return $"Property definition '{definition.Key}' game object name cannot be empty.";
            }

            if (string.IsNullOrWhiteSpace(definition.DisplayName))
            {
                return $"Property definition '{definition.Key}' display name cannot be empty.";
            }

            if (definitions.Any(existing => string.Equals(existing.Key, definition.Key, StringComparison.OrdinalIgnoreCase)))
            {
                return $"Property definition key '{definition.Key}' is already registered.";
            }

            if (definitions.Any(existing => string.Equals(existing.GameObjectName, definition.GameObjectName, StringComparison.Ordinal)))
            {
                return $"Property game object '{definition.GameObjectName}' is already registered.";
            }

            return null;
        }
    }
}
