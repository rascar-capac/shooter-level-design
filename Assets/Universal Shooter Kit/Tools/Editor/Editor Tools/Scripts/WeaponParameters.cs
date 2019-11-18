using System.Collections.Generic;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	[CreateAssetMenu(fileName = "Weapon Parameters", menuName = "Weapon Preset")]
	public class WeaponParameters : ScriptableObject
	{
		public List<WeaponsHelper.Attack> Attacks = new List<WeaponsHelper.Attack>();
		public WeaponsHelper.GrenadeParameters GrenadeParameters;
		public WeaponsHelper.WeaponAnimation characterAnimations;
		
		public List<string> attacksNames = new List<string>();

		public Texture WeaponImage;
		public Texture AimImage;
		
		public AudioClip PickUpWeaponAudio;
		public AudioClip DropWeaponAudio;

		public bool ActiveAim;
		public int inspectorTab;
		public int currentAttack;
		public int bulletTypeInspectorTab;
	}
}
