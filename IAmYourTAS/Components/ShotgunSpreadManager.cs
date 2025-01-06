using HarmonyLib;
using Projectiles;
using UnityEngine;

namespace IAmYourTAS.Components;
public class ShotgunSpreadManager
{
    public static int AmountPerfectlyAccurate { get; set; } = 0;

    [HarmonyPatch(typeof(ProjectileBurst))]
    static class ProjectileBurstPatch
    {
        [HarmonyPatch("Fire")]
        [HarmonyPrefix]
        public static void ModifyAccuracy(Vector3 muzzleOrigin, CharacterOrigin instigator, Transform worldTransform, PlayerWeaponArmed playerWeapon, ref ProjectileBurst __instance, ref bool __state) {
            if (instigator == CharacterOrigin.Player) {
                var componentsInChildren = __instance.gameObject.GetComponentsInChildren<Projectile>(true);
                int len = componentsInChildren.Length;

                for (int i = 0; i < Mathf.Clamp(AmountPerfectlyAccurate, 0, len-1)+1; ++i) {
                    if (componentsInChildren[i] == __instance) continue;
                    componentsInChildren[i].transform.localEulerAngles *= 0;
                    if (componentsInChildren[i].Fire(muzzleOrigin, instigator, worldTransform, playerWeapon)) {
                        __state = true;
                    }
                    componentsInChildren[i].gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPatch("Fire")]
        [HarmonyPostfix]
        public static void ModifyRes(ref bool __result, ref bool __state) {
            if (!__result && __state) __result = true;
        }
    }
}
