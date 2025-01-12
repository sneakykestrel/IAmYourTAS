using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using IAmYourTAS.Components;
using IAmYourTAS.UI;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniverseLib;
using UniverseLib.Config;

namespace IAmYourTAS;

[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
public class Mod : BaseUnityPlugin
{
    public const string pluginGuid = "kestrel.iamyourbeast.iamyourtas";
    public const string pluginName = "I Am Your TAS";
    public const string pluginVersion = "1.0.0";

    public static Mod Instance {  get; private set; }
    private static IAYTInputManager InputManager { get; set; } = new();
    internal static new ManualLogSource Logger;

    public static readonly string pluginPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
    public static readonly string defaultVarHooksPath = Path.Combine(pluginPath, "defaultvarhooks.txt");

    private static string loadBearingColonThree = ":3";
    public void Awake() {
        if (loadBearingColonThree != ":3") Application.Quit();

        Instance = this;
        Logger = base.Logger;
        InputManager.BindToConfig(Config);
        SceneManager.sceneLoaded += FullbrightManager.OnSceneLoaded;
        new Harmony(pluginGuid).PatchAll();

        // *probably* the only settings universelib stuff will want in iayb.
        // might cause issues later though
        var config = new UniverseLibConfig {
            Force_Unlock_Mouse = false,
            Disable_EventSystem_Override = true
        };
        Universe.Init(startupDelay: 0f, OnUniverseInit, ULogHandler, config);

        Logger.LogInfo("Hiiiiiiiiiiii :3");
    }

    private static void ULogHandler(string msg, LogType type) {
        switch (type) {
            case LogType.Error:
                Logger.LogError(msg);
                break;
            case LogType.Assert:
                Logger.LogDebug(msg);
                break;
            case LogType.Warning:
                Logger.LogWarning(msg);
                break;
            case LogType.Log:
                Logger.LogInfo(msg);
                break;
            case LogType.Exception:
                Logger.LogFatal(msg);
                break;
            default:
                Logger.LogInfo(msg);
                break;
        }
    }

    public void Update() {
        VariableHookManager.Update();
        InputManager.Update();
    }

    private void OnUniverseInit() {
        UIManager.Init();
    }

    [HarmonyPatch(typeof(HUDLevelTimer), "Update")]
    public class The
    {
        [HarmonyPostfix]
        public static void Postfix(ref TMP_Text ___timerText) {
            if (ShotgunSpreadManager.AmountPerfectlyAccurate != 0) {
                int idx = ___timerText.text.LastIndexOf('.');
                ___timerText.text = ___timerText.text.Remove(idx, 1).Insert(idx, ":");
            }
        }
    }
}
