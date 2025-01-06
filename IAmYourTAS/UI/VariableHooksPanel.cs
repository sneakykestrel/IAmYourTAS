using IAmYourTAS.Components;
using IAmYourTAS.Util;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Panels;
using UniverseLib.UI.Widgets.ScrollView;

namespace IAmYourTAS.UI;
public class VariableHooksPanel(UIBase owner) : PanelBase(owner), ICellPoolDataSource<VariableHookCell> {

    public override string Name => $"{Mod.pluginName} ~ Variable Hooks";
    public override int MinWidth => 300;
    public override int MinHeight => 250;
    public override Vector2 DefaultAnchorMin => new(0.015f, 0.7f);
    public override Vector2 DefaultAnchorMax => new(0.35f, 0.985f);
    public override bool CanDragAndResize => true;

    public int ItemCount { get => VariableHookManager.Hooks.Count; }

    public static ScrollPool<VariableHookCell> scrollPool;

    public void OnCellBorrowed(VariableHookCell cell) { }

    public static void OnDeleteClicked(VariableHookCell cell) {
        VariableHookManager.Hooks.RemoveAt(cell.currentIdx);
        cell.Disable();
        scrollPool.Refresh(true, false);
    }

    public override void Update() {
        if (!Enabled) return;
        foreach (var cell in scrollPool.CellPool) {
            if (cell.currentIdx >= VariableHookManager.Hooks.Count) continue;
            cell.contentsText.text = VariableHookManager.Hooks[cell.currentIdx].contents;
        }
    }

    public void SetCell(VariableHookCell cell, int index) {
        if (index >= ItemCount) {
            cell.Disable();
            return;
        }
        cell.currentIdx = index;
        cell.pathInput.Text = VariableHookManager.Hooks[index].path;
    }

    public override void SetDefaultSizeAndPosition() {
        base.SetDefaultSizeAndPosition();
        Rect.anchoredPosition = Vector2.zero;

        Dragger.OnEndResize();
    }

    protected override void ConstructPanelContent() {
        TitleBar.transform.Find("CloseHolder").gameObject.SetActive(false);
        TitleBar.GetComponent<Image>().color = UIManager.AccentColor;

        scrollPool = UIFactory.CreateScrollPool<VariableHookCell>(
            ContentRoot,
            "Variable Hooks",
            out GameObject scrollObj,
            out GameObject scrollContent
        );
        scrollPool.Initialize(this);

        var buttonsRow = UIFactory.CreateHorizontalGroup(ContentRoot, "Buttons", false, false, true, true, bgColor: new(1, 1, 1, 0));

        var btn = UIFactory.CreateButton(buttonsRow, "New Hook Button", "+", UIManager.DefaultColorBlock);
        btn.Component.transform.Find("Text").GetComponent<Text>().fontSize = 20;
        UIFactory.SetLayoutElement(btn.GameObject, minWidth: 25, minHeight: 25);
        btn.OnClick = () => {
            VariableHookManager.Hooks.Add(new VariableHook());
            scrollPool.Refresh(true, false);
        };

        UIFactoryUtils.AddSpacer(buttonsRow, width: 5);

        btn = UIFactory.CreateButton(buttonsRow, "Save Hooks Button", "Save as defaults", UIManager.DefaultColorBlock);
        UIFactory.SetLayoutElement(btn.GameObject, minWidth: 125, minHeight: 25);
        btn.OnClick = VariableHookManager.SaveHooksToFile;

        UIFactoryUtils.AddSpacer(buttonsRow, width: 5);

        btn = UIFactory.CreateButton(buttonsRow, "Load Hooks Button", "Restore defaults", UIManager.DefaultColorBlock);
        UIFactory.SetLayoutElement(btn.GameObject, minWidth: 125, minHeight: 25);
        btn.OnClick = VariableHookManager.LoadHooksFromFile;
    }
}