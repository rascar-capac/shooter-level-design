using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(Adjustment))]
	public class AdjustmentEditor : Editor
	{
		private Adjustment script;

		private ReorderableList weaponsList;
//		private ReorderableList enemiesList;
		private ReorderableList charactersList;

		private string curName;

		private bool delete;
		private bool rename;
		private bool renameError;
		private int stateIndex;

		private enum FingersRotationAxises
		{
			X, Y, Z
		}
		
		private FingersRotationAxises axises;

		private void Awake()
		{
			script = (Adjustment) target;
		}

		private void OnEnable()
		{
			/*enemiesList = new ReorderableList(serializedObject, serializedObject.FindProperty("Enemies"), false, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						Application.isPlaying ? "Select an enemy to adjust" : "Add your enemies");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Status");
				},
				
				onAddCallback = items =>
				{
					if (!Application.isPlaying)
					{
						script.Enemies.Add(null);
						script.EnemiesPrefabs.Add(null);
					}
				},
				
				onRemoveCallback = items =>
				{
					if (!Application.isPlaying)
					{
						script.Enemies.Remove(script.Enemies[items.index]);
						script.EnemiesPrefabs.Remove((script.EnemiesPrefabs[items.index]));
					}
				},
				
				onSelectCallback = items =>
				{
					if (!script.Enemies[items.index])
						return;
					
					if (Application.isPlaying && script.enemyIndex != items.index)
					{
						script.ActiveEnemy(items.index);
						script.enemyIndex = items.index;
					}
					else if (!Application.isPlaying)
					{
						script.enemyIndex = items.index;
						
						if (!script.EnemiesPrefabs[items.index])
						{
							script.EnemiesPrefabs[items.index] = Instantiate(script.Enemies[items.index].gameObject, Vector3.zero, Quaternion.identity);
							script.EnemiesPrefabs[items.index].GetComponent<AIController>().OriginalScript = script.Enemies[items.index];
						}
					}

					script.CurrentAiController = script.Enemies[script.enemyIndex];
				},
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					if (Application.isPlaying && index == script.enemyIndex && script.Enemies[index] && script.isPause)
					{
						var options = new GUIStyle {normal = {textColor = Color.green}};
						EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10,
							EditorGUIUtility.singleLineHeight), "Adjustment", options);
					}

					script.Enemies[index] = (AIController) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 1.5f, 
							EditorGUIUtility.singleLineHeight), script.Enemies[index], typeof(AIController), false);
					
					
					if (!Application.isPlaying)
					{
						var enemy = script.EnemiesPrefabs[index];

						if (enemy)
						{
							if (!isActive && enemy.hideFlags == HideFlags.None)
							{
								enemy.hideFlags = HideFlags.HideInHierarchy;
								enemy.SetActive(false);
								EditorApplication.RepaintHierarchyWindow();
								EditorApplication.DirtyHierarchyWindowSorting();
							}
							else if (isActive && enemy.hideFlags == HideFlags.HideInHierarchy)
							{
								enemy.hideFlags = HideFlags.None;
								enemy.SetActive(true);
							}
						}
					}

					if (!Application.isPlaying && index == script.enemyIndex && script.CurrentAiController && isActive)
					{
						var currentController = script.EnemiesPrefabs[script.enemyIndex].GetComponent<AIController>();
						EditorGUILayout.BeginVertical("box");
						EditorGUILayout.BeginVertical("box");
						currentController.DistanceToSee = EditorGUILayout.Slider("Distance to see", currentController.DistanceToSee, 1, 100);
						script.CurrentAiController.DistanceToSee = currentController.DistanceToSee;
						
						currentController.horizontalAngleToSee = EditorGUILayout.Slider("Horizontal angle to see", currentController.horizontalAngleToSee, 1, 180);
						script.CurrentAiController.horizontalAngleToSee = currentController.horizontalAngleToSee;
						
						currentController.heightToSee = EditorGUILayout.Slider("Height to see", currentController.heightToSee, 1, 180);
						script.CurrentAiController.heightToSee = currentController.heightToSee;
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						EditorGUILayout.BeginVertical("box");
						currentController.HeadOffsetX = EditorGUILayout.Slider("Head offset X", currentController.HeadOffsetX, -90, 90);
						script.CurrentAiController.HeadOffsetX = currentController.HeadOffsetX;
						
						currentController.HeadOffsetY = EditorGUILayout.Slider("Head offset Y", currentController.HeadOffsetY, -90, 90);
						script.CurrentAiController.HeadOffsetY = currentController.HeadOffsetY;
						
						currentController.HeadOffsetZ = EditorGUILayout.Slider("Head offset Z", currentController.HeadOffsetZ, -90, 90);
						script.CurrentAiController.HeadOffsetZ = currentController.HeadOffsetZ;
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndVertical();
					}

				}
			};*/

			charactersList = new ReorderableList(serializedObject, serializedObject.FindProperty("Characters"), true, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						Application.isPlaying ? "Select a character to adjust" : "Add your characters");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Status");
				},

				onAddCallback = items =>
				{
					if (!Application.isPlaying) script.Characters.Add(null);
				},

				onRemoveCallback = items =>
				{
					if (!Application.isPlaying) script.Characters.Remove(script.Characters[items.index]);
				},

				onSelectCallback = items =>
				{
					if (!script.Characters[items.index])
						return;

					if (Application.isPlaying && script.characterIndex != items.index)
					{
						script.ActiveCharacter(items.index, false);

						script.characterIndex = items.index;
					}
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					if (Application.isPlaying && index == script.characterIndex && script.Characters[index] && script.isPause)
					{
						var options = new GUIStyle {normal = {textColor = Color.green}};
						EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10,
							EditorGUIUtility.singleLineHeight), "Adjustment", options);
					}

					script.Characters[index] = (Controller) EditorGUI.ObjectField(
						new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						script.Characters[index], typeof(Controller), false);
				}
			};

			weaponsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Weapons"), false, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						Application.isPlaying ? "Select a weapon to adjust" : "Add your weapons");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Status");
				},

				onAddCallback = items =>
				{
					if (!Application.isPlaying)
					{
						script.Weapons.Add(null);
						script.WeaponsPrefabs.Add(null);
					}
				},

				onRemoveCallback = items =>
				{
					if (!Application.isPlaying)
					{
						script.Weapons.Remove(script.Weapons[items.index]);
						script.WeaponsPrefabs.Remove(script.WeaponsPrefabs[items.index]);
					}
				},

				onSelectCallback = items =>
				{
					if (!script.Weapons[items.index] || script.weaponIndex == items.index)
						return;

					if (Application.isPlaying)
					{
						switch (script.Type)
						{
							case Adjustment.AdjustmentType.Enemy:
								if (script.Weapons[items.index].Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
								{
									script.ResetWeapons(false);

									WeaponsHelper.SetWeaponController(script.Weapons[items.index].gameObject, script.WeaponsPrefabs[items.index],
										script.CurrentAiController, script.CurrentAiController.transform);

									var aiController = script.Enemies[script.enemyIndex];
									aiController.Weapons.Add(new WeaponsHelper.WeaponSlotInInventory {weapon = script.Weapons[items.index].gameObject});
									aiController.currentWeapon = 0;
									aiController.TakeWeapon(0);

									script.Weapons[items.index].gameObject.SetActive(true);

									script.Enemies[script.enemyIndex].anim.SetBool("Use weapon", true);

									if (script.Enemies[script.enemyIndex].anim && script.Enemies[script.enemyIndex].anim.GetLayerWeight(1) < 1)
										script.Enemies[script.enemyIndex].anim.SetLayerWeight(1, 1);

								}

								break;
							case Adjustment.AdjustmentType.Character:

								script.ResetWeapons(true);

								var weaponManager = script.Characters[script.characterIndex].gameObject.GetComponent<InventoryManager>();
								WeaponsHelper.SetWeaponController(script.Weapons[items.index].gameObject,
									script.WeaponsPrefabs[items.index], 0, weaponManager,
									script.Characters[script.characterIndex].GetComponent<Controller>(), script.Characters[script.characterIndex].transform);

								if (script.Weapons[items.index].Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
								{
									weaponManager.hasAnyWeapon = true;
									weaponManager.slots[0].weaponsInInventory.Add(new InventoryManager.Weapon
										{weapon = script.Weapons[items.index].gameObject});
									weaponManager.slots[0].currentWeaponInSlot = 0;
									weaponManager.Switch(0);
								}
								else
								{
									weaponManager.grenadeController = script.Weapons[items.index];
									if (script.Weapons[items.index].GrenadeParameters.GrenadeThrow_FPS)
									{
										script.currentController.ClipOverrides["GrenadeFPS"] = script.Weapons[items.index].GrenadeParameters.GrenadeThrow_FPS;
										script.currentController.newController.ApplyOverrides(script.currentController.ClipOverrides);
									}

									if (script.Weapons[items.index].GrenadeParameters.GrenadeThrow_TPS_TDS)
									{
										script.currentController.ClipOverrides["GrenadeTPS"] =
											script.Weapons[items.index].GrenadeParameters.GrenadeThrow_TPS_TDS;
										script.currentController.newController.ApplyOverrides(script.currentController.ClipOverrides);
									}

									WeaponsHelper.PlaceWeapon(script.Weapons[items.index].WeaponInfos[script.Weapons[items.index].SettingsSlotIndex],
										script.Weapons[items.index].transform);

									script.takeGrenadeTime_FPS = script.Weapons[items.index].WeaponInfos[script.Weapons[items.index].SettingsSlotIndex]
										.timeBeforeCreating_FPS;

									script.throwGrenadeTime_FPS =
										script.Weapons[items.index].WeaponInfos[script.Weapons[items.index].SettingsSlotIndex].timeInHand_FPS +
										script.takeGrenadeTime_FPS;

									script.takeGrenadeTime_TPS = script.Weapons[items.index].WeaponInfos[script.Weapons[items.index].SettingsSlotIndex]
										.timeBeforeCreating_TPS;

									script.throwGrenadeTime_TPS =
										script.Weapons[items.index].WeaponInfos[script.Weapons[items.index].SettingsSlotIndex].timeInHand_TPS +
										script.takeGrenadeTime_TPS;

									script.Characters[script.characterIndex].anim.CrossFade("Walk (without weapons)", 0.01f, 2);
									//script.Characters[script.characterIndex].anim.CrossFade("Walk (without weapons)", 0.01f, 3);
									script.Characters[script.characterIndex].anim.SetBool("NoWeapons", true);
								}

								break;
						}

						if (script.CurrentWeaponController &&
						    script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
							Helper.HideAllObjects(script.CurrentWeaponController.IkObjects);

						script.SerializedWeaponController = new SerializedObject(script.Weapons[items.index]);
					}

					script.weaponIndex = items.index;
					script.CurrentWeaponController = script.Weapons[script.weaponIndex];
					script.CheckIKObjects();
					script.oldDebugModeIndex = WeaponsHelper.IkDebugMode.Aim;

//					if (!Application.isPlaying)
//					{
//						if (!script.WeaponsPrefabs[items.index])
//						{
//							script.WeaponsPrefabs[items.index] = Instantiate(script.CurrentWeaponController.gameObject, Vector3.zero, Quaternion.identity);
//							script.WeaponsPrefabs[items.index].GetComponent<WeaponController>().OriginalScript = script.CurrentWeaponController;
//							//script.CurrentWeaponController = script.WeaponsPrefabs[items.index].GetComponent<WeaponController>();
//						}
//					}
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					if (Application.isPlaying && index == script.weaponIndex && script.Weapons[index])
					{
						var options = new GUIStyle {normal = {textColor = Color.green}};
						EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10,
							EditorGUIUtility.singleLineHeight), "Adjustment", options);
					}

					script.Weapons[index] = (WeaponController) EditorGUI.ObjectField(
						new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						script.Weapons[index], typeof(WeaponController), false);

//					if (!Application.isPlaying)
//					{
//						var weapon = script.WeaponsPrefabs[index];
//
//						if (weapon)
//						{
//							if (!isActive && weapon.hideFlags == HideFlags.None)
//							{
//								weapon.hideFlags = HideFlags.HideInHierarchy;
//								weapon.SetActive(false);
//								EditorApplication.RepaintHierarchyWindow();
//								EditorApplication.DirtyHierarchyWindowSorting();
//							}
//							else if (isActive && weapon.hideFlags == HideFlags.HideInHierarchy &&
//							         weapon.GetComponent<WeaponController>().Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfWeapon.Grenade))
//							{
//								weapon.hideFlags = HideFlags.None;
//								weapon.SetActive(true);
//							}
//						}
//					}
//
//					if (!Application.isPlaying && index == script.weaponIndex && script.CurrentWeaponController && isActive &&
//					    script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfWeapon.Grenade))
//					{
//						EditorGUILayout.BeginVertical("box");
//
//						for (var i = 0; i < script.CurrentWeaponController.Attacks.Count; i++)
//						{
//							var attack = script.CurrentWeaponController.Attacks[i];
//							
//							if (attack.AttackType != WeaponsHelper.TypeOfWeapon.Knife)
//							{
//								EditorGUILayout.BeginVertical("box");
//								var attackSpawnPoint = script.WeaponsPrefabs[index].GetComponent<WeaponController>().Attacks[i].AttackSpawnPoint;
//								attackSpawnPoint = (Transform) EditorGUILayout.ObjectField("Attack spawn point", attackSpawnPoint, typeof(Transform), true);
//
//								if (!attackSpawnPoint)
//								{
//									if (GUILayout.Button("Create point"))
//									{
//										attackSpawnPoint = Helper.NewPoint(script.WeaponsPrefabs[script.weaponIndex].gameObject, "Attack Point");
//
//										script.WeaponsPrefabs[index].GetComponent<WeaponController>().Attacks[i].AttackSpawnPoint = attackSpawnPoint;
//
//										var weaponController = PrefabUtility.ReplacePrefab(script.WeaponsPrefabs[script.weaponIndex],
//											script.CurrentWeaponController.gameObject).GetComponent<WeaponController>();
////
//										script.Weapons[script.weaponIndex] = weaponController;
//										script.CurrentWeaponController = script.Weapons[script.weaponIndex];
//
//										//script.CurrentWeaponController = script.WeaponsPrefabs[script.weaponIndex].GetComponent<WeaponController>();
//
//										//break;
//
//										//script.WeaponsPrefabs[script.weaponIndex] = script.Weapons[script.weaponIndex].gameObject;
//									}
//								}
//								else
//								{
//									if (script.attackPointError)
//									{
//										EditorGUILayout.HelpBox("You should adjust the position of the [Attack point]", MessageType.Warning);
//									}
//								}
//
//								EditorGUILayout.EndVertical();
//
//								if (attack.AttackType != WeaponsHelper.TypeOfWeapon.Flamethrower && attack.AttackType != WeaponsHelper.TypeOfWeapon.RocketLauncher)
//								{
//									EditorGUILayout.Space();
//									EditorGUILayout.BeginVertical("box");
//									var shellsSpawnPoint = script.WeaponsPrefabs[index].GetComponent<WeaponController>().Attacks[i].ShellPoint;
//									shellsSpawnPoint = (Transform) EditorGUILayout.ObjectField("Shells spawn point", shellsSpawnPoint, typeof(Transform), true);
//
//									if (!shellsSpawnPoint)
//									{
//										if (GUILayout.Button("Create point"))
//										{
//											shellsSpawnPoint = Helper.NewPoint(script.WeaponsPrefabs[script.weaponIndex].gameObject, "Shells spawn point");
//
//											script.WeaponsPrefabs[index].GetComponent<WeaponController>().Attacks[i].ShellPoint = shellsSpawnPoint;
//
//											var weaponController = PrefabUtility.ReplacePrefab(script.WeaponsPrefabs[script.weaponIndex],
//												script.CurrentWeaponController.gameObject).GetComponent<WeaponController>();
//
//											script.Weapons[script.weaponIndex] = weaponController;
//											script.CurrentWeaponController = script.Weapons[script.weaponIndex];
//										}
//									}
//									else
//									{
//										if (script.shellsPointError)
//										{
//											EditorGUILayout.HelpBox("You should adjust the position of the [Shells spawm point]", MessageType.Warning);
//										}
//									}
//
//									EditorGUILayout.EndVertical();
//								}
//							}
//							
//							if (attack.AttackType == WeaponsHelper.TypeOfWeapon.Knife || attack.AttackType == WeaponsHelper.TypeOfWeapon.Flamethrower)
//							{
//								EditorGUILayout.BeginVertical("box");
//								var attackCollider = script.WeaponsPrefabs[index].GetComponent<WeaponController>().Attacks[i].AttackCollider;
//								attackCollider = (BoxCollider) EditorGUILayout.ObjectField("Attack collider", attackCollider, typeof(Transform), true);
//
//								if (!attackCollider)
//								{
//									if (GUILayout.Button("Create collider"))
//									{
//										if(attack.AttackType == WeaponsHelper.TypeOfWeapon.Knife)
//											attackCollider = Helper.NewCollider("KnifeCollider", "KnifeCollider", script.WeaponsPrefabs[index].transform);
//										else attackCollider =  Helper.NewCollider("Fire Collider", "Fire", script.WeaponsPrefabs[index].transform);
//
//										script.WeaponsPrefabs[index].GetComponent<WeaponController>().Attacks[i].AttackCollider = attackCollider;
//
//										var weaponController = PrefabUtility.ReplacePrefab(script.WeaponsPrefabs[script.weaponIndex],
//											script.CurrentWeaponController.gameObject).GetComponent<WeaponController>();
////
//										script.Weapons[script.weaponIndex] = weaponController;
//										script.CurrentWeaponController = script.Weapons[script.weaponIndex];
//
//										//script.CurrentWeaponController = script.WeaponsPrefabs[script.weaponIndex].GetComponent<WeaponController>();
//
//										//break;
//
//										//script.WeaponsPrefabs[script.weaponIndex] = script.Weapons[script.weaponIndex].gameObject;
//									}
//								}
//								else
//								{
//									if (script.colliderError)
//									{
//										EditorGUILayout.HelpBox("You should adjust the size of the [Attack collider]", MessageType.Warning);
//									}
//								}
//
//								EditorGUILayout.EndVertical();
//							}
//						}
//
//						EditorGUILayout.EndVertical();
//					}
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
//			var objs = FindObjectsOfType<GameObject>();
//			foreach (var obj in objs)
//			{
//				if (obj.hideFlags == HideFlags.HideInHierarchy)
//					obj.hideFlags = HideFlags.None;
//			}

			if (!Application.isPlaying)
			{
//				if (script.inspectorTab == 1)
//				{
//					if (script.WeaponsPrefabs[script.weaponIndex] && script.WeaponsPrefabs[script.weaponIndex].hideFlags != HideFlags.HideInHierarchy)
//					{
//						script.WeaponsPrefabs[script.weaponIndex].hideFlags = HideFlags.HideInHierarchy;
//						script.WeaponsPrefabs[script.weaponIndex].SetActive(false);
//					}
//				}
//				else if (script.inspectorTab == 2 || script.inspectorTab == 0)
//				{
//					if (script.EnemiesPrefabs[script.enemyIndex] && script.EnemiesPrefabs[script.enemyIndex].hideFlags != HideFlags.HideInHierarchy)
//					{
//						script.EnemiesPrefabs[script.enemyIndex].hideFlags = HideFlags.HideInHierarchy;
//						script.EnemiesPrefabs[script.enemyIndex].SetActive(false);
//					}
//				}
			}
			else
			{
				if(script.currentController)
					if (Input.GetKeyDown(script.currentController._gamepadCodes[11]) || Input.GetKeyDown(script.currentController._keyboardCodes[11]) ||
					    Helper.CheckGamepadAxisButton(11, script.currentController._gamepadButtonsAxes, script.currentController.hasAxisButtonPressed, "GetKeyDown",
						    script.currentController.inputs.AxisButtonValues[11]))
						script.currentController.ChangeCameraType();
				
				if (script.CurrentWeaponController && script.CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
				{
					if (script.AnimType == Adjustment.animType.FPS)
					{
						script.currentController.anim.SetLayerWeight(2, 1);
					}
					else
					{
						script.currentController.anim.SetLayerWeight(2, 0);
						script.currentController.anim.SetLayerWeight(3, 0);
					}
				}
			}

			if (script.hide)
			{
				foreach (var obj in script.hideObjects)
				{
					if (obj && obj.hideFlags != HideFlags.HideInHierarchy)
					{
						obj.hideFlags = HideFlags.HideInHierarchy;
					}
				}
			}
			else
			{
				foreach (var obj in script.hideObjects)
				{
					if (obj && obj.hideFlags != HideFlags.None)
						obj.hideFlags = HideFlags.None;
				}
			}
			
			if (Application.isPlaying && script.currentController &&
			    script.oldCameraType != script.Characters[script.characterIndex].TypeOfCamera)
			{
				script.oldCameraType = script.Characters[script.characterIndex].TypeOfCamera;
				Repaint();
			}
			
		}
		

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (script.SerializedWeaponController != null)
				script.SerializedWeaponController.Update();

			EditorGUILayout.Space();

			var style = new GUIStyle
			{
				normal = new GUIStyleState {textColor = new Color32(0, 180, 70, 255)}, fontStyle = FontStyle.Bold
			};

			if (!Application.isPlaying)
			{
				EditorGUILayout.LabelField("Place here your <color=yellow>prefabs</color>," + "\n" + "then go to the [Play Mode] to start adjustment.", style);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}
			else
			{
				if (!script.isPause)
				{
					EditorGUILayout.LabelField("Press the [<color=yellow>Esc</color>] button in the Game window.", style);
					EditorGUILayout.Space();
				}
			}


			EditorGUI.BeginDisabledGroup(Application.isPlaying && !script.isPause);

			script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Characters", "Weapons"});

			switch (script.inspectorTab)
			{
				case 0:

					#region CharacterAdjustment

					EditorGUILayout.Space();
					charactersList.DoLayoutList();
					EditorGUILayout.Space();
					script.Type = Adjustment.AdjustmentType.Character;

					if (Application.isPlaying && script.isPause && script.currentController && script.currentController.DebugMode)
					{
						var curCharInfo = script.currentController.CharacterOffset;
						EditorGUILayout.BeginVertical("box");
						
						//
						EditorGUILayout.LabelField("Character offsets", EditorStyles.boldLabel);
						EditorGUILayout.BeginVertical("box");

						if (script.currentController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
						{
							EditorGUILayout.HelpBox("FP view" + "\n" +
							                        "Adjust the rotation of the character's body so that he looks in the right direction", MessageType.Info);
							
							curCharInfo.xRotationOffset = EditorGUILayout.Slider("Body rotation offset X", curCharInfo.xRotationOffset, -90, 90);
							curCharInfo.yRotationOffset = EditorGUILayout.Slider("Body rotation offset X", curCharInfo.yRotationOffset, -90, 90);
							curCharInfo.zRotationOffset = EditorGUILayout.Slider("Body rotation offset Z", curCharInfo.zRotationOffset, -90, 90);
						}
						else
						{
							EditorGUILayout.HelpBox("TP and TD view" + "\n" +
							                        "The blue arrow should look forward and red should look right.", MessageType.Info);
							
							EditorGUILayout.PropertyField(serializedObject.FindProperty("dirObjRotX"), new GUIContent("Body rotation offset X"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("dirObjRotY"), new GUIContent("Body rotation offset Y"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("dirObjRotZ"), new GUIContent("Body rotation offset Z"));
						}

						EditorGUILayout.Space();

						curCharInfo.CharacterHeight = EditorGUILayout.Slider("Character height offset", curCharInfo.CharacterHeight, -5, 5);
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						
						EditorGUILayout.LabelField("Character collider", EditorStyles.boldLabel);
						EditorGUILayout.BeginVertical("box");
						var col = script.currentController.characterCollider;
						col.center = EditorGUILayout.Vector3Field("Center", col.center);
						col.radius = EditorGUILayout.FloatField("Radius", col.radius);
						col.height = EditorGUILayout.FloatField("Height", col.height);
						EditorGUILayout.EndVertical();

						var curCamera = script.currentController.thisCameraScript;
						var curCamInfo = curCamera.CameraOffset;

						EditorGUILayout.Space();
						
						EditorGUILayout.LabelField("Camera parameters", EditorStyles.boldLabel);
						switch (script.currentController.TypeOfCamera)
						{
							case CharacterHelper.CameraType.ThirdPerson:
								EditorGUILayout.HelpBox("These parameters are responsible for the camera position. " + "\n" +
								                        "Press the [C] button (in the Game) to switch the camera type.", MessageType.Info);
								break;
							case CharacterHelper.CameraType.FirstPerson:
								EditorGUILayout.HelpBox("Use this object to adjust camera position and rotation in FP view." + "\n" +
								                        "Press the [C] button (in the Game) to switch the camera type.", MessageType.Info);
								break;
							case CharacterHelper.CameraType.TopDown:
								EditorGUILayout.HelpBox("These parameters are responsible for the camera position." + "\n" +
								                        "Press the [C] button (in the Game) to switch the camera type.", MessageType.Info);
								break;
						}

						EditorGUILayout.BeginVertical("box");

						switch (script.currentController.TypeOfCamera)
						{
							case CharacterHelper.CameraType.FirstPerson:

								EditorGUI.BeginDisabledGroup(true);
								curCamera.CameraPosition =
									(Transform) EditorGUILayout.ObjectField("Camera position", curCamera.CameraPosition, typeof(GameObject), true);
								EditorGUI.EndDisabledGroup();
								break;

							case CharacterHelper.CameraType.ThirdPerson:
								
								curCamera.CameraAim = EditorGUILayout.Toggle("Aim", curCamera.CameraAim);

								if (!curCamera.CameraAim)
								{
									curCamInfo.normDistance = EditorGUILayout.Slider("Distance", curCamInfo.normDistance, -20, 20);
									curCamInfo.normCameraOffsetX = EditorGUILayout.Slider("Camera offset in X axis", curCamInfo.normCameraOffsetX, -20, 20);
									curCamInfo.normCameraOffsetY = EditorGUILayout.Slider("Camera offset in Y axis", curCamInfo.normCameraOffsetY, -20, 20);
								}
								else
								{
									curCamInfo.aimDistance = EditorGUILayout.Slider("Distance", curCamInfo.aimDistance, -20, 20);
									curCamInfo.aimCameraOffsetX = EditorGUILayout.Slider("Camera offset in X axis", curCamInfo.aimCameraOffsetX, -20, 20);
									curCamInfo.aimCameraOffsetY = EditorGUILayout.Slider("Camera offset in Y axis", curCamInfo.aimCameraOffsetY, -20, 20);
								}

								break;
							case CharacterHelper.CameraType.TopDown:


								curCamInfo.TD_Distance = EditorGUILayout.Slider("Distance", curCamInfo.TD_Distance, -20, 20);
								curCamInfo.TopDownAngle = EditorGUILayout.Slider("Angle", curCamInfo.TopDownAngle, 60, 90);
								curCamInfo.tdCameraOffsetX = EditorGUILayout.Slider("Camera offset in X axis", curCamInfo.tdCameraOffsetX, -20, 20);
								curCamInfo.tdCameraOffsetY = EditorGUILayout.Slider("Camera offset in Y axis", curCamInfo.tdCameraOffsetY, -20, 20);

								break;
						}

						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();


						if (!script.currentController.CharacterOffset.HasTime)
							EditorGUILayout.LabelField("Not any save", style);
						else
						{
							var time = script.currentController.CharacterOffset.SaveTime;
							var date = script.currentController.CharacterOffset.SaveDate;
							EditorGUILayout.LabelField("Last save: " + date.x + "/" + date.y + "/" + date.z + " " +
							                           time.x + ":" + time.y + ":" + time.z, style);
						}

						if (GUILayout.Button("Save"))
						{
							script.currentController.CharacterOffset.SaveDate =
								new Vector3(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
							script.currentController.CharacterOffset.SaveTime =
								new Vector3(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

							script.currentController.CharacterOffset.HasTime = true;

							script.currentController.CharacterOffset.directionObjRotation = new Vector3(script.dirObjRotX, script.dirObjRotY, script.dirObjRotZ);

							script.CurrentCharacterOffsets[script.characterIndex].Clone(script.currentController.CharacterOffset);
							script.currentController.OriginalScript.CharacterOffset.Clone(script.currentController.CharacterOffset);

							script.currentController.thisCameraScript.CameraOffset.cameraObjPos =
								script.currentController.thisCameraScript.CameraPosition.localPosition;

							script.currentController.thisCameraScript.CameraOffset.cameraObjRot =
								script.currentController.thisCameraScript.CameraPosition.localEulerAngles;

							script.CurrentCameraOffsets[script.characterIndex].Clone(script.currentController.thisCameraScript.CameraOffset);
							script.currentController.thisCameraScript.OriginalScript.CameraOffset.Clone(script.currentController.thisCameraScript.CameraOffset);

							script.currentController.OriginalScript.characterCollider.center = script.currentController.characterCollider.center;
							script.currentController.OriginalScript.characterCollider.radius = script.currentController.characterCollider.radius;
							script.currentController.OriginalScript.characterCollider.height = script.currentController.characterCollider.height;

//							script.currentController.thisCameraScript.CameraPosition.parent = script.currentController.BodyObjects.Head;
						}

						EditorGUI.BeginDisabledGroup(!script.currentController.CharacterOffset.HasTime);
						if (GUILayout.Button("Return values from last save"))
						{
							script.currentController.CharacterOffset.Clone(script.CurrentCharacterOffsets[script.characterIndex]);
							script.currentController.thisCameraScript.CameraOffset.Clone(script.CurrentCameraOffsets[script.characterIndex]);

							curCamera.CameraPosition.localPosition = curCamera.CameraOffset.cameraObjPos;
							curCamera.CameraPosition.localEulerAngles = curCamera.CameraOffset.cameraObjRot;

							script.currentController.DirectionObject.localEulerAngles = script.currentController.CharacterOffset.directionObjRotation;
						}

						EditorGUI.EndDisabledGroup();

						if (GUILayout.Button("Set default positions"))
						{
							script.currentController.CharacterOffset.xRotationOffset = 0;
							script.currentController.CharacterOffset.yRotationOffset = 0;
							script.currentController.CharacterOffset.zRotationOffset = 0;
							script.currentController.CharacterOffset.CharacterHeight = -1.1f;
							
							script.currentController.CharacterOffset.directionObjRotation = Vector3.zero;
							script.currentController.DirectionObject.eulerAngles = Vector3.zero;
							
							script.currentController.characterCollider.center = Vector3.zero;
							script.currentController.characterCollider.height = 1;
							script.currentController.characterCollider.radius = 0.5f;

							script.dirObjRotX = 0;
							script.dirObjRotY = 0;
							script.dirObjRotZ = 0;
							
							curCamera.CameraOffset.normDistance = 0;
							curCamera.CameraOffset.normCameraOffsetX = 0;
							curCamera.CameraOffset.normCameraOffsetY = 0;
							
							curCamera.CameraOffset.aimDistance = 0;
							curCamera.CameraOffset.aimCameraOffsetX = 0;
							curCamera.CameraOffset.aimCameraOffsetY = 0;

							curCamera.CameraOffset.cameraObjPos = Vector3.zero;
							curCamera.CameraOffset.cameraObjRot = Vector3.zero;

							curCamera.CameraPosition.localPosition = Vector3.zero;
							curCamera.CameraPosition.localEulerAngles = Vector3.zero;

							curCamera.CameraOffset.TopDownAngle = 80;
							curCamera.CameraOffset.tdCameraOffsetX = 0;
							curCamera.CameraOffset.tdCameraOffsetY = 0;
							curCamera.CameraOffset.TD_Distance = 0;

						}

						EditorGUILayout.EndVertical();
					}

					#endregion

					break;

				case 2:
					EditorGUILayout.Space();
					
					
					EditorGUILayout.LabelField("New enemies will be coming soon.",
						new GUIStyle {normal = new GUIStyleState {textColor = Color.white}, fontStyle = FontStyle.Italic});
					
					//enemiesList.DoLayoutList();
					//EditorGUILayout.Space();
//					if (Application.isPlaying && script.CurrentAiController)
//					{
//						script.enemyState = GUILayout.Toolbar(script.enemyState, new[] {"Idle", "Walk", "Run"});
//
//						if (script.enemyState != script.oldEnemyState)
//						{
//							switch (script.enemyState)
//							{
//								case 0:
//									script.CurrentAiController.anim.Play("Idle", 0);
//									script.CurrentAiController.anim.SetBool("Move", false);
//									script.CurrentAiController.anim.SetBool("Attack state", false);
//									break;
//
//								case 1:
//									script.CurrentAiController.anim.Play("Walk", 0);
//									script.CurrentAiController.anim.SetBool("Move", true);
//									script.CurrentAiController.anim.SetBool("Attack state", false);
//									break;
//
//								case 2:
//									script.CurrentAiController.anim.Play("Run", 0);
//									script.CurrentAiController.anim.SetBool("Move", true);
//									script.CurrentAiController.anim.SetBool("Attack state", true);
//									break;
//							}
//
//							script.oldEnemyState = script.enemyState;
//						}
//						
//						EditorGUILayout.Space();
//					}

					script.Type = Adjustment.AdjustmentType.Enemy;

					break;

				case 1:

					#region WeaponAdjustment

					if (Application.isPlaying && script.isPause && !script.currentController && !script.CurrentAiController)
					{
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("First of all select any character or enemy.", style);
					}

					EditorGUI.BeginDisabledGroup(Application.isPlaying && !script.currentController && !script.CurrentAiController);
					EditorGUILayout.Space();
					weaponsList.DoLayoutList();
					EditorGUILayout.Space();
					EditorGUI.EndDisabledGroup();



					if (Application.isPlaying && script.isPause && script.CurrentWeaponController && (script.currentController || script.CurrentAiController)
					    && script.CurrentWeaponController.ActiveDebug)
					{

						if (script.CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade) && script.Type != Adjustment.AdjustmentType.Enemy ||
						    script.CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade) && script.Type == Adjustment.AdjustmentType.Character || 
						    script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))

						{
							
							var curInfo = script.CurrentWeaponController.CurrentWeaponInfo[script.CurrentWeaponController.SettingsSlotIndex];

							EditorGUILayout.BeginVertical("box");

							if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
							{
								EditorGUILayout.BeginVertical("box");
								script.CurrentWeaponController.SettingsSlotIndex = EditorGUILayout.Popup("Weapon settings slot",
									script.CurrentWeaponController.SettingsSlotIndex, script.CurrentWeaponController.enumNames.ToArray());

								EditorGUILayout.Space();
								EditorGUILayout.LabelField("Copy values from:");
								EditorGUILayout.BeginHorizontal();
								script.copyFromWeaponSlot = EditorGUILayout.Popup(script.copyFromWeaponSlot, script.WeaponsNames.ToArray());

								if (script.Weapons[script.copyFromWeaponSlot].Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
								{
									script.copyFromSlot = EditorGUILayout.Popup(script.copyFromSlot, script.Weapons[script.copyFromWeaponSlot].enumNames.ToArray());
									EditorGUILayout.EndHorizontal();
									if (GUILayout.Button("Copy"))
									{
										script.CurrentWeaponController.CurrentWeaponInfo[script.CurrentWeaponController.SettingsSlotIndex]
											.Clone(script.Weapons[script.copyFromWeaponSlot].WeaponInfos[script.copyFromSlot]);
										WeaponsHelper.SetWeaponPositions(script.CurrentWeaponController, true);
									}
								}
								else
								{
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.HelpBox("You can't cope values from grenade.", MessageType.Warning);
								}

								EditorGUILayout.Space();
								if (!rename)
								{
									if (GUILayout.Button("Rename"))
									{
										rename = true;
										curName = "";
									}
								}
								else
								{
									EditorGUILayout.BeginVertical("box");
									curName = EditorGUILayout.TextField("New name", curName);

									EditorGUILayout.BeginHorizontal();
									
									if (GUILayout.Button("Cancel"))
									{
										rename = false;
										renameError = false;
										curName = "";
									}
									
									if (GUILayout.Button("Save"))
									{
										if (!script.CurrentWeaponController.enumNames.Contains(curName))
										{
											rename = false;
											script.CurrentWeaponController.enumNames[script.CurrentWeaponController.SettingsSlotIndex] = curName;
											script.CurrentWeaponController.OriginalScript.enumNames[script.CurrentWeaponController.SettingsSlotIndex] = curName;
											curName = "";
											renameError = false;
										}
										else
										{
											renameError = true;
										}
									}

									EditorGUILayout.EndHorizontal();
									
									if (renameError)
										EditorGUILayout.HelpBox("This name already exist.", MessageType.Warning);
									
									EditorGUILayout.EndVertical();
								}


								EditorGUI.BeginDisabledGroup(script.CurrentWeaponController.WeaponInfos.Count <= 1);
								if (!delete)
								{
									if (GUILayout.Button("Delete slot"))
									{
										delete = true;
									}
								}
								else
								{
									EditorGUILayout.BeginVertical("box");
									EditorGUILayout.LabelField("Are you sure?");
									EditorGUILayout.BeginHorizontal();
									
									if (GUILayout.Button("No"))
									{
										delete = false;
									}
									
									if (GUILayout.Button("Yes"))
									{
										if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
											Helper.HideAllObjects(script.CurrentWeaponController.IkObjects);
										Selection.activeObject = script.gameObject;

										script.CurrentWeaponController.WeaponInfos.Remove(script.Weapons[script.weaponIndex]
											.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex]);

										script.CurrentWeaponController.CurrentWeaponInfo.Remove(
											script.CurrentWeaponController.CurrentWeaponInfo[script.CurrentWeaponController.SettingsSlotIndex]);

										script.CurrentWeaponController.enumNames.Remove(script.Weapons[script.weaponIndex]
											.enumNames[script.CurrentWeaponController.SettingsSlotIndex]);

										script.CurrentWeaponController.OriginalScript.WeaponInfos.Remove(
											script.CurrentWeaponController.OriginalScript.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex]);

										script.CurrentWeaponController.OriginalScript.enumNames.Remove(
											script.CurrentWeaponController.OriginalScript.enumNames[script.CurrentWeaponController.SettingsSlotIndex]);

										var newInfoIndex = script.CurrentWeaponController.SettingsSlotIndex;

										newInfoIndex++;
										if (newInfoIndex > script.CurrentWeaponController.WeaponInfos.Count - 1)
										{
											newInfoIndex = 0;
										}

										script.CurrentWeaponController.SettingsSlotIndex = newInfoIndex;
										delete = false;
									}

									EditorGUILayout.EndHorizontal();
									EditorGUILayout.EndVertical();
								}

								EditorGUI.EndDisabledGroup();
								EditorGUILayout.EndVertical();

								if (GUILayout.Button("Add new slot"))
								{
									script.CurrentWeaponController.WeaponInfos.Add(new WeaponsHelper.WeaponInfo());
									script.CurrentWeaponController.OriginalScript.WeaponInfos.Add(new WeaponsHelper.WeaponInfo());


									script.CurrentWeaponController.enumNames.Add("Slot " + (script.CurrentWeaponController.enumNames.Count + 1));
									script.CurrentWeaponController.OriginalScript.enumNames.Add("Slot " + script.CurrentWeaponController.enumNames.Count);

									script.CurrentWeaponController.CurrentWeaponInfo.Add(new WeaponsHelper.WeaponInfo());

									script.CurrentWeaponController.SettingsSlotIndex = script.CurrentWeaponController.enumNames.Count - 1;

									break;
								}
							}

							if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
							{

								EditorGUILayout.Space();
								EditorGUILayout.BeginVertical("box");

								EditorGUILayout.HelpBox("Use these parameters to adjust fingers rotations in all axises.", MessageType.Info);
								axises = (FingersRotationAxises) EditorGUILayout.EnumPopup("Fingers rotation axis", axises);

								EditorGUILayout.Space();
								switch (axises)
								{
									case FingersRotationAxises.X:

										curInfo.FingersRightX = EditorGUILayout.Slider("Right Fingers", curInfo.FingersRightX, -25, 25);

										curInfo.ThumbRightX = EditorGUILayout.Slider("Right Thumb", curInfo.ThumbRightX, -25, 25);

										EditorGUILayout.Space();

										curInfo.FingersLeftX = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftX, -25, 25);

										curInfo.ThumbLeftX = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftX, -25, 25);

										break;
									case FingersRotationAxises.Y:

										curInfo.FingersRightY = EditorGUILayout.Slider("Right Fingers", curInfo.FingersRightY, -25, 25);

										curInfo.ThumbRightY = EditorGUILayout.Slider("Right Thumb", curInfo.ThumbRightY, -25, 25);

										EditorGUILayout.Space();

										curInfo.FingersLeftY = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftY, -25, 25);

										curInfo.ThumbLeftY = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftY, -25, 25);

										break;
									case FingersRotationAxises.Z:

										curInfo.FingersRightZ = EditorGUILayout.Slider("Right Fingers", curInfo.FingersRightZ, -25, 25);

										curInfo.ThumbRightZ = EditorGUILayout.Slider("Right Thumb", curInfo.ThumbRightZ, -25, 25);

										EditorGUILayout.Space();

										curInfo.FingersLeftZ = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftZ, -25, 25);

										curInfo.ThumbLeftZ = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftZ, -25, 25);

										break;
								}

								EditorGUILayout.EndVertical();
								EditorGUILayout.Space();

								EditorGUILayout.BeginVertical("box");

								EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("DebugMode"), new GUIContent("Debug Mode"));

								EditorGUILayout.Space();

								if (script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Norm)
								{
									curInfo.disableIkInNormalState = EditorGUILayout.ToggleLeft("Disable IK in normal state", curInfo.disableIkInNormalState);
								}
								else if (script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Crouch)
								{
									curInfo.disableIkInCrouchState = EditorGUILayout.ToggleLeft("Disable IK in crouch state", curInfo.disableIkInCrouchState);
								}

								if (script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Wall)
								{
									EditorGUILayout.HelpBox("Use this object to adjust the position, rotation and size of the Collider. " + "\n" +
									                        "During the game this Collider will check the collision with a wall for this weapon.", MessageType.Info);

									EditorGUI.BeginDisabledGroup(true);
									script.CurrentWeaponController.ColliderToCheckWalls =
										(Transform) EditorGUILayout.ObjectField("Collider to check", script.CurrentWeaponController.ColliderToCheckWalls,
											typeof(Transform), true);
									EditorGUI.EndDisabledGroup();
									EditorGUILayout.Space();
								}

								if (script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Norm && !curInfo.disableIkInNormalState ||  
								    script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Crouch && !curInfo.disableIkInCrouchState
								     || script.CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Norm
								    || script.CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Crouch)
								{
									EditorGUILayout.HelpBox("Use these objects to adjust hands and elbows positions and rotations." + "\n" +
									                        "To adjust other IK positions (aim and wall) switch [Debug Mode].", MessageType.Info);
								}

								EditorGUI.BeginDisabledGroup(true);
								switch (script.CurrentWeaponController.DebugMode)
								{
									case WeaponsHelper.IkDebugMode.Norm:
										if (!curInfo.disableIkInNormalState)
										{
											EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.RightObject"),
												new GUIContent("Right hand IK object"));
											EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.LeftObject"),
												new GUIContent("Left hand IK object"));
											EditorGUILayout.Space();
										}
										break;
									case WeaponsHelper.IkDebugMode.Aim:
										if (!curInfo.disableIkInCrouchState)
										{
											EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.RightAimObject"),
												new GUIContent("Right hand Aim IK object"));
											EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.LeftAimObject"),
												new GUIContent("Left hand Aim IK object"));
											EditorGUILayout.Space();
										}

										break;
									case WeaponsHelper.IkDebugMode.Wall:
										EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.RightWallObject"),
											new GUIContent("Right hand (wall) IK object"));
										EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.LeftWallObject"),
											new GUIContent("Left hand (wall) IK object"));
										EditorGUILayout.Space();
										break;
									case WeaponsHelper.IkDebugMode.Crouch:
										EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.RightCrouchObject"),
											new GUIContent("Right hand (crouch) IK object"));
										EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.LeftCrouchObject"),
											new GUIContent("Left hand (crouch) IK object"));
										EditorGUILayout.Space();
										break;
								}

								if (script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Norm && !curInfo.disableIkInNormalState ||  
								    script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Crouch && !curInfo.disableIkInCrouchState
								    || script.CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Norm
								    || script.CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Crouch)
								{
									EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.RightElbowObject"),
										new GUIContent("Right elbow IK object"));
									EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("IkObjects.LeftElbowObject"),
										new GUIContent("Left elbow IK object"));
									EditorGUILayout.Space();
								}

								EditorGUI.EndDisabledGroup();
								switch (script.CurrentWeaponController.DebugMode)
								{
									case WeaponsHelper.IkDebugMode.Aim:
									{
										EditorGUILayout.Space();

										EditorGUILayout.HelpBox("Press this button to place your hands in the same position as the standard view.", MessageType.Info);
										
										if (GUILayout.Button("Set [Aim IK] to [Normal view] positions"))
										{
											script.CurrentWeaponController.IkObjects.RightAimObject.localPosition =
												script.CurrentWeaponController.IkObjects.RightObject.localPosition;
											script.CurrentWeaponController.IkObjects.RightAimObject.localRotation =
												script.CurrentWeaponController.IkObjects.RightObject.localRotation;

											script.CurrentWeaponController.IkObjects.LeftAimObject.localPosition =
												script.CurrentWeaponController.IkObjects.LeftObject.localPosition;
											script.CurrentWeaponController.IkObjects.LeftAimObject.localRotation =
												script.CurrentWeaponController.IkObjects.LeftObject.localRotation;
										}

										EditorGUILayout.EndVertical();

										break;
									}

									case WeaponsHelper.IkDebugMode.Wall:
									{
										EditorGUILayout.Space();

										EditorGUILayout.HelpBox(
											"Press this button to place your hands in the same position as the standard view.", MessageType.Info);

										if (GUILayout.Button("Set [Walk IK] to [Normal view] positions"))
										{
											script.CurrentWeaponController.IkObjects.RightWallObject.localPosition =
												script.CurrentWeaponController.IkObjects.RightObject.localPosition;
											script.CurrentWeaponController.IkObjects.RightWallObject.localRotation =
												script.CurrentWeaponController.IkObjects.RightObject.localRotation;

											script.CurrentWeaponController.IkObjects.LeftWallObject.localPosition =
												script.CurrentWeaponController.IkObjects.LeftObject.localPosition;
											script.CurrentWeaponController.IkObjects.LeftWallObject.localRotation =
												script.CurrentWeaponController.IkObjects.LeftObject.localRotation;
										}

										EditorGUILayout.EndVertical();

										break;
									}

									case WeaponsHelper.IkDebugMode.Norm:
										EditorGUILayout.EndVertical();
										break;
									case WeaponsHelper.IkDebugMode.Crouch:
										EditorGUILayout.Space();

										EditorGUILayout.HelpBox(
											"Press this button to place your hands in the same position as the standard view.", MessageType.Info);

										if (GUILayout.Button("Set [Crouch IK] to [Normal view] positions"))
										{
											script.CurrentWeaponController.IkObjects.RightCrouchObject.localPosition =
												script.CurrentWeaponController.IkObjects.RightObject.localPosition;
											script.CurrentWeaponController.IkObjects.RightCrouchObject.localRotation =
												script.CurrentWeaponController.IkObjects.RightObject.localRotation;

											script.CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition =
												script.CurrentWeaponController.IkObjects.LeftObject.localPosition;
											script.CurrentWeaponController.IkObjects.LeftCrouchObject.localRotation =
												script.CurrentWeaponController.IkObjects.LeftObject.localRotation;
										}

										EditorGUILayout.EndVertical();
										break;
								}

								EditorGUILayout.EndVertical();
								EditorGUILayout.Space();

							}
							else if (script.CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade) &&
							         script.Type != Adjustment.AdjustmentType.Enemy)
							{
//							if (script.state == Adjustment.GrenadeAdjustmentState.Position)
//							{

								EditorGUILayout.BeginVertical("box");
								EditorGUILayout.HelpBox(
									"First of all, adjust the grenade creation and launch time to match the animation.", MessageType.Info);
								
								script.AnimType = (Adjustment.animType)EditorGUILayout.EnumPopup("Animation for", script.AnimType);
								
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();
								if (GUILayout.Button(Resources.Load(!script.playGrenadeAnimation ? "PlayButton" : "PauseButton") as Texture2D,
									GUILayout.Width(22), GUILayout.Height(22)))
								{
									if (!script.playGrenadeAnimation)
									{
										script.currentController.transform.rotation = Quaternion.Euler(0,0,0);
										script.currentController.transform.position = Vector3.zero;
									}

									script.playGrenadeAnimation = !script.playGrenadeAnimation;
								}

								EditorGUI.BeginDisabledGroup(script.playGrenadeAnimation);

								var length = script.AnimType == Adjustment.animType.TPS_TDS
									? script.CurrentWeaponController.GrenadeParameters.GrenadeThrow_TPS_TDS.length
									: script.CurrentWeaponController.GrenadeParameters.GrenadeThrow_FPS.length;

								if (!script.playGrenadeAnimation)
									script.animationValue = EditorGUILayout.Slider(script.animationValue, 0, length - 0.3f);
								else
								{
									EditorGUILayout.Slider(script.animationValue, 0, length - 0.3f);
								}


								EditorGUI.EndDisabledGroup();

								EditorGUILayout.EndHorizontal();
								EditorGUILayout.Space();

								EditorGUILayout.BeginHorizontal();

								if (script.AnimType == Adjustment.animType.FPS)
								{
									EditorGUILayout.PropertyField(serializedObject.FindProperty("takeGrenadeTime_FPS"), new GUIContent("Time before taking"));
									if (GUILayout.Button("Set"))
									{
										script.takeGrenadeTime_FPS = script.animationValue;
									}

									EditorGUILayout.EndHorizontal();

									EditorGUILayout.BeginHorizontal();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("throwGrenadeTime_FPS"), new GUIContent("Throw time"));
									if (GUILayout.Button("Set"))
									{
										if (script.takeGrenadeTime_FPS < script.animationValue)
											script.throwGrenadeTime_FPS = script.animationValue;
									}
								}
								else
								{
									EditorGUILayout.PropertyField(serializedObject.FindProperty("takeGrenadeTime_TPS"), new GUIContent("Time before taking"));
									if (GUILayout.Button("Set"))
									{
										script.takeGrenadeTime_TPS = script.animationValue;
									}

									EditorGUILayout.EndHorizontal();

									EditorGUILayout.BeginHorizontal();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("throwGrenadeTime_TPS"), new GUIContent("Throw time"));
									if (GUILayout.Button("Set"))
									{
										if (script.takeGrenadeTime_TPS < script.animationValue)
											script.throwGrenadeTime_TPS = script.animationValue;
									}
								}

								EditorGUILayout.EndHorizontal();
								EditorGUILayout.EndVertical();
								EditorGUILayout.Space();

								EditorGUILayout.BeginVertical("box");
								EditorGUILayout.HelpBox("You can adjust the position, rotation and size of grenades in the character’s hand.", MessageType.Info);

								EditorGUI.BeginDisabledGroup(true);
								EditorGUILayout.ObjectField("Grenade", script.CurrentWeaponController.gameObject, typeof(GameObject), true);
								EditorGUI.EndDisabledGroup();
								EditorGUILayout.EndVertical();

								EditorGUILayout.Space();

								EditorGUILayout.BeginVertical("box");
								EditorGUILayout.HelpBox("Use these parameters to adjust fingers rotations in all axises.", MessageType.Info);

								axises = (FingersRotationAxises) EditorGUILayout.EnumPopup("Fingers rotation axis", axises);
								EditorGUILayout.Space();
								switch (axises)
								{
									case FingersRotationAxises.X:
										curInfo.FingersLeftX = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftX, -25, 25);
										curInfo.ThumbLeftX = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftX, -25, 25);

										break;
									case FingersRotationAxises.Y:
										curInfo.FingersLeftY = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftY, -25, 25);
										curInfo.ThumbLeftY = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftY, -25, 25);

										break;
									case FingersRotationAxises.Z:
										curInfo.FingersLeftZ = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftZ, -25, 25);
										curInfo.ThumbLeftZ = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftZ, -25, 25);
										break;
								}

//							}
//							else
//							{
//								EditorGUILayout.BeginHorizontal();
//
//								if (GUILayout.Button(Resources.Load(!script.playGrenadeAnimation ? "PlayButton" : "PauseButton") as Texture2D,
//									GUILayout.Width(22), GUILayout.Height(22)))
//								{
//									script.playGrenadeAnimation = !script.playGrenadeAnimation;
//								}
//
//								EditorGUI.BeginDisabledGroup(script.playGrenadeAnimation);
//								if (!script.playGrenadeAnimation)
//									script.animationValue = EditorGUILayout.Slider(script.animationValue, 0,
//										script.CurrentWeaponController.GrenadeParameters.GrenadeThrow.length - 0.3f);
//								else
//									EditorGUILayout.Slider(script.animationValue, 0,
//										script.CurrentWeaponController.GrenadeParameters.GrenadeThrow.length - 0.3f);
//								EditorGUI.EndDisabledGroup();
//
//								EditorGUILayout.EndHorizontal();
//								EditorGUILayout.Space();
//
//								EditorGUILayout.BeginHorizontal();
//								EditorGUILayout.PropertyField(serializedObject.FindProperty("takeGrenadeTime"), new GUIContent("Time before taking"));
//								if (GUILayout.Button("Set"))
//								{
//									script.takeGrenadeTime = script.animationValue;
//								}
//
//								EditorGUILayout.EndHorizontal();
//								
//								EditorGUILayout.BeginHorizontal();
//								EditorGUILayout.PropertyField(serializedObject.FindProperty("throwGrenadeTime"), new GUIContent("Throw time"));
//								if (GUILayout.Button("Set"))
//								{
//									if(script.takeGrenadeTime < script.animationValue)
//										script.throwGrenadeTime = script.animationValue;
//								}
//								EditorGUILayout.EndHorizontal();
//							}
								EditorGUILayout.EndVertical();
								EditorGUILayout.EndVertical();
								EditorGUILayout.Space();
							}



							if (script.CurrentWeaponController.WeaponInfos.Count > 0 &&
							    !script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].HasTime)
								EditorGUILayout.LabelField("Not any save", style);
							else
							{
								var time = script.CurrentWeaponController
									.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].SaveTime;
								var date = script.CurrentWeaponController
									.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].SaveDate;
								EditorGUILayout.LabelField("Last save: " + date.x + "/" + date.y + "/" + date.z + " " +
								                           time.x + ":" + time.y + ":" + time.z, style);
							}

							if (GUILayout.Button("Save"))
							{
//								if (script.CurrentWeaponController.WeaponType != WeaponsHelper.TypeOfWeapon.Grenade)
//								{
//									
//									curInfo.WeaponSize = script.CurrentWeaponController.transform.localScale;
//									curInfo.WeaponPosition = script.CurrentWeaponController.transform.localPosition;
//									curInfo.WeaponRotation = script.CurrentWeaponController.transform.localEulerAngles;
//
//									curInfo.RightHandPosition = script.CurrentWeaponController.IkObjects.RightObject.localPosition;
//									curInfo.RightHandRotation = script.CurrentWeaponController.IkObjects.RightObject.localEulerAngles;
//
//									curInfo.LeftHandPosition = script.CurrentWeaponController.IkObjects.LeftObject.localPosition;
//									curInfo.LeftHandRotation = script.CurrentWeaponController.IkObjects.LeftObject.localEulerAngles;
//
//									if (script.CurrentWeaponController.hasAimIKChanged)
//									{
//										curInfo.RightAimPosition = script.CurrentWeaponController.IkObjects.RightAimObject.localPosition;
//										curInfo.RightAimRotation = script.CurrentWeaponController.IkObjects.RightAimObject.localEulerAngles;
//
//										curInfo.LeftAimPosition = script.CurrentWeaponController.IkObjects.LeftAimObject.localPosition;
//										curInfo.LeftAimRotation = script.CurrentWeaponController.IkObjects.LeftAimObject.localEulerAngles;
//									}
//
//									if (script.CurrentWeaponController.hasWallIKChanged)
//									{
//										curInfo.RightHandWallPosition =script.CurrentWeaponController.IkObjects.RightWallObject.localPosition;
//										curInfo.RightHandWallRotation =script.CurrentWeaponController.IkObjects.RightWallObject.localEulerAngles;
//
//										curInfo.LeftHandWallPosition = script.CurrentWeaponController.IkObjects.LeftWallObject.localPosition;
//										curInfo.LeftHandWallRotation = script.CurrentWeaponController.IkObjects.LeftWallObject.localEulerAngles;
//									}
//
//									curInfo.LeftElbowPosition =
//										script.CurrentWeaponController.IkObjects.LeftElbowObject.localPosition;
//									curInfo.RightElbowPosition =
//										script.CurrentWeaponController.IkObjects.RightElbowObject.localPosition;
//
////								script.CurrentWeaponController.ColliderToCheckWalls.parent = script.currentController.BodyObjects.RightHand;
//									curInfo.CheckWallsBoxPosition = script.CurrentWeaponController.ColliderToCheckWalls.localPosition;
//
//									curInfo.CheckWallsBoxRotation = script.CurrentWeaponController.ColliderToCheckWalls.localEulerAngles;
//
//									curInfo.CheckWallsColliderSize = script.CurrentWeaponController.ColliderToCheckWalls.localScale;
//								}
//								else
//								{
//
//									var transform = script.CurrentWeaponController.transform;
//									curInfo.WeaponPosition = transform.localPosition;
//									curInfo.WeaponRotation = transform.localEulerAngles;
//									curInfo.WeaponSize = transform.localScale;
//
//									curInfo.timeBeforeCreating = script.takeGrenadeTime;
//									curInfo.timeInHand = script.throwGrenadeTime - script.takeGrenadeTime;
//								}

								curInfo.SaveDate = new Vector3(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
								curInfo.SaveTime = new Vector3(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

								curInfo.HasTime = true;

								script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].Clone(curInfo);

								SaveData();

								if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
								{
									WeaponsHelper.CheckIK(ref script.CurrentWeaponController.CanUseElbowIK,
										ref script.CurrentWeaponController.CanUseIK, ref script.CurrentWeaponController.CanUseAimIK,
										ref script.CurrentWeaponController.CanUseWallIK, ref script.CurrentWeaponController.CanUseCrouchIK, curInfo);

									if (!script.CurrentWeaponController.CanUseIK)
										script.CurrentWeaponController.CanUseIK = true;
								}
							}

							EditorGUI.BeginDisabledGroup(!curInfo.HasTime);
							if (GUILayout.Button("Return values from last save"))
							{
								curInfo.Clone(script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex]);

								if (curInfo.WeaponSize != Vector3.zero)
									script.CurrentWeaponController.transform.localScale = curInfo.WeaponSize;
								else
									script.CurrentWeaponController.transform.localScale = script.currentScales[script.weaponIndex];

								script.CurrentWeaponController.transform.localPosition = curInfo.WeaponPosition;
								script.CurrentWeaponController.transform.localEulerAngles = curInfo.WeaponRotation;


								if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
								{
									if (curInfo.RightHandPosition != Vector3.zero && curInfo.RightHandRotation != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.RightObject.localPosition = curInfo.RightHandPosition;
										script.CurrentWeaponController.IkObjects.RightObject.localEulerAngles = curInfo.RightHandRotation;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.RightObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
										script.CurrentWeaponController.IkObjects.RightObject.localPosition = Vector3.zero;
										script.CurrentWeaponController.IkObjects.RightObject.localRotation = Quaternion.Euler(-90, 0, 0);
									}
									if (curInfo.LeftHandPosition != Vector3.zero && curInfo.LeftHandRotation != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.LeftObject.localPosition =
											curInfo.LeftHandPosition;
										script.CurrentWeaponController.IkObjects.LeftObject.localEulerAngles =
											curInfo.LeftHandRotation;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.LeftObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
										script.CurrentWeaponController.IkObjects.LeftObject.localPosition = Vector3.zero;
										script.CurrentWeaponController.IkObjects.LeftObject.localRotation = Quaternion.Euler(-90, 0, 0);
									}
									
									
									
									if (curInfo.RightCrouchHandPosition != Vector3.zero && curInfo.RightCrouchHandRotation != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.RightCrouchObject.localPosition = curInfo.RightCrouchHandPosition;
										script.CurrentWeaponController.IkObjects.RightCrouchObject.localEulerAngles = curInfo.RightCrouchHandRotation;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.RightCrouchObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
										script.CurrentWeaponController.IkObjects.RightCrouchObject.localPosition = Vector3.zero;
										script.CurrentWeaponController.IkObjects.RightCrouchObject.localRotation = Quaternion.Euler(-90, 0, 0);
									}
									if (curInfo.LeftCrouchHandPosition != Vector3.zero && curInfo.LeftCrouchHandRotation != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition = curInfo.LeftCrouchHandPosition;
										script.CurrentWeaponController.IkObjects.LeftCrouchObject.localEulerAngles = curInfo.LeftCrouchHandRotation;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.LeftCrouchObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
										script.CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition = Vector3.zero;
										script.CurrentWeaponController.IkObjects.LeftCrouchObject.localRotation = Quaternion.Euler(-90, 0, 0);
									}
									
									

									if (curInfo.RightAimPosition != Vector3.zero && curInfo.RightAimRotation != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.RightAimObject.localPosition = script.CurrentWeaponController
											.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].RightAimPosition;
										script.CurrentWeaponController.IkObjects.RightAimObject.localEulerAngles = script.CurrentWeaponController
											.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].RightAimRotation;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.RightAimObject.parent =
											script.CurrentWeaponController.BodyObjects.RightHand;
										script.CurrentWeaponController.IkObjects.RightAimObject.localPosition = Vector3.up;
										script.CurrentWeaponController.IkObjects.RightAimObject.localRotation = Quaternion.Euler(-90, 0, 0);
									}
									if (curInfo.LeftAimPosition != Vector3.zero && curInfo.LeftAimRotation != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.LeftAimObject.localPosition =
											script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].LeftAimPosition;
										script.CurrentWeaponController.IkObjects.LeftAimObject.localEulerAngles = script.CurrentWeaponController
											.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].LeftAimRotation;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.LeftAimObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
										script.CurrentWeaponController.IkObjects.LeftAimObject.localPosition = Vector3.up;
										script.CurrentWeaponController.IkObjects.LeftAimObject.localRotation = Quaternion.Euler(-90, 0, 0);
									}

									
									
									if (curInfo.RightHandWallPosition != Vector3.zero && curInfo.RightHandWallRotation != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.RightWallObject.localPosition =
											curInfo.RightHandWallPosition;
										script.CurrentWeaponController.IkObjects.RightWallObject.localEulerAngles =
											curInfo.RightHandWallRotation;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.RightWallObject.parent =
											script.CurrentWeaponController.BodyObjects.RightHand;
										script.CurrentWeaponController.IkObjects.RightWallObject.localPosition = Vector3.up;
										script.CurrentWeaponController.IkObjects.RightWallObject.localRotation = Quaternion.Euler(-90, 0, 0);
									}
									if (curInfo.LeftHandWallPosition != Vector3.zero && curInfo.LeftHandWallRotation != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.LeftWallObject.localPosition
											= curInfo.LeftHandWallPosition;
										script.CurrentWeaponController.IkObjects.LeftWallObject.localEulerAngles
											= curInfo.LeftHandWallRotation;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.LeftWallObject.parent =
											script.CurrentWeaponController.BodyObjects.RightHand;
										script.CurrentWeaponController.IkObjects.LeftWallObject.localPosition = Vector3.up;
										script.CurrentWeaponController.IkObjects.LeftWallObject.localRotation = Quaternion.Euler(-90, 0, 0);
									}

									if (curInfo.LeftElbowPosition != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.LeftElbowObject.localPosition = script.CurrentWeaponController
											.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex].LeftElbowPosition;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.LeftElbowObject.localPosition = script.currentController.DirectionObject.position - script.currentController.DirectionObject.right * 2;
									}
									
									if (curInfo.RightElbowPosition != Vector3.zero)
									{
										script.CurrentWeaponController.IkObjects.RightElbowObject.localPosition =
											curInfo.RightElbowPosition;
									}
									else
									{
										script.CurrentWeaponController.IkObjects.RightElbowObject.localPosition = script.currentController.DirectionObject.position + script.currentController.DirectionObject.right * 2;
									}
								}
							}

							if (GUILayout.Button("Set default values"))
							{
								if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
								{
									script.CurrentWeaponController.IkObjects.RightObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
									script.CurrentWeaponController.IkObjects.RightObject.localPosition = Vector3.zero;
									script.CurrentWeaponController.IkObjects.RightObject.localRotation = Quaternion.Euler(-90, 0, 0);

									script.CurrentWeaponController.IkObjects.LeftObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
									script.CurrentWeaponController.IkObjects.LeftObject.localPosition = Vector3.zero;
									script.CurrentWeaponController.IkObjects.LeftObject.localRotation = Quaternion.Euler(-90, 0, 0);
									
									script.CurrentWeaponController.IkObjects.RightCrouchObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
									script.CurrentWeaponController.IkObjects.RightCrouchObject.localPosition = Vector3.zero;
									script.CurrentWeaponController.IkObjects.RightCrouchObject.localRotation = Quaternion.Euler(-90, 0, 0);

									script.CurrentWeaponController.IkObjects.LeftCrouchObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
									script.CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition = Vector3.zero;
									script.CurrentWeaponController.IkObjects.LeftCrouchObject.localRotation = Quaternion.Euler(-90, 0, 0);

									script.CurrentWeaponController.IkObjects.RightAimObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
									script.CurrentWeaponController.IkObjects.RightAimObject.localPosition = Vector3.zero;
									script.CurrentWeaponController.IkObjects.RightAimObject.localRotation = Quaternion.Euler(-90, 0, 0);

									script.CurrentWeaponController.IkObjects.LeftAimObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
									script.CurrentWeaponController.IkObjects.LeftAimObject.localPosition = Vector3.zero;
									script.CurrentWeaponController.IkObjects.LeftAimObject.localRotation = Quaternion.Euler(-90, 0, 0);

									script.CurrentWeaponController.IkObjects.RightWallObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
									script.CurrentWeaponController.IkObjects.RightWallObject.localPosition = Vector3.zero;
									script.CurrentWeaponController.IkObjects.RightWallObject.localRotation = Quaternion.Euler(-90, 0, 0);

									script.CurrentWeaponController.IkObjects.LeftWallObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
									script.CurrentWeaponController.IkObjects.LeftWallObject.localPosition = Vector3.zero;
									script.CurrentWeaponController.IkObjects.LeftWallObject.localRotation = Quaternion.Euler(-90, 0, 0);

									script.CurrentWeaponController.IkObjects.LeftElbowObject.localPosition = script.currentController.DirectionObject.position - script.currentController.DirectionObject.right * 2;
									script.CurrentWeaponController.IkObjects.RightElbowObject.localPosition = script.currentController.DirectionObject.position + script.currentController.DirectionObject.right * 2;
								}
//

								curInfo = new WeaponsHelper.WeaponInfo();
								
//								curInfo.FingersLeftX = 0;
//								curInfo.FingersRightX = 0;
//
//								curInfo.FingersLeftY = 0;
//								curInfo.FingersRightY = 0;
//
//								curInfo.FingersLeftZ = 0;
//								curInfo.FingersRightZ = 0;
//
//								curInfo.ThumbLeftX = 0;
//								curInfo.ThumbRightX = 0;
//
//								curInfo.ThumbLeftY = 0;
//								curInfo.ThumbRightY = 0;
//
//								curInfo.ThumbLeftZ = 0;
//								curInfo.ThumbRightZ = 0;
//
//								curInfo.WeaponPosition = Vector3.zero;
//								curInfo.WeaponRotation = Vector3.zero;
//								curInfo.WeaponSize = Vector3.one;

								script.CurrentWeaponController.transform.localPosition = curInfo.WeaponPosition;
								script.CurrentWeaponController.transform.localEulerAngles = curInfo.WeaponRotation;
								
								if (curInfo.WeaponSize != Vector3.zero)
									script.CurrentWeaponController.transform.localScale = curInfo.WeaponSize;
								else
									script.CurrentWeaponController.transform.localScale = script.currentScales[script.weaponIndex];

							}
						}

					}

					#endregion

					break;
			}

			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();

			if (script.SerializedWeaponController != null)
				script.SerializedWeaponController.ApplyModifiedProperties();

//			DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);

				if (script.CurrentWeaponController)
					EditorUtility.SetDirty(script.CurrentWeaponController);

				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}

		}

		void SaveData()
		{
			script.CurrentWeaponController.OriginalScript.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex]
				.Clone(script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.SettingsSlotIndex]);
			
			EditorUtility.SetDirty(script.CurrentWeaponController.OriginalScript);
		}
	}
}
