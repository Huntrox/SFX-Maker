using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SfxSettingUIElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SfxSettingUIElement, UxmlTraits>
    {
    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription titleAttrb = new UxmlStringAttributeDescription
            {name = "title", defaultValue = "Title"};

        UxmlFloatAttributeDescription valueAttrb = new UxmlFloatAttributeDescription {name = "value", defaultValue = 0};

        UxmlFloatAttributeDescription minValueAttrb = new UxmlFloatAttributeDescription
            {name = "minValue", defaultValue = 0};

        UxmlFloatAttributeDescription maxValueAttrb = new UxmlFloatAttributeDescription
            {name = "maxValue", defaultValue = 1};

        UxmlBoolAttributeDescription lockedAttrb = new UxmlBoolAttributeDescription
            {name = "locked", defaultValue = false};


        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as SfxSettingUIElement;
            ate.Clear();

            ate.title = titleAttrb.GetValueFromBag(bag, cc);
            ate.value = valueAttrb.GetValueFromBag(bag, cc);
            ate.minValue = minValueAttrb.GetValueFromBag(bag, cc);
            ate.maxValue = maxValueAttrb.GetValueFromBag(bag, cc);
            ate.locked = lockedAttrb.GetValueFromBag(bag, cc);

            ate.Init();
        }
    }

    private string title;
    private float minValue = 0;
    private float maxValue = 1;
    private float value = 0.5f;
    private bool locked;
    private Action<float> onValueChanged;

    public float MINValue
    {
        get => minValue;
        set => minValue = value;
    }

    public float MAXValue
    {
        get => maxValue;
        set => maxValue = value;
    }

    public float Value
    {
        get => value;
        set => value = value;
    }

    public string Title
    {
        get => title;
        set => title = value;
    }

    public SfxSettingUIElement()
    {
        title = "Volume";
        Init();
    }

    public SfxSettingUIElement(string label, float minVal, float maxVal, float val, bool isLocked,
        Action<float> onValChanged)
    {
        minValue = minVal;
        maxValue = maxVal;
        value = val;
        onValueChanged = onValChanged;
        title = label;
        Init();
    }

    private void Init()
    {
        AddToClassList("sfx-maker_field");
        var label = new Label
        {
            style =
            {
                flexGrow = 1,
                flexShrink = 1,
                maxWidth = 120,
                unityTextAlign = TextAnchor.MiddleLeft,
                color = new StyleColor(Color.white)
            },
            text = title,
            name = "title-label"
        };
        label.AddToClassList("sfx-maker_field_label");
        var sliderLabel = new Label("0.5")
        {
            pickingMode = PickingMode.Ignore
        };
        sliderLabel.AddToClassList("slider-label");
        var slider = new Slider
        {
            name = "value-slider",
            style =
            {
                flexGrow = 1,
                marginBottom = 2,
                marginLeft = 2,
                marginRight = 2,
                marginTop = 2,
            },
            pickingMode = PickingMode.Ignore,
            lowValue = minValue,
            highValue = maxValue,
        };
        slider.RegisterValueChangedCallback(v =>
        {
            value = v.newValue;
            onValueChanged?.Invoke(value);
            sliderLabel.text = value.ToString("F");
        });
        slider.value = value;
        var lockToggle = new Toggle
        {
            style =
            {
                marginBottom = 0,
                marginLeft = 0,
                marginRight = 0,
                marginTop = 0,
            }
        };

        var checkmark = new VisualElement
        {
            name = "checkmark",
        };
        checkmark.AddToClassList("checkmark");

        lockToggle.RegisterValueChangedCallback(v =>
        {
            locked = v.newValue;
            checkmark.style.visibility = locked ? Visibility.Visible : Visibility.Hidden;
        });
        lockToggle.Add(checkmark);
        lockToggle.value = locked;
        checkmark.style.visibility = locked ? Visibility.Visible : Visibility.Hidden;
        sliderLabel.text = value.ToString("F");
        slider.Add(sliderLabel);
        sliderLabel.pickingMode = PickingMode.Ignore;
        Add(label);
        Add(slider);
        //Add(lockToggle);
    }
}