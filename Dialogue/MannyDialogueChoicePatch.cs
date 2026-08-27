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

        private static readonly MannyPropertyChoice[] ManagedChoices =
        {
            new MannyPropertyChoice("motelroom", "Motel Room"),
            new MannyPropertyChoice("seweroffice", "Sewer Office")
        };

        private static void Postfix(
            string dialogueLabel,
            ref Il2CppSystem.Collections.Generic.List<DialogueChoiceData> existingChoices)
        {
            if (!string.Equals(dialogueLabel, SelectLocationDialogueLabel, StringComparison.Ordinal) || existingChoices == null)
            {
                return;
            }

            foreach (var choice in ManagedChoices)
            {
                TryAddMissingChoice(existingChoices, choice);
            }
        }

        private static void TryAddMissingChoice(
            Il2CppSystem.Collections.Generic.List<DialogueChoiceData> existingChoices,
            MannyPropertyChoice choice)
        {
            if (ContainsChoice(existingChoices, choice.PropertyCode))
            {
                return;
            }

            var property = GetProperty(choice.PropertyCode);
            if (property == null || !property.IsOwned || property.EmployeeCapacity <= 0)
            {
                return;
            }

            var choiceData = new DialogueChoiceData
            {
                Guid = Guid.NewGuid().ToString(),
                ChoiceLabel = choice.PropertyCode,
                ChoiceText = choice.DisplayName,
                ShowWorldspaceDialogue = false
            };

            existingChoices.Add(choiceData);
            MainMod.Instance?.VerboseLog(
                $"Added Manny location choice: {choice.PropertyCode}='{choice.DisplayName}'.");
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

        private readonly struct MannyPropertyChoice
        {
            internal MannyPropertyChoice(string propertyCode, string displayName)
            {
                PropertyCode = propertyCode;
                DisplayName = displayName;
            }

            internal string PropertyCode { get; }
            internal string DisplayName { get; }
        }
    }
}
