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
        public bool isAngryZombie = false;
        public List<Transform> WayPoints;
        [Range(1, 100)] public float Stop_AttackDistance;
        [Range(1, 180)] public float FOVAngle;
        [Range(1, 100)] public float visualDetectionDistance;
        [Range(1, 100)] public float soundDetectionDistance;
        [Range(0, 100)] public float detectionDelay;
        [Range(0, 100)] public float maxDistanceFromInitialPosition;

        public enum MoveOnWaypoints
        {
            Random,
            Course,
            FindNearestPoint,
        };

        public MoveOnWaypoints EnemyMovement;

        [HideInInspector] public List<GameObject> targets;
        [HideInInspector] public Controller curTarget;

        [HideInInspector] public bool CanAttack;

        private UnityEngine.AI.NavMeshAgent agent;

        //private Transform target;

        private Animator anim;

        private int currentWP = 0;
        private int PreviousPoint = 0;
        //private float timer = 0;

        private bool HasIndex;

        private List<Collider> overlappingColliders;
        private bool isHearingPlayer;
        private bool isSeeingPlayer;
        private bool isDetectingPlayer;
        private bool isAttackingPlayer;
        private float detectionTimer;
        //        private bool _audio = true;
        //        private bool isStop;

        //void Awake()
        //{
        //    FindPlayers();
        //}

        private void Awake()
        {
            isDetectingPlayer = false;
            isAttackingPlayer = false;
            detectionTimer = detectionDelay;
        }

        void Start()
        {
            anim = gameObject.GetComponent<Animator>();

            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            curTarget = GameObject.FindWithTag("Player").GetComponent<Controller>();

            //StartCoroutine(StepSounds());
        }

        private void OnDrawGizmos()
        {
            if(!isAngryZombie)
            {
                if(isHearingPlayer)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }
                Gizmos.DrawWireSphere(transform.position, soundDetectionDistance);

                if(isSeeingPlayer)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Vector3 FOVLeftLine = Quaternion.AngleAxis(FOVAngle / 2, transform.up) * transform.forward * visualDetectionDistance;
                Gizmos.DrawRay(transform.position, FOVLeftLine);
                Vector3 FOVRightLine = Quaternion.AngleAxis(- FOVAngle / 2, transform.up) * transform.forward * visualDetectionDistance;
                Gizmos.DrawRay(transform.position, FOVRightLine);

                if(isAttackingPlayer)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.blue;
                }
                Gizmos.DrawRay(transform.position, (curTarget.transform.position - transform.position).normalized * visualDetectionDistance);

                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, transform.forward * visualDetectionDistance);
            }
        }

        void Update()
        {
            //timer += Time.deltaTime;

            //if (timer > 2)
            //{
            //    FindPlayers();

            //    if (targets.Count > 0)
            //        curTarget = FindClosestPlayer().transform;
            //    timer = 0;
            //}

            //            if (curTarget & targets.Count > 0)
            //            {
            //                if (Vector3.Distance(transform.position, curTarget.transform.position) < DistanceToSee)
            //                {
            //                    agent.SetDestination(curTarget.transform.position);
            //                    agent.stoppingDistance = Stop_AttackDistance;
            //                    HasIndex = false;
            //                    if (Vector3.Distance(transform.position, curTarget.transform.position) >= Stop_AttackDistance)
            //                    {
            //                        SetAnimationValues(false);
            //                        CanAttack = false;
            ////                        isStop = false;
            //                    }
            //                    else
            //                    {
            //                        SetAnimationValues(true);
            //                        CanAttack = true;
            ////                        isStop = true;
            //                    }
            //                }
            //                else
            //                {
            //                    WayPointsMoving();
            //                    CanAttack = false;
            //                }
            //            }
            //            else
            //            {
            //                WayPointsMoving();
            //                CanAttack = false;
            //            }

            if(isAngryZombie)
            {
                ReachTarget();
            }
            else
            {
                bool isPlayerNear = false;
                overlappingColliders = new List<Collider>(Physics.OverlapSphere(transform.position, visualDetectionDistance));
                foreach(Collider collider in overlappingColliders)
                {
                    if (collider.gameObject == curTarget.gameObject)
                    {
                        isPlayerNear = true;
                        break;
                    }
                }
                Vector3 detector = transform.Find("Detector").transform.position;
                Vector3 detectable = curTarget.transform.Find("Detectable").transform.position;
                Vector3 direction = (detectable - detector).normalized;
                direction.y *= 0;
                float angle = Vector3.Angle(transform.forward, direction);
                Ray ray = new Ray(detector, detectable - detector);
                isSeeingPlayer = isPlayerNear && angle <= FOVAngle / 2 && Physics.Raycast(ray, out RaycastHit hit, visualDetectionDistance) && hit.transform.gameObject == curTarget.gameObject;
            
                isHearingPlayer = false;
                overlappingColliders = new List<Collider>(Physics.OverlapSphere(transform.position, soundDetectionDistance));
                foreach (Collider collider in overlappingColliders)
                {
                    if (collider.gameObject == curTarget.gameObject && curTarget.isSprint)
                    {
                        isHearingPlayer = true;
                        break;
                    }
                }

                if (isSeeingPlayer || isHearingPlayer)
                {
                    isDetectingPlayer = true;
                    isAttackingPlayer = true;
                }
                else
                {
                    if(isAttackingPlayer)
                    {
                        if(isDetectingPlayer)
                        {
                            isDetectingPlayer = false;
                            detectionTimer = detectionDelay;
                        }
                        detectionTimer -= Time.deltaTime;
                        if(detectionTimer <= 0)
                        {
                            isAttackingPlayer = false;
                            //if (Vector3.Distance(transform.position, initialPosition) >= maxDistanceFromInitialPosition)
                            //{
                            //    isAttackingPlayer = false;
                            //}
                        }
                    }
                }

                if(isAttackingPlayer)
                {
                    ReachTarget();
                }
                else
                {
                    WayPointsMoving();
                    CanAttack = false;
                }
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

        //public void FindPlayers()
        //{
        //    targets.Clear();

        //    var foundPlayers = FindObjectsOfType<Controller>();

        //    for (int i = 0; i < foundPlayers.Length; i++)
        //    {
        //        targets.Add(foundPlayers[i].gameObject);
        //    }
        //}

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

        public void ReachTarget()
        {
            agent.SetDestination(curTarget.transform.position);
            agent.stoppingDistance = Stop_AttackDistance;
            HasIndex = false;
            if (Vector3.Distance(transform.position, curTarget.transform.position) >= Stop_AttackDistance)
            {
                SetAnimationValues(false);
                CanAttack = false;
//              isStop = false;
            }
            else
            {
                SetAnimationValues(true);
                CanAttack = true;
//              isStop = true;
            }
        }

        //public GameObject FindClosestPlayer()
        //{
        //    GameObject closest = null;
        //    float distance = Mathf.Infinity;
        //    Vector3 position = transform.position;

        //    foreach (GameObject player in targets)
        //    {
        //        Vector3 diff = player.transform.position - position;

        //        float curDistacne = diff.sqrMagnitude;
        //        if (curDistacne < distance)
        //        {
        //            closest = player;
        //            distance = curDistacne;
        //        }
        //    }

        //    return closest;
        //}

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




