// GercStudio
// © 2018-2019

using UnityEngine;

namespace GercStudio.USK.Scripts
{

    public class Rocket : MonoBehaviour
    {
        [Range(1, 50)] public float Speed;

        public Transform Explosion;
        [HideInInspector] public Transform Camera;
        [HideInInspector] public Transform OriginalCameraPosition;

        [HideInInspector] public Vector3 TargetPoint;

        [HideInInspector] public string OwnerName;

        [HideInInspector] public int damage;

        [HideInInspector] public bool isRaycast;
        [HideInInspector] public bool isEnemy;
        [HideInInspector] public bool isTracer;

        private float timeout;
        private float lifetimeout;
        private float _scatter;
        private bool isTopDown;

        void Start()
        {

            if (Camera)
            {
                OriginalCameraPosition = new GameObject("OriginalCameraPosition").transform;
                OriginalCameraPosition.hideFlags = HideFlags.HideInHierarchy;
                OriginalCameraPosition.position = Camera.position;
                OriginalCameraPosition.rotation =
                    Quaternion.Euler(Camera.eulerAngles.x, Camera.eulerAngles.y, Camera.eulerAngles.z);
            }

            if (gameObject.transform.childCount > 0)
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    if (gameObject.transform.GetChild(i).GetComponent<ParticleSystem>())
                    {
                        ParticleSystem.EmissionModule em = gameObject.transform.GetChild(i)
                            .GetComponent<ParticleSystem>()
                            .emission;
                        em.enabled = false;
                    }
                }
            }
        }

        void Update()
        {
            if (!isTracer)
            {
                if (gameObject.transform.childCount > 0)
                {
                    for (int i = 0; i < gameObject.transform.childCount; i++)
                    {
                        if (gameObject.transform.GetChild(i).GetComponent<ParticleSystem>())
                        {
                            ParticleSystem.EmissionModule em = gameObject.transform.GetChild(i)
                                .GetComponent<ParticleSystem>().emission;
                            em.enabled = true;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("(Rocket) Rocket has not effects (smoke and flames). Add it.", gameObject);
                }

                lifetimeout += Time.deltaTime;

                if (!isEnemy)
                {
                    if (isRaycast)
                    {
                        transform.LookAt(TargetPoint);
                        transform.position =
                            Vector3.MoveTowards(transform.position, TargetPoint, Speed * Time.deltaTime);
                    }
                    else
                    {
                        transform.Translate(Vector3.forward * Speed * Time.deltaTime, OriginalCameraPosition);
                    }
                }
                else
                {
                    transform.Translate(Vector3.forward * Speed * Time.deltaTime);
                }
                
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2))
                {
                    if (hit.transform.gameObject.layer == 8)
                    {
                        if(hit.transform.GetComponent<Controller>())
                            if (hit.transform.GetComponent<Controller>().CharacterName != OwnerName)
                                Dead();
                    }
                    else
                    {
                        Dead();
                    }
                }
                    
                if (lifetimeout > 10)
                    Dead();

            }
            else
            {
                transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                transform.LookAt(TargetPoint);
                transform.position =
                    Vector3.MoveTowards(transform.position, TargetPoint, Speed * Time.deltaTime);
                
                if (transform.position == TargetPoint)
                {
                    transform.position = TargetPoint;
                    Destroy(gameObject, 0);
                }
            }
        }

        public void Dead()
        {
            if (!isRaycast & !isEnemy)
                Destroy(OriginalCameraPosition.gameObject);

            if (Explosion)
            {
                var explosion = Instantiate(Explosion, transform.position + transform.TransformDirection(Vector3.forward) * 1.4f, transform.rotation);
                explosion.GetComponent<Explosion>().damage = damage;
                explosion.GetComponent<Explosion>().OwnerName = OwnerName;
                if (gameObject.transform.childCount > 0)
                {
                    for (int i = 0; i < gameObject.transform.childCount; i++)
                    {
                        if (gameObject.transform.GetChild(i).GetComponent<ParticleSystem>())
                        {
                            ParticleSystem.EmissionModule em = gameObject.transform.GetChild(i)
                                .GetComponent<ParticleSystem>().emission;
                            em.enabled = false;
                            gameObject.transform.GetChild(i).gameObject.AddComponent<DestroyObject>();
                            gameObject.transform.GetChild(i).gameObject.GetComponent<DestroyObject>().destroy_time = 10;
                            gameObject.transform.GetChild(i).parent = null;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError(
                    "(Rocket) <color=red>Missing component</color>: [Explosion]. Add it, otherwise the rocket won't explode.",
                    gameObject);
            }

            Destroy(gameObject);
        }
    }

}




