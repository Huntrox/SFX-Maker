using HuntroxGames.Utils.Audio;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public class AudioJob : ThreadedJob
    {
        private ClipData clipData;
        private string filepath;
        private readonly string filename;
        private readonly string filePath;
        
        public AudioJob(string filename, string filePath, AudioClip clip) : this(filename,filePath, ClipData.FromAudioClip(clip))
        {
        }

        public AudioJob(string filename, string filePath, ClipData clipData)
        {
            this.filename = filename;
            this.filePath = filePath;
            this.clipData = clipData;
        }
        

        public override void Start()
        {
            filepath = SaveWav.CreatePathInTemp(filename);
            base.Start();
        }

        protected override void ThreadFunction() 
            => SaveWav.Save(filepath, clipData);

        protected override void OnFinished()
        {
            if (clipData != null)
                SaveWav.MoveFileFromTemp(filename, filePath);
            clipData = null;
        }
    }
}