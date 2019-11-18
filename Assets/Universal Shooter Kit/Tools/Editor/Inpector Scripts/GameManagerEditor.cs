using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(GameManager))]
	public class GameManagerEditor : Editor {

		
		private GameManager script;

		private ReorderableList enemiesList;
		private ReorderableList charactersList;
		
		private void Awake()
		{
			script = (GameManager) target;
		}

		private void OnEnable()
		{
			enemiesList = new ReorderableList(serializedObject, serializedObject.FindProperty("Enemies"), false, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), "Prefab");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 5 + 2, rect.y, rect.width / 5, 
						EditorGUIUtility.singleLineHeight), "Behaviour");
					
					EditorGUI.LabelField(
						new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + rect.width / 4 + 28, rect.y, rect.width / 8,
							EditorGUIUtility.singleLineHeight), "Count");

					EditorGUI.LabelField(
						new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5, rect.y, rect.width / 3,
							EditorGUIUtility.singleLineHeight), "Spawn method");
					
					EditorGUI.LabelField(
						new Rect(rect.x + rect.width / 4 + 12 + rect.width / 10 + 14 + rect.width / 6 + 7 + rect.width / 5 + 15, rect.y, 
							rect.width - rect.width / 4 - 12 - rect.width / 10 - 14 - rect.width / 6 - 7 - rect.width / 5 - 10,
							EditorGUIUtility.singleLineHeight), "Time");
				},

				onAddCallback = items => { script.Enemies.Add(null); },

				onRemoveCallback = items => { script.Enemies.Remove(script.Enemies[items.index]); },


				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.Enemies[index].enemyPrefab = (GameObject) EditorGUI.ObjectField(
						new Rect(rect.x, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight),
						script.Enemies[index].enemyPrefab, typeof(GameObject), false);

					EditorGUI.BeginDisabledGroup(true);
					script.Enemies[index].WaypointBehavior = (WaypointBehavior) EditorGUI.ObjectField(
						new Rect(rect.x + rect.width / 5 + 5, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight),
						script.Enemies[index].WaypointBehavior, typeof(WaypointBehavior), true);
					EditorGUI.EndDisabledGroup();

					script.Enemies[index].Count = EditorGUI.FloatField(
						new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5 + rect.width / 4 + 28, rect.y, rect.width / 12,
							EditorGUIUtility.singleLineHeight), script.Enemies[index].Count);



					if (script.Enemies[index].CurrentSpawnMethodIndex == 0)
					{

						script.Enemies[index].CurrentSpawnMethodIndex = EditorGUI.Popup(
							new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5, rect.y, rect.width / 4 + 20,
								EditorGUIUtility.singleLineHeight), script.Enemies[index].CurrentSpawnMethodIndex,
							new[] {"Random", "Specific Point"});

					}
					else
					{
						script.Enemies[index].CurrentSpawnMethodIndex = EditorGUI.Popup(
							new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5, rect.y, rect.width / 8 + 10,
								EditorGUIUtility.singleLineHeight), script.Enemies[index].CurrentSpawnMethodIndex, new[] {"Random", "Specific Area"});

						script.Enemies[index].SpawnArea = (GameObject) EditorGUI.ObjectField(
							new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5 + rect.width / 8 + 10 + 3, rect.y, rect.width / 8 + 10,
								EditorGUIUtility.singleLineHeight),
							script.Enemies[index].SpawnArea, typeof(GameObject), true);

//						script.Enemies[index].SpawnArea = (GameObject) EditorGUI.ObjectField(
//							new Rect(rect.x + rect.width / 5 + 7 + rect.width / 12 + 7 + rect.width / 4 + 7, rect.y + EditorGUIUtility.singleLineHeight + 6, rect.width / 4, EditorGUIUtility.singleLineHeight),
//							script.Enemies[index].SpawnArea, typeof(GameObject), true);


					}

					script.Enemies[index].SpawnTimeout = EditorGUI.FloatField(
						new Rect(rect.x + rect.width / 4 + 12 + rect.width / 10 + 14 + rect.width / 6 + 7 + rect.width / 5 + 10 + 7, rect.y,
							rect.width - (rect.width / 5 + 5 + rect.width / 12 + 5 + rect.width / 5 + 7 + rect.width / 8 + 10 + 3 + rect.width / 8 + 10) - 5
							, EditorGUIUtility.singleLineHeight),
						script.Enemies[index].SpawnTimeout);

//					EditorGUI.LabelField(
//						new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 8, rect.width / 2 + 10,
//							EditorGUIUtility.singleLineHeight), "Waypoint behaviour");

				},
			//	onSelectCallback = items => { script.CurrentCharacter = items.index; }


			};

			charactersList = new ReorderableList(serializedObject, serializedObject.FindProperty("Characters"), true, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), "Prefab");
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), "Spawn point");
				},
				
				onAddCallback = items =>
				{
					script.Characters.Add(null);
					script.CharactersSpawnPoints.Add(null);
				},

				onRemoveCallback = items =>
				{
					script.Characters.Remove(script.Characters[items.index]);
					script.CharactersSpawnPoints.Remove(script.CharactersSpawnPoints[items.index]);
				},
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.Characters[index] = (GameObject) EditorGUI.ObjectField(
						new Rect(rect.x, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight),
						script.Characters[index], typeof(GameObject), false);

					script.CharactersSpawnPoints[index] = (Transform) EditorGUI.ObjectField(
						new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight),
						script.CharactersSpawnPoints[index], typeof(Transform), true);
				}
				

			};
			
			EditorApplication.update += Update;
		}

		private void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		private void Update()
		{
			if (!script || !script.gameObject.activeInHierarchy || Application.isPlaying) return;

			if (!script.Ui)
				script.Ui = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI.asset", typeof(UI)) as UI;
			
//			if (!script.defaultCrosshair)
//			{
//				script.defaultCrosshair = CharacterHelper.CreateCrosshair(script.transform, "crosshair");
//			}
//			else
//			{
//				if (script.CrosshairType == CharacterHelper.CrosshairType.Image)
//				{
//					script.defaultCrosshair.hideFlags = HideFlags.HideInHierarchy;
//					//script.defaultCrosshair.SetActive(false);
//				}
//				else
//				{
//					script.defaultCrosshair.hideFlags = HideFlags.None;
//					//script.defaultCrosshair.SetActive(true);
//
//				}
//			}
			
//			if (script.Health)
//				script.Health.gameObject.SetActive(script.inspectorTab == 2);
//
//			if (script.GrenadeAmmo)
//				script.GrenadeAmmo.gameObject.SetActive(script.inspectorTab == 2);
//
//			if (script.WeaponAmmo)
//				script.WeaponAmmo.gameObject.SetActive(script.inspectorTab == 2);
//
//			if (script.crosshair)
//				script.crosshair.gameObject.SetActive(script.inspectorTab == 2);
//			
//			if (script.defaultCrosshair)
//				script.defaultCrosshair.SetActive(script.CrosshairType == CharacterHelper.CrosshairType.Default && script.inspectorTab == 2);
			
//			if(script.pickUpIcon)
//				script.pickUpIcon.gameObject.SetActive(script.inspectorTab == 2);
			
			if(script.Resume)
				script.Resume.gameObject.SetActive(script.inspectorTab == 2 && script.UsePause);
			
			if(script.Exit)
				script.Exit.gameObject.SetActive(script.inspectorTab == 2 && script.UsePause);
			
			if(script.pauseBackground)
				script.pauseBackground.SetActive(script.inspectorTab == 2 && script.UsePause);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();
			script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Characters", "Enemies" , "Pause menu"});

			EditorGUILayout.Space();

			switch (script.inspectorTab)
			{
				case 0:
					//enemiesList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 10;

					charactersList.DoLayoutList();
		
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					break;
				case 1:

					enemiesList.DoLayoutList();
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox("[Behaviour] value will be active when the new AI appear.", MessageType.Info);
					break;

				case 2:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePause"), new GUIContent("Use pause"));
					
					if (script.UsePause)
					{
						EditorGUILayout.BeginVertical("box");
						EditorGUILayout.PropertyField(serializedObject.FindProperty("Exit"), new GUIContent("Exit (button)"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("Resume"), new GUIContent("Resume (button)"));
						EditorGUILayout.PropertyField(serializedObject.FindProperty("Restart"), new GUIContent("Restart (button)"));
						EditorGUILayout.Space();
						EditorGUILayout.PropertyField(serializedObject.FindProperty("pauseBackground"), new GUIContent("Background"));
						EditorGUILayout.EndVertical();
					}
					break;
			}
			
			serializedObject.ApplyModifiedProperties();

//			DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}

		}


	}
}

