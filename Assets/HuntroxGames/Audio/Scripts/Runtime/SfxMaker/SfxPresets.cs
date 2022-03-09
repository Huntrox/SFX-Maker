using System;

namespace HuntroxGames.Utils.Audio
{
    public static class SfxPresets
    {
        private static readonly Random random = new Random();

        private static float RandF(float max) => (float)random.NextDouble()*max;
        private static float Rand(float max) => (float)random.NextDouble()*(max+1);
        
        private static float Rndr(float from , float to) => (float)random.NextDouble()* (to - from) + from;
        
        
        private static bool Chance(double probability) => random.NextDouble() <= probability;

        public static SfxPatch Pickup()
        {
            var patch = new SfxPatch(
                startFrequency: 0.4f + RandF(0.5f),
                envelopeAttackTime: 0,
                envelopeSustainTime: RandF(0.1f),
                envelopeDecayTime: 0.1f + RandF(0.4f),
                envelopeSustainPunch: 0.3f + RandF(0.3f));

            if (Chance(0.5))
            {
                patch.arpSpeed = 0.5f + RandF(0.2f);
                patch.arpMod = 0.2f + RandF(0.4f);
            }

            return patch;
        }

        public static SfxPatch Laser()
        {
            
            var patch = new SfxPatch
            {
                waveForm = (WaveForm) random.Next(0, 3),
                envelopeAttackTime = 0.0f,
                envelopeSustainTime = 0.1f + RandF(0.2f),
                envelopeDecayTime = RandF(0.4f)
            };


            if (Chance(0.5))
                patch.envelopeSustainPunch = RandF(0.3f);

            patch.startFrequency = 0.5f + RandF(0.5f);
            patch.minimumFrequency = (float)Math.Max(0.2, patch.startFrequency - 0.2f - RandF(0.6f));

            if (Chance(1/3.0))
            {
                patch.startFrequency = 0.3f + RandF(0.6f);
                patch.minimumFrequency = RandF(0.1f);
                patch.frequencySlide = -0.35f - RandF(0.3f);
            }

            if (patch.waveForm == WaveForm.Square)
            {
                var probability = Chance(0.5);

                patch.squareWaveDuty = probability ? RandF(0.5f) : 0.4f + RandF(0.5f);
                patch.squareWaveDutySweep = probability ? RandF(0.2f) : -RandF(0.7f);
            }

            if (Chance(1/3.0))
            {
                patch.phaserOffset = RandF(0.2f);
                patch.phaserSweep = -RandF(0.2f);
            }

            if (Chance(0.5))
                patch.hpfCutoffFrequency = RandF(0.3f);

            return patch;
        }
        public static SfxPatch Explosion()
        {
            var patch = new SfxPatch {waveForm = WaveForm.Noise};

            if (Chance(0.5))
            {
                patch.startFrequency = 0.1f + RandF(0.4f);
                patch.frequencySlide = -0.1f + RandF(0.4f);
            }
            else
            {
                patch.startFrequency = 0.2f + RandF(0.7f);
                patch.frequencySlide = -0.2f - RandF(0.2f);
            }
            patch.startFrequency *= patch.startFrequency;

            if (Chance(0.25))
                patch.frequencySlide = 0.0f;
            if (Chance(1/3.0))
                patch.repeatSpeed = 0.3f + RandF(0.5f);

            patch.envelopeAttackTime = 0.0f;
            patch.envelopeSustainTime = 0.1f + RandF(0.3f);
            patch.envelopeDecayTime = RandF(0.5f);

            if (Chance(0.5))
            {
                patch.phaserOffset = -0.3f + RandF(0.9f);
                patch.phaserSweep = -RandF(0.3f);
            }

            patch.envelopeSustainPunch = 0.2f + RandF(0.6f);

            if (Chance(0.5))
            {
                patch.vibratoDepth = RandF(0.7f);
                patch.vibratoSpeed = RandF(0.6f);
            }

            if (Chance(1/3.0))
            {
                patch.arpSpeed = 0.6f + RandF(0.3f);
                patch.arpMod = 0.8f - RandF(1.6f);
            }

            return patch;
        }
        public static SfxPatch PowerUp()
        {
            var patch = new SfxPatch();

            if (Chance(0.5))
            {
                patch.waveForm = WaveForm.Sawtooth;
            }
            else
            {
                patch.waveForm = WaveForm.Square;
                patch.squareWaveDuty = RandF(0.6f);
            }

            if (Chance(0.5))
            {
                patch.startFrequency = 0.2f + RandF(0.3f);
                patch.frequencySlide = 0.1f + RandF(0.4f);
                patch.repeatSpeed = 0.4f + RandF(0.4f);
            }
            else
            {
                patch.startFrequency = 0.2f + RandF(0.3f);
                patch.frequencySlide = 0.05f + RandF(0.2f);
                if (Chance(0.5))
                {
                    patch.vibratoDepth = RandF(0.7f);
                    patch.vibratoSpeed = RandF(0.6f);
                }
            }
            patch.envelopeAttackTime = 0.0f;
            patch.envelopeSustainTime = RandF(0.4f);
            patch.envelopeDecayTime = 0.1f + RandF(0.4f);

            return patch;
        }
        public static SfxPatch Hurt()
        {
            var patch = new SfxPatch();

            patch.waveForm = (WaveForm)random.Next(0, 3);
            if (patch.waveForm == WaveForm.Sine)
                patch.waveForm = WaveForm.Noise;
            if (patch.waveForm == WaveForm.Square)
                patch.squareWaveDuty = RandF(0.6f);
            patch.startFrequency = 0.2f + RandF(0.6f);
            patch.frequencySlide = -0.3f - RandF(0.4f);
            patch.envelopeAttackTime = 0.0f;
            patch.envelopeSustainTime = RandF(0.1f);
            patch.envelopeDecayTime = 0.1f + RandF(0.2f);
            if (Chance(0.5))
                patch.hpfCutoffFrequency = RandF(0.3f);

            return patch;
        }
        public static SfxPatch Jump()
        {
            var patch = new SfxPatch
            {
                waveForm = 0,
                squareWaveDuty = RandF(0.6f),
                startFrequency = 0.3f + RandF(0.3f),
                frequencySlide = 0.1f + RandF(0.2f),
                envelopeAttackTime = 0.0f,
                envelopeSustainTime = 0.1f + RandF(0.3f),
                envelopeDecayTime = 0.1f + RandF(0.2f)
            };

            if (Chance(0.5))
                patch.hpfCutoffFrequency = RandF(0.3f);
            if (Chance(0.5))
                patch.lpfCutoffFrequency = 1.0f - RandF(0.6f);

            return patch;
        }

        public static SfxPatch Blip()
        {
            var patch = new SfxPatch {waveForm = (WaveForm) random.Next(0, 2)};

            if (patch.waveForm == 0)
                patch.squareWaveDuty = RandF(0.6f);
            patch.startFrequency = 0.2f + RandF(0.4f);
            patch.envelopeAttackTime = 0.0f;
            patch.envelopeSustainTime = 0.1f + RandF(0.1f);
            patch.envelopeDecayTime = RandF(0.2f);
            patch.hpfCutoffFrequency = 0.1f;

            return patch;
        }
    }
}