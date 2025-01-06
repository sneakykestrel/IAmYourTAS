using BepInEx.Configuration;
using IAmYourTAS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace IAmYourTAS.Components;

public class IAYTInputManager {
    public struct DefaultBindingInfo(string identifier, Action action, string description = "", KeyCode key = KeyCode.None)
    {
        public Action action = action;
        public string identifier = identifier;
        public string description = description;
        public KeyCode key = key;
    }

    public static List<DefaultBindingInfo> DefaultBindings { get; private set; } = [
        new(
            identifier: "Open Menu",
            description: "The key that opens the ingame menu (it'll be automatically opened when the game is paused too)",
            key: KeyCode.F9,
            action: () => UIManager.ForceShowUI = !UIManager.ForceShowUI
        ),
        new(
            identifier: "Frame Advance",
            description: "An optional shortcut to frame advance",
            action: () => UIManager.MainPanelInstance.frameAdvanceButton.OnClick()
        ),
        new(
            identifier: "Pause Time",
            description: "An optional shortcut to pause time",
            action: () => Toggle(UIManager.MainPanelInstance.pauseTimeToggle)
        ),
        new(
            identifier: "Toggle Fullbright",
            description: "An optional shortcut to toggle fullbright",
            action: () => Toggle(UIManager.MainPanelInstance.fullbrightToggle)
        ),
        new(
            identifier: "Save Location",
            description: "An optional shortcut to save your location",
            action: LocationManager.SaveLocation
        ),
        new(
            identifier: "Load Location",
            description: "An optional shortcut to load your saved location",
            action: LocationManager.RestoreLocation
        ),
        new(
            identifier: "Clear Saved Location",
            description: "An optional shortcut to clear your saved location",
            action: LocationManager.ClearLocation
        ),
        new(
            identifier: "Toggle Wall Visibility",
            description: "An optional shortcut to toggle the visibility of walls",
            action: () => Toggle(UIManager.MainPanelInstance.wallVisibilityToggle)
        ),
        new(
            identifier: "Set timescale to 1x",
            description: "An optional shortcut to set the timescale to 1x",
            action: () => UIManager.MainPanelInstance.timeScaleInput.Text = "1"
        ),
        new(
            identifier: "Set timescale to .5x",
            description: "An optional shortcut to set the timescale to .5x",
            action: () => UIManager.MainPanelInstance.timeScaleInput.Text = "0.5"
        ),
        new(
            identifier: "Set timescale to .25x",
            description: "An optional shortcut to set the timescale to .25x",
            action: () => UIManager.MainPanelInstance.timeScaleInput.Text = "0.25"
        ),
        new(
            identifier: "Set timescale to .1x",
            description: "An optional shortcut to set the timescale to .1x",
            action: () => UIManager.MainPanelInstance.timeScaleInput.Text = "0.1"
        ),
        new(
            identifier: "Set timescale to .05x",
            description: "An optional shortcut to set the timescale to .05x",
            action: () => UIManager.MainPanelInstance.timeScaleInput.Text = "0.05"
        ),
    ];

    // This could be an extension method but. does it need to be really
    private static void Toggle(Toggle t) => t.isOn = !t.isOn;

    private Dictionary<ConfigEntry<KeyCode>, Action> bindings = [];

    // Bind the default config values to a specified config file
    public void BindToConfig(ConfigFile config) {
        foreach (var defaultBinding in DefaultBindings) {
            var configEntry = config.Bind(
                defaultBinding.key != KeyCode.None ? "Keybinds" : "Keybinds.Optional",
                defaultBinding.identifier,
                defaultBinding.key,
                defaultBinding.description
            );

            bindings[configEntry] = defaultBinding.action;
        }
    }

    public void Update() {
        foreach (var binding in bindings) {
            if (Input.GetKeyDown(binding.Key.Value)) { // lol
                binding.Value?.Invoke();
            }
        }
    }
}
