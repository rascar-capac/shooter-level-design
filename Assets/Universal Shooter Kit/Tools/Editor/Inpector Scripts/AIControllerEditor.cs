using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(AIController))]
	public class AIControllerEditor : Editor
	{

		private AIController script;
		
		private ReorderableList weaponsList;

		private bool prefabWarning;
		
		private void Awake()
		{
			script = (AIController) target;
		}
		
		private void OnEnable()
		{
			weaponsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Weapons"), true, true,
				true, true)
			{
				drawHeaderCallback = (Rect rect) =>
				{
					EditorGUI.LabelField(
						new Rect(rect.x + 11, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), "Point");

					EditorGUI.LabelField(
						new Rect(rect.x + rect.width / 2 + 11, rect.y, rect.width / 2,
							EditorGUIUtility.singleLineHeight), "Settings slot");
				},

				onAddCallback = (ReorderableList items) => { script.Weapons.Add(null); },

				onRemoveCallback = (ReorderableList items) => { script.Weapons.Remove(script.Weapons[items.index]); },


				drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
					{
						script.Weapons[index].weapon = (GameObject) EditorGUI.ObjectField(
							new Rect(rect.x, rect.y, rect.width/2, EditorGUIUtility.singleLineHeight),
							script.Weapons[index].weapon, typeof(GameObject), false);
//
//						if (script.Weapons[index].weapon != null && script.Weapons[index].weapon.GetComponent<WeaponController>() &&
//						    script.Weapons[index].weapon.GetComponent<WeaponController>().enumNames != null)
//						{
//							script.Weapons[index].tpSlotIndex = EditorGUI.Popup(
//								new Rect(rect.x + rect.width / 2 + 11, rect.y, rect.width / 2 - 11,
//									EditorGUIUtility.singleLineHeight), script.Weapons[index].tpSlotIndex, 
//								script.Weapons[index].weapon.GetComponent<WeaponController>().enumNames.ToArray());
//						}
//						else
//						{
//							EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 11, rect.y, rect.width / 2 - 11,
//								EditorGUIUtility.singleLineHeight), "Add prefab of weapon");
//						}
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
			var WeaponCount = script.Weapons.Count;
			for (var j = 0; j < WeaponCount; j++)
			{
				if (script.Weapons[j] != null)
				{
					if (script.Weapons[j].weapon)
					{
						if (!script.Weapons[j].weapon.GetComponent<WeaponController>())
						{
							prefabWarning = true;
							script.Weapons[j].weapon = null;
						}
						else
						{
							prefabWarning = false;
						}
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.Space();
			script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new string[] {"Weapons", "?", "?", "?"});

			switch (script.inspectorTab)
			{
				case 0:
					EditorGUILayout.Space();
					weaponsList.DoLayoutList();

					EditorGUILayout.Space();

					if (prefabWarning)
						EditorGUILayout.HelpBox("Your weapon should has [WeaponController] script",
							MessageType.Warning);

					break;

				case 1:

					break;

				case 2:

					break;

				case 3:


					break;

			}

//			DrawDefaultInspector();
			serializedObject.ApplyModifiedProperties();



			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}

		}

	}
}

