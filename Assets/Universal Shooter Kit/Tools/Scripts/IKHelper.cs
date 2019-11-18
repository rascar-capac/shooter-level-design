using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	public static class IKHelper {
		
		
		[Serializable]
		public class FeetIKVariables
		{
			public Quaternion RightFootIKRotation, LeftFootIKRotation;
			public Vector3 RightFootPosition, LeftFootPosition, LeftFootIKPosition, RightFootIKPosition;
			public float LastRightFootPosition;
			public float LastLeftFootPosition;
			public float LastPelvisPosition;
			public float feetToIKPositionSpeed = 0.5f;
			public float heightFromGroundRaycast = 1.15f;
			public float raycastDownDistance = 1.5f;
			public float pelvisOffset;
			
		}

		public static void FeetPositionSolver(Controller controller, string type)
		{
			switch (type)
			{
				case "right":
					FeetPositionSolver(controller.IKVariables.RightFootPosition,
						ref controller.IKVariables.RightFootIKPosition, ref controller.IKVariables.RightFootIKRotation,
						controller.IKVariables.raycastDownDistance, controller.IKVariables.heightFromGroundRaycast,
						controller.IKVariables.pelvisOffset, controller.transform);
					break;
				case "left":
					FeetPositionSolver(controller.IKVariables.LeftFootPosition,
						ref controller.IKVariables.LeftFootIKPosition, ref controller.IKVariables.LeftFootIKRotation,
						controller.IKVariables.raycastDownDistance, controller.IKVariables.heightFromGroundRaycast,
						controller.IKVariables.pelvisOffset, controller.transform);
					break;
			}
		}
		
		public static void FeetPositionSolver(AIController aiController, string type)
		{
			switch (type)
			{
				case "right":
					FeetPositionSolver(aiController.IKVariables.RightFootPosition,
						ref aiController.IKVariables.RightFootIKPosition, ref aiController.IKVariables.RightFootIKRotation,
						aiController.IKVariables.raycastDownDistance, aiController.IKVariables.heightFromGroundRaycast,
						aiController.IKVariables.pelvisOffset, aiController.transform);
					break;
				case "left":
					FeetPositionSolver(aiController.IKVariables.LeftFootPosition,
						ref aiController.IKVariables.LeftFootIKPosition, ref aiController.IKVariables.LeftFootIKRotation,
						aiController.IKVariables.raycastDownDistance, aiController.IKVariables.heightFromGroundRaycast,
						aiController.IKVariables.pelvisOffset, aiController.transform);
					break;
			}
		}

		static void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations,
			float raycastDownDistance, float heightFromGroundRaycast, float pelvisOffset, Transform transform)
		{
			RaycastHit feetOutHit;
//			Debug.DrawLine(fromSkyPosition,
//				fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.green);
			if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit,
				raycastDownDistance + heightFromGroundRaycast, Helper.layerMask()))
			{
				feetIkPositions = fromSkyPosition;
				feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
				feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
				return;
			}

			feetIkPositions = Vector3.zero;
		}

		public static void AdjustFeetTarget(Controller controller, string type)
		{
			switch (type)
			{
				case "right":
					AdjustFeetTarget(ref controller.IKVariables.RightFootPosition, HumanBodyBones.RightFoot,
						controller.anim, controller.transform, controller.IKVariables.heightFromGroundRaycast);
					break;
				case "left":
					AdjustFeetTarget(ref controller.IKVariables.LeftFootPosition, HumanBodyBones.LeftFoot,
						controller.anim, controller.transform, controller.IKVariables.heightFromGroundRaycast);
					break;
			}
		}
		
		public static void AdjustFeetTarget(AIController aiController , string type)
		{
			switch (type)
			{
				case "right":
					AdjustFeetTarget(ref aiController.IKVariables.RightFootPosition, HumanBodyBones.RightFoot,
						aiController.anim, aiController.transform, aiController.IKVariables.heightFromGroundRaycast);
					break;
				case "left":
					AdjustFeetTarget(ref aiController.IKVariables.LeftFootPosition, HumanBodyBones.LeftFoot,
						aiController.anim, aiController.transform, aiController.IKVariables.heightFromGroundRaycast);
					break;
			}
		}
		
		static void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones footBone, Animator anim, Transform transform, float heightFromGroundRaycast)
		{
			if (anim.GetBoneTransform(footBone))
			{
				feetPositions = anim.GetBoneTransform(footBone).position;
				feetPositions.y = transform.position.y + heightFromGroundRaycast;
			}
		}

		public static void MovePelvisHeight(Vector3 RightFootIKPosition, Vector3 LeftFootIKPosition, float LastPelvisPosition, Animator anim,
			Transform transform)
		{
			if (RightFootIKPosition == Vector3.zero || LeftFootIKPosition == Vector3.zero || LastPelvisPosition == 0)
			{
				LastPelvisPosition = anim.bodyPosition.y;
				return;
			}

			float l_offsetPosition = LeftFootIKPosition.y - transform.position.y;
			float r_offsetPosition = RightFootIKPosition.y - transform.position.y;

			float totalOffset = (l_offsetPosition < r_offsetPosition) ? l_offsetPosition : r_offsetPosition;

			Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;

			newPelvisPosition.y =
				Mathf.Lerp(LastPelvisPosition, newPelvisPosition.y, 0.5f);

			anim.bodyPosition = newPelvisPosition;

		}

		public static void MoveFeetToIkPoint(Controller controller, string type)
		{
			switch (type)
			{
				case "right":
					MoveFeetToIkPoint(AvatarIKGoal.RightFoot, controller.IKVariables.RightFootIKPosition,
						controller.IKVariables.RightFootIKRotation, ref controller.IKVariables.LastRightFootPosition, controller.anim,
						controller.transform, controller.IKVariables.feetToIKPositionSpeed);
					break;
				case "left":
					MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, controller.IKVariables.LeftFootIKPosition,
						controller.IKVariables.LeftFootIKRotation, ref controller.IKVariables.LastLeftFootPosition, controller.anim,
						controller.transform, controller.IKVariables.feetToIKPositionSpeed);
					break;
			}
		}

		public static void MoveFeetToIkPoint(AIController aiController, string type)
		{
			switch (type)
			{
				case "right":
					MoveFeetToIkPoint(AvatarIKGoal.RightFoot, aiController.IKVariables.RightFootIKPosition,
						aiController.IKVariables.RightFootIKRotation, ref aiController.IKVariables.LastRightFootPosition, aiController.anim,
						aiController.transform, aiController.IKVariables.feetToIKPositionSpeed);
					break;
				case "left":
					MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, aiController.IKVariables.LeftFootIKPosition,
						aiController.IKVariables.LeftFootIKRotation, ref aiController.IKVariables.LastLeftFootPosition, aiController.anim,
						aiController.transform, aiController.IKVariables.feetToIKPositionSpeed);
					break;
			}
		}
		
		static void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 curPositionIk, Quaternion curRotationIk,
			ref float lastFootPosition, Animator anim, Transform transform, float feetToIKPositionSpeed)
		{
			Vector3 targetIkPosition = anim.GetIKPosition(foot);
			
			if (curPositionIk != Vector3.zero)
			{
				targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
				curPositionIk = transform.InverseTransformPoint(curPositionIk);

				var yPos = Mathf.Lerp(lastFootPosition, curPositionIk.y, feetToIKPositionSpeed);
				targetIkPosition.y += yPos;

				lastFootPosition = yPos;

				targetIkPosition = transform.TransformPoint(targetIkPosition);

				anim.SetIKRotation(foot, curRotationIk);
			}

			anim.SetIKPosition(foot, targetIkPosition);
		}
	}
}

