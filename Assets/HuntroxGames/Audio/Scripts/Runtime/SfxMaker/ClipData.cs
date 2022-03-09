using UnityEngine;

namespace HuntroxGames.Utils.Audio
{
    public class ClipData
    {
        public int Frequency { get; set; }
        public int Channels { get; set; }
        public int Samples { get; set; }
        public float[] Data { get; set; }

        private void InitData()
        {
            Data = new float[Samples * Channels];
        }
        

        public AudioClip GetAudioClip(string clipName = "AudioClip")
        {
            var audioClip = AudioClip.Create(clipName, Samples, Channels, Frequency, false);
            audioClip.SetData(Data, 0);
            return audioClip;
        }

        public void GetData(float[] data, int offsetSamples)
        {
            var index = offsetSamples * Channels;
            var count = Data.Length - index;
            var dataCount = data.Length;

            for (var i = 0; i < count && i < dataCount; i++)
            {
                data[i] = Data[index + i];
            }
        }

        public static ClipData FromAudioClip(AudioClip clip)
        {
            var clipData = new ClipData
            {
                Channels = clip.channels,
                Frequency = clip.frequency,
                Samples = clip.samples
            };
            clipData.InitData();
            clip.GetData(clipData.Data, 0);
            return clipData;
        }
    }
}