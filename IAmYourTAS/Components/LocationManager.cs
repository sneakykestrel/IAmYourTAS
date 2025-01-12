using IAmYourTAS.UI;
using UnityEngine;
using UniverseLib.Utility;

namespace IAmYourTAS.Components;
public static class LocationManager
{
    public static Vector3? savedPosition;
    public static Vector3? savedRotation;

    public static void SaveLocation() {
        savedPosition = GameManager.instance.player?.GetPosition();
        savedRotation = GameManager.instance.player?.GetLookScript().GetBaseRotation();
        if (!savedPosition.HasValue || !savedRotation.HasValue) return;

        UIManager.MainPanelInstance.savedPosInput.Text = ParseUtility.ToStringForInput<Vector3>(savedPosition!.Value);
        UIManager.MainPanelInstance.savedRotInput.Text = ParseUtility.ToStringForInput<Vector3>(savedRotation!.Value);
    }

    public static void ClearLocation() {
        savedPosition = null;
        savedRotation = null;
        UIManager.MainPanelInstance.savedPosInput.Text = null;
        UIManager.MainPanelInstance.savedRotInput.Text = null;
    }

    public static void RestoreLocation() {
        if (savedPosition.HasValue)
            GameManager.instance.player?.GetMovementScript().Teleport(savedPosition!.Value);
        if (savedRotation.HasValue)
            GameManager.instance.player?.GetLookScript().SetBaseRotation(savedRotation!.Value);
    }
}
