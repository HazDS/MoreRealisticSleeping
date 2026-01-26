using MelonLoader;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Il2CppScheduleOne.PlayerScripts;
using MoreRealisticSleeping.Config;
using Il2CppScheduleOne.Money;
using UnityEngine.UI;
using MoreRealisticSleeping.Util;

[assembly: MelonInfo(typeof(MoreRealisticSleeping.MRSCore), "MoreRealisticSleeping (Forked by HazDS)", "1.0.6", "KampfBallerina", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace MoreRealisticSleeping
{

    /*
    ClampWakeTime(int time) -> 0-2400 Ist die Weckzeit
    TODOS: Notification for warning/reminder ~ 1h bzw 60 sec before sleep
            be aware of delay
    TODOS: Adjust Wakeup time automatically to be always 8 hrs (late bed = late wake)
    
    */

    // 1600 = 16:00 Uhr
    public class MRSCore : MelonMod
    {
        public static MRSCore Instance { get; private set; }
        public bool isLegitVersion = false;
        public bool canTriggerSleep = true;
        private bool isCooldownActive = false;
        private bool isForcedSleep = false;
        private bool isFirstSleep = true;
        public SleepCanvas sleepCanvas;
        public DailySummary dailySummary;
        public TimeManager timeManager = null;
        public ConfigState config = null;
        public NotificationsManager notificationsManager;
        public MoneyManager moneyManager;
        public PropertyManager propertyManager;

        public EventManager.EventManager eventManager;
        public static PhoneApp.SleepingApp sleepingApp = new PhoneApp.SleepingApp();

        public bool isMonitorTimeForSleepRunning = false;
        public bool isStartCooldownRunning = false;
        public Coroutine monitorTimeForSleepCoroutine;
        public Coroutine startCooldownCoroutine;
        public Coroutine forceSleepDelayCoroutine;
        public Coroutine initializeLaunderAppCoroutine;
        public Coroutine waitForSleepingAppCoroutine;
        public Coroutine startAppCoroutinesAfterDelayCoroutine;
        public Coroutine initTimeManagerCoroutine;
        public Coroutine initLocalPlayerCoroutine;
        public Coroutine murderPlayerCoroutine;
        public Coroutine onLocalPlayerInitializedCoroutine;
        public Player localPlayer { get; private set; }
        private bool isFirstStart = true;
        public PlayerCrimeData localPlayerCrimeData { get; private set; }
        private string sSceneName = null;


        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            MRSCore.Instance = this;

        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                isFirstStart = false;
                sSceneName = sceneName.ToString();
                MRSCore.Instance.config = Config.ConfigManager.Load();
                MRSCore.Instance.propertyManager = new PropertyManager();

                MRSCore.Instance.moneyManager = UnityEngine.Object.FindObjectOfType<MoneyManager>();
                MRSCore.Instance.notificationsManager = UnityEngine.Object.FindObjectOfType<NotificationsManager>();
                MRSCore.Instance.eventManager = new EventManager.EventManager();

                if (MRSCore.Instance.config.Use_Legit_Version)
                {
                    LoggerInstance.Msg("Use_Legit_Version is enabled. Adjusting behavior accordingly.");
                    isLegitVersion = true;
                }
                else
                {
                    LoggerInstance.Msg("Use_Legit_Version is disabled. Proceeding with default behavior.");
                    startAppCoroutinesAfterDelayCoroutine = (Coroutine)MelonCoroutines.Start(StartAppCoroutinesAfterDelay());
                }
                initLocalPlayerCoroutine = (Coroutine)MelonCoroutines.Start(InitializeLocalPlayer());
                initTimeManagerCoroutine = (Coroutine)MelonCoroutines.Start(InitTimeManager());

                if (Il2CppScheduleOne.Networking.Lobby.Instance.IsHost)
                {
                    //   LoggerInstance.Msg("Current player is the host. Performing host-specific actions.");

                }
                else
                {
                    //   LoggerInstance.Msg("Current player is a client.");

                }

            }
            else if (sceneName.Equals("Menu", StringComparison.OrdinalIgnoreCase) && !isFirstStart)
            {
                sSceneName = sceneName.ToString();
                //LoggerInstance.Msg("Menu scene loaded. Stopping time monitoring.");
                ResetAllVariables();
                isFirstStart = false;

                if (Il2CppScheduleOne.Networking.Lobby.Instance.IsHost)
                {
                    //  LoggerInstance.Msg("Current player is the host. Performing host-specific actions.");


                }
                else
                {
                    //    LoggerInstance.Msg("Current player is a client.");


                }
            }
        }

        private void ResetAllVariables()
        {
            // Überprüfen und Stoppen der Coroutines mit null setzen
            if (initializeLaunderAppCoroutine != null)
            {
                MelonCoroutines.Stop(initializeLaunderAppCoroutine);
                initializeLaunderAppCoroutine = null;
                LoggerInstance.Msg("Stopped InitializeLaunderApp coroutine.");
            }

            if (waitForSleepingAppCoroutine != null)
            {
                MelonCoroutines.Stop(waitForSleepingAppCoroutine);
                waitForSleepingAppCoroutine = null;
                LoggerInstance.Msg("Stopped WaitForSleepingAppAndCreateEntry coroutine.");
            }

            if (startAppCoroutinesAfterDelayCoroutine != null)
            {
                MelonCoroutines.Stop(startAppCoroutinesAfterDelayCoroutine);
                startAppCoroutinesAfterDelayCoroutine = null;
                LoggerInstance.Msg("Stopped StartAppCoroutinesAfterDelay coroutine.");
            }

            if (initTimeManagerCoroutine != null)
            {
                MelonCoroutines.Stop(initTimeManagerCoroutine);
                initTimeManagerCoroutine = null;
                LoggerInstance.Msg("Stopped InitTimeManager coroutine.");
            }

            if (propertyManager != null && propertyManager.removeEffectCoroutine != null)
            {
                MelonCoroutines.Stop(propertyManager.removeEffectCoroutine);
                propertyManager.removeEffectCoroutine = null;
                LoggerInstance.Msg("Stopped RemoveEffect coroutine.");
            }

            if (murderPlayerCoroutine != null)
            {
                MelonCoroutines.Stop(murderPlayerCoroutine);
                murderPlayerCoroutine = null;
                LoggerInstance.Msg("Stopped MurderPlayer coroutine.");
            }

            StopAllCoroutines();
            MRSCore.Instance.config = null;

            isLegitVersion = false;
            timeManager = null;
            sleepCanvas = null;
            dailySummary = null;
            eventManager = null;
            localPlayer = null;
            isFirstSleep = true;

            isForcedSleep = false;
            canTriggerSleep = true;
            isCooldownActive = false;

            propertyManager = null;
            notificationsManager = null;
            moneyManager = null;

            isMonitorTimeForSleepRunning = false;
            isStartCooldownRunning = false;
            sleepingApp._isSleepingAppLoaded = false;

            LoggerInstance.Msg("All variables reset.");
        }

        public IEnumerator InitTimeManager()
        {
            while (timeManager == null)
            {
                timeManager = GameObject.FindObjectOfType<TimeManager>();
                if (timeManager != null)
                {
                    MRSCore.Instance.sleepCanvas = UnityEngine.Object.FindObjectOfType<SleepCanvas>();
                    if (MRSCore.Instance.sleepCanvas != null && isFirstSleep)
                    {
                        void FuncThatCallsOtherFunc() => TriggerAfterSleepEffect();
                        MRSCore.Instance.sleepCanvas.onSleepEndFade.AddListener((UnityAction)FuncThatCallsOtherFunc);
                        void FuncThatCallsCooldownFunc()
                        {
                            if (MRSCore.Instance.startCooldownCoroutine == null && !Instance.isStartCooldownRunning)
                            {
                                MRSCore.Instance.startCooldownCoroutine = (Coroutine)MelonCoroutines.Start(StartCooldown());
                                //LoggerInstance.Msg("StartCooldown coroutine started from listener.");
                            }
                            else
                            {
                                // LoggerInstance.Warning("StartCooldown coroutine is already running. Skipping.");
                            }
                        }
                        MRSCore.Instance.sleepCanvas.onSleepEndFade.AddListener((UnityAction)FuncThatCallsCooldownFunc);
                        isFirstSleep = false; // Setze isFirstSleep auf false, um die Listener nur einmal hinzuzufügen
                    }
                    monitorTimeForSleepCoroutine = (Coroutine)MelonCoroutines.Start(MonitorTimeForSleep());
                    break;
                }
                yield return new WaitForSeconds(2f);
            }
            
            // UI / DailySummary / Container 
            // UI / LevelUp
            // Il2CppScheduleOne.UI.IPostSleepEvent
             /*
            Todesnachricht hinzufügen -> Dann Respawn Button anzeigen verbessern
            LevelUp Screen Fix?
            */
        }

        private IEnumerator InitializeLocalPlayer()
        {
            MelonLogger.Msg("Searching for local player...");
            while (localPlayer == null)
            {
                localPlayer = GameObject.FindObjectsOfType<Player>()?.FirstOrDefault(player => player.IsLocalPlayer);
                if (localPlayer == null)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            localPlayerCrimeData = localPlayer.CrimeData;
            MelonLogger.Msg($"Local player found: {localPlayer.name}");
            initLocalPlayerCoroutine = null;
        }

        public void StopAllCoroutines()
        {
            if (forceSleepDelayCoroutine != null)
            {
                MelonCoroutines.Stop(forceSleepDelayCoroutine);
                LoggerInstance.Msg("Stopped ForceSleepDelay coroutine.");
                forceSleepDelayCoroutine = null;
            }

            // Überprüfe und stoppe die Coroutine MonitorTimeForSleep
            if (monitorTimeForSleepCoroutine != null)
            {
                MelonCoroutines.Stop(monitorTimeForSleepCoroutine);
                monitorTimeForSleepCoroutine = null;
                isMonitorTimeForSleepRunning = false;
                LoggerInstance.Msg("Stopped MonitorTimeForSleep coroutine.");
            }

            // Überprüfe und stoppe die Coroutine StartCooldown
            if (startCooldownCoroutine != null)
            {
                MelonCoroutines.Stop(startCooldownCoroutine);
                startCooldownCoroutine = null;
                isStartCooldownRunning = false;
                LoggerInstance.Msg("Stopped StartCooldown coroutine.");
            }

            if (initLocalPlayerCoroutine != null)
            {
                MelonCoroutines.Stop(initLocalPlayerCoroutine);
                initLocalPlayerCoroutine = null;
                LoggerInstance.Msg("Stopped InitializeLocalPlayer coroutine.");
            }

            isForcedSleep = false;
            canTriggerSleep = true;
            isCooldownActive = false;

            //LoggerInstance.Msg("All coroutines have been checked and stopped if running.");
        }

        public IEnumerator MonitorTimeForSleep()
        {
            isMonitorTimeForSleepRunning = true;

            LoggerInstance.Msg("Monitoring time for sleep...");
            while (true)
            {
                if (timeManager != null)
                {
                    if (400 <= timeManager.CurrentTime && timeManager.CurrentTime < 650) // 4:00 AM - 6:50 AM
                    {
                        if (canTriggerSleep && !isStartCooldownRunning)
                        {
                            if (MRSCore.Instance.config.SleepSettings.Enable_Forced_Sleep)
                            {
                                // Starte die Verzögerung für erzwungenen Schlaf
                                if (forceSleepDelayCoroutine == null)
                                {
                                    LoggerInstance.Msg("Triggering sleep delay as CurrentTime is 4:00AM.");
                                    forceSleepDelayCoroutine = (Coroutine)MelonCoroutines.Start(ForceSleepDelay());
                                }
                            }
                            else
                            {
                                LoggerInstance.Msg("Forced sleep is disabled in the config.");
                                if (startCooldownCoroutine == null)
                                {
                                    startCooldownCoroutine = (Coroutine)MelonCoroutines.Start(StartCooldown());
                                }
                            }


                        }
                    }
                }
                else
                {
                    LoggerInstance.Warning("TimeManager is null. Waiting for initialization...");
                }

                // Wartezeit zwischen den Überprüfungen
                yield return new WaitForSeconds(5f);
            }
        }

        public IEnumerator StartAppCoroutinesAfterDelay()
        {
            bool delayedInit = false;
            while (!delayedInit && !isLegitVersion)
            {
                //  MelonLogger.Msg("Waiting 5 Seconds for other mods to load their apps..");
                delayedInit = true;
                yield return new WaitForSeconds(7f);
            }

            if (!isLegitVersion)
            {
                if (initializeLaunderAppCoroutine == null)
                {
                    initializeLaunderAppCoroutine = (Coroutine)MelonCoroutines.Start(MRSCore.sleepingApp.InitializeLaunderApp());
                }
                else
                {
                    LoggerInstance.Msg("InitializeLaunderApp coroutine is already running. Skipping initialization.");
                }
                if (waitForSleepingAppCoroutine == null)
                {
                    waitForSleepingAppCoroutine = (Coroutine)MelonCoroutines.Start(WaitForSleepingAppAndCreateEntry());
                }
                else
                {
                    LoggerInstance.Msg("WaitForSleepingAppAndCreateEntry coroutine is already running. Skipping initialization.");
                }
            }
        }

        private IEnumerator ForceSleepDelay()
        {
            float delayTime = MRSCore.Instance.config.SleepSettings.Forced_Sleep_Delay;

            if (delayTime <= 0f)
            {
                LoggerInstance.Msg("Forced_Sleep_Delay is set to 0 or less. Skipping delay and triggering forced sleep immediately.");
                ForceSleep();
                if (startCooldownCoroutine == null)
                {
                    startCooldownCoroutine = (Coroutine)MelonCoroutines.Start(StartCooldown());
                }
                forceSleepDelayCoroutine = null;
                yield break;
            }

            LoggerInstance.Msg($"Force sleep delay started for {delayTime} seconds.");

            if (localPlayer == null)
            {
                Player[] players = GameObject.FindObjectsOfType<Player>();

                foreach (Player player in players)
                {
                    // Überprüfe, ob der Spieler der lokale Spieler ist
                    if (player.IsLocalPlayer)
                    {
                        localPlayer = player;
                        break;
                    }
                }
            }

            if (localPlayer == null)
            {
                MelonLogger.Error("Local player not found. Cannot proceed with forced sleep.");
                forceSleepDelayCoroutine = null;
                yield break; // Beende die Coroutine, wenn der lokale Spieler nicht gefunden wurde
            }

            float elapsedTime = 0f;
            while (elapsedTime < delayTime)
            {
                // Überprüfen, ob der Spieler bereits schläft
                if (localPlayer.IsSleeping || localPlayer.IsReadyToSleep)
                {
                    LoggerInstance.Msg("Player is already sleeping. Force sleep canceled.");
                    forceSleepDelayCoroutine = null;
                    yield break; // Beende die Coroutine, wenn der Spieler schläft
                }

                elapsedTime += 1f;
                yield return new WaitForSeconds(1f); // Warte bis zum nächsten Frame
            }

            LoggerInstance.Msg("Force sleep delay ended. Triggering forced sleep.");
            ForceSleep(); // Erzwinge den Schlaf nach Ablauf der Verzögerung
            if (startCooldownCoroutine == null)
            {
                startCooldownCoroutine = (Coroutine)MelonCoroutines.Start(StartCooldown());
            }
            forceSleepDelayCoroutine = null;
            yield break;
        }
        private void ForceSleep()
        {
            // Überprüfen, ob der Cooldown aktiv ist
            if (!canTriggerSleep)
            {
                LoggerInstance.Msg("TriggerSleep() is on cooldown. Skipping execution.");
                return;
            }

            try
            {
                // SleepCanvas-Instanz finden, falls sie noch nicht gesetzt ist
                if (MRSCore.Instance.sleepCanvas == null)
                {
                    MRSCore.Instance.sleepCanvas = UnityEngine.Object.FindObjectOfType<SleepCanvas>();
                }

                // Überprüfen, ob SleepCanvas gefunden wurde
                if (MRSCore.Instance.sleepCanvas != null)
                {
                    if (isFirstSleep)
                    {
                        //Check if Listeners are set
                        void FuncThatCallsOtherFunc() => TriggerAfterSleepEffect();
                        MRSCore.Instance.sleepCanvas.onSleepEndFade.AddListener((UnityAction)FuncThatCallsOtherFunc);
                        void FuncThatCallsCooldownFunc()
                        {
                            if (MRSCore.Instance.startCooldownCoroutine == null && !MRSCore.Instance.isStartCooldownRunning)
                            {
                                MRSCore.Instance.startCooldownCoroutine = (Coroutine)MelonCoroutines.Start(StartCooldown());
                            }
                        }
                        MRSCore.Instance.sleepCanvas.onSleepEndFade.AddListener((UnityAction)FuncThatCallsCooldownFunc);
                        isFirstSleep = false; // Setze isFirstSleep auf false, um die Listener nur einmal hinzuzufügen
                    }

                    canTriggerSleep = false; // Cooldown aktivieren
                    MRSCore.Instance.sleepCanvas.SetIsOpen(open: true); // SleepCanvas öffnen
                    MRSCore.Instance.sleepCanvas.SleepStart(); // Schlaf starten  

                    if (MRSCore.Instance.config.SleepSettings.Auto_Skip_Daily_Summary)
                    {
                        MelonCoroutines.Start(DelayDailySummaryPress(3f));
                    }

                    isForcedSleep = true; // Setze isForcedSleep auf true
                    // LoggerInstance.Msg("Sleep forced successfully.");
                }
                else
                {
                    LoggerInstance.Error("Could not find SleepCanvas instance.");
                }
            }
            catch (Exception ex)
            {
                LoggerInstance.Error("Error in TriggerSleep: " + ex.Message + "\n" + ex.StackTrace);

            }

        }

        public IEnumerator DelayDailySummaryPress(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            MRSCore.Instance.dailySummary = UnityEngine.Object.FindObjectOfType<DailySummary>();
            if (MRSCore.Instance.dailySummary != null)
            {
                MRSCore.Instance.dailySummary.transform.Find("Container/Continue").GetComponent<Button>().onClick.Invoke(); // Klicke auf den Button "Continue"
                MRSCore.Instance.dailySummary.SleepEnd(); // Schlaf beenden
                MRSCore.Instance.dailySummary.Close();
                // MelonLogger.Msg("Clicked on Continue button in DailySummary.");
            }
            else
            {
                LoggerInstance.Error("Could not find DailySummary instance.");
            }
        }

        public IEnumerator StartCooldown()
        {
            // Markiere die Coroutine als aktiv
            if (isCooldownActive || isStartCooldownRunning || startCooldownCoroutine != null)
            {
                // LoggerInstance.Msg("Cooldown is already active. Skipping StartCooldown.");
                yield break; // Beende die Coroutine, wenn bereits ein Cooldown läuft
            }

            isStartCooldownRunning = true;
            float cooldownTime = MRSCore.Instance.config.SleepSettings.Cooldown_Time;
            if (cooldownTime <= 10f)
            {
                LoggerInstance.Error("Cooldown time is not set or invalid. Using default (300sec).");
                cooldownTime = 300f; // Standard-Cooldown-Zeit
            }
            isCooldownActive = true; // Cooldown als aktiv markieren
            LoggerInstance.Msg($"Started {cooldownTime}sec cooldown before next sleep can be triggered.");
            canTriggerSleep = false; // Setze die Variable auf false, um den Cooldown zu aktivieren
            yield return new WaitForSeconds(cooldownTime); // Cooldown-Dauer von 300 Sekunden
            canTriggerSleep = true; // Cooldown beenden
            isCooldownActive = false; // Cooldown als beendet markieren
            isStartCooldownRunning = false;
            LoggerInstance.Msg("Cooldown ended. Sleep triggering is now enabled again.");
            startCooldownCoroutine = null; // Setze die Coroutine-Referenz zurück
        }

        private void TriggerAfterSleepEffect()
        {
            //Spieler wurde zum Schlafen gezwungen
            if (isForcedSleep)
            {
                if (MRSCore.Instance.config.SleepSettings.Enable_Negative_Effects)
                {
                    Player[] players = GameObject.FindObjectsOfType<Player>();
                    foreach (Player player in players)
                    {
                        if (player != null && player.gameObject != null)
                        {
                            // Wahrscheinlichkeit für negative Effekte prüfen
                            var probability = MRSCore.Instance.config.SleepSettings.Negative_Effects_Probability / 100f;
                            var random = new System.Random();
                            if (random.NextDouble() <= probability)
                            {
                                propertyManager.ApplyNegativePropertyToPlayer(player);
                            }
                            else
                            {
                                LoggerInstance.Msg("No negative effect applied due to probability.");
                            }
                            isForcedSleep = false;
                        }
                        else
                        {
                            LoggerInstance.Error("Player or Player GameObject is null.");
                        }
                    }
                }
                else
                {
                    LoggerInstance.Msg("Negative effects are disabled in the config.");
                }

                if (MRSCore.Instance.config.ArrestedEventSettings.Enable_GetArrested_Event)
                {

                    if (localPlayer != null && localPlayer.gameObject != null)
                    {
                        // Wahrscheinlichkeit für verhaftet zu werden prüfen
                        var probability = MRSCore.Instance.config.ArrestedEventSettings.GetArrested_Event_Probability / 100f;
                        var random = new System.Random();
                        if (random.NextDouble() <= probability)
                        {
                            if (MRSCore.Instance.config.ArrestedEventSettings.Enable_GetArrested_Event_SaveSpaces && eventManager.IsPlayerNearSaveProperty())
                            {
                                LoggerInstance.Msg("Not arrested due to save space proximity.");
                                return;
                            }
                            else
                            {
                                eventManager.AddNewPublicSleepingCrime();
                                localPlayer.Arrest_Server();
                            }
                        }
                        else
                        {
                            LoggerInstance.Msg("Not arrested due to probability.");
                        }
                        isForcedSleep = false;
                    }
                    else
                    {
                        LoggerInstance.Error("Player or Player GameObject is null.");
                    }
                }
                else
                {
                    LoggerInstance.Msg("Arrested Event is disabled in the config.");
                }
                if (MRSCore.Instance.config.MurderedEventSettings.Enable_GetMurdered_Event)
                {
                    if (localPlayer != null && localPlayer.gameObject != null)
                    {
                        // Wahrscheinlichkeit für ermordet zu werden prüfen
                        var probability = MRSCore.Instance.config.MurderedEventSettings.GetMurdered_Event_Probability / 100f;
                        var random = new System.Random();
                        if (random.NextDouble() <= probability)
                        {
                            if (MRSCore.Instance.config.MurderedEventSettings.Enable_GetMurdered_Event_SaveSpaces && eventManager.IsPlayerNearSaveProperty())
                            {
                                LoggerInstance.Msg("Not murdered due to save space proximity.");
                                return;
                            }
                            else
                            {
                                murderPlayerCoroutine = (Coroutine)MelonCoroutines.Start(eventManager.MurderPlayer());

                            }

                        }
                    }
                }
                else
                {
                    LoggerInstance.Msg("Murdered Event is disabled in the config.");
                }
            }
            else //Spieler ist frühzeitig schlafen gegangen || TODO: Uhrzeit check, um festzulegen, wann früh ist
            {
                if (MRSCore.Instance.config.SleepSettings.Enable_Positive_Effects)
                {
                    Player[] players = GameObject.FindObjectsOfType<Player>();
                    foreach (Player player in players)
                    {
                        if (player != null && player.gameObject != null)
                        {
                            // Wahrscheinlichkeit für positive Effekte prüfen
                            var probability = MRSCore.Instance.config.SleepSettings.Positive_Effects_Probability / 100f;
                            var random = new System.Random();
                            if (random.NextDouble() <= probability)
                            {
                                propertyManager.ApplyPositivePropertyToPlayer(player);
                            }
                            else
                            {
                                LoggerInstance.Msg("No positive effect applied due to probability.");
                            }
                        }
                        else
                        {
                            LoggerInstance.Error("Player or Player GameObject is null.");
                        }
                    }
                }
                else
                {
                    LoggerInstance.Msg("Positive effects are disabled in the config.");
                }
            }
        }
        private IEnumerator WaitForSleepingAppAndCreateEntry()
        {
            while (!MRSCore.sleepingApp._isSleepingAppLoaded)
            {
                yield return new WaitForSeconds(2f); // Warte 2 Sekunde
            }
            CreateAppEntries();
        }

        public void CreateAppEntries()
        {
            if (isLegitVersion)
            {
                return;
            }

            // Überprüfe, ob die Laundering App geladen ist
            if (!MRSCore.sleepingApp._isSleepingAppLoaded)
            {
                if (waitForSleepingAppCoroutine == null)
                {
                    //LoggerInstance.Msg("SleepingApp is not loaded yet. Waiting for it to load.");
                    waitForSleepingAppCoroutine = (Coroutine)MelonCoroutines.Start(WaitForSleepingAppAndCreateEntry());
                    return;
                }
            }
            //Settings
            sleepingApp.AddEntryFromTemplate("GeneralSettingsSection", "General Settings", "~Forced Sleep, Enable/Disable Effects, Probabilitys, Durations~", null, ColorUtil.GetColor("Cyan"), Util.EmbeddedAssets.LoadUIElementSprite("Settings"));

            sleepingApp.AddEntryFromTemplate("PositiveEffectsSection", "Positive Effects", "~Choose positive Early-Sleep-Effects~", null, ColorUtil.GetColor("Cyan"), Util.EmbeddedAssets.LoadUIElementSprite("PositiveEffects"));

            sleepingApp.AddEntryFromTemplate("NegativeEffectsSection", "Negative Effects", "~Choose negative Forced-Sleep-Effects~", null, ColorUtil.GetColor("Cyan"), Util.EmbeddedAssets.LoadUIElementSprite("NegativeEffects"));

            sleepingApp.AddEntryFromTemplate("ArrestedEventSection", "Arrested Event", "~Adjust settings for the Arrested Event~", null, ColorUtil.GetColor("Cyan"), Util.EmbeddedAssets.LoadUIElementSprite("ArrestedEvent"));

            sleepingApp.AddEntryFromTemplate("MurderedEventSection", "Murdered Event", "~Adjust settings for the Murdered Event~", null, ColorUtil.GetColor("Cyan"), Util.EmbeddedAssets.LoadUIElementSprite("MurderedEvent"));

        }

        public void ChangeAppIconImage(GameObject appIcon, Sprite iconSprite)
        {
            if (iconSprite == null)
            {
                LoggerInstance.Msg("Icon sprite is null, skipping image change.");
                return;
            }
            Transform transform = appIcon.transform.Find("Mask/Image");
            GameObject gameObject = (transform != null) ? transform.gameObject : null;
            if (gameObject == null)
            {
                return;
            }
            Image component = gameObject.GetComponent<Image>();
            if (component != null)
            {
                component.sprite = iconSprite;
            }
        }

        public void RegisterApp(GameObject App, string Title = "Unknown App")
        {
            GameObject appIcons = GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIcons");
            App.transform.SetParent(appIcons.transform, worldPositionStays: false);
            ((MelonBase)this).LoggerInstance.Msg("Added " + Title + " to Homescreen.");
        }


    }
}