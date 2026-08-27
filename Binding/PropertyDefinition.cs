using System;

namespace ConfigurableMoreEmployees
{
    /// <summary>
    /// Describes a property that can receive configurable employee limits and generated idle points.
    /// </summary>
    public sealed class PropertyDefinition
    {
        /// <summary>
        /// Creates a property definition for the built-in registry or external mod registration.
        /// </summary>
        /// <param name="key">Stable unique identifier for this definition.</param>
        /// <param name="gameObjectName">Exact scene game object name used to find the property.</param>
        /// <param name="displayName">Human-readable name used in logs and generated preference descriptions.</param>
        /// <param name="propertyCode">Property code used by the game's employee assignment flow.</param>
        /// <param name="vanillaIdlePointCount">Number of idle points the property normally has before this mod generates more.</param>
        /// <param name="defaultMaxEmployees">Default value for the generated max employees preference.</param>
        /// <param name="addMannyDialogueChoice">Whether Manny should offer this property when it is owned and has capacity above zero.</param>
        /// <param name="placementAreas">Placement rules used to generate extra idle points.</param>
        /// <param name="preferenceKey">Optional explicit preference key. If omitted, one is derived from <paramref name="key"/>.</param>
        /// <param name="preferenceDescription">Optional preference description shown in config UIs.</param>
        /// <param name="required">Whether missing runtime binding for this property should be logged as a required-property issue.</param>
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

        /// <summary>
        /// Stable unique identifier for this definition.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Exact scene game object name used to find the property.
        /// </summary>
        public string GameObjectName { get; }

        /// <summary>
        /// Human-readable property name used in logs and generated preference descriptions.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Property code used by the game's employee assignment flow.
        /// </summary>
        public string PropertyCode { get; }

        /// <summary>
        /// Number of idle points the property normally has before generated idle points are added.
        /// </summary>
        public int VanillaIdlePointCount { get; }

        /// <summary>
        /// Default value for the max employees preference.
        /// </summary>
        public int DefaultMaxEmployees { get; }

        /// <summary>
        /// Whether Manny dialogue should include this property when it is owned and has capacity above zero.
        /// </summary>
        public bool AddMannyDialogueChoice { get; }

        /// <summary>
        /// Placement rules used to generate additional idle points for this property.
        /// </summary>
        public IdlePointPlacementArea[] PlacementAreas { get; }

        /// <summary>
        /// MelonPreferences key that stores the configured employee cap.
        /// </summary>
        public string PreferenceKey { get; }

        /// <summary>
        /// Description shown for the generated employee cap preference.
        /// </summary>
        public string PreferenceDescription { get; }

        /// <summary>
        /// Whether missing runtime binding for this property should be treated as important in logs.
        /// </summary>
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
