using System;
using HarmonyLib;
using Il2CppScheduleOne.Dialogue;
using Il2CppScheduleOne.Property;

namespace ConfigurableMoreEmployees
{
    [HarmonyPatch(typeof(DialogueController_Fixer), nameof(DialogueController_Fixer.ModifyChoiceList))]
    internal static class MannyDialogueChoicePatch
    {
        private const string SelectLocationDialogueLabel = "SELECT_LOCATION";

        private static void Postfix(
            string dialogueLabel,
            ref Il2CppSystem.Collections.Generic.List<DialogueChoiceData> existingChoices)
        {
            if (!string.Equals(dialogueLabel, SelectLocationDialogueLabel, StringComparison.Ordinal) || existingChoices == null)
            {
                return;
            }

            foreach (var definition in PropertyDefinitionRegistry.Definitions)
            {
                if (definition.AddMannyDialogueChoice)
                {
                    TryAddMissingChoice(existingChoices, definition);
                }
            }
        }

        private static void TryAddMissingChoice(
            Il2CppSystem.Collections.Generic.List<DialogueChoiceData> existingChoices,
            PropertyDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(definition.PropertyCode) ||
                ContainsChoice(existingChoices, definition.PropertyCode))
            {
                return;
            }

            var property = GetProperty(definition.PropertyCode);
            if (property == null || !property.IsOwned || property.EmployeeCapacity <= 0)
            {
                return;
            }

            var choiceData = new DialogueChoiceData
            {
                Guid = Guid.NewGuid().ToString(),
                ChoiceLabel = definition.PropertyCode,
                ChoiceText = definition.DisplayName,
                ShowWorldspaceDialogue = false
            };

            existingChoices.Add(choiceData);
            MainMod.Instance?.VerboseLog(
                $"Added Manny location choice: {definition.PropertyCode}='{definition.DisplayName}'.");
        }

        private static bool ContainsChoice(
            Il2CppSystem.Collections.Generic.List<DialogueChoiceData> choices,
            string propertyCode)
        {
            for (var i = 0; i < choices.Count; i++)
            {
                if (string.Equals(choices[i]?.ChoiceLabel, propertyCode, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static Property GetProperty(string propertyCode)
        {
            var properties = Property.Properties;
            if (properties == null)
            {
                return null;
            }

            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                if (property != null && string.Equals(property.PropertyCode, propertyCode, StringComparison.OrdinalIgnoreCase))
                {
                    return property;
                }
            }

            return null;
        }

    }
}
