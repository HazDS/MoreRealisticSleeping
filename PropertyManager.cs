using Il2CppScheduleOne;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Effects;
using MelonLoader;
using MoreRealisticSleeping;
using MoreRealisticSleeping.Config;
using MoreRealisticSleeping.Util;
using UnityEngine;
using System.Collections;
using Il2CppLiquidVolumeFX;
using Il2CppScheduleOne.Networking;
using Il2CppFluffyUnderware.DevTools.Extensions;
using Il2CppJetBrains.Annotations;

public class PropertyManager
{
    private readonly Dictionary<string, Action<Player>> negativeProperties;
    private readonly Dictionary<string, Action<Player>> positiveProperties;
    public string appliedEffect = null;
    public PropertyManager()

    {
        positiveProperties = new Dictionary<string, Action<Player>>
        {
            { "Anti_Gravity", player => ScriptableObject.CreateInstance<AntiGravity>().ApplyToPlayer(player) },
            { "Athletic", player => ScriptableObject.CreateInstance<Athletic>().ApplyToPlayer(player) },
            { "Bright_Eyed", player => ScriptableObject.CreateInstance<BrightEyed>().ApplyToPlayer(player) },
            { "Calming", player => ScriptableObject.CreateInstance<Calming>().ApplyToPlayer(player) },
            { "Calorie_Dense", player => ScriptableObject.CreateInstance<CalorieDense>().ApplyToPlayer(player) },
            { "Electrifying", player => ScriptableObject.CreateInstance<Electrifying>().ApplyToPlayer(player) },
            { "Energizing", player => ScriptableObject.CreateInstance<Energizing>().ApplyToPlayer(player) },
            { "Euphoric", player => ScriptableObject.CreateInstance<Euphoric>().ApplyToPlayer(player) },
            { "Focused", player => ScriptableObject.CreateInstance<Focused>().ApplyToPlayer(player) },
            { "Munchies", player => ScriptableObject.CreateInstance<Munchies>().ApplyToPlayer(player) },
            { "Refreshing", player => ScriptableObject.CreateInstance<Refreshing>().ApplyToPlayer(player) },
            { "Sneaky", player => ScriptableObject.CreateInstance<Sneaky>().ApplyToPlayer(player) },
        };

        negativeProperties = new Dictionary<string, Action<Player>>
        {
            { "Balding", player => ScriptableObject.CreateInstance<Balding>().ApplyToPlayer(player) },
            { "Bright_Eyed", player => ScriptableObject.CreateInstance<BrightEyed>().ApplyToPlayer(player) },
            { "Calming", player => ScriptableObject.CreateInstance<Calming>().ApplyToPlayer(player) },
            { "Calorie_Dense", player => ScriptableObject.CreateInstance<CalorieDense>().ApplyToPlayer(player) },
            { "Cyclopean", player => ScriptableObject.CreateInstance<Cyclopean>().ApplyToPlayer(player) },
            { "Disorienting", player => ScriptableObject.CreateInstance<Disorienting>().ApplyToPlayer(player) },
            { "Electrifying", player => ScriptableObject.CreateInstance<Electrifying>().ApplyToPlayer(player) },
            { "Explosive", player => ScriptableObject.CreateInstance<Explosive>().ApplyToPlayer(player) },
            { "Foggy", player => ScriptableObject.CreateInstance<Foggy>().ApplyToPlayer(player) },
            { "Gingeritis", player => ScriptableObject.CreateInstance<Gingeritis>().ApplyToPlayer(player) },
            { "Glowing", player => ScriptableObject.CreateInstance<Glowie>().ApplyToPlayer(player) },
            { "Laxative", player => ScriptableObject.CreateInstance<Laxative>().ApplyToPlayer(player) },
            { "Long_Faced", player => ScriptableObject.CreateInstance<LongFaced>().ApplyToPlayer(player) },
            { "Paranoia", player => ScriptableObject.CreateInstance<Paranoia>().ApplyToPlayer(player) },
            { "Sedating", player => ScriptableObject.CreateInstance<Sedating>().ApplyToPlayer(player) },
            { "Seizure_Inducing", player => ScriptableObject.CreateInstance<Seizure>().ApplyToPlayer(player) },
            { "Shrinking", player => ScriptableObject.CreateInstance<Shrinking>().ApplyToPlayer(player) },
            { "Slippery", player => ScriptableObject.CreateInstance<Slippery>().ApplyToPlayer(player) },
            { "Smelly", player => ScriptableObject.CreateInstance<Smelly>().ApplyToPlayer(player) },
            { "Spicy", player => ScriptableObject.CreateInstance<Spicy>().ApplyToPlayer(player) },
            { "Lethal", player => ScriptableObject.CreateInstance<Lethal>().ApplyToPlayer(player) },
            { "Jennerising", player => ScriptableObject.CreateInstance<Jennerising>().ApplyToPlayer(player) },
            { "Schizophrenic", player => ScriptableObject.CreateInstance<Schizophrenic>().ApplyToPlayer(player) },
            { "Thought_Provoking", player => ScriptableObject.CreateInstance<ThoughtProvoking>().ApplyToPlayer(player) },
            { "Toxic", player => ScriptableObject.CreateInstance<Toxic>().ApplyToPlayer(player) },
            { "Tropic_Thunder", player => ScriptableObject.CreateInstance<TropicThunder>().ApplyToPlayer(player) },
            { "Zombifying", player => ScriptableObject.CreateInstance<Zombifying>().ApplyToPlayer(player) }
        };
    }

    public void ApplyNegativePropertyToPlayer(Player player, string propertyName = null)
    {
        if (player == null)
        {
            MelonLogger.Warning("Player is null. Cannot apply property.");
            return;
        }

        if (MRSCore.Instance.config == null || MRSCore.Instance.config.EffectSettings == null)
        {
            MelonLogger.Warning("Config or EffectSettings is null. Cannot apply property.");
            return;
        }

        try
        {
            // Wenn keine Property angegeben ist, wähle eine zufällige aktivierte Property
            if (string.IsNullOrEmpty(propertyName))
            {
                var enabledProperties = new List<string>();

                foreach (string property in negativeProperties.Keys)
                {
                    // Verwende GetNegativePropertyValue, um den Wert der Property zu prüfen
                    if (ConfigManager.GetNegativePropertyValue(MRSCore.Instance.config, property))
                    {
                        enabledProperties.Add(property);
                    }
                }

                if (enabledProperties.Count == 0)
                {
                    MelonLogger.Warning("No enabled negative properties found in the config.");
                    return;
                }

                // Wähle eine zufällige Property aus den aktivierten Properties
                var random = new System.Random();
                propertyName = enabledProperties[random.Next(enabledProperties.Count)];
                // MelonLogger.Msg($"No property specified. Randomly selected '{propertyName}'.");
            }

            // Überprüfen, ob die Property in der Config aktiviert ist
            if (!ConfigManager.GetNegativePropertyValue(MRSCore.Instance.config, propertyName))
            {
                MelonLogger.Msg($"Property '{propertyName}' is disabled in the config. Skipping.");
                return;
            }

            // Überprüfen, ob bereits eine Property angewendet wurde
            if (appliedEffect != null)
            {
                //MelonLogger.Msg($"Property '{appliedEffect}' is already applied to player: {player.name}. Skipping.");
                return;
            }

            // Wende die Property mit AddPropertyToPlayer an
            MelonCoroutines.Start(AddPropertyToPlayer(player, propertyName));
            MelonLogger.Msg($"Applied '{propertyName}' to player: {player.name}");
            appliedEffect = propertyName; // Store the applied effect

            float duration = MRSCore.Instance.config.SleepSettings.Negative_Effects_Duration;
            if (duration <= 0f || float.IsNaN(duration))
            {
                duration = 60f;
            }

            if (MRSCore.Instance.config.SleepSettings.Enable_Effect_Notifications)
            {
                string displayName = FormatPropertyName(propertyName);
                Sprite sprite = LoadSpriteFromUserData(displayName);
                MRSCore.Instance.notificationsManager.SendNotification("Negative Effect", $"{displayName}", sprite, duration, true);
            }

            removeEffectCoroutine = (Coroutine)MelonCoroutines.Start(RemovePropertyFromPlayerAfterTime(player, duration, appliedEffect));
        }
        catch (Exception ex)
        {
            MelonLogger.Error("Error while applying property: " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    public void ApplyPositivePropertyToPlayer(Player player, string propertyName = null)
    {
        if (player == null)
        {
            MelonLogger.Warning("Player is null. Cannot apply property.");
            return;
        }

        if (MRSCore.Instance.config == null || MRSCore.Instance.config.EffectSettings == null)
        {
            MelonLogger.Warning("Config or EffectSettings is null. Cannot apply property.");
            return;
        }

        try
        {
            // Wenn keine Property angegeben ist, wähle eine zufällige aktivierte Property
            if (string.IsNullOrEmpty(propertyName))
            {
                var enabledProperties = new List<string>();

                foreach (string property in positiveProperties.Keys)
                {
                    // Verwende GetPositivePropertyValue, um den Wert der Property zu prüfen
                    if (ConfigManager.GetPositivePropertyValue(MRSCore.Instance.config, property))
                    {
                        enabledProperties.Add(property);
                    }
                }

                if (enabledProperties.Count == 0)
                {
                    MelonLogger.Warning("No enabled positive properties found in the config.");
                    return;
                }

                // Wähle eine zufällige Property aus den aktivierten Properties
                var random = new System.Random();
                propertyName = enabledProperties[random.Next(enabledProperties.Count)];
                // MelonLogger.Msg($"No property specified. Randomly selected '{propertyName}'.");
            }

            // Überprüfen, ob die Property in der Config aktiviert ist
            if (!ConfigManager.GetPositivePropertyValue(MRSCore.Instance.config, propertyName))
            {
                MelonLogger.Warning($"Property '{propertyName}' is disabled in the config. Skipping.");
                return;
            }

            // Überprüfen, ob bereits eine Property angewendet wurde
            if (appliedEffect != null)
            {
                MelonLogger.Warning($"Property '{appliedEffect}' is already applied to player: {player.name}. Skipping.");
                return;
            }

            // Wende die Property mit AddPropertyToPlayer an
            MelonCoroutines.Start(AddPropertyToPlayer(player, propertyName));
            appliedEffect = propertyName; // Store the applied effect

            float duration = MRSCore.Instance.config.SleepSettings.Positive_Effects_Duration;
            if (duration <= 0f || float.IsNaN(duration))
            {
                duration = 60f;
            }

            MelonLogger.Msg($"Applied {propertyName} to player: {player.name} for {duration} seconds.");
            if (MRSCore.Instance.config.SleepSettings.Enable_Effect_Notifications)
            {
                string displayName = FormatPropertyName(propertyName);
                Sprite sprite = LoadSpriteFromUserData(displayName);
                MRSCore.Instance.notificationsManager.SendNotification("Positive Effect", $"{displayName}", sprite, duration, true);
            }

            MelonCoroutines.Start(RemovePropertyFromPlayerAfterTime(player, duration, appliedEffect));
        }
        catch (Exception ex)
        {
            MelonLogger.Error("Error while applying property: " + ex.Message + "\n" + ex.StackTrace);
        }
    }


    public static string FormatPropertyName(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return string.Empty;
        }

        // Ersetze Unterstriche durch Leerzeichen
        string formattedName = propertyName.Replace("_", " ");

        // Konvertiere den ersten Buchstaben jedes Wortes in Großbuchstaben
        formattedName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(formattedName.ToLower());

        return formattedName;
    }
    public IEnumerator RemovePropertyFromPlayerAfterTime(Player player, float timeInSeconds, string propertyName)
    {
        if (player == null)
        {
            MelonLogger.Warning("Player is null. Cannot remove property.");
            yield break;
        }
        //MelonLogger.Msg($"Coroutine started for player: {player.name}. Waiting for {timeInSeconds} seconds.");
        yield return new WaitForSeconds(timeInSeconds);
        appliedEffect = null;
        switch (propertyName.ToLower())
        {
            case "antigravitiy":
            case "anti_gravity":
                if (antiGravity == null) antiGravity = ScriptableObject.CreateInstance<AntiGravity>();
                if (antiGravity != null) antiGravity.ClearFromPlayer(player);
                break;
            case "athletic":
                if (athletic == null) athletic = ScriptableObject.CreateInstance<Athletic>();
                if (athletic != null) athletic.ClearFromPlayer(player);
                break;
            case "balding":
                if (balding == null) balding = ScriptableObject.CreateInstance<Balding>();
                if (balding != null) balding.ClearFromPlayer(player);
                break;
            case "brighteyed":
            case "bright_eyed":
                if (brightEyed == null) brightEyed = ScriptableObject.CreateInstance<BrightEyed>();
                if (brightEyed != null) brightEyed.ClearFromPlayer(player);
                break;
            case "calming":
                if (calming == null) calming = ScriptableObject.CreateInstance<Calming>();
                if (calming != null) calming.ClearFromPlayer(player);
                break;
            case "caloriedense":
            case "calorie_dense":
                if (calorieDense == null) calorieDense = ScriptableObject.CreateInstance<CalorieDense>();
                if (calorieDense != null) calorieDense.ClearFromPlayer(player);
                break;
            case "cyclopean":
                if (cyclopean == null) cyclopean = ScriptableObject.CreateInstance<Cyclopean>();
                if (cyclopean != null) cyclopean.ClearFromPlayer(player);
                break;
            case "disorienting":
                if (disorienting == null) disorienting = ScriptableObject.CreateInstance<Disorienting>();
                if (disorienting != null) disorienting.ClearFromPlayer(player);
                break;
            case "electrifying":
                if (electrifying == null) electrifying = ScriptableObject.CreateInstance<Electrifying>();
                if (electrifying != null) electrifying.ClearFromPlayer(player);
                break;
            case "energizing":
                if (energizing == null) energizing = ScriptableObject.CreateInstance<Energizing>();
                if (energizing != null) energizing.ClearFromPlayer(player);
                break;
            case "euphoric":
                if (euphoric == null) euphoric = ScriptableObject.CreateInstance<Euphoric>();
                if (euphoric != null) euphoric.ClearFromPlayer(player);
                break;
            case "explosive":
                if (explosive == null) explosive = ScriptableObject.CreateInstance<Explosive>();
                if (explosive != null) explosive.ClearFromPlayer(player);
                break;
            case "foggy":
                if (foggy == null) foggy = ScriptableObject.CreateInstance<Foggy>();
                if (foggy != null) foggy.ClearFromPlayer(player);
                break;
            case "focused":
                if (focused == null) focused = ScriptableObject.CreateInstance<Focused>();
                if (focused != null) focused.ClearFromPlayer(player);
                break;
            case "gingeritis":
                if (gingeritis == null) gingeritis = ScriptableObject.CreateInstance<Gingeritis>();
                if (gingeritis != null) gingeritis.ClearFromPlayer(player);
                break;
            case "glowing":
            case "glowie":
                if (glowie == null) glowie = ScriptableObject.CreateInstance<Glowie>();
                if (glowie != null) glowie.ClearFromPlayer(player);
                break;
            case "jennerising":
                if (jennerising == null) jennerising = ScriptableObject.CreateInstance<Jennerising>();
                if (jennerising != null) jennerising.ClearFromPlayer(player);
                break;
            case "laxative":
                if (laxative == null) laxative = ScriptableObject.CreateInstance<Laxative>();
                if (laxative != null) laxative.ClearFromPlayer(player);
                break;
            case "lethal":
                if (lethal == null) lethal = ScriptableObject.CreateInstance<Lethal>();
                if (lethal != null) lethal.ClearFromPlayer(player);
                break;
            case "longfaced":
            case "long_faced":
                if (longFaced == null) longFaced = ScriptableObject.CreateInstance<LongFaced>();
                if (longFaced != null) longFaced.ClearFromPlayer(player);
                break;
            case "munchies":
                if (munchies == null) munchies = ScriptableObject.CreateInstance<Munchies>();
                if (munchies != null) munchies.ClearFromPlayer(player);
                break;
            case "paranoia":
                if (paranoia == null) paranoia = ScriptableObject.CreateInstance<Paranoia>();
                if (paranoia != null) paranoia.ClearFromPlayer(player);
                break;
            case "refreshing":
                if (refreshing == null) refreshing = ScriptableObject.CreateInstance<Refreshing>();
                if (refreshing != null) refreshing.ClearFromPlayer(player);
                break;
            case "schizophrenic":
                if (schizophrenic == null) schizophrenic = ScriptableObject.CreateInstance<Schizophrenic>();
                if (schizophrenic != null) schizophrenic.ClearFromPlayer(player);
                break;
            case "sedating":
                if (sedating == null) sedating = ScriptableObject.CreateInstance<Sedating>();
                if (sedating != null) sedating.ClearFromPlayer(player);
                break;
            case "seizure_inducing":
            case "seizureinducing":
            case "seizure":
                if (seizure == null) seizure = ScriptableObject.CreateInstance<Seizure>();
                if (seizure != null) seizure.ClearFromPlayer(player);
                break;
            case "shrinking":
                if (shrinking == null) shrinking = ScriptableObject.CreateInstance<Shrinking>();
                if (shrinking != null) shrinking.ClearFromPlayer(player);
                break;
            case "slippery":
                if (slippery == null) slippery = ScriptableObject.CreateInstance<Slippery>();
                if (slippery != null) slippery.ClearFromPlayer(player);
                break;
            case "smelly":
                if (smelly == null) smelly = ScriptableObject.CreateInstance<Smelly>();
                if (smelly != null) smelly.ClearFromPlayer(player);
                break;
            case "sneaky":
                if (sneaky == null) sneaky = ScriptableObject.CreateInstance<Sneaky>();
                if (sneaky != null) sneaky.ClearFromPlayer(player);
                break;
            case "spicy":
                if (spicy == null) spicy = ScriptableObject.CreateInstance<Spicy>();
                if (spicy != null) spicy.ClearFromPlayer(player);
                break;
            case "thoughtprovoking":
            case "thought_provoking":
                if (thoughtProvoking == null) thoughtProvoking = ScriptableObject.CreateInstance<ThoughtProvoking>();
                if (thoughtProvoking != null) thoughtProvoking.ClearFromPlayer(player);
                break;
            case "toxic":
                if (toxic == null) toxic = ScriptableObject.CreateInstance<Toxic>();
                if (toxic != null) toxic.ClearFromPlayer(player);
                break;
            case "tropicthunder":
            case "tropic_thunder":
                if (tropicThunder == null) tropicThunder = ScriptableObject.CreateInstance<TropicThunder>();
                if (tropicThunder != null) tropicThunder.ClearFromPlayer(player);
                break;
            case "zombifying":
                if (zombifying == null) zombifying = ScriptableObject.CreateInstance<Zombifying>();
                if (zombifying != null) zombifying.ClearFromPlayer(player);
                break;
            default:
                MelonLogger.Warning($"Property '{propertyName}' not found in the available properties.");
                break;
        }
    }

    public IEnumerator AddPropertyToPlayer(Player player, string propertyName)
    {
        if (player == null)
        {
            MelonLogger.Warning("Player is null. Cannot remove property.");
            yield break;
        }

        if (propertyName == null)
        {
            MelonLogger.Warning("Property name is null. Cannot remove property.");
            yield break;
        }

        switch (propertyName.ToLower())
        {
            case "antigravitiy":
            case "anti_gravity":
                if (antiGravity == null) antiGravity = ScriptableObject.CreateInstance<AntiGravity>();
                if (antiGravity != null) antiGravity.ApplyToPlayer(player);
                break;
            case "athletic":
                if (athletic == null) athletic = ScriptableObject.CreateInstance<Athletic>();
                if (athletic != null) athletic.ApplyToPlayer(player);
                break;
            case "balding":
                if (balding == null) balding = ScriptableObject.CreateInstance<Balding>();
                if (balding != null) balding.ApplyToPlayer(player);
                break;
            case "brighteyed":
            case "bright_eyed":
                if (brightEyed == null) brightEyed = ScriptableObject.CreateInstance<BrightEyed>();
                if (brightEyed != null) brightEyed.ApplyToPlayer(player);
                break;
            case "calming":
                if (calming == null) calming = ScriptableObject.CreateInstance<Calming>();
                if (calming != null) calming.ApplyToPlayer(player);
                break;
            case "caloriedense":
            case "calorie_dense":
                if (calorieDense == null) calorieDense = ScriptableObject.CreateInstance<CalorieDense>();
                if (calorieDense != null) calorieDense.ApplyToPlayer(player);
                break;
            case "cyclopean":
                if (cyclopean == null) cyclopean = ScriptableObject.CreateInstance<Cyclopean>();
                if (cyclopean != null) cyclopean.ApplyToPlayer(player);
                break;
            case "disorienting":
                if (disorienting == null) disorienting = ScriptableObject.CreateInstance<Disorienting>();
                if (disorienting != null) disorienting.ApplyToPlayer(player);
                break;
            case "electrifying":
                if (electrifying == null) electrifying = ScriptableObject.CreateInstance<Electrifying>();
                if (electrifying != null) electrifying.ApplyToPlayer(player);
                break;
            case "energizing":
                if (energizing == null) energizing = ScriptableObject.CreateInstance<Energizing>();
                if (energizing != null) energizing.ApplyToPlayer(player);
                break;
            case "euphoric":
                if (euphoric == null) euphoric = ScriptableObject.CreateInstance<Euphoric>();
                if (euphoric != null) euphoric.ApplyToPlayer(player);
                break;
            case "explosive":
                if (explosive == null) explosive = ScriptableObject.CreateInstance<Explosive>();
                if (explosive != null) explosive.ApplyToPlayer(player);
                break;
            case "foggy":
                if (foggy == null) foggy = ScriptableObject.CreateInstance<Foggy>();
                if (foggy != null) foggy.ApplyToPlayer(player);
                break;
            case "focused":
                if (focused == null) focused = ScriptableObject.CreateInstance<Focused>();
                if (focused != null) focused.ApplyToPlayer(player);
                break;
            case "gingeritis":
                if (gingeritis == null) gingeritis = ScriptableObject.CreateInstance<Gingeritis>();
                if (gingeritis != null) gingeritis.ApplyToPlayer(player);
                break;
            case "glowing":
            case "glowie":
                if (glowie == null) glowie = ScriptableObject.CreateInstance<Glowie>();
                if (glowie != null) glowie.ApplyToPlayer(player);
                break;
            case "jennerising":
                if (jennerising == null) jennerising = ScriptableObject.CreateInstance<Jennerising>();
                if (jennerising != null) jennerising.ApplyToPlayer(player);
                break;
            case "laxative":
                if (laxative == null) laxative = ScriptableObject.CreateInstance<Laxative>();
                if (laxative != null) laxative.ApplyToPlayer(player);
                break;
            case "lethal":
                if (lethal == null) lethal = ScriptableObject.CreateInstance<Lethal>();
                if (lethal != null) lethal.ApplyToPlayer(player);
                break;
            case "longfaced":
            case "long_faced":
                if (longFaced == null) longFaced = ScriptableObject.CreateInstance<LongFaced>();
                if (longFaced != null) longFaced.ApplyToPlayer(player);
                break;
            case "munchies":
                if (munchies == null) munchies = ScriptableObject.CreateInstance<Munchies>();
                if (munchies != null) munchies.ApplyToPlayer(player);
                break;
            case "paranoia":
                if (paranoia == null) paranoia = ScriptableObject.CreateInstance<Paranoia>();
                if (paranoia != null) paranoia.ApplyToPlayer(player);
                break;
            case "refreshing":
                if (refreshing == null) refreshing = ScriptableObject.CreateInstance<Refreshing>();
                if (refreshing != null) refreshing.ApplyToPlayer(player);
                break;
            case "schizophrenic":
                if (schizophrenic == null) schizophrenic = ScriptableObject.CreateInstance<Schizophrenic>();
                if (schizophrenic != null) schizophrenic.ApplyToPlayer(player);
                break;
            case "sedating":
                if (sedating == null) sedating = ScriptableObject.CreateInstance<Sedating>();
                if (sedating != null) sedating.ApplyToPlayer(player);
                break;
            case "seizure_inducing":
            case "seizureinducing":
            case "seizure":
                if (seizure == null) seizure = ScriptableObject.CreateInstance<Seizure>();
                if (seizure != null) seizure.ApplyToPlayer(player);
                break;
            case "shrinking":
                if (shrinking == null) shrinking = ScriptableObject.CreateInstance<Shrinking>();
                if (shrinking != null) shrinking.ApplyToPlayer(player);
                break;
            case "slippery":
                if (slippery == null) slippery = ScriptableObject.CreateInstance<Slippery>();
                if (slippery != null) slippery.ApplyToPlayer(player);
                break;
            case "smelly":
                if (smelly == null) smelly = ScriptableObject.CreateInstance<Smelly>();
                if (smelly != null) smelly.ApplyToPlayer(player);
                break;
            case "sneaky":
                if (sneaky == null) sneaky = ScriptableObject.CreateInstance<Sneaky>();
                if (sneaky != null) sneaky.ApplyToPlayer(player);
                break;
            case "spicy":
                if (spicy == null) spicy = ScriptableObject.CreateInstance<Spicy>();
                if (spicy != null) spicy.ApplyToPlayer(player);
                break;
            case "thoughtprovoking":
            case "thought_provoking":
                if (thoughtProvoking == null) thoughtProvoking = ScriptableObject.CreateInstance<ThoughtProvoking>();
                if (thoughtProvoking != null) thoughtProvoking.ApplyToPlayer(player);
                break;
            case "toxic":
                if (toxic == null) toxic = ScriptableObject.CreateInstance<Toxic>();
                if (toxic != null) toxic.ApplyToPlayer(player);
                break;
            case "tropicthunder":
            case "tropic_thunder":
                if (tropicThunder == null) tropicThunder = ScriptableObject.CreateInstance<TropicThunder>();
                if (tropicThunder != null) tropicThunder.ApplyToPlayer(player);
                break;
            case "zombifying":
                if (zombifying == null) zombifying = ScriptableObject.CreateInstance<Zombifying>();
                if (zombifying != null) zombifying.ApplyToPlayer(player);
                break;
            default:
                MelonLogger.Warning($"Property '{propertyName}' not found in the available properties.");
                break;
        }
    }
    static Sprite LoadSpriteFromUserData(string fileName)
    {
        // Load effect sprite from embedded resources
        Sprite sprite = EmbeddedAssets.LoadEffectSprite(fileName);
        if (sprite != null)
        {
            return sprite;
        }

        // Fallback to app icon if effect not found
        if (appIconSprite == null)
        {
            appIconSprite = EmbeddedAssets.LoadAppIcon();
        }
        return appIconSprite;
    }

    public AntiGravity antiGravity;
    public Athletic athletic;
    public Balding balding;
    public BrightEyed brightEyed;
    public Calming calming;
    public CalorieDense calorieDense;
    public Cyclopean cyclopean;
    public Disorienting disorienting;
    public Electrifying electrifying;
    public Energizing energizing;
    public Euphoric euphoric;
    public Explosive explosive;
    public Focused focused;
    public Foggy foggy;
    public Gingeritis gingeritis;
    public Glowie glowie;
    public Jennerising jennerising;
    public Laxative laxative;
    public Lethal lethal;
    public LongFaced longFaced;
    public Munchies munchies;
    public Paranoia paranoia;
    public Refreshing refreshing;
    public Schizophrenic schizophrenic;
    public Sedating sedating;
    public Seizure seizure;
    public Shrinking shrinking;
    public Slippery slippery;
    public Smelly smelly;
    public Sneaky sneaky;
    public Spicy spicy;
    public ThoughtProvoking thoughtProvoking;
    public Toxic toxic;
    public TropicThunder tropicThunder;
    public Zombifying zombifying;
    public static Sprite appIconSprite;
    public Coroutine removeEffectCoroutine = null;


}