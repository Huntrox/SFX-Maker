using UnityEditor;
using UnityEngine;

namespace HuntroxGames
{
    [CustomEditor(typeof(SfxMaker))]
    public class SfxMakerEditor : Editor
    {
        private SfxMaker sfxMaker;

        private void OnEnable()
        {
            sfxMaker = (SfxMaker) target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Sfx Maker"))
                SfxMakerUI.ShowExample();
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pickup"))
                sfxMaker.Pickup();
            if (GUILayout.Button("Laser"))
                sfxMaker.Laser();
            if (GUILayout.Button("Explosion"))
                sfxMaker.Explosion();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PowerUp"))
                sfxMaker.PowerUp();
            if (GUILayout.Button("Hurt"))
                sfxMaker.Hurt();
            if (GUILayout.Button("Jump"))
                sfxMaker.Jump();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Blip"))
                sfxMaker.Blip();
            if (GUILayout.Button("Play"))
                sfxMaker.Play();
            GUILayout.EndHorizontal();
        }
    }
}