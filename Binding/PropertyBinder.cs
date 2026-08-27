using System.Collections.Generic;
using System.Linq;
using Il2CppScheduleOne.Property;

namespace ConfigurableMoreEmployees
{
    internal sealed class PropertyBindResult
    {
        private PropertyBindResult(bool success, PropertyHandler[] handlers, string errorMessage)
        {
            Success = success;
            Handlers = handlers;
            ErrorMessage = errorMessage;
        }

        internal bool Success { get; }
        internal PropertyHandler[] Handlers { get; }
        internal string ErrorMessage { get; }

        internal static PropertyBindResult Ok(PropertyHandler[] handlers)
        {
            return new PropertyBindResult(true, handlers, string.Empty);
        }

        internal static PropertyBindResult Fail(string errorMessage)
        {
            return new PropertyBindResult(false, new PropertyHandler[0], errorMessage);
        }
    }

    internal static class PropertyBinder
    {
        internal static PropertyBindResult Bind(IReadOnlyList<Property> candidates)
        {
            var handlers = new List<PropertyHandler>();
            var errors = new List<string>();

            foreach (var binding in PropertyBindingConstants.RequiredBindings)
            {
                var matches = candidates
                    .Where(candidate => PropertyFinder.GetGameObjectName(candidate) == binding.GameObjectName)
                    .ToArray();

                if (matches.Length == 0)
                {
                    errors.Add($"Missing property binding: {binding.DisplayName} ({binding.GameObjectName})");
                    continue;
                }

                if (matches.Length > 1)
                {
                    errors.Add($"Duplicate property binding: {binding.DisplayName} ({binding.GameObjectName}) matched {matches.Length} candidates");
                    continue;
                }

                handlers.Add(new PropertyHandler(binding, matches[0]));
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

            return PropertyBindResult.Ok(handlers.ToArray());
        }
    }
}
