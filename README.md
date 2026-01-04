# MoreRealisticSleeping

![SleepingAppIcon](https://github.com/user-attachments/assets/8e48292b-4764-4c76-b02f-c211799fedfc)

## 📖 About the Mod
`MoreRealisticSleeping` is a mod for the game *Schedule I* that enhances the sleeping mechanics, making them more realistic and engaging. It introduces features like forced sleep, positive and negative effects, and customizable settings for sleep durations and probabilities.

## ⚠️ Known Incompatibilities / Issues
- **Incompatibility with other mods modifying the sleep system:**  
    Mods that alter the sleep mechanics or the `SleepCanvas` may conflict with `MoreRealisticSleeping`.
- **Multiplayer Effects:**
    The effects are most likely client-side, meaning that it won't be visible in Mutliplayer
    It might not work for Joining Players (please test this yourself)

## 🆕 Whats new in V1.0.4?
- **Updated for Schedule I v0.4.2f9**
    The mod has been updated to work with the latest version of Schedule I (IL2CPP Main Branch).
- **Embedded Assets**
    All images and icons are now embedded directly in the DLL. No more external asset files needed in the UserData folder.
- **API Compatibility Fixes**
    Fixed breaking changes from game updates including namespace migrations and method signature changes.

![Murdered Event](https://github.com/user-attachments/assets/67a83d73-6b28-4ed0-931a-8ec20bff2f72)

![Arrested Event](https://github.com/user-attachments/assets/0c9bc414-60f8-419a-8366-7cbd3d934b59)

![Arrested Crime Note](https://github.com/user-attachments/assets/3c1ddc06-644a-4eba-b654-f6bd50ea0f20)

## 🚀 Planned Features
- **Custom Sleep Times:**  
  Allow players to set forced sleep time and wake-up times
- **Expanded Effect Settings:**  
  Add more granular control over positive and negative effects, including intensity etc.

---

## 🛠️ Features

### Legit Mode

The `Legit Mode` feature ensures a fair and balanced gameplay experience by disabling custom app loading and enforcing stricter rules. This mode is particularly useful for players who want to avoid the temptation of cheating or ensure compatibility with other mods.

- **Enable Legit Mode and Configure:**  
    Use the `MoreRealisticLaundering.json` configuration file to enable `Use_Legit_Version` and set it to `true`.  
    ```json
    {
        "Use_Legit_Version": true
    }
    ```
- **Behavior:**  
    All settings will be loaded on startup using the JSON configuration.
    No custom app will be loaded in-game, ensuring a more authentic experience.
- **Default Value:**  
    The default value for `Use_Legit_Version` is `false`.

This mode may help resolve compatibility issues with certain other mods. Try it out to see if it improves your gameplay experience!

### Multiplayer Support

The `Multiplayer Support` feature ensures a synchronized sleeping experience for all players in a multiplayer session. This feature requires all players to have the mod installed and configured with identical settings.

- **Requirements:**  
    All players in the session must have `MoreRealisticSleeping` installed.  
    The `MoreRealisticSleeping.json` configuration file must be identical for all players to avoid desynchronization.

- **Behavior:**  
    All players are forced to sleep simultaneously during the designated sleep hours.  
    The game resumes once the host player has completed their sleep cycle.  

This feature enhances the cooperative gameplay experience by ensuring that all players adhere to the same sleep mechanics, maintaining balance and immersion in multiplayer sessions.

### Dynamic App Integration

The `Dynamic App Integration` feature allows seamless interaction like other in-game apps, enhancing the overall gameplay experience. This feature ensures that the mod dynamically adapts to changes in the game environment.

![General Settings](https://github.com/user-attachments/assets/d8c150df-53a7-48c1-8bc6-d31124b8e4db)

- **Real-Time Updates:**  
    Changes made to the settings are applied instantly without requiring a game restart.

- **Saved Configurations:**  
    Changes made to the settings are saved instantly to the `MoreRealisticSleeping.json`.
    Adjust the mod to your needs once and forget about it.

  ![Notification Save](https://github.com/user-attachments/assets/20537cf9-3a3f-4bfd-bc9e-f29bd731aa23)

You can also modify these settings before game start via the `MoreRealisticSleeping.json` configuration file when using the `Legit Mode` to suit your preferences and gameplay style.

![App Icon](https://github.com/user-attachments/assets/8e16302d-b54a-45aa-b374-ff1eab92ab6e)

### Post-Sleep Effects

The `Post-Sleep Effects` feature introduces a variety of outcomes after the player wakes up, adding depth and unpredictability to the sleeping mechanics. These effects can be both positive and negative, depending on the player's actions and the configured probabilities.

- **Positive Effects:**  
    - **Choose Positive Effects:**  
        Players can select from a variety of positive effects to be added to the randomized pool.
      
        ![Positive Effects](https://github.com/user-attachments/assets/68c5bb72-14f4-4f92-8e5d-98af21d6a518)

    - **Configurable Duration:**  
        Adjust the duration of positive effects to suit your gameplay style.

    - **Adjustable Probability:**  
        Modify the probability of triggering a positive post-sleep effect.  

    - **Condition for Positive Effects:**  
        Positive effects are only triggered if the player goes to sleep early, encouraging better sleep habits in the game.

- **Negative Effects:**  
    - **Choose Negative Effects:**  
        Players can select from a variety of negative effects to be added to the randomized pool.
      
        ![Negative Effects](https://github.com/user-attachments/assets/c246f726-c5fa-4e8e-9dcf-d670ca757614)

    - **Configurable Duration:**  
        Adjust the duration of negative effects to suit your gameplay style.

    - **Adjustable Probability:**  
        Modify the probability of triggering a negative post-sleep effect.  

    - **Condition for Negative Effects:**  
        Negative effects are only triggered if the player is forced to sleep.

This feature adds an element of strategy to the sleeping mechanics, encouraging players to carefully manage their sleep schedules and actions to maximize benefits while minimizing risks.

### Visual Effect Indicators

The `Visual Effect Indicators` feature enhances the gameplay experience by providing a visual representation of active post-sleep effects. This feature can be toggled on or off based on player preference.

- **Enable Visual Indicators:**  
    Players can enable or disable visual indicators for post-sleep effects via the `MoreRealisticSleeping.json` configuration file or the ingame app.  

- **Behavior:**  
    When enabled, a visual indicator will be displayed on the screen for the entire duration of the active effect.  
    The indicator will vary depending on whether the effect is positive or negative, providing clear feedback to the player.
  
    ![Notification Effect](https://github.com/user-attachments/assets/741f7a93-d061-47f2-a3b4-cec66e2c7219)

- **Configuration Example:**  
    ```json
    {
        "Enable_Effect_Notifications": true, ### When true, the Post-Sleep Effects are visualized by a notification for the whole duration of the effect
    }
    ```

This feature adds an additional layer of immersion and clarity, allowing players to easily track the effects influencing their gameplay.

## ⚙️ Configuration
The mod's settings can be customized via the `MoreRealisticSleeping.json` configuration file, located in the games `UserData` directory or the `SleepDose App`.
```json
{
    "Use_Legit_Version": false, ### When enabled, no ingame app will be generated
    "SleepSettings": {
        "Enable_Forced_Sleep": true, ### When true, the player will be forced to sleep at ~ 4 AM
        "Cooldown_Time": 500, ### Specifies the cooldown before another sleep can be forced - leave this at 500, it's more like a debug feature
        "Forced_Sleep_Delay": 0, ### Specifies the delay after 4AM before Sleep actually is triggered
        "Auto_Skip_Daily_Summary": false, ### Will skip all Sleep Canvas and Daily Summary screens automatically (use as host if afk)
        "Enable_Positive_Effects": true, ### When true, positive Post-Sleep Effects can accur
        "Positive_Effects_Probability": 25,  ### Probability/Change in % for positive effects to accur
        "Positive_Effects_Duration": 60, ### Duration in seconds for positive effects
        "Enable_Negative_Effects": true, ### When true, negative Post-Sleep Effects can accur
        "Negative_Effects_Probability": 50, ### Probability/Change in % for negative effects to accur
        "Negative_Effects_Duration": 60, ### Duration in seconds for negative effects
        "Enable_Effect_Notifications": true ### When true, the Post-Sleep Effects are visualized by a notification for the whole duration of the effect
    },
    "EffectSettings": {
        "PositiveEffectSettings": { ### When true, the related effect is added to the pool of randomzied poistive effects that can happen
            "Anti_Gravity": true,
            "Athletic": true,
            "Bright_Eyed": true,
            "Calming": true,
            "Calorie_Dense": true,
            "Electrifying": true,
            "Energizing": true,
            "Euphoric": true,
            "Focused": true,
            "Munchies": false,
            "Refreshing": false,
            "Sneaky": true
        },
        "NegativeEffectSettings": { ### When true, the related effect is added to the pool of randomzied negative effects that can happen
            "Balding": true,
            "Bright_Eyed": true,
            "Calming": true,
            "Calorie_Dense": true,
            "Cyclopean": true,
            "Disorienting": true,
            "Electrifying": true,
            "Explosive": false,
            "Foggy": true,
            "Gingeritis": true,
            "Glowing": true,
            "Jennerising": true,
            "Laxative": true,
            "Lethal": false,
            "Long_Faced": true,
            "Paranoia": true,
            "Schizophrenic": true,
            "Sedating": true,
            "Seizure_Inducing": true,
            "Shrinking": true,
            "Slippery": true,
            "Smelly": true,
            "Spicy": true,
            "Thought_Provoking": true,
            "Toxic": true,
            "Tropic_Thunder": true,
            "Zombifying": true
        }
    }
}
```
