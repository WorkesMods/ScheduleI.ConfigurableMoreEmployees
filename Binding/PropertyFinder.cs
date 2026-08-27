using System;
using System.Collections.Generic;
using Il2CppScheduleOne.Property;

namespace ConfigurableMoreEmployees
{
    internal static class PropertyFinder
    {
        internal static IReadOnlyList<Property> FindCandidates()
        {
            var candidates = new List<Property>();
            var properties = Property.Properties;

            if (properties == null)
            {
                return candidates;
            }

            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                if (property == null)
                {
                    continue;
                }

                var gameObjectName = GetGameObjectName(property);
                if (string.Equals(gameObjectName, PropertyBindingConstants.IgnoredRvParentGameObjectName, StringComparison.Ordinal))
                {
                    continue;
                }

                candidates.Add(property);
            }

            return candidates;
        }

        internal static string GetParentGameObjectName(Property property)
        {
            var parent = property?.transform?.parent;
            return parent != null ? parent.gameObject.name : property?.gameObject?.name ?? string.Empty;
        }

        internal static string GetGameObjectName(Property property)
        {
            return property?.gameObject?.name ?? string.Empty;
        }

        internal static string GetDiagnosticLabel(Property property)
        {
            var idlePointCount = property?.EmployeeIdlePoints != null ? property.EmployeeIdlePoints.Length : 0;
            var employeeCount = property?.Employees != null ? property.Employees.Count : 0;

            return
                $"type='{property?.GetIl2CppType()?.FullName ?? "<null>"}', " +
                $"gameObject='{GetGameObjectName(property)}', " +
                $"parent='{GetParentGameObjectName(property)}', " +
                $"propertyName='{property?.PropertyName ?? "<null>"}', " +
                $"propertyCode='{property?.PropertyCode ?? "<null>"}', " +
                $"employeeCapacity={property?.EmployeeCapacity.ToString() ?? "<null>"}, " +
                $"employees={employeeCount}, " +
                $"idlePoints={idlePointCount}";
        }
    }
}
