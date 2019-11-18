using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(UI))]
	public class UIEditor : Editor {
		
		private UI script;

		public void Awake()
		{
			script = (UI) target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.BeginVertical("box");
			EditorGUI.BeginDisabledGroup(script.UIPrefab);
			if (GUILayout.Button("Create default UI"))
			{
				DestroyImmediate(Helper.CreateUI(script));
			}
			EditorGUI.EndDisabledGroup();
			if (script.UIPrefab)
			{
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("The character UI prefab already exist.", MessageType.Info);
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UIPrefab"), new GUIContent("UI prefab"));
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
			}
			else
			{
				EditorGUILayout.EndVertical();
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Health"), new GUIContent("Health text"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("WeaponAmmo"), new GUIContent("Weapons ammo text"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeAmmo"), new GUIContent("Grenades ammo text"));
			EditorGUILayout.EndVertical();
					
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCrosshair"), new GUIContent("Crosshair"));
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("pickUpIcon"), new GUIContent("Pickup icon"));
			EditorGUILayout.EndVertical();
			
			serializedObject.ApplyModifiedProperties();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
			}
		}

		
	}
}

