using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace GercStudio.USK.Scripts
{

	[CustomEditor(typeof(SurfaceParameters))]
	public class SurfaceParametersEditor : Editor
	{
		
		private SurfaceParameters script;
		private ReorderableList shellsList;
		public ReorderableList[] stepsList = new ReorderableList[0];

		private void Awake()
		{
			script = (SurfaceParameters) target;
		}
		
		void OnEnable()
		{
			EditorApplication.update += Update;
			
			if(!script.inputs)
				return;
			
			Array.Resize(ref stepsList, script.inputs.CharacterTags.Count);
			Array.Resize(ref script.footstepsSounds, script.inputs.CharacterTags.Count);

			for (var i = 0; i < stepsList.Length; i++)
			{
				var i1 = i;
				stepsList[i] = new ReorderableList(serializedObject, serializedObject.FindProperty("footstepsSounds")
					.GetArrayElementAtIndex(i).FindPropertyRelative("FootstepsAudios"), false, true, true, true)
				{
					drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Sounds"); },

					onAddCallback = items => { script.footstepsSounds[i1].FootstepsAudios.Add(null); },

					onRemoveCallback = items =>
					{
						script.footstepsSounds[i1].FootstepsAudios.Remove(script.footstepsSounds[i1].FootstepsAudios[items.index]);
					},

					drawElementCallback = (rect, index, isActive, isFocused) =>
					{
						script.footstepsSounds[i1].FootstepsAudios[index] =
							(AudioClip) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.footstepsSounds[i1].FootstepsAudios[index], typeof(AudioClip), false);
					}
				};
			}
			
			shellsList = new ReorderableList(serializedObject, serializedObject.FindProperty("ShellDropSounds"), false, true, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Sounds"); },

				onAddCallback = items => { script.ShellDropSounds.Add(null); },

				onRemoveCallback = items =>
				{
					script.ShellDropSounds.Remove(script.ShellDropSounds[items.index]);
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.ShellDropSounds[index] =
						(AudioClip) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.ShellDropSounds[index], typeof(AudioClip), false);
				}
			};
		}

		void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		void Update()
		{
			if(!script.inputs)
				script.inputs = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(Inputs)) as Inputs;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.Space();
			
			script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Effects", "Step sounds", "Shell sounds"});

			switch (script.inspectorTab)
			{
				case 0:
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Sparks"), new GUIContent("Sparks"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Hit"), new GUIContent("Hit"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("HitAudio"), new GUIContent("Hit audio"));
					
					break;


				case 1:

					EditorGUILayout.Space();
					
					if(!script.inputs)
						return;
					
					script.currentTag = EditorGUILayout.Popup("Character tag", script.currentTag, script.inputs.CharacterTags.ToArray());
					
					EditorGUILayout.Space();
					for (var i = 0; i < stepsList.Length; i++)
					{
						if(i == script.currentTag)
							stepsList[i].DoLayoutList();
					}

					break;


				case 2:
					EditorGUILayout.Space();
					shellsList.DoLayoutList();

					break;
			}



			
			serializedObject.ApplyModifiedProperties();
			
			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
				
				if(!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}

	}
}


