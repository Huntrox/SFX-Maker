using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using System.Linq;

#if UNITY_EDITOR
using UnityEditor.ProjectWindowCallback;
using UnityEditor;
#endif
namespace HuntroxGames.Utils.Audio
{
	public static class AudioManagerUtils
	{
		public static AudioSource CreateAudioSource(Transform parent)
		{
			GameObject tempAudioSource = new GameObject("UniqueAudioSource");
			tempAudioSource.transform.parent = parent.transform;
			tempAudioSource.transform.position = Vector3.zero;
			return tempAudioSource.AddComponent<AudioSource>();
		}
		public static bool IsNullOrEmpty<T>(this T[] array) => array == null || array.Length < 1;
		public static bool IsNullOrEmpty<T>(this List<T> list) => list == null || list.Count < 1;
		public static bool IsNullOrEmpty<T>(this Queue<T> queue) => queue == null || queue.Count < 1;
		public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1, T2> dictionary) => dictionary == null || dictionary.Count < 1;

		// public static TweenerCore<float, float, FloatOptions> DOFade(this AudioSource target, float endValue, float duration)
		// {
		// 	if (endValue < 0) endValue = 0;
		// 	else if (endValue > 1) endValue = 1;
		// 	TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.volume, x => target.volume = x, endValue, duration);
		// 	t.SetTarget(target);
		// 	return t;
		// }


#if UNITY_EDITOR
		public static void Resize<T>(this List<T> list, int size, T element = default(T))
		{
			int count = list.Count;

			if (size < count)
			{
				list.RemoveRange(size, count - size);
			}
			else if (size > count)
			{
				if (size > list.Capacity)
					list.Capacity = size;

				list.AddRange(System.Linq.Enumerable.Repeat(element, size - count));
			}
		}
		public static void RemoveAt<T>(ref T[] arr, int index)
		{
			for (int a = index; a < arr.Length - 1; a++)
			{
				arr[a] = arr[a + 1];
			}
			Array.Resize(ref arr, arr.Length - 1);
		}
		public static string[] GetCurrentScenesName()
		{
			List<string> scenes = new List<string>();
			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				scenes.Add(Path.GetFileNameWithoutExtension(scene.path));
			}
			return scenes.ToArray();
		}
		public static GUIStyle LabelStyle(int fontSize = 18, bool IsBoold = true)
		{
			GUIStyle gUI = new GUIStyle();

			gUI.fontSize = fontSize;
			if (IsBoold)
				gUI.fontStyle = FontStyle.Bold;
			gUI.alignment = TextAnchor.MiddleCenter;
			gUI.normal.textColor = GUI.color;
			return gUI;
		}
		public static void CreateNewAudioMixer(string path)
		{
			var doCreateAudioMixer = typeof(EndNameEditAction).Assembly.GetType("UnityEditor.ProjectWindowCallback.DoCreateAudioMixer");
			var icon = EditorGUIUtility.IconContent("d_Audio Mixer@2x").image as Texture2D;
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, (EndNameEditAction)ScriptableObject.CreateInstance(doCreateAudioMixer), path, icon, null);
		}

		public static Texture2D PlayBtnIcon() =>
			   EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "PlayButton On@2x" : "PlayButton@2x").image as Texture2D;
		public static Texture2D StopBtnIcon() =>
			EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_PreMatQuad@2x" : "PreMatQuad@2x").image as Texture2D;

		public static string GenGUID(int length = 16)
			=> new string(Guid.NewGuid().ToString("N").Take(length).ToArray());
#endif
	}
}