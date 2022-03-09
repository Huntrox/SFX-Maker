using System;
using UnityEngine;

namespace HuntroxGames.Utils.Audio
{
    public enum WaveForm
    {
        Square,
        Sawtooth,
        Sine,
        Noise
    }

    [Serializable]
    public class SfxPatch
    {
        
        [Header("Envelope")]
        public WaveForm waveForm;
        public float envelopeAttackTime;
        public float envelopeSustainTime;
        public float envelopeDecayTime;
        public float envelopeSustainPunch;
        
        
        [Header("Frequency")]
        public float startFrequency;
        public float minimumFrequency;
        public float frequencySlide;
        public float frequencyDeltaSlide;
        
        /// <summary>Strength of the vibrato effect.</summary>
        [Header("Frequency")]
        public float vibratoDepth;
        /// <summary>Rate of the vibrato effect.</summary>
        public float vibratoSpeed;
        
        
        [Header("Duty Cycle")]
        public float squareWaveDuty;
        public float squareWaveDutySweep;
        
        [Header("Arpeggios")]
        public float arpSpeed;
        public float arpMod;
        
        [Header("ReTrigger")]
        public float repeatSpeed;
        
        [Header("Flanger")]
        public float phaserOffset;
        public float phaserSweep;

        
        [Header("Low-Pass Filter")]
        public float lpfResonance;
        public float lpfCutoffFrequency;
        public float lpfCutoffSweep;

        [Header("High-Pass Filter")]
        public float hpfCutoffFrequency;
        public float hpfSweep;

        
        
        public SfxPatch(
            WaveForm waveForm = WaveForm.Square,
            float startFrequency = 0.3f,
            float minimumFrequency = 0.1f,
            float frequencySlide = 0,
            float frequencyDeltaSlide = 0,
            float squareWaveDuty = 0,
            float squareWaveDutySweep = 0,
            float vibratoDepth = 0,
            float vibratoSpeed = 0,
            float envelopeAttackTime = 0,
            float envelopeSustainTime = 0.3f,
            float envelopeDecayTime = 0.4f,
            float envelopeSustainPunch = 0,
            float lpfResonance = 0,
            float lpfCutoffFrequency = 1,
            float lpfCutoffSweep = 0,
            float hpfCutoffFrequency = 0,
            float hpfSweep = 0,
            float phaserOffset = 0,
            float phaserSweep = 0,
            float repeatSpeed = 0,
            float arpSpeed = 0,
            float arpMod = 0)
        {
            this.waveForm = waveForm;
            this.startFrequency = startFrequency;
            this.minimumFrequency = minimumFrequency;
            this.frequencySlide = frequencySlide;
            this.frequencyDeltaSlide = frequencyDeltaSlide;
            this.squareWaveDuty = squareWaveDuty;
            this.squareWaveDutySweep = squareWaveDutySweep;
            this.vibratoDepth = vibratoDepth;
            this.vibratoSpeed = vibratoSpeed;
            this.envelopeAttackTime = envelopeAttackTime;
            this.envelopeSustainTime = envelopeSustainTime;
            this.envelopeDecayTime = envelopeDecayTime;
            this.envelopeSustainPunch = envelopeSustainPunch;
            this.lpfResonance = lpfResonance;
            this.lpfCutoffFrequency = lpfCutoffFrequency;
            this.lpfCutoffSweep = lpfCutoffSweep;
            this.hpfCutoffFrequency = hpfCutoffFrequency;
            this.hpfSweep = hpfSweep;
            this.phaserOffset = phaserOffset;
            this.phaserSweep = phaserSweep;
            this.repeatSpeed = repeatSpeed;
            this.arpSpeed = arpSpeed;
            this.arpMod = arpMod;
        }

        public string ToJson() => JsonUtility.ToJson(this);
        public SfxPatch FromJson(string json)
        {
            try
            {
                JsonUtility.FromJsonOverwrite(json,this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return this;
        }
    }
}