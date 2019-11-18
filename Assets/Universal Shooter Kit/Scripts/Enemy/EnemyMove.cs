// GercStudio
// © 2018-2019

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GercStudio.USK.Scripts
{

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(EnemyAttack))]
    [RequireComponent(typeof(EnemyHealth))]

    public class EnemyMove : MonoBehaviour
    {
        [Range(1, 100)] public float Stop_AttackDistance;
        [Range(1, 100)] public float DistanceToSee;

        public enum MoveOnWaypoints
        {
            Random,
            Course,
            FindNearestPoint
        };

        public MoveOnWaypoints EnemyMovement;

        [HideInInspector] public List<GameObject> targets;
        [HideInInspector] public List<Transform> WayPoints;
        [HideInInspector] public Transform curTarget;

        [HideInInspector] public bool CanAttack;

        private UnityEngine.AI.NavMeshAgent agent;

        private Transform _target;

        private Animator anim;

        private int currentWP = 0;
        private int PreviousPoint = 0;
        private float timer = 0;

        private bool HasIndex;
//        private bool _audio = true;
//        private bool isStop;

        void Awake()
        {
            FindPlayers();
        }

        void Start()
        {
            anim = gameObject.GetComponent<Animator>();

            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            var foundObjects = GameObject.FindGameObjectsWithTag("WayPoint");

            for (int i = 0; i < foundObjects.Length; i++)
            {
                WayPoints.Add(foundObjects[i].transform);
            }

            //StartCoroutine(StepSounds());
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer > 2)
            {
                FindPlayers();
                
                if (targets.Count > 0)
                    curTarget = FindClosestPlayer().transform;
                timer = 0;
            }

            if (curTarget & targets.Count > 0)
            {
                if (Vector3.Distance(transform.position, curTarget.position) < DistanceToSee)
                {
                    agent.SetDestination(curTarget.position);
                    agent.stoppingDistance = Stop_AttackDistance;
                    HasIndex = false;
                    if (Vector3.Distance(transform.position, curTarget.position) >= Stop_AttackDistance)
                    {
                        SetAnimationValues(false);
                        CanAttack = false;
//                        isStop = false;
                    }
                    else
                    {
                        SetAnimationValues(true);
                        CanAttack = true;
//                        isStop = true;
                    }
                }
                else
                {
                    WayPointsMoving();
                    CanAttack = false;
                }
            }
            else
            {
                WayPointsMoving();
                CanAttack = false;
            }
        }

        void WayPointsMoving()
        {
            if (WayPoints.Count > 0)
            {
                switch (EnemyMovement)
                {
                    case MoveOnWaypoints.Course:
                        Course_Move();
                        break;
                    case MoveOnWaypoints.Random:
                        Random_Move();
                        break;
                    case MoveOnWaypoints.FindNearestPoint:
                        FindNearestPoint_Move();
                        break;
                }

                SetAnimationValues(false);
            }
            else
                Debug.LogError(
                    "(Enemy) Not found waypoints with tag 'WayPoint'. Add them, otherwise the enemy won't move through the control points");
        }

        void SetAnimationValues(bool value)
        {
            if (Helper.HasParameter("CanWalk", anim))
                anim.SetBool("CanWalk", !value);
            else
                Debug.LogError(
                    "(Enemy) Not found variable 'CanWalk' in Animator. Create it, otherwise the aim animation won't work correctly. To find detailed information 'How tune the animator' read my Documentation.",
                    gameObject);

            if (Helper.HasParameter("CanAttack", anim))
                anim.SetBool("CanAttack", value);
            else
                Debug.LogError(
                    "(Enemy) Not found variable 'CanAttack' in Animator. Create it, otherwise the aim animation won't work correctly. To find detailed information 'How tune the animator' read my Documentation.",
                    gameObject);
        }

        public void FindPlayers()
        {
            targets.Clear();

            var foundPlayers = FindObjectsOfType<Controller>();

            for (int i = 0; i < foundPlayers.Length; i++)
            {
                targets.Add(foundPlayers[i].gameObject);
            }
        }

        public void Course_Move()
        {
            if (!HasIndex)
            {
                agent.stoppingDistance = 3;
                currentWP = GetNearestObject(WayPoints);
                HasIndex = true;
            }

            agent.SetDestination(WayPoints[currentWP].position);
            if (Vector3.Distance(WayPoints[currentWP].position, transform.position) < 5)
            {
                PreviousPoint = currentWP;
                currentWP++;
                if (currentWP >= WayPoints.Count)
                    currentWP = 0;
            }
        }

        public void FindNearestPoint_Move()
        {
            if (!HasIndex)
                agent.stoppingDistance = 3;

            agent.SetDestination(WayPoints[GetNearestObject(WayPoints)].position);
            if (Vector3.Distance(WayPoints[GetNearestObject(WayPoints)].position, transform.position) < 5)
            {
                PreviousPoint = currentWP;
                currentWP = GetNearestObject(WayPoints);
            }
        }

        public void Random_Move()
        {
            if (!HasIndex)
            {
                agent.stoppingDistance = 3;
                currentWP = GetNearestObject(WayPoints);
                HasIndex = true;
            }

            agent.SetDestination(WayPoints[currentWP].position);
            if (Vector3.Distance(WayPoints[currentWP].position, transform.position) < 5)
            {
                PreviousPoint = currentWP;
                currentWP = Random.Range(0, WayPoints.Count);
            }
        }

        public GameObject FindClosestPlayer()
        {
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;

            foreach (GameObject player in targets)
            {
                Vector3 diff = player.transform.position - position;

                float curDistacne = diff.sqrMagnitude;
                if (curDistacne < distance)
                {
                    closest = player;
                    distance = curDistacne;
                }
            }

            return closest;
        }

        /*IEnumerator StepSounds()
        {
            float interval = 0;
            while (true)
            {
                if (!isStop)
                {
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(transform.position + new Vector3(1, 1, 1), Vector3.down, out hit))
                    {
                        if (hit.collider.GetComponent<Surface>())
                        {
                            Surface surface = hit.collider.GetComponent<Surface>();
                            if (surface.Material)
                            {
                                if (surface.FootstepsAudios.Length > 0)
                                {
                                    for (int i = 0; i < surface.FootstepsAudios.Length; i++)
                                        if (!surface.FootstepsAudios[i])
                                            _audio = false;

                                    if (_audio)
                                    {
                                        GetComponent<AudioSource>().clip =
                                            surface.FootstepsAudios[Random.Range(0, surface.FootstepsAudios.Length)];
                                        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
                                        interval = GetComponent<AudioSource>().clip.length;
                                    }
                                    else
                                        Debug.LogWarning(
                                            "(Surface) Not all values of footsteps sounds are filled. Add them otherwise the sounds won't play.");
                                }
                                else
                                    Debug.LogWarning(
                                        "(Surface) Missing components: [Footsteps sounds]. Add them otherwise the sounds won't play.");
                            }
                            else
                                Debug.LogError(
                                    "(Surface) Missing Component: [Material]. Add it to initialize the surface.",
                                    surface.gameObject);
                        }

                        interval *= 0.75f;
                    }

                    yield return new WaitForSeconds(interval);
                }
                else yield return 0;
            }
        }*/

        int GetNearestObject(List<Transform> objects)
        {
            int bestPointIndex = 0;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            for (int i = 0; i < objects.Count; i++)
            {
                Vector3 directionToObject = objects[i].position - currentPosition;
                float dSqrToTarget = directionToObject.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr & i != currentWP & i != PreviousPoint)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestPointIndex = i;
                }
            }

            return bestPointIndex;
        }

    }

}




