using HarmonyLib;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace IAmYourTAS.UI;

public static class UIManager {
    private static bool m_forceShowUI;
    public static bool ForceShowUI {
        get => m_forceShowUI;
        set {
            UIBase.Enabled = value;
            m_forceShowUI = value;
        }
    }
    public static UIBase UIBase { get; private set; }
    public static MainPanel MainPanelInstance { get; private set; }
    public static VariableHooksPanel HooksPanelInstance { get; private set; }

    public static Color SecondaryColor { get; private set; } = new Color32(130, 154, 177, 255); // Bright
    public static Color MainColor { get; private set; } = new Color32(72, 101, 129, 255);
    public static Color AccentColor { get; private set; } = new Color32(16, 42, 67, 255); // Dark

    public static ColorBlock DefaultColorBlock { get; private set; } = new ColorBlock 
    {
        normalColor = MainColor,
        disabledColor = MainColor,
        selectedColor = SecondaryColor,
        highlightedColor = SecondaryColor,
        pressedColor = AccentColor,
        colorMultiplier = 1f,
        fadeDuration = 0.1f
    };

    public static ColorBlock DefaultColorBlockRed { get; private set; } = new ColorBlock
    {
        normalColor = new Color32(201, 24, 74, 255),
        disabledColor = new Color32(201, 24, 74, 255),
        selectedColor = new Color32(255, 77, 109, 255),
        highlightedColor = new Color32(255, 77, 109, 255),
        pressedColor = new Color32(128, 15, 47, 255),
        colorMultiplier = 1f,
        fadeDuration = 0.1f
    };
    
    internal static void Init() {
        UIBase = UniversalUI.RegisterUI(Mod.pluginGuid, Update);
        MainPanelInstance = new(UIBase);
        HooksPanelInstance = new(UIBase) { Enabled = false };
        UIBase.Enabled = false;
    }

    static void Update() {
        HooksPanelInstance?.Update();
    }

    public static void ShowUI() {
        UIBase.Enabled = true;
    }

    // Hides the ui, unless it should be forced open
    public static void HideUI() {
        if (m_forceShowUI) return;
        UIBase.Enabled = false;
    }

    [HarmonyPatch(typeof(UIPauseMenu))]
    public class PauseMenuPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void OnStart() => ShowUI();

        [HarmonyPatch("Close")]
        [HarmonyPrefix]
        public static void OnClose() => HideUI();

        [HarmonyPatch("RestartLevel")]
        [HarmonyPrefix]
        public static void OnRestart() => HideUI();

        [HarmonyPatch("ReturnToLevelSelect")]
        [HarmonyPrefix]
        public static void OnLevelSelect() => HideUI();
    }
}
