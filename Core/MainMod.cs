using System;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(
    typeof(ConfigurableMoreEmployees.MainMod),
    "Configurable More Employees",
    "0.1.0",
    "Workes")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ConfigurableMoreEmployees
{
    public sealed class MainMod : MelonMod
    {
        internal static MainMod Instance;

        internal static MelonPreferences_Category ConfigCategory;
        internal static MelonPreferences_Category DebugCategory;
        internal static MelonPreferences_Entry<int> StorageUnitMaxEmployees;
        internal static MelonPreferences_Entry<int> SweatshopMaxEmployees;
        internal static MelonPreferences_Entry<int> BungalowMaxEmployees;
        internal static MelonPreferences_Entry<int> BarnMaxEmployees;
        internal static MelonPreferences_Entry<int> ManorMaxEmployees;
        internal static MelonPreferences_Entry<int> DocksWarehouseMaxEmployees;
        internal static MelonPreferences_Entry<int> MotelMaxEmployees;
        internal static MelonPreferences_Entry<int> SewerMaxEmployees;
        internal static MelonPreferences_Entry<bool> DebugShowIdlePointMarkers;
        internal static MelonPreferences_Entry<bool> DebugVerboseLogging;

        private PropertyHandler[] propertyHandlers = Array.Empty<PropertyHandler>();
        private UnityEngine.Transform idlePointTemplate;
        private int propertiesStartedThisScene;
        private bool expectedPropertiesStarted;
        private bool bindingAttemptedForScene;
        private bool deactivated;
        private EventInfo modManagerPhonePreferencesSavedEvent;
        private EventInfo modManagerMenuPreferencesSavedEvent;
        private Delegate modManagerPhonePreferencesSavedHandler;
        private Delegate modManagerMenuPreferencesSavedHandler;

        public override void OnInitializeMelon()
        {
            Instance = this;
            ConfigCategory = MelonPreferences.CreateCategory(
                "Configurable More Employees",
                "Configurable More Employees");
            DebugCategory = MelonPreferences.CreateCategory(
                "Configurable More Employees Debug",
                "Configurable More Employees Debug");

            StorageUnitMaxEmployees = ConfigCategory.CreateEntry(
                "StorageUnitMaxEmployees",
                5,
                "Storage Unit Max Employees",
                "Maximum employees allowed at the Storage Unit.");
            SweatshopMaxEmployees = ConfigCategory.CreateEntry(
                "SweatshopMaxEmployees",
                3,
                "Sweatshop Max Employees",
                "Maximum employees allowed at the Sweatshop.");
            BungalowMaxEmployees = ConfigCategory.CreateEntry(
                "BungalowMaxEmployees",
                7,
                "Bungalow Max Employees",
                "Maximum employees allowed at the Bungalow.");
            BarnMaxEmployees = ConfigCategory.CreateEntry(
                "BarnMaxEmployees",
                13,
                "Barn Max Employees",
                "Maximum employees allowed at the Barn.");
            ManorMaxEmployees = ConfigCategory.CreateEntry(
                "ManorMaxEmployees",
                17,
                "Manor Max Employees",
                "Maximum employees allowed at the Manor.");
            DocksWarehouseMaxEmployees = ConfigCategory.CreateEntry(
                "DocksWarehouseMaxEmployees",
                17,
                "Docks Warehouse Max Employees",
                "Maximum employees allowed at the Docks Warehouse.");
            MotelMaxEmployees = ConfigCategory.CreateEntry(
                "MotelMaxEmployees",
                0,
                "Motel Max Employees",
                "Maximum employees allowed at the Motel.");
            SewerMaxEmployees = ConfigCategory.CreateEntry(
                "SewerMaxEmployees",
                0,
                "Sewer Max Employees",
                "Maximum employees allowed at the Sewer Office.");
            DebugShowIdlePointMarkers = DebugCategory.CreateEntry(
                "DebugShowIdlePointMarkers",
                false,
                "Debug Show Idle Point Markers",
                "Shows visible in-world markers for vanilla and generated employee idle points.");
            DebugVerboseLogging = DebugCategory.CreateEntry(
                "DebugVerboseLogging",
                false,
                "Debug Verbose Logging",
                "Logs detailed placement decisions.");

            LoggerInstance.Msg("Configurable More Employees loaded.");
            LogConfiguredLimits("Configured max employees");
            SubscribeToModManagerEvents();
        }

        public override void OnDeinitializeMelon()
        {
            UnsubscribeFromModManagerEvents();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            ResetRuntimeState();
        }

        internal void BeginWorldLoad(string reason)
        {
            ResetRuntimeState();
            LoggerInstance.Msg($"World load started: {reason}");
        }

        internal void CompleteWorldLoad(string reason)
        {
            if (deactivated)
            {
                return;
            }

            LoggerInstance.Msg($"World load completed: {reason}");

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
                return;
            }

            // The game sets IsGameLoaded=false repeatedly during startup/menu transitions.
            // Runtime state is cleared by explicit world load, exit-to-menu, or scene unload events.
        }

        internal void LeaveWorld(string reason)
        {
            ResetRuntimeState();
            LoggerInstance.Msg($"World state cleared: {reason}");
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
            LoggerInstance.Msg(
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

            LoggerInstance.Msg("Binding configured properties.");
            bindingAttemptedForScene = true;
            var candidates = PropertyFinder.FindCandidates();
            var bindResult = PropertyBinder.Bind(candidates);

            if (!bindResult.Success)
            {
                deactivated = true;
                propertyHandlers = Array.Empty<PropertyHandler>();
                LoggerInstance.Error("Configurable More Employees is deactivated for this scene.");
                LoggerInstance.Error(bindResult.ErrorMessage);
                return;
            }

            propertyHandlers = bindResult.Handlers;
            idlePointTemplate = propertyHandlers
                .Select(handler => handler.GetFirstExistingIdlePoint())
                .FirstOrDefault(template => template != null);

            if (idlePointTemplate == null)
            {
                LoggerInstance.Error("No existing idle point template was found. Properties that need generated idle points will be skipped.");
            }

            ApplyConfiguredMaxEmployees();
        }

        internal UnityEngine.Transform GetIdlePointTemplate()
        {
            return idlePointTemplate;
        }

        private void SubscribeToModManagerEvents()
        {
            if (!MelonBase.RegisteredMelons.Any(mod => mod?.Info?.Name == "Mod Manager & Phone App"))
            {
                LoggerInstance.Msg("Mod Manager - Phone App was not found. Settings will still work through MelonPreferences.cfg.");
                return;
            }

            try
            {
                var eventsType = FindModManagerEventsType();
                if (eventsType == null)
                {
                    LoggerInstance.Warning("Mod Manager - Phone App was found, but its settings events type was unavailable.");
                    return;
                }

                modManagerPhonePreferencesSavedEvent = eventsType.GetEvent(
                    "OnPhonePreferencesSaved",
                    BindingFlags.Public | BindingFlags.Static);
                modManagerMenuPreferencesSavedEvent = eventsType.GetEvent(
                    "OnMenuPreferencesSaved",
                    BindingFlags.Public | BindingFlags.Static);

                modManagerPhonePreferencesSavedHandler = SubscribeToModManagerEvent(modManagerPhonePreferencesSavedEvent);
                modManagerMenuPreferencesSavedHandler = SubscribeToModManagerEvent(modManagerMenuPreferencesSavedEvent);

                if (modManagerPhonePreferencesSavedHandler == null && modManagerMenuPreferencesSavedHandler == null)
                {
                    LoggerInstance.Warning("Mod Manager - Phone App was found, but no compatible save events were available.");
                    return;
                }

                LoggerInstance.Msg("Subscribed to Mod Manager - Phone App save events.");
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Could not subscribe to Mod Manager - Phone App events: {ex}");
            }
        }

        private static Type FindModManagerEventsType()
        {
            const string eventTypeName = "ModManagerPhoneApp.ModSettingsEvents";
            var eventsType = Type.GetType($"{eventTypeName}, ModManager&PhoneApp");
            if (eventsType != null)
            {
                return eventsType;
            }

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetType(eventTypeName, false))
                .FirstOrDefault(type => type != null);
        }

        private Delegate SubscribeToModManagerEvent(EventInfo eventInfo)
        {
            if (eventInfo?.EventHandlerType == null)
            {
                return null;
            }

            var handler = Delegate.CreateDelegate(
                eventInfo.EventHandlerType,
                this,
                nameof(HandleSettingsUpdated));
            eventInfo.AddEventHandler(null, handler);
            return handler;
        }

        private void UnsubscribeFromModManagerEvents()
        {
            try
            {
                if (modManagerPhonePreferencesSavedEvent != null && modManagerPhonePreferencesSavedHandler != null)
                {
                    modManagerPhonePreferencesSavedEvent.RemoveEventHandler(null, modManagerPhonePreferencesSavedHandler);
                }

                if (modManagerMenuPreferencesSavedEvent != null && modManagerMenuPreferencesSavedHandler != null)
                {
                    modManagerMenuPreferencesSavedEvent.RemoveEventHandler(null, modManagerMenuPreferencesSavedHandler);
                }
            }
            catch
            {
            }
            finally
            {
                modManagerPhonePreferencesSavedEvent = null;
                modManagerMenuPreferencesSavedEvent = null;
                modManagerPhonePreferencesSavedHandler = null;
                modManagerMenuPreferencesSavedHandler = null;
            }
        }

        private void HandleSettingsUpdated()
        {
            try
            {
                LogConfiguredLimits("Settings updated");
                ApplyConfiguredMaxEmployees();
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Could not apply updated settings: {ex}");
            }
        }

        private void ApplyConfiguredMaxEmployees()
        {
            if (deactivated || propertyHandlers.Length == 0)
            {
                return;
            }

            foreach (var handler in propertyHandlers)
            {
                var maxEmployees = GetConfiguredMaxEmployees(handler.Binding);
                handler.ApplyMaxEmployees(maxEmployees);
            }

            RefreshDebugVisuals();
        }

        private void RefreshDebugVisuals()
        {
            foreach (var handler in propertyHandlers)
            {
                handler.SetIdlePointMarkersVisible(DebugShowIdlePointMarkers.Value);
            }
        }

        private static int GetConfiguredMaxEmployees(PropertyBinding binding)
        {
            switch (binding.Key)
            {
                case PropertyBindingKey.StorageUnit:
                    return StorageUnitMaxEmployees.Value;
                case PropertyBindingKey.Sweatshop:
                    return SweatshopMaxEmployees.Value;
                case PropertyBindingKey.Bungalow:
                    return BungalowMaxEmployees.Value;
                case PropertyBindingKey.Barn:
                    return BarnMaxEmployees.Value;
                case PropertyBindingKey.Manor:
                    return ManorMaxEmployees.Value;
                case PropertyBindingKey.DocksWarehouse:
                    return DocksWarehouseMaxEmployees.Value;
                case PropertyBindingKey.MotelRoom:
                    return MotelMaxEmployees.Value;
                case PropertyBindingKey.SewerOffice:
                    return SewerMaxEmployees.Value;
                default:
                    return 0;
            }
        }

        private void LogConfiguredLimits(string prefix)
        {
            LoggerInstance.Msg(
                $"{prefix}: " +
                $"Storage Unit {StorageUnitMaxEmployees.Value}, " +
                $"Sweatshop {SweatshopMaxEmployees.Value}, " +
                $"Bungalow {BungalowMaxEmployees.Value}, " +
                $"Barn {BarnMaxEmployees.Value}, " +
                $"Manor {ManorMaxEmployees.Value}, " +
                $"Docks Warehouse {DocksWarehouseMaxEmployees.Value}, " +
                $"Motel {MotelMaxEmployees.Value}, " +
                $"Sewer {SewerMaxEmployees.Value}");
            VerboseLog(
                $"Debug settings: markers {DebugShowIdlePointMarkers.Value}, " +
                $"verbose logging {DebugVerboseLogging.Value}");
        }

        internal void VerboseLog(string message)
        {
            if (DebugVerboseLogging?.Value == true)
            {
                LoggerInstance.Msg(message);
            }
        }

        private void ResetRuntimeState()
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
    }

}
