using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class VUMeter
{

    //public static GUIStyle progressBarBack { get { return s_Current.m_ProgressBarBack; } }
    //private GUIStyle m_ProgressBarBar, m_ProgressBarText, m_ProgressBarBack;



    public static GUIStyle progressBarBack
    {
		get
		{
            if(m_ProgressBarBack == null)
                m_ProgressBarBack = GetStyle("ProgressBarBack");
            return m_ProgressBarBack;
        }
    }

    private static GUIStyle m_ProgressBarBack;






    static Texture2D s_VerticalVUTexture;
    static Texture2D s_HorizontalVUTexture;
    const float VU_SPLIT = 0.9f;


    public struct SmoothingData
    {
        public float lastValue;
        public float peakValue;
        public float peakValueTime;
    }

    public static Texture2D verticalVUTexture
    {
        get
        {
            if (s_VerticalVUTexture == null)
                s_VerticalVUTexture = LoadIcon("VUMeterTextureVertical");
            return s_VerticalVUTexture;
        }

    }

    public static Texture2D horizontalVUTexture
    {
        get
        {
            if (s_HorizontalVUTexture == null)
                s_HorizontalVUTexture = LoadIcon("VUMeterTextureHorizontal");
            return s_HorizontalVUTexture;
        }
    }


    public static void HorizontalMeter(Rect position, float value, float peak, Texture2D foregroundTexture, Color peakColor)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        Color temp = GUI.color;

        // Draw background
        //EditorStyles.progressBarBack.Draw(position, false, false, false, false);
        progressBarBack.Draw(position, false, false, false, false);

        // Draw foreground
        GUI.color = new Color(1f, 1f, 1f, GUI.enabled ? 1 : 0.5f);
        float width = position.width * value - 2;
        if (width < 2)
            width = 2;
        Rect newRect = new Rect(position.x + 1, position.y + 1, width, position.height - 2);
        Rect uvRect = new Rect(0, 0, value, 1);
        GUI.DrawTextureWithTexCoords(newRect, foregroundTexture, uvRect);

        // Draw peak indicator
        GUI.color = peakColor;
        float peakpos = position.width * peak - 2;
        if (peakpos < 2)
            peakpos = 2;
        newRect = new Rect(position.x + peakpos, position.y + 1, 1, position.height - 2);
        GUI.DrawTexture(newRect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);

        // Reset color
        GUI.color = temp;
    }

    public static void VerticalMeter(Rect position, float value, float peak, Texture2D foregroundTexture, Color peakColor)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        Color temp = GUI.color;

        // Draw background
        //EditorStyles.progressBarBack.Draw(position, false, false, false, false);
        progressBarBack.Draw(position, false, false, false, false);

        // Draw foreground
        GUI.color = new Color(1f, 1f, 1f, GUI.enabled ? 1 : 0.5f);
        float height = (position.height - 2) * value;
        if (height < 2)
            height = 2;
        Rect newRect = new Rect(position.x + 1, (position.y + position.height - 1) - height, position.width - 2, height);
        Rect uvRect = new Rect(0, 0, 1, value);
        GUI.DrawTextureWithTexCoords(newRect, foregroundTexture, uvRect);

        // Draw peak indicator
        GUI.color = peakColor;
        float peakpos = (position.height - 2) * peak;
        if (peakpos < 2)
            peakpos = 2;
        newRect = new Rect(position.x + 1, (position.y + position.height - 1) - peakpos, position.width - 2, 1);
        GUI.DrawTexture(newRect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);

        // Reset color
        GUI.color = temp;
    }

    // Auto smoothing version
    public static void HorizontalMeter(Rect position, float value, ref SmoothingData data, Texture2D foregroundTexture, Color peakColor)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        float renderValue, renderPeak;
        SmoothVUMeterData(ref value, ref data, out renderValue, out renderPeak);
        HorizontalMeter(position, renderValue, renderPeak, foregroundTexture, peakColor);
    }

    // Auto smoothing version
    public static void VerticalMeter(Rect position, float value, ref SmoothingData data, Texture2D foregroundTexture, Color peakColor)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        float renderValue, renderPeak;
        SmoothVUMeterData(ref value, ref data, out renderValue, out renderPeak);
        VerticalMeter(position, renderValue, renderPeak, foregroundTexture, peakColor);
    }

    static void SmoothVUMeterData(ref float value, ref SmoothingData data, out float renderValue, out float renderPeak)
    {
        if (value <= data.lastValue)
        {
            value = Mathf.Lerp(data.lastValue, value, Time.smoothDeltaTime * 7.0f);
        }
        else
        {
            value = Mathf.Lerp(value, data.lastValue, Time.smoothDeltaTime * 2.0f);
            data.peakValue = value;
            data.peakValueTime = Time.realtimeSinceStartup;
        }

        if (value > 1.0f / VU_SPLIT)
            value = 1.0f / VU_SPLIT;
        if (data.peakValue > 1.0f / VU_SPLIT)
            data.peakValue = 1.0f / VU_SPLIT;

        renderValue = value * VU_SPLIT;
        renderPeak = data.peakValue * VU_SPLIT;

        data.lastValue = value;
    }


    public static Texture2D LoadIcon(string path)
    {
        ////EditorGUIUtility.LoadIcon("VUMeterTextureVertical");
        Assembly unityEditorAssembly = typeof(EditorGUIUtility).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.EditorGUIUtility");
        MethodInfo method = audioUtilClass.GetMethod(
            "LoadIcon",
            BindingFlags.Static | BindingFlags.NonPublic,
            null,
            new Type[] { typeof(string) },
            null
        );

        if (method.ReturnType == typeof(Texture2D))
            return method.Invoke(null, new object[] { path }) as Texture2D;
        else
            return null;
    }



    public static GUIStyle GetStyle(string style)
      => GUI.skin.FindStyle(style);

}
