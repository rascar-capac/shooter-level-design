using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
	public static class CharacterHelper
	{
		[Serializable]
		public class CharacterOffset
		{

			public Vector3 directionObjRotation;
			
			public float xRotationOffset;
			public float yRotationOffset;
			public float zRotationOffset;
			public float CharacterHeight = -1.1f;
			
			public Vector3 SaveTime;
			public Vector3 SaveDate;
			
			public bool HasTime;

			public void Clone(CharacterOffset cloneFrom)
			{
				directionObjRotation = cloneFrom.directionObjRotation;
				
				xRotationOffset = cloneFrom.xRotationOffset;
				yRotationOffset = cloneFrom.yRotationOffset;
				zRotationOffset = cloneFrom.zRotationOffset;
				CharacterHeight = cloneFrom.CharacterHeight;

				SaveTime = cloneFrom.SaveTime;
				SaveDate = cloneFrom.SaveDate;
				HasTime = cloneFrom.HasTime;
			}
		}

		[Serializable]
		public class CameraOffset
		{
			public float tpCameraOffsetX;
			public float tpCameraOffsetY;
			
			public float normCameraOffsetX;
			public float normCameraOffsetY;
			public float normDistance;
			
			public float aimCameraOffsetX;
			public float aimCameraOffsetY;
			public float aimDistance;

			public float tdCameraOffsetX;
			public float tdCameraOffsetY;

			public Vector3 cameraObjPos;
			public Vector3 cameraObjRot;

			public float TD_Distance;
			public float Distance;
			public float TopDownAngle = 80;

			public void Clone(CameraOffset cloneFrom)
			{
				tpCameraOffsetX = cloneFrom.tpCameraOffsetX;
				tpCameraOffsetY = cloneFrom.tpCameraOffsetY;
				
				aimCameraOffsetX = cloneFrom.aimCameraOffsetX;
				aimCameraOffsetY = cloneFrom.aimCameraOffsetY;
				
				normCameraOffsetX = cloneFrom.normCameraOffsetX;
				normCameraOffsetY = cloneFrom.normCameraOffsetY;

				tdCameraOffsetX = cloneFrom.tdCameraOffsetX;
				tdCameraOffsetY = cloneFrom.tdCameraOffsetY;

				cameraObjPos = cloneFrom.cameraObjPos;
				cameraObjRot = cloneFrom.cameraObjRot;

				TopDownAngle = cloneFrom.TopDownAngle;
				TD_Distance = cloneFrom.TD_Distance;
				Distance = cloneFrom.Distance;
				normDistance = cloneFrom.normDistance;
				aimDistance = cloneFrom.aimDistance;
			}
		}
		
		[Serializable]
		public class BodyObjects
		{
			public Transform RightHand;
			public Transform LeftHand;
			public Transform TopBody;
			public Transform Head;
			public Transform Hips;
			public Transform Chest;
		}

		[Serializable]
		public class CameraParameters
		{
			[Range(60, 30)] public float TPAimDepth = 40f;
			[Range(60, 30)] public float FPAimDepth = 40f;
			[Range(1, 10)] public float X_MouseSensitivity = 5f;
			[Range(1, 10)] public float Y_MouseSensitivity = 5f;
			[Range(1, 5)] public float AimX_MouseSensitivity = 2f;
			[Range(1, 5)] public float AimY_MouseSensitivity = 2f;
			[Range(-90, 0)] public float Y_MinLimit = -40f;
			[Range(0, 90)] public float Y_MaxLimit = 80f;
			[Range(0.01f, 0.5f)] public float tpSmoothX = 0.05f;
			[Range(0.01f, 0.5f)] public float tpSmoothY = 0.1f;
			[Range(0.01f, 0.5f)] public float tdSmoothX = 0.1f;

			[Range(1, 5)] public float fps_AimX_MouseSensitivity = 1f;
			[Range(1, 5)] public float fps_AimY_MouseSensitivity = 1f;
			[Range(1, 10)] public float fps_X_MouseSensitivity = 2f;
			[Range(1, 10)] public float fps_Y_MouseSensitivity = 2f;
			[Range(0.1f, 5)] public float fps_X_Smooth = 3f;
			[Range(0.1f, 5)] public float fps_Y_Smooth = 3f;
			[Range(-90, 0)] public float fps_MinRotationX = -30;
			[Range(0, 90)] public float fps_MaxRotationX = 40;

			public bool ActiveFP = true;
			public bool ActiveTP = true;
			public bool ActiveTD = true;

			[Range(1, 10)] public float TD_X_MouseSensitivity = 5f;
		}

		[Serializable]
		public class PhotonCharacter
		{
			public GameObject Character;
			public GameObject Image;
		}
		
		[Serializable]
		public class PhotonLevel
		{
			public Button LevelButton;
			public string LevelName;
#if UNITY_EDITOR
			public SceneAsset Scene;
#endif
		}

		public enum CameraType
		{
			ThirdPerson,
			FirstPerson,
			TopDown
		}

#if UNITY_EDITOR
		public static GameObject CreateCrosshair(Transform parent, string type)
		{
			if (type != "pickup")
			{
				var crosshair = new GameObject("Crosshair") {layer = 5};
				crosshair.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
				crosshair.transform.SetParent(parent);
				crosshair.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

				Helper.newCrosshairPart("Up", new Vector2(0, 30), new Vector2(4, 20), crosshair);
				Helper.newCrosshairPart("Down", new Vector2(0, -30), new Vector2(4, 20), crosshair);
				Helper.newCrosshairPart("Right", new Vector2(30, 0), new Vector2(20, 4), crosshair);
				Helper.newCrosshairPart("Left", new Vector2(-30, 0), new Vector2(20, 4), crosshair);
				
				return crosshair;
			}

			var pickUpIcon = new GameObject("PickUp Icon") {layer = 5};
			pickUpIcon.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
			pickUpIcon.transform.SetParent(parent);
			pickUpIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			var outline = pickUpIcon.AddComponent<Outline>();
			outline.effectColor = new Color(0, 0, 0, 1);

			var sprite = AssetDatabase.LoadAssetAtPath(
				"Assets/Universal Shooter Kit/Textures & Materials/Inventory/HandIcon.png", typeof(Sprite)) as Sprite;

			pickUpIcon.AddComponent<Image>().sprite = sprite;

			return pickUpIcon;
		}
#endif
		

		public static void CheckCameraPoints(Vector3 from, Vector3 to, GameObject disabledObject, Camera camera)
		{
			RaycastHit hitInfo;
			Helper.ClipPlanePoints clipPlanePoints = Helper.NearPoints(to, camera);

			if (disabledObject != null)
				disabledObject.GetComponent<MeshRenderer>().enabled = true;

			if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo, Helper.layerMask()))
				if (hitInfo.collider.GetComponent<MeshRenderer>())
				{
					disabledObject = hitInfo.collider.gameObject;
					disabledObject.GetComponent<MeshRenderer>().enabled = false;
				}

			if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo, Helper.layerMask()))
				if (hitInfo.collider.GetComponent<MeshRenderer>())
				{
					disabledObject = hitInfo.collider.gameObject;
					disabledObject.GetComponent<MeshRenderer>().enabled = false;
				}

			if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo, Helper.layerMask()))
				if (hitInfo.collider.GetComponent<MeshRenderer>())
				{
					disabledObject = hitInfo.collider.gameObject;
					disabledObject.GetComponent<MeshRenderer>().enabled = false;
				}

			if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo, Helper.layerMask()))
				if (hitInfo.collider.GetComponent<MeshRenderer>())
				{
					disabledObject = hitInfo.collider.gameObject;
					disabledObject.GetComponent<MeshRenderer>().enabled = false;
				}
		}

		public static float CheckCameraPoints(Vector3 from, Vector3 to, GameObject disabledObject, Transform transform, Camera camera)
		{
			var nearestDistance = -1f;
			RaycastHit hitInfo;
			var clipPlanePoints = Helper.NearPoints(to, camera);

			if (disabledObject != null)
				disabledObject.GetComponent<MeshRenderer>().enabled = true;

			if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo, Helper.layerMask()))
				nearestDistance = hitInfo.distance;

			if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo, Helper.layerMask()))
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;

			if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo, Helper.layerMask()))
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;

			if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo, Helper.layerMask()))
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;

			if (Physics.Linecast(from, to + transform.forward * - camera.nearClipPlane, out hitInfo, Helper.layerMask()))
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;

			return nearestDistance;
		}
		
		public static Vector3 CalculatePosition(float RotationX, float RotationY, float distance, CameraType TypeOfCamera, Controller thisController, float tdAngle)
		{
			var direction = new Vector3(0, 0, -distance);
			var rotation = Quaternion.Euler(0, 0, 0);

			switch (TypeOfCamera)
			{
				case CameraType.ThirdPerson:
					rotation = Quaternion.Euler(RotationX, RotationY, 0);
					return thisController.BodyObjects.Head.position + rotation * direction;
			
				case CameraType.TopDown:
					rotation = Quaternion.Euler(tdAngle, RotationY, 0);
					return new Vector3(thisController.transform.position.x, thisController.transform.position.y + 2, thisController.transform.position.z) + rotation * direction;
			}
			return thisController.BodyObjects.Head.position + rotation * direction;
		}
		

		public static void SwitchCamera(CameraType curType, CameraType newType, Controller controller)
		{
			if (curType != newType)
				controller.changeCameraType = true;

			if (!controller.changeCameraType) return;
			
			switch (newType)
			{
				case CameraType.ThirdPerson:
					
					controller.thisCameraScript.inspectorTab = 0;
					controller.TypeOfCamera = CameraType.ThirdPerson;

					controller.anim.SetLayerWeight(3, controller.WeaponManager.hasAnyWeapon ? 1 : 0);
					controller.anim.SetLayerWeight(2, 0);
					controller.currentAnimatorLayer = 3;

					controller.anim.SetBool("TPS", true);
					controller.anim.SetBool("FPS", false);
					controller.anim.SetBool("TDS", false);

					if (controller.WeaponManager.weaponController && controller.WeaponManager.weaponController.SwitchToFpCamera
					                                              && controller.WeaponManager.weaponController.IsAimEnabled)
					{
						controller.WeaponManager.weaponController.SwitchToFpCamera = false;
						controller.WeaponManager.weaponController.WasSetSwitchToFP = true;
					}
					
					switch (curType)
					{
						case CameraType.FirstPerson:
							controller.thisCameraScript.setCameraType = false;
							SetFPCamera(controller);
							
							if(controller.isCrouch)
								controller.WeaponManager.weaponController.CrouchHands();
							
							break;
						case CameraType.TopDown:
							controller.thisCameraScript.setCameraType = true;
							break;
					}

					break;
				case CameraType.FirstPerson:

					if (controller.CameraParameters.ActiveFP)
					{
						controller.TypeOfCamera = CameraType.FirstPerson;

						controller.thisCameraScript.maxMouseAbsolute = controller.middleAngleX + controller.CameraParameters.fps_MaxRotationX;
						controller.thisCameraScript.minMouseAbsolute = controller.middleAngleX + controller.CameraParameters.fps_MinRotationX;

						var offset = new Vector3(controller.CharacterOffset.xRotationOffset, controller.CharacterOffset.yRotationOffset,
							controller.CharacterOffset.zRotationOffset);
						controller.thisCameraScript.targetDirection = controller.BodyObjects.TopBody.eulerAngles - offset;

						controller.thisCameraScript._mouseAbsolute = new Vector2(0, 0);
						controller.thisCameraScript._smoothMouse = new Vector2(0, 0);
						controller.thisCameraScript.setCameraType = false;

						controller.anim.SetBool("FPS", true);
						controller.anim.SetBool("TPS", false);
						controller.anim.SetBool("TDS", false);

						controller.thisCameraScript.setCameraType = false;

						if (controller.WeaponManager.weaponController && controller.WeaponManager.weaponController.setCrouchHands)
							controller.WeaponManager.weaponController.CrouchHands();

						controller.anim.SetLayerWeight(3, 0);
						controller.anim.SetLayerWeight(2, 1);
						controller.currentAnimatorLayer = 2;
						controller.thisCameraScript.inspectorTab = 1;

						if (controller.isCrouch)
						{
							controller.defaultCharacterCenter += controller.CrouchHeight;
							controller.CharacterController.center = new Vector3(controller.CharacterController.center.x, controller.defaultCharacterCenter,
								controller.CharacterController.center.z);
						}
					}

					break;
				case CameraType.TopDown:

					if (controller.CameraParameters.ActiveTD)
					{
						if(controller.isCrouch)
							controller.DeactivateCrouch();
						
						controller.thisCameraScript.inspectorTab = 2;
						controller.TypeOfCamera = CameraType.TopDown;

						controller.anim.SetBool("TDS", true);
						controller.anim.SetBool("TPS", false);
						controller.anim.SetBool("FPS", false);

						controller.anim.SetLayerWeight(3, controller.WeaponManager.hasAnyWeapon ? 1 : 0);
						controller.anim.SetLayerWeight(2, 0);
						controller.currentAnimatorLayer = 3;

						switch (curType)
						{
							case CameraType.ThirdPerson:
								controller.thisCameraScript.setCameraType = true;
								break;
							case CameraType.FirstPerson:
							{
								controller.thisCameraScript.setCameraType = false;
								SetFPCamera(controller);
								break;
							}
						}
					}

					break;
			}

			if (controller.AdjustmentScene)
			{
				controller.transform.position = Vector3.zero;
				controller.transform.eulerAngles = Vector3.zero;
				controller.BodyObjects.TopBody.eulerAngles = Vector3.zero;
				controller.thisCameraScript.mouseX = 0;
				controller.thisCameraScript.mouseY = 0;
			}
			

			if (controller.WeaponManager.hasAnyWeapon)
			{
				if(!controller.AdjustmentScene)
					WeaponsHelper.SetHandsSettingsSlot(ref controller.WeaponManager.weaponController.SettingsSlotIndex, controller.characterTag, controller.TypeOfCamera,
						controller.WeaponManager.weaponController);
			
				controller.anim.CrossFade("Idle", 0, 2);
				controller.anim.CrossFade("Idle", 0, 3);
				controller.anim.SetBool("CanWalkWithWeapon", false);
				controller.WeaponManager.weaponController.StartCoroutine("WalkWithWeaponTimeout");
			}

//			foreach (var grenadeSlot in controller.WeaponManager.Grenades.Where(grenadeSlot => grenadeSlot.Grenade))
//			{
//				WeaponsHelper.SetHandsSettingsSlot(ref grenadeSlot.GrenadeScript.SettingsSlotIndex, controller.characterTag, controller.TypeOfCamera, grenadeSlot.GrenadeScript);
//			}
			
			controller.thisCameraScript.Reset();
		}

		static void SetFPCamera(Controller controller)
		{
			controller.thisCameraScript.canViewTarget = false;
			controller.StartCoroutine(controller.thisCameraScript.cameraTimeout());
								
			if (controller.isCrouch)
			{
				controller.defaultCharacterCenter -=controller.CrouchHeight;
				controller.CharacterController.center = new Vector3(controller.CharacterController.center.x, controller.defaultCharacterCenter, controller.CharacterController.center.z);
			}
		}

		public static void PlayStepSound(Surface surface, AudioSource source, int characterTag)
		{
			if (surface.Material)
			{
				if (surface.FootstepsSounds[characterTag].FootstepsAudios.Count > 0)
				{
					var sound = surface.FootstepsSounds[characterTag].FootstepsAudios[Random.Range(0, surface.FootstepsSounds[characterTag].FootstepsAudios.Count - 1)];

					if (sound)
					{
						source.clip = sound;
						source.PlayOneShot(source.clip);
					}
					else
						Debug.LogWarning("(Surface [<Color=green>" + surface.Material.name + "</color>]) Not all values of footsteps sounds are filled.");
				}
				else
					Debug.LogWarning("(Surface [<Color=green>" + surface.Material.name + "</color>])<Color=yellow> Missing components</color>: [Footsteps sounds].");
			}
			else
				Debug.LogError("(Surface [<Color=green>" + surface.Material.name + "</color>])<Color=yellow> Missing Component</color>: [Material]. Add it to initialize the surface.", surface.gameObject);
		}
		
	}
}

