using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IAmYourTAS.Components;
public static class FullbrightManager
{
    private static Color cachedLightColor = new();
    private static ShadowQuality cachedShadowQuality = new();
    private static Dictionary<Light, bool> cachedLights = [];
    private static bool enabled;

    private static void Enable() {
        cachedLightColor = RenderSettings.ambientLight;
        cachedShadowQuality = QualitySettings.shadows;
        RenderSettings.ambientLight = new Color(1f, 1f, 1f, 1f);
        QualitySettings.shadows = ShadowQuality.Disable;
        foreach (var kv in cachedLights) {
            kv.Key.enabled = false;
        }
    }

    private static void CacheLights() {
        foreach (var light in GameObject.FindObjectsOfType<Light>()) {
            cachedLights[light] = light.enabled;
        }
    }

    public static void Toggle(bool value) {
        enabled = value;
        if (enabled) {
            Enable();
        } else {
            RenderSettings.ambientLight = cachedLightColor;
            QualitySettings.shadows = cachedShadowQuality;
            foreach (var kv in cachedLights) {
                kv.Key.enabled = kv.Value;
            }
        }
    }

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        cachedLights.Clear();
        CacheLights();
        if (enabled) {
            Enable();
        }
    }
}
