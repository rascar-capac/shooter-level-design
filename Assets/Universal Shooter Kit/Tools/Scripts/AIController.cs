// GercStudio
// © 2018-2019

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    public class AIController : MonoBehaviour
    {
        public bool take;
        
        public bool remove;
        
        public bool aim;

        public bool reload;
        
        [UnityEngine.Range(1, 100)] public float DistanceToSee;
        [UnityEngine.Range(1, 180)] public float horizontalAngleToSee;
        [UnityEngine.Range(1, 180)] public float heightToSee;

        [UnityEngine.Range(-90, 90)] public float HeadOffsetX;
        [UnityEngine.Range(-90, 90)] public float HeadOffsetY;
        [UnityEngine.Range(-90, 90)] public float HeadOffsetZ;
        
        public float SmoothIKSwitch;

        public int inspectorTab;
        public int nextBehaviour;
        public int currentBehaviour;
        public int lastBehaviour;
        public int currentWeapon;
        
        public List<WeaponsHelper.WeaponSlotInInventory> Weapons;
        
        [SerializeField] public CharacterHelper.BodyObjects BodyObjects;
        public IKHelper.FeetIKVariables IKVariables;
        
        public WeaponController weaponController;
        public AIController OriginalScript;

        public Image yellowImg;
        public Image redImg;

        public bool warning;
        public bool attack;
        
        public bool UseWeapon;
        
        public bool HasWeaponTaken;
        public bool hasAnyWeapon;
        public bool WeaponAdjustment;
        public bool hasWeaponTaken;
        public bool ActiveDebug;

        public bool AdjustmentScene;

        public enum EnemyStates
        {
            Waypoints,
            Warning, 
            Attack,
            Cover,
            FindAfterAttack
        }

        public EnemyStates State;

        public WaypointBehavior movementBehavior;

        public AnimatorOverrideController newController;

        public Helper.AnimationClipOverrides ClipOverrides;
        
        public AnimationClip[] MovementAnimations = new AnimationClip[20];

        [Serializable]
        public class Player
        {
            public GameObject player;
            public bool HearPlayer;
            public bool SeePlayer;
            public float warningValue;
            public float attackValue;
            public float distanceBetween;
            public float hearTime;
        }
        
        [SerializeField]
        public List<Player> Players;
        
        public Transform curTarget;
        [HideInInspector] public bool CanAttack;

        private NavMeshAgent agent;

        public Animator anim;

        public Vector3 currentWaypointPosition;
        private Vector3 currentCoverDirection;
        private Vector3 currentCoverPosition;
        
        public List<Transform> checkPositions;
        public GameObject currentCover;
        
        public int currentCheckPointNumber;

//        private int currentWP = 0;
//        private int PreviousPoint = 0;
        private float timer;
        private float addPointToCheckFunctionCount;
        private int attackCheckPointsCount;

        private bool HasIndex;
//        private bool _audio = true;
//        private bool isStop;
        private bool isMovementPause;
        private bool isNextAction;
        private bool nearCheckPoint;
        private bool playerInSeeArea;
        private bool setPointToCheck;
        private bool setCoverPoint;
        private bool behindCover;
//        private bool attackBehindCover;

        private float bodyRotationUpLimit_y;
        private float bodyRotationDownLimit_y;
        
        private float bodyRotationUpLimit_x;
        private float bodyRotationDownLimit_x;

        void Awake()
        {
            FindWeapons();
            FindPlayers();
        }

        void Start()
        {
            anim = gameObject.GetComponent<Animator>();

            agent = GetComponent<NavMeshAgent>();

            agent.updatePosition = false;
            
            
            var adj = FindObjectOfType<Adjustment>();
            
            if (adj)
            {
                AdjustmentScene = true;
                return;
            }
            
            anim.SetBool("Move", true);
        }

        IEnumerator StartMovement()
        {
            yield return new WaitForSeconds(0);
            anim.SetBool("Move", true);
            agent.updateRotation = true;
            agent.isStopped = false;
            StopCoroutine(StartMovement());
        }
        
        IEnumerator Think()
        {
            yield return new WaitForSeconds(10);
            
            Destroy(checkPositions[currentCheckPointNumber].gameObject);
            if (currentCheckPointNumber + 1 < checkPositions.Count)
            {
                currentCheckPointNumber += 1;
                agent.SetDestination(checkPositions[currentCheckPointNumber].position);
            }
            else
            {
                if (State == EnemyStates.Warning)
                {
                    warning = false;
                    State = EnemyStates.Waypoints;
                    agent.SetDestination(currentWaypointPosition);
                }
                else if (State == EnemyStates.FindAfterAttack)
                {
                    attack = false;
                    State = EnemyStates.Waypoints;
                    agent.SetDestination(currentWaypointPosition);
                    setPointToCheck = false;
                }
            }

            nearCheckPoint = false;
            StartCoroutine(StartMovement());
            StopCoroutine(Think());
        }

        void CheckSomePoints()
        {
            anim.SetBool("Attack state", false);
            
            if (Vector3.Distance(checkPositions[currentCheckPointNumber].position, transform.position) <= 5 & !nearCheckPoint)
            {
                nearCheckPoint = true;
                StopMovement();
                StartCoroutine(Think());
            }
        }

        IEnumerator AttackVision()
        {
            yield return new WaitForSeconds(5);
            
//            currentCheckPointNumber = 1;
//
//            addPointToCheckFunctionCount = 0;
//            AddPointToCheck(transform.forward, DistanceToSee);
//                
//            addPointToCheckFunctionCount = 0;
//            AddPointToCheck(transform.right, DistanceToSee);
//                
//            addPointToCheckFunctionCount = 0;
//            AddPointToCheck(-transform.forward, DistanceToSee);
//                
//            addPointToCheckFunctionCount = 0;
//            AddPointToCheck(-transform.right, DistanceToSee);
//            
//            Destroy(checkPositions[0].gameObject);
//
//            agent.SetDestination(checkPositions[1].position);
//            checkPositions.Clear();
//            nearCheckPoint = false;
//            setPointToCheck = false;
//            StartMovement();
            CreatePointToCheck(Players[0].player.transform.position, "attack");
            StopCoroutine(AttackVision());
        }

        void Attack()
        {
            if (!setCoverPoint)
            {
                anim.SetBool("Attack state", true);
                StartCoroutine(StartMovement());
                setCoverPoint = true;
                var coverPos = GetCoverPoint();
                if (coverPos != Vector3.zero)
                {
                    currentCoverPosition = coverPos;
                    agent.SetDestination(currentCoverPosition);
                    State = EnemyStates.Cover;
                }
                else
                {
                    // attack without covers
                    agent.isStopped = true; 
                }
            }
        }

        void FindAfterAttack()
        {
            if (setPointToCheck)
            {
                if (Vector3.Distance(checkPositions[currentCheckPointNumber].position, transform.position) <= 5 & !nearCheckPoint)
                {
                    nearCheckPoint = true;
                    StopMovement();
                    StartCoroutine(Think());
                }

                if (Players[0].HearPlayer || Players[0].SeePlayer)
                {
                    //returnAttack
                    State = EnemyStates.Attack;
                    setCoverPoint = false;
                    setPointToCheck = false;
                    var count = checkPositions.Count;
                    for (var i = 0; i < count; i++)
                    {
                        if(checkPositions[i])
                            Destroy(checkPositions[i].gameObject);
                    }
                    checkPositions.Clear();
                }
            }
        }

        void Cover()
        {
            if (!setCoverPoint)
            {
                StartCoroutine(StartMovement());
//                if(!weaponController.IsAimEnabled)
//                    weaponController.Aim();
                
                setCoverPoint = true;
                var coverPos = GetCoverPoint();
                if (coverPos != Vector3.zero)
                {
                    currentCoverPosition = coverPos;
                    
                    if (Vector3.Distance(currentCoverPosition, transform.position) < 15)
                    {
                        if(!anim.GetBool("Crouch"))
                            anim.SetBool("Crouch", true);
                    }
                    else
                    {
                        if(anim.GetBool("Crouch"))
                            anim.SetBool("Crouch", false);
                    }
                    
                    agent.SetDestination(currentCoverPosition);
                }
                else
                {
                    agent.isStopped = true;
                    print("not cover");
                }
            }
            else
            {
                if (agent.remainingDistance <= 1 && !nearCheckPoint)
                {
                    //anim.SetBool("Cover state", true);
                    // weaponController.Aim();
                    anim.SetBool("Crouch", true);
                    //agent.SetDestination(Players[0].player.transform.position);
                    nearCheckPoint = true;
                    behindCover = true;
                    StopMovement();
                }

                if (currentCover)
                {
                    CheckCoverPosition();
                    BehindCover();
                }
                else
                {
                    setCoverPoint = false;
                   // print("not cover");
                }
            }
        }

        void CheckCoverPosition()
        {
            var newDirection = currentCover.transform.position - Players[0].player.transform.position;

            if (Mathf.Abs(Helper.AngleBetween(currentCoverDirection, newDirection)) > 60 && nearCheckPoint)
            {
                behindCover = false;
                timer = 0;
                setCoverPoint = false;
                nearCheckPoint = false;
            }
        }

        void CheckRotation()
        {
            var newDirection = Players[0].player.transform.position - transform.position;
            var angle = Helper.AngleBetween(transform.forward, newDirection);
            
            anim.SetFloat("Angle to player", angle);

            var direction = Players[0].player.GetComponent<Controller>().BodyObjects.Head.transform.position - BodyObjects.TopBody.position;

            var middleAngleX = Helper.AngleBetween(direction, BodyObjects.TopBody).x;
            var middleAngleY = Helper.AngleBetween(direction, BodyObjects.TopBody).y;

           if (anim.GetBool("Move") || anim.GetBool("Crouch"))
           {
               bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 0, 5 * Time.deltaTime);
               bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, 0, 5 * Time.deltaTime);
               
               bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, 0, 5 * Time.deltaTime);
               bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, 0, 5 * Time.deltaTime);
           }
           else
           {
               bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 60, 5 * Time.deltaTime);
               bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, -60, 5 * Time.deltaTime);
               
               bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, 45, 5 * Time.deltaTime);
               bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, -45, 5 * Time.deltaTime);
           }
           
           
           if (middleAngleY > bodyRotationUpLimit_y)
               middleAngleY = bodyRotationUpLimit_y;
           else if(middleAngleY < bodyRotationDownLimit_y)
               middleAngleY = bodyRotationDownLimit_y;
           
           if (middleAngleX > bodyRotationUpLimit_x)
               middleAngleX = bodyRotationUpLimit_x;
           else if(middleAngleX < bodyRotationDownLimit_x)
               middleAngleX = bodyRotationDownLimit_x;
           
            BodyObjects.TopBody.Rotate(-middleAngleX, -middleAngleY, 0);
        }

        void BehindCover()
        {
            if (behindCover)
            {
                timer += Time.deltaTime;

                if (anim.GetBool("Crouch") && timer > 30)
                {
//                    attackBehindCover = true;
                    Aim();
                    anim.SetBool("Crouch", false);
                    timer = 0;
                }
                else if (!anim.GetBool("Crouch") && timer > 30)
                {
//                    attackBehindCover = false;
                    Aim();
                    anim.SetBool("Crouch", true);
                    weaponController.Attack(false, "Auto", true);
                    anim.SetBool("Attack", false);
                    timer = 0;
                }
            }
            else
            {
//                attackBehindCover = false;
            }
        }

        void Aim()
        {
            weaponController.Aim(false, false);
        }

        Vector3 GetCoverPoint()
        {
            var collidersNearEnemy = Physics.OverlapSphere(transform.position, DistanceToSee * 2);
            var collidersNearPlayer = Physics.OverlapSphere(Players[0].player.transform.position, DistanceToSee * 2);

            var coversNearEnemy = new List<GameObject>();
            var coversNearPlayer = new List<GameObject>();

            foreach (var collider in coversNearPlayer)
            {
                if (collider.gameObject.GetComponent<Surface>() && collider.gameObject.GetComponent<Surface>().isCover)
                {
                    coversNearPlayer.Add(collider.gameObject);
                }
            }

            foreach (var collider in collidersNearEnemy)
            {
                if (collider.gameObject.GetComponent<Surface>() && collider.gameObject.GetComponent<Surface>().isCover)
                {
                    if (collidersNearPlayer.Any(col =>
                        col.gameObject.GetInstanceID() == collider.gameObject.GetInstanceID()))
                    {
                        coversNearEnemy.Add(collider.gameObject);
                    }
                }
            }

            currentCover = FindClosestObject(coversNearEnemy.ToArray());

            if (!currentCover)
                currentCover = FindClosestObject(coversNearPlayer.ToArray());

            var newPoint = Vector3.zero;

            if (currentCover)
            {
                currentCoverDirection = currentCover.transform.position - Players[0].player.transform.position;
                currentCoverDirection.Normalize();

                newPoint = currentCover.transform.position;

                var i = 0f;
                while (currentCover.GetComponent<Collider>().bounds.Contains(newPoint - currentCoverDirection))
                {
                    i += 1;
                    newPoint = currentCover.transform.position + currentCoverDirection * i;
                }
                
                var point = new GameObject("Point to check");
                point.transform.position = newPoint;

#if UNITY_EDITOR
                Helper.AddObjectIcon(point, "Point to check (warning)");  
#endif
               
            }

            return newPoint;
        }

        void Update()
        {
//            var dirToClosestCorner = GetClosestPathCorner(agent.path) - transform.position;
//            var lookRot = Quaternion.LookRotation(dirToClosestCorner);
//            var angleBetween = Helper.AngleBetween(dirToClosestCorner, transform.forward);
            
            
//            
//            if (anim.GetBool("Move") && Mathf.Abs(angleBetween) < 90)
//            {
//                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 1 * Time.deltaTime);
//                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
//            }



//            Debug.DrawRay(GetClosestPathCorner(agent.path), Vector3.up * 10, Color.green);
//
//            Debug.DrawRay(transform.position + Vector3.up, dirToClosestCorner.normalized * 10, Color.magenta);
//            if (aim)
//            {
//                aim = false;
//                weaponController.Aim();
//            }

            if (AdjustmentScene)
                return;

            if (hasAnyWeapon && weaponController)
            {
                //!!!//
//                if(weaponController.WeaponType == WeaponsHelper.TypeOfWeapon.SingleShots || weaponController.WeaponType == WeaponsHelper.TypeOfWeapon.RocketLauncher ||
//                   weaponController.WeaponType == WeaponsHelper.TypeOfWeapon.Knife)
//                        weaponController.Attack(attackBehindCover, "Single", true);
//                else weaponController.Attack(attackBehindCover, "Auto", true);
            }

//            
            if (State == EnemyStates.Warning || State == EnemyStates.Attack)
            {
                if (!UseWeapon)
                {
                    UseWeapon = true;
                    TakeWeapon(0);
                }
            }
            else if (State == EnemyStates.Waypoints)
            {
                if (UseWeapon)
                {
                    RemoveWeapon();
                    UseWeapon = false;
                }
            }

            if (reload)
            {
                reload = false;
                weaponController.Reload();
            }
            
            if (remove)
            {
                UseWeapon = false;
                remove = false;
                anim.SetBool("Use weapon", false);
            }

            if (UseWeapon & anim.GetLayerWeight(1) < 1)
            {
                anim.SetLayerWeight(1, 1);
            }
            if (!UseWeapon & anim.GetLayerWeight(1) > 0)
            {
                anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0, 2 * Time.deltaTime));
            }
            
//            if (curTarget)
//                currentTargetPosition = curTarget.position;
           
            timer += Time.deltaTime;
            
//            if (timer > 2)
//            {
//                FindPlayers();
////                if (targets.Count > 0)
////                    curTarget = FindClosestPlayer().transform;
//                timer = 0;
//            }

            
            switch (State)
            {
                case EnemyStates.Waypoints:
                    WayPointsMoving();
                    break;
                case EnemyStates.Warning:
                    CheckSomePoints();
                    break;
                case EnemyStates.Attack:
                    Attack();
                    break;
                case EnemyStates.Cover:
                    Cover();
                    break;
                case EnemyStates.FindAfterAttack:
                    FindAfterAttack();
                    break;
            }

            if (Players.Count > 0)
            {
                yellowImg.fillAmount = Players[0].warningValue;
                redImg.fillAmount = Players[0].attackValue;
            }

//
            if (Players.Count > 0 & State != EnemyStates.Cover)
            {
//                foreach (var player in Players)
//                {
                    var distance = Vector3.Distance(transform.position, Players[0].player.transform.position);
                    Players[0].distanceBetween = distance;
                    
                    if (distance < DistanceToSee)
                    {
                        if (CheckRaycast(Players[0].player.GetComponent<Controller>().BodyObjects.Hips))
                        {
                            Players[0].SeePlayer = true;
                            
                            if(!warning)
                                IncreaseWarningValue(3);
                            else
                            {
                                if (!attack)
                                    IncreaseAttackValue(3);
                            }
                        }

                        if (CheckRaycast(Players[0].player.GetComponent<Controller>().BodyObjects.Head))
                        {
                            Players[0].SeePlayer = true;
                            
                            if(!warning)
                                IncreaseWarningValue(3);
                            else
                            {
                                if (!attack)
                                    IncreaseAttackValue(3);
                            }
                        }

                        if (!CheckRaycast(Players[0].player.GetComponent<Controller>().BodyObjects.Hips) &
                            !CheckRaycast(Players[0].player.GetComponent<Controller>().BodyObjects.Head))
                            Players[0].SeePlayer = false;

//                        if (CheckRaycast(Players[0].player.transform))
//                        {
//                            increaseValue(3);
//                            Players[0].SeePlayer = true;
//                        }
//                        else
//                        {
//                            Players[0].SeePlayer = false; 
//                        }

                        Debug.DrawLine(BodyObjects.Head.position, Players[0].player.GetComponent<Controller>().BodyObjects.Hips.position,
                            CheckRaycast(Players[0].player.GetComponent<Controller>().BodyObjects.Hips) ? Color.red : Color.green);
                        
                        Debug.DrawLine(BodyObjects.Head.position, Players[0].player.GetComponent<Controller>().BodyObjects.Head.position,
                            CheckRaycast(Players[0].player.GetComponent<Controller>().BodyObjects.Head) ? Color.red : Color.green);
                        
//                        Debug.DrawLine(head.position, Players[0].player.transform.position,
//                            CheckRaycast(Players[0].player.transform) ? Color.red : Color.green);
                    }
                    else
                    {
                        Players[0].SeePlayer = false;
                    }

                    if (Players[0].HearPlayer & !warning)
                    {
                        IncreaseWarningValue(2);
                    }
                    else if (Players[0].HearPlayer & warning & !attack)
                    {
                        IncreaseAttackValue(2);
                    }

//                    if (State == EnemyStates.Attack & !Players[0].HearPlayer & !Players[0].SeePlayer)
//                    {
//                        if (!setPointToCheck)
//                        {
//                            CreatePointToCheck(Players[0].player.transform.position, "attack");
//                            currentCheckPointNumber = 0;
//                            agent.SetDestination(checkPositions[0].position);
//                            StartMovement();
//                            StartCoroutine(AttackVision());
//                            setPointToCheck = true;
//                        }
//                    }
//                    if (!warning & !Players[0].SeePlayer & !Players[0].HearPlayer)
//                        reductionValue();
                    

                    if (Players[0].attackValue < 0)
                        Players[0].attackValue = 0;
            }
        }

        private void LateUpdate()
        {
//            if (!anim.GetBool("Move"))
//            {
            if (Players.Count > 0)
                CheckRotation();

//            }
//            else
//            {
//                BodyObjects.TopBody.LookAt(Players[0].player.transform);
//                anim.SetFloat("Angle to player", 0);
//            }
        }

        void AddPointToCheck(Vector3 direction, float distance)
        {
            addPointToCheckFunctionCount++;
            var finalPosition = checkPositions[0].position + direction * distance + Vector3.up;
            var hitColliders = Physics.OverlapSphere(finalPosition, 1);
            
            if (hitColliders.Length <= 0)
            {
                CreatePointToCheck(finalPosition, "warning");
            }
            else
            {
                if(addPointToCheckFunctionCount < 3)
                    AddPointToCheck(direction, distance / 2);
            }
        }

        void CreatePointToCheck(Vector3 position, string type)
        {
            var point = new GameObject("Point to check");
            point.transform.position = position;
#if UNITY_EDITOR
            Helper.AddObjectIcon(point, type == "warning" ? "Point to check (warning)" : "Point to check (attack)");
#endif
            
            checkPositions.Add(point.transform);
        }

        void IncreaseAttackValue(float speed)
        {
            if (Players[0].attackValue < 1)
            {
                Players[0].attackValue += Time.deltaTime / speed;
            }
            else
            {
                attack = true;
                State = EnemyStates.Attack;
                var count = checkPositions.Count;
                for (var i = 0; i < count; i++)
                {
                    if(checkPositions[i])
                        Destroy(checkPositions[i].gameObject);
                }
                checkPositions.Clear();
                //agent.SetDestination(Players[0].player.transform.position);
                StopMovement();
            }
        }

        void IncreaseWarningValue(float speed)
        {
            if (Players[0].warningValue < 1)
            {
                Players[0].warningValue += Time.deltaTime / speed;
            }
            else
            {
                warning = true;
                State = EnemyStates.Warning;
                currentCheckPointNumber = 0;
                checkPositions.Clear();

                CreatePointToCheck(Players[0].player.transform.position + Vector3.up, "warning");

                addPointToCheckFunctionCount = 0;
                AddPointToCheck(transform.forward, DistanceToSee);
                
                addPointToCheckFunctionCount = 0;
                AddPointToCheck(transform.right, DistanceToSee);
                
                addPointToCheckFunctionCount = 0;
                AddPointToCheck(-transform.forward, DistanceToSee);
                
                addPointToCheckFunctionCount = 0;
                AddPointToCheck(-transform.right, DistanceToSee);

//                if (Players[0].attackValue < 1)
//                    Players[0].attackValue += Time.deltaTime / 10;

                StartCoroutine(ISawSomething());
            }
        }

        IEnumerator ISawSomething()
        {
            StopMovement();
            yield return new WaitForSeconds(Random.Range(1,10));
            
            if (attack)
            {
                StopCoroutine(ISawSomething());
                yield return 0;
            }
            else
            {
                agent.SetDestination(checkPositions[0].position);
                StartCoroutine(StartMovement());
                StopCoroutine(ISawSomething());
            }
        }

        void reductionValue()
        {
            if (Players[0].attackValue > 0)
            {
                Players[0].attackValue -= Time.deltaTime;
            }
            else
            {
                if (Players[0].warningValue > 0)
                    Players[0].warningValue -= Time.deltaTime;
            }
        }

        bool CheckRaycast(Transform targetPoint)
        {
            var direction = targetPoint.position - BodyObjects.Head.position;
            var look = Quaternion.LookRotation(direction);

            var horizontalAngle = look.eulerAngles.y;
            if (horizontalAngle > 180)
                horizontalAngle -= 360;

            var spineAngleY = BodyObjects.Head.eulerAngles.y;
            if (spineAngleY > 180)
                spineAngleY -= 360;

            var middleAngleY = Mathf.DeltaAngle(horizontalAngle, spineAngleY);
            
            var verticalAngle = look.eulerAngles.x;
            if (verticalAngle > 180)
                verticalAngle -= 360;

            var spineAngleX = BodyObjects.Head.eulerAngles.x;
            if (spineAngleX > 180)
                spineAngleX -= 360;

            var middleAngleX = Mathf.DeltaAngle(verticalAngle, spineAngleX);

            RaycastHit info;
            Physics.Linecast(BodyObjects.Head.position, targetPoint.position, out info);

            var checkRay = false;
            
            if (info.transform)
            {
                if (!info.transform.root.gameObject.GetComponent<Controller>() & !info.transform.root.GetComponent<AIController>())
                    checkRay = true;
            }
            
            return Mathf.Abs(middleAngleY) < horizontalAngleToSee / 2
                   & Mathf.Abs(middleAngleX) < Mathf.Abs(Mathf.Asin(heightToSee / 2 / DistanceToSee) * 180 / Mathf.PI) & !checkRay;
        }
        
        void ChoiceNextAction()
        {
            switch (movementBehavior.points[currentBehaviour].action)
            {
                case Helper.NextPointAction.NextPoint:
                    CalculationNextPointIndex(Helper.NextPointAction.NextPoint);
                    break;
                case Helper.NextPointAction.RandomPoint:
                    CalculationNextPointIndex(Helper.NextPointAction.RandomPoint);
                    break;
                case Helper.NextPointAction.ClosestPoint:
                    CalculationNextPointIndex(Helper.NextPointAction.ClosestPoint);
                    break;
                case Helper.NextPointAction.Stop:
                    StopMovement();
                    break;
            }
            

            if (movementBehavior.points[currentBehaviour].waitTime > 0 & movementBehavior.points[currentBehaviour].action != Helper.NextPointAction.Stop)
            {
                isMovementPause = true;
                StopMovement();
            }
            else
            {
                agent.SetDestination(currentWaypointPosition);
            }
        }

        void WayPointsMoving()
        {
            anim.SetBool("Attack state", false);
            
            if (movementBehavior.points.Count > 0)
            {
                if (!HasIndex)
                {
                    currentWaypointPosition = movementBehavior.points[0].point.transform.position;
                    HasIndex = true;
                    agent.SetDestination(currentWaypointPosition);
                }

                if (Vector3.Distance(currentWaypointPosition, transform.position) <= 1)
                {
                    if (!isNextAction)
                    {
                        lastBehaviour = currentBehaviour;
                        currentBehaviour = nextBehaviour;
                        ChoiceNextAction();
                        isNextAction = true;
                    }
                }
                else
                {
                    isNextAction = false;
                }
            }
            else
                Debug.LogError(
                    "(Enemy) Not found waypoints with tag 'WayPoint'. Add them, otherwise the enemy won't move through the control points");
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.root.GetComponent<Controller>())
            {
                foreach (var player in Players)
                {
                    if (player.player.Equals(other.transform.root.gameObject))
                    {
                        player.HearPlayer = true;
                        player.hearTime += Time.deltaTime;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.root.GetComponent<Controller>())
            {
                foreach (var player in Players)
                {
                    if (player.player.Equals(other.transform.root.gameObject))
                    {
                        player.HearPlayer = false;
                        player.hearTime = 0;
                    }
                }
            }
        }

        IEnumerator MovePause(float time)
        {
            yield return new WaitForSeconds(time + 2);
            isMovementPause = false;
            agent.SetDestination(currentWaypointPosition);
            anim.SetBool("Move", true);
            //GetComponent<LocomotionSimpleAgent>().canMove = true;
//            StartCoroutine(SmoothMovementStateChange(1));
            StopCoroutine("MovePause");
        }

//        void SetAnimationValues(bool value)
//        {
//            if (Helper.HasParameter("CanWalk", anim))
//                anim.SetBool("CanWalk", !value);
//            else
//                Debug.LogError(
//                    "(Enemy) Not found variable 'CanWalk' in Animator. Create it, otherwise the aim animation won't work correctly. To find detailed information 'How tune the animator' read my Documentation.",
//                    gameObject);
//
//            if (Helper.HasParameter("CanAttack", anim))
//                anim.SetBool("CanAttack", value);
//            else
//                Debug.LogError(
//                    "(Enemy) Not found variable 'CanAttack' in Animator. Create it, otherwise the aim animation won't work correctly. To find detailed information 'How tune the animator' read my Documentation.",
//                    gameObject);
//        }

        public void FindPlayers()
        {
            if(Players.Count > 0)
                Players.Clear();
            var foundPlayers = FindObjectsOfType<Controller>();

            foreach (var player in foundPlayers)
            {
                Players.Add(new Player {HearPlayer = false, player = player.gameObject});
            }
        }

        public void CalculationNextPointIndex(Helper.NextPointAction currentAction)
        {
            switch (currentAction)
            {
                case Helper.NextPointAction.NextPoint:
                    nextBehaviour++;
                    if (nextBehaviour >= movementBehavior.points.Count)
                        nextBehaviour = 0;
                    break;
                case Helper.NextPointAction.RandomPoint:
                    nextBehaviour = Random.Range(0, movementBehavior.points.Count);
                    break;
                case Helper.NextPointAction.ClosestPoint:
                    nextBehaviour = GetNearestPoint(movementBehavior.points);
                    break;
            }
            currentWaypointPosition = movementBehavior.points[nextBehaviour].point.transform.position;
        }
        
        
        public void StopMovement()
        {
            agent.isStopped = true;
            anim.SetBool("Move", false);
            agent.updateRotation = false;
            if (isMovementPause & State == EnemyStates.Waypoints) StartCoroutine(MovePause(movementBehavior.points[currentBehaviour].waitTime));
        }

        GameObject FindClosestObject(GameObject[] objects)
        {
            GameObject closest = null;
            var distance = float.MaxValue;

            foreach (var obj in objects)
            {
                var enemyDistance = Vector3.Distance(obj.transform.position, transform.position);
                var characterDistance = Vector3.Distance(obj.transform.position, Players[0].player.transform.position);
                
                if (characterDistance > 10 && enemyDistance < distance)
                {
                    closest = obj;
                    distance = enemyDistance;
                }
            }

            return closest;
        }

        void OnAnimatorMove()
        {
//            //Update postion to agent position
//            transform.position = agent.nextPosition;
//
//            // Update position based on animation movement using navigation surface height
//            Vector3 position = anim.rootPosition;
//            position.y = agent.nextPosition.y;
//            transform.position = position;

            if(!anim)
                return;

            agent.speed = (anim.deltaPosition / Time.deltaTime).magnitude;
            
            //Debug.DrawRay(transform.position + Vector3.up, agent.velocity.normalized * 10, Color.cyan);

//            var direction = Vector3.zero;
//            
//            var closestPathCorner = GetClosestPathCorner(agent.path);
//            
//            if(closestPathCorner != Vector3.zero)
//              direction = transform.position - closestPathCorner;
//
//            anim.SetFloat("Horizontal", direction.normalized.x);
//            anim.SetFloat("Vertical", direction.normalized.y);

            transform.position = agent.nextPosition;
            transform.rotation = anim.rootRotation;
        }

        Vector3 GetClosestPathCorner(NavMeshPath path)
        {
            var dist = float.MaxValue;
            var closestCorner = Vector3.zero;
            
            if (path.corners.Length > 0)
            {
                foreach (var corner in path.corners)
                {
                    var curDist = Vector3.Distance(transform.position, corner);
                    if (Math.Abs(curDist) < 0.1f || curDist > dist) continue;

                    dist = curDist;
                    closestCorner = corner;
                }
            }
            else
            {
                closestCorner = agent.pathEndPosition;
            }

            return closestCorner;
        }
        
//        public void SetAnimatorParameters()
//        {
//            if (weaponController.characterAnimations.WeaponAttack)
//                controller.ClipOverrides["WeaponAttack"] = weaponController.characterAnimations.WeaponAttack;
//            else
//                Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon attack] animation.",
//                    weaponController.gameObject);
//
//            if (weaponController.characterAnimations.WeaponIdle)
//                controller.ClipOverrides["WeaponIdle"] = weaponController.characterAnimations.WeaponIdle;
//            else
//                Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon idle] animation.",
//                    weaponController.gameObject);
//
//            if (weaponController.WeaponType != WeaponController.TypeOfWeapon.Knife)
//            {
//                if (weaponController.characterAnimations.WeaponReload)
//                    controller.ClipOverrides["WeaponReload"] = weaponController.characterAnimations.WeaponReload;
//                else
//                    Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon reload] animation.",
//                        weaponController.gameObject);
//            }
//
//            if (weaponController.characterAnimations.WeaponWalk)
//                controller.ClipOverrides["WeaponWalk"] = weaponController.characterAnimations.WeaponWalk;
//            else
//                Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon walk] animation.",
//                    weaponController.gameObject);
//            
//            if (weaponController.characterAnimations.WeaponRun)
//                controller.ClipOverrides["WeaponRun"] = weaponController.characterAnimations.WeaponRun;
//            else
//                Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon run] animation.",
//                    weaponController.gameObject);
//           
//            if (weaponController.characterAnimations.TakeWeapon)
//                controller.ClipOverrides["TakeWeapon"] = weaponController.characterAnimations.TakeWeapon;
//            else
//                Debug.LogWarning("<color=yellow>Missing Component</color> [Take weapon] animation.",
//                    weaponController.gameObject);
//
//            controller.newController.ApplyOverrides(controller.ClipOverrides);
//
//            StartCoroutine("SetAnimParameters");
//        }
        
        public void ResetAnimatorParameters()
        {
            foreach (AnimatorControllerParameter parameter in anim.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    if (parameter.name != "Move")
                        anim.SetBool(parameter.name, false);
                }

            }
        }

        public void Null_Weapons()
        {
            if (!hasAnyWeapon) return;
            
            foreach (var weapon in Weapons)
            {
                weapon.weapon.SetActive(false);
            }
        }
        
        IEnumerator TakeWeapon()
        {
            yield return new WaitForSeconds(weaponController.characterAnimations.TakeWeapon.length /2);
            hasWeaponTaken = true;
            SmoothIKSwitch = 0;
           // weaponController.canAttack = true;
            //anim.SetBool("HasWeaponTaken", false);
           // StartCoroutine("ShootingTimeout");
            StopCoroutine("TakeWeapon");
        }


        IEnumerator RemoveWeaponTimeout()
        {
            yield return new WaitForSeconds(1);
            weaponController.gameObject.SetActive(false);
            weaponController = null;
            ResetAnimatorParameters();
        }
        

        void RemoveWeapon()
        {
            anim.SetBool("Use weapon", false);
            StartCoroutine(RemoveWeaponTimeout());
            
        }
        
        public void TakeWeapon(int weaponIndex)
        {
            StopCoroutine("TakeWeapon");

            Null_Weapons();
            Weapons[weaponIndex].weapon.SetActive(true);
            weaponController = Weapons[weaponIndex].weapon.GetComponent<WeaponController>();
            weaponController.canAttack = false;
            
            weaponController.AIController = this;
            weaponController.characterAnimations.anim = anim;

            weaponController.WeaponParent = Helper.Parent.Enemy;
            ResetAnimatorParameters();

            //SetAnimatorParameters();

            //gun = slots[slot].weaponsInInventory[slots[slot].currentWeaponInSlot].weapon;
            
            hasWeaponTaken = false;
            anim.SetBool("Use weapon", true);
            anim.CrossFade("Entry", 0, 1);
            StartCoroutine(TakeWeapon());

            //anim.SetBool("CanWalkWithWeapon", true);
            //anim.SetBool("NoWeapons", false);
            
            hasAnyWeapon = true;
        }

        public void FindWeapons()
        {
            hasAnyWeapon = false;

            for (var i = 0; i < Weapons.Count; i++)
            {
                var weapon = Weapons[i].weapon;
//                var saveSlot = Weapons[i].tpSlotIndex;
                if (!weapon) return;
                if (!weapon.GetComponent<WeaponController>())
                    return;

                var instantiatedWeapon = Instantiate(weapon);
                Weapons[i].weapon = instantiatedWeapon;
                Weapons[i].weapon.SetActive(false);
                
                hasAnyWeapon = true;
                
                var weaponController = instantiatedWeapon.GetComponent<WeaponController>();
                weaponController.WeaponParent = Helper.Parent.Enemy;
                weaponController.Parent = transform;
                weaponController.OriginalScript = weapon.GetComponent<WeaponController>();
                weaponController.AIController = this;
               // weaponController.curAmmo = weaponController.maxAmmo;
                weaponController.enabled = true;
//                weaponController.tpsSettingsSlot = saveSlot;

                if (!BodyObjects.RightHand) continue;
                if (instantiatedWeapon.transform.parent != BodyObjects.RightHand)
                    instantiatedWeapon.transform.parent = BodyObjects.RightHand;
            }

        }

        #region Sounds

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

            #endregion
            
        int GetNearestPoint(List<WaypointBehavior.Behavior> points)
        {
            var bestPointIndex = 0;
            var closestDistanceSqr = Mathf.Infinity;
            var myPosition = transform.position;
            for (var i = 0; i < points.Count; i++)
            {
                Vector3 directionToObject = points[i].point.transform.position - myPosition;
                var dSqrToTarget = directionToObject.sqrMagnitude;
                
                if (dSqrToTarget < closestDistanceSqr & i != nextBehaviour & i != lastBehaviour)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestPointIndex = i;
                }
            }
            

            return bestPointIndex;
        }
        
        #region HandsIK

        private void OnAnimatorIK(int layerIndex)
        {
            if (weaponController)
            {
                if (weaponController.IsReloadEnabled)
                {
                    Helper.FingersRotate(null, anim, "Reload");
                }
                else
                { 
                    Helper.FingersRotate(weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex], anim, "Weapon");
                }

                if (!weaponController.ActiveDebug)
                {
                    if (weaponController.IsReloadEnabled || !UseWeapon)
                    {
                        if (SmoothIKSwitch >= 0)
                            SmoothIKSwitch -= 1 * Time.deltaTime;
                        else
                        {
                            SmoothIKSwitch = 0;
                            //anim.SetBool("HasWeaponTaken", false);
                        }
                    }
                    else
                    {
                        if (SmoothIKSwitch <= 1)
                            SmoothIKSwitch += 1 * Time.deltaTime;
                        else
                        {
                            SmoothIKSwitch = 1;
                           // anim.SetBool("HasWeaponTaken", true);
                        }
                    }

                    if (layerIndex == 1)
                    {
                        if (weaponController.IkObjects.RightObject && weaponController.IkObjects.LeftObject)
                        {
                            if (weaponController.CanUseIK && hasWeaponTaken)
                            {
                                Helper.HandIK(this, weaponController, weaponController.IkObjects.LeftObject,
                                    weaponController.IkObjects.RightObject, BodyObjects.LeftHand, BodyObjects.RightHand,
                                    SmoothIKSwitch);
                            }
                        }
                    }
                }
                else
                {
                    // Elbows rotation 
                    anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
                    anim.SetIKHintPosition(AvatarIKHint.LeftElbow,
                        weaponController.IkObjects.LeftElbowObject.position);

                    anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
                    anim.SetIKHintPosition(AvatarIKHint.RightElbow,
                        weaponController.IkObjects.RightElbowObject.position);
                    //

                    if (weaponController.DebugMode == WeaponsHelper.IkDebugMode.Aim)
                    {
                        weaponController.hasAimIKChanged = true;
                        Helper.HandIK(this, weaponController, weaponController.IkObjects.LeftAimObject,
                            weaponController.IkObjects.RightAimObject, BodyObjects.TopBody, BodyObjects.TopBody, 1);
                    }
                    else if (weaponController.DebugMode == WeaponsHelper.IkDebugMode.Wall)
                    {
                        weaponController.hasWallIKChanged = true;
                        Helper.HandIK(this, weaponController, weaponController.IkObjects.LeftWallObject,
                            weaponController.IkObjects.RightWallObject, BodyObjects.TopBody, BodyObjects.TopBody, 1);
                    }
                    else
                    {
                        Helper.HandIK(this, weaponController, weaponController.IkObjects.LeftObject,
                            weaponController.IkObjects.RightObject, BodyObjects.TopBody, BodyObjects.TopBody, 1);
                    }
                }
            }
            else
            {
                Helper.FingersRotate(null, anim, "Null");
            }

            
            // Feet IK
            if (layerIndex == 0)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                //anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("RightFoot"));

                IKHelper.MoveFeetToIkPoint(this, "right");

                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                //anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("LeftFoot"));

                IKHelper.MoveFeetToIkPoint(this, "left");
            }

            //anim.bodyPosition = new Vector3(anim.bodyPosition.x, -2.3f, anim.bodyPosition.z);
        }
        #endregion
        
        #region FeetIK

        void FixedUpdate()
        {
            IKHelper.AdjustFeetTarget(this, "right");
            IKHelper.AdjustFeetTarget(this, "left");

            IKHelper.FeetPositionSolver(this, "right");
            IKHelper.FeetPositionSolver(this, "left");
        }

        #endregion

        #region gizmos

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
//            var radius = DistanceToSee * Math.Abs(Mathf.Tan(horizontalAngleToSee * Mathf.Deg2Rad / 2));
//
//            var side = DistanceToSee * Math.Abs(Mathf.Cos(horizontalAngleToSee * Mathf.Deg2Rad / 2));

            //var lookAt = GetComponent<LookAt> ();

            var dir = BodyObjects.Head.forward;
            dir = Quaternion.Euler(HeadOffsetX, HeadOffsetY, HeadOffsetZ) * dir;


            var xLeftDir = Quaternion.Euler(0, horizontalAngleToSee / 2, 0) * dir;
            var xRightDir = Quaternion.Euler(0, -horizontalAngleToSee / 2, 0) * dir;

            var position = BodyObjects.Head.transform.position;

            Handles.zTest = CompareFunction.Greater;
            Handles.color = new Color32(255, 0, 0, 50);
            DrawArea(position, xRightDir, xLeftDir, dir);

            Handles.zTest = CompareFunction.Less;
            Handles.color = new Color32(255, 0, 0, 255);
            DrawArea(position, xRightDir, xLeftDir, dir);

        }

        void DrawArea(Vector3 position, Vector3 xRightDir, Vector3 xLeftDir, Vector3 dir)
        {
            Handles.DrawLine(position, position + xLeftDir * DistanceToSee - transform.up * heightToSee / 2);
            Handles.DrawLine(position, position + xRightDir * DistanceToSee - transform.up * heightToSee / 2);

            Handles.DrawWireArc(position - transform.up * heightToSee / 2, transform.up, dir, horizontalAngleToSee / 2,
                DistanceToSee);
            Handles.DrawWireArc(position - transform.up * heightToSee / 2, transform.up, dir, -horizontalAngleToSee / 2,
                DistanceToSee);


            Handles.DrawLine(position + xLeftDir * DistanceToSee,
                position + xLeftDir * DistanceToSee + transform.up * heightToSee / 2);
            Handles.DrawLine(position + xRightDir * DistanceToSee,
                position + xRightDir * DistanceToSee + transform.up * heightToSee / 2);

            Handles.DrawLine(position + xLeftDir * DistanceToSee,
                position + xLeftDir * DistanceToSee - transform.up * heightToSee / 2);
            Handles.DrawLine(position + xRightDir * DistanceToSee,
                position + xRightDir * DistanceToSee - transform.up * heightToSee / 2);

            Handles.DrawLine(position, position + xLeftDir * DistanceToSee + transform.up * heightToSee / 2);
            Handles.DrawLine(position, position + xRightDir * DistanceToSee + transform.up * heightToSee / 2);

            Handles.DrawWireArc(position + transform.up * heightToSee / 2, transform.up, dir, horizontalAngleToSee / 2,
                DistanceToSee);
            Handles.DrawWireArc(position + transform.up * heightToSee / 2, transform.up, dir, -horizontalAngleToSee / 2,
                DistanceToSee);
        }
        
#endif

        #endregion
    }
}




