using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(Inputs))]
	public class InputsEditor : Editor
	{
		private Inputs script;

		public void Awake()
		{
			script = (Inputs) target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			script.tab = GUILayout.Toolbar(script.tab, new string[] {"Character", "Weapons", "Inventory", "Other"});
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginVertical("box");
			EditorGUI.BeginDisabledGroup(script.MobileInputs);
			if (GUILayout.Button("Create mobile buttons"))
			{
				DestroyImmediate(Helper.CreateButtons(script));
			}
			EditorGUI.EndDisabledGroup();
			if (script.MobileInputs)
			{
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("The mobile buttons already exist.", MessageType.Info);
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("MobileInputs"), new GUIContent("Mobile buttons"));
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();
			}
			else
			{
				EditorGUILayout.EndVertical();
			}
			
			EditorGUILayout.Space();
			switch (script.tab)
			{
				case 0:
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.GamepadAxes[0] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Forward/backward", script.GamepadAxes[0]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("invertAxes").GetArrayElementAtIndex(0), new GUIContent("Invert"));

					script.GamepadAxes[1] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Right/left", script.GamepadAxes[1]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("invertAxes").GetArrayElementAtIndex(1), new GUIContent("Invert"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[12] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Forward", script.KeyBoardCodes[12]);
					script.KeyBoardCodes[13] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Backward", script.KeyBoardCodes[13]);
					script.KeyBoardCodes[14] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Right", script.KeyBoardCodes[14]);
					script.KeyBoardCodes[15] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Left", script.KeyBoardCodes[15]);
					
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.LabelField("Mobile", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					
					script.uiButtons[12] = (GameObject)EditorGUILayout.ObjectField("Move stick", script.uiButtons[12], typeof(GameObject), true);
					
					script.uiButtons[13] = (GameObject)EditorGUILayout.ObjectField("Stick outline", script.uiButtons[13], typeof(GameObject), true);
					
					EditorGUILayout.PropertyField(serializedObject.FindProperty("StickRange"), new GUIContent("Stick range (in px)"));
		
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Rotate camera", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.GamepadAxes[2] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Horizontal axis", script.GamepadAxes[2]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("invertAxes").GetArrayElementAtIndex(2), new GUIContent("Invert"));

					script.GamepadAxes[3] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Vertical axis", script.GamepadAxes[3]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("invertAxes").GetArrayElementAtIndex(3), new GUIContent("Invert"));
					EditorGUILayout.EndVertical();
		
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Change camera type", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[11] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[11]);
					
					if(CheckGamepadCode(11))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[11] =
						(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[11]);

					if (CheckGamepadCode(11))
					{
						script.AxisButtonValues[11] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[11]);
						EditorGUILayout.EndVertical();
					}

					script.uiButtons[2] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[2], typeof(GameObject), true);
					
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Change character", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[18] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[18]);
					
					if(CheckGamepadCode(16))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[16] =
						(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[16]);

					if (CheckGamepadCode(16))
					{
						script.AxisButtonValues[16] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[16]);
						EditorGUILayout.EndVertical();
					}

					script.uiButtons[16] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[16], typeof(GameObject), true);
					
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Sprint", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("PressSprintButton"),
						new GUIContent("Press Button"));
					
					EditorGUILayout.HelpBox(script.PressSprintButton ? "Hold the button to run." : "Click the button to run. Press again to stop running.", MessageType.Info);
					
					script.KeyBoardCodes[0] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[0]);
					
					if(CheckGamepadCode(0))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[0] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[0]);
					
					if (CheckGamepadCode(0))
					{
						script.AxisButtonValues[0] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[0]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[6] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[6], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Crouch", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("PressCrouchButton"),
						new GUIContent("Press Button"));
					EditorGUILayout.HelpBox(
						script.PressCrouchButton
							? "Hold the button to crouch."
							: "Click the button to crouch. Press again to stop squatting.", MessageType.Info);
					script.KeyBoardCodes[1] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[1]);
					
					if(CheckGamepadCode(1))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[1] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[1]);
					
					if (CheckGamepadCode(1))
					{
						script.AxisButtonValues[1] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[1]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[7] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[7], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Jump", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[2] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[2]);
					
					if(CheckGamepadCode(2))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[2] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[2]);
					
					if (CheckGamepadCode(2))
					{
						script.AxisButtonValues[2] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[2]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[8] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[8], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					break;
				
				case 1:
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Attack", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[3] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[3]);
					
					if(CheckGamepadCode(3))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[3] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[3]);
					
					if (CheckGamepadCode(3))
					{
						script.AxisButtonValues[3] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[3]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[5] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[5], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Reload", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[4] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[4]);
					
					if(CheckGamepadCode(4))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[4] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[4]);
					
					if (CheckGamepadCode(4))
					{
						script.AxisButtonValues[4] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[4]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[1] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[1], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Aim", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[5] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[5]);
					
					if(CheckGamepadCode(5))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[5] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[5]);
					
					if (CheckGamepadCode(5))
					{
						script.AxisButtonValues[5] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[5]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[0] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[0], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Launch grenade", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[6] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[6]);
					
					if(CheckGamepadCode(6))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[6] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[6]);
					
					if (CheckGamepadCode(6))
					{
						script.AxisButtonValues[6] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[6]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[3] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[3], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Change attack type", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[19] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[19]);
					
					if(CheckGamepadCode(17))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[17] =
						(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[17]);

					if (CheckGamepadCode(17))
					{
						script.AxisButtonValues[17] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[17]);
						EditorGUILayout.EndVertical();
					}

					script.uiButtons[17] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[17], typeof(GameObject), true);
					
					EditorGUILayout.EndVertical();
					
					break;
				
				case 2:
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Open/Close Inventory", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("pressInventoryButton"),
						new GUIContent("Press button"), true);
					EditorGUILayout.HelpBox(
						script.pressInventoryButton
							? "Hold the button to keep the inventory open."
							: "Click the button to open the inventory, then click again to close.", MessageType.Info);
					script.KeyBoardCodes[7] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[7]);
					
					if(CheckGamepadCode(7))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[7] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[7]);
					
					if (CheckGamepadCode(7))
					{
						script.AxisButtonValues[7] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[7]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[10] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[10], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Pickup object", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[8] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[8]);
					
					if(CheckGamepadCode(8))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[8] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[8]);
					
					if (CheckGamepadCode(8))
					{
						script.AxisButtonValues[8] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[8]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[11] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[11], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Drop weapon", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[9] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[9]);
					
					if(CheckGamepadCode(9))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[9] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[9]);
					
					if (CheckGamepadCode(9))
					{
						script.AxisButtonValues[9] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[9]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[4] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[4], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Change weapon", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.LabelField("Up", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[16] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[16]);
					
					if(CheckGamepadCode(14))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[14] =
						(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[14]);
					
					if (CheckGamepadCode(14))
					{
						script.AxisButtonValues[14] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[14]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[14] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[14], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					EditorGUILayout.LabelField("Down", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[17] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[17]);
					
					if(CheckGamepadCode(15))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[15] =
						(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[15]);
					
					if (CheckGamepadCode(15))
					{
						script.AxisButtonValues[15] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[15]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[15] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[15], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Change weapon in slot", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.GamepadAxes[4] =
						(Helper.GamepadAxes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadAxes[4]);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Use health", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					
					
					script.GamepadCodes[12] =
						(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[12]);
					
					if (CheckGamepadCode(12))
						script.AxisButtonValues[12] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[12]);

					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Use ammo", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.GamepadCodes[13] =
						(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[13]);
					
					if (CheckGamepadCode(13))
						script.AxisButtonValues[13] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[13]);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Stick to choice weapons", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script._Stick = (Inputs.Stick) EditorGUILayout.EnumPopup("Stick", script._Stick);
					EditorGUILayout.EndVertical();
					
					break;
				case 3:
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Pause", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("box");
					script.KeyBoardCodes[10] =
						(Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[10]);
					
					if(CheckGamepadCode(10))
						EditorGUILayout.BeginVertical("box");
					
					script.GamepadCodes[10] =
								(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[10]);
					
					if (CheckGamepadCode(10))
					{
						script.AxisButtonValues[10] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[10]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[9] =
						(GameObject)EditorGUILayout.ObjectField("Mobile", script.uiButtons[9], typeof(GameObject), true);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("box");
					EditorGUILayout.HelpBox("Switching from the gamepad to the keyboard/mouse and back is automatic. " + "\n" +
					                        "But sometimes there are problems, and the computer sees the gamepad, even if it is disabled - this is due to the drivers in Windows. " + "\n" +
					                        "You can disable the gamepads control manually.", MessageType.Info);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ForcedControllerDisconect"), new GUIContent("Disconnect gamepads"), true);
					EditorGUILayout.EndVertical();
					
					break;
			}
			serializedObject.ApplyModifiedProperties();

			//DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
			}
		}

		bool CheckGamepadCode(int number)
		{
			if (script.GamepadCodes[number] == Helper.GamepadCodes._3rdAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._4thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._5thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._6thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._7thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._8thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._9thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._10thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._11thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._12thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._13thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._14thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._15thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._16thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._17thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._18thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._19thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes.XAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes.YAxis)
			{
				return true;
			}
			return false;
		}
	}
}
