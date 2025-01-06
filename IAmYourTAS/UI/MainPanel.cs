using IAmYourTAS.Components;
using IAmYourTAS.Util;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace IAmYourTAS.UI;

public class MainPanel(UIBase owner) : UniverseLib.UI.Panels.PanelBase(owner)
{
    public override string Name => $"{Mod.pluginName} v{Mod.pluginVersion}";
    public override int MinWidth => 200;
    public override int MinHeight => 450;
    
    public override Vector2 DefaultAnchorMin => new(0.85f, 0.55f);
    public override Vector2 DefaultAnchorMax => new(0.985f, 0.985f); // new(0.865f, 0.975f);
    public override bool CanDragAndResize => true;

    Slider timeScaleSlider;
    public InputFieldRef timeScaleInput;
    InputFieldRef frameAdvanceInput;
    public InputFieldRef savedPosInput;
    public InputFieldRef savedRotInput;
    public Toggle pauseTimeToggle;
    public Toggle wallVisibilityToggle;
    public Toggle saveTimesToggle;
    public Toggle fullbrightToggle;
    public ButtonRef frameAdvanceButton;

    public override void SetDefaultSizeAndPosition() {
        base.SetDefaultSizeAndPosition();
        Rect.anchoredPosition = Vector2.zero;
        Dragger.OnEndResize();
    }

    protected override void ConstructPanelContent() {
        // Remove close button since we don't want the main panel to close on its own
        TitleBar.transform.Find("CloseHolder").gameObject.SetActive(false);
        TitleBar.GetComponent<Image>().color = UIManager.AccentColor;
        UIFactoryUtils.AddCategoryTitle(ContentRoot, "Time Controls");

        #region Timescale slider

        UIFactoryUtils.CreateSliderWithReadout(ContentRoot, "Timescale: ", out timeScaleSlider, out timeScaleInput, ScaleSlider_OnValueChanged, ScaleReadout_OnValueChanged);
        GameObject row;

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);

        #endregion

        #region Pause time
        UIFactory.CreateToggle(ContentRoot, "Pause Time Toggle", out pauseTimeToggle, out Text pTimeToggleText);
        pauseTimeToggle.isOn = false;
        pauseTimeToggle.onValueChanged.AddListener(TimescaleManager.SetTimePaused);
        pTimeToggleText.text = "Pause Time?";
        pauseTimeToggle.transform.Find("Background").Find("Checkmark").GetComponent<Image>().color = UIManager.MainColor;

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);
        #endregion

        #region Frame advance
        row = UIFactory.CreateHorizontalGroup(ContentRoot, "Frame advance", false, false, true, true, bgColor: new(1, 1, 1, 0));

        frameAdvanceButton = UIFactoryUtils.AddSimpleButton(row, "Advance Frames", FrameAdvance_OnButtonClick);

        UIFactoryUtils.AddSpacer(row, width: 10);

        frameAdvanceInput = UIFactory.CreateInputField(row, "Frame Advance Input", "Frames");
        UIFactory.SetLayoutElement(frameAdvanceInput.GameObject, minWidth: 50, minHeight: 25, flexibleWidth: 100);
        frameAdvanceInput.Text = "1";
        frameAdvanceInput.PlaceholderText.alignment = TextAnchor.MiddleCenter;
        UIFactoryUtils.CenterInputFieldText(frameAdvanceInput);

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);



        void FrameAdvance_OnButtonClick() {
            try {
                int frames = int.Parse(frameAdvanceInput.Text);
                Mod.Instance.StartCoroutine(TimescaleManager.AdvanceFrames(frames));
            } catch {
                frameAdvanceInput.Text = "";
            }
        }
    #endregion

        UIFactoryUtils.AddCategoryTitle(ContentRoot, "Location Controls");

        #region Save/Load location
        row = UIFactory.CreateHorizontalGroup(ContentRoot, "Location Save Buttons", false, false, true, true, bgColor: new(1, 1, 1, 0));

        UIFactoryUtils.AddSimpleButton(row, "Save", LocationManager.SaveLocation);
        UIFactoryUtils.AddSimpleButton(row, "Load", LocationManager.RestoreLocation);
        UIFactoryUtils.AddSimpleButton(row, "Clear", LocationManager.ClearLocation);

        row = UIFactory.CreateHorizontalGroup(ContentRoot, "Location Save Inputs", false, false, true, true, bgColor: new(1, 1, 1, 0));
        savedPosInput = UIFactory.CreateInputField(row, "Position input", "Saved position");
        UIFactoryUtils.CenterInputFieldText(savedPosInput);
        savedPosInput.PlaceholderText.alignment = TextAnchor.MiddleCenter;
        UIFactory.SetLayoutElement(savedPosInput.GameObject, minWidth: 30, minHeight: 25, flexibleWidth: 97);
        savedPosInput.OnValueChanged += SavedPos_OnValueChanged;

        UIFactoryUtils.AddSpacer(row, width: 6);

        savedRotInput = UIFactory.CreateInputField(row, "Rotation input", "Saved rotation");
        UIFactoryUtils.CenterInputFieldText(savedRotInput);
        savedRotInput.PlaceholderText.alignment = TextAnchor.MiddleCenter;
        UIFactory.SetLayoutElement(savedRotInput.GameObject, minWidth: 30, minHeight: 25, flexibleWidth: 97);
        savedRotInput.OnValueChanged += SavedRot_OnValueChanged;

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);
        #endregion

        UIFactoryUtils.AddCategoryTitle(ContentRoot, "Variables");

        #region Variable hooks

        UIFactoryUtils.AddSimpleButton(ContentRoot, "Variable Hooks Panel", ToggleVarHooksPanel, flexibleWidth: 9999);

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);
        #endregion

        UIFactoryUtils.AddCategoryTitle(ContentRoot, "Extras");

        #region No time saving
        UIFactory.CreateToggle(ContentRoot, "Time Saving Toggle", out saveTimesToggle, out Text saveTimesToggleText);
        saveTimesToggle.isOn = false;
        saveTimesToggle.onValueChanged.AddListener(LevelTimeSaveManager.SetShouldSaveTimes);
        saveTimesToggleText.text = "Save Times On Level Complete?";
        saveTimesToggle.transform.Find("Background").Find("Checkmark").GetComponent<Image>().color = UIManager.MainColor;

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);
        #endregion

        #region Fullbright
        UIFactory.CreateToggle(ContentRoot, "Fullbright Toggle", out fullbrightToggle, out Text fullbrightToggleText);
        fullbrightToggle.isOn = false;
        fullbrightToggle.onValueChanged.AddListener(FullbrightManager.Toggle);
        fullbrightToggleText.text = "Enable Fullbright?";
        fullbrightToggle.transform.Find("Background").Find("Checkmark").GetComponent<Image>().color = UIManager.MainColor;

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);
        #endregion

        #region Shotgun utils
        InputFieldRef shotgunShellsAccurateInput = null;

        row = UIFactory.CreateHorizontalGroup(ContentRoot, "Shotgun Accuracy", false, false, true, true, bgColor: new(1, 1, 1, 0));


        shotgunShellsAccurateInput = UIFactory.CreateInputField(row, "ScaleReadout", "");
        UIFactoryUtils.CenterInputFieldText(shotgunShellsAccurateInput);
        shotgunShellsAccurateInput.Text = "0";
        UIFactory.SetLayoutElement(shotgunShellsAccurateInput.GameObject, minWidth: 25, minHeight: 25);
        shotgunShellsAccurateInput.OnValueChanged += AccurateShotgunShellsInput_OnValueChanged;

        UIFactoryUtils.AddSpacer(row, width: 10);
        
        var label = UIFactory.CreateLabel(row, "Label", "Perfect shotgun accuracy (shells/shot)");
        UIFactory.SetLayoutElement(label.gameObject, minWidth: 90, minHeight: 25);

        void AccurateShotgunShellsInput_OnValueChanged(string value) {
            try {
                ShotgunSpreadManager.AmountPerfectlyAccurate = int.Parse(value);
            } catch { }
        };

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);
        #endregion

        #region Wallhacks

        UIFactory.CreateToggle(ContentRoot, "Wall Visibility Toggle", out wallVisibilityToggle, out Text wallVisibilityText);
        wallVisibilityToggle.isOn = true;
        wallVisibilityToggle.onValueChanged.AddListener(ObjectVisibilityManager.SetWallVisibility);
        wallVisibilityText.text = "Show walls?";
        wallVisibilityToggle.transform.Find("Background").Find("Checkmark").GetComponent<Image>().color = UIManager.MainColor;

        UIFactoryUtils.AddSpacer(ContentRoot, height: 5);
        #endregion
    }

    private void ToggleVarHooksPanel() {
        UIManager.HooksPanelInstance.Enabled = !UIManager.HooksPanelInstance.Enabled;
    }

    private void SavedPos_OnValueChanged(string value) {
        if (NullableVec3TryParse(value, out Vector3? vec, out _))
            LocationManager.savedPosition = vec;
    }

    private void SavedRot_OnValueChanged(string value) {
        if (NullableVec3TryParse(value, out Vector3? vec, out _))
            LocationManager.savedRotation = vec;
    }

    private void ScaleSlider_OnValueChanged(float value) {
        timeScaleInput.Text = value.ToString("F2");
        TimescaleManager.UpdateScale(value);
    }

    private void ScaleReadout_OnValueChanged(string value) {
        try {
            timeScaleSlider.value = float.Parse(value);
        } catch { timeScaleInput.Text = timeScaleSlider.value.ToString("F2"); }
    }

    // Parses a vec3 from a string that may be empty
    private bool NullableVec3TryParse(string value, out Vector3? vec, out System.Exception parseException) {
        var tmp = new Vector3();
        vec = null;
        parseException = null;
        if (value == string.Empty) {
            return true;
        }
        try {
            string[] xyz = value.Trim('(', ')').Replace(", ", ",").Split(' ', ',');
            tmp.x = float.Parse(xyz[0].Trim());
            tmp.y = float.Parse(xyz[1].Trim());
            tmp.z = float.Parse(xyz[2].Trim());
            vec = tmp;
        } catch (System.Exception e) {
            parseException = e.GetInnerMostException();
            return false;
        }
        return true;
    }
}
