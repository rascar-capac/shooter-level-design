using UnityEngine;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{

	public class GenerateLevel : MonoBehaviour
	{
		public GameObject[] Blocks;

		private bool use;

		private void OnTriggerEnter(Collider other)
		{
			if (!other.GetComponent<Controller>() || other.GetComponent<AIController>() || use) return;

			var position = transform.position;

			CreateBlock(new Vector3(position.x, position.y, position.z + 50));
			CreateBlock(new Vector3(position.x, position.y, position.z - 50));
			CreateBlock(new Vector3(position.x + 50, position.y, position.z));
			CreateBlock(new Vector3(position.x - 50, position.y, position.z));
			CreateBlock(new Vector3(position.x + 50, position.y, position.z + 50));
			CreateBlock(new Vector3(position.x - 50, position.y, position.z - 50));
			CreateBlock(new Vector3(position.x - 50, position.y, position.z + 50));
			CreateBlock(new Vector3(position.x + 50, position.y, position.z - 50));

			use = true;
		}

		void CreateBlock(Vector3 center)
		{
			var colliders = Physics.OverlapSphere(center, 1);

			if (colliders.Length == 0)
			{
				var block = Instantiate(Blocks[Random.Range(0, Blocks.Length - 1)], center, Quaternion.identity);
				block.hideFlags = HideFlags.HideInHierarchy;
			}
		}
	}
}

