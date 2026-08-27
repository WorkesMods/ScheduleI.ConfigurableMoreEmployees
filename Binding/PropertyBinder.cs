using System.Collections.Generic;
using System.Linq;
using Il2CppScheduleOne.Property;

namespace ConfigurableMoreEmployees
{
    internal sealed class PropertyBindResult
    {
        private PropertyBindResult(
            bool success,
            PropertyHandler[] handlers,
            string errorMessage,
            string[] warnings)
        {
            Success = success;
            Handlers = handlers;
            ErrorMessage = errorMessage;
            Warnings = warnings;
        }

        internal bool Success { get; }
        internal PropertyHandler[] Handlers { get; }
        internal string ErrorMessage { get; }
        internal string[] Warnings { get; }

        internal static PropertyBindResult Ok(PropertyHandler[] handlers, string[] warnings)
        {
            return new PropertyBindResult(true, handlers, string.Empty, warnings);
        }

        internal static PropertyBindResult Fail(string errorMessage)
        {
            return new PropertyBindResult(false, new PropertyHandler[0], errorMessage, new string[0]);
        }
    }

    internal static class PropertyBinder
    {
        internal static PropertyBindResult Bind(
            IReadOnlyList<Property> candidates,
            IReadOnlyList<PropertyDefinition> definitions)
        {
            var handlers = new List<PropertyHandler>();
            var errors = new List<string>();
            var warnings = new List<string>();

            foreach (var definition in definitions)
            {
                var matches = candidates
                    .Where(candidate => PropertyFinder.GetGameObjectName(candidate) == definition.GameObjectName)
                    .ToArray();

                if (matches.Length == 0)
                {
                    var message = $"Missing property binding: {definition.DisplayName} ({definition.GameObjectName})";
                    if (definition.Required)
                    {
                        errors.Add(message);
                    }
                    else
                    {
                        warnings.Add(message);
                    }

                    continue;
                }

                if (matches.Length > 1)
                {
                    errors.Add($"Duplicate property binding: {definition.DisplayName} ({definition.GameObjectName}) matched {matches.Length} candidates");
                    continue;
                }

                handlers.Add(new PropertyHandler(definition, matches[0]));
            }

            if (errors.Count > 0)
            {
                var candidateDetails = candidates.Count == 0
                    ? "No candidates were found."
                    : string.Join("; ", candidates.Select(PropertyFinder.GetDiagnosticLabel));

                return PropertyBindResult.Fail(
                    string.Join("; ", errors) +
                    ". Candidate properties: " +
                    candidateDetails);
            }

            return PropertyBindResult.Ok(handlers.ToArray(), warnings.ToArray());
        }
    }
}
