using System.Collections.Generic;
using System.IO;
using HuntroxGames.Utils;
using HuntroxGames.Utils.Audio;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class SfxMakerUI : EditorWindow
{
    [MenuItem("HuntroxUtils/Audio/Sfx Maker")]
    public static void ShowExample()
    {
        SfxMakerUI wnd = GetWindow<SfxMakerUI>();
        wnd.titleContent = new GUIContent("Sfx Maker");
    }

    private int sampleRate = 44100;
    private int SampleRateIndex => (sampleRate / sampleFreq) - 1;
    private int sampleFreq = 44100;
    private float masterVol = 0.05f;
    private float soundVol = 0.5f;
    private SfxPatch patch = new SfxPatch();
    private AudioClipGenerator audioClipGenerator = new AudioClipGenerator();

    private AudioSource source = null;
    private AudioJob saveJob;
    private List<string> recent = new List<string>();

    private ProgressBar progressBar;
    private Button exportBtn;

    private void OnEnable()
    {
        if (source == null)
        {
            var go = new GameObject("Source", typeof(AudioSource));
            go.hideFlags = HideFlags.HideAndDontSave;
            source = go.GetComponent<AudioSource>();
        }
    }

    private void OnDisable()
    {
        if (source != null)
        {
            DestroyImmediate(source.gameObject);
        }
    }

    private void Update()
    {
        if (saveJob != null) ShowSavingProgress();
    }

    private void ShowSavingProgress()
    {
        if (saveJob == null)
            return;

        if (saveJob.Update())
        {
            AssetDatabase.Refresh();
            progressBar.style.visibility = Visibility.Hidden;
            exportBtn.SetEnabled(true);
            saveJob = null;
        }
        else
        {
            exportBtn.SetEnabled(false);
            progressBar.style.visibility = Visibility.Visible;
            progressBar.title = "Saving Audio data";
            progressBar.value = SaveWav.Progress;
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // var visualTree =
        //     AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/HuntroxGames/Audio/Resources/SfxMakerUI.uxml");
        //

        var visualTree = Resources.Load<VisualTreeAsset>("SfxMakerUI");
        var stylesheet = Resources.Load<StyleSheet>("SfxMakerUI");
        visualTree.CloneTree(root);
        root.styleSheets.Add(stylesheet);
        InitUI();
    }


    private void InitUI()
    {
        var body = rootVisualElement.Q("body");
        DrawParams();
        InitializePresetsBtns(body);
        progressBar = rootVisualElement.Q<ProgressBar>("export-progress");
        progressBar.style.visibility = Visibility.Hidden;
        var vuMeter = rootVisualElement.Q<VUMeterUIElement>();
        vuMeter.audioSource = source;
        var plyBtn = rootVisualElement.Q<Button>("play-btn");
        exportBtn = rootVisualElement.Q<Button>("export-btn");
        var volSlider = rootVisualElement.Q<Slider>("vol-slider");
        var sampleRateTabs = rootVisualElement.Q<WaveFormTabsUIElement>("sample-rate-tabs");
        sampleRateTabs.SetupTabs(SampleRateIndex, "44k", "22k", "11k", "6k");
        sampleRateTabs.onValueChanged = SetSampleRate;

        volSlider.RegisterValueChangedCallback(v =>
        {
            soundVol = v.newValue;
            PlayPreview();
        });

        plyBtn.clicked += PlayPreview;
        exportBtn.clicked += Export;
    }

    private void Export()
    {
        var exportPath =
            EditorUtility.SaveFilePanelInProject("Save Audio", "clip", "wav", "choice path to save the file",
                "Assets");

        if (string.IsNullOrEmpty(exportPath) || string.IsNullOrWhiteSpace(exportPath))
            return;

        var fileName = Path.GetFileName(exportPath);
        var dir = Path.GetDirectoryName(exportPath);


        exportBtn.SetEnabled(false);
        saveJob = new AudioJob(fileName, dir, audioClipGenerator.GetAudioClip(patch, sampleFreq, soundVol, masterVol));
        saveJob.Start();
        //Debug.Log(JsonUtility.ToJson(patch));
    }

    public void InitializePresetsBtns(VisualElement bodyContainer)
    {
        var pickupBtn = bodyContainer.Q<Button>("pickup-btn");
        var laserBtn = bodyContainer.Q<Button>("laser-btn");
        var explosionBtn = bodyContainer.Q<Button>("explosion-btn");
        var powerupBtn = bodyContainer.Q<Button>("powerup-btn");
        var hurtBtn = bodyContainer.Q<Button>("hurt-btn");
        var jumpBtn = bodyContainer.Q<Button>("jump-btn");
        var blipBtn = bodyContainer.Q<Button>("blip-btn");

        pickupBtn.clicked += () => SetPreset(SfxPresets.Pickup());
        laserBtn.clicked += () => SetPreset(SfxPresets.Laser());
        explosionBtn.clicked += () => SetPreset(SfxPresets.Explosion());
        powerupBtn.clicked += () => SetPreset(SfxPresets.PowerUp());
        hurtBtn.clicked += () => SetPreset(SfxPresets.Hurt());
        jumpBtn.clicked += () => SetPreset(SfxPresets.Jump());
        blipBtn.clicked += () => SetPreset(SfxPresets.Blip());
    }

    public void DrawParams()
    {
        var paramsContainer = rootVisualElement.Q("params-container");
        paramsContainer.Clear();
        paramsContainer.Add(CreateParamLabel("WaveForm"));
        paramsContainer.Add(new WaveFormTabsUIElement((int) patch.waveForm, System.Enum.GetNames(typeof(WaveForm))
            , SetWaveForm));
        CreateEnvelopeParams(paramsContainer);
        CreateFrequencyParams(paramsContainer);
        CreateVibratoParams(paramsContainer);
        CreateArpeggiationParams(paramsContainer);
        CreateDutyCycleParams(paramsContainer);
        CreateRepeatParams(paramsContainer);
        CreateFlangerParams(paramsContainer);
        CreateLPFilterParams(paramsContainer);
        CreateHPFilterParams(paramsContainer);
    }


    #region Params

    private void CreateEnvelopeParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("Envelope"));
        paramsContainer.Add(new SfxSettingUIElement("Attack", 0, 1, patch.envelopeAttackTime, false,
            v => patch.envelopeAttackTime = v));
        paramsContainer.Add(new SfxSettingUIElement("Sustain", 0, 1, patch.envelopeSustainTime, false,
            v => patch.envelopeSustainTime = v));
        paramsContainer.Add(new SfxSettingUIElement("Punch", 0, 1, patch.envelopeSustainPunch, false,
            v => patch.envelopeSustainPunch = v));
        paramsContainer.Add(new SfxSettingUIElement("Decay", 0, 1, patch.envelopeDecayTime, false,
            v => patch.envelopeDecayTime = v));
    }

    private void CreateFrequencyParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("Frequency"));
        paramsContainer.Add(new SfxSettingUIElement("Frequency", 0, 1, patch.startFrequency, false,
            v => patch.startFrequency = v));
        paramsContainer.Add(new SfxSettingUIElement("Min Freq", 0, 1, patch.minimumFrequency, false,
            v => patch.minimumFrequency = v));
        paramsContainer.Add(new SfxSettingUIElement("Slide", -1, 1, patch.frequencySlide, false,
            v => patch.frequencySlide = v));
        paramsContainer.Add(new SfxSettingUIElement("Delta slide", -1, 1, patch.frequencyDeltaSlide, false,
            v => patch.frequencyDeltaSlide = v));
    }


    private void CreateVibratoParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("Vibrato"));
        paramsContainer.Add(new SfxSettingUIElement("Depth", 0, 1, patch.vibratoDepth, false,
            v => patch.vibratoDepth = v));
        paramsContainer.Add(new SfxSettingUIElement("Speed", 0, 1, patch.vibratoSpeed, false,
            v => patch.vibratoSpeed = v));
    }

    private void CreateArpeggiationParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("Arpeggiation"));
        paramsContainer.Add(new SfxSettingUIElement("FrequencyMult", -1, 1, patch.arpMod, false,
            v => patch.arpMod = v));
        paramsContainer.Add(
            new SfxSettingUIElement("ChangeSpeed", 0, 1, patch.arpSpeed, false, v => patch.arpSpeed = v));
    }

    private void CreateDutyCycleParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("Duty Cycle"));
        paramsContainer.Add(new SfxSettingUIElement("Cycle", 0, 1, patch.squareWaveDuty, false,
            v => patch.squareWaveDuty = v));
        paramsContainer.Add(new SfxSettingUIElement("Sweep", -1, 1, patch.squareWaveDutySweep, false,
            v => patch.squareWaveDutySweep = v));
    }

    private void CreateRepeatParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("Repeat"));
        paramsContainer.Add(new SfxSettingUIElement("Rate", 0, 1, patch.repeatSpeed, false,
            v => patch.repeatSpeed = v));
    }

    private void CreateFlangerParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("Flanger"));
        paramsContainer.Add(new SfxSettingUIElement("Offset", -1, 1, patch.phaserOffset, false,
            v => patch.phaserOffset = v));
        paramsContainer.Add(new SfxSettingUIElement("Sweep", -1, 1, patch.phaserSweep, false,
            v => patch.phaserSweep = v));
    }

    private void CreateLPFilterParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("Low-Pass Filter"));
        paramsContainer.Add(new SfxSettingUIElement("Cutoff Frequency", 0, 1, patch.lpfCutoffFrequency, false,
            v => patch.lpfCutoffFrequency = v));
        paramsContainer.Add(new SfxSettingUIElement("Cutoff Sweep ", 0, 1, patch.lpfCutoffSweep, false,
            v => patch.lpfCutoffSweep = v));
        paramsContainer.Add(new SfxSettingUIElement("Resonance", 0, 1, patch.lpfResonance, false,
            v => patch.lpfResonance = v));
    }

    private void CreateHPFilterParams(VisualElement paramsContainer)
    {
        paramsContainer.Add(CreateParamLabel("High-Pass Filter"));
        paramsContainer.Add(new SfxSettingUIElement("Cutoff Frequency", 0, 1, patch.hpfCutoffFrequency, false,
            v => patch.hpfCutoffFrequency = v));
        paramsContainer.Add(new SfxSettingUIElement("Cutoff Sweep ", 0, 1, patch.hpfSweep, false,
            v => patch.hpfSweep = v));
    }

    #endregion

    private void SetWaveForm(int value)
    {
        patch.waveForm = (WaveForm) value;
        PlayPreview();
    }

    private void SetPreset(SfxPatch preset)
    {
        AddRecent();
        patch = preset;
        DrawParams();
        PlayPreview();
    }

    private void PlayPreview()
    {
        var clip = audioClipGenerator.GetAudioClip(patch, sampleFreq, soundVol, masterVol);
        //AudioDataHandler.PlayClip(clip);
        source.clip = clip;
        source.Play();
    }

    private void AddRecent()
    {
        var json = patch.ToJson();
        recent.Add(json);
        RepaintRecentList();
    }

    private void RepaintRecentList()
    {
        var view = rootVisualElement.Q("recent-view");
        view.Clear();
        for (var i = 0; i < recent.Count; i++)
        {
            int index = i;
            var rec = recent[i];
            var btn = new Button(() => LoadPatchJson(rec))
            {
                text = index.ToString("0000")
            };
            btn.AddToClassList("buttons");
            view.Insert(0, btn);
        }
    }

    private void SetSampleRate(int index)
    {
        sampleFreq = sampleRate / (index + 1);
        PlayPreview();
    }

    private void LoadPatchJson(string json)
    {
        patch = patch.FromJson(json);
        DrawParams();
        PlayPreview();
    }

    private Label CreateParamLabel(string waveform)
    {
        var label = new Label(waveform);
        label.AddToClassList("params-title");
        return label;
    }
}