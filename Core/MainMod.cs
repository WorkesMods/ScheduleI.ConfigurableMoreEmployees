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

        internal EmployeeLimitPreferences Preferences { get; private set; }
        internal ConfigurableMoreEmployeesService Service { get; private set; }

        private ModManagerIntegration modManagerIntegration;

        public override void OnInitializeMelon()
        {
            Instance = this;
            Preferences = new EmployeeLimitPreferences();
            Service = new ConfigurableMoreEmployeesService(Preferences);
            modManagerIntegration = new ModManagerIntegration(LoggerInstance);

            LoggerInstance.Msg("Configurable More Employees loaded.");
            Service.LogConfiguredLimits("Configured max employees");
            modManagerIntegration.Subscribe(Service.HandleSettingsUpdated);
        }

        public override void OnDeinitializeMelon()
        {
            modManagerIntegration?.Unsubscribe();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            Service?.ResetRuntimeState();
        }

        internal void VerboseLog(string message)
        {
            if (Preferences?.DebugVerboseLogging?.Value == true)
            {
                LoggerInstance.Msg(message);
            }
        }

        internal void EnsurePreferencesForDefinition(PropertyDefinition definition)
        {
            Preferences?.EnsureEntry(definition);
        }
    }

}
