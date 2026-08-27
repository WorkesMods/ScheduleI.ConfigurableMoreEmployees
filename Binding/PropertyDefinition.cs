using System;

namespace ConfigurableMoreEmployees
{
    public sealed class PropertyDefinition
    {
        public PropertyDefinition(
            string key,
            string gameObjectName,
            string displayName,
            string propertyCode,
            int vanillaIdlePointCount,
            int defaultMaxEmployees,
            bool addMannyDialogueChoice,
            IdlePointPlacementArea[] placementAreas,
            string preferenceKey = null,
            string preferenceDescription = null,
            bool required = false)
        {
            Key = key;
            GameObjectName = gameObjectName;
            DisplayName = displayName;
            PropertyCode = propertyCode;
            VanillaIdlePointCount = vanillaIdlePointCount;
            DefaultMaxEmployees = defaultMaxEmployees;
            AddMannyDialogueChoice = addMannyDialogueChoice;
            PlacementAreas = placementAreas ?? Array.Empty<IdlePointPlacementArea>();
            PreferenceKey = string.IsNullOrWhiteSpace(preferenceKey)
                ? $"{NormalizePreferenceKey(key)}MaxEmployees"
                : preferenceKey;
            PreferenceDescription = preferenceDescription ??
                $"Maximum employees allowed at {displayName}.";
            Required = required;
        }

        public string Key { get; }
        public string GameObjectName { get; }
        public string DisplayName { get; }
        public string PropertyCode { get; }
        public int VanillaIdlePointCount { get; }
        public int DefaultMaxEmployees { get; }
        public bool AddMannyDialogueChoice { get; }
        public IdlePointPlacementArea[] PlacementAreas { get; }
        public string PreferenceKey { get; }
        public string PreferenceDescription { get; }
        public bool Required { get; }

        private static string NormalizePreferenceKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "CustomProperty";
            }

            var characters = key.ToCharArray();
            var writeIndex = 0;
            for (var i = 0; i < characters.Length; i++)
            {
                if (char.IsLetterOrDigit(characters[i]))
                {
                    characters[writeIndex] = characters[i];
                    writeIndex++;
                }
            }

            return writeIndex == 0
                ? "CustomProperty"
                : new string(characters, 0, writeIndex);
        }
    }
}
