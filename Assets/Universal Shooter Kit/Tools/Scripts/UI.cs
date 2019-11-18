using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	public class UI : ScriptableObject {

		public Text GrenadeAmmo;
		public Text WeaponAmmo;
		public Text Health;

		public Image pickUpIcon;
		public GameObject defaultCrosshair;

		public GameObject UIPrefab;
	}
}

