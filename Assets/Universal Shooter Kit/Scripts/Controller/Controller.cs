// GercStudio
// © 2018-2019

using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	[RequireComponent(typeof(Animator))]
	public class Controller : MonoBehaviour
	{
		public CharacterController CharacterController;
		public InventoryManager WeaponManager;
		public Controller OriginalScript;

		public CharacterHelper.CameraType TypeOfCamera;
		public CharacterHelper.CameraParameters CameraParameters;
		
		public Animator anim;

		public AudioSource leftAudioSource;
		public AudioSource rightAudioSource;

		public Helper.AnimationClipOverrides ClipOverrides;

		public CharacterHelper.CharacterOffset CharacterOffset;
		
		[Range(10, 40)] public float JumpSpeed = 20;
		[Range(2, 20)] public float JumpHeight = 2;
		
		[Range(1, 100)] public float PlayerHealth = 100;

		public float NormForwardSpeed = 4;
		public float NormBackwardSpeed = 3;
		public float NormLateralSpeed = 3;
		public float RunForwardSpeed = 8;
		public float RunBackwardSpeed = 6;
		public float RunLateralSpeed = 6;
		public float CrouchForwardSpeed = 2;
		public float CrouchBackwardSpeed = 1.5f;
		public float CrouchLateralSpeed = 1.5f;

		[Range(0,50)] public int CrouchIdleNoise;
		[Range(0,50)] public int CrouchMovementNoise;
		[Range(0,50)] public int SprintMovementNoise;
		[Range(0,50)] public int MovementNoise;
		[Range(0,50)] public int IdleNoise;
		[Range(0,50)] public int JumpNoise;
		public int characterTag;

		public float noiseRadius;
		public float CurrentSpeed;
		[Range(0, 2)] public float CrouchHeight = 2f;
		[Range(0, 1)] public float CrouchHeightBeforeJump = 0.8f;
		
		public float defaultCharacterCenter;
		public float middleAngleX;

		public int inspectorTabTop;
		public int inspectorSettingsTab;
		public int cameraInspectorTab;

//		public bool canShowCrosshair;
//		public bool canShowPickupIcon;
		public bool ActiveCharacter;
		public bool CameraSmoothWhenJumping;
		public bool isMultiplayerCharacter;
		public bool multiplayerCrouch;
		public bool activeJump = true;
		public bool activeSprint = true;
		public bool activeCrouch = true;
		public bool changeCameraType;
		public bool isPause;
		public bool DebugMode;
		public bool[] hasAxisButtonPressed = new bool[18];
		public bool AdjustmentScene;

		public CharacterHelper.BodyObjects BodyObjects = new CharacterHelper.BodyObjects();
		public IKHelper.FeetIKVariables IKVariables;
		
		public Transform Ragdoll;
		public Transform DirectionObject;

		public GameObject thisCamera;
		public GameObject UiButtonsGameObject;
		
		public SphereCollider noiseCollider;

		public CameraController thisCameraScript;

		public AvatarMask fpsMask;
		public AvatarMask tpsMask;

		public Text Health_Text;

		public enum Direction
		{
			Forward,
			Backward,
			Left,
			Right,
			Stationary,
			ForwardLeft,
			ForwardRight,
			BackwardLeft,
			BackwardRight
		}

		public Direction MoveDirection;

		private Vector3 directionVector;
		public Vector3 MoveVector;
		public Vector3 BodyLocalEulerAngles;

		public Quaternion RotationAngle;
		public Quaternion CurrentRotation;

		public CapsuleCollider characterCollider;
		public Rigidbody colliderRigidbody;

		public AnimatorOverrideController newController;

		public Inputs inputs;

		public KeyCode[] _keyboardCodes = new KeyCode[20];
		public KeyCode[] _gamepadCodes = new KeyCode[18];
		
		public string[] _gamepadAxes = new string[5];
		public string[] _gamepadButtonsAxes = new string[18];

		public string KillerName;
		public string KillMethod;
		public string CharacterName;
		
		public Button[] uiButtons = new Button[18];

		private RaycastHit distanceInfo;
		private RaycastHit heightInfo;

		private Transform bodylooks;

		public bool SmoothCameraWhenMoving = true;
		
		private bool isObstacle;
		private bool CanMove;
		private bool wasRunningActiveBeforeJump;
		private bool isSprint;
		private bool isJump;
		public bool isCrouch;
		private bool deactivateCrouch;
		private bool activateCrouch;
		private bool isCeiling;
		private bool leftStepSound;
		private bool rightStepSound;

		private bool instantiateRagdoll;

		// steps for jumping
		private bool step1;
		private bool step2;
		private bool step3;
		private bool step4;
		private bool step5;
		private bool step6;

		private bool crouchTimeOut = true;
		private bool setDefaultDistance;
		private bool notOnFloor;
		private bool OnFloor;

		public bool hasMoveButtonPressed;
		//private bool setDefaultHeight;
		private bool canClickMoveButton;
		private bool clickMoveButton;
		
		private float defaultDistance;
		public float SmoothIKSwitch = 1;
		private float BodyHeight;
		private float JumpPosition;
		public float defaultHeight = -1;
		private float hipsAngleX;
		private float spineAngleX;
		public float currentGravity;
		private float defaultGravity;
		private float newJumpHeight;
		private float bodyRotationUpLimit_y;
		private float bodyRotationDownLimit_y;
		private float bodyRotationUpLimit_x;
		private float bodyRotationDownLimit_x;
		private float angleBetweenCharacterAndCamera;
		private float pressButtonTimeout;
		public float changeCameraTypeTimeout;
		public float currentCharacterControllerCenter;
		
		private int touchId = -1;
		public int currentAnimatorLayer;
		
		private Vector3 CheckCollisionVector;

		private Vector2 MobileMoveStickDirection;
		private Vector2 MobileTouchjPointA, MobileTouchjPointB;

		private GameObject moveStick;
		private GameObject moveStickOutline;
		
		private RaycastHit HeightInfo;

		void Awake()
		{
			if (!inputs)
			{
				Debug.LogError("<color=red>Missing component</color> [Input]. Please reimport this kit.");
				Debug.Break();
			}

			if(Application.isMobilePlatform)
				UiButtonsGameObject = Instantiate(inputs.MobileInputs);

			if (Application.isMobilePlatform)
			{
				for (var i = 0; i < 12; i++)
				{
					uiButtons[i] = UiButtonsGameObject.transform.GetChild(i).GetComponent<Button>();
				}

				moveStick = UiButtonsGameObject.transform.GetChild(13).gameObject;
				moveStickOutline = UiButtonsGameObject.transform.GetChild(12).gameObject;
				
				for (var i = 14; i < 18; i++)
				{
					uiButtons[i] = UiButtonsGameObject.transform.GetChild(i).GetComponent<Button>();
				}
			}

			for (var i = 0; i < 20; i++)
			{
				Helper.ConvertKeyCodes(ref _keyboardCodes[i], inputs.KeyBoardCodes[i]);
			}

			for (var i = 0; i < 18; i++)
			{
				Helper.ConvertGamepadCodes(ref _gamepadCodes[i], inputs.GamepadCodes[i]);
			}
			
			for (var i = 0; i < 18; i++)
			{
				Helper.ConvertAxes(ref _gamepadButtonsAxes[i], inputs.GamepadCodes[i]);
			}

			for (var i = 0; i < 5; i++)
			{
				Helper.ConvertAxes(ref _gamepadAxes[i], inputs.GamepadAxes[i]);
			}

			//CharacterController = gameObject.GetComponent<CharacterController>();

			WeaponManager = gameObject.GetComponent<InventoryManager>();

			anim = gameObject.GetComponent<Animator>();

			if (!WeaponManager)
			{
				Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon Manager] script. Please, add it.");
				Debug.Break();
			}

			if (!CharacterController)
			{
				Debug.LogWarning("<color=yellow>Missing Component</color> [Character Controller]. Please, add it.");
				Debug.Break();
			}

			if (!anim)
			{
				Debug.LogWarning("<color=yellow>Missing Component</color> [Animator]. Please, add it.");
				Debug.Break();
			}

			newController = new AnimatorOverrideController(anim.runtimeAnimatorController);
			anim.runtimeAnimatorController = newController;

			ClipOverrides = new Helper.AnimationClipOverrides(newController.overridesCount);
			newController.GetOverrides(ClipOverrides);

//			for (var i = 0; i < 20; i++)
//			{
//				if (MovementAnimations[i])
//				{
//					ClipOverrides[overrideAnimationsNames[i]] = MovementAnimations[i];
//				}
//				else if (i < 9)
//				{
//					Debug.LogWarning(
//						"<color=yellow>Missing Component</color> [" + overrideAnimationsNames[i].Remove(0, 1) + "] animation", 
//						gameObject);
//				}
//				else if (i >= 9 & i < 17)
//				{
//					if (activeSprint)
//						Debug.LogWarning(
//							"<color=yellow>Missing Component</color> [" + overrideAnimationsNames[i].Remove(0, 1) + "] animation",
//							gameObject);
//				}
//				else if (i >= 17 & i < 19)
//				{
//					Debug.LogWarning(
//						"<color=yellow>Missing Component</color> [" + overrideAnimationsNames[i].Remove(0, 1) + "] animation",
//						gameObject);
//				}
//				else if (i == 19)
//				{
//					if (activeSprint)
//					{
//						Debug.LogWarning("<color=yellow>Missing Component</color> [Flying in air] animation", gameObject);
//					}
//				}
//			}

			newController.ApplyOverrides(ClipOverrides);


			if (DirectionObject)
			{
				DirectionObject.localEulerAngles = CharacterOffset.directionObjRotation;
			}
			else
			{
				Debug.LogError("<color=yellow>Missing component</color>: [Direction Object]. Please create your character again.");
				Debug.Break();
			}
			
			if (!thisCamera)
			{
				var foundObjects = FindObjectsOfType<CameraController>();
				foreach (CameraController camera in foundObjects)
				{
					if (camera.transform.parent == transform)
						thisCamera = camera.GetComponent<CameraController>().gameObject;
				}
			}

			if (thisCamera)
			{
				thisCameraScript = thisCamera.GetComponent<CameraController>();
				thisCameraScript.thisController = this;

			}
			else
			{
				Debug.LogError("<Color=red>Missing component</color> [This camera] in Controller Script", gameObject);
				Debug.Break();
			}
		}

		void Start()
		{
			StartCoroutine("StepSounds");
			StartCoroutine("SetDefaultHeight");

			OnFloor = true;
			//setDefaultHeight = false;
			
			defaultGravity = Physics.gravity.y;
			currentGravity = defaultGravity;
			
			deactivateCrouch = true;

			WeaponManager.pressInventoryButton = inputs.pressInventoryButton;

			var center = CharacterController.center;
			center = new Vector3(center.x, -CharacterOffset.CharacterHeight, center.z);
			CharacterController.center = center;
			defaultCharacterCenter = -CharacterOffset.CharacterHeight;

			Helper.CreateNoiseCollider(transform, this);

			if (isMultiplayerCharacter)
			{
				thisCameraScript.SetAnimVariables();
				return;
			}

			Helper.ChangeLayersRecursively(transform, "Character");

			Input.simulateMouseWithTouches = false;

			if (Application.isMobilePlatform)
			{
				Helper.AddButtonsEvents(uiButtons, WeaponManager, this);
				
				var gameManager = FindObjectOfType<GameManager>();
				if (gameManager)
				{
					if (uiButtons[9])
						uiButtons[9].onClick.AddListener(gameManager.Pause);
					
					if(uiButtons[16])
						uiButtons[16].onClick.AddListener(gameManager.SwitchCharacter);
				}
			}
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			
//			if(thisCameraScript.PickUpCrosshair)
//				thisCameraScript.PickUpCrosshair.gameObject.SetActive(false);

			CameraSmoothWhenJumping = true;

			if (thisCameraScript)
			{
				thisCameraScript.maxMouseAbsolute = middleAngleX + CameraParameters.fps_MaxRotationX;
				thisCameraScript.minMouseAbsolute = middleAngleX + CameraParameters.fps_MinRotationX;
			}

			gameObject.tag = "Player";
			
		
			if (!Health_Text && !AdjustmentScene)
			{
				Debug.LogWarning("(Player) <color=yellow>Missing component</color> [Health Text]. Add it in the GameManger script");
			}
		}
		

		void Update()
		{
			noiseCollider.radius = noiseRadius;

			if (isMultiplayerCharacter || !ActiveCharacter)
				return;

			currentCharacterControllerCenter = CharacterController.center.y;
				 
			anim.SetFloat("CameraAngle", Helper.AngleBetween(transform.forward, thisCamera.transform.forward));

			changeCameraTypeTimeout += Time.deltaTime;

			if (!AdjustmentScene)
			{
				if (Input.GetKeyDown(_gamepadCodes[11]) || Input.GetKeyDown(_keyboardCodes[11]) ||
				    Helper.CheckGamepadAxisButton(11, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKeyDown", inputs.AxisButtonValues[11]))
					ChangeCameraType();
			}
			else
			{
				if(Input.GetKeyDown(KeyCode.C))
					ChangeCameraType();
			}

			if (Input.GetKeyDown(_gamepadCodes[2]) || Input.GetKeyDown(_keyboardCodes[2]) ||
			    Helper.CheckGamepadAxisButton(2, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKeyDown",  inputs.AxisButtonValues[2]))
				Jump();

			if (inputs.PressSprintButton)
			{
				if ((Input.GetKey(_gamepadCodes[0]) || Input.GetKey(_keyboardCodes[0]) ||
				    Helper.CheckGamepadAxisButton(0, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKey",  inputs.AxisButtonValues[0])) &
				    activeSprint)
					Sprint(true, "press");
				else Sprint(false, "press");
			}
			else
			{
				if ((Input.GetKeyDown(_gamepadCodes[0]) || Input.GetKeyDown(_keyboardCodes[0]) ||
				     Helper.CheckGamepadAxisButton(0, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKeyDown", inputs.AxisButtonValues[0])) & activeSprint)
					Sprint(true, "click");
			}

			if (inputs.PressCrouchButton)
			{
				if (Input.GetKey(_gamepadCodes[1]) || Input.GetKey(_keyboardCodes[1]) ||
				    Helper.CheckGamepadAxisButton(1, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKey",  inputs.AxisButtonValues[1]))
				{
					if (!activateCrouch)
					{
						Crouch(true, "press");
						deactivateCrouch = false;
						activateCrouch = true;
					}
				}
				else
				{
					if (!deactivateCrouch)
					{
						Crouch(false, "press");
						deactivateCrouch = true;
						activateCrouch = false;
					}
				}
			}
			else
			{
				if (Input.GetKeyDown(_gamepadCodes[1]) || Input.GetKeyDown(_keyboardCodes[1]) ||
				    Helper.CheckGamepadAxisButton(1, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKeyDown", inputs.AxisButtonValues[1]))
				{
					if (crouchTimeOut)
					{
						if(isSprint)
							DeactivateSprint();
						
						Crouch(true, "click");
						crouchTimeOut = false;
						StartCoroutine("CrouchTimeout");
					}
				}
			}
			
			CheckHealth();
			GetLocomotionInput();
			SnapAlignCharacterWithCamera();
			ProcessMotion();
			
			if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson) JumpingProcess();
			
			HeightDetection();
			
		}

		void HeightDetection()
		{
			if (defaultHeight == -1)
				return;

			RaycastHit info;
			RaycastHit info2;
//			Debug.DrawRay(BodyObjects.Hips.position - transform.forward, Vector3.down * 10, anim.GetBool("OnFloor") ? Color.green : Color.red);
//			Debug.DrawRay(BodyObjects.Hips.position + transform.forward * 2, Vector3.down * 10, anim.GetBool("OnFloorForward") ? Color.green : Color.red);
			
			if (Physics.Raycast(BodyObjects.Hips.position - transform.forward, Vector3.down, out info, 100, Helper.layerMask()))
			{
				anim.SetBool("OnFloor", !(defaultHeight + 1 < info.distance));
			}

			if (Physics.Raycast(BodyObjects.Hips.position + transform.forward * 2, Vector3.down, out info2, 100, Helper.layerMask()))
			{
				if (defaultHeight + 1 < info2.distance)
				{
					if (anim.GetBool("OnFloorForward"))
						anim.SetFloat("FallingHeight", info2.distance);
					
					anim.SetBool("OnFloorForward", false);
				}
				else
				{
					anim.SetBool("OnFloorForward", true);
				}
			}
		}

		IEnumerator SetDefaultHeight()
		{
			yield return new WaitForSeconds(2);
			RaycastHit info;
			if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out info, 100, Helper.layerMask()))
			{
				defaultHeight = info.distance;
			}
			
			StopCoroutine("SetDefaultHeight");
		}

		void GetLocomotionInput()
		{
			
			directionVector = Vector3.zero;

			if (!isPause)
			{
				hasMoveButtonPressed = false;
				if (Application.isMobilePlatform & !WeaponManager.GamepadConnect)
				{
					CheckMobileInputs();
					directionVector = new Vector3(MobileMoveStickDirection.x, 0, MobileMoveStickDirection.y);
				}
				else
				{
					if (WeaponManager.GamepadConnect)
					{
						hasMoveButtonPressed = false;

						var Horizontal = Input.GetAxis(_gamepadAxes[0]);
						var Vertical = Input.GetAxis(_gamepadAxes[1]);

						if (inputs.invertAxes[0])
							Horizontal *= -1;

						if (inputs.invertAxes[1])
							Vertical *= -1;

						if (Horizontal > 0.1f || Horizontal < -0.1f || Vertical > 0.1f || Vertical < -0.1f)
							hasMoveButtonPressed = true;

						directionVector = new Vector3(Horizontal, 0, Vertical);
					}
					else
					{
						hasMoveButtonPressed = false;
						if (Input.GetKey(_keyboardCodes[12]))
						{
							directionVector += Vector3.forward;
							hasMoveButtonPressed = true;
						}

						if (Input.GetKey(_keyboardCodes[14]))
						{
							directionVector += Vector3.right;
							hasMoveButtonPressed = true;
						}

						if (Input.GetKey(_keyboardCodes[13]))
						{
							directionVector -= Vector3.forward;
							hasMoveButtonPressed = true;
						}

						if (Input.GetKey(_keyboardCodes[15]))
						{
							directionVector -= Vector3.right;
							hasMoveButtonPressed = true;
						}
					}
				}

				anim.SetBool("PressMoveAxis", hasMoveButtonPressed);
				
				if (hasMoveButtonPressed)
				{
					if (isSprint)
						noiseRadius = Mathf.Lerp(noiseRadius, SprintMovementNoise, 5 * Time.deltaTime);
					else if (isCrouch)
					{
						noiseRadius = Mathf.Lerp(noiseRadius, CrouchMovementNoise, 5 * Time.deltaTime);
					}
					else
					{
						noiseRadius = Mathf.Lerp(noiseRadius, MovementNoise, 5 * Time.deltaTime);
					}
				}
				else
				{
					if (!isCrouch)
						noiseRadius = Mathf.Lerp(noiseRadius, IdleNoise, 5 * Time.deltaTime);
					else
					{
						noiseRadius = Mathf.Lerp(noiseRadius, CrouchIdleNoise, 5 * Time.deltaTime);
					}
				}

				CheckCollisionVector = directionVector * 100;

				if (CanMove)
				{
					if (hasMoveButtonPressed)
					{
						anim.SetFloat("Horizontal", directionVector.x, 0.5f, Time.deltaTime);
						anim.SetFloat("Vertical", directionVector.z, 0.5f, Time.deltaTime);
					}
					else
					{
						anim.SetFloat("Horizontal", directionVector.x, 0.5f, Time.deltaTime);
						anim.SetFloat("Vertical", directionVector.z, 0.5f, Time.deltaTime);
					}
				}
				else
				{
					anim.SetFloat("Horizontal", 0, 0.3f, Time.deltaTime);
					anim.SetFloat("Vertical", 0, 0.3f, Time.deltaTime);
				}
			}
			else
			{
				anim.SetFloat("Horizontal", 0, 0.3f, Time.deltaTime);
				anim.SetFloat("Vertical", 0, 0.3f, Time.deltaTime);

				Sprint(false, "press");
			}

			if (!hasMoveButtonPressed)
				canClickMoveButton = true;
			
			if (hasMoveButtonPressed && canClickMoveButton)
			{
				clickMoveButton = hasMoveButtonPressed;
				canClickMoveButton = false;
			}
			
			if (clickMoveButton && WeaponManager.weaponController && WeaponManager.weaponController.IsAimEnabled && isCrouch && TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
			{
				WeaponManager.weaponController.Aim(true, false);
//				clickMoveButton = false;
			}

			MoveVector = new Vector3(anim.GetFloat("Horizontal"), 0, anim.GetFloat("Vertical"));

			angleBetweenCharacterAndCamera = Helper.AngleBetween(transform.TransformDirection(new Vector3(-MoveVector.x, MoveVector.y, MoveVector.z)),
					thisCamera.transform.forward);

			anim.SetFloat("Angle", angleBetweenCharacterAndCamera);
			
			if(!hasMoveButtonPressed)
			{
				//pressButtonTimeout = 0;
//				anim.SetBool("Move button has pressed", false);
				anim.SetBool("Move", false);
			}

			if (Mathf.Abs(MoveVector.x) > 0.6f || Math.Abs(MoveVector.z) > 0.6f)
			{
				anim.SetBool("MoveButtonHasPressed", false);
				pressButtonTimeout = 0;
				anim.SetBool("Move", true);
				clickMoveButton = false;
			}

			if (clickMoveButton)
			{
				pressButtonTimeout += Time.deltaTime;
			}

			if (pressButtonTimeout >= 0.2f && Mathf.Abs(MoveVector.x) < 0.3f && Math.Abs(MoveVector.z) < 0.3f)
			{
				anim.SetBool("MoveButtonHasPressed", true);

				if (pressButtonTimeout >= 0.3f)
				{
					anim.SetBool("MoveButtonHasPressed", false);
					pressButtonTimeout = 0;
					clickMoveButton = false;
				}
			}
//			else if(Mathf.Abs(MoveVector.x) > 0.2f || Math.Abs(MoveVector.z) > 0.2f)
//			{
//				pressButtonTimeout += Time.deltaTime;
//			}
//
//			if (pressButtonTimeout >= 0.2f)
//			{
//				anim.SetBool("Move button has pressed", true);
//			}

			if (SmoothCameraWhenMoving)
			{
				if (hasMoveButtonPressed)
				{
					if (!isSprint && !isCrouch)
						thisCameraScript.cameraMovementDistance = Mathf.Lerp(thisCameraScript.cameraMovementDistance, 6, 2 * Time.deltaTime);
					else if (isSprint)
						thisCameraScript.cameraMovementDistance = Mathf.Lerp(thisCameraScript.cameraMovementDistance, 7, 3 * Time.deltaTime);
					else if (isCrouch)
						thisCameraScript.cameraMovementDistance = Mathf.Lerp(thisCameraScript.cameraMovementDistance, 5, 3 * Time.deltaTime);

				}
				else
				{
					if (!isCrouch && !isSprint)
						thisCameraScript.cameraMovementDistance = Mathf.Lerp(thisCameraScript.cameraMovementDistance, 5, 2 * Time.deltaTime);
					else if (isCrouch) thisCameraScript.cameraMovementDistance = Mathf.Lerp(thisCameraScript.cameraMovementDistance, 4.5f, 3 * Time.deltaTime);
					else if (isSprint) thisCameraScript.cameraMovementDistance = Mathf.Lerp(thisCameraScript.cameraMovementDistance, 6, 3 * Time.deltaTime);
				}
			}


			CurrentMoveDirection();
		}
		

		void CheckMobileInputs()
		{
			if (Input.touches.Length > 0)
			{
				for (var i = 0; i < Input.touches.Length; i++)
				{
					var touch = Input.GetTouch(i);

					if (touchId == -1 & touch.phase == TouchPhase.Began & touch.position.x < Screen.width / 2)
					{
						var eventSystem = FindObjectOfType<EventSystem>();

						if (!eventSystem.currentInputModule.IsPointerOverGameObject(touch.fingerId))
						{
							touchId = touch.fingerId;

							MobileTouchjPointA = touch.position;

							moveStick.gameObject.SetActive(true);
							moveStickOutline.gameObject.SetActive(true);

							moveStick.transform.position = MobileTouchjPointA;
							moveStickOutline.transform.position = MobileTouchjPointA;
						}
					}

					if (touch.fingerId == touchId)
					{
						if (touch.position.x < Screen.width / 2)
						{
							MobileTouchjPointB = new Vector2(touch.position.x, touch.position.y);

							var offset = MobileTouchjPointB - MobileTouchjPointA;

							if (offset.x > 0.1f || offset.y > 0.1f || offset.x < -0.1 || offset.y < -0.1f)
								hasMoveButtonPressed = true;

							MobileMoveStickDirection = Vector2.ClampMagnitude(offset, inputs.StickRange);

							moveStick.transform.position = new Vector2(
								MobileTouchjPointA.x + MobileMoveStickDirection.x,
								MobileTouchjPointA.y + MobileMoveStickDirection.y);

							MobileMoveStickDirection /= inputs.StickRange;
						}
						if (touch.phase == TouchPhase.Ended)
						{
							touchId = -1;
							MobileMoveStickDirection = Vector2.zero;
							moveStick.gameObject.SetActive(false);
							moveStickOutline.gameObject.SetActive(false);
						}
					}
				}
			}
			else
			{
				moveStick.gameObject.SetActive(false);
				moveStickOutline.gameObject.SetActive(false);
			}
		}

		void CheckHealth()
		{
			if (Health_Text)
			{
				if (PlayerHealth < 0)
				{
					Health_Text.text = "0";
				}
				else
				{
					if (PlayerHealth >= 75)
						Health_Text.color = Color.green;
					if (PlayerHealth >= 50 & PlayerHealth < 75)
						Health_Text.color = Color.yellow;
					if (PlayerHealth >= 25 & PlayerHealth < 50)
						Health_Text.color = new Color32(255, 140, 0, 255);
					if (PlayerHealth < 25)
						Health_Text.color = Color.red;

					Health_Text.text = PlayerHealth.ToString("F0");
				}
			}

			if (PlayerHealth > 0) return;
			
			if (Ragdoll && !instantiateRagdoll)
			{
				instantiateRagdoll = true;
				var dead = Instantiate(Ragdoll, transform.position, Quaternion.identity);
				Helper.CopyTransformsRecurse(transform, dead);
			}
			else
			{
				Debug.LogWarning("Missing component [Ragdoll]. Add it, otherwise the rag doll won't be created after the death of the player.", gameObject);
			}

			thisCameraScript.enabled = false;
			Destroy(gameObject);
		}

		void ProcessMotion()
		{

			MoveVector = transform.TransformDirection(MoveVector);

			CheckCollisionVector = TypeOfCamera != CharacterHelper.CameraType.TopDown
				? thisCamera.transform.TransformDirection(CheckCollisionVector)
				: transform.TransformDirection(CheckCollisionVector);

			if (MoveVector.magnitude > 1)
				MoveVector = MoveVector.normalized;


			if (CheckCollisionVector.magnitude > 1)
				CheckCollisionVector = CheckCollisionVector.normalized;

			if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
			{
				if (OnFloor && !isJump)
				{
					CurrentSpeed = MoveDirection == Direction.Stationary
						? Mathf.Lerp(CurrentSpeed, MoveSpeed(), 0.5f * Time.deltaTime)
						: Mathf.Lerp(CurrentSpeed, MoveSpeed(), 3 * Time.deltaTime);
				}
				else
				{
					if (isSprint)
						CurrentSpeed -= 3 * Time.deltaTime;
					else CurrentSpeed -= 1 * Time.deltaTime;
				}

				if (CurrentSpeed < 0)
					CurrentSpeed = 0;

			}

			if (Physics.Raycast(transform.position + Vector3.up * 2, CheckCollisionVector, out distanceInfo, CheckCollisionVector.magnitude * 10, Helper.layerMask()))
			{
				if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				{
					if (distanceInfo.distance < 1.5f)
					{
						CurrentSpeed = 0;
						isObstacle = true;
					}
					else
					{
						StartCoroutine(MovePause());
					}
				}
				else
				{
					if (distanceInfo.distance < 3)
					{
						isObstacle = true;
					}
					else
					{
						StartCoroutine(MovePause());
					}
				}
			}
			else
			{
				StartCoroutine(MovePause());
			}


			MoveVector = new Vector3(MoveVector.x * CurrentSpeed, 0, MoveVector.z * CurrentSpeed);

			if (!isObstacle)
			{
				if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
					CharacterController.Move(new Vector3(MoveVector.x, 0, MoveVector.z) * Time.deltaTime);
				CanMove = true;
			}
			else
			{
				anim.SetBool("Move", false);
				CanMove = false;
			}

			CharacterController.Move(new Vector3(0, currentGravity, 0) * Time.deltaTime);
		}

		public void Jump()
		{
			if (activeJump & !isPause && OnFloor)
			{
				if (Physics.Raycast(BodyObjects.Head.position, Vector3.up, out HeightInfo, 100, Helper.layerMask()))
				{
					if (HeightInfo.distance - 2.5f <= transform.position.y + 1)
					{
						newJumpHeight = HeightInfo.distance - 2.5f;

						if (newJumpHeight < 2)
						{
							Debug.Log("Your character has not jump because the ceiling is too low.");
							return;
						}
						isCeiling = true;
					}
				}
				isJump = true;

				if (TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
				{
					currentGravity = 0;
					anim.SetBool("Jump", true);
					StartCoroutine("JumpTimeout");
				}
			}
		}

		IEnumerator JumpTimeout()
		{
			yield return new WaitForSeconds(1);
			anim.SetBool("Jump", false);
			currentGravity = defaultGravity;
			isJump = false;
			StopCoroutine("JumpTimeout");
		}

		IEnumerator CrouchTimeout()
		{
			yield return new WaitForSeconds(1);
			crouchTimeOut = true;
			StopCoroutine("CrouchTimeout");
		}

		#region JumpingProcess

		void JumpingProcess()
		{
			if (isJump & !step1)
			{
				CameraSmoothWhenJumping = false;

				BodyHeight = Mathf.Lerp(CharacterController.center.y, defaultCharacterCenter + CrouchHeightBeforeJump, 4 * Time.deltaTime);

				CharacterController.center = new Vector3(CharacterController.center.x, BodyHeight, CharacterController.center.z);

				if (Math.Abs(BodyHeight - (defaultCharacterCenter + CrouchHeightBeforeJump)) < 0.2f)
				{
					CharacterController.center = new Vector3(CharacterController.center.x, defaultCharacterCenter + CrouchHeightBeforeJump, CharacterController.center.z);
					step1 = true;
				}
			}

			if (step1 & !step2)
			{
				BodyHeight = Mathf.Lerp(CharacterController.center.y, defaultCharacterCenter - 0.1f, 6 * Time.deltaTime);
				CharacterController.center = new Vector3(CharacterController.center.x, BodyHeight, CharacterController.center.z);

				if (Math.Abs(CharacterController.center.y - defaultCharacterCenter) < 0.1f)
				{
					CharacterController.center = new Vector3(CharacterController.center.x, defaultCharacterCenter, CharacterController.center.z);
					step2 = true;
					SmoothIKSwitch = 0;
					
					anim.SetBool("Jump", true);

					if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out HeightInfo, 100, Helper.layerMask()))
					{
						defaultDistance = HeightInfo.distance;
						setDefaultDistance = true;
					}

					if (!isCeiling)
						JumpPosition = transform.position.y + JumpHeight;
					else JumpPosition = transform.position.y + newJumpHeight;

					BodyHeight = JumpSpeed;
				}
			}

			if (step1 & step2 & !step3)
			{
				currentGravity = 0;
				BodyHeight = Mathf.Lerp(BodyHeight, 0, 0.7f * JumpSpeed / JumpHeight * Time.deltaTime);
				Vector3 jumpPos = new Vector3(transform.position.x, JumpPosition, transform.position.z);
				transform.position = Vector3.Lerp(transform.position, jumpPos, 0.1f * JumpSpeed * Time.deltaTime);

				if (Math.Abs(transform.position.y - JumpPosition) < 0.5f)
				{
					step3 = true;
					currentGravity = defaultGravity;
				}
			}

			if (!notOnFloor & !step4 & !step5 & setDefaultDistance)
			{
				if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out HeightInfo, 100, Helper.layerMask()))
				{
					if (Math.Abs(HeightInfo.distance - defaultDistance) > 1)
					{
						if (isJump)
						{
							if (!step1 || !step2 || !step3)
								return;
						}

						notOnFloor = true;
						SmoothIKSwitch = 0;
						CameraSmoothWhenJumping = false;
						
						anim.SetBool("Jump", true);
					}
				}
				else
				{
					notOnFloor = true;
					SmoothIKSwitch = 0;
					
					anim.SetBool("Jump", true);
				}
			}

			if (notOnFloor & setDefaultDistance)
			{
				if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out HeightInfo, 100, Helper.layerMask()))
				{
					if (DebugMode)
					{
						defaultDistance = HeightInfo.distance;
						anim.SetBool("Jump", false);
					}
					else
					{
						if (Math.Abs(HeightInfo.distance - defaultDistance) < 0.5f)
						{
							anim.SetBool("Jump", false);
							step4 = true;
							SmoothIKSwitch = 1;
							notOnFloor = false;
						}
					}
				}
			}

			if (step4 & !step5)
			{
				BodyHeight = Mathf.Lerp(CharacterController.center.y, defaultCharacterCenter + CrouchHeightBeforeJump,
					6 * Time.deltaTime);
				CharacterController.center = new Vector3(CharacterController.center.x, BodyHeight, CharacterController.center.z);

				if (Math.Abs(CharacterController.center.y - (defaultCharacterCenter + CrouchHeightBeforeJump)) < 0.1f)
				{
					CharacterController.center = new Vector3(CharacterController.center.x, defaultCharacterCenter + CrouchHeightBeforeJump, CharacterController.center.z);
					step5 = true;
				}
			}

			if (step4 & step5)
			{
				BodyHeight = Mathf.Lerp(CharacterController.center.y, defaultCharacterCenter - 0.1f,
					4 * Time.deltaTime);
				CharacterController.center = new Vector3(CharacterController.center.x,
					BodyHeight,
					CharacterController.center.z);

				if (Math.Abs(CharacterController.center.y - defaultCharacterCenter) < 0.1f)
				{
					CharacterController.center = new Vector3(CharacterController.center.x, defaultCharacterCenter, CharacterController.center.z);
					step1 = false;
					step2 = false;
					step3 = false;
					step4 = false;
					step5 = false;
					isCeiling = false;
					isJump = false;
					CameraSmoothWhenJumping = true;
				}
			}
		}

		#endregion

		public void Sprint(bool active, string type)
		{
			if (activeSprint && !isPause && !isJump && !isCrouch)
			{
				if (type == "press")
				{
					if(isCrouch)
						return;
					
					if (active)
						ActivateSprint();
					else
						DeactivateSprint();
				}
				else
				{
					if(!isSprint)
						ActivateSprint();
					else
						DeactivateSprint();
				}
			}
		}

		void DeactivateSprint()
		{
			isSprint = false;
			anim.SetBool("Sprint", false);
		}
		
		void ActivateSprint()
		{
			isSprint = true;
			anim.SetBool("Sprint", true);
		}

		public void Crouch(bool active, string type)
		{
			if (activeCrouch && !isPause && !isJump && !isSprint && TypeOfCamera != CharacterHelper.CameraType.TopDown)
			{
				if (type == "press")
				{
					if(isSprint)
						return;
					if (active)
						ActivateCrouch();
					else
						DeactivateCrouch();
				}
				else
				{
					if (!isCrouch)
						ActivateCrouch();
					else
						DeactivateCrouch();
				}
			}
		}

		public void ActivateCrouch()
		{
			if (!isMultiplayerCharacter)
			{
				if (isSprint)
					Sprint(false, "press");
				anim.SetBool("Crouch", true);

				if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				{
					defaultCharacterCenter += CrouchHeight;
					StartCoroutine("ChangeBodyHeight");
				}
				else if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
				{
					if (WeaponManager.hasAnyWeapon && !isCrouch)
						WeaponManager.weaponController.CrouchHands();
					
					multiplayerCrouch = true;
				}
				isCrouch = true;
			}
		}

		public void DeactivateCrouch()
		{
			if (!isMultiplayerCharacter)
			{
				if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				{
					defaultCharacterCenter -= CrouchHeight;
					StartCoroutine("ChangeBodyHeight");
				}
				else if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
				{
					if (WeaponManager.hasAnyWeapon && isCrouch)
						WeaponManager.weaponController.CrouchHands();
					
					multiplayerCrouch = true;
				}
				
				anim.SetBool("Crouch", false);
				isCrouch = false;
			}
		}

		public void ChangeCameraType()
		{
			if(!AdjustmentScene && isPause || isJump || anim.GetBool("Grenade") || WeaponManager.weaponController && WeaponManager.weaponController.IsReloadEnabled)
				return;
			
			if(changeCameraTypeTimeout <= 1 || WeaponManager.weaponController && !WeaponManager.weaponController.TakeWeaponInAimMode)
				return;
			
			if(thisCameraScript.deepAim)
				thisCameraScript.deepAim = false;

			if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson && CameraParameters.ActiveTD)
			{
				if (WeaponManager.weaponController && !WeaponManager.weaponController.IsAimEnabled)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.TopDown, this);
				else if(WeaponManager.weaponController && WeaponManager.weaponController.IsAimEnabled)
				{
					WeaponManager.weaponController.Aim(true, false);
					StartCoroutine(ChangeCameraTimeout(CharacterHelper.CameraType.TopDown));
				}
				else if(!WeaponManager.weaponController)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.TopDown, this);
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson && CameraParameters.ActiveTP)
			{
				if (WeaponManager.weaponController && !WeaponManager.weaponController.IsAimEnabled ||
				    WeaponManager.weaponController && WeaponManager.weaponController.IsAimEnabled && WeaponManager.weaponController.ActiveAimTPS)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.ThirdPerson, this);
				else if(WeaponManager.weaponController && !WeaponManager.weaponController.ActiveAimTPS && WeaponManager.weaponController.IsAimEnabled)
				{
					WeaponManager.weaponController.Aim(true, false);
					StartCoroutine(ChangeCameraTimeout(CharacterHelper.CameraType.ThirdPerson));
				}
				else if(!WeaponManager.weaponController)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.ThirdPerson, this);
				
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraParameters.ActiveFP)
			{
				if (WeaponManager.weaponController && !WeaponManager.weaponController.IsAimEnabled ||
				    WeaponManager.weaponController && WeaponManager.weaponController.IsAimEnabled && WeaponManager.weaponController.ActiveAimFPS)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.FirstPerson, this);
				else if(WeaponManager.weaponController && !WeaponManager.weaponController.ActiveAimFPS && WeaponManager.weaponController.IsAimEnabled)
				{
					WeaponManager.weaponController.Aim(true, false);
					StartCoroutine(ChangeCameraTimeout(CharacterHelper.CameraType.FirstPerson));
				}
				else if(!WeaponManager.weaponController)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.FirstPerson, this);
				
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraParameters.ActiveTD)
			{
				if (WeaponManager.weaponController && !WeaponManager.weaponController.IsAimEnabled)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.TopDown, this);
				else if(WeaponManager.weaponController && WeaponManager.weaponController.IsAimEnabled)
				{
					WeaponManager.weaponController.Aim(true, false);
					StartCoroutine("ChangeCameraTimeout");
				}
				else if(!WeaponManager.weaponController)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.TopDown, this);
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.TopDown && CameraParameters.ActiveTP)
			{
				CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.ThirdPerson, this);
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.TopDown && CameraParameters.ActiveFP)
			{
				CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.FirstPerson, this);
			}
			
			changeCameraTypeTimeout = 0;
		}

		public void ChangeCameraType(CharacterHelper.CameraType type)
		{
			if(!AdjustmentScene && isPause)
				return;
			
			if(changeCameraTypeTimeout <= 0.5f || WeaponManager.weaponController && !WeaponManager.weaponController.TakeWeaponInAimMode)
				return;
			
			changeCameraTypeTimeout = 0;
			
			CharacterHelper.SwitchCamera(TypeOfCamera, type, this);
		}

		public void SnapAlignCharacterWithCamera()
		{
			if ((Math.Abs(MoveVector.x) > 0.8f || Math.Abs(MoveVector.z) > 0.8f) && (!WeaponManager.weaponController || WeaponManager.weaponController && !WeaponManager.weaponController.IsAimEnabled) &&
			    (anim.GetCurrentAnimatorStateInfo(1).IsName("Walk_Forward") || anim.GetCurrentAnimatorStateInfo(1).IsName("Run_Forward")) || anim.GetCurrentAnimatorStateInfo(1).IsName("Crouch_Walk_Forward"))
			{
				var angle = angleBetweenCharacterAndCamera;

				var _angle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angle, transform.eulerAngles.z);
				
				if (Mathf.Abs(angle) < 135)
					transform.rotation = Quaternion.Slerp(transform.rotation, _angle, 3 * Time.deltaTime);

			}
			else if ((Math.Abs(MoveVector.x) > 0.8f || Math.Abs(MoveVector.z) > 0.8f) && (WeaponManager.weaponController && WeaponManager.weaponController.IsAimEnabled || TypeOfCamera != CharacterHelper.CameraType.ThirdPerson))
			{
				var angle = Mathf.DeltaAngle(transform.eulerAngles.y, thisCamera.transform.parent.eulerAngles.y);
				
				var _angle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angle, transform.eulerAngles.z);
				
				transform.rotation = Quaternion.Slerp(transform.rotation, _angle, 3 * Time.deltaTime);
			}

			CurrentRotation = transform.rotation;
		}

		public void BodyLookAt(Transform bodylookAt)
		{
			//TopBodyOffset();

//			if (!AdjustmentScene)
//			{
				if (WeaponManager.weaponController)
				{
					if (!WeaponManager.hasAnyWeapon || TypeOfCamera != CharacterHelper.CameraType.TopDown && !WeaponManager.weaponController.IsAimEnabled
					                                || !isCrouch && (Mathf.Abs(MoveVector.x) > 0.4f || Math.Abs(MoveVector.z) > 0.4f) || isCrouch &&
					                                !anim.GetCurrentAnimatorStateInfo(1).IsName("Crouch_Aim_Idle") &&
					                                !anim.GetCurrentAnimatorStateInfo(1).IsName("Crouch_Aim_Turn_90_L") &&
					                                !anim.GetCurrentAnimatorStateInfo(1).IsName("Crouch_Aim_Turn_90_L")
					) // || Math.Abs(anim.GetFloat("CameraAngle")) > 45))
					{
						bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 0, 3 * Time.deltaTime);
						bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, 0, 3 * Time.deltaTime);

						bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, 0, 3 * Time.deltaTime);
						bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, 0, 3 * Time.deltaTime);
					}
					else
					{
						if (Math.Abs(anim.GetFloat("CameraAngle")) < 45)
						{
							bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 60, 3 * Time.deltaTime);
							bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, -60, 3 * Time.deltaTime);

							bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, CameraParameters.fps_MaxRotationX + 30, 3 * Time.deltaTime);
							bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, CameraParameters.fps_MinRotationX, 3 * Time.deltaTime);
						}
						else
						{
							bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 60, 1 * Time.deltaTime);
							bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, -60, 1 * Time.deltaTime);

							bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, CameraParameters.fps_MaxRotationX + 30, 1 * Time.deltaTime);
							bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, CameraParameters.fps_MinRotationX, 1 * Time.deltaTime);
						}
					}
				}
				else
				{
					bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 0, 3 * Time.deltaTime);
					bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, 0, 3 * Time.deltaTime);

					bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, 0, 3 * Time.deltaTime);
					bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, 0, 3 * Time.deltaTime);
				}
//			}
//			else
//			{
//				bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 90, 3 * Time.deltaTime);
//				bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, -90, 3 * Time.deltaTime);
//
//				bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, CameraParameters.fps_MaxRotationX + 30, 3 * Time.deltaTime);
//				bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, CameraParameters.fps_MinRotationX, 3 * Time.deltaTime);
//			}

			var direction = bodylookAt.position - DirectionObject.position;

			var middleAngleX = Helper.AngleBetween(direction, DirectionObject).x;
			var middleAngleY = Helper.AngleBetween(direction, DirectionObject).y;

			if (middleAngleY > bodyRotationUpLimit_y)
				middleAngleY = bodyRotationUpLimit_y;
			else if (middleAngleY < bodyRotationDownLimit_y)
				middleAngleY = bodyRotationDownLimit_y;

			if (middleAngleX > bodyRotationUpLimit_x)
				middleAngleX = bodyRotationUpLimit_x;
			else if (middleAngleX < bodyRotationDownLimit_x)
				middleAngleX = bodyRotationDownLimit_x;


			if (!isCrouch)
			{
				BodyObjects.TopBody.RotateAround(DirectionObject.position, Vector3.up, -middleAngleY);
				BodyObjects.TopBody.RotateAround(DirectionObject.position, DirectionObject.TransformDirection(Vector3.right), -middleAngleX);
			}
			else
			{
				BodyObjects.TopBody.RotateAround(DirectionObject.position, Vector3.up, -middleAngleY);
				BodyObjects.TopBody.RotateAround(DirectionObject.position, DirectionObject.TransformDirection(Vector3.right), -middleAngleX);
			}
			
		}

		public void TopBodyOffset()
		{
			if (!AdjustmentScene)
			{
				BodyObjects.TopBody.Rotate(Vector3.right, CharacterOffset.xRotationOffset);
				BodyObjects.TopBody.Rotate(Vector3.up, CharacterOffset.yRotationOffset);
				BodyObjects.TopBody.Rotate(Vector3.forward, CharacterOffset.zRotationOffset);
			}
			else
			{
				BodyObjects.TopBody.eulerAngles = new Vector3(CharacterOffset.xRotationOffset, CharacterOffset.yRotationOffset, CharacterOffset.zRotationOffset);
			}
		}

		public void BodyRotate()
		{
			if (DebugMode) return;
			
			BodyLocalEulerAngles = BodyObjects.TopBody.localEulerAngles;

			if (BodyLocalEulerAngles.x > 180)
				BodyLocalEulerAngles.x -= 360;
			if (BodyLocalEulerAngles.y > 180)
				BodyLocalEulerAngles.y -= 360;

			if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
			{
				var hipsAngleY = transform.eulerAngles.y;
				var spineAngleY = BodyObjects.TopBody.eulerAngles.y - CharacterOffset.yRotationOffset;
				var middleAngleY = Mathf.DeltaAngle(hipsAngleY, spineAngleY);

				if (middleAngleY > 50)
				{
					RotationAngle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (middleAngleY - 50), transform.eulerAngles.z);

					transform.rotation = Quaternion.Slerp(transform.rotation, RotationAngle, middleAngleY - 50 * Time.deltaTime);

				}

				else if (middleAngleY < -50)
				{
					RotationAngle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - (-50 - middleAngleY), transform.eulerAngles.z);

					transform.rotation = Quaternion.Slerp(transform.rotation, RotationAngle, -middleAngleY + -50 * Time.deltaTime);
				}
			}

			hipsAngleX = transform.eulerAngles.x;
			spineAngleX = BodyObjects.TopBody.eulerAngles.x - CharacterOffset.xRotationOffset;
			middleAngleX = Mathf.DeltaAngle(hipsAngleX, spineAngleX);

			
//			if (-middleAngleX > CameraParameters.fps_MaxRotationX)
//			{
//				thisCameraScript.maxMouseAbsolute = thisCameraScript._mouseAbsolute.y;
//			}
//			else if (-middleAngleX < CameraParameters.fps_MinRotationX)
//			{
//				thisCameraScript.minMouseAbsolute = thisCameraScript._mouseAbsolute.y;
//			}
		}

		private void CurrentMoveDirection()
		{
			var forward = false;
			var backward = false;
			var left = false;
			var right = false;
			
			NullDirectionAnimations();
			
			if (directionVector.z > 0)
				forward = true;
			if (directionVector.z < 0)
				backward = true;
			if (directionVector.x > 0)
				right = true;
			if (directionVector.x < 0)
				left = true;

			if (forward)
			{
				if (left)
				{
					MoveDirection = Direction.ForwardLeft;
					anim.SetBool("ForwardLeft", true);
				}
				else if (right)
				{
					MoveDirection = Direction.ForwardRight;
					anim.SetBool("ForwardRight", true);
				}
				else
				{
					MoveDirection = Direction.Forward;
					anim.SetBool("Forward", true);
				}
			}
			else if (backward)
			{
				if (left)
				{
					MoveDirection = Direction.BackwardLeft;
					anim.SetBool("BackwardLeft", true);
				}
				else if (right)
				{
					MoveDirection = Direction.BackwardRight;
					anim.SetBool("BackwardRight", true);
				}
				else
				{
					MoveDirection = Direction.Backward;
					anim.SetBool("Backward", true);
				}
			}
			else if (right)
			{
				MoveDirection = Direction.Right;
				anim.SetBool("Right", true);
			}
			else if (left)
			{
				MoveDirection = Direction.Left;
				anim.SetBool("Left", true);
			}
			else
			{
				MoveDirection = Direction.Stationary;
			}
		}

		void NullDirectionAnimations()
		{
			anim.SetBool("Forward", false);
			anim.SetBool("ForwardRight", false);
			anim.SetBool("ForwardLeft", false);
			anim.SetBool("Left", false);
			anim.SetBool("Right", false);
			anim.SetBool("BackwardLeft", false);
			anim.SetBool("BackwardRight", false);
			anim.SetBool("Backward", false);
		}

		float MoveSpeed()
		{
			var moveSpeed = 0f;

			switch (MoveDirection)
			{
				case Direction.Stationary:
					moveSpeed = 0;
					break;
				case Direction.Forward:
					moveSpeed = ChoiceSpeed(NormForwardSpeed, RunForwardSpeed, CrouchForwardSpeed);
					break;
				case Direction.Backward:
					moveSpeed = ChoiceSpeed(NormBackwardSpeed, RunBackwardSpeed, CrouchBackwardSpeed);
					break;
				case Direction.Right:
					moveSpeed = ChoiceSpeed(NormLateralSpeed, RunLateralSpeed, CrouchLateralSpeed);
					break;
				case Direction.Left:
					moveSpeed = ChoiceSpeed(NormLateralSpeed, RunLateralSpeed, CrouchLateralSpeed);
					break;
				case Direction.ForwardRight:
					moveSpeed = ChoiceSpeed(NormForwardSpeed, RunForwardSpeed, CrouchForwardSpeed);
					break;
				case Direction.ForwardLeft:
					moveSpeed = ChoiceSpeed(NormForwardSpeed, RunForwardSpeed, CrouchForwardSpeed);
					break;
				case Direction.BackwardRight:
					moveSpeed = ChoiceSpeed(NormBackwardSpeed, RunBackwardSpeed, CrouchBackwardSpeed);
					break;
				case Direction.BackwardLeft:
					moveSpeed = ChoiceSpeed(NormBackwardSpeed, RunBackwardSpeed, CrouchBackwardSpeed);
					break;
			}
			return moveSpeed;
		}

		float ChoiceSpeed(float norm, float run, float crouch)
		{
			float speed;
			
			if (isSprint)
				speed = run;
			else if (isCrouch)
				speed = crouch;
			else
				speed = norm;
			
			return speed;
		}

		IEnumerator MovePause()
		{
			yield return new WaitForSeconds(0.1f);
			isObstacle = false;
			StopCoroutine(MovePause());
		}
		
		IEnumerator ChangeCameraTimeout(CharacterHelper.CameraType type)
		{
			yield return new WaitForSeconds(0.5f);
			CharacterHelper.SwitchCamera(TypeOfCamera, type, this);
			StopCoroutine("ChangeCameraTimeout");
		}

		IEnumerator StepSounds()
		{
			var hit = new RaycastHit();
			while (true)
			{
				if (Physics.Raycast(transform.position, Vector3.down, out hit))
				{
					if (hit.collider.GetComponent<Surface>())
					{
						var surface = hit.collider.GetComponent<Surface>();

						if (rightStepSound && anim.GetFloat("RightFootForward") > 0.8f)
						{
							if(rightAudioSource)
								CharacterHelper.PlayStepSound(surface, rightAudioSource, characterTag);
							rightStepSound = false;
						}

						if (leftStepSound && anim.GetFloat("LeftFootForward") > 0.8f)
						{
							if(leftAudioSource)
								CharacterHelper.PlayStepSound(surface, leftAudioSource, characterTag);
							leftStepSound = false;
						}
					}
				}

				if (anim.GetFloat("RightFootForward") < -0.5)
					rightStepSound = true;
				
				if(anim.GetFloat("LeftFootForward") < -0.5)
					leftStepSound = true;

				yield return 0;
			}
		}

		IEnumerator ChangeBodyHeight()
		{
			while (true)
			{
				var crouchHeight = Mathf.Lerp(CharacterController.center.y, defaultCharacterCenter, 10 * Time.deltaTime);
				CharacterController.center = new Vector3(CharacterController.center.x, crouchHeight, CharacterController.center.z);

				if (Math.Abs(crouchHeight - defaultCharacterCenter) < 0.1f & isCrouch)
				{
					StopCoroutine("ChangeBodyHeight");
					break;
				}
				else if (Math.Abs(crouchHeight - defaultCharacterCenter) < 0.1f & !isCrouch)
				{
					StopCoroutine("ChangeBodyHeight");
					break;
				}

				yield return 0;
			}
		}
		
		#region HealthMethods

        void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Fire"))
            {
                if (other.transform.root.GetComponent<EnemyAttack>())
                {
                    PlayerHealth -= other.transform.root.GetComponent<EnemyAttack>().FireDamage * Time.deltaTime;
                    if (PlayerHealth <= 0)
                    {
                        KillerName = "Enemy";
                        KillMethod = "fried";
                    }
                }
                else if (other.transform.parent.GetComponent<WeaponController>())
                    if (!GetComponent<InventoryManager>().Coop)
                    {
	                    var weaponController = other.transform.parent.GetComponent<WeaponController>();
	                    
                        PlayerHealth -= weaponController.Attacks[weaponController.currentAttack].weapon_damage * Time.deltaTime;
                        
                        if (PlayerHealth <= 0)
                        {
                            KillerName = other.transform.root.GetComponent<Controller>().CharacterName;
                            KillMethod = "fried";
                        }
                    }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("KnifeCollider"))
            {
	            print("eneter");
	            if (!GetComponent<InventoryManager>().Coop)
                {
	                var weaponController = other.transform.parent.GetComponent<WeaponController>();
                    if (PlayerHealth - weaponController.Attacks[weaponController.currentAttack].weapon_damage <= 0)
                    {
                        KillerName = other.transform.root.GetComponent<Controller>().CharacterName;
                        KillMethod = "knifed";
                    }

                    PlayerHealth -= weaponController.Attacks[weaponController.currentAttack].weapon_damage;
                }
            }
        }

        public void Damage(int damage, string killerName)
        {
            if (killerName == "Enemy")
            {
                if (PlayerHealth - damage <= 0)
                {
                    KillerName = killerName;
                    KillMethod = "shot";
                }

                PlayerHealth -= damage;
            }
            else
            {
	            if (WeaponManager.Coop) return;
	            
	            if (PlayerHealth - damage <= 0)
                {
	                KillerName = killerName;
	                KillMethod = "shot";
                }

                PlayerHealth -= damage;
            }
        }

        public void ExplosionDamage(int damage, string killerName)
        {
            if (killerName == "Enemy")
            {
                if (PlayerHealth - damage <= 0)
                {
                    KillerName = killerName;
                    KillMethod = "blew up";
                }

                PlayerHealth -= damage;
            }
            else
            {
                if (!WeaponManager.Coop)
                {
                    if (PlayerHealth - damage <= 0)
                    {
                        KillerName = killerName;
                        KillMethod = "blew up";
                    }

                    PlayerHealth -= damage;
                }
            }
        }

        public void MeleeAttack(int damage)
        {
            if (PlayerHealth - damage <= 0)
            {
                KillerName = "Enemy";
                KillMethod = "killed";
            }

            PlayerHealth -= damage;
        }
        
        #endregion
		
		#region FeetIK

		void OnAnimatorIK(int layerIndex)
		{
			IKVariables.LastPelvisPosition = anim.bodyPosition.y;
			
			if (layerIndex != 1) return;

			if (!isMultiplayerCharacter)
			{
				if (TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
				{
					if (isJump && (anim.GetCurrentAnimatorStateInfo(1).IsName("Jumple_Idle") || anim.GetCurrentAnimatorStateInfo(1).IsName("Jump_Movemement_L")
					                                                                         || anim.GetCurrentAnimatorStateInfo(1).IsName("Falling Loop") ||
					                                                                         anim.GetCurrentAnimatorStateInfo(1).IsName("Jump_Land_Hard")
					                                                                         || anim.GetCurrentAnimatorStateInfo(1).IsName("Jump_Land_Walk Loop") ||
					                                                                         anim.GetCurrentAnimatorStateInfo(1).IsName("Start Falling")))
					{
						if (SmoothIKSwitch > 0.1f)
							SmoothIKSwitch = Mathf.Lerp(SmoothIKSwitch, 0, 5 * Time.deltaTime);
						else SmoothIKSwitch = 0;
					}
					else
					{
						if (SmoothIKSwitch < 0.9f)
							SmoothIKSwitch = Mathf.Lerp(SmoothIKSwitch, 0, 5 * Time.deltaTime);
						else SmoothIKSwitch = 1;
					}
				}
				else
				{
					if (!isJump)
						SmoothIKSwitch = 1;
				}
			}

			anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, SmoothIKSwitch);
			anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("RightFoot"));

			IKHelper.MoveFeetToIkPoint(this, "right");

			anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, SmoothIKSwitch);
			anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("LeftFoot"));

			IKHelper.MoveFeetToIkPoint(this, "left");
		}

		void FixedUpdate()
		{
			IKHelper.AdjustFeetTarget(this, "right");
			IKHelper.AdjustFeetTarget(this, "left");

			IKHelper.FeetPositionSolver(this, "right");
			IKHelper.FeetPositionSolver(this, "left");
		}

		#endregion
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if(!Application.isPlaying || AdjustmentScene)
				return;
			
			Handles.zTest = CompareFunction.Less;
			Handles.color = new Color32(255, 150, 0, 255);
			Helper.DrawWireCapsule(transform.position + characterCollider.center, transform.rotation, characterCollider.radius, characterCollider.height, Handles.color);
			
			Handles.zTest = CompareFunction.Greater;
			Handles.color = new Color32(255, 150, 0, 50);
			Helper.DrawWireCapsule(transform.position + characterCollider.center, transform.rotation, characterCollider.radius, characterCollider.height, Handles.color);
			
			Handles.zTest = CompareFunction.Less;
			Handles.color = new Color32(255, 255, 255, 255);
			Handles.DrawWireDisc(BodyObjects.Hips.position, transform.up, noiseRadius);
			
			Handles.zTest = CompareFunction.Greater;
			Handles.color = new Color32(255, 255, 255, 50);
			Handles.DrawWireDisc(BodyObjects.Hips.position, transform.up, noiseRadius);
		}
#endif
	}
}


