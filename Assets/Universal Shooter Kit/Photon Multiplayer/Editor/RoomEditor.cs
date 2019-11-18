using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GercStudio.USK.Scripts
{

    [CustomEditor(typeof(RoomManager))]
    public class RoomEditor : Editor
    {
        public RoomManager script;
        private ReorderableList enemiesList;

        public void Awake()
        {
            script = (RoomManager) target;
        }

        public void OnEnable()
        {
            enemiesList = new ReorderableList(serializedObject, serializedObject.FindProperty("Enemies"), false, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),"Enemy");

//                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), "Character UI");
                },
               
                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.Enemies.Add(null);
                    }
                },
				
                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.Enemies.Remove(script.Enemies[items.index]);
                    }
                },
               
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.Enemies[index] = (GameObject) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.Enemies[index], typeof(GameObject), false);
                }
            };
            
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (!script.Ui && !Application.isPlaying)
                script.Ui = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI.asset", typeof(UI)) as UI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            
            script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Start menu", "Game stats",  "Enemies"});

            switch (script.inspectorTab)
            {
              case 0:
                  
                  
                          EditorGUILayout.Space();
                          EditorGUILayout.LabelField("Start Menu Elements", EditorStyles.boldLabel);
                          EditorGUILayout.BeginVertical("box");
                          EditorGUILayout.PropertyField(serializedObject.FindProperty("StartMenu"),
                              new GUIContent("Start Menu (Panel)"));
                          EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultCamera"),
                              new GUIContent("Default Camera"));
                          EditorGUILayout.Space();
                          EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayersPanel"),
                              new GUIContent("Players Content"));
                          EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerListingPrefab"),
                              new GUIContent("Player Listing (Prefab)"));
                          EditorGUILayout.Space();
                          EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
                          EditorGUILayout.PropertyField(serializedObject.FindProperty("StartButton"), new GUIContent("Start"));
                          EditorGUILayout.PropertyField(serializedObject.FindProperty("BackButton"), new GUIContent("Exit"));
                          EditorGUILayout.PropertyField(serializedObject.FindProperty("ResumeButton"), new GUIContent("Resume"));
                          EditorGUILayout.EndVertical();


                          break;
              
              case 1:
                  
                  
                  EditorGUILayout.Space();
                  EditorGUILayout.LabelField("Game Stats Elements", EditorStyles.boldLabel);
                  EditorGUILayout.BeginVertical("box");
                  EditorGUILayout.PropertyField(serializedObject.FindProperty("StatsContent"),
                      new GUIContent("Stats Content"));
                  EditorGUILayout.PropertyField(serializedObject.FindProperty("StatsText"), new GUIContent("Stats (Prefab)"));
                  EditorGUILayout.Space();
                  EditorGUILayout.PropertyField(serializedObject.FindProperty("TimerText"),
                      new GUIContent("Match Timer (Text)"));
                  EditorGUILayout.PropertyField(serializedObject.FindProperty("RestartTimer"),
                      new GUIContent("Restart Timer (Text)"));
                  EditorGUILayout.PropertyField(serializedObject.FindProperty("RestartTime"), new GUIContent("Restart Time"));
                  EditorGUILayout.EndVertical();

                  break;
              
              case 2:
                  EditorGUILayout.Space();
                  EditorGUILayout.LabelField("Enemies control", EditorStyles.boldLabel);
                  

                  enemiesList.DoLayoutList();

                  EditorGUILayout.Space();
                  EditorGUILayout.BeginVertical("box");
                  EditorGUILayout.PropertyField(serializedObject.FindProperty("timeout"), new GUIContent("Spawn Timeout "));
                  EditorGUILayout.PropertyField(serializedObject.FindProperty("quantity"),
                      new GUIContent("Quantity of enemies"));
                  EditorGUILayout.EndVertical();
                  break;
            }

            serializedObject.ApplyModifiedProperties();
//            DrawDefaultInspector();
            if (GUI.changed)
                EditorUtility.SetDirty(script.gameObject);


        }
    }

}

