using IAmYourTAS.Components;
using IAmYourTAS.Util;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;

namespace IAmYourTAS.UI;
public class VariableHookCell : ICell
{
    public bool Enabled { get; }
    public RectTransform Rect { get; set; }
    public float DefaultHeight => 25f;
    public GameObject UIRoot { get; set; }

    public int currentIdx = 0;

    public Text contentsText;
    public InputFieldRef pathInput;

    public void Enable() {
        UIRoot.SetActive(true);
    }

    public void Disable() {
        UIRoot.SetActive(false);
    }

    private void Path_OnValueChanged(string path) {
        VariableHookManager.Hooks[currentIdx].path = path;
    }

    public GameObject CreateContent(GameObject parent) {
        UIRoot = UIFactory.CreateHorizontalGroup(parent, "Variable Cell", true, false, true, true, bgColor: UIManager.MainColor);
        Rect = UIRoot.GetComponent<RectTransform>();

        UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

        pathInput = UIFactory.CreateInputField(UIRoot, "Path input", "Variable path");
        UIFactoryUtils.CenterInputFieldText(pathInput);
        pathInput.PlaceholderText.alignment = TextAnchor.MiddleCenter;
        UIFactory.SetLayoutElement(pathInput.GameObject, minHeight: 25, flexibleWidth: 9999);
        pathInput.OnValueChanged += Path_OnValueChanged;

        UIFactoryUtils.AddSpacer(UIRoot, width: 5);

        contentsText = UIFactory.CreateLabel(UIRoot, "Var Contents", "null", TextAnchor.MiddleCenter);
        UIFactory.SetLayoutElement(contentsText.gameObject, minWidth: 120, minHeight: 25, flexibleWidth: 0);
        contentsText.horizontalOverflow = HorizontalWrapMode.Overflow;

        UIFactoryUtils.AddSpacer(UIRoot, width: 5);

        var delBtn = UIFactory.CreateButton(UIRoot, "Delete Button", "-", UIManager.DefaultColorBlockRed);
        UIFactory.SetLayoutElement(delBtn.Component.gameObject, minWidth: 25, minHeight: 25);
        delBtn.Component.transform.Find("Text").GetComponent<Text>().fontSize = 20;
        delBtn.OnClick = () => VariableHooksPanel.OnDeleteClicked(this);

        return UIRoot;
    }
}
