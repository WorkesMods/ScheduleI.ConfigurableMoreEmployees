using System.Collections.Generic;
using System.Linq;
using MelonLoader;

namespace ConfigurableMoreEmployees
{
    internal sealed class EmployeeLimitPreferences
    {
        private readonly MelonPreferences_Category configCategory;
        private readonly Dictionary<string, MelonPreferences_Entry<int>> maxEmployeeEntries =
            new Dictionary<string, MelonPreferences_Entry<int>>();

        internal EmployeeLimitPreferences()
        {
            configCategory = MelonPreferences.CreateCategory(
                "Configurable More Employees",
                "Configurable More Employees");
            var debugCategory = MelonPreferences.CreateCategory(
                "Configurable More Employees Debug",
                "Configurable More Employees Debug");

            foreach (var definition in PropertyDefinitionRegistry.Definitions)
            {
                EnsureEntry(definition);
            }

            DebugShowIdlePointMarkers = debugCategory.CreateEntry(
                "DebugShowIdlePointMarkers",
                false,
                "Debug Show Idle Point Markers",
                "Shows visible in-world markers for vanilla and generated employee idle points.");
            DebugVerboseLogging = debugCategory.CreateEntry(
                "DebugVerboseLogging",
                false,
                "Debug Verbose Logging",
                "Logs detailed placement decisions.");
        }

        internal MelonPreferences_Entry<bool> DebugShowIdlePointMarkers { get; }
        internal MelonPreferences_Entry<bool> DebugVerboseLogging { get; }

        internal void EnsureEntry(PropertyDefinition definition)
        {
            if (maxEmployeeEntries.ContainsKey(definition.Key))
            {
                return;
            }

            maxEmployeeEntries[definition.Key] = configCategory.CreateEntry(
                definition.PreferenceKey,
                definition.DefaultMaxEmployees,
                $"{definition.DisplayName} Max Employees",
                definition.PreferenceDescription);
        }

        internal int GetMaxEmployees(PropertyDefinition definition)
        {
            return maxEmployeeEntries.TryGetValue(definition.Key, out var entry)
                ? entry.Value
                : definition.DefaultMaxEmployees;
        }

        internal string FormatConfiguredLimits()
        {
            return string.Join(
                ", ",
                PropertyDefinitionRegistry.Definitions.Select(definition =>
                    $"{definition.DisplayName} {GetMaxEmployees(definition)}"));
        }
    }
}
