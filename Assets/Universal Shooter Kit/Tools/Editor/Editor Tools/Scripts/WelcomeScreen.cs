// GercStudio
// © 2018-2019

using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace GercStudio.USK.Scripts
{
	
	[InitializeOnLoad]
	public class WelcomeScreen : EditorWindow
	{
		private static WelcomeScreen window;
		private static readonly Vector2 windowsSize = new Vector2(500f, 350f);
		private Vector2 scrollPosition;

		private static string windowHeaderText = "Welcome!";
		private readonly string copyright = "© Copyright " + DateTime.Now.Year + " GercStudio";

		private const string isShowAtStartEditorPrefs = "WelcomeScreenShowAtStart";
		private static bool isShowAtStart = true;

		private static bool isInited;

		private static GUIStyle headerStyle;
		private static GUIStyle copyrightStyle;

		private static Texture2D PUNTexture;
		private static Texture2D youTubeIcon;
		private static Texture2D unityConnectIcon;
		private static Texture2D supportIcon;
		private static Texture2D twitterIcon;

		static WelcomeScreen()
		{
			EditorApplication.update -= GetShowAtStart;
			EditorApplication.update += GetShowAtStart;
		}

		private void OnGUI()
		{
			if (!isInited)
			{
				Init();
			}

			GUI.Button(new Rect(0f, 0f, windowsSize.x, 58f), "", headerStyle);

			GUILayoutUtility.GetRect(position.width, 70f);
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.black;
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 12;
			style.alignment = TextAnchor.MiddleCenter;

			
			GUILayout.Label(
				"The GercStudio team is grateful that you have purchased our project! " + "\n" +
				"We believe that thanks to it you will be able to create impressive games.", style);
			GUILayout.Space(20f);

			if (DrawButton(supportIcon, "Support", "First of all, please read the docs. If it didn't help, email us."))
				Process.Start("mailto:gercstudio@gmail.com?subject=USK Support");

			if (DrawButton(youTubeIcon, "YouTube Channel", "Watch our tutorial videos."))
				Process.Start("https://www.youtube.com/channel/UC82Abqbtvl3wsO86yDdGVng");

			if (DrawButton(unityConnectIcon, "Unity Forum", "Follow the news and ask questions on our forum."))
				Process.Start(
					"https://forum.unity.com/threads/released-universal-shooter-kit-fps-tps-tds.596497/#post-3986848");




			EditorGUILayout.EndScrollView();

			EditorGUILayout.LabelField(copyright, copyrightStyle);
		}

		private static bool Init()
		{
			try
			{
				headerStyle = new GUIStyle();
				headerStyle.normal.background = Resources.Load("HeaderLogo") as Texture2D;
				headerStyle.normal.textColor = Color.white;
				headerStyle.fontStyle = FontStyle.Bold;
				headerStyle.padding = new RectOffset(340, 0, 27, 0);
				headerStyle.margin = new RectOffset(0, 0, 0, 0);

				copyrightStyle = new GUIStyle();
				copyrightStyle.alignment = TextAnchor.MiddleRight;
				supportIcon = Resources.Load("Support") as Texture2D;
				youTubeIcon = Resources.Load("YouTube") as Texture2D;
				unityConnectIcon = Resources.Load("UnityConnect") as Texture2D;

				isInited = true;
			}
			catch (Exception e)
			{
				Debug.Log("WELCOME SCREEN INIT: " + e);
				return false;
			}

			return true;
		}

		private static bool DrawButton(Texture2D icon, string title = "", string description = "")
		{
			GUILayout.BeginHorizontal();

			GUILayout.Space(34f);
			GUILayout.Box(icon, GUIStyle.none, GUILayout.MaxWidth(48f), GUILayout.MaxHeight(48f));
			GUILayout.Space(10f);

			GUILayout.BeginVertical();

			GUILayout.Space(1f);
			GUILayout.Label(title, EditorStyles.boldLabel);
			GUILayout.Label(description);

			GUILayout.EndVertical();

			GUILayout.EndHorizontal();

			Rect rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

			GUILayout.Space(10f);

			return Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition);
		}


		private static void GetShowAtStart()
		{
			EditorApplication.update -= GetShowAtStart;

			isShowAtStart = EditorPrefs.GetBool(isShowAtStartEditorPrefs, true);

			if (isShowAtStart)
			{
				EditorApplication.update -= OpenAtStartup;
				EditorApplication.update += OpenAtStartup;
			}
		}

		private static void OpenAtStartup()
		{
			if (isInited && Init())
			{
				OpenWindow();

				EditorApplication.update -= OpenAtStartup;
			}
		}

		//[MenuItem("USK/Welcome Screen", false)]
		public static void OpenWindow()
		{
			if (window == null)
			{
				window = GetWindow<WelcomeScreen>(true, windowHeaderText, true);
				window.maxSize = window.minSize = windowsSize;
			}
		}

		private void OnEnable()
		{
			window = this;
		}

		private void OnDestroy()
		{
			window = null;

			EditorPrefs.SetBool(isShowAtStartEditorPrefs, false);
		}
	}

}

