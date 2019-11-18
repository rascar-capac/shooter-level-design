// GercStudio
// © 2018-2019

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	public class CameraController : MonoBehaviour
	{
		public CameraController OriginalScript;

		public CharacterHelper.CameraOffset CameraOffset = new CharacterHelper.CameraOffset();

		public Controller thisController;

		private Transform targetLookAt;
		public Transform CameraPosition;
		public Transform Crosshair;
		public Transform PickUpCrosshair;
		public GameObject LayerCamera;

		public float CrosshairOffsetX;
		public float CrosshairOffsetY = 300;

		public float maxMouseAbsolute;
		public float minMouseAbsolute;
		public float cameraMovementDistance = 5;

		public int inspectorTab;
		
		public bool CameraAim;
		public bool deepAim;
		public bool Occlusion;
		public bool cameraDebug;
		public bool setCameraType;
		public bool canViewTarget;

		public Camera Camera;
		public Camera AimCamera;

		public Vector2 targetDirection;
		public Vector2 _mouseAbsolute;
		public Vector2 _smoothMouse;

		private Vector3 desiredCameraPosition = Vector3.zero;
		private Vector3 desiredBodyLookAtPosition = Vector3.zero;
		private Vector3 _position = Vector3.zero;
		private Vector3 bodyLookAtPosition = Vector3.zero;

		private Vector2 mouseDelta;
		private Vector2 TPmouseDelta;

		private Quaternion desiredRotation;

		private GameObject disabledObject = null;
		private Transform preOcclededCamera;
		public Transform CameraPos;
		public Transform bodylookat;
		private Transform body;

		private float CurrentSensitivityX;
		private float CurrentSensitivityY;
		public float mouseX;
		public float mouseY;
		private float velX;
		private float velY;
		private float velZ;
		private float normDepth;
		private float CurrentDistance;
		private float CurrentOffsetX;
		private float CurrentOffsetY;
		private float desiredDistance;

		private float desiredOffsetX;

//		private float desiredOffsetY;
		private float textuteAlpha;

		private int touchId = -1;

		private Collider[] _occlusionCollisers = new Collider[2];

		void Start()
		{
			if (thisController == null)
			{
				Debug.Log("Disconnect between camera and controller");
				return;
			}

			Camera = GetComponent<Camera>();
			LayerCamera = Helper.NewCamera("LayerCamera", transform, "CameraController").gameObject;
			LayerCamera.SetActive(false);
			LayerCamera.hideFlags = HideFlags.HideInHierarchy;

			normDepth = Camera.fieldOfView;

			CameraPos = new GameObject("Camera").transform;

			if (thisController.WeaponManager.aimTextureImage)
				thisController.WeaponManager.aimTextureImage.gameObject.SetActive(false);

			preOcclededCamera = new GameObject("preoclCamera").transform;
			preOcclededCamera.parent = CameraPos;
			preOcclededCamera.localPosition = Vector3.zero;
			preOcclededCamera.hideFlags = HideFlags.HideInHierarchy;

			transform.parent = CameraPos;
			transform.position = new Vector3(0, 0, 0);
			transform.rotation = Quaternion.Euler(0, 0, 0);

			body = thisController.BodyObjects.TopBody;
			bodylookat = new GameObject("BodyLookAt").transform;
			bodylookat.hideFlags = HideFlags.HideInHierarchy;
			targetDirection = CameraPos.localEulerAngles;

			CameraOffset.tpCameraOffsetX = CameraOffset.normCameraOffsetX;
			CameraOffset.tpCameraOffsetY = CameraOffset.normCameraOffsetY;
			CameraOffset.Distance = CameraOffset.normDistance;
			
			if (CameraPosition)
			{
				CameraPosition.parent = thisController.BodyObjects.Head;
				CameraPosition.localPosition = CameraOffset.cameraObjPos;
				CameraPosition.localEulerAngles = CameraOffset.cameraObjRot;
			}
			else
			{
				Debug.LogError("<color=red>Missing component</color>: [Camera position]", gameObject);
				Debug.Break();
			}

			if (thisController.CameraParameters.ActiveFP && thisController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				thisController.TypeOfCamera = CharacterHelper.CameraType.FirstPerson;
			else if (thisController.CameraParameters.ActiveTP & thisController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
				thisController.TypeOfCamera = CharacterHelper.CameraType.ThirdPerson;
			else if (thisController.CameraParameters.ActiveTD & thisController.TypeOfCamera == CharacterHelper.CameraType.TopDown)
				thisController.TypeOfCamera = CharacterHelper.CameraType.TopDown;
			else if (thisController.CameraParameters.ActiveFP)
				thisController.TypeOfCamera = CharacterHelper.CameraType.FirstPerson;
			else if (thisController.CameraParameters.ActiveTD)
				thisController.TypeOfCamera = CharacterHelper.CameraType.TopDown;
			else if (thisController.CameraParameters.ActiveTP)
				thisController.TypeOfCamera = CharacterHelper.CameraType.ThirdPerson;
			else
			{
				Debug.LogError("Please select any active camera view.", gameObject);
				Debug.Break();
			}

			AimCamera.gameObject.SetActive(false);

			SetAnimVariables();

			setCameraType = true;

			if (!Crosshair && !thisController.AdjustmentScene)
			{
				Debug.LogWarning("<color=yellow>Missing component</color> [Crosshair].");
			}

//			if (PickUpCrosshair)
//				PickUpCrosshair.gameObject.SetActive(false);
			

			Reset();
		}

		void Update()
		{
			if (thisController && !thisController.ActiveCharacter)
			{
				if (GetComponent<Camera>().enabled)
					GetComponent<Camera>().enabled = false;

				if (AimCamera.enabled)
					AimCamera.enabled = false;
			}

			if (thisController.ActiveCharacter)
			{
				if (!thisController.isPause && !thisController.WeaponManager.inventoryWheel.activeSelf && !thisController.isMultiplayerCharacter)
				{

					if (Crosshair)
					{
						Crosshair.gameObject.SetActive(!thisController.WeaponManager.isPickUp && thisController.WeaponManager.weaponController &&
						                               !thisController.WeaponManager.weaponController.wallDetect &&
						                               (thisController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && !CameraAim ||
						                                thisController.TypeOfCamera != CharacterHelper.CameraType.FirstPerson) &&
						                               (thisController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraAim ||
						                                thisController.TypeOfCamera != CharacterHelper.CameraType.ThirdPerson));

						if (thisController.TypeOfCamera == CharacterHelper.CameraType.TopDown)
						{
							if (Crosshair.GetComponent<RectTransform>().anchoredPosition != new Vector2(CrosshairOffsetX, CrosshairOffsetY))
							{
								Crosshair.GetComponent<RectTransform>().anchoredPosition = new Vector2(CrosshairOffsetX, CrosshairOffsetY);
								Crosshair.gameObject.SetActive(false);
								Crosshair.gameObject.SetActive(true);
							}
						}
						else
						{
							if (Crosshair.GetComponent<RectTransform>().anchoredPosition != new Vector2(0, 0))
								Crosshair.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
						}
					}

					if (!Application.isMobilePlatform)
					{
						if (PickUpCrosshair)
							PickUpCrosshair.gameObject.SetActive(thisController.WeaponManager.isPickUp);
					}
					else
					{
						if (thisController.uiButtons[11])
							thisController.uiButtons[11].gameObject.SetActive(thisController.WeaponManager.isPickUp);
					}
				}
				else
				{
					if (Crosshair)
						Crosshair.gameObject.SetActive(false);

					if (PickUpCrosshair)
						PickUpCrosshair.gameObject.SetActive(false);

					if (thisController.uiButtons[11])
						thisController.uiButtons[11].gameObject.SetActive(false);
				}
			}

			if (!thisController.WeaponManager.weaponController)
			{
				if (Input.GetKeyDown(thisController._gamepadCodes[5]) || Input.GetKeyDown(thisController._keyboardCodes[5]) ||
				    Helper.CheckGamepadAxisButton(5, thisController._gamepadButtonsAxes, thisController.hasAxisButtonPressed,
					    "GetKeyDown", thisController.inputs.AxisButtonValues[5]))
					Aim();
			}


			if (!CameraAim)
			{
				if (Math.Abs(CameraOffset.tpCameraOffsetX - CameraOffset.normCameraOffsetX) > 0.1f)
					CameraOffset.tpCameraOffsetX = Mathf.Lerp(CameraOffset.tpCameraOffsetX, CameraOffset.normCameraOffsetX, 5 * Time.deltaTime);

				if (Math.Abs(CameraOffset.tpCameraOffsetY - CameraOffset.normCameraOffsetY) > 0.1f)
					CameraOffset.tpCameraOffsetY = Mathf.Lerp(CameraOffset.tpCameraOffsetY, CameraOffset.normCameraOffsetY, 5 * Time.deltaTime);

				if (Math.Abs(CameraOffset.Distance - CameraOffset.normDistance) > 0.1f)
				{
					CameraOffset.Distance = Mathf.Lerp(CameraOffset.Distance, CameraOffset.normDistance, 10 * Time.deltaTime);
					Reset();
				}
				
				if(thisController.AdjustmentScene)
					Reset();
			}
			else
			{
				if (Math.Abs(CameraOffset.tpCameraOffsetX - CameraOffset.aimCameraOffsetX) > 0.1f)
					CameraOffset.tpCameraOffsetX = Mathf.Lerp(CameraOffset.tpCameraOffsetX, CameraOffset.aimCameraOffsetX, 5 * Time.deltaTime);
				
				if (Math.Abs(CameraOffset.tpCameraOffsetY - CameraOffset.aimCameraOffsetY) > 0.1f)
					CameraOffset.tpCameraOffsetY = Mathf.Lerp(CameraOffset.tpCameraOffsetY, CameraOffset.aimCameraOffsetY, 5 * Time.deltaTime);

				if (Math.Abs(CameraOffset.Distance - CameraOffset.aimDistance) > 0.1f)
				{
					CameraOffset.Distance = Mathf.Lerp(CameraOffset.Distance, CameraOffset.aimDistance, 10 * Time.deltaTime);
					Reset();
				}
					
				if(thisController.AdjustmentScene)
					Reset();
			}
			

			LayerCamera.GetComponent<Camera>().fieldOfView = Camera.fieldOfView;
			
			if (cameraDebug)
				Reset();


			desiredOffsetX = CurrentOffsetX;

			if (CameraAim)
			{
				switch (thisController.TypeOfCamera)
				{
					case CharacterHelper.CameraType.FirstPerson:
						
						Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, thisController.CameraParameters.FPAimDepth, 0.3f);

						if (Math.Abs(Camera.fieldOfView - thisController.CameraParameters.FPAimDepth) < 10)
						{
							if (thisController.WeaponManager.weaponController && thisController.WeaponManager.weaponController.UseAimTexture)
							{
								AimCamera.fieldOfView = Mathf.Lerp(AimCamera.fieldOfView,
									thisController.WeaponManager.weaponController.aimTextureDepth, 0.5f);
								thisController.WeaponManager.aimTextureImage.gameObject.SetActive(true);

								var color = thisController.WeaponManager.aimTextureImage.GetComponent<RawImage>().color;

								color.a = Mathf.Lerp(color.a, 1, 0.5f);

								thisController.WeaponManager.aimTextureImage.GetComponent<RawImage>().color = color;
							}
						}

						if (Math.Abs(Camera.fieldOfView - thisController.CameraParameters.FPAimDepth) < 7)
						{
							if (thisController.WeaponManager.weaponController && thisController.WeaponManager.weaponController.UseAimTexture)
							{
								AimCamera.targetTexture = null;
								AimCamera.gameObject.SetActive(true);
							}
						}

						break;
					case CharacterHelper.CameraType.ThirdPerson:
						
						Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, thisController.CameraParameters.TPAimDepth, 0.3f);
						
						DisableAimTextures();
						
//						if (Math.Abs(Camera.fieldOfView - thisController.CameraParameters.TPAimDepth) < 2)
//						{
//							if (thisController.WeaponManager.weaponController && thisController.WeaponManager.weaponController.UseAimTexture)
//							{
//								AimCamera.targetTexture = null;
//
//								thisController.WeaponManager.aimTextureImage.gameObject.SetActive(true);
//								AimCamera.gameObject.SetActive(true);
//							}
//						}

						break;
				}
			}
			else
			{
				Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, normDepth, 0.3f);

				DisableAimTextures();
			}
		}

		void LateUpdate()
		{
//			if(thisController && !thisController.ActiveCharacter)
//				return;

			if (thisController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
			{
				if (!CameraPosition)
					return;

				FpCameraRotation();
			}
			else
			{
				GetMouseAxis();

				switch (thisController.TypeOfCamera)
				{
					case CharacterHelper.CameraType.ThirdPerson:
						tpCheckIfOccluded();
						ChangeCurrentPosition();
						tpCalculateDesiredPosition();
						break;
					case CharacterHelper.CameraType.TopDown:
						tdCalculateDesiredPosition();
						CharacterHelper.CheckCameraPoints(thisController.BodyObjects.Head.position, desiredCameraPosition, disabledObject, Camera);
						break;
				}

				if(Time.timeScale > 0)
					UpdatePosition();
			}
		}

		void ChangeCurrentPosition()
		{
			CurrentDistance = Mathf.Lerp(CurrentDistance, desiredDistance, 0.6f);

			CurrentOffsetX = Mathf.Lerp(CurrentOffsetX, desiredOffsetX, 0.6f);

//			CurrentOffsetY = Mathf.Lerp(CurrentOffsetY, desiredOffsetY, 0.1f);
		}

		void tdCalculateDesiredPosition()
		{
			desiredCameraPosition = CharacterHelper.CalculatePosition(mouseY, mouseX, 10, thisController.TypeOfCamera, thisController, CameraOffset.TopDownAngle - 10);
		}

		void tpCalculateDesiredPosition()
		{
			desiredCameraPosition = CharacterHelper.CalculatePosition(mouseY, mouseX, cameraMovementDistance, thisController.TypeOfCamera, thisController, 0);
			desiredBodyLookAtPosition = CharacterHelper.CalculatePosition(mouseY, mouseX, 5, thisController.TypeOfCamera, thisController, 0);
		}

		void UpdatePosition()
		{
			transform.localEulerAngles = Vector3.zero;
			switch (thisController.TypeOfCamera)
			{
				case CharacterHelper.CameraType.ThirdPerson:
				{
					var posX = Mathf.SmoothDamp(_position.x, desiredCameraPosition.x, ref velX, thisController.CameraParameters.tpSmoothX);
					var posY = Mathf.SmoothDamp(_position.y, desiredCameraPosition.y, ref velY, thisController.CameraParameters.tpSmoothY);
					var posZ = Mathf.SmoothDamp(_position.z, desiredCameraPosition.z, ref velZ, thisController.CameraParameters.tpSmoothX);
					_position = new Vector3(posX, posY, posZ);

					if (setCameraType)
					{
						if (thisController.CameraSmoothWhenJumping)
							CameraPos.position = _position;
						else
						{
							CameraPos.position = new Vector3(_position.x,
								Mathf.Lerp(CameraPos.position.y, _position.y, 2.5f * Time.deltaTime), _position.z);
						}

						transform.localPosition = new Vector3(CurrentOffsetX, CurrentOffsetY, CurrentDistance);
						CameraPos.LookAt(thisController.BodyObjects.Head);
					}
					else
					{
						CameraPos.position = Helper.MoveObjInNewPosition(CameraPos.position, _position, 0.5f);
						transform.localPosition = Helper.MoveObjInNewPosition(transform.localPosition,
							new Vector3(CurrentOffsetX, CurrentOffsetY, CurrentDistance), 0.5f);

						if (Math.Abs(CameraPos.position.x - _position.x) < 0.1f & Math.Abs(CameraPos.position.y - _position.y) < 0.1f &
						    Math.Abs(CameraPos.position.z - _position.z) < 0.1f & Math.Abs(transform.localPosition.x - CurrentOffsetX) < 0.01f &
						    Math.Abs(transform.localPosition.y - CurrentOffsetY) < 0.01f & Math.Abs(transform.localPosition.z - CurrentDistance) < 0.01f)
						{
							setCameraType = true;
						}

						if (canViewTarget)
							CameraPos.LookAt(thisController.BodyObjects.Head);
					}

					var bodyPosX = Mathf.SmoothDamp(bodyLookAtPosition.x, desiredBodyLookAtPosition.x, ref velX, thisController.CameraParameters.tpSmoothX);
					var bodyPosY = Mathf.SmoothDamp(bodyLookAtPosition.y, desiredBodyLookAtPosition.y, ref velY, thisController.CameraParameters.tpSmoothY);
					var bodyPosZ = Mathf.SmoothDamp(bodyLookAtPosition.z, desiredBodyLookAtPosition.z, ref velZ, thisController.CameraParameters.tpSmoothX);
					
					bodyLookAtPosition = new Vector3(bodyPosX, bodyPosY, bodyPosZ);

					bodylookat.position = bodyLookAtPosition;
					bodylookat.RotateAround(thisController.BodyObjects.Head.position, Vector3.right, 180);

					var newPos = bodylookat.position;
					bodylookat.position = bodyLookAtPosition;
					bodylookat.RotateAround(thisController.BodyObjects.Head.position, Vector3.up, 185);

					bodylookat.position = new Vector3(bodylookat.position.x, newPos.y, bodylookat.position.z);
					break;
				}

				case CharacterHelper.CameraType.TopDown:
				{
					var posX = Mathf.SmoothDamp(_position.x, desiredCameraPosition.x, ref velX, thisController.CameraParameters.tdSmoothX);
					var posY = Mathf.SmoothDamp(_position.y, desiredCameraPosition.y, ref velY, thisController.CameraParameters.tdSmoothX);
					var posZ = Mathf.SmoothDamp(_position.z, desiredCameraPosition.z, ref velZ, thisController.CameraParameters.tdSmoothX);
					_position = new Vector3(posX, posY, posZ);

					if (setCameraType)
					{
						CameraPos.position = _position;
						transform.localPosition = new Vector3(CameraOffset.tdCameraOffsetX, CameraOffset.tdCameraOffsetY, CameraOffset.TD_Distance);
						CameraPos.LookAt(thisController.BodyObjects.Head);
					}
					else
					{
						CameraPos.position = Helper.MoveObjInNewPosition(CameraPos.position, _position, 0.5f);
						transform.localPosition = Helper.MoveObjInNewPosition(transform.localPosition,
							new Vector3(CameraOffset.tdCameraOffsetX, CameraOffset.tdCameraOffsetY, CameraOffset.TD_Distance), 0.5f);

						if (Math.Abs(CameraPos.position.x - _position.x) < 0.1f &
						    Math.Abs(CameraPos.position.y - _position.y) < 0.1f &
						    Math.Abs(CameraPos.position.z - _position.z) < 0.1f &
						    Math.Abs(transform.localPosition.x - CameraOffset.tdCameraOffsetX) < 0.01f &
						    Math.Abs(transform.localPosition.y - CameraOffset.tdCameraOffsetY) < 0.01f &
						    Math.Abs(transform.localPosition.z - CameraOffset.TD_Distance) < 0.01f)
						{
							setCameraType = true;
						}

						if (canViewTarget)
							CameraPos.LookAt(thisController.BodyObjects.Head);
					}



					bodylookat.position = new Vector3(CameraPos.position.x, thisController.BodyObjects.Hips.position.y,
						CameraPos.position.z);

					bodylookat.RotateAround(thisController.BodyObjects.Head.position, Vector3.up, 180);

					bodyLookAtPosition = _position;
					break;
				}
			}

			if (body)
				thisController.BodyLookAt(bodylookat);

			thisController.BodyRotate();
		}

		public IEnumerator cameraTimeout()
		{
			yield return new WaitForSeconds(0.01f);
			canViewTarget = true;
			StopCoroutine("cameraTimeout");
		}

		void CheckTouchCamera()
		{
			if (Input.touchCount > 0)
			{
				for (var i = 0; i < Input.touches.Length; i++)
				{
					var touch = Input.GetTouch(i);

					if (touch.position.x > Screen.width / 2 & touchId == -1 &
					    touch.phase == TouchPhase.Began)
					{
						touchId = touch.fingerId;
					}

					if (touch.fingerId == touchId)
					{
						if (touch.position.x > Screen.width / 2)
							mouseDelta = touch.deltaPosition / 75;
						else
						{
							mouseDelta = Vector2.zero;
						}

						if (touch.phase == TouchPhase.Ended)
						{
							touchId = -1;
							mouseDelta = Vector2.zero;
						}
					}
				}
			}
		}

		void FpCameraRotation()
		{
			mouseY = 0;
			mouseX = CameraPos.eulerAngles.y;
			
			var targetOrientation = Quaternion.Euler(targetDirection);

			if (!thisController.isPause && thisController.ActiveCharacter)
			{
				if (Application.isMobilePlatform & !thisController.WeaponManager.GamepadConnect)
					CheckTouchCamera();
				else
				{
					if (!thisController.WeaponManager.GamepadConnect)
					{
						mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X") / 2, Input.GetAxisRaw("Mouse Y") / 2);
					}
					else
					{
						var Horizontal = Input.GetAxisRaw(thisController._gamepadAxes[2]) / 3;
						if (thisController.inputs.invertAxes[2])
							Horizontal *= -1;
						var Vertical = Input.GetAxisRaw(thisController._gamepadAxes[3]) / 3;
						if (thisController.inputs.invertAxes[3])
							Vertical *= -1;

						mouseDelta = new Vector2(Horizontal, Vertical);
					}
				}


				mouseDelta = Vector2.Scale(mouseDelta,
					new Vector2(CurrentSensitivityX * thisController.CameraParameters.fps_X_Smooth, CurrentSensitivityY * thisController.CameraParameters.fps_Y_Smooth));

				_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1 / thisController.CameraParameters.fps_X_Smooth);
				_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1 / thisController.CameraParameters.fps_Y_Smooth);
				_mouseAbsolute += _smoothMouse;
			}

			if (_mouseAbsolute.y > maxMouseAbsolute)
			{
				_mouseAbsolute.y = maxMouseAbsolute;
			}
			else if (_mouseAbsolute.y < minMouseAbsolute)
			{
				_mouseAbsolute.y = minMouseAbsolute;
			}

			var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);

			desiredRotation = yRotation * Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

			if (setCameraType)
			{
				CameraPos.rotation = desiredRotation;
				body.rotation = CameraPos.rotation;

				thisController.TopBodyOffset();
				thisController.BodyRotate();

				transform.rotation = Quaternion.Euler(CameraPosition.eulerAngles.x, CameraPosition.eulerAngles.y, 0);
				//transform.localEulerAngles = new Vector3(0, 50, 0);
				CameraPos.position = CameraPosition.position;
			}
			else
			{
			
				_mouseAbsolute.x = 17;

				if (thisController.isCrouch)
				{
					_mouseAbsolute.x = -17;
					_mouseAbsolute.y = 55;
				}

				transform.localPosition = Helper.MoveObjInNewPosition(transform.localPosition, Vector3.zero, 0.8f);

				CameraPos.rotation = Quaternion.Slerp(CameraPos.rotation, desiredRotation, 0.5f);
				body.rotation = CameraPos.rotation;

				thisController.TopBodyOffset();
				thisController.BodyRotate();

				transform.rotation = Quaternion.Slerp(transform.rotation,
					Quaternion.Euler(CameraPosition.eulerAngles.x, CameraPosition.eulerAngles.y, 0), 0.5f);
				CameraPos.position = Helper.MoveObjInNewPosition(CameraPos.position, CameraPosition.position, 0.5f);

				if (Math.Abs(CameraPos.position.x - CameraPosition.position.x) < 0.6f &
				    Math.Abs(CameraPos.position.y - CameraPosition.position.y) < 0.6f &
				    Math.Abs(CameraPos.position.z - CameraPosition.position.z) < 0.6f)
				{
					setCameraType = true;
				}

			}
		}

		void GetMouseAxis()
		{
			if (thisController.isPause || !thisController.ActiveCharacter) return;

			if (!Application.isMobilePlatform)
			{
				if (!thisController.WeaponManager.GamepadConnect)
				{
					mouseX += Input.GetAxis("Mouse X") * CurrentSensitivityX;
					mouseY -= Input.GetAxis("Mouse Y") * CurrentSensitivityY;
				}
				else
				{
					var Horizontal = Input.GetAxisRaw(thisController._gamepadAxes[2]) * CurrentSensitivityX;
					if (thisController.inputs.invertAxes[2])
						Horizontal *= -1;

					var Vertical = Input.GetAxisRaw(thisController._gamepadAxes[3]) * CurrentSensitivityY;
					if (thisController.inputs.invertAxes[3])
						Vertical *= -1;

					mouseX += Horizontal;
					mouseY -= Vertical;
				}
			}
			else
			{
				if (Input.touchCount > 0)
				{
					for (var i = 0; i < Input.touches.Length; i++)
					{
						var touch = Input.GetTouch(i);

						if (touch.position.x > Screen.width / 2 & touchId == -1 &
						    touch.phase == TouchPhase.Began)
						{
							touchId = touch.fingerId;
						}

						if (touch.fingerId == touchId)
						{
							if (touch.position.x > Screen.width / 2)
							{
								TPmouseDelta = touch.deltaPosition / 10;
							}
							else
							{
								TPmouseDelta = Vector2.zero;
							}

							if (touch.phase == TouchPhase.Ended)
							{
								touchId = -1;
								TPmouseDelta = Vector2.zero;
							}
						}
					}
				}
			}

			var vector = new Vector2(mouseX, mouseY);

			vector.x += TPmouseDelta.x;
			vector.y -= TPmouseDelta.y;

			mouseX = vector.x;
			mouseY = vector.y;

			mouseY = Helper.ClampAngle(mouseY, thisController.CameraParameters.Y_MinLimit, thisController.CameraParameters.Y_MaxLimit);
		}

		public void DeepAim()
		{
			if (!deepAim)
			{
				if (thisController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
				{
					if (!CameraAim)
						Aim();
					
					thisController.ChangeCameraType(CharacterHelper.CameraType.FirstPerson);
					deepAim = true;

					SetSensitivity();
				}
			}
			else
			{
				if (thisController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				{
					if (CameraAim)
						Aim();
					
					thisController.ChangeCameraType(CharacterHelper.CameraType.ThirdPerson);
					deepAim = false;
				}
			}
		}

		public void Aim()
		{
			if (thisController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !CameraAim)
			{
				CameraAim = true;

//				var angle = Helper.AngleBetween(thisController.transform.forward, transform.forward);
//				thisController.anim.SetFloat("Angle", angle);

				thisController.anim.SetBool("Aim", true);
				normDepth = Camera.fieldOfView;
				Reset();
			}
			else if (thisController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraAim)
			{
				thisController.anim.SetBool("Aim", false);
				CameraAim = false;
				Reset();
			}
			else if (thisController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && !CameraAim)
			{
				CameraAim = true;
				thisController.anim.SetBool("Aim", true);
				normDepth = Camera.fieldOfView;
				Reset();
			}
			else if (thisController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && CameraAim)
			{
				thisController.anim.SetBool("Aim", false);
				CameraAim = false;

				if (deepAim)
				{
					DeepAim();
				}

				Reset();
			}
		}
		
		void DisableAimTextures()
		{
			if (thisController.WeaponManager.weaponController)
				if (thisController.WeaponManager.weaponController.UseAimTexture)
				{
					var color = thisController.WeaponManager.aimTextureImage.GetComponent<RawImage>().color;

					color.a = Mathf.Lerp(color.a, 0, 0.5f);

					thisController.WeaponManager.aimTextureImage.GetComponent<RawImage>().color = color;

					if (color.a <= 0)
					{
						thisController.WeaponManager.aimTextureImage.gameObject.SetActive(false);
					}
				}

			if (thisController.WeaponManager.weaponController)
				if (thisController.WeaponManager.weaponController.UseScope)
				{
					if (!AimCamera.gameObject.activeSelf)
						AimCamera.gameObject.SetActive(true);

					AimCamera.fieldOfView = Mathf.Lerp(AimCamera.fieldOfView,
						thisController.WeaponManager.weaponController.scopeDepth, 0.5f);
					AimCamera.targetTexture = thisController.WeaponManager.ScopeScreenTexture;
				}
				else
				{
					AimCamera.targetTexture = null;
					if (AimCamera.gameObject.activeSelf)
						AimCamera.gameObject.SetActive(false);
				}
		}

		public void SetSensitivity()
		{
			if (thisController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
			{
				if (CameraAim)
				{
					CurrentSensitivityX = thisController.CameraParameters.AimX_MouseSensitivity;
					CurrentSensitivityY = thisController.CameraParameters.AimY_MouseSensitivity;
				}
				else
				{
					CurrentSensitivityX = thisController.CameraParameters.X_MouseSensitivity;
					CurrentSensitivityY = thisController.CameraParameters.Y_MouseSensitivity;
				}
			}
			else
			{
				if (CameraAim)
				{
					CurrentSensitivityX = thisController.CameraParameters.fps_AimX_MouseSensitivity;
					CurrentSensitivityY = thisController.CameraParameters.fps_AimY_MouseSensitivity;
				}
				else
				{
					CurrentSensitivityX = thisController.CameraParameters.fps_X_MouseSensitivity;
					CurrentSensitivityY = thisController.CameraParameters.fps_Y_MouseSensitivity;
				}
			}
		}

		public void Reset()
		{
			if(!thisController)
				return;
			
			switch (thisController.TypeOfCamera)
			{
				case CharacterHelper.CameraType.ThirdPerson:
				{
					SetSensitivity();

					CurrentDistance = CameraOffset.Distance;
					desiredDistance = CurrentDistance;

					CurrentOffsetX = CameraOffset.tpCameraOffsetX;
					CurrentOffsetY = CameraOffset.tpCameraOffsetY;

					desiredOffsetX = CurrentOffsetX;
					
					if (preOcclededCamera)
						preOcclededCamera.localPosition = new Vector3(CameraOffset.tpCameraOffsetX, CameraOffset.tpCameraOffsetY, CameraOffset.Distance);

					break;
				}

				case CharacterHelper.CameraType.TopDown:
				{
					CurrentSensitivityX = thisController.CameraParameters.TD_X_MouseSensitivity;
					break;
				}

				case CharacterHelper.CameraType.FirstPerson:
				{
					SetSensitivity();
					break;
				}
			}
		}

		public void SetAnimVariables()
		{
			switch (thisController.TypeOfCamera)
			{
				case CharacterHelper.CameraType.ThirdPerson:
				case CharacterHelper.CameraType.TopDown:

					if (thisController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
						thisController.anim.SetBool("TPS", true);
					else thisController.anim.SetBool("TDS", true);
					
					if (thisController.WeaponManager.hasAnyWeapon)
					{
						thisController.anim.SetLayerWeight(3, 1);
						thisController.anim.SetLayerWeight(2, 0);

						thisController.currentAnimatorLayer = 3;
					}

					break;

				case CharacterHelper.CameraType.FirstPerson:

					thisController.anim.SetBool("FPS", true);
					thisController.anim.SetLayerWeight(3, 0);
					thisController.anim.SetLayerWeight(2, 1);
					thisController.currentAnimatorLayer = 2;

					break;
			}
		}

		void tpCheckIfOccluded()
		{
			RaycastHit Hit;
			var nearestDistance = CharacterHelper.CheckCameraPoints(thisController.BodyObjects.Head.position, transform.position, disabledObject, transform, Camera);

			if (nearestDistance > -1)
			{
				desiredDistance += 0.2f;

				if (desiredDistance > 4.5f)
					desiredDistance = 4.5f;

				if (Physics.Raycast(transform.position - transform.right * 2, transform.right, out Hit, 5))
				{
					desiredOffsetX -= 0.1f;

					if (desiredOffsetX < 0.3f)
						desiredOffsetX = 0.3f;
				}
			}
			else
			{
				nearestDistance = CharacterHelper.CheckCameraPoints(thisController.BodyObjects.Head.position, preOcclededCamera.position, 
					disabledObject, transform, Camera);

				var canChangeDist = true;

				var size = Physics.OverlapSphereNonAlloc(preOcclededCamera.position, 1, _occlusionCollisers);

				if (size > 0)
				{
					canChangeDist = false;
				}

				if (!Physics.Raycast(transform.position - transform.right * 3, transform.right, out Hit, 6) &
				    !Physics.Raycast(transform.position + transform.right * 3, transform.right * -1, out Hit, 6) &
				    canChangeDist)
				{
					desiredOffsetX += 0.1f;

					if (desiredOffsetX > CameraOffset.tpCameraOffsetX)
						desiredOffsetX = CameraOffset.tpCameraOffsetX;
				}

				if (nearestDistance <= -1 & canChangeDist)
				{
					desiredDistance += 0.01f;
					if (desiredDistance > CameraOffset.Distance)
						desiredDistance = CameraOffset.Distance;
				}
			}
		}
	}
}


