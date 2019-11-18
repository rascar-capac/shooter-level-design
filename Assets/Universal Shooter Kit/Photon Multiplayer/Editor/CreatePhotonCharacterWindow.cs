using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEditor;

namespace GercStudio.USK.Scripts
{
	public class CreatePhotonCharacterWindow : EditorWindow
	{

		public GameObject Character;

		private Vector2 scrollPos;

		private GUIStyle LabelStyle;

		private bool hasCreated;
		private bool startCreation;
		private bool characterError;
		private bool CharacterAdded;
//		private bool fontError;
//		private bool fontErrorGUI;

		private float startVal;
		private float progress;



		[MenuItem("Window/USK/Create/Multiplayer Character")]
		public static void ShowWindow()
		{
			GetWindow(typeof(CreatePhotonCharacterWindow), true, "", true).ShowUtility();
		}

		void OnEnable()
		{
			EditorApplication.update += Update;
		}

		void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		private void Awake()
		{
			if (LabelStyle != null) return;
			LabelStyle = new GUIStyle();
			LabelStyle.normal.textColor = Color.black;
			LabelStyle.fontStyle = FontStyle.Bold;
			LabelStyle.fontSize = 12;
			LabelStyle.alignment = TextAnchor.MiddleCenter;
		}

		void Update()
		{
			if (Character)
			{
				if (!CharacterAdded)
				{
					//fontError = false;
					characterError = false;
//					if (!Character.activeInHierarchy)
//					{
//						Character = Instantiate(Character, Vector3.zero, Quaternion.Euler(Vector3.zero));
//						Character.SetActive(true);
//					}

					CharacterAdded = true;
				}
				else
				{
					if (!Character.GetComponent<Controller>() ||
					    !Character.GetComponent<InventoryManager>() ||
					    !Character.GetComponent<Animator>())
					{
						characterError = true;
						Character = null;
						return;
					}

					if (!Character.GetComponent<Animator>())
					{
						characterError = true;
						Character = null;
						return;
					}

					if (!Character.GetComponent<Animator>().runtimeAnimatorController)
					{
						characterError = true;
						Character = null;
						return;
					}

					if (Character.GetComponent<Animator>().runtimeAnimatorController.name != "Character")
					{
						characterError = true;
						Character = null;
						return;

					}

					if (!Character.GetComponent<Animator>().avatar)
					{
						characterError = true;
						Character = null;
						return;
					}

					if (!Character.GetComponent<Animator>().avatar.isHuman)
					{
						characterError = true;
						Character = null;
						return;
					}

				}

				if (startCreation & progress > 1.1f)
				{
//					if (CheckFont())
//					{
						//fontError = false;
						CreateCharacter();
						hasCreated = true;
						startVal = (float) EditorApplication.timeSinceStartup;
//					}
//					else
//					{
//						fontError = true;
//						fontErrorGUI = true;
//					}

					startCreation = false;
				}
			}
			else
			{
				CharacterAdded = false;
//				fontError = false;
//				fontErrorGUI = false;
			}

			if (hasCreated)
			{
				if (progress > 5)
				{
					hasCreated = false;
//					fontError = false;
					Character = null;
				}
			}
		}

//		bool CheckFont()
//		{
//			if (!font)
//				font = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
//
//			if (font)
//				return true;
//
//			return false;
//		}

		private void OnGUI()
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));
			
			EditorGUILayout.Space();
			GUILayout.Label("Create Multiplayer Character", LabelStyle);
			EditorGUILayout.Space();
			if (hasCreated)
			{
				var style = new GUIStyle
				{
					normal = {textColor = Color.green},
					fontStyle = FontStyle.Bold,
					fontSize = 10,
					alignment = TextAnchor.MiddleCenter
				};
				EditorGUILayout.LabelField("Character has been created", style);
				EditorGUILayout.HelpBox(
					"To use this character in the game, set it in [Lobby] script in the Lobby scene.",
					MessageType.Info);
				EditorGUILayout.Space();
			}

			EditorGUILayout.BeginVertical("box");

			if (!Character)
			{
				if (characterError)
				{
					EditorGUILayout.HelpBox(
						"The character must have [Controller], [Health] and [WeaponManger] scripts," +
						" and also [AnimatorController] component with [Character] controller." + "\n\n" +
						"Please first create a character for a single game mode: [Window -> USK -> Create Character], then put it here.",
						MessageType.Warning);
				}
				else
				{
					EditorGUILayout.HelpBox("Please put a single-player character here.", MessageType.Info);
				}
			}

			Character = (GameObject) EditorGUILayout.ObjectField("Single Player Character", Character, typeof(GameObject), true);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();

//			if (fontError)
//			{
//				EditorGUILayout.BeginVertical("box");
//				if (fontErrorGUI)
//					EditorGUILayout.HelpBox("Standard font for the UI was not found, please set some other.", MessageType.Warning);
//
//				font = (Font) EditorGUILayout.ObjectField("Font", font, typeof(Font), false);
//				EditorGUILayout.EndVertical();
//				EditorGUILayout.Space();
//
//				fontErrorGUI = !font;
//			}

			if (Character )//& !fontErrorGUI)
			{
				if (startCreation)
					EditorGUI.ProgressBar(new Rect(3, 70, position.width - 6, 20), progress / 1, "Creation...");
			}

			EditorGUI.BeginDisabledGroup(!Character);// || fontErrorGUI);

			if (!startCreation)
				if (GUILayout.Button("Create"))
				{
					//fontError = false;
					characterError = false;
					startVal = (float) EditorApplication.timeSinceStartup;
					startCreation = true;
				}

			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndScrollView();

			progress = (float) (EditorApplication.timeSinceStartup - startVal);
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}

		void CreateCharacter()
		{
			Character = Instantiate(Character, Vector3.zero, Quaternion.Euler(Vector3.zero));
			Character.SetActive(true);
			
			PhotonView photonView;
			PhotonAnimatorView photonAnimatorView;
			CharacterSync characterSync;
			PhotonTransformView transformView;

			photonView = !Character.GetComponent<PhotonView>() ? Character.AddComponent<PhotonView>() : Character.GetComponent<PhotonView>();

			if (photonView != null)
			{
				if (!Character.GetComponent<PhotonAnimatorView>())
				{
					photonAnimatorView = Character.AddComponent<PhotonAnimatorView>();

					photonAnimatorView.SetLayerSynchronized(0, PhotonAnimatorView.SynchronizeType.Disabled);
					photonAnimatorView.SetLayerSynchronized(1, PhotonAnimatorView.SynchronizeType.Continuous);
					photonAnimatorView.SetLayerSynchronized(2, PhotonAnimatorView.SynchronizeType.Continuous);
					photonAnimatorView.SetLayerSynchronized(3, PhotonAnimatorView.SynchronizeType.Continuous);
					
					photonAnimatorView.SetParameterSynchronized("Move", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Crouch", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("FPS", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("TPS", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
						
					photonAnimatorView.SetParameterSynchronized("TDS", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Aim", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Horizontal", PhotonAnimatorView.ParameterType.Float,
						PhotonAnimatorView.SynchronizeType.Continuous);
					photonAnimatorView.SetParameterSynchronized("Vertical", PhotonAnimatorView.ParameterType.Float,
						PhotonAnimatorView.SynchronizeType.Continuous);
					
					photonAnimatorView.SetParameterSynchronized("FallingHeight", PhotonAnimatorView.ParameterType.Float,
						PhotonAnimatorView.SynchronizeType.Continuous);
					
					photonAnimatorView.SetParameterSynchronized("OnFloor", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Continuous);
					
					photonAnimatorView.SetParameterSynchronized("OnFloorForward", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Continuous);
					
					photonAnimatorView.SetParameterSynchronized("Jump", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Angle", PhotonAnimatorView.ParameterType.Float,
						PhotonAnimatorView.SynchronizeType.Continuous);
					
					photonAnimatorView.SetParameterSynchronized("CameraAngle", PhotonAnimatorView.ParameterType.Float,
						PhotonAnimatorView.SynchronizeType.Continuous);
					
					photonAnimatorView.SetParameterSynchronized("MoveButtonHasPressed", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Sprint", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Attack", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("PressMoveAxis", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("NoWeapons", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("HasWeaponTaken", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("CanWalkWithWeapon", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Forward", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Backward", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Left", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("Right", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("ForwardLeft", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("ForwardRight", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("BackwardLeft", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
					photonAnimatorView.SetParameterSynchronized("BackwardRight", PhotonAnimatorView.ParameterType.Bool,
						PhotonAnimatorView.SynchronizeType.Discrete);
					
				}
				else
				{
					photonAnimatorView = Character.GetComponent<PhotonAnimatorView>();
				}
				
				transformView = !Character.GetComponent<PhotonTransformView>() ? Character.AddComponent<PhotonTransformView>() : Character.GetComponent<PhotonTransformView>();

				characterSync = !Character.GetComponent<CharacterSync>() ? Character.AddComponent<CharacterSync>() : Character.GetComponent<CharacterSync>();

				if (characterSync)
					if (!characterSync.PlayerStatsText)
						characterSync.PlayerStatsText = CreateStatsText().GetComponent<TextMesh>();


				if (photonView && characterSync && photonAnimatorView && transformView)
				{
					photonView.ObservedComponents = new List<Component> {characterSync, photonAnimatorView, transformView};
					photonView.Synchronization = ViewSynchronization.UnreliableOnChange;

					SaveCharacter();
				}
			}
		}

		void SaveCharacter()
		{
			if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Photon Multiplayer/Resources/"))
			{
				Directory.CreateDirectory("Assets/Universal Shooter Kit/Photon Multiplayer/Resources/");
			}

			var index = 0;
			while(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Photon Multiplayer/Resources/" + Character.name + " " + index + ".prefab", typeof(GameObject)) != null)
			{
				index++;
			}
			
#if !UNITY_2018_3_OR_NEWER
			var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Photon Multiplayer/Resources/" + Character.name + " " + index + ".prefab");
			PrefabUtility.ReplacePrefab(Character, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
			PrefabUtility.SaveAsPrefabAsset(Character, "Assets/Universal Shooter Kit/Photon Multiplayer/Resources/" + Character.name + " " + index + ".prefab");
#endif
			
			EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Photon Multiplayer/Resources/" + Character.name + " " + index + ".prefab", typeof(GameObject)) as GameObject);
			DestroyImmediate(Character);
		}

		GameObject CreateStatsText()
		{
			var statsText = new GameObject {name = "Stats Text"};
			var material = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/3dTextMaterial.mat", typeof(Material)) as Material;
			var font = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
			
			
			if (!material)
			{
				material = new Material(Shader.Find("Standard"));
				var shader = Shader.Find("3D Text Shader");
				
				if (shader)
					material.shader = shader;

				if(font != null)
					material.mainTexture = font.material.mainTexture;
				
				material.color = Color.green;
			}

			statsText.AddComponent<MeshRenderer>().material = material;

			var textMesh = statsText.AddComponent<TextMesh>();
			textMesh.text = "100";
			textMesh.anchor = TextAnchor.UpperCenter;
			textMesh.alignment = TextAlignment.Center;
			textMesh.fontSize = 250;
			textMesh.fontStyle = FontStyle.Bold;
			
			if(font)
				textMesh.font = font;
			
			textMesh.color = Color.green;

			statsText.transform.parent = Character.transform;
			statsText.transform.position = Character.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).position + Vector3.up;
			statsText.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

			return statsText;
		}
	}
}
