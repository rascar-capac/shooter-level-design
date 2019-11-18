// GercStudio
// © 2018-2019

using UnityEngine;
using System.Collections.Generic;

namespace GercStudio.USK.Scripts
{

    [RequireComponent(typeof(AIController))]
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(AudioSource))]

    public class EnemyAttack : MonoBehaviour
    {
        [Range(1, 100)] public int FireDamage;
        [Range(1, 100)] public int RocketDamage;
        [Range(1, 100)] public int BulletDamage;
        [Range(1, 100)] public int MeleeDamage;

        [Range(0, 0.3f)] public float ScatterOfBullets = 0.01f;
        public float RateOfShoot_Bullet = 0.5f;
        public float RateOfAttack = 1;
        public float RateOfShoot_Rocket = 1;

        public Transform Rocket;
        public Transform Fire;
        public Transform MuzzleFlash;
        public Transform[] BulletSpawn;
        public Transform[] FireSpawn;
        public Transform[] RocketSpawn;
        public List<GameObject> _Fire;

        public AudioClip BulletAttackAudio;
        public AudioClip FireAttackAudio;
        public AudioClip MeleeAttackAudio;
        public AudioClip RocketAttackAudio;

        public bool Gun_Attack;
        public bool Fire_Attack;
        public bool Melee_Attack;
        public bool Rocket_Attack;

        private int currentRocketSpawPoint = 0;

        private bool audioplay;
        private bool showDebug;

        private RaycastHit Hit;

        private float _rateofAttack;
        private float Rocket_rateofShoot;

        private EnemyMove enemy_move;

        private AudioSource audio_sourse;

        void Start()
        {
            enemy_move = GetComponent<EnemyMove>();

            if (Fire_Attack)
            {
                for (int i = 0; i < FireSpawn.Length; i++)
                {
                    if (Fire & FireSpawn[i])
                    {
                        GameObject fb = Instantiate(Fire, FireSpawn[i].position, FireSpawn[i].rotation).gameObject;
                        _Fire.Add(fb);
                        _Fire[i].transform.parent = FireSpawn[i].transform;
                    }
                }
            }

//            if (Melee_Attack)
//            {
//                enemy_move.Stop_AttackDistance = 3.5f;
//            }
        }

        void Update()
        {
            if (_rateofAttack <= RateOfShoot_Bullet || _rateofAttack <= RateOfAttack)
                _rateofAttack += Time.deltaTime;

            if (Rocket_rateofShoot <= RateOfShoot_Rocket)
                Rocket_rateofShoot += Time.deltaTime;

            if (enemy_move.CanAttack)
            {
                if (Gun_Attack & _rateofAttack > RateOfShoot_Bullet)
                {
                    GunAttack();
                }

                if (Fire_Attack)
                {
                    FireAttack();
                }

                if (Rocket_Attack & Rocket_rateofShoot > RateOfShoot_Rocket)
                {
                    RocketAttack();
                }

                if (Melee_Attack & _rateofAttack > RateOfAttack)
                {
                    MeleeAttack();
                }
            }
            else
            {
                showDebug = false;
                if (Fire_Attack)
                {
                    if (FireAttackAudio)
                        if (audioplay)
                        {
                            audioplay = false;
                            GetComponent<AudioSource>().Stop();
                        }

                    for (int i = 0; i < _Fire.Count; i++)
                    {
                        if (Fire)
                        {
                            _Fire[i].GetComponent<BoxCollider>().enabled = false;
                            ParticleSystem.EmissionModule em = _Fire[i].GetComponent<ParticleSystem>().emission;
                            em.enabled = false;
                        }
                    }

                }
            }
        }

        public void GunAttack()
        {
            if (BulletSpawn.Length > 0)
            {
                for (int i = 0; i < BulletSpawn.Length; i++)
                {
                    if (BulletSpawn[i])
                    {
                        if (BulletAttackAudio)
                            GetComponent<AudioSource>().PlayOneShot(BulletAttackAudio);
                        else
                            Debug.LogWarning(
                                "(Enemy) Missing component [AttackAudio]. Add it, otherwise the sound of enemy shooting won't be played.",
                                gameObject);

                        Vector3 Direction = BulletSpawn[i].TransformDirection(
                            Vector3.forward + new Vector3(Random.Range(-ScatterOfBullets, ScatterOfBullets),
                                Random.Range(-ScatterOfBullets, ScatterOfBullets), 0));
                        if (MuzzleFlash)
                        {
                            var Flash = Instantiate(MuzzleFlash, BulletSpawn[i].position, BulletSpawn[i].rotation);
                            Flash.parent = BulletSpawn[i].transform;
                            Flash.gameObject.AddComponent<DestroyObject>().destroy_time = 0.17f;
                        }
                        else
                        {
                            Debug.LogWarning(
                                "(Enemy) Missing component [MuzzleFlash_prefab]. Add it, otherwise muzzle flash won't be displayed.",
                                gameObject);
                        }

                        _rateofAttack = 0;
                        if (Physics.Linecast(BulletSpawn[i].position, enemy_move.curTarget.position + new Vector3(Random.Range(-ScatterOfBullets, ScatterOfBullets),
                                                                          Random.Range(-ScatterOfBullets, ScatterOfBullets), 0), out Hit))
                        {
                            Quaternion HitRotation = Quaternion.FromToRotation(Vector3.up, Hit.normal);
                           // Debug.DrawRay(BulletSpawn[i].position, Direction * 10000f, Color.red);
                            if (Hit.collider.CompareTag("Player"))
                            {
                                if (Hit.collider.GetComponent<Controller>())
                                {
                                    Hit.collider.GetComponent<Controller>().Damage(BulletDamage, "Enemy");
                                }
                                else
                                {
                                    Debug.LogWarning(
                                        "(Enemy) Not found GameObject: Player with [Health] script. Add it, otherwise the character won't take damage");
                                }
                            }

                            if (Hit.transform.GetComponent<Rigidbody>())
                                Hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(Direction * 500, Hit.point);

                            if (Hit.collider.GetComponent<Surface>())
                            {
                                Surface surface = Hit.collider.GetComponent<Surface>();
                                if (surface.Material)
                                {
                                    if (surface.Sparks & surface.Hit)
                                    {
                                        Instantiate(surface.Sparks, Hit.point + (Hit.normal * 0.01f),
                                            HitRotation);
                                        Transform hitGO = Instantiate(surface.Hit,
                                            Hit.point + (Hit.normal * 0.001f), HitRotation).transform;
                                        if (surface.HitAudio)
                                        {
                                            hitGO.gameObject.AddComponent<AudioSource>();
                                            hitGO.gameObject.GetComponent<AudioSource>().clip = surface.HitAudio;
                                            hitGO.gameObject.GetComponent<AudioSource>()
                                                .PlayOneShot(hitGO.gameObject.GetComponent<AudioSource>().clip);
                                        }
                                        else
                                            Debug.LogWarning(
                                                "(Surface) <color=yellow>Missing component</Color> [HitAudio]. Add it, otherwise the sound of hit won't be played.");

                                        hitGO.parent = Hit.transform;
                                    }
                                    else
                                        Debug.LogWarning(
                                            "(Surface) <color=yellow>Missing components</Color>: [Sparks] and/or [Hit]. Add them, otherwise the rocket launcher won't shoot.");
                                }
                                else
                                    Debug.LogError(
                                        "(Surface) <color=red>Missing Component</Color>: [Material]. Add it to initialize the surface.",
                                        surface.gameObject);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError(
                            "(Enemy) <color=red>Missing components [BulletSpawn]</Color>. Add it, otherwise the enemy won't shoot.",
                            gameObject);
                        Debug.Break();
                    }
                }
            }
            else
            {
                Debug.LogError(
                    "(Enemy) <color=red>Missing components</color> [BulletSpawn]. Add it, otherwise the enemy won't shoot.",
                    gameObject);
                Debug.Break();
            }
        }

        public void FireAttack()
        {
            if (FireSpawn.Length > 0)
            {
                for (int i = 0; i < _Fire.Count; i++)
                {
                    if (Fire)
                    {
                        if (FireAttackAudio)
                        {
                            if (!audioplay)
                            {
                                GetComponent<AudioSource>().clip = FireAttackAudio;
                                GetComponent<AudioSource>().loop = true;
                                GetComponent<AudioSource>().Play();
                                audioplay = true;
                            }
                        }
                        else
                        {
                            if (!showDebug)
                            {
                                Debug.LogWarning(
                                    "(Weapon) <color=yellow>Missing component</color> [AttackAudio]. Add it, otherwise the sound of shooting won't be played.",
                                    gameObject);
                                showDebug = true;

                            }
                        }

                        _Fire[i].GetComponent<BoxCollider>().enabled = true;
                        ParticleSystem.EmissionModule em = _Fire[i].GetComponent<ParticleSystem>().emission;
                        em.enabled = true;
                    }
                    else
                    {
                        Debug.LogError(
                            "(Enemy) <color=red>Missing component</color> [Fire_prefab]. Add it, otherwise the flamethrower won't attack.",
                            gameObject);
                        Debug.Break();
                    }
                }
            }
            else
            {
                Debug.LogError(
                    "(Enemy) <color=red>Missing components</color>: [FireSpawn] and/or [Fire_prefab]. Add it, otherwise the flamethrower won't attack.",
                    gameObject);
                Debug.Break();
            }
        }

        public void RocketAttack()
        {
            if (RocketSpawn.Length > 0)
            {
                if (Rocket & RocketSpawn[currentRocketSpawPoint])
                {
                    if (RocketAttackAudio)
                        GetComponent<AudioSource>().PlayOneShot(RocketAttackAudio);
                    else
                        Debug.LogWarning(
                            "(Enemy) <Color=yellow>Missing component</color> [AttackAudio]. Add it, otherwise the sound of enemy shooting won't be played.",
                            gameObject);

                    Transform rocket = Instantiate(Rocket, RocketSpawn[currentRocketSpawPoint].position,
                        RocketSpawn[currentRocketSpawPoint].rotation);
                    rocket.GetComponent<Rocket>().damage = RocketDamage;
                    rocket.GetComponent<Rocket>().OwnerName = "Enemy";
                    rocket.GetComponent<Rocket>().isEnemy = true;
                    Rocket_rateofShoot = 0;

                    if (currentRocketSpawPoint >= RocketSpawn.Length - 1)
                        currentRocketSpawPoint = 0;
                    else currentRocketSpawPoint++;
                }
                else
                {
                    Debug.LogError(
                        "(Enemy) <color=red>Missing components</color>: [RocketSpawn] and/or [Rocket_prefab]. Add it, otherwise the rocket launcher won't attack.",
                        gameObject);
                    Debug.Break();
                }
            }
            else
            {
                Debug.LogError(
                    "(Enemy) <color=red>Missing components</color>: [RocketSpawn] and/or [Rocket_prefab]. Add it, otherwise the rocket launcher won't attack.",
                    gameObject);
                Debug.Break();
            }
        }

        public void MeleeAttack()
        {
            if (MeleeAttackAudio)
                GetComponent<AudioSource>().PlayOneShot(MeleeAttackAudio);
            else
                Debug.LogWarning(
                    "(Enemy) <color=yellow>Missing component</color> [AttackAudio]. Add it, otherwise the sound of enemy shooting won't be played.",
                    gameObject);

            GetComponent<EnemyMove>().curTarget.GetComponent<Controller>().MeleeAttack(MeleeDamage);

            _rateofAttack = 0;
        }
    }

}





 

		




