// GercStudio
// © 2018-2019

using UnityEngine;

namespace GercStudio.USK.Scripts
{

	public class ShellControll : MonoBehaviour
	{
		private AudioSource _audio;
		private Rigidbody _rigidbody;
		private float ShellSpeed;
		//[HideInInspector] public Transform weapon;
		[HideInInspector] public Transform ShellPoint;

		void Start()
		{
			_rigidbody = gameObject.AddComponent<Rigidbody>();
			_audio = gameObject.AddComponent<AudioSource>();
			_audio.spatialBlend = 1;
			_audio.volume = 0.5f;
			_audio.minDistance = 10;
			_audio.maxDistance = 70;
			gameObject.AddComponent<BoxCollider>().isTrigger = true;
			transform.parent = null;
			
			//if(weapon.GetComponent<WeaponController>().WeaponParent == Helper.Parent.Character)
				//_rigidbody.velocity = weapon.GetComponent<WeaponController>().Controller.transform.TransformDirection(Vector3.right * Random.Range(1f, 3f));
			//else _rigidbody.velocity = weapon.GetComponent<WeaponController>().AIController.transform.TransformDirection(Vector3.right * Random.Range(1f, 3f));

			_rigidbody.velocity = ShellPoint.forward * Random.Range(1f, 3f);
			
			transform.RotateAround(transform.position, transform.right, Random.Range(-30,30));
			transform.RotateAround(transform.position, transform.up, Random.Range(-20,20));
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<Surface>())
			{
				var surface = other.GetComponent<Surface>();
				gameObject.GetComponent<BoxCollider>().isTrigger = false;
				if(surface.ShellDropSounds.Count > 0)
					_audio.PlayOneShot(surface.ShellDropSounds[Random.Range(0, surface.ShellDropSounds.Count - 1)]);
				
				Destroy(gameObject, 20);
			}
		}
	}

}


