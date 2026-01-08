using System.Collections;
using Il2CppFluffyUnderware.DevTools.Extensions;
using Il2CppScheduleOne.Dialogue;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Vehicles;
using MelonLoader;
using MelonLoader.Utils;
using MoreRealisticSleeping.Config;
using MoreRealisticSleeping.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoreRealisticSleeping.PhoneApp
{

    public class SleepingApp
    {
        public IEnumerator InitializeLaunderApp()
        {
            while (MRSCore.Instance == null)
            {
                MelonLogger.Msg("Waiting for Instance to be initialized...");
                yield return new WaitForSeconds(1f);
            }

            MelonCoroutines.Start(FontLoader.InitializeFonts());
            while (!Util.FontLoader.openSansBoldIsInitialized || !Util.FontLoader.openSansSemiBoldIsInitialized)
            {
                MelonLogger.Msg("Waiting for Fonts to be loaded...");
                yield return new WaitForSeconds(2f);
            }
            yield return MelonCoroutines.Start(CreateApp("SleepDose", "SleepDose", true, EmbeddedAssets.LoadAppIcon()));
            yield break;
        }

        public IEnumerator CreateApp(string IDName, string Title, bool IsRotated = true, Sprite iconSprite = null)
        {
            GameObject cloningCandidateProducts = null;
            string cloningNameProducts = null;
            GameObject cloningCandidateDeliveries = null;
            string cloningNameDeliveries = null;
            GameObject cloningCandidateProductsCopy = null;
            string cloningNameProductsCopy = null;
            GameObject icons = null;

            // Warte auf das AppIcons-Objekt
            yield return MelonCoroutines.Start(Utils.WaitForObject(
                "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIcons/",
                delegate (GameObject obj)
                {
                    icons = obj;
                }
            ));

            // Bestimme das CloningCandidate basierend auf IsRotated
            if (IsRotated)
            {
                yield return MelonCoroutines.Start(Utils.WaitForObject(
                   "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/DeliveryApp",
                   delegate (GameObject objD)
                   {
                       cloningCandidateDeliveries = objD;
                       cloningNameDeliveries = "Deliveries";
                   }
               ));

                yield return MelonCoroutines.Start(Utils.WaitForObject(
                     "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/ProductManagerApp",
                     delegate (GameObject objP)
                     {
                         cloningCandidateProductsCopy = objP;
                         cloningNameProductsCopy = "Products";
                     }
                 ));

                yield return MelonCoroutines.Start(Utils.WaitForObject(
                    "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/ProductManagerApp",
                    delegate (GameObject obj)
                    {
                        cloningCandidateProducts = obj;
                        cloningNameProducts = "Products";
                    }
                ));
            }

            // Klone das App-Canvas
            GameObject parentCanvas = GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/");

            GameObject deliveriesApp = UnityEngine.Object.Instantiate(cloningCandidateDeliveries, parentCanvas.transform);
            deliveriesApp.name = "DeliveriesTemp";

            GameObject productsApp = UnityEngine.Object.Instantiate(cloningCandidateProductsCopy, parentCanvas.transform);
            deliveriesApp.name = "ProductsTemp";

            GameObject futureSleepApp = UnityEngine.Object.Instantiate(cloningCandidateProducts, parentCanvas.transform);
            futureSleepApp.name = IDName;

            // Aktualisiere den Namen vom App-Icon für das Deliveries Copy Object
            GameObject appIconByNameDeliveries = Utils.ChangeLabelFromAppIcon(cloningNameDeliveries, "DeliveriesCopy");

            // Aktualisiere den Namen vom App-Icon für das Products Copy Object
            GameObject appIconByNameProducts = Utils.ChangeLabelFromAppIcon(cloningNameProductsCopy, "ProductsCopy");

            Transform container = futureSleepApp.transform.Find("Container");

            // Remove FiltersPanel if it exists (added by Product Manager Filter mod)
            // This ensures compatibility when that mod is installed
            Transform filtersPanel = container?.Find("FiltersPanel");
            if (filtersPanel != null)
            {
                MelonLogger.Msg("Removing FiltersPanel from cloned app (Product Manager Filter compatibility)");
                UnityEngine.Object.DestroyImmediate(filtersPanel.gameObject);
            }

            //Adjust Topbar for Sleep App
            Transform topbarTransform = container.Find("Topbar");
            if (topbarTransform != null)
            {
                //Adjust Topbar Color
                Image topbarImage = topbarTransform.GetComponent<Image>();
                topbarImage.color = ColorUtil.GetColor("Cyan");

                //Adjust Topbar Title
                Transform topbarTitleTransform = topbarTransform.Find("Title");
                if (topbarTitleTransform != null)
                {
                    topbarTitleTransform.GetComponent<Text>().text = Title;
                }

                Transform topbarSubtitleTransform = topbarTransform.Find("Subtitle");
                if (topbarSubtitleTransform != null)
                {
                    topbarSubtitleTransform.GetComponent<Text>().text = "by KampfBallerina";
                    topbarSubtitleTransform.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                    topbarSubtitleTransform.GetComponent<Text>().color = ColorUtil.GetColor("White");
                    topbarSubtitleTransform.GetComponent<RectTransform>().anchorMax = new Vector2(0.98f, 1);
                }
            }

            // Scroll View from the Sleep App
            Transform scrollViewProductsApp = container.Find("Scroll View");
            if (scrollViewProductsApp != null)
            {
                // This needs to stay to avoid the Meth Oven to brick
                scrollViewProductsApp.gameObject.SetActive(false);
                scrollViewProductsApp.gameObject.name = "DeactivatedScrollView";
                /* This needs to stay since new products try to find the SleepingApp ScrollView*/
            }

            Transform containerTransformClonedDeliveriesApp = deliveriesApp.transform.Find("Container");
            if (containerTransformClonedDeliveriesApp != null)
            {
                Transform scrollViewTransformDeliveriesClone = containerTransformClonedDeliveriesApp.Find("Scroll View");
                if (scrollViewTransformDeliveriesClone != null)
                {

                    Transform scrollViewViewport = scrollViewTransformDeliveriesClone.transform.Find("Viewport");
                    if (scrollViewViewport != null)
                    {
                        Transform scrollViewContent = scrollViewViewport.transform.Find("Content");
                        /* Utils.ClearChildren(scrollViewContent, child =>
                             child.name == "Space" ||
                             child == scrollViewContent.GetChild(0).gameObject ||
                             child.transform.IsChildOf(scrollViewContent.GetChild(0))
                         );*/
                    }

                    Transform orderSubmitted = scrollViewTransformDeliveriesClone.transform.Find("OrderSubmitted");
                    if (orderSubmitted != null)
                    {
                        UnityEngine.Object.DestroyImmediate(orderSubmitted.gameObject);
                    }
                    scrollViewTransformDeliveriesClone.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(-100, -60); // Set the sizeDelta to match the original
                    scrollViewTransformDeliveriesClone.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, -30); // Set the anchoredPosition to match the original
                    scrollViewTransformDeliveriesClone.SetParent(container, false);
                }
            }

            Transform settingsTransform = container.Find("Details");
            if (settingsTransform != null)
            {
                // Ändere den Namen des Details-Objekts
                settingsTransform.name = "Details";
                settingsTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(95f, -60f); // Setze die Größe des Details-Objekts
                settingsTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-47.5f, -30f); // Setze die Position des Details-Objekts

                Transform detailsTitleTransform = settingsTransform.Find("Scroll View/Viewport/Content/Title");
                if (detailsTitleTransform != null)
                {
                    detailsTitleObject = detailsTitleTransform.gameObject;
                    detailsTitleObject.SetActive(false);
                    Text detailsTitleText = detailsTitleTransform.GetComponent<Text>();
                    detailsTitleText.text = "DetailsTitleText";
                }

                Transform detailsSubtitleTransform = settingsTransform.Find("Scroll View/Viewport/Content/Description");
                if (detailsSubtitleTransform != null)
                {
                    detailsSubtitleTransform.GetComponent<Text>().text = "This is going to be the description of the app.";
                    detailsSubtitleTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0); // Set the anchoredPosition to match the original
                    detailsSubtitleTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50); // Set the sizeDelta to match the original
                    detailsSubtitleTransform.GetComponent<Text>().fontSize = 18;
                    detailsSubtitleTransform.gameObject.SetActive(false);
                    detailsSubtitleObject = detailsSubtitleTransform.gameObject;
                }

                //  MelonLogger.Msg("Deleting unneeded objects");
                settingsContentTransform = settingsTransform.Find("Scroll View/Viewport/Content");
                Transform toDeleteTransform = settingsContentTransform.Find("Value");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Value-Objekt
                toDeleteTransform = settingsContentTransform.Find("Effects");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Effects-Objekt
                toDeleteTransform = settingsContentTransform.Find("Addiction");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Addiction-Objekt
                toDeleteTransform = settingsContentTransform.Find("Recipes");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Recipes-Objekt
                toDeleteTransform = settingsContentTransform.Find("RecipesContainer");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das RecipesContainer-Objekt
                toDeleteTransform = settingsContentTransform.Find("Listed");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Listed-Objekt
                toDeleteTransform = settingsContentTransform.Find("Delisted");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Delisted-Objekt
                toDeleteTransform = settingsContentTransform.Find("NotDiscovered");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das NotDiscovered-Objekt
                toDeleteTransform = settingsContentTransform.Find("Properties");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Properties-Objekt

                Transform instructionTransform = settingsTransform.Find("Instruction");
                if (instructionTransform != null)
                {
                    InstructionsTextObject = instructionTransform.gameObject;
                    Text instructionsText = instructionTransform.GetComponent<Text>();
                    instructionsText.text = "Select a section to adjust settings";
                    instructionsText.fontSize = 20;

                }

                // Adjust the Spacing for the right Scroll View, so that the Title is on top
                Transform settingsSpaceTransform = settingsContentTransform.Find("Space");
                if (settingsSpaceTransform != null)
                {
                    settingsSpaceTransform.gameObject.SetActive(true); // Hide
                    RectTransform settingsSpaceRect = settingsSpaceTransform.GetComponent<RectTransform>();
                    settingsSpaceRect.sizeDelta = new Vector2(settingsSpaceRect.sizeDelta.x, 200f);
                }
            }

            CreateTemplates(container, settingsContentTransform);

            // Destroy the Temp Apps
            UnityEngine.Object.DestroyImmediate(appIconByNameDeliveries.gameObject);
            deliveriesApp.Destroy();
            UnityEngine.Object.DestroyImmediate(appIconByNameProducts.gameObject);
            productsApp.Destroy();

            // Aktualisiere den Namen vom App-Icon für das Sleeping App Object
            GameObject appIconByName = Utils.ChangeLabelFromAppIcon(cloningNameProducts, Title);

            // Ändere das App-Icon-Bild
            MRSCore.Instance.ChangeAppIconImage(appIconByName, iconSprite);

            // Registriere die App
            MRSCore.Instance.RegisterApp(appIconByName, Title);

            settingsContentTransform.gameObject.SetActive(true);
            AddUiForGeneralSettings(settingsContentTransform);
            AddUiForPositiveEffects(settingsContentTransform);
            AddUiForNegativeEffects(settingsContentTransform);
            AddUIForArrestedEvent(settingsContentTransform);
            AddUIForMurderedEvent(settingsContentTransform);
            _isSleepingAppLoaded = true;
        }



        void CreateTemplates(Transform deliveriesContainer, Transform productsContainer = null)
        {
            if (deliveriesContainer == null)
            {
                MelonLogger.Error("Container not found!");
                return;
            }

            //Products Templates
            if (productsContainer != null)
            {
                Transform toggleTransform = productsContainer.Find("Toggle");
                if (toggleTransform != null)
                {
                    RectTransform toggleRectTransform = toggleTransform.GetComponent<RectTransform>();
                    if (toggleRectTransform != null)
                    {
                        // toggleRectTransform.anchoredPosition = new Vector2(0, 0); // Set the anchoredPosition to match the original
                    }

                    HorizontalLayoutGroup horizontalContainerGroup = toggleTransform.GetComponent<HorizontalLayoutGroup>();
                    if (horizontalContainerGroup == null)
                    {
                        horizontalContainerGroup = toggleTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
                        horizontalContainerGroup.spacing = 100f; // Abstand zwischen den Elementen
                        horizontalContainerGroup.childForceExpandWidth = false; // Breite der Kinder nicht erzwingen
                        horizontalContainerGroup.childForceExpandHeight = false; // Höhe der Kinder nicht erzwingen
                        horizontalContainerGroup.childControlWidth = true; // Breite der Kinder steuern
                        horizontalContainerGroup.childControlHeight = true; // Höhe der Kinder steuern
                        horizontalContainerGroup.childAlignment = TextAnchor.MiddleLeft; // Elemente linksbündig ausrichten
                    }

                    // Füge einen ContentSizeFitter hinzu, um die Breite automatisch anzupassen
                    ContentSizeFitter contentSizeFitter = toggleTransform.GetComponent<ContentSizeFitter>();
                    if (contentSizeFitter == null)
                    {
                        contentSizeFitter = toggleTransform.gameObject.AddComponent<ContentSizeFitter>();
                        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // Passe die Breite an den Inhalt an
                        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained; // Höhe nicht anpassen
                    }


                    Transform toggleTextTransform = toggleTransform.Find("Text");
                    if (toggleTextTransform != null)
                    {
                        Text textComponent = toggleTextTransform.GetComponent<Text>();
                        textComponent.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                        textComponent.text = "toggleTransformText";
                        textComponent.color = ColorUtil.GetColor("White");
                        textComponent.fontSize = 18;
                        toggleTextTransform.SetAsFirstSibling();
                        toggleTextTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(325, 30);

                        LayoutElement labelLayoutElement = toggleTextTransform.GetComponent<LayoutElement>();
                        if (labelLayoutElement == null)
                        {
                            labelLayoutElement = toggleTextTransform.gameObject.AddComponent<LayoutElement>();
                            labelLayoutElement.minWidth = 325; // Mindestbreite des Labels
                        }
                    }

                    Toggle toggleToggle = toggleTransform.GetComponent<Toggle>();
                    if (toggleToggle != null)
                    {
                        toggleToggle.isOn = false; // Set the toggle to off
                        toggleToggle.onValueChanged.RemoveAllListeners(); // Remove existing listeners to avoid old functionality after copying
                    }
                    Transform backgroundTransform = toggleTransform.Find("Background");
                    if (backgroundTransform != null)
                    {
                        backgroundTransform.name = "CheckboxBackground";
                        LayoutElement checkboxLayoutElement = backgroundTransform.GetComponent<LayoutElement>();
                        if (checkboxLayoutElement == null)
                        {
                            checkboxLayoutElement = backgroundTransform.gameObject.AddComponent<LayoutElement>();
                            checkboxLayoutElement.minWidth = 30; // Mindestbreite des Labels
                            checkboxLayoutElement.preferredWidth = 30; // Bevorzugte Breite des Labels
                            checkboxLayoutElement.minHeight = 30; // Mindesthöhe des Labels
                            checkboxLayoutElement.preferredHeight = 30; // Bevorzugte Höhe des Labels

                        }
                    }
                    Transform checkmarkTransform = backgroundTransform.Find("Checkmark");
                    if (checkmarkTransform != null)
                    {
                        checkmarkTransform.GetComponent<Image>().color = ColorUtil.GetColor("Cyan");
                    }

                    RectTransform checkboxRect = toggleTransform.gameObject.GetComponent<RectTransform>();
                    if (checkboxRect != null)
                    {
                        checkboxRect.sizeDelta = new Vector2(30, 30);
                    }

                    // Save as Template
                    checkboxTemplate = UnityEngine.Object.Instantiate(toggleTransform.gameObject);
                    checkboxTemplate.name = "Checkbox Template";
                    checkboxTemplate.SetActive(true);

                    toggleTransform.gameObject.Destroy(); // Zerstöre das ursprüngliche GameObject
                }
            }


            // Deliveries Templates and Cleaning
            Transform scrollViewTransform = deliveriesContainer.Find("Scroll View");
            if (scrollViewTransform != null)
            {
                Transform orderSubmittedTransform = scrollViewTransform.Find("OrderSubmitted");
                if (orderSubmittedTransform != null)
                {
                    orderSubmittedTransform.gameObject.Destroy(); // Zerstöre das ursprüngliche GameObject
                }

                Transform viewportTransform = scrollViewTransform.Find("Viewport");
                if (viewportTransform != null)
                {
                    Transform viewportContentTransform = viewportTransform.Find("Content");
                    if (viewportContentTransform != null)
                    {
                        sleepingAppViewportContentTransform = viewportContentTransform;
                        // Suche nach den GameObjects "Dan's Hardware" und "Gas-Mart (West)" und "Space" und speichere sie in Variablen
                        GameObject dansHardware = viewportContentTransform.Find("Dan's Hardware").gameObject;
                        GameObject gasMartWest = viewportContentTransform.Find("Gas-Mart (West)").gameObject;
                        GameObject viewPortContentSpace = viewportContentTransform.Find("Space").gameObject;

                        if (dansHardware != null && DansHardwareTemplate == null)
                        {
                            DansHardwareTemplate = UnityEngine.Object.Instantiate(dansHardware);
                            DansHardwareTemplate.name = "Dan's Hardware Template";
                            DansHardwareTemplate.SetActive(false); // Deaktiviere das Template

                            Transform contentsDansTemplateTransform = DansHardwareTemplate.transform.Find("Contents");
                            if (contentsDansTemplateTransform != null)
                            {
                                UnityEngine.Object.DestroyImmediate(contentsDansTemplateTransform.gameObject); // Entferne den "Contents"
                                                                                                               //   MelonLogger.Msg("Removed Contents from Dans Hardware Template");
                            }

                            UnityEngine.Object.Destroy(dansHardware); // Entferne das ursprüngliche GameObject
                        }

                        if (gasMartWest != null && GasMartWestTemplate == null)
                        {
                            GasMartWestTemplate = UnityEngine.Object.Instantiate(gasMartWest);
                            GasMartWestTemplate.name = "Gas-Mart (West) Template";
                            GasMartWestTemplate.SetActive(false); // Deaktiviere das Template

                            Transform contentsMarketTemplateTransform = GasMartWestTemplate.transform.Find("Contents");
                            if (contentsMarketTemplateTransform != null)
                            {
                                UnityEngine.Object.DestroyImmediate(contentsMarketTemplateTransform.gameObject); // Entferne den "Contents"
                                                                                                                 //   MelonLogger.Msg("Removed Contents from Gas Mart West Template");
                            }
                            UnityEngine.Object.Destroy(gasMartWest); // Entferne das ursprüngliche GameObject
                        }

                        if (viewPortContentSpace != null && viewPortContentSpaceTemplate == null)
                        {
                            viewPortContentSpaceTemplate = UnityEngine.Object.Instantiate(viewPortContentSpace);
                            viewPortContentSpaceTemplate.name = "Space Template";
                            viewPortContentSpaceTemplate.SetActive(false); // Deaktiviere das Template
                            UnityEngine.Object.Destroy(viewPortContentSpace); // Entferne das ursprüngliche GameObject
                        }

                        Utils.ClearChildren(viewportContentTransform);
                        AddSpaceFromTemplate(viewportContentTransform);
                        //  MelonLogger.Msg("Saved Dan's Hardware, Gas Mart and Space as Templates");
                    }
                    else
                    {
                        MelonLogger.Error("Viewport Content not found!");
                    }
                }
            }
        }

        public void AddSpaceFromTemplate(Transform parentTransform = null)
        {
            if (parentTransform == null)
            {
                parentTransform = sleepingAppViewportContentTransform;
                if (parentTransform == null)
                {
                    MelonLogger.Error("ViewportContentTransform is null!");
                    return;
                }
            }

            if (viewPortContentSpaceTemplate == null)
            {
                MelonLogger.Error("Can't find Space Template!");
                return;
            }

            // Überprüfe, ob bereits ein "Space"-Eintrag existiert
            Transform existingSpace = parentTransform.Find("Space");
            if (existingSpace != null)
            {
                // Verschiebe den vorhandenen "Space"-Eintrag an die letzte Position
                existingSpace.SetAsLastSibling();
                //    MelonLogger.Msg("Moved existing Space entry to the last position.");
                return;
            }

            // Erstelle ein Duplikat des Templates
            GameObject newSpace = UnityEngine.Object.Instantiate(viewPortContentSpaceTemplate, parentTransform);
            newSpace.name = "Space";
            newSpace.transform.SetAsLastSibling(); // Stelle sicher, dass der Eintrag der letzte ist
            newSpace.SetActive(true);

            //MelonLogger.Msg("Added Custom Spacing for new Entries as the last item.");
        }

        public void AddEntryFromTemplate(string newObjectName, string newTitle, string newSubtitle = null, GameObject template = null, Color newBackgroundColor = default,
        Sprite iconSprite = null, Transform parentTransform = null, bool isFirstEntry = false)
        {
            if (parentTransform == null)
            {
                if (sleepingAppViewportContentTransform == null)
                {
                    MelonLogger.Error("ViewportContentTransform is null!");
                    return;
                }
                else
                {
                    parentTransform = sleepingAppViewportContentTransform;
                }
            }

            if (template == null)
            {
                if (iconSprite != null)
                {
                    if (DansHardwareTemplate)
                    {
                        template = DansHardwareTemplate; // Standard-Template verwenden, wenn kein Template angegeben ist
                    }
                    else if (GasMartWestTemplate)
                    {
                        template = GasMartWestTemplate; // Standard-Template verwenden, wenn kein Template angegeben ist
                    }
                }
            }

            // Erstelle ein Duplikat des Templates
            GameObject newEntry = UnityEngine.Object.Instantiate(template, parentTransform);
            newEntry.name = newObjectName;

            //Find the Transforms
            Transform headerTransform = newEntry.transform.Find("Header");
            Transform iconTransform = headerTransform.Find("Icon");
            Transform imageTransform = iconTransform.Find("Image");
            Transform titleTransform = headerTransform.Find("Title");
            Transform subtitleTransform = headerTransform.Find("Description");
            Transform arrowTransform = headerTransform.Find("Arrow");

            // Hide the Arrow
            arrowTransform.gameObject.SetActive(false);

            // Change Title
            Text titleTextComponent = titleTransform.GetComponent<Text>();
            if (titleTextComponent != null)
            {
                titleTextComponent.text = newTitle;
            }

            // Change Subtitle
            Text subtitleTextComponent = subtitleTransform.GetComponent<Text>();
            if (subtitleTextComponent != null)
            {
                if (string.IsNullOrEmpty(newSubtitle))
                {
                    subtitleTextComponent.gameObject.SetActive(false); // Hide the subtitle if it's empty
                }
                else
                {
                    subtitleTextComponent.text = newSubtitle;
                    subtitleTextComponent.gameObject.SetActive(true); // Show the subtitle if it's not empty
                }
            }

            // Change Header Color (optional)
            Image headerImageComponent = headerTransform.GetComponent<Image>();
            if (headerImageComponent != null && newBackgroundColor != default)
            {
                headerImageComponent.color = newBackgroundColor;
            }

            // Change Icon Image
            if (iconSprite != null)
            {
                Image iconImageComponent = imageTransform.GetComponent<Image>();
                if (iconImageComponent != null)
                {
                    iconImageComponent.sprite = iconSprite;
                }
            }

            Button headerButton = headerTransform.GetComponent<Button>();
            if (headerButton != null)
            {
                headerButton.name = newObjectName + " Button";
                headerButton.onClick.RemoveAllListeners(); // Remove existing listeners to avoid old functionality after copying
                if (newObjectName == "GeneralSettingsSection")
                {
                    void FuncThatCallsFunc() => GeneralSettingsSectionClicked();
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }
                else if (newObjectName == "PositiveEffectsSection")
                {
                    void FuncThatCallsFunc() => PositiveEffectsSectionClicked();
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }
                else if (newObjectName == "NegativeEffectsSection")
                {
                    void FuncThatCallsFunc() => NegativeEffectsSectionClicked();
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }
                else if (newObjectName == "ArrestedEventSection")
                {
                    void FuncThatCallsFunc() => ArrestedEventSectionClicked();
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }
                else if (newObjectName == "MurderedEventSection")
                {
                    void FuncThatCallsFunc() => MurderedEventSectionClicked();
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }
                else
                {
                    void FuncThatCallsFunc() => MelonLogger.Msg("Clicked.");
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }

                // Set the new entry as the first child if isFirstEntry is true
                if (isFirstEntry)
                {
                    newEntry.transform.SetAsFirstSibling();
                }
                newEntry.SetActive(true);
                //  MelonLogger.Msg($"Added new entry: {newObjectName} with text: {newTitle}");
            }
            AddSpaceFromTemplate(parentTransform); // Add space after the new entry
        }

        void PositiveEffectsSectionClicked()
        {
            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = "Positive Effects";

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = "Choose which effects you would like to add to the random effects pool.";

            if (positiveEffectsObject == null)
            {
                MelonLogger.Error("positiveEffectsObject is null!");
                return;
            }

            if (generalSettingsObject.activeSelf)
            {
                generalSettingsObject.SetActive(false);
            }

            if (!positiveEffectsObject.activeSelf)
            {
                positiveEffectsObject.SetActive(true);
            }

            if (negativeEffectsObject.activeSelf)
            {
                negativeEffectsObject.SetActive(false);
            }

            if (arrestedEventObject.activeSelf)
            {
                arrestedEventObject.SetActive(false);
            }

            if (murderedEventObject.activeSelf)
            {
                murderedEventObject.SetActive(false);
            }

            SetCheckboxValue(positiveEffectsObject.transform, "Anti Gravity", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Anti_Gravity);
            SetCheckboxValue(positiveEffectsObject.transform, "Athletic", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Athletic);
            SetCheckboxValue(positiveEffectsObject.transform, "Bright Eyed", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Bright_Eyed);
            SetCheckboxValue(positiveEffectsObject.transform, "Calming", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Calming);
            SetCheckboxValue(positiveEffectsObject.transform, "Calorie Dense", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Calorie_Dense);
            SetCheckboxValue(positiveEffectsObject.transform, "Electrifying", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Electrifying);
            SetCheckboxValue(positiveEffectsObject.transform, "Energizing", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Energizing);
            SetCheckboxValue(positiveEffectsObject.transform, "Euphoric", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Euphoric);
            SetCheckboxValue(positiveEffectsObject.transform, "Focused", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Focused);
            SetCheckboxValue(positiveEffectsObject.transform, "Munchies", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Munchies);
            SetCheckboxValue(positiveEffectsObject.transform, "Refreshing", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Refreshing);
            SetCheckboxValue(positiveEffectsObject.transform, "Sneaky", MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Sneaky);
            SetCheckboxValue(positiveEffectsObject.transform, "Check All", false);
        }

        void NegativeEffectsSectionClicked()
        {
            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = "Negative Effects";

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = "Choose which effects you would like to add to the random effects pool.";

            // MelonLogger.Msg("General Settings Section clicked.");
            if (negativeEffectsObject == null)
            {
                MelonLogger.Error("negativeEffectsObject is null!");
                return;
            }

            if (generalSettingsObject.activeSelf)
            {
                generalSettingsObject.SetActive(false);
            }

            if (positiveEffectsObject.activeSelf)
            {
                positiveEffectsObject.SetActive(false);
            }

            if (!negativeEffectsObject.activeSelf)
            {
                negativeEffectsObject.SetActive(true);
            }

            if (arrestedEventObject.activeSelf)
            {
                arrestedEventObject.SetActive(false);
            }

            if (murderedEventObject.activeSelf)
            {
                murderedEventObject.SetActive(false);
            }

            SetCheckboxValue(negativeEffectsObject.transform, "Balding", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Balding);
            SetCheckboxValue(negativeEffectsObject.transform, "Bright Eyed", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Bright_Eyed);
            SetCheckboxValue(negativeEffectsObject.transform, "Calming", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Calming);
            SetCheckboxValue(negativeEffectsObject.transform, "Calorie Dense", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Calorie_Dense);
            SetCheckboxValue(negativeEffectsObject.transform, "Cyclopean", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Cyclopean);
            SetCheckboxValue(negativeEffectsObject.transform, "Disorienting", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Disorienting);
            SetCheckboxValue(negativeEffectsObject.transform, "Electrifying", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Electrifying);
            SetCheckboxValue(negativeEffectsObject.transform, "Explosive", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Explosive);
            SetCheckboxValue(negativeEffectsObject.transform, "Foggy", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Foggy);
            SetCheckboxValue(negativeEffectsObject.transform, "Gingeritis", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Gingeritis);
            SetCheckboxValue(negativeEffectsObject.transform, "Glowing", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Glowing);
            SetCheckboxValue(negativeEffectsObject.transform, "Jennerising", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Jennerising);
            SetCheckboxValue(negativeEffectsObject.transform, "Laxative", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Laxative);
            SetCheckboxValue(negativeEffectsObject.transform, "Lethal", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Lethal);
            SetCheckboxValue(negativeEffectsObject.transform, "Long Faced", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Long_Faced);
            SetCheckboxValue(negativeEffectsObject.transform, "Paranoia", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Paranoia);
            SetCheckboxValue(negativeEffectsObject.transform, "Schizophrenic", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Schizophrenic);
            SetCheckboxValue(negativeEffectsObject.transform, "Sedating", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Sedating);
            SetCheckboxValue(negativeEffectsObject.transform, "Seizure Inducing", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Seizure_Inducing);
            SetCheckboxValue(negativeEffectsObject.transform, "Shrinking", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Shrinking);
            SetCheckboxValue(negativeEffectsObject.transform, "Slippery", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Slippery);
            SetCheckboxValue(negativeEffectsObject.transform, "Smelly", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Smelly);
            SetCheckboxValue(negativeEffectsObject.transform, "Spicy", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Spicy);
            SetCheckboxValue(negativeEffectsObject.transform, "Thought Provoking", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Thought_Provoking);
            SetCheckboxValue(negativeEffectsObject.transform, "Toxic", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Toxic);
            SetCheckboxValue(negativeEffectsObject.transform, "Tropic Thunder", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Tropic_Thunder);
            SetCheckboxValue(negativeEffectsObject.transform, "Zombifying", MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Zombifying);
            SetCheckboxValue(negativeEffectsObject.transform, "Check All", false);
        }

        void GeneralSettingsSectionClicked()
        {
            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = "General Settings";

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = "Enable / disable settings, adjust global values etc. ";

            // MelonLogger.Msg("General Settings Section clicked.");
            if (generalSettingsObject == null)
            {
                MelonLogger.Error("GeneralSettingsObject is null!");
                return;
            }

            if (!generalSettingsObject.activeSelf)
            {
                generalSettingsObject.SetActive(true);
            }

            if (positiveEffectsObject.activeSelf)
            {
                positiveEffectsObject.SetActive(false);
            }

            if (negativeEffectsObject.activeSelf)
            {
                negativeEffectsObject.SetActive(false);
            }

            if (arrestedEventObject.activeSelf)
            {
                arrestedEventObject.SetActive(false);
            }

            if (murderedEventObject.activeSelf)
            {
                murderedEventObject.SetActive(false);
            }

            // Lade die Daten aus der Config und aktualisiere die UI-Elemente

            SetCheckboxValue(generalSettingsObject.transform, "Forced Sleep", MRSCore.Instance.config.SleepSettings.Enable_Forced_Sleep);
            SetInputFieldValue(generalSettingsObject.transform, "Cooldown Time", MRSCore.Instance.config.SleepSettings.Cooldown_Time);
            SetInputFieldValue(generalSettingsObject.transform, "Forced Sleep Delay", MRSCore.Instance.config.SleepSettings.Forced_Sleep_Delay);
            SetCheckboxValue(generalSettingsObject.transform, "Auto Skip Daily Summary", MRSCore.Instance.config.SleepSettings.Auto_Skip_Daily_Summary);
            SetCheckboxValue(generalSettingsObject.transform, "Positive Effects", MRSCore.Instance.config.SleepSettings.Enable_Positive_Effects);
            SetInputFieldValue(generalSettingsObject.transform, "Positive Effects Duration", MRSCore.Instance.config.SleepSettings.Positive_Effects_Duration);
            SetInputFieldValue(generalSettingsObject.transform, "Positive Effects Probability", MRSCore.Instance.config.SleepSettings.Positive_Effects_Probability);
            SetCheckboxValue(generalSettingsObject.transform, "Negative Effects", MRSCore.Instance.config.SleepSettings.Enable_Negative_Effects);
            SetInputFieldValue(generalSettingsObject.transform, "Negative Effects Duration", MRSCore.Instance.config.SleepSettings.Negative_Effects_Duration);
            SetInputFieldValue(generalSettingsObject.transform, "Negative Effects Probability", MRSCore.Instance.config.SleepSettings.Negative_Effects_Probability);
            SetCheckboxValue(generalSettingsObject.transform, "Notifications", MRSCore.Instance.config.SleepSettings.Enable_Effect_Notifications);
        }

        void ArrestedEventSectionClicked()
        {
            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = "Arrested Event Settings";

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = "Choose the settings for the Get Arrested Event - when you're sleeping in public.";

            if (arrestedEventObject == null)
            {
                MelonLogger.Error("ArrestedEventSettingsObject is null!");
                return;
            }

            if (!arrestedEventObject.activeSelf)
            {
                arrestedEventObject.SetActive(true);
            }

            if (generalSettingsObject.activeSelf)
            {
                generalSettingsObject.SetActive(false);
            }

            if (positiveEffectsObject.activeSelf)
            {
                positiveEffectsObject.SetActive(false);
            }

            if (negativeEffectsObject.activeSelf)
            {
                negativeEffectsObject.SetActive(false);
            }

            if (murderedEventObject.activeSelf)
            {
                murderedEventObject.SetActive(false);
            }

            // Lade die Daten aus der Config und aktualisiere die UI-Elemente
            SetCheckboxValue(arrestedEventObject.transform, "Arrested Event", MRSCore.Instance.config.ArrestedEventSettings.Enable_GetArrested_Event);
            SetCheckboxValue(arrestedEventObject.transform, "Save Spaces", MRSCore.Instance.config.ArrestedEventSettings.Enable_GetArrested_Event_SaveSpaces);
            SetInputFieldValue(arrestedEventObject.transform, "Event Probability", MRSCore.Instance.config.ArrestedEventSettings.GetArrested_Event_Probability);
        }

        void MurderedEventSectionClicked()
        {
            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = "Murdered Event Settings";

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = "Choose the settings for the Get Murdered Event - when you get killed for sleeping in public.";

            if (murderedEventObject == null)
            {
                MelonLogger.Error("murderedEventObject is null!");
                return;
            }

            if (!murderedEventObject.activeSelf)
            {
                murderedEventObject.SetActive(true);
            }

            if (arrestedEventObject.activeSelf)
            {
                arrestedEventObject.SetActive(false);
            }

            if (generalSettingsObject.activeSelf)
            {
                generalSettingsObject.SetActive(false);
            }

            if (positiveEffectsObject.activeSelf)
            {
                positiveEffectsObject.SetActive(false);
            }

            if (negativeEffectsObject.activeSelf)
            {
                negativeEffectsObject.SetActive(false);
            }

            // Lade die Daten aus der Config und aktualisiere die UI-Elemente
            SetCheckboxValue(murderedEventObject.transform, "Murdered Event", MRSCore.Instance.config.MurderedEventSettings.Enable_GetMurdered_Event);
            SetCheckboxValue(murderedEventObject.transform, "Respawning", MRSCore.Instance.config.MurderedEventSettings.Allow_GetMurdered_Event_Respawning);
            SetCheckboxValue(murderedEventObject.transform, "Save Spaces", MRSCore.Instance.config.MurderedEventSettings.Enable_GetMurdered_Event_SaveSpaces);
            SetInputFieldValue(murderedEventObject.transform, "Event Probability", MRSCore.Instance.config.MurderedEventSettings.GetMurdered_Event_Probability);
        }

        void AddSeperatorLine(Transform parentTransform, float width = 455, float height = 2, string sColor = "Light Grey")
        {
            GameObject separatorLine = new GameObject("Separator Line");
            separatorLine.transform.SetParent(parentTransform, false);

            RectTransform separatorRect = separatorLine.AddComponent<RectTransform>();
            separatorRect.sizeDelta = new Vector2(width, height); // Width and height of the line
            separatorRect.anchorMin = new Vector2(0.5f, 0.5f);
            separatorRect.anchorMax = new Vector2(0.5f, 0.5f);
            separatorRect.pivot = new Vector2(0.5f, 0.5f);

            LayoutElement seperatorLayoutElement = separatorLine.AddComponent<LayoutElement>();
            seperatorLayoutElement.minHeight = height; // Set the minimum height of the line
            seperatorLayoutElement.preferredHeight = height; // Set the preferred height of the line
            seperatorLayoutElement.minWidth = width; // Set the minimum width of the line
            seperatorLayoutElement.preferredWidth = width; // Set the preferred width of the line

            Image separatorImage = separatorLine.AddComponent<Image>();
            separatorImage.color = ColorUtil.GetColor(sColor); // Set the color of the line
        }
        void AddSaveButton(Transform parentTransform, string saveString = null, bool addSaveSpace = false)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null! Cannot add Save Button.");
                return;
            }

            if (addSaveSpace)
            {
                GameObject saveSpaceObject = new GameObject("SaveSpace");
                saveSpaceObject.transform.SetParent(parentTransform, false);
                RectTransform saveSpaceRect = saveSpaceObject.AddComponent<RectTransform>();
                {
                    saveSpaceRect.sizeDelta = new Vector2(100, 15); // Abstand zwischen dem letzten Element und dem Button
                }

                LayoutElement spaceLayoutElement = saveSpaceObject.GetComponent<LayoutElement>();
                if (spaceLayoutElement == null)
                {
                    spaceLayoutElement = saveSpaceObject.AddComponent<LayoutElement>();
                    spaceLayoutElement.minHeight = 35; // Mindesthöhe des Space
                    spaceLayoutElement.preferredHeight = 35; // Bevorzugte Höhe des Space
                    spaceLayoutElement.minWidth = 100; // Mindestbreite des Space
                    spaceLayoutElement.preferredWidth = 100; // Bevorzugte Breite des Space
                }
            }

            // Erstelle ein neues GameObject für den Button
            GameObject saveButtonObject = new GameObject("Save Button");
            saveButtonObject.transform.SetParent(parentTransform, false);
            saveButtonObject.SetActive(true);

            // Füge ein RectTransform hinzu, falls es nicht existiert
            RectTransform buttonRect = saveButtonObject.GetComponent<RectTransform>();
            if (buttonRect == null)
            {
                buttonRect = saveButtonObject.AddComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(455, 30); // Standardgröße des Buttons
            }

            // Füge ein Button-Objekt hinzu
            Button saveButton = saveButtonObject.GetComponent<Button>();
            if (saveButton == null)
            {
                saveButton = saveButtonObject.AddComponent<Button>();
            }

            // Füge ein Hintergrundbild hinzu
            Image buttonImage = saveButtonObject.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = saveButtonObject.AddComponent<Image>();
                if (saveButtonSprite == null)
                {
                    saveButtonSprite = EmbeddedAssets.LoadUIElementSprite("SaveButton");
                }
                if (saveButtonSprite != null)
                {
                    buttonImage.sprite = saveButtonSprite;
                }
                else
                {
                    buttonImage.color = new Color(0.2f, 0.6f, 0.2f); // Grüne Farbe für den Button
                }
            }

            // Erstelle ein neues GameObject für den Text des Buttons
            GameObject buttonTextObject = new GameObject("Save Button Text");
            buttonTextObject.transform.SetParent(saveButtonObject.transform, false);

            // Füge eine Text-Komponente hinzu
            Text buttonText = buttonTextObject.GetComponent<Text>();
            if (buttonText == null)
            {
                buttonText = buttonTextObject.AddComponent<Text>();
                buttonText.text = "Save & Apply";
                buttonText.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                buttonText.fontSize = 18;
                buttonText.color = Color.white;
                buttonText.alignment = TextAnchor.MiddleCenter;
            }

            // Positioniere den Text innerhalb des Buttons
            RectTransform textRect = buttonTextObject.GetComponent<RectTransform>();
            if (textRect == null)
            {
                textRect = buttonTextObject.AddComponent<RectTransform>();
            }
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            // Füge ein LayoutElement hinzu, um die Integration in die VerticalLayoutGroup zu gewährleisten
            LayoutElement layoutElement = saveButtonObject.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = saveButtonObject.AddComponent<LayoutElement>();
                layoutElement.minHeight = 35; // Mindesthöhe des Buttons
                layoutElement.preferredHeight = 35; // Bevorzugte Höhe des Buttons
                layoutElement.minWidth = 200; // Mindestbreite des Buttons
                layoutElement.preferredWidth = 455; // Bevorzugte Breite des Buttons
            }


            if (saveString == "GeneralSettings")
            {
                void FuncThatCallsFunc() => SaveGeneralSettings(buttonText);
                saveButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
            }
            else if (saveString == "PositiveEffects")
            {
                void FuncThatCallsFunc() => SavePositiveEffects(buttonText);
                saveButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
            }
            else if (saveString == "NegativeEffects")
            {
                void FuncThatCallsFunc() => SaveNegativeEffects(buttonText);
                saveButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
            }
            else if (saveString == "ArrestedEvent")
            {
                void FuncThatCallsFunc() => SaveArrestedEvent(buttonText);
                saveButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
            }
            else if (saveString == "MurderedEvent")
            {
                void FuncThatCallsFunc() => SaveMurderedEvent(buttonText);
                saveButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
            }
            else
            {
                MelonLogger.Error("Save string is null or empty!");
            }

        }
        void AddLabelInputPair(string labelText, Transform parentTransform, string prefixText)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error($"Parent transform is null for label: {labelText}");
                return;
            }

            if (string.IsNullOrEmpty(labelText))
            {
                MelonLogger.Error("Label text is null or empty!");
                return;
            }

            // Erstelle einen Container für das Label und das Inputfeld
            GameObject container = new GameObject($"{labelText} Horizontal Container");
            container.transform.SetParent(parentTransform, false);
            //  container.SetActive(false);

            // Füge eine HorizontalLayoutGroup hinzu, falls sie nicht existiert
            HorizontalLayoutGroup horizontalContainerGroup = container.GetComponent<HorizontalLayoutGroup>();
            if (horizontalContainerGroup == null)
            {
                horizontalContainerGroup = container.AddComponent<HorizontalLayoutGroup>();
                horizontalContainerGroup.spacing = 30f; // Abstand zwischen den Elementen
                horizontalContainerGroup.childAlignment = TextAnchor.MiddleLeft; // Elemente linksbündig ausrichten
                horizontalContainerGroup.childForceExpandWidth = false; // Breite der Kinder nicht erzwingen
                horizontalContainerGroup.childForceExpandHeight = false; // Höhe der Kinder nicht erzwingen
            }

            // Erstelle ein neues GameObject für das Label
            GameObject labelObject = new GameObject($"{labelText} Label");
            labelObject.transform.SetParent(container.transform, false);

            // Füge eine Text-Komponente hinzu, falls sie nicht existiert
            Text labelTextComponent = labelObject.GetComponent<Text>();
            if (labelTextComponent == null)
            {
                labelTextComponent = labelObject.AddComponent<Text>();
                labelTextComponent.text = labelText;
                labelTextComponent.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                labelTextComponent.fontSize = 18;
                labelTextComponent.color = Color.white;
                labelTextComponent.alignment = TextAnchor.MiddleLeft;
            }

            // Füge eine RectTransform-Komponente hinzu, falls sie nicht existiert
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            if (labelRect == null)
            {
                labelRect = labelObject.AddComponent<RectTransform>();
            }
            labelRect.sizeDelta = new Vector2(325, 30); // Breite und Höhe des Labels
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0, 0.5f);
            labelRect.pivot = new Vector2(0, 0.5f);

            // Füge ein LayoutElement hinzu, falls es nicht existiert
            LayoutElement layoutElement = labelObject.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = labelObject.AddComponent<LayoutElement>();
                layoutElement.minWidth = 325; // Mindestbreite des Input-Felds
                layoutElement.preferredWidth = 325; // Bevorzugte Breite des Input-Felds
                layoutElement.minHeight = 30; // Mindesthöhe des Input-Felds
                layoutElement.preferredHeight = 30; // Bevorzugte Höhe des Input-Felds
            }

            // Erstelle ein neues GameObject für das Inputfeld
            GameObject inputObject = new GameObject($"{labelText} Input");
            inputObject.transform.SetParent(container.transform, false);

            // Füge ein Hintergrundbild hinzu, falls es nicht existiert
            Image backgroundImage = inputObject.GetComponent<Image>();
            if (backgroundImage == null)
            {
                backgroundImage = inputObject.AddComponent<Image>();

                if (inputBackgroundSprite == null)
                {
                    inputBackgroundSprite = EmbeddedAssets.LoadUIElementSprite("InputBackground");
                }
                if (inputBackgroundSprite != null)
                {
                    backgroundImage.sprite = inputBackgroundSprite;
                }
                else
                {
                    backgroundImage.color = ColorUtil.GetColor("Grey");
                }
            }

            // Füge eine InputField-Komponente hinzu, falls sie nicht existiert
            InputField inputFieldComponent = inputObject.GetComponent<InputField>();
            if (inputFieldComponent == null)
            {
                inputFieldComponent = inputObject.AddComponent<InputField>();
            }

            // Stelle sicher, dass das RectTransform des Input-Felds korrekt ist
            RectTransform inputRect = inputObject.GetComponent<RectTransform>();
            if (inputRect == null)
            {
                inputRect = inputObject.AddComponent<RectTransform>();
            }
            inputRect.sizeDelta = new Vector2(100, 30); // Setze die Größe des Input-Felds auf die gleiche Größe wie der Hintergrund
            inputRect.anchorMin = new Vector2(0, 0.5f);
            inputRect.anchorMax = new Vector2(0, 0.5f);
            inputRect.pivot = new Vector2(0, 0.5f);
            inputRect.anchoredPosition = new Vector2(0, 0);

            // Füge ein LayoutElement hinzu, falls es nicht existiert
            LayoutElement layoutElementInput = inputObject.GetComponent<LayoutElement>();
            if (layoutElementInput == null)
            {
                layoutElementInput = inputObject.AddComponent<LayoutElement>();
                layoutElementInput.minWidth = 100; // Mindestbreite des Input-Felds
                layoutElementInput.preferredWidth = 100; // Bevorzugte Breite des Input-Felds
                layoutElementInput.minHeight = 30; // Mindesthöhe des Input-Felds
                layoutElementInput.preferredHeight = 30; // Bevorzugte Höhe des Input-Felds
            }

            // Erstelle ein neues GameObject für den Text des Inputfelds
            GameObject inputTextObject = new GameObject($"{labelText} Input Text");
            inputTextObject.transform.SetParent(inputObject.transform, false);

            // Füge eine Text-Komponente hinzu, falls sie nicht existiert
            Text inputTextComponent = inputTextObject.GetComponent<Text>();
            if (inputTextComponent == null)
            {
                inputTextComponent = inputTextObject.AddComponent<Text>();
                inputTextComponent.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                inputTextComponent.fontSize = 18;
                inputTextComponent.color = ColorUtil.GetColor("Cyan");
                inputTextComponent.alignment = TextAnchor.MiddleRight;
            }

            // Weise den Text dem InputField zu
            inputFieldComponent.textComponent = inputTextComponent;

            inputFieldComponent.characterLimit = 6; // Maximal 6 Zeichen eingeben
            inputFieldComponent.text = "6666";

            void FuncThatCallsFunc(string value) => onEndEditCheck(inputFieldComponent, value, labelText);
            inputFieldComponent.onEndEdit.AddListener((UnityAction<string>)FuncThatCallsFunc);

            inputFieldComponent.lineType = InputField.LineType.SingleLine; // Einzeiliges Eingabefeld
            inputFieldComponent.contentType = InputField.ContentType.IntegerNumber; // Nur Zahlen erlauben

            // Erstelle ein neues GameObject für das "$"-Symbol
            GameObject prefixObject = new GameObject($"{labelText} Prefix");

            // Setze die Größe des Prefix-Objekts
            prefixObject.transform.SetParent(inputObject.transform, false);

            // Füge eine Text-Komponente für das "$"-Symbol hinzu
            Text prefixTextComponent = prefixObject.AddComponent<Text>();
            prefixTextComponent.text = prefixText;
            prefixTextComponent.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            prefixTextComponent.fontSize = 18;
            prefixTextComponent.color = ColorUtil.GetColor("Cyan");
            prefixTextComponent.alignment = TextAnchor.MiddleLeft;

            // Positioniere das "$"-Symbol innerhalb des Inputfelds
            RectTransform prefixRect = prefixObject.GetComponent<RectTransform>();
            if (prefixRect == null)
            {
                prefixRect = prefixObject.AddComponent<RectTransform>();
            }
            prefixRect.sizeDelta = new Vector2(20, 30); // Leicht nach rechts verschoben
            prefixRect.anchorMin = new Vector2(0, 0.5f);
            prefixRect.anchorMax = new Vector2(0, 0.5f);
            prefixRect.pivot = new Vector2(0, 0.5f);
            prefixRect.anchoredPosition = new Vector2(5, 0);

            // Verschiebe den Text des Inputfelds nach rechts, um Platz für das "$"-Symbol zu schaffen
            RectTransform inputTextRect = inputTextObject.GetComponent<RectTransform>();
            if (inputTextRect != null)
            {
                inputTextRect.sizeDelta = new Vector2(100, 30); // Setze die Größe des Text-Objekts auf die gleiche Größe wie das Input-Feld
                inputTextRect.offsetMin = new Vector2(-20, inputTextRect.offsetMin.y);
                inputTextRect.offsetMax = new Vector2(40, inputTextRect.offsetMax.y); // Verschiebe den linken Rand nach rechts
            }

            //  MelonLogger.Msg($"Added Label, Prefix '$', and InputField for '{labelText}' to Horizontal Container.");

        }

        void onEndEditCheck(InputField inputFieldComponent, string value, string labelText)
        {
            if (appIconSprite == null)
            {
                appIconSprite = EmbeddedAssets.LoadAppIcon();
            }

            string subTitleString; //= $"for <color=#329AC5>{displayName}</color>";

            string input = value.Trim(); // Entferne Leerzeichen am Anfang und Ende der Eingabe
            if (labelText == "Cooldown Time")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 500
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && parsedValue > 600)
                {
                    inputFieldComponent.text = "500";
                    if (MRSCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Maximum is <color=#329AC5>600 seconds</color>";
                        MRSCore.Instance.notificationsManager.SendNotification("Cooldown Time", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Cooldown Time was > 600. Applying '500 seconds' as value.");
                }
                else if (parsedValue < 60)
                {
                    inputFieldComponent.text = "500";
                    if (MRSCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>at least 60 seconds</color>";
                        MRSCore.Instance.notificationsManager.SendNotification("Cooldown Time", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Cooldown Time was < 60. Applying '500 seconds' as value.");
                }
            }
            else if (labelText == "Forced Sleep Delay")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 60
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && (parsedValue < 0 || parsedValue > 1000))
                {
                    inputFieldComponent.text = "0";
                    if (MRSCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>0 to 1000 seconds</color>";
                        MRSCore.Instance.notificationsManager.SendNotification("Forced Sleep Delay", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Forced Sleep Delay was < 0 or > 1000. Applying '0 seconds' as value.");
                }
            }
            else if (labelText == "Positive Effects Duration")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 60
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && parsedValue < 10)
                {
                    inputFieldComponent.text = "60";
                    if (MRSCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>at least 10 seconds</color>";
                        MRSCore.Instance.notificationsManager.SendNotification("Positive Effects Duration", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Positive Effects Duration was < 60. Applying '60 seconds' as value.");
                }
            }
            else if (labelText == "Negative Effects Duration")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 60
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && parsedValue < 10)
                {
                    inputFieldComponent.text = "60";
                    if (MRSCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>at least 10 seconds</color>";
                        MRSCore.Instance.notificationsManager.SendNotification("Negative Effects Duration", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Negative Effects Duration was < 10. Applying '60 seconds' as value.");
                }
            }
            else if (labelText == "Positive Effects Probability")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 0
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && (parsedValue < 1 || parsedValue > 100))
                {
                    inputFieldComponent.text = "25";
                    if (MRSCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>0 to 100 %</color>";
                        MRSCore.Instance.notificationsManager.SendNotification("Positive Effects Probability", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Positive Effects Probability was < 1 or > 100. Applying '25' as value.");
                }
            }
            else if (labelText == "Negative Effects Probability")
            {
                if (string.IsNullOrEmpty(input) || float.TryParse(input, out float parsedValue) && (parsedValue < 1 || parsedValue > 100))
                {
                    inputFieldComponent.text = "50";
                    if (MRSCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>0 to 100 %</color>";
                        MRSCore.Instance.notificationsManager.SendNotification("Negative Effects Probability", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Negative Effects Probability < 1 or > 100. Applying '50 %' as value.");
                }
            }
            else if (labelText == "Event Probability")
            {
                if (string.IsNullOrEmpty(input) || float.TryParse(input, out float parsedValue) && (parsedValue < 1 || parsedValue > 100))
                {
                    inputFieldComponent.text = "10";
                    if (MRSCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>0 to 100 %</color>";
                        MRSCore.Instance.notificationsManager.SendNotification("Event Probability", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Event Probability < 1 or > 100. Applying '10 %' as value.");
                }
            }

        }

        private void SetInputFieldValue(Transform parentTransform, string displayName, float value)
        {
            Transform inputFieldTransform = parentTransform.Find($"{displayName} Horizontal Container/{displayName} Input");
            if (inputFieldTransform == null)
            {
                MelonLogger.Error($"Transform not found for {displayName}. Skipping.");
                return;
            }

            InputField inputField = inputFieldTransform.GetComponent<InputField>();
            if (inputField == null)
            {
                MelonLogger.Error($"InputField component not found for {displayName}. Skipping.");
                return;
            }

            inputField.text = value.ToString();
        }

        private void SetCheckboxValue(Transform parentTransform, string displayName, bool value)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                MelonLogger.Error("Display name is null or empty!");
                return;
            }
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null! Cannot set checkbox value.");
                return;
            }

            Transform checkboxTransform = parentTransform.Find($"{displayName} Checkbox");
            if (checkboxTransform != null)
            {

                Toggle toggle = checkboxTransform.GetComponent<Toggle>();
                if (toggle != null)
                {
                    if (displayName == "Check All")
                    {
                        toggle.onValueChanged.RemoveAllListeners(); // Remove Listener to prevent unwanted toggles
                        toggle.isOn = value;
                        string sSection = null;
                        if (parentTransform == positiveEffectsObject.transform)
                        {
                            sSection = "PositiveEffects";
                        }
                        else if (parentTransform == negativeEffectsObject.transform)
                        {
                            sSection = "NegativeEffects";
                        }
                        void FuncThatCallsFunc(bool isAllOn) => ToggleAllEffects(checkboxTransform.gameObject, isAllOn, sSection);
                        toggle.onValueChanged.AddListener((UnityAction<bool>)FuncThatCallsFunc);
                    }
                    else if (displayName == "Check Lethal")
                    {
                        toggle.onValueChanged.RemoveAllListeners(); // Remove Listener to prevent unwanted toggles
                        toggle.isOn = value;
                        void FuncThatCallsFunc(bool isLethalOn) => ToggleLethalEffects(checkboxTransform.gameObject, isLethalOn);
                        toggle.onValueChanged.AddListener((UnityAction<bool>)FuncThatCallsFunc);
                    }
                    else if (displayName == "Check Useless")
                    {
                        toggle.onValueChanged.RemoveAllListeners(); // Remove Listener to prevent unwanted toggles
                        toggle.isOn = value;
                        string sSection = null;
                        if (parentTransform == positiveEffectsObject.transform)
                        {
                            sSection = "PositiveEffects";
                        }
                        else if (parentTransform == negativeEffectsObject.transform)
                        {
                            sSection = "NegativeEffects";
                        }
                        void FuncThatCallsFunc(bool isUselessOn) => ToggleUselessEffects(checkboxTransform.gameObject, isUselessOn, sSection);
                        toggle.onValueChanged.AddListener((UnityAction<bool>)FuncThatCallsFunc);
                    }
                    else
                    {
                        toggle.isOn = value;
                    }
                }
            }
            else
            {
                MelonLogger.Error($"Checkbox transform not found for {displayName}.");
            }



        }

        void SaveGeneralSettings(Text buttonText)
        {
            if (isSaveStillRunning)
            {
                MelonLogger.Msg("Save is still running. Please wait a few seconds before saving again.");
                return;
            }

            if (generalSettingsObject == null)
            {
                MelonLogger.Error("GeneralSettingsObject is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            Transform generalSetttingsTransform = generalSettingsObject.transform;
            if (generalSetttingsTransform == null)
            {
                MelonLogger.Error("GeneralSettingsObject transform is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            isSaveStillRunning = true;
            buttonText.text = "Saving...";
            // Deaktiviere den Save-Button
            Button saveButton = buttonText.GetComponentInParent<Button>();
            if (saveButton != null)
            {
                saveButton.interactable = false;
            }
            MRSCore.Instance.StopAllCoroutines();

            // Greife auf die Eingabefelder zu und speichere die Werte
            Transform sleepDelayInputTransform = generalSetttingsTransform.Find("Forced Sleep Delay Horizontal Container/Forced Sleep Delay Input");
            Transform cooldownInputTransform = generalSetttingsTransform.Find("Cooldown Time Horizontal Container/Cooldown Time Input");
            Transform posEffectDurationInputTransform = generalSetttingsTransform.Find("Positive Effects Duration Horizontal Container/Positive Effects Duration Input");
            Transform posEffectProbabilityInputTransform = generalSetttingsTransform.Find("Positive Effects Probability Horizontal Container/Positive Effects Probability Input");
            Transform negEffectDurationInputTransform = generalSetttingsTransform.Find("Negative Effects Duration Horizontal Container/Negative Effects Duration Input");
            Transform negEffectProbabilityInputTransform = generalSetttingsTransform.Find("Negative Effects Probability Horizontal Container/Negative Effects Probability Input");

            float currentSleepDelay = MRSCore.Instance.config.SleepSettings.Forced_Sleep_Delay;
            float currentCooldownTime = MRSCore.Instance.config.SleepSettings.Cooldown_Time;
            float currentPosEffectDuration = MRSCore.Instance.config.SleepSettings.Positive_Effects_Duration;
            float currentPosEffectProbability = MRSCore.Instance.config.SleepSettings.Positive_Effects_Probability;
            float currentNegEffectDuration = MRSCore.Instance.config.SleepSettings.Negative_Effects_Duration;
            float currentNegEffectProbability = MRSCore.Instance.config.SleepSettings.Negative_Effects_Probability;

            // Aktualisiere Forced Sleep Delay
            if (sleepDelayInputTransform != null)
            {
                InputField sleepDelayInputField = sleepDelayInputTransform.GetComponent<InputField>();
                if (sleepDelayInputField != null && float.TryParse(sleepDelayInputField.text, out float parsedSleepDelay))
                {
                    currentSleepDelay = parsedSleepDelay;
                }
            }
            MRSCore.Instance.config.SleepSettings.Forced_Sleep_Delay = currentSleepDelay;

            // Aktualisiere Cooldown Time
            if (cooldownInputTransform != null)
            {
                InputField cooldownInputField = cooldownInputTransform.GetComponent<InputField>();
                if (cooldownInputField != null && float.TryParse(cooldownInputField.text, out float parsedCooldownTime))
                {
                    currentCooldownTime = parsedCooldownTime;
                }
            }
            MRSCore.Instance.config.SleepSettings.Cooldown_Time = currentCooldownTime;

            // Aktualisiere Positive Effects Duration
            if (posEffectDurationInputTransform != null)
            {
                InputField posEffectDurationInputField = posEffectDurationInputTransform.GetComponent<InputField>();
                if (posEffectDurationInputField != null && float.TryParse(posEffectDurationInputField.text, out float parsedPosEffectDuration))
                {
                    currentPosEffectDuration = parsedPosEffectDuration;
                }
            }
            MRSCore.Instance.config.SleepSettings.Positive_Effects_Duration = currentPosEffectDuration;

            // Aktualisiere Positive Effects Probability
            if (posEffectProbabilityInputTransform != null)
            {
                InputField posEffectProbabilityInputField = posEffectProbabilityInputTransform.GetComponent<InputField>();
                if (posEffectProbabilityInputField != null && float.TryParse(posEffectProbabilityInputField.text, out float parsedPosEffectProbability))
                {
                    currentPosEffectProbability = parsedPosEffectProbability;
                }
            }
            MRSCore.Instance.config.SleepSettings.Positive_Effects_Probability = currentPosEffectProbability;

            // Aktualisiere Negative Effects Duration
            if (negEffectDurationInputTransform != null)
            {
                InputField negEffectDurationInputField = negEffectDurationInputTransform.GetComponent<InputField>();
                if (negEffectDurationInputField != null && float.TryParse(negEffectDurationInputField.text, out float parsedNegEffectDuration))
                {
                    currentNegEffectDuration = parsedNegEffectDuration;
                }
            }
            MRSCore.Instance.config.SleepSettings.Negative_Effects_Duration = currentNegEffectDuration;

            // Aktualisiere Negative Effects Probability
            if (negEffectProbabilityInputTransform != null)
            {
                InputField negEffectProbabilityInputField = negEffectProbabilityInputTransform.GetComponent<InputField>();
                if (negEffectProbabilityInputField != null && float.TryParse(negEffectProbabilityInputField.text, out float parsedNegEffectProbability))
                {
                    currentNegEffectProbability = parsedNegEffectProbability;
                }
            }
            MRSCore.Instance.config.SleepSettings.Negative_Effects_Probability = currentNegEffectProbability;

            // Aktualisiere die Checkboxen

            bool isForcedSleepChecked = generalSettingsObject.transform.Find("Forced Sleep Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.SleepSettings.Enable_Forced_Sleep = isForcedSleepChecked;

            bool isAutoSkipChecked = generalSettingsObject.transform.Find("Auto Skip Daily Summary Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.SleepSettings.Auto_Skip_Daily_Summary = isAutoSkipChecked;

            bool isPositiveEffectsChecked = generalSettingsObject.transform.Find("Positive Effects Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.SleepSettings.Enable_Positive_Effects = isPositiveEffectsChecked;

            bool isNegativeEffectsChecked = generalSettingsObject.transform.Find("Negative Effects Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.SleepSettings.Enable_Negative_Effects = isNegativeEffectsChecked;

            bool isNotificationsChecked = generalSettingsObject.transform.Find("Notifications Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.SleepSettings.Enable_Effect_Notifications = isNotificationsChecked;

            // Speichere die aktualisierte Config
            ConfigManager.Save(MRSCore.Instance.config);

            // Zeige eine Benachrichtigung an
            if (MRSCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Config saved";
                if (settingsSprite == null)
                {
                    settingsSprite = EmbeddedAssets.LoadUIElementSprite("Settings");
                }
                MRSCore.Instance.notificationsManager.SendNotification("General Settings", subTitleString, settingsSprite, 5, true);
            }

            // MRSCore.Instance.monitorTimeForSleepCoroutine = (Coroutine)MelonCoroutines.Start(MRSCore.Instance.MonitorTimeForSleep());

            // Reaktiviere den Save-Button
            if (saveButton != null)
            {

                saveButton.interactable = true;
            }

            if (buttonText != null)
            {
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
            }
        }


        void AddUiForGeneralSettings(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null!");
                return;
            }

            generalSettingsObject = new GameObject("GeneralSettings");
            //  LayoutElement layoutElement = generalSettingsObject.AddComponent<LayoutElement>();
            VerticalLayoutGroup contentVerticalLayout = generalSettingsObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = generalSettingsObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform generalSettingsRect = generalSettingsObject.GetComponent<RectTransform>();
            if (generalSettingsRect != null)
            {
                generalSettingsRect.offsetMin = new Vector2(60, generalSettingsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = generalSettingsObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = generalSettingsObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite
            generalSettingsObject.transform.SetParent(parentTransform, false);


            GameObject enableForcedSleepCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, generalSettingsObject.transform);
            enableForcedSleepCheckbox.name = "Forced Sleep Checkbox";
            Transform enableForcedSleepCheckboxTextTransform = enableForcedSleepCheckbox.transform.Find("Text");
            enableForcedSleepCheckboxTextTransform.GetComponent<Text>().text = "Enable Forced Sleep";

            GameObject enableAutoSleepCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, generalSettingsObject.transform);
            enableAutoSleepCheckbox.name = "Auto Skip Daily Summary Checkbox";
            Transform enableAutoSleepCheckboxTextTransform = enableAutoSleepCheckbox.transform.Find("Text");
            enableAutoSleepCheckboxTextTransform.GetComponent<Text>().text = "Auto Skip Daily Summary";

            AddLabelInputPair("Forced Sleep Delay", generalSettingsObject.transform, "s");
            AddLabelInputPair("Cooldown Time", generalSettingsObject.transform, "s");

            AddSeperatorLine(generalSettingsObject.transform, 455, 2, "Light Grey");

            GameObject posEffectsCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, generalSettingsObject.transform);
            posEffectsCheckbox.name = "Positive Effects Checkbox";
            Transform posEffectsCheckboxTextTransform = posEffectsCheckbox.transform.Find("Text");
            posEffectsCheckboxTextTransform.GetComponent<Text>().text = "Enable Positive Effects";

            AddLabelInputPair("Positive Effects Duration", generalSettingsObject.transform, "s");
            AddLabelInputPair("Positive Effects Probability", generalSettingsObject.transform, "%");

            GameObject negEffectsCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, generalSettingsObject.transform);
            negEffectsCheckbox.name = "Negative Effects Checkbox";
            Transform negEffectsCheckboxTextTransform = negEffectsCheckbox.transform.Find("Text");
            negEffectsCheckboxTextTransform.GetComponent<Text>().text = "Enable Negative Effects";

            AddLabelInputPair("Negative Effects Duration", generalSettingsObject.transform, "s");
            AddLabelInputPair("Negative Effects Probability", generalSettingsObject.transform, "%");

            GameObject notificationsCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, generalSettingsObject.transform);
            notificationsCheckbox.name = "Notifications Checkbox";
            Transform notificationsCheckboxTextTransform = notificationsCheckbox.transform.Find("Text");
            notificationsCheckboxTextTransform.GetComponent<Text>().text = "Enable Notifications";

            AddSaveButton(generalSettingsObject.transform, "GeneralSettings"); // Add the save button to the GeneralSettings object

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            generalSettingsObject.SetActive(false);
        }

        void AddUiForPositiveEffects(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null!");
                return;
            }

            positiveEffectsObject = new GameObject("PositiveEffects");
            //  LayoutElement layoutElement = generalSettingsObject.AddComponent<LayoutElement>();
            VerticalLayoutGroup contentVerticalLayout = positiveEffectsObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = positiveEffectsObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform positiveEffectSettingsRect = positiveEffectsObject.GetComponent<RectTransform>();
            if (positiveEffectSettingsRect != null)
            {
                positiveEffectSettingsRect.offsetMin = new Vector2(60, positiveEffectSettingsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = positiveEffectsObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = positiveEffectsObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite
            positiveEffectsObject.transform.SetParent(parentTransform, false);


            // New UI Elements

            // Add Check All Button
            GameObject checkAllCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, positiveEffectsObject.transform);
            checkAllCheckbox.name = "Check All Checkbox";
            Transform checkAllCheckboxTextTransform = checkAllCheckbox.transform.Find("Text");
            checkAllCheckboxTextTransform.GetComponent<Text>().text = "Toggle all Effects";
            void FuncThatCallsFunc(bool isOn) => ToggleAllEffects(checkAllCheckbox, isOn, "PositiveEffects");
            checkAllCheckbox.GetComponent<Toggle>().onValueChanged.AddListener((UnityAction<bool>)FuncThatCallsFunc);

            // Create Toggle Useless Button
            GameObject toggleUselessCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, positiveEffectsObject.transform);
            toggleUselessCheckbox.name = "Check Useless Checkbox";
            Transform toggleUselessCheckboxTextTransform = toggleUselessCheckbox.transform.Find("Text");
            toggleUselessCheckboxTextTransform.GetComponent<Text>().text = "Toggle Useless";
            void FuncThatCallsFunc2(bool isOn) => ToggleUselessEffects(toggleUselessCheckbox, isOn, "PositiveEffects");
            toggleUselessCheckbox.GetComponent<Toggle>().onValueChanged.AddListener((UnityAction<bool>)FuncThatCallsFunc2);

            AddSeperatorLine(positiveEffectsObject.transform, 455, 2, "Light Grey");

            foreach (string effectName in ConfigManager.GetPositiveEffectNames())
            {
                // Erstelle eine neue Checkbox basierend auf dem Template
                GameObject effectCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, positiveEffectsObject.transform);
                effectCheckbox.name = $"{effectName} Checkbox";

                // Setze den Text der Checkbox
                Transform effectCheckboxTextTransform = effectCheckbox.transform.Find("Text");
                if (effectCheckboxTextTransform != null)
                {
                    Text checkboxText = effectCheckboxTextTransform.GetComponent<Text>();
                    if (checkboxText != null)
                    {
                        checkboxText.text = $"Enable {effectName}";
                    }
                    else
                    {
                        MelonLogger.Warning($"Text component not found for {effectName} Checkbox.");
                    }
                }
                else
                {
                    MelonLogger.Warning($"Text transform not found for {effectName} Checkbox.");
                }
            }

            AddSaveButton(positiveEffectsObject.transform, "PositiveEffects"); // Add the save button to the GeneralSettings object

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            positiveEffectsObject.SetActive(false);
        }

        void SavePositiveEffects(Text buttonText)
        {
            if (isSaveStillRunning)
            {
                MelonLogger.Msg("Save is still running. Please wait a few seconds before saving again.");
                return;
            }

            if (positiveEffectsObject == null)
            {
                MelonLogger.Error("PositiveEffectsObject is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            Transform positiveEffectsTransform = positiveEffectsObject.transform;
            if (positiveEffectsTransform == null)
            {
                MelonLogger.Error("PositiveEffectsObject transform is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            isSaveStillRunning = true;
            buttonText.text = "Saving...";

            // Deaktiviere den Save-Button
            Button saveButton = buttonText.GetComponentInParent<Button>();
            if (saveButton != null)
            {
                saveButton.interactable = false;
            }
            MRSCore.Instance.StopAllCoroutines();

            bool isAntiGravityChecked = positiveEffectsObject.transform.Find("Anti Gravity Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Anti_Gravity = isAntiGravityChecked;

            bool isAthleticChecked = positiveEffectsObject.transform.Find("Athletic Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Athletic = isAthleticChecked;

            bool isBrightEyedChecked = positiveEffectsObject.transform.Find("Bright Eyed Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Bright_Eyed = isBrightEyedChecked;

            bool isCalmingChecked = positiveEffectsObject.transform.Find("Calming Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Calming = isCalmingChecked;

            bool isCalorieDenseChecked = positiveEffectsObject.transform.Find("Calorie Dense Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Calorie_Dense = isCalorieDenseChecked;

            bool isElectrifyingChecked = positiveEffectsObject.transform.Find("Electrifying Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Electrifying = isElectrifyingChecked;

            bool isEnergizingChecked = positiveEffectsObject.transform.Find("Energizing Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Energizing = isEnergizingChecked;

            bool isEuphoricChecked = positiveEffectsObject.transform.Find("Euphoric Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Euphoric = isEuphoricChecked;

            bool isFocusedChecked = positiveEffectsObject.transform.Find("Focused Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Focused = isFocusedChecked;

            bool isMunchiesChecked = positiveEffectsObject.transform.Find("Munchies Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Munchies = isMunchiesChecked;

            bool isRefreshingChecked = positiveEffectsObject.transform.Find("Refreshing Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Refreshing = isRefreshingChecked;

            bool isSneakyChecked = positiveEffectsObject.transform.Find("Sneaky Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.PositiveEffectSettings.Sneaky = isSneakyChecked;

            // Speichere die aktualisierte Config
            ConfigManager.Save(MRSCore.Instance.config);

            // Zeige eine Benachrichtigung an
            if (MRSCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Config saved";
                if (positiveEffectsSprite == null)
                {
                    positiveEffectsSprite = EmbeddedAssets.LoadUIElementSprite("PositiveEffects");
                }
                MRSCore.Instance.notificationsManager.SendNotification("Positive Effects", subTitleString, positiveEffectsSprite, 5, true);
            }

            // Reaktiviere den Save-Button
            if (saveButton != null)
            {

                saveButton.interactable = true;
            }

            if (buttonText != null)
            {
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
            }
        }

        void AddUiForNegativeEffects(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null!");
                return;
            }

            negativeEffectsObject = new GameObject("NegativeEffects");
            //  LayoutElement layoutElement = generalSettingsObject.AddComponent<LayoutElement>();
            VerticalLayoutGroup contentVerticalLayout = negativeEffectsObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = negativeEffectsObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform negativeEffectSettingsRect = negativeEffectsObject.GetComponent<RectTransform>();
            if (negativeEffectSettingsRect != null)
            {
                negativeEffectSettingsRect.offsetMin = new Vector2(60, negativeEffectSettingsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = negativeEffectsObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = negativeEffectsObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite
            negativeEffectsObject.transform.SetParent(parentTransform, false);


            // New UI Elements

            //Add Check All Button
            GameObject checkAllCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, negativeEffectsObject.transform);
            checkAllCheckbox.name = "Check All Checkbox";
            Transform checkAllCheckboxTextTransform = checkAllCheckbox.transform.Find("Text");
            checkAllCheckboxTextTransform.GetComponent<Text>().text = "Toggle all Effects";
            void FuncThatCallsFunc(bool isAllOn) => ToggleAllEffects(checkAllCheckbox, isAllOn, "NegativeEffects");
            checkAllCheckbox.GetComponent<Toggle>().onValueChanged.AddListener((UnityAction<bool>)FuncThatCallsFunc);

            /* This Button is not needed for now since there is no useless effects for the negative effects
            // Create Toggle Useless Button
            GameObject toggleUselessCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, negativeEffectsObject.transform);
            toggleUselessCheckbox.name = "Check Useless Checkbox";
            Transform toggleUselessCheckboxTextTransform = toggleUselessCheckbox.transform.Find("Text");
            toggleUselessCheckboxTextTransform.GetComponent<Text>().text = "Toggle Useless";
            void FuncThatCallsFunc2(bool isUselessOn) => ToggleUselessEffects(toggleUselessCheckbox, isUselessOn, "NegativeEffects");
            toggleUselessCheckbox.GetComponent<Toggle>().onValueChanged.AddListener((UnityAction<bool>)FuncThatCallsFunc2); */

            // Create Toggle Lethal Button
            GameObject toggleLethalCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, negativeEffectsObject.transform);
            toggleLethalCheckbox.name = "Check Lethal Checkbox";
            Transform toggleLethalCheckboxTextTransform = toggleLethalCheckbox.transform.Find("Text");
            toggleLethalCheckboxTextTransform.GetComponent<Text>().text = "Toggle Lethal";
            void FuncThatCallsFunc3(bool isLethalOn) => ToggleLethalEffects(toggleLethalCheckbox, isLethalOn);
            toggleLethalCheckbox.GetComponent<Toggle>().onValueChanged.AddListener((UnityAction<bool>)FuncThatCallsFunc3);

            AddSeperatorLine(negativeEffectsObject.transform, 455, 2, "Light Grey");

            foreach (string effectName in ConfigManager.GetNegativeEffectNames())
            {
                // Erstelle eine neue Checkbox basierend auf dem Template
                GameObject effectCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, negativeEffectsObject.transform);
                effectCheckbox.name = $"{effectName} Checkbox";

                // Setze den Text der Checkbox
                Transform effectCheckboxTextTransform = effectCheckbox.transform.Find("Text");
                if (effectCheckboxTextTransform != null)
                {
                    Text checkboxText = effectCheckboxTextTransform.GetComponent<Text>();
                    if (checkboxText != null)
                    {
                        checkboxText.text = $"Enable {effectName}";
                    }
                    else
                    {
                        MelonLogger.Warning($"Text component not found for {effectName} Checkbox.");
                    }
                }
                else
                {
                    MelonLogger.Warning($"Text transform not found for {effectName} Checkbox.");
                }
            }

            AddSaveButton(negativeEffectsObject.transform, "NegativeEffects"); // Add the save button to the GeneralSettings object

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            negativeEffectsObject.SetActive(false);
        }

        private void ToggleAllEffects(GameObject checkAllCheckbox, bool booleanValue, string sSection)
        {
            //MelonLogger.Msg($"ToggleAllEffects called with value: {booleanValue}");
            Transform parentTransform = checkAllCheckbox.transform.parent;
            // Iteriere durch alle Kinder von negativeEffectsObject
            if (sSection == "PositiveEffects")
            {
                foreach (string effectName in ConfigManager.GetPositiveEffectNames())
                {
                    GameObject effectCheckbox = parentTransform.Find($"{effectName} Checkbox").gameObject;
                    if (effectCheckbox == null)
                    {
                        MelonLogger.Error($"Checkbox for {effectName} not found.");
                        continue;
                    }
                    Toggle toggle = effectCheckbox.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = booleanValue;
                    }
                }
            }
            else if (sSection == "NegativeEffects")
            {
                foreach (string effectName in ConfigManager.GetNegativeEffectNames())
                {
                    GameObject effectCheckbox = parentTransform.Find($"{effectName} Checkbox").gameObject;
                    if (effectCheckbox == null)
                    {
                        MelonLogger.Error($"Checkbox for {effectName} not found.");
                        continue;
                    }
                    Toggle toggle = effectCheckbox.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = booleanValue;
                    }
                }
            }
        }

        private void ToggleLethalEffects(GameObject checkLethalCheckbox, bool booleanValue)
        {
            //MelonLogger.Msg($"ToggleLethalEffects called with value: {booleanValue}");
            Transform parentTransform = checkLethalCheckbox.transform.parent;
            // Iteriere durch alle Kinder
            foreach (string effectName in ConfigManager.GetLethalEffectNames())
            {
                GameObject effectCheckbox = parentTransform.Find($"{effectName} Checkbox").gameObject;
                if (effectCheckbox == null)
                {
                    MelonLogger.Error($"Checkbox for {effectName} not found.");
                    continue;
                }
                Toggle toggle = effectCheckbox.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = booleanValue;
                }
            }
        }

        private void ToggleUselessEffects(GameObject checkUselessCheckbox, bool booleanValue, string sSection)
        {
            // MelonLogger.Msg($"ToggleAllEffects called with value: {booleanValue}");
            Transform parentTransform = checkUselessCheckbox.transform.parent;
            // Iteriere durch alle Kinder von negativeEffectsObject
            if (sSection == "PositiveEffects")
            {
                foreach (string effectName in ConfigManager.GetUselessEffectNames())
                {
                    GameObject effectCheckbox = parentTransform.Find($"{effectName} Checkbox").gameObject;
                    if (effectCheckbox == null)
                    {
                        MelonLogger.Error($"Checkbox for {effectName} not found.");
                        continue;
                    }
                    Toggle toggle = effectCheckbox.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = booleanValue;
                    }
                }
            }
            else if (sSection == "NegativeEffects")
            {
                foreach (string effectName in ConfigManager.GetNegativeEffectNames())
                {
                    GameObject effectCheckbox = parentTransform.Find($"{effectName} Checkbox").gameObject;
                    if (effectCheckbox == null)
                    {
                        MelonLogger.Error($"Checkbox for {effectName} not found.");
                        continue;
                    }
                    Toggle toggle = effectCheckbox.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = booleanValue;
                    }
                }
            }
        }

        void SaveNegativeEffects(Text buttonText)
        {
            if (isSaveStillRunning)
            {
                MelonLogger.Msg("Save is still running. Please wait a few seconds before saving again.");
                return;
            }

            if (negativeEffectsObject == null)
            {
                MelonLogger.Error("NegativeEffectsObject is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            Transform negativeEffectsTransform = negativeEffectsObject.transform;
            if (negativeEffectsTransform == null)
            {
                MelonLogger.Error("NegativeEffectsObject transform is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            isSaveStillRunning = true;
            buttonText.text = "Saving...";

            // Deaktiviere den Save-Button
            Button saveButton = buttonText.GetComponentInParent<Button>();
            if (saveButton != null)
            {
                saveButton.interactable = false;
            }
            MRSCore.Instance.StopAllCoroutines();

            // Checkboxen für negative Effekte speichern
            bool isBaldingChecked = negativeEffectsObject.transform.Find("Balding Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Balding = isBaldingChecked;

            bool isBrightEyedChecked = negativeEffectsObject.transform.Find("Bright Eyed Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Bright_Eyed = isBrightEyedChecked;

            bool isCalmingChecked = negativeEffectsObject.transform.Find("Calming Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Calming = isCalmingChecked;

            bool isCalorieDenseChecked = negativeEffectsObject.transform.Find("Calorie Dense Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Calorie_Dense = isCalorieDenseChecked;

            bool isCyclopeanChecked = negativeEffectsObject.transform.Find("Cyclopean Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Cyclopean = isCyclopeanChecked;

            bool isDisorientingChecked = negativeEffectsObject.transform.Find("Disorienting Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Disorienting = isDisorientingChecked;

            bool isElectrifyingChecked = negativeEffectsObject.transform.Find("Electrifying Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Electrifying = isElectrifyingChecked;

            bool isExplosiveChecked = negativeEffectsObject.transform.Find("Explosive Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Explosive = isExplosiveChecked;

            bool isFoggyChecked = negativeEffectsObject.transform.Find("Foggy Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Foggy = isFoggyChecked;

            bool isGingeritisChecked = negativeEffectsObject.transform.Find("Gingeritis Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Gingeritis = isGingeritisChecked;

            bool isGlowingChecked = negativeEffectsObject.transform.Find("Glowing Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Glowing = isGlowingChecked;

            bool isJennerisingChecked = negativeEffectsObject.transform.Find("Jennerising Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Jennerising = isJennerisingChecked;

            bool isLaxativeChecked = negativeEffectsObject.transform.Find("Laxative Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Laxative = isLaxativeChecked;

            bool isLethalChecked = negativeEffectsObject.transform.Find("Lethal Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Lethal = isLethalChecked;

            bool isLongFacedChecked = negativeEffectsObject.transform.Find("Long Faced Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Long_Faced = isLongFacedChecked;

            bool isParanoiaChecked = negativeEffectsObject.transform.Find("Paranoia Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Paranoia = isParanoiaChecked;

            bool isSchizophrenicChecked = negativeEffectsObject.transform.Find("Schizophrenic Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Schizophrenic = isSchizophrenicChecked;

            bool isSedatingChecked = negativeEffectsObject.transform.Find("Sedating Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Sedating = isSedatingChecked;

            bool isSeizureInducingChecked = negativeEffectsObject.transform.Find("Seizure Inducing Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Seizure_Inducing = isSeizureInducingChecked;

            bool isShrinkingChecked = negativeEffectsObject.transform.Find("Shrinking Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Shrinking = isShrinkingChecked;

            bool isSlipperyChecked = negativeEffectsObject.transform.Find("Slippery Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Slippery = isSlipperyChecked;

            bool isSmellyChecked = negativeEffectsObject.transform.Find("Smelly Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Smelly = isSmellyChecked;

            bool isSpicyChecked = negativeEffectsObject.transform.Find("Spicy Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Spicy = isSpicyChecked;

            bool isThoughtProvokingChecked = negativeEffectsObject.transform.Find("Thought Provoking Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Thought_Provoking = isThoughtProvokingChecked;

            bool isToxicChecked = negativeEffectsObject.transform.Find("Toxic Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Toxic = isToxicChecked;

            bool isTropicThunderChecked = negativeEffectsObject.transform.Find("Tropic Thunder Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Tropic_Thunder = isTropicThunderChecked;

            bool isZombifyingChecked = negativeEffectsObject.transform.Find("Zombifying Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.EffectSettings.NegativeEffectSettings.Zombifying = isZombifyingChecked;

            // Speichere die aktualisierte Config
            ConfigManager.Save(MRSCore.Instance.config);

            // Zeige eine Benachrichtigung an
            if (MRSCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Config saved";
                if (negativeEffectsSprite == null)
                {
                    negativeEffectsSprite = EmbeddedAssets.LoadUIElementSprite("NegativeEffects");
                }
                MRSCore.Instance.notificationsManager.SendNotification("Negative Effects", subTitleString, negativeEffectsSprite, 5, true);
            }

            // Reaktiviere den Save-Button
            if (saveButton != null)
            {
                saveButton.interactable = true;
            }

            if (buttonText != null)
            {
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
            }
        }

        void SaveArrestedEvent(Text buttonText)
        {
            if (isSaveStillRunning)
            {
                MelonLogger.Msg("Save is still running. Please wait a few seconds before saving again.");
                return;
            }

            if (arrestedEventObject == null)
            {
                MelonLogger.Error("ArrestedEventObject is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            Transform arrestedEventTransform = arrestedEventObject.transform;
            if (arrestedEventTransform == null)
            {
                MelonLogger.Error("ArrestedEventObject transform is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            isSaveStillRunning = true;
            buttonText.text = "Saving...";

            // Deaktiviere den Save-Button
            Button saveButton = buttonText.GetComponentInParent<Button>();
            if (saveButton != null)
            {
                saveButton.interactable = false;
            }
            MRSCore.Instance.StopAllCoroutines();

            // Greife auf die Eingabefelder zu und speichere die Werte
            Transform eventProbabilityTransform = arrestedEventTransform.Find("Event Probability Horizontal Container/Event Probability Input");

            float currentEventProbability = MRSCore.Instance.config.ArrestedEventSettings.GetArrested_Event_Probability;

            // Aktualisiere GetArrested_Event_Probability
            if (eventProbabilityTransform != null)
            {
                InputField eventProbabilityInputField = eventProbabilityTransform.GetComponent<InputField>();
                if (eventProbabilityInputField != null && float.TryParse(eventProbabilityInputField.text, out float parsedProbability))
                {
                    currentEventProbability = parsedProbability;
                }
            }
            MRSCore.Instance.config.ArrestedEventSettings.GetArrested_Event_Probability = currentEventProbability;

            // Checkboxen für arrested event speichern
            bool isArrestedEventChecked = arrestedEventObject.transform.Find("Arrested Event Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.ArrestedEventSettings.Enable_GetArrested_Event = isArrestedEventChecked;

            bool isSaveSpacesChecked = arrestedEventObject.transform.Find("Save Spaces Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.ArrestedEventSettings.Enable_GetArrested_Event_SaveSpaces = isSaveSpacesChecked;

            // Speichere die aktualisierte Config
            ConfigManager.Save(MRSCore.Instance.config);

            // Zeige eine Benachrichtigung an
            if (MRSCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Config saved";
                if (arrestedEventSprite == null)
                {
                    arrestedEventSprite = EmbeddedAssets.LoadUIElementSprite("ArrestedEvent");
                }
                MRSCore.Instance.notificationsManager.SendNotification("Arrested Event", subTitleString, arrestedEventSprite, 5, true);
            }

            // Reaktiviere den Save-Button
            if (saveButton != null)
            {
                saveButton.interactable = true;
            }

            if (buttonText != null)
            {
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
            }
        }

        void AddUIForArrestedEvent(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null!");
                return;
            }

            arrestedEventObject = new GameObject("ArrestedEvent");

            VerticalLayoutGroup contentVerticalLayout = arrestedEventObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = arrestedEventObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform negativeEffectSettingsRect = arrestedEventObject.GetComponent<RectTransform>();
            if (negativeEffectSettingsRect != null)
            {
                negativeEffectSettingsRect.offsetMin = new Vector2(60, negativeEffectSettingsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = arrestedEventObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = arrestedEventObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite
            arrestedEventObject.transform.SetParent(parentTransform, false);


            // New UI Elements
            GameObject enableArrestedEventCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, arrestedEventObject.transform);
            enableArrestedEventCheckbox.name = "Arrested Event Checkbox";
            Transform enableArrestedEventCheckboxTextTransform = enableArrestedEventCheckbox.transform.Find("Text");
            enableArrestedEventCheckboxTextTransform.GetComponent<Text>().text = "Enable Arrested Event";

            GameObject enableSaveSpacesCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, arrestedEventObject.transform);
            enableSaveSpacesCheckbox.name = "Save Spaces Checkbox";
            Transform enableSaveSpacesCheckboxTextTransform = enableSaveSpacesCheckbox.transform.Find("Text");
            enableSaveSpacesCheckboxTextTransform.GetComponent<Text>().text = "Enable Save Spaces";

            AddLabelInputPair("Event Probability", arrestedEventObject.transform, "%");

            AddSaveButton(arrestedEventObject.transform, "ArrestedEvent"); // Add the save button to the GeneralSettings object

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            arrestedEventObject.SetActive(false);
        }

        void AddUIForMurderedEvent(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null!");
                return;
            }

            murderedEventObject = new GameObject("MurderedEvent");

            VerticalLayoutGroup contentVerticalLayout = murderedEventObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = murderedEventObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform murderedEventSettingsRect = murderedEventObject.GetComponent<RectTransform>();
            if (murderedEventSettingsRect != null)
            {
                murderedEventSettingsRect.offsetMin = new Vector2(60, murderedEventSettingsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = murderedEventObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = murderedEventObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite
            murderedEventObject.transform.SetParent(parentTransform, false);


            // New UI Elements
            GameObject enableMurderedEventCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, murderedEventObject.transform);
            enableMurderedEventCheckbox.name = "Murdered Event Checkbox";
            Transform enableMurderedEventCheckboxTextTransform = enableMurderedEventCheckbox.transform.Find("Text");
            enableMurderedEventCheckboxTextTransform.GetComponent<Text>().text = "Enable Murdered Event";

            GameObject enableRespawningCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, murderedEventObject.transform);
            enableRespawningCheckbox.name = "Respawning Checkbox";
            Transform enableRespawningCheckboxTextTransform = enableRespawningCheckbox.transform.Find("Text");
            enableRespawningCheckboxTextTransform.GetComponent<Text>().text = "Allow Respawning";

            GameObject enableSaveSpacesCheckbox = UnityEngine.Object.Instantiate(checkboxTemplate, murderedEventObject.transform);
            enableSaveSpacesCheckbox.name = "Save Spaces Checkbox";
            Transform enableSaveSpacesCheckboxTextTransform = enableSaveSpacesCheckbox.transform.Find("Text");
            enableSaveSpacesCheckboxTextTransform.GetComponent<Text>().text = "Enable Save Spaces";

            AddLabelInputPair("Event Probability", murderedEventObject.transform, "%");

            AddSaveButton(murderedEventObject.transform, "MurderedEvent"); // Add the save button to the GeneralSettings object

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            murderedEventObject.SetActive(false);
        }

        void SaveMurderedEvent(Text buttonText)
        {
            if (isSaveStillRunning)
            {
                MelonLogger.Msg("Save is still running. Please wait a few seconds before saving again.");
                return;
            }

            if (murderedEventObject == null)
            {
                MelonLogger.Error("murderedEventObject is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            Transform murderedEventTransform = murderedEventObject.transform;
            if (murderedEventTransform == null)
            {
                MelonLogger.Error("murderedEventObject transform is null! Cannot save settings.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            isSaveStillRunning = true;
            buttonText.text = "Saving...";

            // Deaktiviere den Save-Button
            Button saveButton = buttonText.GetComponentInParent<Button>();
            if (saveButton != null)
            {
                saveButton.interactable = false;
            }
            MRSCore.Instance.StopAllCoroutines();

            // Greife auf die Eingabefelder zu und speichere die Werte
            Transform eventProbabilityTransform = murderedEventTransform.Find("Event Probability Horizontal Container/Event Probability Input");

            float currentEventProbability = MRSCore.Instance.config.MurderedEventSettings.GetMurdered_Event_Probability;

            // Aktualisiere GetArrested_Event_Probability
            if (eventProbabilityTransform != null)
            {
                InputField eventProbabilityInputField = eventProbabilityTransform.GetComponent<InputField>();
                if (eventProbabilityInputField != null && float.TryParse(eventProbabilityInputField.text, out float parsedProbability))
                {
                    currentEventProbability = parsedProbability;
                }
            }
            MRSCore.Instance.config.MurderedEventSettings.GetMurdered_Event_Probability = currentEventProbability;

            // Checkboxen für arrested event speichern
            bool isMurderedEventChecked = murderedEventObject.transform.Find("Murdered Event Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.MurderedEventSettings.Enable_GetMurdered_Event = isMurderedEventChecked;

            bool isRespawningChecked = murderedEventObject.transform.Find("Respawning Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.MurderedEventSettings.Enable_GetMurdered_Event_SaveSpaces = isRespawningChecked;

            bool isSaveSpacesChecked = murderedEventObject.transform.Find("Save Spaces Checkbox").GetComponent<Toggle>().isOn;
            MRSCore.Instance.config.MurderedEventSettings.Enable_GetMurdered_Event_SaveSpaces = isSaveSpacesChecked;


            // Speichere die aktualisierte Config
            ConfigManager.Save(MRSCore.Instance.config);

            // Zeige eine Benachrichtigung an
            if (MRSCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Config saved";
                if (murderedEventSprite == null)
                {
                    murderedEventSprite = EmbeddedAssets.LoadUIElementSprite("MurderedEvent");
                }
                MRSCore.Instance.notificationsManager.SendNotification("Murdered Event", subTitleString, murderedEventSprite, 5, true);
            }

            // Reaktiviere den Save-Button
            if (saveButton != null)
            {
                saveButton.interactable = true;
            }

            if (buttonText != null)
            {
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
            }
        }




        public bool _isSleepingAppLoaded = false;
        private bool isSaveStillRunning = false;
        private Transform sleepingAppViewportContentTransform;
        public GameObject DansHardwareTemplate;
        public GameObject GasMartWestTemplate;
        private GameObject generalSettingsObject;
        private GameObject positiveEffectsObject;
        private GameObject negativeEffectsObject;
        private GameObject arrestedEventObject;
        private GameObject murderedEventObject;
        private GameObject checkboxTemplate;
        private GameObject viewPortContentSpaceTemplate;
        private GameObject detailsTitleObject;
        private GameObject detailsSubtitleObject;
        private Transform settingsContentTransform;
        private GameObject InstructionsTextObject;
        private Sprite appIconSprite;
        private Sprite saveButtonSprite;
        private Sprite inputBackgroundSprite;
        private Sprite settingsSprite;
        private Sprite positiveEffectsSprite;
        private Sprite negativeEffectsSprite;
        private Sprite arrestedEventSprite;
        private Sprite murderedEventSprite;
    }
}