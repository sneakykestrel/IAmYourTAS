using HarmonyLib;

namespace IAmYourTAS.Components;
public class LevelTimeSaveManager
{
    public static bool saveTimes = false;

    public static void SetShouldSaveTimes(bool value) => saveTimes = value;

    [HarmonyPatch(typeof(UILevelCompleteTimeScoreBar))]
    public class TimeScoreBarPatch
    {

        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static void InitPre(ref object __state) {
            __state = GameManager.instance.progressManager.GetLevelData
                (GameManager.instance.levelController.GetInformationSetter().GetInformation())
                .GetBestTime();
        }

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void InitPost(ref object __state) {
            if (!saveTimes) {
                GameManager.instance.progressManager.GetLevelData
                    (GameManager.instance.levelController.GetInformationSetter().GetInformation())
                    .SetNewBestTime((float)__state);
            }
        }
    }
}
