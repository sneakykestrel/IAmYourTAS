using IAmYourTAS.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace IAmYourTAS.Util;
internal static class UIFactoryUtils
{
    public static void AddSpacer(GameObject root, int height = 0, int width = 0) {
        GameObject obj = UIFactory.CreateUIObject("Spacer", root);
        UIFactory.SetLayoutElement(obj, minHeight: height, minWidth: width, flexibleHeight: 0, flexibleWidth: 0);
    }

    public static void AddCategoryTitle(GameObject root, string name) {
        var label = UIFactory.CreateLabel(root, name + " Title", name, TextAnchor.MiddleCenter);
        UIFactory.SetLayoutElement(label.gameObject, minHeight: 25, flexibleWidth: 9999);
    }

    public static ButtonRef AddSimpleButton(GameObject root, string name, Action onClick, int? minWidth = 70, int? minHeight = 25, int? flexibleWidth = 140) {
        var btn = UIFactory.CreateButton(root, name + " Button", name, UIManager.DefaultColorBlock);
        btn.OnClick = onClick;
        UIFactory.SetLayoutElement(btn.GameObject, minWidth, minHeight, flexibleWidth);
        return btn;
    }

    public static void CenterInputFieldText(InputFieldRef field) {
        field.GameObject.transform.Find("TextArea/Text").GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
    }

    public static void CreateSliderWithReadout(GameObject root, string labelText, out UnityEngine.UI.Slider slider, out InputFieldRef inputField, Action<float> onSliderChanged, Action<string> onReadoutChanged, float sliderMin = 0, float sliderMax = 1) {
        var row = UIFactory.CreateHorizontalGroup(root, labelText+" Slider", false, false, true, true, bgColor: new(1, 1, 1, 0));
        var label = UIFactory.CreateLabel(row, "Label", labelText);
        UIFactory.SetLayoutElement(label.gameObject, minWidth: 90, minHeight: 25);

        UIFactory.CreateSlider(row, "Slider", out slider);
        UIFactory.SetLayoutElement(slider.gameObject, minWidth: 70, minHeight: 25, flexibleWidth: 9999);
        slider.value = 1;
        slider.minValue = sliderMin;
        slider.maxValue = sliderMax;
        slider.onValueChanged.AddListener(onSliderChanged);
        slider.colors = UIManager.DefaultColorBlock;
        slider.image.color = Color.white;

        AddSpacer(row, width: 10);

        inputField = UIFactory.CreateInputField(row, "ScaleReadout", "");
        CenterInputFieldText(inputField);
        inputField.Text = slider.value.ToString("F2");
        UIFactory.SetLayoutElement(inputField.GameObject, minWidth: 30, minHeight: 25, flexibleWidth: 100);
        inputField.OnValueChanged += onReadoutChanged;
    }

}
