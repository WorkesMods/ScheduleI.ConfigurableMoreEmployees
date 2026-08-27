using System;
using System.Linq;
using UnityEngine;

namespace ConfigurableMoreEmployees
{
    internal sealed class ConfigurableMoreEmployeesService
    {
        private readonly EmployeeLimitPreferences preferences;
        private PropertyHandler[] propertyHandlers = Array.Empty<PropertyHandler>();
        private Transform idlePointTemplate;
        private int propertiesStartedThisScene;
        private bool expectedPropertiesStarted;
        private bool bindingAttemptedForScene;
        private bool deactivated;

        internal ConfigurableMoreEmployeesService(EmployeeLimitPreferences preferences)
        {
            this.preferences = preferences;
        }

        internal void BeginWorldLoad(string reason)
        {
            ResetRuntimeState();
            MainMod.Instance.LoggerInstance.Msg($"World load started: {reason}");
        }

        internal void CompleteWorldLoad(string reason)
        {
            if (deactivated)
            {
                return;
            }

            MainMod.Instance.LoggerInstance.Msg($"World load completed: {reason}");

            if (bindingAttemptedForScene)
            {
                RefreshDebugVisuals();
                return;
            }

            TryBindAndApplyForCurrentScene();
        }

        internal void SetWorldLoaded(bool isLoaded, string reason)
        {
            if (isLoaded)
            {
                CompleteWorldLoad(reason);
            }

            // The game sets IsGameLoaded=false repeatedly during startup/menu transitions.
            // Runtime state is cleared by explicit world load, exit-to-menu, or scene unload events.
        }

        internal void LeaveWorld(string reason)
        {
            ResetRuntimeState();
            MainMod.Instance.LoggerInstance.Msg($"World state cleared: {reason}");
        }

        internal void RecordPropertyStarted(Il2CppScheduleOne.Property.Property property)
        {
            if (deactivated || bindingAttemptedForScene)
            {
                return;
            }

            propertiesStartedThisScene++;

            if (propertiesStartedThisScene < PropertyBindingConstants.ExpectedPropertyStartCount || expectedPropertiesStarted)
            {
                return;
            }

            expectedPropertiesStarted = true;
            MainMod.Instance.LoggerInstance.Msg(
                $"Detected {propertiesStartedThisScene}/{PropertyBindingConstants.ExpectedPropertyStartCount} properties.");
            TryBindAndApplyForCurrentScene();
        }

        internal void TryBindAndApplyForCurrentScene()
        {
            if (deactivated || bindingAttemptedForScene)
            {
                return;
            }

            if (propertiesStartedThisScene < PropertyBindingConstants.ExpectedPropertyStartCount)
            {
                return;
            }

            MainMod.Instance.LoggerInstance.Msg("Binding configured properties.");
            bindingAttemptedForScene = true;
            var candidates = PropertyFinder.FindCandidates();
            var bindResult = PropertyBinder.Bind(candidates, PropertyDefinitionRegistry.Definitions);

            foreach (var warning in bindResult.Warnings)
            {
                MainMod.Instance.LoggerInstance.Warning(warning);
            }

            if (!bindResult.Success)
            {
                deactivated = true;
                propertyHandlers = Array.Empty<PropertyHandler>();
                MainMod.Instance.LoggerInstance.Error("Configurable More Employees is deactivated for this scene.");
                MainMod.Instance.LoggerInstance.Error(bindResult.ErrorMessage);
                return;
            }

            propertyHandlers = bindResult.Handlers;
            idlePointTemplate = propertyHandlers
                .Select(handler => handler.GetFirstExistingIdlePoint())
                .FirstOrDefault(template => template != null);

            if (idlePointTemplate == null)
            {
                MainMod.Instance.LoggerInstance.Error("No existing idle point template was found. Properties that need generated idle points will be skipped.");
            }

            ApplyConfiguredMaxEmployees();
        }

        internal Transform GetIdlePointTemplate()
        {
            return idlePointTemplate;
        }

        internal void HandleSettingsUpdated()
        {
            try
            {
                LogConfiguredLimits("Settings updated");
                ApplyConfiguredMaxEmployees();
            }
            catch (Exception ex)
            {
                MainMod.Instance.LoggerInstance.Error($"Could not apply updated settings: {ex}");
            }
        }

        internal void LogConfiguredLimits(string prefix)
        {
            MainMod.Instance.LoggerInstance.Msg($"{prefix}: {preferences.FormatConfiguredLimits()}");
            MainMod.Instance.VerboseLog(
                $"Debug settings: markers {preferences.DebugShowIdlePointMarkers.Value}, " +
                $"verbose logging {preferences.DebugVerboseLogging.Value}");
        }

        internal void ResetRuntimeState()
        {
            foreach (var handler in propertyHandlers)
            {
                handler.ClearIdlePointMarkers();
            }

            propertyHandlers = Array.Empty<PropertyHandler>();
            idlePointTemplate = null;
            propertiesStartedThisScene = 0;
            expectedPropertiesStarted = false;
            bindingAttemptedForScene = false;
            deactivated = false;
        }

        private void ApplyConfiguredMaxEmployees()
        {
            if (deactivated || propertyHandlers.Length == 0)
            {
                return;
            }

            foreach (var handler in propertyHandlers)
            {
                handler.ApplyMaxEmployees(preferences.GetMaxEmployees(handler.Definition));
            }

            RefreshDebugVisuals();
        }

        private void RefreshDebugVisuals()
        {
            foreach (var handler in propertyHandlers)
            {
                handler.SetIdlePointMarkersVisible(preferences.DebugShowIdlePointMarkers.Value);
            }
        }
    }
}
