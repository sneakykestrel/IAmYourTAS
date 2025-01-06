using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace IAmYourTAS;

static class TimescaleManager
{
    private static TimeScale timeScale;
    private static TimeScale pauseScale;
    private static float scaleAmount = 1;
    private static bool timePaused = false;
    private static bool overrideTimeManager = false;

    public static void UpdateScale(float scale) {
        scaleAmount = scale;
        if (GameManager.instance.timeManager == null) return;
        timeScale.SetScale(scaleAmount);
    }

    public static void SetTimePaused(bool value) {
        timePaused = value;
        if (GameManager.instance.timeManager == null) return;
        pauseScale.SetScale(timePaused ? 0 : 1);
    }

    // Skip frames 
    public static IEnumerator AdvanceFrames(int numFrames) {
        // Are we actually paused?
        if (Time.timeScale != 0) yield break;
        //Mod.Logger.LogFatal($"Starting frame advance of {numFrames} frames on frame {Time.frameCount}");
        int stopFrame = Time.frameCount + numFrames;

        overrideTimeManager = true;
        Time.timeScale = scaleAmount;
        yield return new WaitUntil(() => Time.frameCount >= stopFrame);
        overrideTimeManager = false;
        //Mod.Logger.LogFatal($"Stopped frame advance on frame {Time.frameCount}");
    }

    // Skips all timemanager timescale modifications while overrideTimeManager is true
    [HarmonyPatch(typeof(TimeManager))]
    public class TimeManagerPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool OverrideTimeManager() {
            return !overrideTimeManager; 
        }

    }

    // TimeManager is most likely initialized once LevelController.Initialize is called
    // So it's reasonable to init timescales here
    [HarmonyPatch(typeof(LevelController))]
    public class LevelControllerPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void InitScale() {
            timeScale = GameManager.instance.timeManager.CreateTimeScale(scaleAmount);
            pauseScale = GameManager.instance.timeManager.CreateTimeScale(timePaused?0:1);
        }
    }

}
