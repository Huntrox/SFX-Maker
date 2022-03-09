using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace HuntroxGames.Utils.Audio
{
    public sealed class AudioClipGenerator
    {
        private enum Envelope
        {
            Attack = 0,
            Sustain = 1,
            Decay = 2
        }

        private readonly Random random = new Random();
        private float masterVol = 0.05f;
        private float soundVol = 0.5f;
        private SfxPatch patch;
        private const int PHASER_BUFFER_LENGTH = 1024;
        private const int NOISE_BUFFER_LENGTH = 32;

        //private bool isPlaying;

        private int phase;
        private double frequencyPeriod;
        private double frequencyMaxPeriod;
        private double frequencySlide;
        private double frequencyDeltaSlide;
        private int period;
        private float squareDuty;
        private float squareSlide;
        private Envelope envelope;
        private int envelopeTime;
        private readonly int[] envelopeLength = new int[3];
        public int EnvFullLength => envelopeLength.Sum();
        private float envelopeVolume;
        private float frequencyPhase;
        private float frequencyDeltaPhase;
        private int iPhase;
        private readonly float[] phaserBuffer = new float[PHASER_BUFFER_LENGTH];
        private int ipp;
        private readonly float[] noiseBuffer = new float[NOISE_BUFFER_LENGTH];
        private float fltp;
        private float fltdp;
        private float fltw;
        private float fltw_d;
        private float fltdmp;
        private float fltphp;
        private float flthp;
        private float flthp_d;
        private float vibPhase;
        private float vibSpeed;
        private float vibAmp;
        private int repTime;
        private int repLimit;
        private int arpTime;
        private int arpLimit;
        private double arpMod;
        private AudioClip audioClip = null;

        private void InitializePatch(SfxPatch patch, float vol = 0.5f, float master = 0.05f)
        {
            this.patch = patch;
            masterVol = master;
            soundVol = vol;
            Initialize(restart: false);
            // isPlaying = true;
        }

        private void Initialize(bool restart)
        {
            
            frequencyPeriod = 100.0 / (patch.startFrequency * patch.startFrequency + 0.001);
            period = (int) frequencyPeriod;
            frequencyMaxPeriod = 100.0 / (patch.minimumFrequency * patch.minimumFrequency + 0.001);
            frequencySlide = 1.0 - Math.Pow(patch.frequencySlide, 3.0) * 0.01;
            frequencyDeltaSlide = -Math.Pow(patch.frequencyDeltaSlide, 3.0) * 0.000001;
            squareDuty = 0.5f - patch.squareWaveDuty * 0.5f;
            squareSlide = -patch.squareWaveDutySweep * 0.00005f;
            if (patch.arpMod >= 0f)
                arpMod = 1.0 - Math.Pow(patch.arpMod, 2.0) * 0.9;
            else
                arpMod = 1.0 + Math.Pow(patch.arpMod, 2.0) * 10.0;
            arpTime = 0;
            arpLimit = (int) (Math.Pow(1f - patch.arpSpeed, 2f) * 20000 + 32);
            if (patch.arpSpeed == 1f)
                arpLimit = 0;

            if (!restart)
            {
                phase = 0;
                // reset filter
                fltp = 0f;
                fltdp = 0f;
                fltw = (float) Math.Pow(patch.lpfCutoffFrequency, 3f) * 0.1f;
                fltw_d = 1f + patch.lpfCutoffSweep * 0.0001f;
                fltdmp = 5f / (1f + (float) Math.Pow(patch.lpfResonance, 2f) * 20f) * (0.01f + fltw);
                if (fltdmp > 0.8f)
                    fltdmp = 0.8f;
                fltphp = 0f;
                flthp = (float) Math.Pow(patch.hpfCutoffFrequency, 2f) * 0.1f;
                flthp_d = 1f + patch.hpfSweep * 0.0003f;
                // reset vibrato
                vibPhase = 0f;
                vibSpeed = (float) Math.Pow(patch.vibratoSpeed, 2f) * 0.01f;
                vibAmp = patch.vibratoDepth * 0.5f;
                // reset envelope
                envelopeVolume = 0f;
                envelope = Envelope.Attack;
                envelopeTime = 0;
                envelopeLength[(int) Envelope.Attack] =
                    (int) (patch.envelopeAttackTime * patch.envelopeAttackTime * 100000f);
                envelopeLength[(int) Envelope.Sustain] =
                    (int) (patch.envelopeSustainTime * patch.envelopeSustainTime * 100000f);
                envelopeLength[(int) Envelope.Decay] =
                    (int) (patch.envelopeDecayTime * patch.envelopeDecayTime * 100000f);

                frequencyPhase = (float) Math.Pow(patch.phaserOffset, 2f) * 1020f;
                if (patch.phaserOffset < 0f)
                    frequencyPhase = -frequencyPhase;
                frequencyDeltaPhase = (float) Math.Pow(patch.phaserSweep, 2f) * 1f;
                if (patch.phaserSweep < 0f)
                    frequencyDeltaPhase = -frequencyDeltaPhase;
                iPhase = Math.Abs((int) frequencyPhase);
                ipp = 0;
                Array.Clear(phaserBuffer, 0, PHASER_BUFFER_LENGTH);

                for (var i = 0; i < NOISE_BUFFER_LENGTH; i++)
                    noiseBuffer[i] = (float) random.NextDouble() * 2f - 1f;

                repTime = 0;
                repLimit = (int) (Math.Pow(1f - patch.repeatSpeed, 2f) * 20000 + 32);
                if (patch.repeatSpeed == 0f)
                    repLimit = 0;
            }
        }

        private int ReadData(ref float[] buffer, int offset, int sampleCount)
        {
            for (var i = 0; i < sampleCount; i++)
            {
                repTime++;
                if (repLimit != 0 && repTime >= repLimit)
                {
                    repTime = 0;
                    Initialize(restart: true);
                }

                arpTime++;
                if (arpLimit != 0 && arpTime >= arpLimit)
                {
                    arpLimit = 0;
                    frequencyPeriod *= arpMod;
                }

                frequencySlide += frequencyDeltaSlide;
                frequencyPeriod *= frequencySlide;
                if (frequencyPeriod > frequencyMaxPeriod)
                {
                    frequencyPeriod = frequencyMaxPeriod;
                    // if (patch.minimumFrequency > 0f)
                    //     isPlaying = false;
                }

                var rfPeriod = (float) frequencyPeriod;
                if (vibAmp > 0f)
                {
                    vibPhase += vibSpeed;
                    rfPeriod = (float) (frequencyPeriod * (1.0 + Math.Sin(vibPhase) * vibAmp));
                }

                period = (int) rfPeriod;
                if (period < 8)
                    period = 8;

                squareDuty += squareSlide;
                if (squareDuty < 0f)
                    squareDuty = 0f;
                else if (squareDuty > 0.5f)
                    squareDuty = 0.5f;

                // volume envelope
                envelopeTime++;
                if (envelopeTime > envelopeLength[(int) envelope])
                {
                    envelopeTime = 0;
                    if (envelope != Envelope.Decay)
                    //     isPlaying = false;
                    // else
                        envelope++;
                }

                switch (envelope)
                {
                    case Envelope.Attack:
                        envelopeVolume = (float) envelopeTime / envelopeLength[(int) Envelope.Attack];
                        break;
                    case Envelope.Sustain:
                        envelopeVolume =
                            1f + (float) Math.Pow(1f - (float) envelopeTime / envelopeLength[(int) Envelope.Sustain],
                                1f) * 2f * patch.envelopeSustainPunch;
                        break;
                    case Envelope.Decay:
                        envelopeVolume = 1f - (float) envelopeTime / envelopeLength[(int) Envelope.Decay];
                        break;
                }

                // phaser step
                frequencyPhase += frequencyDeltaPhase;
                iPhase = Math.Abs((int) frequencyPhase);
                if (iPhase > 1023)
                    iPhase = 1023;

                if (flthp_d != 0)
                {
                    flthp *= flthp_d;
                    if (flthp < 0.00001f)
                        flthp = 0.00001f;
                    if (flthp > 0.1f)
                        flthp = 0.1f;
                }

                // 8x oversampling
                var overSample = 0f;
                for (var si = 0; si < 8; si++)
                {
                    var sample = 0f;
                    phase++;
                    if (phase >= period)
                    {
//				        phase = 0;
                        phase %= period;
                        if (patch.waveForm == WaveForm.Noise)
                        {
                            for (var j = 0; j < NOISE_BUFFER_LENGTH; j++)
                                noiseBuffer[j] = (float) random.NextDouble() * 2f - 1f;
                        }
                    }

                    // base waveform
                    var fp = (float) phase / period;
                    switch (patch.waveForm)
                    {
                        case WaveForm.Square:
                            sample = fp < squareDuty ? 0.5f : -0.5f;
                            break;
                        case WaveForm.Sawtooth:
                            sample = 1f - fp * 2;
                            break;
                        case WaveForm.Sine:
                            sample = (float) Math.Sin(fp * 2 * Math.PI);
                            break;
                        case WaveForm.Noise:
                            sample = noiseBuffer[phase * NOISE_BUFFER_LENGTH / period];
                            break;
                    }

                    // lp filter
                    var pp = fltp;
                    fltw *= fltw_d;
                    if (fltw < 0f)
                        fltw = 0f;
                    if (fltw > 0.1f)
                        fltw = 0.1f;
                    if (patch.lpfCutoffFrequency != 1f)
                    {
                        fltdp += (sample - fltp) * fltw;
                        fltdp -= fltdp * fltdmp;
                    }
                    else
                    {
                        fltp = sample;
                        fltdp = 0f;
                    }

                    fltp += fltdp;
                    // hp filter
                    fltphp += fltp - pp;
                    fltphp -= fltphp * flthp;
                    sample = fltphp;
                    // phaser
                    phaserBuffer[ipp & 1023] = sample;
                    sample += phaserBuffer[(ipp - iPhase + PHASER_BUFFER_LENGTH) & 1023];
                    ipp = (ipp + 1) & 1023;
                    // final accumulation and envelope application
                    overSample += sample * envelopeVolume;
                }

                overSample = overSample / 8 * masterVol;

                overSample *= 2f * soundVol;

                if (overSample > 1f)
                    overSample = 1f;
                if (overSample < -1f)
                    overSample = -1f;

                buffer[offset++] = overSample;
            }

            return sampleCount;
        }

        
        public AudioClip GetAudioClip(SfxPatch patch, int sampleFreq, float vol, float master, string clipName = "clip")
        {
            InitializePatch(patch, vol, master);
            float[] samples = new float[EnvFullLength];
            var sampleCount = ReadData(ref samples, 0, samples.Length);
            audioClip = AudioClip.Create(clipName, sampleCount,
                1, sampleFreq, false);
            audioClip.SetData(samples, 0);
            return audioClip;
        }
        
    }
}