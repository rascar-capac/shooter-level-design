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
        public List<Transform> WayPoints = null;
        public AudioClip attackingSound = null;
        public AudioClip abortSound = null;
        [Range(1, 100)] public float Stop_AttackDistance;
        [Range(1, 180)] public float FOVAngle;
        [Range(1, 100)] public float visualDetectionDistance;
        [Range(1, 100)] public float soundDetectionDistance;
        [Range(0, 100)] public float detectionDelay;

        public enum MoveOnWaypoints
        {
            Random,
            Course,
            FindNearestPoint,
            Wait
        };

        public MoveOnWaypoints EnemyMovement;

        [HideInInspector] public List<GameObject> targets;
        [HideInInspector] public Controller target;
        [HideInInspector] public bool CanAttack;

        private UnityEngine.AI.NavMeshAgent agent;

        private Animator anim;

        private int currentWP = 0;
        private int PreviousPoint = 0;

        private bool HasIndex;

        private GameObject initialPosition;
        private List<Collider> overlappingColliders;
        private bool isHearingPlayer;
        private bool isSeeingPlayer;
        private bool isDetectingPlayer;
        private bool isAttackingPlayer;
        private float detectionTimer;
        private AudioSource audioSource;

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
            audioSource = GetComponent<AudioSource>();
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
                    Gizmos.color = Color.cyan;
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
                //Gizmos.DrawWireSphere(transform.position, visualDetectionDistance);

                if(target)
                {
                    if(isAttackingPlayer)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.gray;
                    }
                    Gizmos.DrawRay(transform.position, (target.transform.position - transform.position).normalized * visualDetectionDistance);
                }

                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, transform.forward * visualDetectionDistance);
            }
        }

        void Update()
        {
            if (isAngryZombie)
            {
                ReachTarget();
            }
            else
            {
                if(WayPoints.Count == 0)
                {
                    initialPosition = new GameObject();
                    initialPosition.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    WayPoints.Insert(0, initialPosition.transform);
                }
                bool isPlayerNear = false;
                overlappingColliders = new List<Collider>(Physics.OverlapSphere(transform.position, visualDetectionDistance));
                foreach(Collider collider in overlappingColliders)
                {
                    if (collider.gameObject == target.gameObject)
                    {
                        isPlayerNear = true;
                        break;
                    }
                }
                Vector3 detector = transform.Find("Detector").transform.position;
                Vector3 detectable = target.transform.Find("Detectable").transform.position;
                Vector3 direction = (detectable - detector).normalized;
                direction.y *= 0;
                float angle = Vector3.Angle(transform.forward, direction);
                Ray ray = new Ray(detector, detectable - detector);
                isSeeingPlayer = isPlayerNear && angle <= FOVAngle / 2 && Physics.Raycast(ray, out RaycastHit hit, visualDetectionDistance) && hit.transform.gameObject == target.gameObject;

                isHearingPlayer = false;
                overlappingColliders = new List<Collider>(Physics.OverlapSphere(transform.position, soundDetectionDistance));
                foreach (Collider collider in overlappingColliders)
                {
                    if (collider.gameObject == target.gameObject && !target.isCrouch)
                    {
                        isHearingPlayer = true;
                        break;
                    }
                }

                if (isSeeingPlayer || isHearingPlayer)
                {
                    if(!isAttackingPlayer)
                    {
                        audioSource.PlayOneShot(attackingSound);
                    }
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
                            audioSource.PlayOneShot(abortSound);
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
                    case MoveOnWaypoints.Wait:
                        Wait_Move();
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

        public void Course_Move()
        {
            if (!HasIndex)
            {
                agent.stoppingDistance = 0.5f;
                currentWP = GetNearestObject(WayPoints);
                HasIndex = true;
            }

            agent.SetDestination(WayPoints[currentWP].position);
            if (Vector3.Distance(WayPoints[currentWP].position, transform.position) < 3)
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
                agent.stoppingDistance = 0.5f;

            agent.SetDestination(WayPoints[GetNearestObject(WayPoints)].position);
            if (Vector3.Distance(WayPoints[GetNearestObject(WayPoints)].position, transform.position) < 3)
            {
                PreviousPoint = currentWP;
                currentWP = GetNearestObject(WayPoints);
            }
        }

        public void Random_Move()
        {
            if (!HasIndex)
            {
                agent.stoppingDistance = 0.5f;
                currentWP = GetNearestObject(WayPoints);
                HasIndex = true;
            }

            agent.SetDestination(WayPoints[currentWP].position);
            if (Vector3.Distance(WayPoints[currentWP].position, transform.position) < 3)
            {
                PreviousPoint = currentWP;
                currentWP = Random.Range(0, WayPoints.Count);
            }
        }

        public void Wait_Move()
        {
            if(!HasIndex)
            {
                agent.stoppingDistance = 1;
            }
            Transform initPos = initialPosition.transform;
            if(transform.position != initPos.position)
            {
                agent.SetDestination(initPos.position);
                if (Vector3.Distance(initPos.position, transform.position) < 1)
                {
                    agent.Warp(initPos.position);
                    transform.rotation = initPos.rotation;
                }
            }
        }

        public void ReachTarget()
        {
            agent.SetDestination(target.transform.position);
            agent.stoppingDistance = Stop_AttackDistance;
            HasIndex = false;
            Vector3 detector = transform.Find("Detector").transform.position;
            Vector3 detectable = target.transform.Find("Detectable").transform.position;
            Vector3 direction = (detectable - detector).normalized;
            direction.y *= 0;
            if (Vector3.Distance(transform.position, target.transform.position) >= Stop_AttackDistance)
            {
                SetAnimationValues(false);
                CanAttack = false;
            }
            else if (Vector3.Angle(transform.forward, direction) < 90)
            {
                SetAnimationValues(true);
                CanAttack = true;
            }
        }

        int GetNearestObject(List<Transform> positions)
        {
            int bestPointIndex = 0;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            for (int i = 0; i < positions.Count; i++)
            {
                Vector3 directionToObject = positions[i].position - currentPosition;
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




