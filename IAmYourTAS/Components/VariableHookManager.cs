﻿using HarmonyLib;
using IAmYourTAS.UI;
using System.Collections.Generic;
using System.IO;

namespace IAmYourTAS.Components;
public static class VariableHookManager
{
    static VariableHookManager() {
        LoadHooksFromFile();
    }

    public static List<VariableHook> Hooks { get; private set; } = new();

    public static void AddHook(string path) {
        Hooks.Add(new VariableHook(path));
    }

    public static void Update() {
        if (UIManager.HooksPanelInstance?.Enabled is null or false) return;

        foreach (var hook in Hooks) {
            if (hook.path == "") continue;
            var t = Traverse.Create(GameManager.instance);

            // Very simple, could (and should) improve at a later date
            try {
                foreach (var component in hook.path.Split('.')) {
                    t = component.EndsWith("()") ? 
                        t.Method(component.Substring(0, component.Length - 2)) : 
                        t.Field(component);
                }
                hook.contents = t.GetValue().ToString();
            } catch {
                hook.contents = "null";
            }
        }
    }

    public static void LoadHooksFromFile() {
        if (!File.Exists(Mod.defaultVarHooksPath)) {
            File.Create(Mod.defaultVarHooksPath);
        }
        Hooks.Clear();
        foreach (string path in File.ReadAllLines(Mod.defaultVarHooksPath)) {
            AddHook(path);
        }
        VariableHooksPanel.scrollPool?.Refresh(true, false);
    }

    public static void SaveHooksToFile() {
        using var sw = new StreamWriter(Mod.defaultVarHooksPath, false);
        foreach (var hook in Hooks) {
            sw.WriteLine(hook.path);
        }
    }
}

public class VariableHook(string path = "", string contents = "") {
    public string path = path;
    public string contents = contents;
}
