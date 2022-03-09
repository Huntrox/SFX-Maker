using System;
using System.Collections;
using System.Collections.Generic;
using HuntroxGames.Utils.Audio;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveFormTabsUIElement : VisualElement
{
    
    public new class UxmlFactory : UxmlFactory<WaveFormTabsUIElement, UxmlTraits>
    {
    }


    public int index = 0;

    private string[] tabs;
    private VisualElement[] btns;
    public Action<int> onValueChanged;

    public WaveFormTabsUIElement()
    {
        Init(index,"tab one","tab two");
    }

    public WaveFormTabsUIElement(int index,string[] tabs,Action<int> onValueChanged)
    {
        this.index = index;
        this.onValueChanged = onValueChanged;
        this.tabs = tabs;
        Init(index,tabs);
    }
    private void Init(int indx,params string[] tabs)
    {
        //AddToClassList("sfx-maker_field");
        AddToClassList("params-tabs");
        style.alignContent = Align.Stretch;
        style.justifyContent = Justify.SpaceBetween;
      
        SetupTabs(indx,tabs);
    }

    public void SetupTabs(int indx,params string[] tabs)
    {
        //var waveTypes= System.Enum.GetNames(typeof(WaveForm));
        index = indx;
        this.tabs = tabs;
        Clear();
        btns = new VisualElement[tabs.Length];
        for (int i = 0; i < tabs.Length; i++)
        {
            int currIndex = i;
            btns[i] = CreateTab(tabs[currIndex], currIndex);
        }

        foreach (var btn in btns)
        {
            Add(btn);
        }
    }

    private Button CreateTab(string tabName, int currIndex)
    {
        var btn = new Button
        {
            text = tabName,

        };
        btn.AddToClassList("tab");
        if (currIndex == index)
            btn.AddToClassList("tab-focus");



        btn.clicked += () =>
        {
            for (int j = 0; j < btns.Length; j++)
            {
                btns[j].RemoveFromClassList("tab-focus");
            }

            btns[currIndex].AddToClassList("tab-focus");
            index = currIndex;
            onValueChanged?.Invoke(index);
        };
        return btn;
    }
}
