using UnityEngine;
using UnityEngine.UIElements;

public class VUMeterUIElement : IMGUIContainer
{
    public new class UxmlFactory : UxmlFactory<VUMeterUIElement, UxmlTraits>
    {
    }

    public float value = 0;
    public float peak = 0;
    public VUMeter.SmoothingData smoothing = new VUMeter.SmoothingData();
    public AudioSource audioSource;
    private int qSamples = 4096;
    private float[] samples;
    private float volume;

    public ViewOrientation orientation;

    public VUMeterUIElement(AudioSource audioSource, ViewOrientation orientation = ViewOrientation.Horizontal)
    {
        this.audioSource = audioSource;
        this.orientation = orientation;
        samples = new float[qSamples];
        onGUIHandler = OnGui;
    }

    public VUMeterUIElement()
    {
        samples = new float[qSamples];
        orientation = ViewOrientation.Horizontal;
        onGUIHandler = OnGui;
    }


    private void OnGui()
    {
        if (audioSource != null)
        {
            volume = GetChannelVolume(0) + GetChannelVolume(1);
            if (audioSource.isPlaying)
                MarkDirtyRepaint();
        }

        if (orientation == ViewOrientation.Horizontal)
            VUMeter.HorizontalMeter(contentRect, volume, ref smoothing, VUMeter.horizontalVUTexture, Color.white);
        else
            VUMeter.VerticalMeter(contentRect, volume, ref smoothing, VUMeter.horizontalVUTexture, Color.white);
    }

    private float GetChannelVolume(int channel)
    {
        audioSource.GetOutputData(samples, channel);
        float sum = 0;
        foreach (float f in samples)
            sum += f * f;
        return Mathf.Sqrt(sum / qSamples);
    }
}

public enum ViewOrientation
{
    Horizontal,
    Vertical,
}