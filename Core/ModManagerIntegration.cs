using System;
using System.Linq;
using System.Reflection;
using MelonLoader;

namespace ConfigurableMoreEmployees
{
    internal sealed class ModManagerIntegration
    {
        private readonly MelonLogger.Instance logger;
        private EventInfo phonePreferencesSavedEvent;
        private EventInfo menuPreferencesSavedEvent;
        private Delegate phonePreferencesSavedHandler;
        private Delegate menuPreferencesSavedHandler;

        internal ModManagerIntegration(MelonLogger.Instance logger)
        {
            this.logger = logger;
        }

        internal void Subscribe(Action onSettingsSaved)
        {
            if (!MelonBase.RegisteredMelons.Any(mod => mod?.Info?.Name == "Mod Manager & Phone App"))
            {
                logger.Msg("Mod Manager - Phone App was not found. Settings will still work through MelonPreferences.cfg.");
                return;
            }

            try
            {
                var eventsType = FindEventsType();
                if (eventsType == null)
                {
                    logger.Warning("Mod Manager - Phone App was found, but its settings events type was unavailable.");
                    return;
                }

                phonePreferencesSavedEvent = eventsType.GetEvent(
                    "OnPhonePreferencesSaved",
                    BindingFlags.Public | BindingFlags.Static);
                menuPreferencesSavedEvent = eventsType.GetEvent(
                    "OnMenuPreferencesSaved",
                    BindingFlags.Public | BindingFlags.Static);

                phonePreferencesSavedHandler = SubscribeToEvent(phonePreferencesSavedEvent, onSettingsSaved);
                menuPreferencesSavedHandler = SubscribeToEvent(menuPreferencesSavedEvent, onSettingsSaved);

                if (phonePreferencesSavedHandler == null && menuPreferencesSavedHandler == null)
                {
                    logger.Warning("Mod Manager - Phone App was found, but no compatible save events were available.");
                    return;
                }

                logger.Msg("Subscribed to Mod Manager - Phone App save events.");
            }
            catch (Exception ex)
            {
                logger.Error($"Could not subscribe to Mod Manager - Phone App events: {ex}");
            }
        }

        internal void Unsubscribe()
        {
            try
            {
                phonePreferencesSavedEvent?.RemoveEventHandler(null, phonePreferencesSavedHandler);
                menuPreferencesSavedEvent?.RemoveEventHandler(null, menuPreferencesSavedHandler);
            }
            catch
            {
            }
            finally
            {
                phonePreferencesSavedEvent = null;
                menuPreferencesSavedEvent = null;
                phonePreferencesSavedHandler = null;
                menuPreferencesSavedHandler = null;
            }
        }

        private static Type FindEventsType()
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

        private static Delegate SubscribeToEvent(EventInfo eventInfo, Action onSettingsSaved)
        {
            if (eventInfo?.EventHandlerType == null)
            {
                return null;
            }

            var handler = Delegate.CreateDelegate(
                eventInfo.EventHandlerType,
                onSettingsSaved.Target,
                onSettingsSaved.Method);
            eventInfo.AddEventHandler(null, handler);
            return handler;
        }
    }
}
