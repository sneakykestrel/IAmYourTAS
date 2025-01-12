using Enemy;
using HarmonyLib;
using UnityEngine;

namespace IAmYourTAS.Components;
public static class ObjectVisibilityManager
{
    private static bool showWalls = true;

    public static void SetWallVisibility(bool value) {
        showWalls = value;
        if (value) {
            GameManager.instance.cameraManager.GetManagersCamera().cullingMask |= GameManager.instance.layermasks.levelGeometry;
        } else {
            GameManager.instance.cameraManager.GetManagersCamera().cullingMask &= ~GameManager.instance.layermasks.levelGeometry;
        }
    }

    [HarmonyPatch(typeof(CameraManager))]
    public class CameraManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void SetVisibilityOnInit(ref Camera ___myCamera) {
            if (!showWalls)
                ___myCamera.cullingMask &= ~GameManager.instance.layermasks.levelGeometry;
        }
    }

    // Fix enemies' layers as they're on the default layer usually for some reason
    [HarmonyPatch(typeof(EnemyHuman))]
    public class EnemyHumanPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void FixLayers(EnemyHuman __instance) {
            var obj = __instance.transform.Find("Animator/Enemy:Enemy_mesh").gameObject;
            obj.layer = LayerMask.NameToLayer("Enemy");
        }
    }
}
