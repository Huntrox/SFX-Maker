using System.Collections;
using System.Collections.Generic;
using HuntroxGames.Utils.Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace HuntroxGames
{
    public class SfxMaker : MonoBehaviour
    {
        public AudioSource audioSource;
        public int sampleFreq = 44100;
        public float masterVol = 0.05f;
        public float soundVol = 0.5f;
        public SfxPatch patch = new SfxPatch();
        private AudioClipGenerator audioClipGenerator = new AudioClipGenerator();
        private AudioClip audioClip;

        
        public void Pickup()
        {
            patch = SfxPresets.Pickup();
            Play();
        }
        public void Laser()
        {
            patch = SfxPresets.Laser();
            Play();
        }
        public void Explosion()
        {
            patch = SfxPresets.Explosion();
            Play();
        }
        public void PowerUp()
        {
            patch = SfxPresets.PowerUp();
            Play();
        }
        public void Hurt()
        {
            patch = SfxPresets.Hurt();
            Play();
        }
        public void Jump()
        {
            patch = SfxPresets.Jump();
            Play();
        }
        public void Blip()
        {
            patch = SfxPresets.Blip();
            Play();
        }
        public void Play()
        {
            audioSource.clip = audioClipGenerator.GetAudioClip(patch,sampleFreq,soundVol,masterVol);
            audioSource.Play();
        }
        private void SaveToWav()
        {
            if (audioClipGenerator != null)
            {
                var path = "Assets/mySFX/sfx.wav";
                SaveWav.CreatePathInProject(path);
                SaveWav.Save(path, ClipData.FromAudioClip(audioClip));
            }
        }
    }
}