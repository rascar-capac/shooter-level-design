// GercStudio
// © 2018-2019

using UnityEngine;

namespace GercStudio.USK.Scripts
{

    [RequireComponent(typeof(AIController))]
    [RequireComponent(typeof(EnemyAttack))]

    public class EnemyHealth : MonoBehaviour
    {
        [Range(1, 500)] public float Enemy_health = 100;

        public Transform Ragdoll;
        public Transform Enemy_health_text;

        [HideInInspector] public bool isRagdoll;

        void Awake()
        {
            if (!Enemy_health_text)
                Debug.LogWarning(
                    "(Enemy) Missing component [Enemy_health_text]. Add it, otherwise the health of enemy won't be displayed.",
                    gameObject);
        }

        void Update()
        {
            if (Enemy_health_text)
            {
                if (GetComponent<EnemyMove>().targets.Count == 1)
                {
                    if (GetComponent<EnemyMove>().targets[0])
                    {
                        Enemy_health_text.LookAt(GetComponent<EnemyMove>().targets[0].GetComponent<Controller>()
                            .thisCamera
                            .transform);
                        Enemy_health_text.RotateAround(Enemy_health_text.position,
                            Enemy_health_text.up, 180);
                    }
                }

                if (Enemy_health >= 75)
                    Enemy_health_text.GetComponent<Renderer>().material.color = Color.green;

                if (Enemy_health >= 50 & Enemy_health < 75)
                    Enemy_health_text.GetComponent<Renderer>().material.color = Color.yellow;

                if (Enemy_health >= 25 & Enemy_health < 50)
                    Enemy_health_text.GetComponent<Renderer>().material.color = new Color32(255, 140, 0, 255);

                if (Enemy_health < 25)
                    Enemy_health_text.GetComponent<Renderer>().material.color = Color.red;

                Enemy_health_text.GetComponent<TextMesh>().text = Enemy_health.ToString("F0");
            }

            if (Enemy_health <= 0)
            {
                CreateRagdoll();
                Destroy(gameObject, 0.3f);
            }
        }

        public void CreateRagdoll()
        {
            if (Ragdoll)
            {
                if (!isRagdoll)
                {
                    Transform dead = Instantiate(Ragdoll, transform.position, transform.rotation) as Transform;
                    Helper.CopyTransformsRecurse(transform, dead);
                    isRagdoll = true;
                }
            }
            else
            {
                Debug.LogWarning(
                    "(Enemy) Missing component [Ragdoll]. Add it, otherwise the rag doll won't be created after the death of the enemy.",
                    gameObject);
            }
        }


        void OnTriggerStay(Collider Col)
        {
            if (Col.CompareTag("Fire"))
            {
                if (Col.transform.root.GetComponent<Controller>())
                {
                    var weaponController = Col.transform.root.GetComponent<Controller>().WeaponManager.weaponController;
                    if (weaponController.Attacks[weaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Flame)
                    {
                        Enemy_health -= weaponController.Attacks[weaponController.currentAttack].weapon_damage * Time.deltaTime;
                    }
                }
            }
        }

        void OnTriggerEnter(Collider Col)
        {
            if (Col.CompareTag("KnifeCollider"))
            {
                var weaponController = Col.transform.root.GetComponent<Controller>().WeaponManager.weaponController;
                if (weaponController.Attacks[weaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Knife)
                {
                    Enemy_health -= weaponController.Attacks[weaponController.currentAttack].weapon_damage;
                }
            }
        }
    }

}





