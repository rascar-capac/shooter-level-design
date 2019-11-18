// GercStudio
// © 2018-2019

using System.Linq;
using UnityEngine;

namespace GercStudio.USK.Scripts
{

    public class Explosion : MonoBehaviour
    {
        public float Radius;
        public float Force;
        public float Time;

        [HideInInspector] public int damage;

        [HideInInspector] public string OwnerName;

        private bool anyDamage;

        void Start()
        {
            ExplosionProcess();
            Transform[] allChildren = GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.GetInstanceID() != GetInstanceID())
                {
                    child.parent = null;
                    child.gameObject.AddComponent<DestroyObject>().destroy_time = 5;
                }
            }
        }

        void ExplosionProcess()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
            foreach (var collider in hitColliders)
            {
                if (collider.GetComponent<EnemyHealth>())
                    collider.GetComponent<EnemyHealth>().Enemy_health -= damage;

                if (collider.GetComponent<Rigidbody>())
                    collider.GetComponent<Rigidbody>()
                        .AddExplosionForce(Force * 50, transform.position, Radius, 0.0f);

                if (collider.GetComponent<Controller>())
                    collider.GetComponent<Controller>().ExplosionDamage(damage, OwnerName);

                if (collider.GetComponent<WeaponController>())
                {
                    if (collider.GetComponent<WeaponController>().Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
                    {
                        collider.GetComponent<WeaponController>().Explosion();
                        break;
                    }
                }
                    
            }

            Destroy(gameObject, Time);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }

}



