// GercStudio
// © 2018-2019

using UnityEngine;

namespace GercStudio.USK.Scripts
{

	public class Robot_RotateTop : MonoBehaviour
	{
		public Transform top;

		public float RotationSpeedTop;

		private Transform target;

		private EnemyMove _aiController;

		void Start()
		{
			_aiController = GetComponent<EnemyMove>();
		}

		void LateUpdate()
		{
			if (_aiController.curTarget)
			{
				target = _aiController.curTarget;
				if (Vector3.Distance(transform.position, target.position) <= _aiController.DistanceToSee)
					top.transform.rotation = Quaternion.Slerp(top.transform.rotation,
						Quaternion.LookRotation(target.position - top.transform.position),
						Time.deltaTime * RotationSpeedTop);
				else
					top.transform.rotation =
						Quaternion.Slerp(top.transform.rotation, transform.localRotation, Time.deltaTime);
			}
			else
				top.transform.rotation =
					Quaternion.Slerp(top.transform.rotation, transform.localRotation, Time.deltaTime);
		}
	}

}


