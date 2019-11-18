using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
    
   [CustomEditor(typeof(Lobby))]
   public class LobbyEditor : Editor
   {
       public Lobby script;
       
       private ReorderableList charactersList;
       private ReorderableList levelsList;
   
       public void Awake()
       {
           script = (Lobby) target;
       }

       public void OnEnable()
       {
           charactersList = new ReorderableList(serializedObject, serializedObject.FindProperty("Characters"), true, true, true, true)
           {
               drawHeaderCallback = rect =>
               {
                   EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),"Character");

                   EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), "Character UI");
               },
               
               onAddCallback = items =>
               {
                   if (!Application.isPlaying)
                   {
                       script.Characters.Add(new CharacterHelper.PhotonCharacter());
                   }
               },
				
               onRemoveCallback = items =>
               {
                   if (!Application.isPlaying)
                   {
                       script.Characters.Remove(script.Characters[items.index]);
                   }
               },
               
               drawElementCallback = (rect, index, isActive, isFocused) =>
               {
                   script.Characters[index].Character = (GameObject) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                       script.Characters[index].Character, typeof(GameObject), false);
                   
                   script.Characters[index].Image = (GameObject) EditorGUI.ObjectField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight),
                       script.Characters[index].Image, typeof(GameObject), true);
               }
           };
           
           levelsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Levels"), false, true, true, true)
           {
               drawHeaderCallback = rect =>
               {
                   EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),"Scene");

                   EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), "Level Button");
               },
               
               onAddCallback = items =>
               {
                   if (!Application.isPlaying)
                   {
                       script.Levels.Add(new CharacterHelper.PhotonLevel());
                   }
               },
				
               onRemoveCallback = items =>
               {
                   if (!Application.isPlaying)
                   {
                       script.Levels.Remove(script.Levels[items.index]);
                   }
               },
               
               drawElementCallback = (rect, index, isActive, isFocused) =>
               {
//                   script.Levels[index].LevelName = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), script.Levels[index].LevelName);
                   
                   script.Levels[index].Scene = (SceneAsset) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                       script.Levels[index].Scene, typeof(SceneAsset), false);
                   
                   script.Levels[index].LevelButton = (Button) EditorGUI.ObjectField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight),
                       script.Levels[index].LevelButton, typeof(Button), true);
               }
           };

           EditorApplication.update += Update;
       }

       private void OnDisable()
       {
           EditorApplication.update -= Update;
       }

       public void Update()
       {
           foreach (var level in script.Levels)
           {
               if (!level.Scene) continue;

               if (!string.Equals(level.Scene.name, level.LevelName, StringComparison.Ordinal))
                   level.LevelName = level.Scene.name;
           }
       }

       public override void OnInspectorGUI()
       {
//           Array.Resize(ref script.CharactersPrefabs, script.CharactersCount);
//           Array.Resize(ref script.CharactersImages, script.CharactersCount);
//           Array.Resize(ref script.LevelsNames, script.LevelsCount);
//           Array.Resize(ref script.LevelsButtons, script.LevelsCount);
           
           serializedObject.Update();
           
           
           EditorGUILayout.Space();
           
           script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Characters", "Levels", "UI Elements"});

           switch (script.inspectorTab)
           {
               case 0:
                   
                   EditorGUILayout.Space();
                   charactersList.DoLayoutList();
                   EditorGUILayout.Space();
                   break;

               case 1:
                   EditorGUILayout.Space();
                   levelsList.DoLayoutList();
                   EditorGUILayout.Space();

                   if (GUILayout.Button("Add to Build Settings"))
                   {
                       var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
                       
                       editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(SceneManager.GetActiveScene().path, true));
                       
                       foreach (var sceneAsset in script.Levels)
                       {
                           if (!sceneAsset.Scene) continue;
                           
                           var scenePath = AssetDatabase.GetAssetPath(sceneAsset.Scene);
                           
                           if (!string.IsNullOrEmpty(scenePath))
                               editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                       }
                       
                       EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
                   }

                   break;

               case 2:

                   EditorGUILayout.Space();
                   EditorGUILayout.LabelField("Menu Panels", EditorStyles.boldLabel);
                   EditorGUILayout.BeginVertical("box", GUILayout.Width(EditorGUIUtility.currentViewWidth - 42));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("MainMenu"), new GUIContent("Main Menu"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("FindRooms"), new GUIContent("Find Rooms"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("CreateRooms"), new GUIContent("Create Rooms"));
                   EditorGUILayout.EndVertical();
                   EditorGUILayout.Space();

                   EditorGUILayout.LabelField("'Main Menu' UI Elements", EditorStyles.boldLabel);
                   EditorGUILayout.BeginVertical("box", GUILayout.Width(EditorGUIUtility.currentViewWidth - 42));

                   EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerNameText"), new GUIContent("Player Name (Input Field)"));
                   EditorGUILayout.Space();
                   EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("FindRoomsButton"), new GUIContent("Find Rooms"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("CreateRoomButton"), new GUIContent("Create RoomManager"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("RandomRoomButton"), new GUIContent("Random RoomManager"));

                   EditorGUILayout.BeginHorizontal();
                   EditorGUILayout.LabelField("Up Count", GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 27));
                   EditorGUILayout.LabelField("Down Count", GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 27));
                   EditorGUILayout.EndHorizontal();

                   EditorGUILayout.BeginHorizontal();
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("DownCharacterCount"), new GUIContent(""),
                       GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 27));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("UpCharacterCount"), new GUIContent(""),
                       GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 27));
                   EditorGUILayout.EndHorizontal();

                   EditorGUILayout.EndVertical();
                   EditorGUILayout.Space();

                   EditorGUILayout.LabelField("'Find Rooms' UI Elements", EditorStyles.boldLabel);
                   EditorGUILayout.BeginVertical("box", GUILayout.Width(EditorGUIUtility.currentViewWidth - 42));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("RoomListingPrefab"), new GUIContent("RoomManager Listing (Prefab)"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("roomsPanel"), new GUIContent("Rooms Content"));
                   EditorGUILayout.Space();
                   EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("FindRoomsBackButton"), new GUIContent("Find Rooms Back"));
                   EditorGUILayout.EndVertical();
                   EditorGUILayout.Space();

                   EditorGUILayout.LabelField("'Create Rooms' UI Elements", EditorStyles.boldLabel);
                   EditorGUILayout.BeginVertical("box", GUILayout.Width(EditorGUIUtility.currentViewWidth - 42));
                   EditorGUILayout.BeginVertical("box");
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxPlayersSlider"), new GUIContent("Max Players (Slider)"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxPlayersText"), new GUIContent("Max Players (Text)"));
                   EditorGUILayout.EndVertical();
                   EditorGUILayout.BeginVertical("box");
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("MinPlayersSlider"), new GUIContent("Min Players (Slider)"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("MinPlayersText"), new GUIContent("Min Players (Text)"));
                   EditorGUILayout.EndVertical();
                   EditorGUILayout.BeginVertical("box");
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("TimeSlider"), new GUIContent("Match Time (Slider)"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("TimeValueText"), new GUIContent("Match Time (Text)"));
                   EditorGUILayout.EndVertical();
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("RoomName"), new GUIContent("RoomManager Name (InputField)"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("GameModeDropdown"), new GUIContent("Game Mode (DropDown)"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("EnemiesToggle"), new GUIContent("On/Off Enemies (Toggle)"));
                   EditorGUILayout.Space();
                   EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("CreateRoomsBackButton"), new GUIContent("Create RoomManager Back"));
                   EditorGUILayout.PropertyField(serializedObject.FindProperty("Create"), new GUIContent("Create RoomManager"));
                   EditorGUILayout.EndVertical();
                   EditorGUILayout.Space();
                   break;
           }
//
//           EditorGUILayout.LabelField("Characters",EditorStyles.boldLabel);
//           EditorGUILayout.BeginVertical("box",GUILayout.Width(EditorGUIUtility.currentViewWidth - 42));
//           EditorGUILayout.PropertyField(serializedObject.FindProperty("CharactersCount"), new GUIContent("Characters Count"));
//           int Playerscount = script.CharactersCount;        
//           for (int i = 0; i < Playerscount; i++)
//           {
//               EditorGUILayout.BeginVertical("box");
//               EditorGUILayout.PropertyField(serializedObject.FindProperty("CharactersPrefabs").GetArrayElementAtIndex(i),new GUIContent("Character " + (i + 1)));
//               EditorGUILayout.PropertyField(serializedObject.FindProperty("CharactersImages").GetArrayElementAtIndex(i),new GUIContent("Character Image"));
//               EditorGUILayout.EndVertical();
//           }
//           EditorGUILayout.EndVertical();
//           EditorGUILayout.Space();
//           
//           EditorGUILayout.LabelField("Levels",EditorStyles.boldLabel);
//           EditorGUILayout.BeginVertical("box",GUILayout.Width(EditorGUIUtility.currentViewWidth - 42));
//           EditorGUILayout.PropertyField(serializedObject.FindProperty("LevelsCount"), new GUIContent("Levels Count"));
//           int Levelscount = script.LevelsCount;
//           
//           for (int i = 0; i < Levelscount; i++)
//           {
//               EditorGUILayout.BeginVertical("box");
//               EditorGUILayout.PropertyField(serializedObject.FindProperty("LevelsNames").GetArrayElementAtIndex(i),new GUIContent("Level Build Name " + (i + 1)));
//               EditorGUILayout.PropertyField(serializedObject.FindProperty("LevelsButtons").GetArrayElementAtIndex(i),new GUIContent("Level Button"));
//               EditorGUILayout.EndVertical();
//           }
//           EditorGUILayout.EndVertical();
//           EditorGUILayout.Space();
           
          
           
           serializedObject.ApplyModifiedProperties();
           if (GUI.changed)
               EditorUtility.SetDirty(script.gameObject);
   
       }
   
   } 
    
}


