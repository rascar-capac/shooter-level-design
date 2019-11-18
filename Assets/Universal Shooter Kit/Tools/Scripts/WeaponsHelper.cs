using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	public static class WeaponsHelper 
	{
		[Serializable]
		public class WeaponInfo
		{
			public Vector3 SaveTime;
			public Vector3 SaveDate;
			
			public bool HasTime;
			public bool disableIkInNormalState;
			public bool disableIkInCrouchState;

			public Vector3 WeaponSize;
            
			public Vector3 WeaponPosition;
			public Vector3 WeaponRotation;
            
			public Vector3 RightHandPosition;
			public Vector3 LeftHandPosition;
            
			public Vector3 RightHandRotation;
			public Vector3 LeftHandRotation;
			
			public Vector3 RightCrouchHandPosition;
			public Vector3 LeftCrouchHandPosition;
            
			public Vector3 RightCrouchHandRotation;
			public Vector3 LeftCrouchHandRotation;
            
			public Vector3 RightAimPosition;
			public Vector3 LeftAimPosition;
            
			public Vector3 RightAimRotation;
			public Vector3 LeftAimRotation;

			public Vector3 RightElbowPosition;
			public Vector3 LeftElbowPosition;

			public Vector3 RightHandWallPosition;
			public Vector3 LeftHandWallPosition;
            
			public Vector3 RightHandWallRotation;
			public Vector3 LeftHandWallRotation;

			public float FingersRightX;
			public float FingersLeftX;
            
			public float FingersRightY;
			public float FingersLeftY;
            
			public float FingersRightZ;
			public float FingersLeftZ;
            
			public float ThumbRightX;
			public float ThumbLeftX;
            
			public float ThumbRightY;
			public float ThumbLeftY;
            
			public float ThumbRightZ;
			public float ThumbLeftZ;

			public Vector3 CheckWallsColliderSize = Vector3.one;
			public Vector3 CheckWallsBoxPosition;
			public Vector3 CheckWallsBoxRotation;
			
			public float timeInHand_FPS = 2;
			public float timeBeforeCreating_FPS = 1;
			
			public float timeInHand_TPS = 2;
			public float timeBeforeCreating_TPS = 1;

			public void Clone(WeaponInfo CloneFrom)
			{
				disableIkInNormalState = CloneFrom.disableIkInNormalState;
				disableIkInCrouchState = CloneFrom.disableIkInCrouchState;
				
				HasTime = CloneFrom.HasTime;
				SaveTime = CloneFrom.SaveTime;
				SaveDate = CloneFrom.SaveDate;
				WeaponSize = CloneFrom.WeaponSize;
				WeaponPosition = CloneFrom.WeaponPosition;
				WeaponRotation = CloneFrom.WeaponRotation;

				RightHandPosition = CloneFrom.RightHandPosition;
				RightHandRotation = CloneFrom.RightHandRotation;
				LeftHandPosition = CloneFrom.LeftHandPosition;
				LeftHandRotation = CloneFrom.LeftHandRotation;
				
				RightCrouchHandPosition = CloneFrom.RightCrouchHandPosition;
				RightCrouchHandRotation = CloneFrom.RightCrouchHandRotation;
				LeftCrouchHandPosition = CloneFrom.LeftCrouchHandPosition;
				LeftCrouchHandRotation = CloneFrom.LeftCrouchHandRotation;

				RightAimPosition = CloneFrom.RightAimPosition;
				RightAimRotation = CloneFrom.RightAimRotation;
				LeftAimPosition = CloneFrom.LeftAimPosition;
				LeftAimRotation = CloneFrom.LeftAimRotation;

				RightHandWallPosition = CloneFrom.RightHandWallPosition;
				RightHandWallRotation = CloneFrom.RightHandWallRotation;
				LeftHandWallPosition = CloneFrom.LeftHandWallPosition;
				LeftHandWallRotation = CloneFrom.LeftHandWallRotation;

				RightElbowPosition = CloneFrom.RightElbowPosition;
				LeftElbowPosition = CloneFrom.LeftElbowPosition;

				FingersRightX = CloneFrom.FingersRightX;
				FingersRightY = CloneFrom.FingersRightY;
				FingersRightZ = CloneFrom.FingersRightZ;

				FingersLeftX = CloneFrom.FingersLeftX;
				FingersLeftY = CloneFrom.FingersLeftY;
				FingersLeftZ = CloneFrom.FingersLeftZ;

				ThumbRightX = CloneFrom.ThumbRightX;
				ThumbRightY = CloneFrom.ThumbRightY;
				ThumbRightZ = CloneFrom.ThumbRightZ;

				ThumbLeftX = CloneFrom.ThumbLeftX;
				ThumbLeftY = CloneFrom.ThumbLeftY;
				ThumbLeftZ = CloneFrom.ThumbLeftZ;

				CheckWallsColliderSize = CloneFrom.CheckWallsColliderSize;
				CheckWallsBoxPosition = CloneFrom.CheckWallsBoxPosition;
				CheckWallsBoxRotation = CloneFrom.CheckWallsBoxRotation;

				timeInHand_FPS = CloneFrom.timeInHand_FPS;
				timeBeforeCreating_FPS = CloneFrom.timeBeforeCreating_FPS;

				timeInHand_TPS = CloneFrom.timeInHand_TPS;
				timeBeforeCreating_TPS = CloneFrom.timeBeforeCreating_TPS;
			}
		}
		
		[Serializable]
		public class WeaponAnimation
		{
			public AnimationClip WeaponIdle;
//			public AnimationClip WeaponAttack;
//			public AnimationClip WeaponReload;
			public AnimationClip TakeWeapon;
			public AnimationClip WeaponWalk;
			public AnimationClip WeaponRun;
			public AnimationClip RemoveWeapon;

			public Animator anim;
		}

		[Serializable]
		public class BulletsSettings
		{
			public bool Active = true;
			[Range(1, 100)] public int weapon_damage;
			[Range(0, 10)] public float RateOfShoot = 0.5f;
			[Range(0, 0.1f)] public float ScatterOfBullets;
		}

		[Serializable]
		public class Attack
		{
			public TypeOfAttack AttackType = TypeOfAttack.Bullets;
			public List<BulletsSettings> BulletsSettings = new List<BulletsSettings>(2);
			
			public AnimationClip WeaponAttack;
			public AnimationClip WeaponAutoShoot;
			public AnimationClip WeaponReload;

			[Range(1, 100)] public int weapon_damage;
			[Range(0, 10)] public float RateOfShoot = 0.5f;
			[Range(0, 0.1f)] public float ScatterOfBullets;

			public int currentBulletType;
			public float curAmmo = 12;
			public float maxAmmo = 12;
			public float inventoryAmmo = 24;
			
			public GameObject Rocket;
			public GameObject Fire;
			public GameObject Tracer;
			public GameObject MuzzleFlash;
			public GameObject Shell;
			
			public Transform AttackSpawnPoint;
			public Transform ShellPoint;
			
			public BoxCollider AttackCollider;
			
			public AudioClip AttackAudio;
			public AudioClip ReloadAudio;
			public AudioClip NoAmmoShotAudio;
			
			public string AmmoType = "gun";
		}

		[Serializable]
		public class IKSlot
		{
			public int fpsSettingsSlot;
			public int tpsSettingsSlot;
			public int tpsCrouchSettingsSlot;
			public int tdsSettingsSlot;
			
			public int currentTag;
		}

		[Serializable]
		public class WeaponSlotInInventory
		{
			public GameObject weapon;
//			public int tpSlotIndex;
//			public int tdSlotIndex;
//			public int fpSlotIndex;
//			public List<string> saveSlotsNames;
		}

		[Serializable]
		public class GrenadeSlot
		{
			public GameObject Grenade;
			public int grenadeAmmo;
//			public int saveSlotIndex;
			public WeaponController GrenadeScript;
		}
		
		[Serializable]
		public class IKObjects
		{
			public Transform RightObject;
			public Transform LeftObject;
			
			public Transform RightAimObject;
			public Transform LeftAimObject;
			
			public Transform RightCrouchObject;
			public Transform LeftCrouchObject;

			public Transform RightWallObject;
			public Transform LeftWallObject;

			public Transform RightElbowObject;
			public Transform LeftElbowObject;
		}

		[Serializable]
		public class GrenadeParameters
		{
			[Range(1, 100)] public float GrenadeSpeed = 20;
			[Range(0.1f, 30)]public float GrenadeExplosionTime = 3;
			
			public bool ExplodeWhenTouchGround;
			
			public GameObject GrenadeExplosion;
			
			public AudioClip ThrowAudio;

			public AnimationClip GrenadeThrow_FPS;
			public AnimationClip GrenadeThrow_TPS_TDS;
		}
		
//		[Serializable]
//		public class GrenadeInfo
//		{
//			
//		}

		public enum IkDebugMode
		{
			Norm, Aim, Wall, Crouch
		}
		
		public enum TypeOfAttack
		{
			Bullets,
			Rockets,
			GrenadeLauncher,
			Flame,
			Knife,
			Grenade
		}
		

		public static bool CheckIKPosition(Vector3 position_1, Vector3 position_2, Vector3 rotation_1, Vector3 rotation_2)
		{
			return position_1 != Vector3.zero || rotation_1 != Vector3.zero || position_2 != Vector3.zero ||
			       rotation_2 != Vector3.zero;
		}

		public static bool CheckIKPosition(Vector3 position_1, Vector3 position_2)
		{
			return position_1 != Vector3.zero || position_2 != Vector3.zero;
		}

		public static void PlaceWeapon(WeaponInfo weaponInfo, Transform target)
		{
			if (weaponInfo.WeaponSize != Vector3.zero)
				target.localScale = weaponInfo.WeaponSize;

			target.localPosition = weaponInfo.WeaponPosition;
			target.localEulerAngles = weaponInfo.WeaponRotation;
		}
		
		public static void CheckIK(ref bool CanUseElbowIK, ref bool CanUseIK, ref bool CanUseAimIK, ref bool CanUseWallIK, ref bool CanUseCrouchIK, WeaponInfo weaponInfo)
		{
			CanUseElbowIK = CheckIKPosition(weaponInfo.LeftElbowPosition, weaponInfo.RightElbowPosition);
            
			CanUseIK = CheckIKPosition(weaponInfo.RightHandPosition, weaponInfo.LeftHandPosition, 
				weaponInfo.RightHandRotation, weaponInfo.LeftHandRotation);

			CanUseCrouchIK = CheckIKPosition(weaponInfo.RightCrouchHandPosition, weaponInfo.LeftCrouchHandPosition, weaponInfo.RightCrouchHandRotation,
				weaponInfo.LeftCrouchHandRotation);

			CanUseAimIK = CheckIKPosition(weaponInfo.RightAimPosition, weaponInfo.LeftAimPosition, 
				weaponInfo.RightAimRotation, weaponInfo.LeftAimRotation);
                
			CanUseWallIK = CheckIKPosition(weaponInfo.RightHandWallPosition, weaponInfo.LeftHandWallPosition, 
				weaponInfo.RightHandWallRotation, weaponInfo.LeftHandWallRotation);
		}

		public static void SetWeaponPositions(WeaponController weaponController, bool placeAll)
		{
			weaponController.canUseValuesInAdjustment = false;
			//CurrentWeaponInfo[SettingsSlotIndex].Clone(WeaponInfos[SettingsSlotIndex]);
			if (weaponController.Attacks[weaponController.currentAttack].AttackType != TypeOfAttack.Grenade)
			{
				CheckIK(ref weaponController.CanUseElbowIK, ref weaponController.CanUseIK, ref weaponController.CanUseAimIK, 
					ref weaponController.CanUseWallIK, ref weaponController.CanUseCrouchIK, weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex]);
				PlaceAllIKObjects(weaponController, weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex], placeAll);
			}
			PlaceWeapon(weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex], weaponController.transform);
			weaponController.ColliderToCheckWalls.localPosition = weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].CheckWallsBoxPosition;
			weaponController.ColliderToCheckWalls.localEulerAngles = weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].CheckWallsBoxRotation;
			weaponController.ColliderToCheckWalls.localScale = weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].CheckWallsColliderSize;
			weaponController.canUseValuesInAdjustment = true;
		}

		public static void PlaceAllIKObjects(WeaponController script, WeaponInfo weaponInfo, bool placeAll)
		{
			//ResetIKValues(script);
			
			var BodyObjects = new CharacterHelper.BodyObjects();

			BodyObjects = script.WeaponParent == Helper.Parent.Character ? script.Controller.BodyObjects : script.AIController.BodyObjects;

			if (!script.Controller.AdjustmentScene)
			{
				if (script.CanUseAimIK && script.IsAimEnabled)
				{
					PlaceIKObject(script.IkObjects.RightObject, weaponInfo.RightAimPosition, weaponInfo.RightAimRotation,
						BodyObjects.TopBody, BodyObjects.RightHand);

					PlaceIKObject(script.IkObjects.LeftObject, weaponInfo.LeftAimPosition, weaponInfo.LeftAimRotation,
						BodyObjects.TopBody, BodyObjects.LeftHand);
				}
				else if (script.CanUseWallIK && script.wallDetect)
				{
					PlaceIKObject(script.IkObjects.RightObject, weaponInfo.RightHandWallPosition, weaponInfo.RightHandWallRotation,
						BodyObjects.TopBody, BodyObjects.RightHand);

					PlaceIKObject(script.IkObjects.LeftObject, weaponInfo.LeftHandWallPosition, weaponInfo.LeftHandWallRotation,
						BodyObjects.TopBody, BodyObjects.LeftHand);
				}
				else if (script.CanUseCrouchIK && script.setCrouchHands)
				{
					PlaceIKObject(script.IkObjects.RightObject, weaponInfo.RightCrouchHandPosition, weaponInfo.RightCrouchHandRotation,
						BodyObjects.TopBody, BodyObjects.RightHand);

					PlaceIKObject(script.IkObjects.LeftObject, weaponInfo.LeftCrouchHandPosition, weaponInfo.LeftCrouchHandRotation,
						BodyObjects.TopBody, BodyObjects.LeftHand);
				}
				else
				{
					if (script.IsAimEnabled)
					{
						script.Controller.thisCameraScript.Aim();
						script._scatter = script.Attacks[script.currentAttack].ScatterOfBullets;
						script.IsAimEnabled = false;
					}

					PlaceIKObject(script.IkObjects.RightObject, weaponInfo.RightHandPosition,
						weaponInfo.RightHandRotation, BodyObjects.TopBody, BodyObjects.RightHand);

					PlaceIKObject(script.IkObjects.LeftObject, weaponInfo.LeftHandPosition,
						weaponInfo.LeftHandRotation, BodyObjects.TopBody, BodyObjects.LeftHand);
				}
			}
			else
			{
				if (script.DebugMode == IkDebugMode.Norm || placeAll)
				{
					PlaceIKObject(script.IkObjects.RightObject, weaponInfo.RightHandPosition,
						weaponInfo.RightHandRotation, BodyObjects.TopBody, BodyObjects.RightHand);

					PlaceIKObject(script.IkObjects.LeftObject, weaponInfo.LeftHandPosition,
						weaponInfo.LeftHandRotation, BodyObjects.TopBody, BodyObjects.LeftHand);
				}
			}

			if (script.DebugMode == IkDebugMode.Wall || placeAll)
			{
				PlaceIKObject(script.IkObjects.RightWallObject, weaponInfo.RightHandWallPosition,
					weaponInfo.RightHandWallRotation, BodyObjects.TopBody, BodyObjects.RightHand);
				PlaceIKObject(script.IkObjects.LeftWallObject, weaponInfo.LeftHandWallPosition,
					weaponInfo.LeftHandWallRotation, BodyObjects.TopBody, BodyObjects.LeftHand);
			}

			if (script.DebugMode == IkDebugMode.Aim || placeAll)
			{
				PlaceIKObject(script.IkObjects.RightAimObject, weaponInfo.RightAimPosition,
					weaponInfo.RightAimRotation, BodyObjects.TopBody, BodyObjects.RightHand);
				PlaceIKObject(script.IkObjects.LeftAimObject, weaponInfo.LeftAimPosition,
					weaponInfo.LeftAimRotation, BodyObjects.TopBody, BodyObjects.LeftHand);
			}

			if (script.DebugMode == IkDebugMode.Crouch || placeAll)
			{
				PlaceIKObject(script.IkObjects.RightCrouchObject, weaponInfo.RightCrouchHandPosition,
					weaponInfo.RightCrouchHandRotation, BodyObjects.TopBody, BodyObjects.RightHand);
				PlaceIKObject(script.IkObjects.LeftCrouchObject, weaponInfo.LeftCrouchHandPosition,
					weaponInfo.LeftCrouchHandRotation, BodyObjects.TopBody, BodyObjects.LeftHand);
			}

			PlaceIKObject(script.IkObjects.RightElbowObject, weaponInfo.RightElbowPosition, script.Controller.DirectionObject.position + script.Controller.DirectionObject.right * 2, BodyObjects.TopBody);
			PlaceIKObject(script.IkObjects.LeftElbowObject,  weaponInfo.LeftElbowPosition, script.Controller.DirectionObject.position - script.Controller.DirectionObject.right * 2, BodyObjects.TopBody);
		}

//		static void ResetIKValues(WeaponController script)
//		{
//			ResetIKValue(ref script.CurrentWeaponInfo.RightHandPosition, ref script.CurrentWeaponInfo.RightHandRotation,
//				script.IkObjects.RightObject, script.Controller.RightHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.LeftHandPosition, ref script.CurrentWeaponInfo.LeftHandRotation,
//				script.IkObjects.LeftObject, script.Controller.LeftHand, script.Controller.TopBody);
//			
//			ResetIKValue(ref script.CurrentWeaponInfo.RightAimPosition, ref script.CurrentWeaponInfo.RightAimRotation,
//				script.IkObjects.RightAimObject, script.Controller.RightHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.LeftAimPosition, ref script.CurrentWeaponInfo.LeftAimRotation,
//				script.IkObjects.LeftAimObject, script.Controller.LeftHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.RightHandWallPosition,
//				ref script.CurrentWeaponInfo.RightHandWallRotation,
//				script.IkObjects.RightWallObject, script.Controller.RightHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.LeftHandWallPosition,
//				ref script.CurrentWeaponInfo.LeftHandWallRotation,
//				script.IkObjects.LeftWallObject, script.Controller.LeftHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.RightElbowPosition, script.IkObjects.RightElbowObject,
//				script.Controller, script.Controller.transform.right);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.LeftElbowPosition, script.IkObjects.LeftElbowObject,
//				script.Controller, -script.Controller.transform.right);
//		}

//		static bool CheckIKValue(Vector3 value1, Vector3 value2, Transform obj, Transform parent)
//		{
//			if (value1 == Vector3.zero & value2 == Vector3.zero)
//			{
//				obj.parent = parent;
//				obj.localPosition = Vector3.zero;
//				obj.localRotation = Quaternion.Euler(-90, 0, 0);
//				//obj.parent = parent2;
//
//				return false;
////				value1 = obj.localPosition;
////				value2 = obj.localEulerAngles;
//			}
//
//			return true;
//		}

//		static bool CheckIKValue(Vector3 value1, Transform obj, Vector3 dir)
//		{
//			if (value1 == Vector3.zero)
//			{
//				obj.localPosition = dir * 2;
//				value1 = obj.localPosition;
//				return false;
//			}
//
//			return true;
//		}

		public static void SetHandsSettingsSlot(ref int SettingsSlotIndex, int tag, WeaponController weaponController, bool enable)
		{
			if (weaponController.IkSlots.Any(slot => slot.currentTag == tag))
			{
				var _slot = weaponController.IkSlots.Find(slot => slot.currentTag == tag);

				SettingsSlotIndex = enable ? _slot.fpsSettingsSlot : _slot.tpsSettingsSlot;
			}
		}

		public static void SetHandsSettingsSlot(ref int SettingsSlotIndex, int tag, CharacterHelper.CameraType type, WeaponController weaponController)
		{
			if (weaponController.IkSlots.Any(slot => slot.currentTag == tag))
			{
				var _slot = weaponController.IkSlots.Find(slot => slot.currentTag == tag);
				
				switch (type)
				{
					case CharacterHelper.CameraType.FirstPerson:
						SettingsSlotIndex = _slot.fpsSettingsSlot;
						break;
					case CharacterHelper.CameraType.ThirdPerson:
						SettingsSlotIndex = _slot.tpsSettingsSlot;;
						break;
					case CharacterHelper.CameraType.TopDown:
						SettingsSlotIndex = _slot.tdsSettingsSlot;
						break;
				}
			}
			else
			{
				switch (type)
				{
					case CharacterHelper.CameraType.FirstPerson:
						SettingsSlotIndex = weaponController.IkSlots[0].fpsSettingsSlot;
						break;
					case CharacterHelper.CameraType.ThirdPerson:
						SettingsSlotIndex = weaponController.IkSlots[0].tpsSettingsSlot;
						break;
					case CharacterHelper.CameraType.TopDown:
						SettingsSlotIndex = weaponController.IkSlots[0].tdsSettingsSlot;
						break;
				}
			}
		}

		static void PlaceIKObject(Transform obj, Vector3 pos, Vector3 dir, Transform parent)
		{
			if (pos == Vector3.zero)
			{
				obj.parent = parent;
				obj.localPosition = dir * 2;
			}
			else
			{
				obj.parent = parent;
				obj.localPosition = pos;
			}
		}
		public static void PlaceIKObject(Transform obj, Vector3 pos,  Vector3 rot, Transform parent, Transform parent2)
		{
			if (pos == Vector3.zero & rot == Vector3.zero)
			{
				obj.parent = parent2;
				obj.localPosition = Vector3.zero;
				obj.localEulerAngles = Vector3.zero;
			}
			else
			{
				obj.parent = parent;
				obj.localPosition = pos;
				obj.localEulerAngles = rot;
			}
		}
		
		public static void MovingIKObject(Transform RightObj, Transform LeftObj, Vector3 LeftTargetPos, Vector3 LeftTargetRot,
			Vector3 RightTargetPos, Vector3 RightTargetRot, string type)
		{
			int speed;
			
			speed = type == "crouch" ? 2 : 3;

			RightObj.localPosition = Vector3.MoveTowards(RightObj.localPosition,
				RightTargetPos, speed * Time.deltaTime);
			RightObj.localRotation = Quaternion.Slerp(RightObj.localRotation,
				Quaternion.Euler(RightTargetRot), 10 * Time.deltaTime);

			LeftObj.localPosition = Vector3.MoveTowards(LeftObj.localPosition,
				LeftTargetPos, speed * Time.deltaTime);
			LeftObj.localRotation = Quaternion.Slerp(LeftObj.localRotation,
				Quaternion.Euler(LeftTargetRot), 10 * Time.deltaTime);
		}
		
		public static void SmoothPositionChange(Transform obj, Vector3 targetPosition, Vector3 targetRotation, Transform parent)
		{
			obj.parent = parent;

			obj.localPosition = Vector3.MoveTowards(obj.localPosition, targetPosition, 20 * Time.deltaTime);
			obj.localRotation = Quaternion.Slerp(obj.localRotation, Quaternion.Euler(targetRotation), 20 * Time.deltaTime);
		}

		public static void SetWeaponController(GameObject instantiatedWeapon, GameObject originalWeapon, int saveSlot, InventoryManager manager, Controller controller, Transform parent)
		{
			var weaponController = instantiatedWeapon.GetComponent<WeaponController>();

			SetWeaponController(weaponController, instantiatedWeapon, originalWeapon, parent, controller.BodyObjects);

//			weaponController.tpsSettingsSlot = saveSlot;
			weaponController.WeaponParent = Helper.Parent.Character;
			weaponController.WeaponManager = manager;
			weaponController.Controller = controller;
			
			weaponController.enabled = true;
		}

		public static void SetWeaponController(GameObject instantiatedWeapon, GameObject originalWeapon, InventoryManager manager, Controller controller, Transform parent)
		{
			var weaponController = instantiatedWeapon.GetComponent<WeaponController>();
			
			SetWeaponController(weaponController, instantiatedWeapon, originalWeapon,parent, controller.BodyObjects);
			
			weaponController.WeaponParent = Helper.Parent.Character;
			weaponController.WeaponManager = manager;
			weaponController.Controller = controller;
			
			weaponController.enabled = true;
		}
		
		public static void SetWeaponController (GameObject instantiatedWeapon, GameObject originalWeapon, AIController controller, Transform parent)
		{
			var weaponController = instantiatedWeapon.GetComponent<WeaponController>();
			
			SetWeaponController(weaponController, instantiatedWeapon, originalWeapon,parent, controller.BodyObjects);
			
			weaponController.WeaponParent = Helper.Parent.Enemy;
			weaponController.AIController = controller;
			
			weaponController.enabled = true;
			
		}

		static void SetWeaponController(WeaponController weaponController, GameObject instantiatedWeapon, GameObject originalWeapon, Transform parent, CharacterHelper.BodyObjects objects)
		{
			weaponController = instantiatedWeapon.GetComponent<WeaponController>();
			weaponController.OriginalScript = originalWeapon.GetComponent<WeaponController>();
			weaponController.Parent = parent;

			foreach (var attack in weaponController.Attacks)
			{
				attack.curAmmo = attack.maxAmmo;
			}

			if (weaponController.Attacks[weaponController.currentAttack].AttackType != TypeOfAttack.Grenade)
			{
				if (objects.RightHand && instantiatedWeapon.transform.parent != objects.RightHand)
					instantiatedWeapon.transform.parent = objects.RightHand;
			}
			else
			{
				if (objects.LeftHand && instantiatedWeapon.transform.parent != objects.LeftHand)
					instantiatedWeapon.transform.parent = objects.LeftHand;
			}
		}
		
		public static void ChangeIKPosition(Vector3 leftPosition, Vector3 rightPosition, Vector3 leftRotation, Vector3 rightRotation, WeaponController weaponController)
		{
			if (weaponController.WeaponParent == Helper.Parent.Character)
			{
				weaponController.characterAnimations.anim.SetBool("CanWalkWithWeapon", false);
//				weaponController.characterAnimations.anim.CrossFade("Idle", 0, 2);
//				weaponController.characterAnimations.anim.CrossFade("Idle", 0, 3);
				weaponController.StartCoroutine("WalkWithWeaponTimeout");
			}

			if (weaponController.IkObjects.RightObject)
				SmoothPositionChange(weaponController.IkObjects.RightObject, rightPosition, rightRotation, weaponController.BodyObjects.TopBody);

			if (weaponController.IkObjects.LeftObject)
				SmoothPositionChange(weaponController.IkObjects.LeftObject, leftPosition, leftRotation, weaponController.BodyObjects.TopBody);
		}

	}
}

