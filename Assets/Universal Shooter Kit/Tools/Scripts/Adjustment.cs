using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	
	public class Adjustment : MonoBehaviour
	{
		
		public List<AIController> Enemies;
		public List<Controller> Characters;
		public List<WeaponController> Weapons;
		
		public List<GameObject> CharactersPrefabs;
		public List<GameObject> WeaponsPrefabs;
		public List<GameObject> EnemiesPrefabs;
		
		public List<GameObject> hideObjects;
		public List<string> WeaponsNames;
		public List<Vector3> currentScales;
		public List<CharacterHelper.CharacterOffset> CurrentCharacterOffsets;
		public List<CharacterHelper.CameraOffset> CurrentCameraOffsets;

		public GameObject Background;
		public GameObject Point;

		public Camera enemyCamera;
		public Camera menuCamera;

		public CharacterHelper.CameraType oldCameraType;

		public Controller currentController;
		public WeaponController CurrentWeaponController;
		public AIController CurrentAiController;

#if UNITY_EDITOR
		public SerializedObject SerializedWeaponController;
#endif
		
		public int copyFromSlot;
		public int copyFromWeaponSlot;
		public int enemyState;
		public int oldEnemyState;
		public int inspectorTab;
		public int oldInspectorTab;
		public int characterIndex = int.MaxValue;
		public int weaponIndex = int.MaxValue;
		public int enemyIndex = int.MaxValue;
		
		public float animationValue;
		public float takeGrenadeTime_FPS;
		public float throwGrenadeTime_FPS;

		[Range(-180,180)]public float dirObjRotX;
		[Range(-180,180)]public float dirObjRotY;
		[Range(-180,180)]public float dirObjRotZ;
		
		public float takeGrenadeTime_TPS;
		public float throwGrenadeTime_TPS;
		
		public bool isPause;

		public bool hide;
		public bool playGrenadeAnimation;
		public bool oldNormBoolValue;
		public bool oldCrouchBoolValue;

		public enum animType
		{
			FPS, TPS_TDS
		}

		public animType AnimType;

		public enum AdjustmentType
		{
			Enemy, Character
		}
		
		public AdjustmentType Type;
		
		public WeaponsHelper.IkDebugMode oldDebugModeIndex;

		public void Start()
		{
			weaponIndex = -1;
			CurrentWeaponController = null;

#if UNITY_EDITOR
			Selection.activeObject = gameObject;
#endif
			Background.SetActive(false);

			foreach (var weapon in WeaponsPrefabs)
			{
				Destroy(weapon);
			}
			WeaponsPrefabs.Clear();
			
			foreach (var enemy in EnemiesPrefabs)
			{
				Destroy(enemy);
			}
			EnemiesPrefabs.Clear();
			
			oldInspectorTab = inspectorTab;

			var scripts = new List<WeaponController>();
			
			for (var i = 0; i < Weapons.Count; i++)
			{
				if (Weapons[i])
				{
					WeaponsPrefabs.Add(Instantiate(Weapons[i].gameObject));
					currentScales.Add(Vector3.one);
					WeaponsNames.Add(Weapons[i].name);
					WeaponsPrefabs[WeaponsPrefabs.Count - 1].transform.position = Vector3.zero;
					var tempWeapon = Weapons[i];
					Weapons[i] = WeaponsPrefabs[WeaponsPrefabs.Count - 1].GetComponent<WeaponController>();
					currentScales[currentScales.Count - 1] = WeaponsPrefabs[WeaponsPrefabs.Count - 1].transform.localScale;
					Weapons[i].SettingsSlotIndex = 0;
					Weapons[i].gameObject.SetActive(false);
					WeaponsPrefabs[WeaponsPrefabs.Count - 1] = tempWeapon.gameObject;
				}
				else
				{
					scripts.Add(Weapons[i]);
				}
			}

			foreach (var script in scripts)
			{
				Weapons.Remove(script);
			}
			
			
			for (var i = 0; i < Enemies.Count; i++)
			{
				if (Enemies[i])
				{
					enemyIndex = 0;
					
					EnemiesPrefabs.Add(Instantiate(Enemies[i].gameObject));
					
					EnemiesPrefabs[i].transform.position = Vector3.zero;
					EnemiesPrefabs[i].transform.eulerAngles = new Vector3(0,180,0);
					var tempEnemy = Enemies[i];

					Enemies[i] = EnemiesPrefabs[i].GetComponent<AIController>();
					Enemies[i].gameObject.SetActive(false);
					Enemies[i].gameObject.hideFlags = HideFlags.HideInHierarchy;

					EnemiesPrefabs[i] = tempEnemy.gameObject;
					Enemies[i].OriginalScript = EnemiesPrefabs[i].GetComponent<AIController>();
				}
				else
				{
					
				}
			}
			
			var characterControllers = new List<Controller>();
			
			for (var i = 0; i < Characters.Count; i++)
			{
				if (Characters[i])
				{
					characterIndex = 0;
					
					CharactersPrefabs.Add(Instantiate(Characters[i].gameObject));
					
					CharactersPrefabs[i].transform.position = Vector3.zero;
					var tempCharacter = Characters[i];

					Characters[i] = CharactersPrefabs[i].GetComponent<Controller>();
					Characters[i].gameObject.SetActive(false);
					Characters[i].gameObject.hideFlags = HideFlags.HideInHierarchy;

					Characters[i].thisCamera.SetActive(false);
					Characters[i].thisCamera.hideFlags = HideFlags.HideInHierarchy;
					
					var curCharOffset = new CharacterHelper.CharacterOffset();
					curCharOffset.Clone(Characters[i].CharacterOffset);
					CurrentCharacterOffsets.Add(curCharOffset);

					var curCamOffset = new CharacterHelper.CameraOffset();
					curCamOffset.Clone(Characters[i].thisCameraScript.CameraOffset);
					CurrentCameraOffsets.Add(curCamOffset);
					
					if (Characters[i].gameObject.GetComponent<InventoryManager>().GrenadeAmmoUI)
						Characters[i].gameObject.GetComponent<InventoryManager>().GrenadeAmmoUI.gameObject.SetActive(false);

					if (Characters[i].gameObject.GetComponent<InventoryManager>().AmmoUI)
						Characters[i].gameObject.GetComponent<InventoryManager>().AmmoUI.gameObject.SetActive(false);

					CharactersPrefabs[i] = tempCharacter.gameObject;

					Characters[i].OriginalScript = CharactersPrefabs[i].GetComponent<Controller>();
					Characters[i].thisCameraScript.OriginalScript = CharactersPrefabs[i].GetComponent<Controller>().thisCamera.GetComponent<CameraController>();
				}
				else
				{
					characterControllers.Add(Characters[i]);
				}
			}
			
			foreach (var script in characterControllers)
			{
				Characters.Remove(script);
			}
			
			if (Characters.Count > 0)
			{
				ActiveCharacter(0, false);
				menuCamera.gameObject.SetActive(false);
				inspectorTab = 0;
			}
//			else if (Enemies.Count > 0)
//			{
//				ActiveEnemy(0);
//				menuCamera.gameObject.SetActive(false);
//				inspectorTab = 1;
//			}
			else
			{
				Debug.LogWarning("You should add any character to the Adjustment script");
#if UNITY_EDITOR
				EditorApplication.isPlaying = false;
#endif
			}
			
			Pause();
		}

		public void LateUpdate()
		{
			
#if UNITY_EDITOR
			ActiveEditorTracker.sharedTracker.isLocked = true;
#endif
			
			if (characterIndex != -1 && Characters.Count > 0 && inspectorTab == 0)
				currentController = Characters[characterIndex];

//			if (enemyIndex != -1 && Enemies.Count > 0 && inspectorTab == 1)
//			{
//				CurrentAiController = Enemies[enemyIndex];
//			}

//			if (inspectorTab == 1 && Enemies.Count > 0)
//				Type = AdjustmentType.Enemy;
//			else
			if (inspectorTab == 0 && Characters.Count > 0)
				Type = AdjustmentType.Character;

			if (currentController)
			{
				currentController.DebugMode = isPause;
				currentController.isPause = isPause;
				

				if (currentController.DebugMode)
				{
					var center = currentController.gameObject.GetComponent<CharacterController>().center;
					center = new Vector3(center.x, -currentController.CharacterOffset.CharacterHeight, center.z);
					currentController.gameObject.GetComponent<CharacterController>().center = center;
					currentController.defaultCharacterCenter = -currentController.CharacterOffset.CharacterHeight;

					currentController.DirectionObject.localEulerAngles = new Vector3(dirObjRotX, dirObjRotY, dirObjRotZ);
				}
			}
			
//			if (CurrentWeaponController && (inspectorTab == 1 || inspectorTab == 0))
//			{
//				if (CurrentWeaponController.ColliderToCheckWalls &&
//				    CurrentWeaponController.ColliderToCheckWalls.gameObject.hideFlags != HideFlags.HideInHierarchy)
//				{
//					CurrentWeaponController.ColliderToCheckWalls.gameObject.hideFlags = HideFlags.HideInHierarchy;
//					CurrentWeaponController.ColliderToCheckWalls.gameObject.SetActive(true);
//				}
//					
//				if (CurrentWeaponController.WeaponType != WeaponsHelper.TypeOfWeapon.Grenade)
//				{
//					Helper.HideAllObjects(CurrentWeaponController.IkObjects);
//				}
//			}

			if (oldInspectorTab != inspectorTab)
			{
//				if (CurrentWeaponController)
//				{
//					if (CurrentWeaponController.WeaponType != WeaponsHelper.TypeOfWeapon.Grenade)
//						Helper.HideAllObjects(CurrentWeaponController.IkObjects);
//				}
				if (inspectorTab == 0)// || inspectorTab == 1)
				{
					if (CurrentWeaponController && CurrentWeaponController.ActiveDebug)
					{
						if (CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
							Helper.HideAllObjects(CurrentWeaponController.IkObjects);
						else
						{
							//Characters[index].anim.CrossFade("Walk (without weapons)", 0.01f, 2);
						}
					}

					switch (inspectorTab)
					{
						case 0:
						{
							if (CurrentAiController)
							{
								CurrentAiController.gameObject.SetActive(false);
								CurrentAiController.gameObject.hideFlags = HideFlags.HideInHierarchy;
								//CurrentAiController = null;
							}
					
							//characterIndex = 0;
							ActiveCharacter(characterIndex, true);
							enemyCamera.gameObject.SetActive(false);
							break;
						}

						case 2://!! was 1//
						{
							if (currentController)
							{
								currentController.gameObject.SetActive(false);
								currentController.thisCamera.SetActive(false);
								
								currentController.gameObject.hideFlags = HideFlags.HideInHierarchy;
								currentController.thisCamera.hideFlags = HideFlags.HideInHierarchy;
							}

							//enemyIndex = 0;
							ActiveEnemy(enemyIndex);
							enemyCamera.gameObject.SetActive(true);
							break;
						}
					}
				}

				oldInspectorTab = inspectorTab;
			}
			
			
			if (CurrentWeaponController)
			{
				if (CurrentWeaponController.canUseValuesInAdjustment)
				{
					var curInfo = CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex];
					if (CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
					{
						curInfo.WeaponSize = CurrentWeaponController.transform.localScale;
						curInfo.WeaponPosition = CurrentWeaponController.transform.localPosition;
						curInfo.WeaponRotation = CurrentWeaponController.transform.localEulerAngles;

						curInfo.RightHandPosition = CurrentWeaponController.IkObjects.RightObject.localPosition;
						curInfo.RightHandRotation = CurrentWeaponController.IkObjects.RightObject.localEulerAngles;

						curInfo.LeftHandPosition = CurrentWeaponController.IkObjects.LeftObject.localPosition;
						curInfo.LeftHandRotation = CurrentWeaponController.IkObjects.LeftObject.localEulerAngles;

						if (CurrentWeaponController.hasAimIKChanged)
						{
							curInfo.RightAimPosition = CurrentWeaponController.IkObjects.RightAimObject.localPosition;
							curInfo.RightAimRotation = CurrentWeaponController.IkObjects.RightAimObject.localEulerAngles;

							curInfo.LeftAimPosition = CurrentWeaponController.IkObjects.LeftAimObject.localPosition;
							curInfo.LeftAimRotation = CurrentWeaponController.IkObjects.LeftAimObject.localEulerAngles;
						}

						if (CurrentWeaponController.hasWallIKChanged)
						{
							curInfo.RightHandWallPosition = CurrentWeaponController.IkObjects.RightWallObject.localPosition;
							curInfo.RightHandWallRotation = CurrentWeaponController.IkObjects.RightWallObject.localEulerAngles;

							curInfo.LeftHandWallPosition = CurrentWeaponController.IkObjects.LeftWallObject.localPosition;
							curInfo.LeftHandWallRotation = CurrentWeaponController.IkObjects.LeftWallObject.localEulerAngles;
						}

						if (CurrentWeaponController.hasCrouchIKChanged)
						{
							curInfo.RightCrouchHandPosition = CurrentWeaponController.IkObjects.RightCrouchObject.localPosition;
							curInfo.RightCrouchHandRotation = CurrentWeaponController.IkObjects.RightCrouchObject.localEulerAngles;

							curInfo.LeftCrouchHandPosition = CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition;
							curInfo.LeftCrouchHandRotation = CurrentWeaponController.IkObjects.LeftCrouchObject.localEulerAngles;
						}

						curInfo.LeftElbowPosition =
							CurrentWeaponController.IkObjects.LeftElbowObject.localPosition;
						curInfo.RightElbowPosition =
							CurrentWeaponController.IkObjects.RightElbowObject.localPosition;

//								CurrentWeaponController.ColliderToCheckWalls.parent = currentController.BodyObjects.RightHand;
						curInfo.CheckWallsBoxPosition = CurrentWeaponController.ColliderToCheckWalls.localPosition;

						curInfo.CheckWallsBoxRotation = CurrentWeaponController.ColliderToCheckWalls.localEulerAngles;

						if(CurrentWeaponController.ColliderToCheckWalls.localScale != Vector3.zero)
							curInfo.CheckWallsColliderSize = CurrentWeaponController.ColliderToCheckWalls.localScale;
						else CurrentWeaponController.ColliderToCheckWalls.localScale = Vector3.one;
					}
					else
					{

						var transform = CurrentWeaponController.transform;

						curInfo.WeaponPosition = transform.localPosition;
						curInfo.WeaponRotation = transform.localEulerAngles;
						curInfo.WeaponSize = transform.localScale;
						
						if (AnimType == animType.FPS)
						{
							curInfo.timeBeforeCreating_FPS = takeGrenadeTime_FPS;
							curInfo.timeInHand_FPS = throwGrenadeTime_FPS - takeGrenadeTime_FPS;
						}
						else
						{
							curInfo.timeBeforeCreating_TPS = takeGrenadeTime_TPS;
							curInfo.timeInHand_TPS = throwGrenadeTime_TPS - takeGrenadeTime_TPS;
						}
					}
				}

				if(!(Type == AdjustmentType.Enemy && CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade)))
					CurrentWeaponController.ActiveDebug = isPause && inspectorTab == 1;

				if (CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
				{
					if (oldNormBoolValue != CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInNormalState)
					{
						CheckIKObjects();
						oldNormBoolValue = CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInNormalState;
					}

					if (oldCrouchBoolValue != CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInCrouchState)
					{
						CheckIKObjects();
						WeaponsHelper.CheckIK(ref CurrentWeaponController.CanUseElbowIK, ref CurrentWeaponController.CanUseIK, ref CurrentWeaponController.CanUseAimIK, 
							ref CurrentWeaponController.CanUseWallIK, ref CurrentWeaponController.CanUseCrouchIK, CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex]);
						
						if(!CurrentWeaponController.CanUseCrouchIK)
							WeaponsHelper.PlaceAllIKObjects(CurrentWeaponController, CurrentWeaponController.WeaponInfos[CurrentWeaponController.SettingsSlotIndex], false);
						
						oldCrouchBoolValue = CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInCrouchState;
					}

					if (CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Aim && currentController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
					    Point.SetActive(true);
					else Point.SetActive(false);
					
					if (oldDebugModeIndex != CurrentWeaponController.DebugMode)
					{
						CheckIKObjects();

						if (CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Crouch && !currentController.isCrouch && currentController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
						{
							currentController.ActivateCrouch();
						}
						else if (CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Crouch && currentController.isCrouch)
						{
							currentController.DeactivateCrouch();
						}
						
						if((oldDebugModeIndex == WeaponsHelper.IkDebugMode.Norm && CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInNormalState ||
						    oldDebugModeIndex == WeaponsHelper.IkDebugMode.Crouch && CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInCrouchState) &&
						   (CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Aim && !CurrentWeaponController.CanUseAimIK || 
						    CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Wall && !CurrentWeaponController.CanUseWallIK) ||
						   CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Crouch && !CurrentWeaponController.CanUseCrouchIK) 
							WeaponsHelper.PlaceAllIKObjects(CurrentWeaponController, CurrentWeaponController.WeaponInfos[CurrentWeaponController.SettingsSlotIndex], false);

						oldDebugModeIndex = CurrentWeaponController.DebugMode;
					}

					if (CurrentWeaponController.ColliderToCheckWalls)
					{
						if (CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Wall)
						{
							if (CurrentWeaponController.ColliderToCheckWalls.gameObject.hideFlags != HideFlags.None)
							{
								CurrentWeaponController.ColliderToCheckWalls.gameObject.hideFlags = HideFlags.None;
								CurrentWeaponController.ColliderToCheckWalls.gameObject.SetActive(true);
							}
						}
						else
						{
							if (CurrentWeaponController.ColliderToCheckWalls.gameObject.hideFlags != HideFlags.HideInHierarchy)
							{
								CurrentWeaponController.ColliderToCheckWalls.gameObject.hideFlags = HideFlags.HideInHierarchy;
								CurrentWeaponController.ColliderToCheckWalls.gameObject.SetActive(false);
							}
						}
					}
				}
				else
				{
					if (Type != AdjustmentType.Enemy)
					{
						if (AnimType == animType.FPS)
						{
							if (!playGrenadeAnimation)
							{
								currentController.anim.speed = 0;
								currentController.anim.Play("Grenade Throw", 2, animationValue / CurrentWeaponController.GrenadeParameters.GrenadeThrow_FPS.length);
							}
							else
							{
								currentController.anim.speed = 1;

								if (animationValue > CurrentWeaponController.GrenadeParameters.GrenadeThrow_TPS_TDS.length)
								{
									playGrenadeAnimation = false;
									animationValue = 0;
								}
								else
								{
									currentController.anim.Play("Grenade Throw", 2);
									animationValue = currentController.anim.GetCurrentAnimatorStateInfo(2).normalizedTime *
									                 CurrentWeaponController.GrenadeParameters.GrenadeThrow_FPS.length;
								}
							}
						}
						else
						{
							if (!playGrenadeAnimation)
							{
								currentController.anim.speed = 0;
								currentController.anim.Play("Grenade_Throw", 1, animationValue / CurrentWeaponController.GrenadeParameters.GrenadeThrow_TPS_TDS.length);
							}
							else
							{
								currentController.anim.speed = 1;

								if (animationValue > CurrentWeaponController.GrenadeParameters.GrenadeThrow_TPS_TDS.length)
								{
									playGrenadeAnimation = false;
									animationValue = 0;
								}
								else
								{
									currentController.anim.Play("Grenade_Throw", 1);
									
									animationValue = currentController.anim.GetCurrentAnimatorStateInfo(1).normalizedTime *
									                 CurrentWeaponController.GrenadeParameters.GrenadeThrow_TPS_TDS.length;
								}
							}
						}

						if (AnimType == animType.FPS)
							CurrentWeaponController.gameObject.SetActive(animationValue >= takeGrenadeTime_FPS && animationValue < throwGrenadeTime_FPS);
						else
							CurrentWeaponController.gameObject.SetActive(animationValue >= takeGrenadeTime_TPS && animationValue < throwGrenadeTime_TPS);
					}

//					else
//					{
//						CurrentWeaponController.gameObject.SetActive(true);
//						currentController.anim.speed = 0;
//						currentController.anim.Play("Walk (without weapons)");
//					}
				}
			}


			if (isPause && !Cursor.visible)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			
			for (var i = 0; i < Weapons.Count; i++)
			{
				if (weaponIndex != i && Weapons[i].gameObject.hideFlags != HideFlags.HideInHierarchy)
				{
					Weapons[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
					Weapons[i].gameObject.SetActive(false);
				}
				else if (weaponIndex == i && Weapons[i].gameObject.hideFlags == HideFlags.HideInHierarchy)
				{
					if (Type == AdjustmentType.Enemy && Weapons[i].Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade)||
					    Type == AdjustmentType.Character && Weapons[i].Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade) ||
					    Weapons[i].Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
					{
						Weapons[i].gameObject.hideFlags = HideFlags.None;
						Weapons[i].gameObject.SetActive(true);
					}
				}
			}
			
//			for (var i = 0; i < Characters.Count; i++)
//			{
//				if (characterIndex != i && Characters[i].gameObject.hideFlags != HideFlags.HideInHierarchy)
//				{
//					Characters[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
//					
//					if (Characters[i].thisCameraScript && Characters[i].thisCameraScript.CameraPos)
//						Characters[i].thisCameraScript.CameraPos.hideFlags = HideFlags.HideInHierarchy;
//					
//					Characters[i].gameObject.SetActive(false);
//					Characters[i].thisCamera.SetActive(false);
//				}
//				else if (characterIndex == i && Characters[i].gameObject.hideFlags == HideFlags.HideInHierarchy)
//				{
//					Characters[i].gameObject.hideFlags = HideFlags.None;
//					
//					if (Characters[i].thisCameraScript && Characters[i].thisCameraScript.CameraPos)
//						Characters[i].thisCameraScript.CameraPos.hideFlags = HideFlags.None;
//					
//					Characters[i].thisCamera.SetActive(true);
//					Characters[i].gameObject.SetActive(true);
//				}
//			}
			
//			if (Input.GetKeyDown(KeyCode.Escape))
//			{
//				Pause();
//			}
		}

		public void CheckIKObjects()
		{
			if (!CurrentWeaponController.ActiveDebug || inspectorTab == 0) return;// || inspectorTab == 1) return;
			
			switch (CurrentWeaponController.DebugMode)
			{
				case WeaponsHelper.IkDebugMode.Aim:
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightWallObject, Color.yellow);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftWallObject, Color.yellow);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightObject, Color.red);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftObject, Color.red);

					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.LeftAimObject, Color.blue);
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.RightAimObject, Color.blue);
					
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightCrouchObject, Color.magenta);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftCrouchObject, Color.magenta);
					
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.RightElbowObject, Color.green);
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.LeftElbowObject, Color.green);
					
					CurrentWeaponController.ActiveAimTPS = true;
					CurrentWeaponController.ActiveAimFPS = true;
					break;
				case WeaponsHelper.IkDebugMode.Wall:
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.RightWallObject, Color.yellow);
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.LeftWallObject, Color.yellow);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightObject, Color.red);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftObject, Color.red);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightAimObject, Color.blue);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftAimObject, Color.blue);
					
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightCrouchObject, Color.magenta);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftCrouchObject, Color.magenta);
					
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.RightElbowObject, Color.green);
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.LeftElbowObject, Color.green);
					break;
				case WeaponsHelper.IkDebugMode.Norm:
					
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInNormalState, HideFlags.None, CurrentWeaponController.IkObjects.RightObject, Color.red);
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInNormalState, HideFlags.None, CurrentWeaponController.IkObjects.LeftObject, Color.red);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightAimObject, Color.blue);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftAimObject, Color.blue);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightWallObject, Color.yellow);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftWallObject, Color.yellow);
					
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightCrouchObject, Color.magenta);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftCrouchObject, Color.magenta);
					
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInNormalState, HideFlags.None, CurrentWeaponController.IkObjects.RightElbowObject, Color.green);
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInNormalState, HideFlags.None, CurrentWeaponController.IkObjects.LeftElbowObject, Color.green);
					break;
				case WeaponsHelper.IkDebugMode.Crouch:
					
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightObject, Color.red);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftObject, Color.red);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightAimObject, Color.blue);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftAimObject, Color.blue);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightWallObject, Color.yellow);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftWallObject, Color.yellow);
					
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInCrouchState, HideFlags.None, CurrentWeaponController.IkObjects.RightCrouchObject, Color.magenta);
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInCrouchState, HideFlags.None, CurrentWeaponController.IkObjects.LeftCrouchObject, Color.magenta);
					
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInCrouchState, HideFlags.None, CurrentWeaponController.IkObjects.RightElbowObject, Color.green);
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInCrouchState, HideFlags.None, CurrentWeaponController.IkObjects.LeftElbowObject, Color.green);
					break;
				
			}
		}

		void Pause()
		{
			isPause = !isPause;

			if (!isPause && CurrentWeaponController && CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
			{
				Helper.HideAllObjects(CurrentWeaponController.IkObjects);
			}

			if (CurrentWeaponController && CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
			{
				CurrentWeaponController.gameObject.SetActive(isPause);
				CurrentWeaponController.WeaponManager.grenadeController = isPause ? CurrentWeaponController : null;
				currentController.anim.CrossFade("Walk (without weapons)", 0.01f, 2);
			}
			
			Background.SetActive(isPause);
			
			Cursor.visible = isPause;
			Cursor.lockState = isPause ? CursorLockMode.None : CursorLockMode.Locked;

			if (currentController)
			{
				currentController.thisCameraScript.cameraDebug = isPause;
				currentController.isPause = isPause;
				
//				if (currentController.thisCameraScript && currentController.thisCameraScript.crosshair)
//					currentController.thisCameraScript.crosshair.gameObject.SetActive(!isPause);
			}
		}

		public void ActiveEnemy(int index)
		{
			Enemies[enemyIndex].gameObject.SetActive(false);

			Enemies[enemyIndex].gameObject.hideFlags = HideFlags.HideInHierarchy;
			
			if(Enemies[index].anim && Enemies[index].anim.GetLayerWeight(1) > 0)
				Enemies[index].anim.SetLayerWeight(1, 0); //smooth change here
			
			Enemies[index].gameObject.SetActive(true);

			Enemies[index].gameObject.hideFlags = HideFlags.None;
			
			if (currentController)
				currentController = null;
			
			
			ResetWeapons(false);
			
			//CurrentWeaponController = null;
			weaponIndex = -1;
		}

		public void ActiveCharacter(int index, bool changeInspectorTab)
		{
//			if (!changeInspectorTab)
//			{
//				Characters[characterIndex].gameObject.SetActive(false);
//				Characters[characterIndex].thisCamera.SetActive(false);
//				
//
//				
//				
//				//Characters[index].anim.CrossFade("Walk (without weapons)", 0.01f, 2);
//			}
			
			Characters[characterIndex].gameObject.hideFlags = HideFlags.HideInHierarchy;
			
			if(Characters[index].thisCameraScript.CameraPos)
				Characters[index].thisCameraScript.CameraPos.hideFlags = HideFlags.HideInHierarchy;
			else Characters[index].thisCamera.hideFlags = HideFlags.HideInHierarchy;
			
			Characters[characterIndex].gameObject.SetActive(false);
			Characters[characterIndex].thisCamera.SetActive(false);
			Characters[characterIndex].ActiveCharacter = false;
			
			oldCameraType = Characters[index].TypeOfCamera;
			
			Characters[index].gameObject.SetActive(true);
			Characters[index].thisCamera.SetActive(true);
			Characters[index].ActiveCharacter = true;
			
			Characters[index].gameObject.hideFlags = HideFlags.None;
			
			if(Characters[index].thisCameraScript.CameraPos)
				Characters[index].thisCameraScript.CameraPos.hideFlags = HideFlags.None;
			else Characters[index].thisCamera.hideFlags = HideFlags.None;

			if (CurrentAiController)
				CurrentAiController = null;

			Characters[index].anim.SetBool("NoWeapons", true);

			Characters[index].thisCameraScript.SetAnimVariables();
			
			dirObjRotX = Characters[index].CharacterOffset.directionObjRotation.x;
			dirObjRotY = Characters[index].CharacterOffset.directionObjRotation.y;
			dirObjRotZ = Characters[index].CharacterOffset.directionObjRotation.z;
			
			ResetWeapons(true);

			//CurrentWeaponController = null;
			weaponIndex = -1;
		}

		public void ResetWeapons(bool isCharacter)
		{
			if (isCharacter)
			{
				foreach (var character in Characters)
				{
					var manager = character.gameObject.GetComponent<InventoryManager>();
					manager.slots[0].weaponsInInventory.Clear();
					manager.weaponController = null;
					manager.hasAnyWeapon = false;

					if (!manager.gun) continue;
					manager.gun.transform.parent = null;
					manager.gun = null;
				}
			}
			else
			{
//				foreach (var enemy in Enemies)
//				{
//					enemy.Weapons.Clear();
//					enemy.weaponController = null;
//					enemy.hasAnyWeapon = false;
//				}
			}
		}

#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			if (Application.isPlaying && currentController && currentController.characterCollider && inspectorTab == 0)
			{
				Handles.zTest = CompareFunction.Less;
				Handles.color = new Color32(255, 150, 0, 255);
				Helper.DrawWireCapsule(currentController.transform.position + currentController.characterCollider.center, transform.rotation,
					currentController.characterCollider.radius, currentController.characterCollider.height, Handles.color);


				if (currentController.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
				{
					Handles.color = new Color32(0, 0, 255, 255);
					Handles.ArrowHandleCap(0, currentController.DirectionObject.position, Quaternion.LookRotation(currentController.DirectionObject.forward), 1,
						EventType.Repaint);

					Handles.color = new Color32(255, 0, 0, 255);
					Handles.ArrowHandleCap(0, currentController.DirectionObject.position, Quaternion.LookRotation(currentController.DirectionObject.right), 1,
						EventType.Repaint);
				}
				else
				{
					Handles.color = new Color32(255, 255, 0, 255);
					Handles.ArrowHandleCap(0, currentController.thisCameraScript.CameraPosition.position, currentController.thisCameraScript.CameraPosition.rotation,
						1,
						EventType.Repaint);
				}

				Handles.zTest = CompareFunction.Greater;
				Handles.color = new Color32(255, 150, 0, 50);
				Helper.DrawWireCapsule(currentController.transform.position + currentController.characterCollider.center, transform.rotation,
					currentController.characterCollider.radius, currentController.characterCollider.height, Handles.color);

				if (currentController.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
				{
					Handles.color = new Color32(0, 0, 255, 50);
					Handles.ArrowHandleCap(0, currentController.DirectionObject.position, Quaternion.LookRotation(currentController.DirectionObject.forward), 1,
						EventType.Repaint);

					Handles.color = new Color32(255, 0, 0, 50);
					Handles.ArrowHandleCap(0, currentController.DirectionObject.position, Quaternion.LookRotation(currentController.DirectionObject.right), 1,
						EventType.Repaint);
				}
				else
				{
					Handles.color = new Color32(255, 255, 0, 50);
					Handles.ArrowHandleCap(0, currentController.thisCameraScript.CameraPosition.position, currentController.thisCameraScript.CameraPosition.rotation,
						1,
						EventType.Repaint);
				}
			}


			if (!Application.isPlaying || !CurrentWeaponController || CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Wall ||
			    !CurrentWeaponController.ColliderToCheckWalls) return;
			
			Handles.matrix = CurrentWeaponController.ColliderToCheckWalls.localToWorldMatrix;

			if (Selection.activeGameObject == CurrentWeaponController.ColliderToCheckWalls.gameObject)
			{
				Handles.zTest = CompareFunction.Greater;
				Handles.color = new Color32(255, 255, 0, 50);
				Handles.DrawWireCube(Vector3.zero, CurrentWeaponController.ColliderToCheckWalls.localScale);

				Handles.zTest = CompareFunction.Less;
				Handles.color = new Color32(255, 255, 0, 255);
				Handles.DrawWireCube(Vector3.zero, CurrentWeaponController.ColliderToCheckWalls.localScale);
			}
			else
			{
				Handles.zTest = CompareFunction.Greater;
				Handles.color = new Color32(255, 100, 0, 30);
				Handles.DrawWireCube(Vector3.zero, CurrentWeaponController.ColliderToCheckWalls.localScale);

				Handles.zTest = CompareFunction.Less;
				Handles.color = new Color32(255, 100, 0, 150);
				Handles.DrawWireCube(Vector3.zero, CurrentWeaponController.ColliderToCheckWalls.localScale);
			}
		}
#endif
	}

}
