using HarmonyLib;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.UI.MainMenu;

namespace ConfigurableMoreEmployees
{
    [HarmonyPatch(typeof(Property), nameof(Property.Start))]
    internal static class PropertyStartPatch
    {
        private static void Postfix(Property __instance)
        {
            MainMod.Instance?.RecordPropertyStarted(__instance);
        }
    }

    [HarmonyPatch(typeof(LoadManager), nameof(LoadManager.StartGame))]
    internal static class LoadManagerStartGamePatch
    {
        private static void Prefix()
        {
            MainMod.Instance?.BeginWorldLoad(nameof(LoadManager.StartGame));
        }
    }

    [HarmonyPatch(typeof(LoadManager), nameof(LoadManager.LoadLastSave))]
    internal static class LoadManagerLoadLastSavePatch
    {
        private static void Prefix()
        {
            MainMod.Instance?.BeginWorldLoad(nameof(LoadManager.LoadLastSave));
        }
    }

    [HarmonyPatch(typeof(LoadManager), nameof(LoadManager.LoadAsClient))]
    internal static class LoadManagerLoadAsClientPatch
    {
        private static void Prefix()
        {
            MainMod.Instance?.BeginWorldLoad(nameof(LoadManager.LoadAsClient));
        }
    }

    [HarmonyPatch(typeof(LoadManager), nameof(LoadManager.LoadTutorialAsClient))]
    internal static class LoadManagerLoadTutorialAsClientPatch
    {
        private static void Prefix()
        {
            MainMod.Instance?.BeginWorldLoad(nameof(LoadManager.LoadTutorialAsClient));
        }
    }

    [HarmonyPatch(typeof(LoadManager), nameof(LoadManager.ExitToMenu))]
    internal static class LoadManagerExitToMenuPatch
    {
        private static void Prefix(SaveInfo autoLoadSave, MainMenuPopup.Data mainMenuPopup, bool preventLeaveLobby)
        {
            MainMod.Instance?.LeaveWorld(nameof(LoadManager.ExitToMenu));
        }
    }

    [HarmonyPatch(typeof(LoadManager), "set_IsGameLoaded")]
    internal static class LoadManagerSetIsGameLoadedPatch
    {
        private static void Postfix(bool value)
        {
            MainMod.Instance?.SetWorldLoaded(value, nameof(LoadManager.IsGameLoaded));
        }
    }
}
