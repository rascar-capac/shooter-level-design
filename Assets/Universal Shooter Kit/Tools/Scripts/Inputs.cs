using System.Collections.Generic;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	
	public class Inputs : ScriptableObject
	{
		public Helper.GamepadAxes[] GamepadAxes = new Helper.GamepadAxes[5];
		public Helper.GamepadCodes[] GamepadCodes = new Helper.GamepadCodes[18];
		public Helper.AxisButtonValue[] AxisButtonValues = new Helper.AxisButtonValue[18];
		public Helper.KeyBoardCodes[] KeyBoardCodes = new Helper.KeyBoardCodes[20];

		public bool PressSprintButton;
		public bool PressCrouchButton;
		public bool pressInventoryButton;
		public bool ForcedControllerDisconect;
		
//		[SerializeField]
		public GameObject[] uiButtons = new GameObject[18];
		
		public GameObject MobileInputs;
		
		public List<string> CharacterTags = new List<string>{"Character"};

		public enum Stick
		{
			MovementStick,
			CameraStick
		}

		public Stick _Stick;
		
		public bool[] invertAxes = new bool[5];

		public int tab;
		[Range(100, 1000)]
		public int StickRange;
	}
}
