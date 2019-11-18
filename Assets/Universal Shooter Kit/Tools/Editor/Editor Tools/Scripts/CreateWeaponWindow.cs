// GercStudio
// © 2018-2019

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace GercStudio.USK.Scripts
{
	public class CreateWeaponWindow : EditorWindow
	{
		public GameObject WeaponModel;

		public WeaponParameters parameters;
		private WeaponParameters tempParameters;

		private float startVal;
		private float progress;

		private bool weaponError;
		private bool shellError;
		private bool imageError;
		private bool muzzleFlashError;
		private bool fireError;
		private bool rocketError;
		private bool bulletTracerError;
		private bool animationsError;
		private bool audioError;
		private bool explosionError;
		//private bool fontError;

		private bool weaponErrorGUI;
		private bool shellErrorGUI;
		private bool imageErrorGUI;
		private bool muzzleFlashErrorGUI;
		private bool fireErrorGUI;
		private bool rocketErrorGUI;
		private bool bulletTracerErrorGUI;
		private bool animationsErrorGUI;
		private bool audioErrorGUI;
		private bool explosionErrorGUI;
		//private bool fontErrorGUI;

		private bool WeaponAdded;
		private bool hasCreated;
		private bool startCreation;
		private bool createAnyway;
		private bool characterError;
		private bool CharacterAdded;

		private Font font;

		private GUIStyle LabelStyle;

		private Vector2 scrollPos;

		[MenuItem("Window/USK/Create/Weapon")]
		public static void ShowWindow()
		{
			GetWindow(typeof(CreateWeaponWindow), true, "", true).ShowUtility();
		}

		void OnEnable()
		{
			EditorApplication.update += Update;
		}

		void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		private void Awake()
		{
			if(!font)
				font = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
			
			if (LabelStyle != null) return;

			LabelStyle = new GUIStyle
			{
				normal = {textColor = Color.black},
				fontStyle = FontStyle.Bold,
				fontSize = 12,
				alignment = TextAnchor.MiddleCenter
			};
		}

		void Update()
		{
			if (WeaponModel)
			{
				if (!WeaponAdded)
				{
					ResetErrors();
//					if (!WeaponModel.activeInHierarchy)
//						WeaponModel = Instantiate(WeaponModel, Vector3.zero, Quaternion.Euler(Vector3.zero));
					WeaponAdded = true;
				}
			}
			else
			{
				ResetErrors();
				ResetVariables();
			}

			if (parameters)
			{
//				if (parameters.WeaponType == WeaponParameters.CreationWeaponType.Grenade)
//				{
//					if (Character)
//					{
//						if (!CharacterAdded)
//						{
//							ResetErrors();
//							if (!Character.activeInHierarchy & Character.GetComponent<InventoryManager>())
//								Character = Instantiate(Character, Vector3.zero, Quaternion.Euler(Vector3.zero));
//							CharacterAdded = true;
//						}
//						else
//						{
//							if (!Character.GetComponent<InventoryManager>())
//							{
//								characterError = true;
//								Character = null;
//							}
//							else
//							{
//								characterError = false;
//							}
//						}
//					}
//					else
//					{
//						ResetErrors();
//						ResetVariables();
//						CharacterAdded = false;
//					}
//				}
			}
			else
			{
				ResetErrors();
				ResetVariables();
			}

			if (startCreation & progress > 1.1f)
			{
				ResetErrors();
				GetAnimations();
				GetVariables();

//				if (parameters.WeaponType == WeaponParameters.CreationWeaponType.Grenade)
//				{
//					//CheckFont();
//
//					
//						if (!audioError & !animationsError & !explosionError || createAnyway)
//						{
//							CreateGrenade();
//							hasCreated = true;
//							createAnyway = false;
//							startVal = (float) EditorApplication.timeSinceStartup;
//						}
//
//				}
//				else
//				{
				if (!fireError && !bulletTracerError && !rocketError && !muzzleFlashError && !shellError && !imageError && !audioError &&
				    !animationsError && !explosionError || createAnyway)
				{
					ManageParent();
					AddScripts();
					SetVariabales();
					CreateObjects();
					SaveWeaponToPrefab();

					hasCreated = true;
					createAnyway = false;
					startVal = (float) EditorApplication.timeSinceStartup;
				}
//				}

				startCreation = false;
			}

			if (hasCreated)
			{
				ResetErrors();

				if (progress > 10)
				{
					hasCreated = false;

					WeaponModel = null;
					parameters = null;
					ResetVariables();
				}
			}

		}

		private void OnGUI()
		{
			scrollPos = 
				EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width),
					GUILayout.Height(position.height));


			EditorGUILayout.Space();
			GUILayout.Label("Create Weapon", LabelStyle);
			EditorGUILayout.Space();
			if (hasCreated)
			{
				GUIStyle style = new GUIStyle
				{
					normal = {textColor = Color.green}, fontStyle = FontStyle.Bold, fontSize = 10, alignment = TextAnchor.MiddleCenter
				};
				EditorGUILayout.LabelField("Weapon has been created", style);
				
				
				EditorGUILayout.HelpBox("Open the Adjustment scene to regulate your character" + "\n" + "[Window -> USK -> Adjust]" +"\n\n" +
				                        "To use this weapon in a game move it to the [Inventory] script (on your character).", MessageType.Info);
				

				EditorGUILayout.Space();
			}

			EditorGUILayout.BeginVertical("box");
			WeaponModel =
				(GameObject) EditorGUILayout.ObjectField("Weapon Model", WeaponModel, typeof(GameObject), true);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical("box");
			if (parameters == null)
			{
				EditorGUILayout.HelpBox(
					"Please set weapon preset from [Universal Shooter Kit / Presets / Weapons] or create your own.",
					MessageType.Info);
			}

			parameters = (WeaponParameters) EditorGUILayout.ObjectField("Weapon Preset", parameters, typeof(WeaponParameters), false);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();

			if (WeaponModel & parameters)
			{
				//!!!!!!
				if (parameters.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
				{
					if (animationsErrorGUI)
					{
						EditorGUILayout.BeginVertical("box");
						
						if (animationsError)
							EditorGUILayout.HelpBox("All animations not found. Set them yourself from", MessageType.Warning);

						parameters.characterAnimations.WeaponIdle = (AnimationClip) EditorGUILayout.ObjectField("Idle", parameters.characterAnimations.WeaponIdle, typeof(AnimationClip), false);
						parameters.characterAnimations.WeaponWalk = (AnimationClip) EditorGUILayout.ObjectField("Walk", parameters.characterAnimations.WeaponWalk, typeof(AnimationClip), false);
						parameters.characterAnimations.WeaponRun = (AnimationClip) EditorGUILayout.ObjectField("Run", parameters.characterAnimations.WeaponRun, typeof(AnimationClip), false);
						parameters.characterAnimations.TakeWeapon = (AnimationClip) EditorGUILayout.ObjectField("Take from inventory", parameters.characterAnimations.TakeWeapon, typeof(AnimationClip), false);
						EditorGUILayout.Space();
						
						for (var i = 0; i < parameters.Attacks.Count; i++)
						{
							var attack = parameters.Attacks[i];
							
							EditorGUILayout.LabelField("Attack: " + parameters.attacksNames[i], EditorStyles.boldLabel);
//                              
							if (attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets)
							{
								attack.WeaponAttack  = (AnimationClip) EditorGUILayout.ObjectField("Single shot", attack.WeaponAttack , typeof(AnimationClip), false);
								attack.WeaponAutoShoot  = (AnimationClip) EditorGUILayout.ObjectField("Auto Shoot", attack.WeaponAutoShoot , typeof(AnimationClip), false);
							}
							else
							{
								attack.WeaponAttack  = (AnimationClip) EditorGUILayout.ObjectField("Attack", attack.WeaponAttack , typeof(AnimationClip), false);
							}


							if (attack.AttackType != WeaponsHelper.TypeOfAttack.Knife)
							{
								attack.WeaponReload  = (AnimationClip) EditorGUILayout.ObjectField("Weapon Reload", attack.WeaponReload , typeof(AnimationClip), false);
							}
//  
							if (i < parameters.Attacks.Count - 1)
								EditorGUILayout.Space();
						}

						
//						if (parameters.attackType != WeaponsHelper.TypeOfAttack.Knife)
//						{
//							WeaponReload = (AnimationClip) EditorGUILayout.ObjectField("WeaponReload", 
//								WeaponReload, typeof(AnimationClip), false);
//						}

//						TakeWeapon = (AnimationClip) EditorGUILayout.ObjectField("TakeWeapon", TakeWeapon, typeof(AnimationClip), false);
//						WeaponWalk = (AnimationClip) EditorGUILayout.ObjectField("WeaponWalk", WeaponWalk, typeof(AnimationClip), false);
//						WeaponRun = (AnimationClip) EditorGUILayout.ObjectField("WeaponRun", WeaponRun, typeof(AnimationClip), false);
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						//!!
//						if (parameters.attackType != WeaponsHelper.TypeOfAttack.Knife)
//						{
//							if (WeaponIdle & WeaponAttack & WeaponReload & TakeWeapon & WeaponWalk & WeaponRun)
//								animationsError = false;
//							else animationsError = true;
//						}
//						else
//						{
//							if (WeaponIdle & WeaponAttack & TakeWeapon & WeaponWalk & WeaponRun)
//								animationsError = false;
//							else animationsError = true;
//						}
//!!
					}

					if (audioErrorGUI)
					{
						EditorGUILayout.BeginVertical("box");
						
						if (audioError)
							EditorGUILayout.HelpBox("Not all sounds are set. Please set them now or later in [WeaponController] script.", MessageType.Warning);
						
						
							parameters.PickUpWeaponAudio = (AudioClip) EditorGUILayout.ObjectField("Pickup", parameters.PickUpWeaponAudio, typeof(AudioClip), false);
							parameters.DropWeaponAudio = (AudioClip) EditorGUILayout.ObjectField("Drop", parameters.DropWeaponAudio, typeof(AudioClip), false);
							
							EditorGUILayout.Space();
							for (var i = 0; i < parameters.Attacks.Count; i++)
							{
								var attack = parameters.Attacks[i];

								EditorGUILayout.LabelField("Attack: " + parameters.attacksNames[i], EditorStyles.boldLabel);
								
								attack.AttackAudio = (AudioClip) EditorGUILayout.ObjectField("Attack audio", attack.AttackAudio, typeof(AudioClip), false);

								if (attack.AttackType != WeaponsHelper.TypeOfAttack.Knife)
								{
									attack.ReloadAudio = (AudioClip) EditorGUILayout.ObjectField("Reload", attack.ReloadAudio, typeof(AudioClip), false);
									attack.NoAmmoShotAudio = (AudioClip) EditorGUILayout.ObjectField("Attack without ammo", attack.NoAmmoShotAudio, typeof(AudioClip), false);
								
								}
								if (i < parameters.Attacks.Count - 1)
									EditorGUILayout.Space();
							}

//						AttackAudio = (AudioClip) EditorGUILayout.ObjectField("Attack audio", AttackAudio, typeof(AudioClip), false);
//						DropAudio = (AudioClip) EditorGUILayout.ObjectField("Drop audio", DropAudio, typeof(AudioClip), false);
//						PickupAudio = (AudioClip) EditorGUILayout.ObjectField("Pick up audio", PickupAudio, typeof(AudioClip), false);
				
//						if (parameters.attackType != WeaponsHelper.TypeOfAttack.Knife)
//						{
//							ReloadAudio = (AudioClip) EditorGUILayout.ObjectField("Reload audio",
//								ReloadAudio, typeof(AudioClip), false);
//							ShootWithoutAmmoAudio = (AudioClip) EditorGUILayout.ObjectField("Shoot with ammo audio",
//								ShootWithoutAmmoAudio, typeof(AudioClip), false);
//						}

						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();

						//!!
//						if (parameters.attackType != WeaponsHelper.TypeOfAttack.Knife)
//						{
//							if (ReloadAudio & AttackAudio & PickupAudio & DropAudio & ShootWithoutAmmoAudio)
//								audioError = false;
//							else audioError = true;
//						}
//						else
//						{
//							if (AttackAudio & PickupAudio & DropAudio)
//								audioError = false;
//							else audioError = true;
//						}
//!!
					}

					if (shellErrorGUI)
					{
						
						EditorGUILayout.BeginVertical("box");
						if (shellError)
							EditorGUILayout.HelpBox("Shell prefab is not set. Please set them now or later in [WeaponController] script.", MessageType.Warning);
						
						for (var i = 0; i < parameters.Attacks.Count; i++)
						{
							var attack = parameters.Attacks[i];

							EditorGUILayout.LabelField("Attack: " + parameters.attacksNames[i], EditorStyles.boldLabel);
							
							attack.Shell = (GameObject) EditorGUILayout.ObjectField("Shell (prefab)", attack.Shell, typeof(GameObject), false);
							
						}
						
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						shellError = parameters.Attacks.Any(attack => !attack.Shell);
					}
					
					if (imageErrorGUI)
					{
						EditorGUILayout.BeginVertical("box");
						if (imageError)
							EditorGUILayout.HelpBox(
								"Image texture for inventory is not set. Please set them now or later in [WeaponController] script.",
								MessageType.Warning);

						parameters.WeaponImage = (Texture) EditorGUILayout.ObjectField("Image (texture)", parameters.WeaponImage, typeof(Texture), false);
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						imageError = !parameters.WeaponImage;
					}

					if (muzzleFlashErrorGUI)
					{
						EditorGUILayout.BeginVertical("box");
						if (muzzleFlashError)
							EditorGUILayout.HelpBox("Muzzle Flash prefab is not set. Please set them now or later in [WeaponController] script.", MessageType.Warning);

						for (var i = 0; i < parameters.Attacks.Count; i++)
						{
							var attack = parameters.Attacks[i];
							EditorGUILayout.LabelField("Attack: " + parameters.attacksNames[i], EditorStyles.boldLabel);
							attack.MuzzleFlash = (GameObject) EditorGUILayout.ObjectField("Muzzle Flash (prefab)", attack.MuzzleFlash, typeof(GameObject), false);
						}

						
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						muzzleFlashError = parameters.Attacks.Any(attack => !attack.MuzzleFlash);
					}

					if (rocketErrorGUI)
					{
						EditorGUILayout.BeginVertical("box");
						if (rocketError)
							EditorGUILayout.HelpBox("Rocket prefab is not set. Please set them now or later in [WeaponController] script.", MessageType.Warning);

						for (var i = 0; i < parameters.Attacks.Count; i++)
						{
							var attack = parameters.Attacks[i];
							EditorGUILayout.LabelField("Attack: " + parameters.attacksNames[i], EditorStyles.boldLabel);
							attack.Rocket = (GameObject) EditorGUILayout.ObjectField("Rocket (prefab)", attack.Rocket, typeof(GameObject), false);
						}

						
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						rocketError = parameters.Attacks.Any(attack => !attack.Rocket);
					}

					if (bulletTracerErrorGUI)
					{
						EditorGUILayout.BeginVertical("box");
						if (bulletTracerError)
							EditorGUILayout.HelpBox("Bullet Tracer is prefab not set. Please set them now or later in [WeaponController] script.", MessageType.Warning);

						for (var i = 0; i < parameters.Attacks.Count; i++)
						{
							var attack = parameters.Attacks[i];
							EditorGUILayout.LabelField("Attack: " + parameters.attacksNames[i], EditorStyles.boldLabel);
							attack.Tracer = (GameObject) EditorGUILayout.ObjectField("Bullet Tracer (prefab)", attack.Tracer, typeof(GameObject), false);
						}

						
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						bulletTracerError = parameters.Attacks.Any(attack => !attack.Tracer);
					}

					if (fireErrorGUI)
					{
						EditorGUILayout.BeginVertical("box");
						if (fireError)
							EditorGUILayout.HelpBox("Fire prefab is not set. Please set them now or later in [WeaponController] script.", MessageType.Warning);

						for (var i = 0; i < parameters.Attacks.Count; i++)
						{
							var attack = parameters.Attacks[i];
							EditorGUILayout.LabelField("Attack: " + parameters.attacksNames[i], EditorStyles.boldLabel);
							attack.Fire = (GameObject) EditorGUILayout.ObjectField("Fire (prefab)", attack.Fire, typeof(GameObject), false);
						}

						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						fireError = parameters.Attacks.Any(attack => !attack.Fire);
					}
				}
				else
				{
//					if (characterError)
//					{
//						EditorGUILayout.HelpBox(
//							"Character must have [WeaponManager] script.", MessageType.Warning);
//					}
//
//					if (Character == null & !characterError)
//					{
//						EditorGUILayout.HelpBox("Please set character.", MessageType.Info);
//					}
//
//					Character =
//						(GameObject) EditorGUILayout.ObjectField("Character", Character, typeof(GameObject),
//							true);
//					EditorGUILayout.EndVertical();
//					EditorGUILayout.Space();
//
//					if (Character)
//					{
						if (animationsErrorGUI)
						{
							EditorGUILayout.BeginVertical("box");
							if (animationsError)
								EditorGUILayout.HelpBox("Grenade animations is not set.", MessageType.Warning);

							parameters.GrenadeParameters.GrenadeThrow_FPS = (AnimationClip) EditorGUILayout.ObjectField("Throw", parameters.GrenadeParameters.GrenadeThrow_FPS, typeof(AnimationClip), false);
							parameters.GrenadeParameters.GrenadeThrow_TPS_TDS = (AnimationClip) EditorGUILayout.ObjectField("Throw without weapons", parameters.GrenadeParameters.GrenadeThrow_TPS_TDS, typeof(AnimationClip), false);
							
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();

							animationsError = !parameters.GrenadeParameters.GrenadeThrow_FPS || parameters.GrenadeParameters.GrenadeThrow_TPS_TDS;
						}

						if (audioErrorGUI)
						{
							EditorGUILayout.BeginVertical("box");
							if (audioError)
								EditorGUILayout.HelpBox("Grenade throw audio is not set.", MessageType.Warning);

							parameters.GrenadeParameters.ThrowAudio = (AudioClip) EditorGUILayout.ObjectField("Throw", parameters.GrenadeParameters.ThrowAudio, typeof(AudioClip), false);
							
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
							
							audioError = !parameters.GrenadeParameters.ThrowAudio;
						}

						if (explosionErrorGUI)
						{
							EditorGUILayout.BeginVertical("box");
							if (explosionError)
								EditorGUILayout.HelpBox("Explosion is not set.", MessageType.Warning);
							parameters.GrenadeParameters.GrenadeExplosion = (GameObject) EditorGUILayout.ObjectField("Explosion", parameters.GrenadeParameters.GrenadeExplosion, typeof(GameObject), false);
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
							explosionError = !parameters.GrenadeParameters.GrenadeExplosion ;
						}

//						if (fontErrorGUI)
//						{
//							EditorGUILayout.Space();
//							EditorGUILayout.BeginVertical("box");
//
//							if (fontError)
//								EditorGUILayout.HelpBox(
//									"Standard font for the UI was not found, please set some other.",
//									MessageType.Warning);
//
//							font = (Font) EditorGUILayout.ObjectField("Font", font, typeof(Font), false);
//							EditorGUILayout.EndVertical();
//							EditorGUILayout.Space();
//
//							fontError = !font;
//
//						}
//					}
				}

				if (startCreation)
				{
//					EditorGUI.ProgressBar(
//						parameters.WeaponType != WeaponsHelper.TypeOfWeapon.Grenade
//							? new Rect(3, 100, position.width - 6, 20)
//							: new Rect(3, 130, position.width - 6, 20), progress / 1, "Creation...");

					EditorGUI.ProgressBar(new Rect(3, 100, position.width - 6, 20), progress / 1, "Creation...");
				}
			}
			else
			{
				WeaponAdded = false;
			}


			EditorGUI.BeginDisabledGroup(!WeaponModel || !parameters);

//			if (parameters)
//				if (parameters.WeaponType == WeaponParameters.CreationWeaponType.Grenade)
//					EditorGUI.BeginDisabledGroup(!Character);

			if (!fireError & !bulletTracerError & !rocketError & !muzzleFlashError & !shellError & !imageError & !audioError &
			    !animationsError & !explosionError)
			{
				if (!startCreation & !hasCreated)
					if (GUILayout.Button("Create"))
					{
						tempParameters = parameters;
						ResetErrors();
						startVal = (float) EditorApplication.timeSinceStartup;
						startCreation = true;
					}
			}
			else
			{
				if (!startCreation & !hasCreated)
				{
					if (parameters != tempParameters)
					{
						ResetErrors();
						ResetVariables();
						tempParameters = parameters;
						return;
					}

					if (GUILayout.Button("Create anyway"))
					{
						ResetErrors();
						startVal = (float) EditorApplication.timeSinceStartup;
						startCreation = true;
						createAnyway = true;
					}
				}
			}

//			if (parameters)
//				if (parameters.WeaponType == WeaponParameters.CreationWeaponType.Grenade)
//					EditorGUI.EndDisabledGroup();

			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndScrollView();

			progress = (float) (EditorApplication.timeSinceStartup - startVal);
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}

//		void CheckFont()
//		{
//			if (!font)
//			{
//				fontError = true;
//				fontErrorGUI = true;
//				font = AssetDatabase.LoadAssetAtPath(
//					"Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
//
//				if (font)
//				{
//					fontError = false;
//					fontErrorGUI = false;
//				}
//			}
//		}


		void ResetErrors()
		{
			shellError = false;
			imageError = false;
			muzzleFlashError = false;
			fireError = false;
			rocketError = false;
			bulletTracerError = false;
			animationsError = false;
			audioError = false;
			explosionError = false;
//			fontError = false;
//			fontErrorGUI = false;
			shellErrorGUI = false;
			imageErrorGUI = false;
			muzzleFlashErrorGUI = false;
			fireErrorGUI = false;
			rocketErrorGUI = false;
			bulletTracerErrorGUI = false;
			animationsErrorGUI = false;
			audioErrorGUI = false;
			explosionErrorGUI = false;
		}

		void ResetVariables()
		{
//			Shell = null;
//			weaponImage = null;
//			Rocket = null;
//			Fire = null;
//			Tracer = null;
//			MuzzleFlash = null;
//			AttackAudio = null;
//			PickupAudio = null;
//			DropAudio = null;
//			ShootWithoutAmmoAudio = null;
//			ReloadAudio = null;
//			GrenadeThrowAudio = null;
//			WeaponIdle = null;
//			WeaponAttack = null;
//			WeaponReload = null;
//			TakeWeapon = null;
//			WeaponWalk = null;
//			WeaponRun = null;
//			GrenadeThrow = null;
//			GrenadeThrowWithoutWeapons = null;
//			Explosion = null;
		}

		void CreateGrenade()
		{
//			if (!WeaponModel.GetComponent<Grenade>())
//				WeaponModel.AddComponent<Grenade>();
//
//			if (Explosion)
//				WeaponModel.GetComponent<Grenade>().Explosion = Explosion.transform;
//
//			InventoryManager manager = Character.GetComponent<InventoryManager>();
//
//			manager.activeGrenades = true;
//
//			manager.GrenadeSpeed = parameters.GrenadeSpeed;
//			manager.grenadeDamage = parameters.grenadeDamage;
//			manager.GrenadeAmmo = parameters.GrenadeAmmo;
//			manager.ExplosionTime = parameters.ExplosionTime;
//			manager.ThrowAudio = ThrowAudio;
//			manager.GrenadeThrow = GrenadeThrow;
//			manager.GrenadeThrowWithoutWeapons = GrenadeThrowWithoutWeapons;
//
//			Object prefab =
//				PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/_Weapons/" + WeaponModel.name +
//				                                ".prefab");
//			PrefabUtility.ReplacePrefab(WeaponModel, prefab, ReplacePrefabOptions.ConnectToPrefab);
//
//			manager.GrenadePrefabs = AssetDatabase.LoadAssetAtPath(
//				"Assets/Universal Shooter Kit/Prefabs/_Weapons/" + WeaponModel.name + ".prefab",
//				typeof(GameObject)) as GameObject;
//			;
//
//			if (manager.canvas & !manager.GrenadeAmmoUI)
//			{
//				GameObject GrenadeInt = Helper.NewText("Grenade(Int)", manager.canvas.transform, new Vector2(-600, 330),
//					new Vector2(200, 50), "10", font, 45,
//					TextAnchor.UpperLeft, Color.green);
//
//				Helper.NewText("Grenade(Text)", GrenadeInt.transform, new Vector2(-265, 0), new Vector2(300, 50),
//					"Grens", font, 45, TextAnchor.UpperRight, Color.white);
//
//				manager.GrenadeAmmoUI = GrenadeInt.GetComponent<Text>();
//			}
//
//			DestroyImmediate(WeaponModel);
		}

		void CreateObjects()
		{
			WeaponController controller = WeaponModel.GetComponent<WeaponController>();

			foreach (var attack in controller.Attacks)
			{
				if (attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets && !attack.ShellPoint)
				{
					attack.ShellPoint = controller.Attacks.Any(_attack => _attack.ShellPoint)
						? controller.Attacks.Find(_attack => _attack.ShellPoint).ShellPoint
						: Helper.NewPoint(controller.gameObject, "Shell Spawn Point");
				}

				if (attack.AttackType != WeaponsHelper.TypeOfAttack.Knife & !attack.AttackSpawnPoint)
				{
					attack.AttackSpawnPoint = controller.Attacks.Any(_attack => _attack.AttackSpawnPoint)
						? controller.Attacks.Find(_attack => _attack.AttackSpawnPoint).AttackSpawnPoint
						: Helper.NewPoint(controller.gameObject, "Attack Point");
				}


				if (attack.AttackType == WeaponsHelper.TypeOfAttack.Knife)
				{
					attack.AttackCollider = controller.Attacks.Any(_attack => _attack.AttackCollider && _attack.AttackType == WeaponsHelper.TypeOfAttack.Knife)
						? controller.Attacks.Find(_attack => _attack.AttackCollider && _attack.AttackType == WeaponsHelper.TypeOfAttack.Knife).AttackCollider
						: Helper.NewCollider("Knife Collider", "KnifeCollider", WeaponModel.transform);
				}

				if (attack.AttackType == WeaponsHelper.TypeOfAttack.Flame)
				{
					attack.AttackCollider = controller.Attacks.Any(_attack => _attack.AttackCollider && _attack.AttackType == WeaponsHelper.TypeOfAttack.Flame)
						? controller.Attacks.Find(_attack => _attack.AttackCollider && _attack.AttackType == WeaponsHelper.TypeOfAttack.Flame).AttackCollider
						: Helper.NewCollider("Fire Collider", "Fire", WeaponModel.transform);

				}
			}
		}
	


		

		void GetAnimations()
		{
			if (parameters.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
			{
				if (!parameters.characterAnimations.WeaponIdle)
				{
					animationsError = true;
					animationsErrorGUI = true;
				}

				if (parameters.Attacks.Any(attack => !attack.WeaponAttack))
				{
					animationsError = true;
					animationsErrorGUI = true;
				}

				if (parameters.Attacks.Any(attack => !attack.WeaponReload && attack.AttackType != WeaponsHelper.TypeOfAttack.Knife))
				{
					animationsError = true;
					animationsErrorGUI = true;
				}

				if (!parameters.characterAnimations.TakeWeapon)
				{
					animationsError = true;
					animationsErrorGUI = true;
				}


				if (!parameters.characterAnimations.WeaponWalk)
				{
					animationsError = true;
					animationsErrorGUI = true;
				}

				if (!parameters.characterAnimations.WeaponRun)
				{
					animationsError = true;
					animationsErrorGUI = true;
				}
			}
			else
			{
				if (!parameters.GrenadeParameters.GrenadeThrow_FPS)
				{
					animationsError = true;
					animationsErrorGUI = true;
				}

				if (!parameters.GrenadeParameters.GrenadeThrow_TPS_TDS)
				{
					animationsError = true;
					animationsErrorGUI = true;
				}
			}
		}

		void AddScripts()
		{
			if (!WeaponModel.GetComponent<WeaponController>())
				WeaponModel.AddComponent<WeaponController>();

			if (!WeaponModel.GetComponent<Rigidbody>()) return;
			WeaponModel.GetComponent<Rigidbody>().useGravity = false;
			WeaponModel.GetComponent<Rigidbody>().isKinematic = true;
		}

		void ManageParent()
		{
			WeaponModel = Instantiate(WeaponModel, Vector3.zero, Quaternion.Euler(Vector3.zero));
			
			if (WeaponModel.GetComponent<Animator>())
				DestroyImmediate(WeaponModel.GetComponent<Animator>());

			var parent = new GameObject().transform;
			parent.name = WeaponModel.name;
			parent.parent = WeaponModel.transform;
			parent.localPosition = Vector3.zero;
			parent.localRotation = Quaternion.Euler(Vector3.zero);
			parent.parent = null;
			WeaponModel.transform.parent = parent;
			WeaponModel.name = "Render";
			WeaponModel = parent.gameObject;
		}

		void GetVariables()
		{

			if (parameters.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
			{
				if (parameters.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Rockets && !attack.Rocket))
				{
					rocketError = true;
					rocketErrorGUI = true;
				}

				if (parameters.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Flame && !attack.Fire))
				{
					fireError = true;
					fireErrorGUI = true;
				}

				if (parameters.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets && !attack.Tracer))
				{
					bulletTracerError = true;
					bulletTracerErrorGUI = true;
				}

				if (parameters.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets && !attack.MuzzleFlash))
				{
					muzzleFlashError = true;
					muzzleFlashErrorGUI = true;
				}

				if (parameters.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets && !attack.Shell))
				{
					shellError = true;
					shellErrorGUI = true;
				}


				if (parameters.Attacks.Any(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Knife && !attack.ReloadAudio || !attack.NoAmmoShotAudio))
				{
					audioError = true;
					audioErrorGUI = true;
				}

				if (!parameters.WeaponImage)
				{
					imageError = true;
					imageErrorGUI = true;
				}

				if (parameters.Attacks.Any(attack => !attack.AttackAudio))
				{
					audioError = true;
					audioErrorGUI = true;
				}

				if (!parameters.DropWeaponAudio || !parameters.PickUpWeaponAudio)
				{
					audioError = true;
					audioErrorGUI = true;
				}
			}
			else
			{
				if (!parameters.GrenadeParameters.ThrowAudio)
				{
					audioError = true;
					audioErrorGUI = true;
				}

				if (!parameters.GrenadeParameters.GrenadeExplosion)
				{
					explosionError = true;
					explosionErrorGUI = true;
				}
			}
		}

		void SaveWeaponToPrefab()
		{
			if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/_Weapons/"))
			{
				Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/_Weapons/");
			}
			
			var index = 0;
			while(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_Weapons/" + WeaponModel.name + " " + index + ".prefab", typeof(GameObject)) != null)
			{
				index++;
			}

#if !UNITY_2018_3_OR_NEWER
			var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/_Weapons/" + WeaponModel.name + " " + index + ".prefab");
			PrefabUtility.ReplacePrefab(WeaponModel, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
			PrefabUtility.SaveAsPrefabAsset(WeaponModel, "Assets/Universal Shooter Kit/Prefabs/_Weapons/" + WeaponModel.name + " " + index + ".prefab");
#endif

			EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_Weapons/" + WeaponModel.name + " " + index + ".prefab",
				typeof(GameObject)));

			DestroyImmediate(WeaponModel);
			
		}

		void SetVariabales()
		{
			var weaponController = WeaponModel.GetComponent<WeaponController>();

			//weaponController.Attacks = new List<WeaponsHelper.Attack> {new WeaponsHelper.Attack()};
//			weaponController.Attacks[0].BulletsSettings = new List<WeaponsHelper.BulletsSettings>
//			{
//				new WeaponsHelper.BulletsSettings(), new WeaponsHelper.BulletsSettings()
//			};

//			weaponController.attacksNames = new List<string> {"Attack 0"};
			
//			weaponController.WeaponInfos = new List<WeaponsHelper.WeaponInfo> {new WeaponsHelper.WeaponInfo()};
//			weaponController.enumNames = new List<string> {"Slot 1"};

			weaponController.Attacks.Clear();
			weaponController.attacksNames.Clear();
			
			for (var i = 0; i < parameters.Attacks.Count; i++)
			{
				var attack = parameters.Attacks[i];
				weaponController.Attacks.Add(attack);
				weaponController.attacksNames.Add(parameters.attacksNames[i]);
			}

			if (weaponController.Attacks.Count == 0)
			{
				weaponController.Attacks.Add(new WeaponsHelper.Attack());
				weaponController.attacksNames.Add("Attack 0");
			}

			weaponController.ActiveAimTPS = parameters.ActiveAim;
			weaponController.ActiveAimFPS = parameters.ActiveAim;
			
			if(parameters.WeaponImage)
				weaponController.WeaponImage = parameters.WeaponImage;
			
			
			if (parameters.AimImage)
			{
				weaponController.UseAimTexture = true;
				weaponController.AimCrosshairTexture = parameters.AimImage;
			}

			if(parameters.DropWeaponAudio)
				weaponController.DropWeaponAudio = parameters.DropWeaponAudio;
			
			if(parameters.PickUpWeaponAudio)
				weaponController.PickUpWeaponAudio = parameters.PickUpWeaponAudio;

			weaponController.characterAnimations = parameters.characterAnimations;

			weaponController.GrenadeParameters = parameters.GrenadeParameters;
			
			weaponController.inputs = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(Inputs)) as Inputs;
		}
	}
}
