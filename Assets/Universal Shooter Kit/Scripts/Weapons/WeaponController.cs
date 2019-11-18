// GercStudio
// © 2018-2019

using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
// ReSharper disable All

namespace GercStudio.USK.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class WeaponController : MonoBehaviour
    {
        public Controller Controller;
        public AIController AIController;
        public InventoryManager WeaponManager;
        public WeaponController OriginalScript;

        public GameObject ScopeScreen;

        public Transform Parent;
        public Transform ColliderToCheckWalls;

        public WeaponsHelper.GrenadeParameters GrenadeParameters;
        public WeaponsHelper.IKObjects IkObjects;
        public CharacterHelper.BodyObjects BodyObjects;

        public List<WeaponsHelper.Attack> Attacks = new List<WeaponsHelper.Attack>() {new WeaponsHelper.Attack()};
        
        public List<WeaponsHelper.IKSlot> IkSlots = new List<WeaponsHelper.IKSlot>{new WeaponsHelper.IKSlot()};

        public WeaponsHelper.IkDebugMode DebugMode;

        public int currentAttack;
        
        [Range(20,1)]public float scopeDepth;
        [Range(20,1)]public float aimTextureDepth;

        public AudioClip DropWeaponAudio;
        public AudioClip PickUpWeaponAudio;

        public WeaponsHelper.WeaponAnimation characterAnimations;

        public Texture WeaponImage;
        public Texture AimCrosshairTexture;

        public List<WeaponsHelper.WeaponInfo> WeaponInfos = new List<WeaponsHelper.WeaponInfo>{new WeaponsHelper.WeaponInfo()};
        public List<WeaponsHelper.WeaponInfo> CurrentWeaponInfo;

        public Helper.Parent WeaponParent;

        public Vector3 Direction;
        
        public Inputs inputs;

        public RaycastHit Hit;

        //inspector variables
        public string currentTab;
        public List<string> enumNames = new List<string>{"Slot 1"};
        public List<string> attacksNames = new List<string>{"Bullet attack"};

        public int inspectorTabTop;
        public int bulletTypeInspectorTab;
        public int inspectorTabBottom;
        public int SettingsSlotIndex;
        public int lastSettingsSlotIndex;
        //
        
        public bool canUseValuesInAdjustment;
        public bool SwitchToFpCamera;
        public bool WasSetSwitchToFP;
        public bool UseScope;
        public bool UseAimTexture;
        public bool IsReloadEnabled;
        public bool AttackAudioPlay;
        public bool IsAimEnabled;
        public bool UIButtonAttack; // can attack from mobile inputs
        public bool ActiveAimTPS;
        public bool ActiveAimFPS;
        public bool canAttack;
        public bool TakeWeaponInAimMode = true;
        public bool TakeWeaponInWallMode = true;
        public bool TakeWeaponInCrouchlMode = true;

        //Check bools for IK
        public bool CanUseIK;
        public bool CanUseElbowIK;
        public bool CanUseWallIK;
        public bool CanUseCrouchIK;
        public bool CanUseAimIK;
        public bool hasAimIKChanged;
        public bool hasWallIKChanged;
        public bool hasCrouchIKChanged;
        public bool wallDetect;

        //Check bools for Photon synchronization
        public bool isMultiplayerWeapon;
        public bool MultiplayerReload;
        public bool MultiplayerAttack;
        public bool MultiplayerFire;
        public bool MultiplayerRocket;
        public bool MultiplayerRocketRaycast;
        public bool MultiplayerBulletHit;
        public bool MultiplayerAim;
        public bool MultiplayerChangeAttack;

        //Variables for Inspector
        public bool ActiveDebug;
        public bool PickUpWeapon;
        public Transform parent;
        //

        // temporary camera for processing the direction of shooting with top down view
        private Transform tempCamera;

        private bool hasAttackButtonPressed;
        private bool canGrenadeExplosion;
        private bool setColliderPosition;
        private bool aimWasSetBeforeAttack;
        private bool aimTimeout = true;
        public bool setCrouchHands;
        public bool pressAimButtonOnce;
        
        private float _rateOfShoot;
        private float disableAimAfterAttackTimeout;
        public float _scatter;
        

        private void OnEnable()
        {
            if (WeaponParent == Helper.Parent.Character && Controller && !Controller.AdjustmentScene)
            {
                WeaponsHelper.SetHandsSettingsSlot(ref SettingsSlotIndex, Controller.characterTag, Controller.TypeOfCamera, this);
            }
            else
            {
               //SettingsSlotIndex = tpsSettingsSlot; 
            }

            lastSettingsSlotIndex = SettingsSlotIndex;

            CurrentWeaponInfo.Clear();

            for (var i = 0; i < WeaponInfos.Count; i++)
            {
                var info = new WeaponsHelper.WeaponInfo();
                info.Clone(WeaponInfos[i]);
                CurrentWeaponInfo.Add(info);
            }

            if (Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
                return;
            
            WeaponsHelper.PlaceWeapon(CurrentWeaponInfo[SettingsSlotIndex], transform);

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            GetComponent<BoxCollider>().isTrigger = true;

            if (WeaponParent == Helper.Parent.Character)
            {
                BodyObjects = Controller.BodyObjects;
                if (!Controller)
                    return;
            }
            else
            {
                BodyObjects = AIController.BodyObjects;
                if (!AIController)
                    return;
            }

            foreach (var attack in Attacks)
            {
                if (attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets)
                {
                    if (bulletTypeInspectorTab == 0)
                    {
                        if(attack.BulletsSettings[0].Active)
                            attack.currentBulletType = 0;
                        else attack.currentBulletType = 1;
                    }
                    else if (bulletTypeInspectorTab == 1)
                    {
                        if (attack.BulletsSettings[1].Active)
                            attack.currentBulletType = 1;
                        else attack.currentBulletType = 0;
                    }
                }
            }
            
            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets)
                ChangeBulletType();

            if (!isMultiplayerWeapon)
            {
                tempCamera = new GameObject("tempCamera").transform;
                tempCamera.hideFlags = HideFlags.HideInHierarchy;

                _scatter = Attacks[currentAttack].ScatterOfBullets;

                if (WeaponParent == Helper.Parent.Character)
                    characterAnimations.anim = Controller.anim;
                else
                {
                    characterAnimations.anim = AIController.anim;
                }

                foreach (var attack in Attacks.Where(attack => attack.AttackCollider))
                {
                    attack.AttackCollider.enabled = false;
                }
            }

            CanUseIK = false;

            wallDetect = false;

            if(!IkObjects.RightObject)
                Helper.CreateObjects(IkObjects, transform, Controller.AdjustmentScene, true);

            WeaponsHelper.CheckIK(ref CanUseElbowIK, ref CanUseIK, ref CanUseAimIK, ref CanUseWallIK, ref CanUseCrouchIK, CurrentWeaponInfo[SettingsSlotIndex]);

            if (Controller.isCrouch && CanUseCrouchIK && Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                setCrouchHands = true;
            
            WeaponsHelper.PlaceAllIKObjects(this, CurrentWeaponInfo[SettingsSlotIndex], true);

        }

        void Start()
        {

            //currentAttack = 0;
            canUseValuesInAdjustment = true;

            if (UseAimTexture)
                SwitchToFpCamera = true;

            if (Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
            {
                SettingsSlotIndex = 0;
                return;
            }

            if (WeaponParent == Helper.Parent.Character)
            {
                if (!Controller.AdjustmentScene)
                    ActiveDebug = false;
            }
            else
            {
                if (!AIController.AdjustmentScene)
                    ActiveDebug = false;
            }


            Attacks[currentAttack].curAmmo = Attacks[currentAttack].maxAmmo;

            if (!ColliderToCheckWalls)
            {
                ColliderToCheckWalls = new GameObject("Collider to check").transform;
                

                ColliderToCheckWalls.parent = BodyObjects.TopBody;

                ColliderToCheckWalls.localPosition = CurrentWeaponInfo[SettingsSlotIndex].CheckWallsBoxPosition;
                ColliderToCheckWalls.localEulerAngles = CurrentWeaponInfo[SettingsSlotIndex].CheckWallsBoxRotation;
                
                if(CurrentWeaponInfo[SettingsSlotIndex].CheckWallsColliderSize != Vector3.zero)
                    ColliderToCheckWalls.localScale = CurrentWeaponInfo[SettingsSlotIndex].CheckWallsColliderSize;
                else ColliderToCheckWalls.localScale = Vector3.one;
            }
        }

        void Update()
        {
            if (lastSettingsSlotIndex != SettingsSlotIndex)
            {
                WeaponsHelper.SetWeaponPositions(this, true);
                lastSettingsSlotIndex = SettingsSlotIndex;
            }

            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade) return;
            
            _rateOfShoot += Time.deltaTime;
            disableAimAfterAttackTimeout += Time.deltaTime;

            if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && IsAimEnabled && disableAimAfterAttackTimeout > 5 &&
                aimWasSetBeforeAttack && !IsReloadEnabled)
            {
                Aim(true, false);
                aimWasSetBeforeAttack = false;
            }

//            if (WeaponManager.hasWeaponTaken && ColliderToCheckWalls.parent != Controller.BodyObjects.TopBody && !setColliderPosition)
//            {
//                ColliderToCheckWalls.parent = Controller.BodyObjects.TopBody;
//                setColliderPosition = true;
//            }

            if (WeaponParent == Helper.Parent.Character)
            {
                CharacterUpdate();
            }
            else
            {
                EnemyUpdate();
            }
        }
        
        
        void EnemyUpdate()
        {
            
        }

        void CharacterUpdate()
        {
            if (!WeaponManager)
                return;

            CheckWall();

            if (isMultiplayerWeapon || Controller && !Controller.ActiveCharacter)
                return;

            CheckButtons();
        }

        void PlayNoAmmoSound()
        {
            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Knife)
                return;

            if (Attacks[currentAttack].NoAmmoShotAudio)
                GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].NoAmmoShotAudio);
        }

        public void ChangeBulletType()
        {
            Attacks[currentAttack].weapon_damage = Attacks[currentAttack].BulletsSettings[Attacks[currentAttack].currentBulletType].weapon_damage;
            Attacks[currentAttack].RateOfShoot = Attacks[currentAttack].BulletsSettings[Attacks[currentAttack].currentBulletType].RateOfShoot;
            Attacks[currentAttack].ScatterOfBullets = Attacks[currentAttack].BulletsSettings[Attacks[currentAttack].currentBulletType].ScatterOfBullets;
            _scatter = Attacks[currentAttack].ScatterOfBullets;
        }

        public void ChangeAttack()
        {
            if (!isMultiplayerWeapon)
            {
                if (IsReloadEnabled || WeaponManager.creategrenade || WeaponManager.isPickUp || Controller.isPause)
                    return;

                MultiplayerChangeAttack = true;
            }

            var newAttack = 0;
            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].BulletsSettings[0].Active &&  Attacks[currentAttack].BulletsSettings[1].Active)
            {
                if (Attacks[currentAttack].currentBulletType == 0)
                {
                    Attacks[currentAttack].currentBulletType++;
                    ChangeBulletType();
                    WeaponManager.SetWeaponAnimations(true);
                    return;
                }
            }
            
            newAttack = currentAttack + 1;
            if (newAttack > Attacks.Count - 1)
                newAttack = 0;

            currentAttack = newAttack;
            
            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].BulletsSettings[0].Active && Attacks[currentAttack].BulletsSettings[1].Active)
            {
                if (Attacks[currentAttack].currentBulletType == 1)
                {
                    Attacks[currentAttack].currentBulletType--;
                    ChangeBulletType();
                }
                ChangeBulletType();
            }
            
            WeaponManager.SetWeaponAnimations(true);
        }
        
        void CheckButtons()
        {
            if (Input.GetKeyDown(Controller._gamepadCodes[5]) || Input.GetKeyDown(Controller._keyboardCodes[5]) ||
                Helper.CheckGamepadAxisButton(5, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed,
                    "GetKeyDown", Controller.inputs.AxisButtonValues[5]))    
                Aim(false,false);


                if (Input.GetKeyDown(Controller._gamepadCodes[17]) || Input.GetKeyDown(Controller._keyboardCodes[19]) ||
                Helper.CheckGamepadAxisButton(17, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed,
                    "GetKeyDown", Controller.inputs.AxisButtonValues[17]))
                ChangeAttack();
            

            if (Input.GetKeyDown(Controller._gamepadCodes[3]) || Input.GetKeyDown(Controller._keyboardCodes[3]) ||
                UIButtonAttack || Helper.CheckGamepadAxisButton(3, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, 
                    "GetKeyDown", Controller.inputs.AxisButtonValues[3]))
            {
                Attack(true, "Single",false);
            }
            else
            {
                Attack(false, "Single",false);
            }

            if (Input.GetKey(Controller._gamepadCodes[3]) || Input.GetKey(Controller._keyboardCodes[3]) ||
                UIButtonAttack || Helper.CheckGamepadAxisButton(3, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed,"GetKey",
                    Controller.inputs.AxisButtonValues[3]))
            {
                Attack(true, "Auto",false);
            }
            else
            {
                Attack(false, "Auto",false);
            }
            

            if (Input.GetKeyDown(Controller._gamepadCodes[4]) || Input.GetKeyDown(Controller._keyboardCodes[4]) ||
                Helper.CheckGamepadAxisButton(4, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown",
                    Controller.inputs.AxisButtonValues[4]))
                Reload();
        }

        void CheckWall()
        {
            if (!CanUseWallIK)
                return;

            var hitColliders =
                Physics.OverlapBox(ColliderToCheckWalls.position, ColliderToCheckWalls.localScale / 2, ColliderToCheckWalls.rotation, Helper.layerMask());

            if (wallDetect)
            {
                var wall = false;

                foreach (var col in hitColliders)
                {
                    if (!col) continue;
                    
                    if (!col.transform.root.GetComponent<Controller>())
                        wall = true;
                }

                if (wall) return;

                wallDetect = false;
                TakeWeaponInWallMode = false;

                Controller.thisCameraScript.LayerCamera.SetActive(false);
                
                characterAnimations.anim.SetBool("CanWalkWithWeapon", false);
                if (!characterAnimations.anim.GetCurrentAnimatorStateInfo(2).IsName("Take Weapon") &&
                    !characterAnimations.anim.GetCurrentAnimatorStateInfo(3).IsName("Take Weapon"))
                {
                    characterAnimations.anim.CrossFade("Idle", 0, 2);
                    characterAnimations.anim.CrossFade("Idle", 0, 3);
                }

                StartCoroutine("WalkWithWeaponTimeout");

                if (IkObjects.RightObject)
                    WeaponsHelper.SmoothPositionChange(IkObjects.RightObject, CurrentWeaponInfo[SettingsSlotIndex].RightHandWallPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].RightHandWallRotation, BodyObjects.TopBody);

                if (IkObjects.LeftObject)
                    WeaponsHelper.SmoothPositionChange(IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftHandWallPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].LeftHandWallRotation, BodyObjects.TopBody);

                StartCoroutine("StartSetWallHandsPosition");
                StartCoroutine("StopSetWallHandPosition");

            }
            else
            {
                if (hitColliders.Any(collider => !collider.transform.root.GetComponent<Controller>()))
                {
                    if (!isMultiplayerWeapon)
                        Controller.thisCameraScript.LayerCamera.SetActive(true);

                    ColliderToCheckWalls.parent = BodyObjects.TopBody;
                    wallDetect = true;
                    TakeWeaponInWallMode = false;
                    
                    if (IkObjects.RightObject)
                        WeaponsHelper.SmoothPositionChange(IkObjects.RightObject, CurrentWeaponInfo[SettingsSlotIndex].RightHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].RightHandRotation, BodyObjects.TopBody);

                    if (IkObjects.LeftObject)
                        WeaponsHelper.SmoothPositionChange(IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].LeftHandRotation, BodyObjects.TopBody);

                    StartCoroutine("StartSetWallHandsPosition");
                    StartCoroutine("StopSetWallHandPosition");
                }
            }
        }

        public void SwitchAttack(string type)
        {
            switch (type)
            {
                case "Single":
                {
                    switch (Attacks[currentAttack].AttackType)
                    {
                        case WeaponsHelper.TypeOfAttack.Bullets:
                            if(Attacks[currentAttack].currentBulletType == 0)
                            BulletAttack();
                            return;
                        case WeaponsHelper.TypeOfAttack.Rockets:
                        case WeaponsHelper.TypeOfAttack.GrenadeLauncher:
                            RocketAttack();
                            return;
                        case WeaponsHelper.TypeOfAttack.Knife:
                            KnifeAttack();
                            break;
                    }

                    break;
                }

                case "Auto":
                {
                    switch (Attacks[currentAttack].AttackType)
                    {
                        case WeaponsHelper.TypeOfAttack.Flame:
                            FireAttack();
                            return;
                        case WeaponsHelper.TypeOfAttack.Bullets:
                            if(Attacks[currentAttack].currentBulletType == 1)
                               BulletAttack();
                            break;
                    }

                    break;
                }
            }
        }

        IEnumerator SetAimBeforeAttackTimeout(string type)
        {
            yield return new WaitForSeconds(0.7f);
            SwitchAttack(type);
            StopCoroutine("SetAimBeforeAttackTimeout");
        }

        public void Attack(bool isAttack, string type, bool enemy)
        {
//            if (enemy)
//            {
//                if (isAttack && curAmmo > 0 && !IsReloadEnabled && canAttack)
//                {
//                    switch (type)
//                    {
//                        case "Single":
//                        {
//                            switch (WeaponType)
//                            {
//                                case WeaponsHelper.TypeOfWeapon.SingleShots:
//                                    BulletAttack();
//                                    return;
//                                case WeaponsHelper.TypeOfWeapon.RocketLauncher:
//                                    RocketAttack();
//                                    return;
//                                case WeaponsHelper.TypeOfWeapon.Knife:
//                                    KnifeAttack();
//                                    break;
//                            }
//
//                            break;
//                        }
//
//                        case "Auto":
//                        {
//                            switch (WeaponType)
//                            {
//                                case WeaponsHelper.TypeOfWeapon.Flamethrower:
//                                    FireAttack();
//                                    return;
//                                case WeaponsHelper.TypeOfWeapon.AutomaticShots:
//                                    BulletAttack();
//                                    break;
//                            }
//
//                            break;
//                        }
//                    }
//                }
//                else
//                {
//                    switch (type)
//                    {
//                        case "Single":
//                        {
//                            if (WeaponType == WeaponsHelper.TypeOfWeapon.Knife || WeaponType == WeaponsHelper.TypeOfWeapon.RocketLauncher ||
//                                WeaponType == WeaponsHelper.TypeOfWeapon.SingleShots)
//                            {
//                                if (WeaponType == WeaponsHelper.TypeOfWeapon.Knife)
//                                {
//                                    if (_rateofShoot > characterAnimations.WeaponAttack.length)
//                                        if (AttackCollider)
//                                            AttackCollider.enabled = false;
//                                }
//
//                                if (_rateofShoot >= characterAnimations.WeaponAttack.length / 2 || IsReloadEnabled)
//                                {
//                                    characterAnimations.anim.SetBool("Attack", false);
//                                }
//                            }
//
//                            break;
//                        }
//
//                        case "Auto":
//                        {
//                            if (WeaponType == WeaponsHelper.TypeOfWeapon.Flamethrower)
//                            {
//                                FireAttackOff();
//                                characterAnimations.anim.SetBool("Attack", false);
//                            }
//                            else if (WeaponType == WeaponsHelper.TypeOfWeapon.AutomaticShots)
//                                characterAnimations.anim.SetBool("Attack", false);
//
//                            break;
//                        }
//                    }
//                }
//
//
//            }
//            else
//            {

                if (isAttack & !WeaponManager.creategrenade && !IsReloadEnabled && !Controller.isPause && canAttack && !WeaponManager.isPickUp)
                {
                    disableAimAfterAttackTimeout = 0;

                    if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !IsAimEnabled && ActiveAimTPS && !wallDetect)
                    {
                        Aim(false, false);
                        aimWasSetBeforeAttack = true;
                        StartCoroutine(SetAimBeforeAttackTimeout(type));
                    }
                    else
                    {
                        SwitchAttack(type);
                    }
                }
                else
                {
                    switch (type)
                    {
                        case "Single":
                        {
                            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Knife || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Rockets ||
                                Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.GrenadeLauncher && Attacks[currentAttack].currentBulletType == 0)
                            {
                                if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Knife)
                                {
                                    if (_rateOfShoot > Attacks[currentAttack].WeaponAttack.length)
                                        if (Attacks[currentAttack].AttackCollider)
                                            Attacks[currentAttack].AttackCollider.enabled = false;
                                }
                                
                                if (Attacks[currentAttack].WeaponAttack &&_rateOfShoot >= Attacks[currentAttack].WeaponAttack.length / 2 || IsReloadEnabled)
                                {
                                    characterAnimations.anim.SetBool("Attack", false);
                                }
                            }

                            break;
                        }

                        case "Auto":
                        {
                            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Flame)
                            {
                                FireAttackOff();
                                characterAnimations.anim.SetBool("Attack", false);
                            }
                            else if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].currentBulletType == 1)
                                characterAnimations.anim.SetBool("Attack", false);

                            break;
                        }
                    }
                    
                }
//            }
        }

        #region BulletAttack

        private void BulletAttack()
        {
            if (_rateOfShoot > Attacks[currentAttack].RateOfShoot)
            {
                if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].currentBulletType == 0)
                    UIButtonAttack = false;

                _rateOfShoot = 0;

                if (Attacks[currentAttack].curAmmo <= 0)
                {
                    characterAnimations.anim.SetBool("Attack", false);
                    PlayNoAmmoSound();
                    return;
                }
                
                if (Attacks[currentAttack].AttackAudio)
                    GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].AttackAudio);
                else
                    Debug.LogWarning("(Weapon) <color=yellow>Missing component</color> [Attack Audio]. Add it, otherwise the sound of shooting won't be played.", gameObject);

                characterAnimations.anim.SetBool("Attack", true);

                MultiplayerAttack = true;

                if (Attacks[currentAttack].AttackSpawnPoint)
                {
                    Attacks[currentAttack].curAmmo -= 1;

                    if (Attacks[currentAttack].MuzzleFlash)
                    {
                        var Flash = Instantiate(Attacks[currentAttack].MuzzleFlash, Attacks[currentAttack].AttackSpawnPoint.position, Attacks[currentAttack].AttackSpawnPoint.rotation);
                        Flash.transform.parent = gameObject.transform;
                        Helper.ChangeLayersRecursively(Flash.transform, "Character");

                        Flash.gameObject.AddComponent<DestroyObject>().destroy_time = 0.17f;
                    }
                    else
                    {
                        Debug.LogWarning(
                            "(Weapon) <color=yellow>Missing component</color> [MuzzleFlash]. Add it, otherwise muzzle flash won't be displayed.",
                            gameObject);
                    }

                    if (Attacks[currentAttack].Shell & Attacks[currentAttack].ShellPoint)
                    {
                        var _shell = Instantiate(Attacks[currentAttack].Shell, Attacks[currentAttack].ShellPoint.position, Attacks[currentAttack].ShellPoint.localRotation);
                        Helper.ChangeLayersRecursively(_shell.transform, "Character");

//                        _shell.gameObject.AddComponent<ShellControll>().weapon = transform;
                        _shell.gameObject.AddComponent<ShellControll>().ShellPoint = Attacks[currentAttack].ShellPoint;

                    }
                    else
                    {
                        Debug.LogWarning(
                            "(Weapon) <color=yellow>Missing component</color> [Shell]. Add it, otherwise shells won't be created.",
                            gameObject);
                    }
                    

                    if (WeaponParent == Helper.Parent.Character)
                    {
                        if (UpdateAttackDirection())
                        {
                            MultiplayerBulletHit = true;
                            var HitRotation = Quaternion.FromToRotation(Vector3.up, Hit.normal);
                            
                            if (Attacks[currentAttack].Tracer)
                            {
                                var _Tracer = Instantiate(Attacks[currentAttack].Tracer, Attacks[currentAttack].AttackSpawnPoint.position, Attacks[currentAttack].AttackSpawnPoint.rotation);
                                _Tracer.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                                var tracerScript = _Tracer.gameObject.AddComponent<Rocket>();
                                tracerScript.isTracer = true;
                                tracerScript.TargetPoint = Hit.point;
                                tracerScript.Speed = 200;
                            }
                            else
                                Debug.LogWarning(
                                    "(Weapon) <color=yellow>Missing component</color> [Tracer]. Add it, otherwise bullet tracer won't be displayed.",
                                    gameObject);

                            if (Hit.collider.GetComponent<EnemyHealth>())
                            {
                                Hit.collider.GetComponent<EnemyHealth>().Enemy_health -= Attacks[currentAttack].weapon_damage;
                            }

                            if (Hit.collider.GetComponent<Controller>())
                            {
                                Hit.collider.GetComponent<Controller>().Damage(Attacks[currentAttack].weapon_damage, Controller.CharacterName);
                            }

                            if (Hit.collider.GetComponent<WeaponController>())
                            {
                                if (Hit.collider.GetComponent<WeaponController>().Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
                                {
                                    Hit.collider.GetComponent<WeaponController>().Explosion();
                                }
                            }

                            if (Hit.transform.GetComponent<Rigidbody>())
                            {
                                Hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(Direction * 800, Hit.point);
                            }

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
                                            AudioSource _audio = hitGO.gameObject.AddComponent<AudioSource>();
                                            _audio.clip = surface.HitAudio;
                                            _audio.spatialBlend = 1;
                                            _audio.minDistance = 10;
                                            _audio.maxDistance = 100;
                                            _audio.PlayOneShot(hitGO.gameObject.GetComponent<AudioSource>().clip);
                                        }
                                        else
                                            Debug.LogWarning(
                                                "(Surface) <color=yellow>Missing component</color> [HitAudio]. Add it, otherwise the sound of hit won't be played.");

                                        hitGO.parent = Hit.transform;
                                    }
                                    else
                                        Debug.LogWarning(
                                            "(Surface) <color=yellow>Missing components</color>: [Sparks] and/or [Hit]. Add them, otherwise the rocket launcher won't shoot.");
                                }
                                else
                                    Debug.LogError(
                                        "(Surface) <color=yellow>Missing Component</color>: [Material]. Add it to initialize the surface.",
                                        surface.gameObject);
                            }
                        }
                        else
                        {
                            MultiplayerBulletHit = false;
                        }
                    }
                }
                else
                {
                    Debug.LogError(
                        "(Weapon) <color=red>Missing component</color> [AttackSpawnPoint]. Add it, otherwise hit effects won't be played",
                        gameObject);
                    Debug.Break();
                }
            }
            else
            {
                if (Attacks[currentAttack].RateOfShoot >= 1)
                    characterAnimations.anim.SetBool("Attack", false);
            }
        }

        #endregion

        #region RocketsAttack

        public void RocketAttack()
        {
            if (_rateOfShoot > Attacks[currentAttack].RateOfShoot & IsReloadEnabled == false)
            {
                UIButtonAttack = false;
                
                if (Attacks[currentAttack].curAmmo <= 0)
                {
                    PlayNoAmmoSound();
                    return;
                }
                
                if (Attacks[currentAttack].AttackAudio)
                    GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].AttackAudio);
                else
                    Debug.LogWarning(
                        "(Weapon) <color=yellow>Missing component</color> [AttackAudio]. Add it, otherwise the sound of shooting won't be played.",
                        gameObject);

                characterAnimations.anim.SetBool("Attack", true);

                if (Attacks[currentAttack].AttackSpawnPoint & Attacks[currentAttack].Rocket)
                {
                    Attacks[currentAttack].curAmmo -= 1;
                    var rocket = Instantiate(Attacks[currentAttack].Rocket, Attacks[currentAttack].AttackSpawnPoint.position, Attacks[currentAttack].AttackSpawnPoint.rotation);

                    if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Rockets)
                    {
                        if (UpdateAttackDirection())
                        {
                            MultiplayerRocket = true;
                            MultiplayerRocketRaycast = true;
                            rocket.GetComponent<Rocket>().damage = Attacks[currentAttack].weapon_damage;
                            rocket.GetComponent<Rocket>().Camera = Controller.thisCamera.transform;
                            rocket.GetComponent<Rocket>().isRaycast = MultiplayerRocketRaycast;
                            rocket.GetComponent<Rocket>().TargetPoint = Hit.point;
                            if (Controller.CharacterName != null)
                                rocket.GetComponent<Rocket>().OwnerName = Controller.CharacterName;
                        }
                        else
                        {
                            MultiplayerRocketRaycast = false;
                            rocket.GetComponent<Rocket>().Camera = Controller.thisCamera.transform;
                            rocket.GetComponent<Rocket>().isRaycast = MultiplayerRocketRaycast;
                            if (Controller.CharacterName != null)
                                rocket.GetComponent<Rocket>().OwnerName = Controller.CharacterName;
                        }
                    }
                    else
                    {
                        var grenadeScript = rocket.GetComponent<WeaponController>();
                        grenadeScript.Controller = Controller;
                        grenadeScript.enabled = true;

                        var rigidBody = rocket.GetComponent<Rigidbody>();

                        rigidBody.useGravity = true;
                        rigidBody.isKinematic = false;
                        
                        if (Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
                            rigidBody.velocity = Controller.thisCamera.transform.TransformDirection(Vector3.forward * grenadeScript.GrenadeParameters.GrenadeSpeed);
                        else
                            rigidBody.velocity = Controller.thisCamera.transform.TransformDirection(Vector3.up * grenadeScript.GrenadeParameters.GrenadeSpeed);

                        grenadeScript.StartCoroutine("GrenadeFlying");
                    }
                }
                else
                {
                    Debug.LogError("(Weapon) <color=red>Missing components</color>: [AttackSpawnPoint] and/or [RocketPrefab]. Add them, otherwise the rocket launcher won't shoot.", gameObject);
                    Debug.Break();
                }

                _rateOfShoot = 0;
            }
        }

        #endregion

        #region FireAttack

        public void FireAttack()
        {
            if (!IsReloadEnabled)
            {
                if (Attacks[currentAttack].curAmmo <= 0 && !AttackAudioPlay)
                {
                    PlayNoAmmoSound();
                    AttackAudioPlay = true;
                    return;
                }

                characterAnimations.anim.SetBool("Attack", true);
                
                if (!AttackAudioPlay)
                {
                    if (Attacks[currentAttack].AttackAudio)
                    {
                        GetComponent<AudioSource>().clip = Attacks[currentAttack].AttackAudio;
                        GetComponent<AudioSource>().Play();
                        AttackAudioPlay = true;
                    }
                    else
                    {
                        Debug.LogWarning(
                            "(Weapon) <color=yellow>Missing component</color> [AttackAudio]. Add it, otherwise the sound of shooting won't be played.",
                            gameObject);
                    }
                }

                if (Attacks[currentAttack].AttackSpawnPoint & Attacks[currentAttack].Fire)
                {
                    Attacks[currentAttack].curAmmo -= 1 * Time.deltaTime;
                    MultiplayerFire = true;
                    Instantiate(Attacks[currentAttack].Fire, Attacks[currentAttack].AttackSpawnPoint.position, Attacks[currentAttack].AttackSpawnPoint.rotation);
                }
                else
                {
                    Debug.LogError(
                        "(Weapon) <color=red>Missing components</color>: [AttackSpawnPoint] and/or [Fire_prefab]. Add it, otherwise the flamethrower won't attack.",
                        gameObject);
                    Debug.Break();
                }

                if (Attacks[currentAttack].AttackCollider)
                    Attacks[currentAttack].AttackCollider.enabled = true;
                else
                {
                    Debug.LogError(
                        "(Weapon) <color=red>Missing components</color>: [FireCollider]. Add it, otherwise the flamethrower won't attack.",
                        gameObject);
                    Debug.Break();
                }
            }
        }

        public void FireAttackOff()
        {
            if (Attacks[currentAttack].AttackAudio)
                if (AttackAudioPlay)
                {
                    AttackAudioPlay = false;
                    GetComponent<AudioSource>().Stop();
                }

            if (Attacks[currentAttack].AttackCollider)
                Attacks[currentAttack].AttackCollider.enabled = false;
        }

        #endregion

        #region KnifeAttack

        public void KnifeAttack()
        {
            if (_rateOfShoot > Attacks[currentAttack].RateOfShoot)
            {
                UIButtonAttack = false;
                if (Attacks[currentAttack].AttackCollider)
                    Attacks[currentAttack].AttackCollider.enabled = true;
                else
                {
                    Debug.LogError(
                        "(Weapon) <color=red>Missing component</color> [knifeCollider]. Add it, otherwise the knife won't attack.",
                        gameObject);
                    Debug.Break();
                }

                MultiplayerAttack = true;
                _rateOfShoot = 0;
                characterAnimations.anim.SetBool("Attack", true);

                if (Attacks[currentAttack].AttackAudio)
                    GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].AttackAudio);
                else
                    Debug.LogWarning(
                        "(Weapon) <color=yellow>Missing component</color> [AttackAudio]. Add it, otherwise the sound of shooting won't be played.",
                        gameObject);
            }
        }

        #endregion

        public void Reload()
        {
            var pause = false;

            if (!isMultiplayerWeapon)
            {
                MultiplayerReload = true;
                
                if (WeaponParent == Helper.Parent.Character)
                    pause = Controller.isPause ? true : false;
            }
            
            if (Attacks[currentAttack].inventoryAmmo > 0 && Attacks[currentAttack].curAmmo != Attacks[currentAttack].maxAmmo && !pause && !wallDetect || isMultiplayerWeapon)
            {
                if (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && IsAimEnabled)
                {
                    if(!isMultiplayerWeapon)
                        Aim(true, true);
                    
                    StartCoroutine("ReloadTimeout");
                }
                else
                {
                    IsReloadEnabled = true;
                    characterAnimations.anim.SetBool("Reload", true);
                    PlayReloadAudio();
                    StartCoroutine(DisableAnimation());
                    StartCoroutine(ReloadProcess());
                }

            }
        }
        
        void PlayReloadAudio()
        {
            if (Attacks[currentAttack].ReloadAudio)
                GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].ReloadAudio);
            else
            {
                Debug.LogWarning("(Weapon) <color=yellow>Missing component</color> [ReloadAudio]. Add it, otherwise the sound of reloading won't be played.", gameObject);
            }

            StopCoroutine("PlayReloadAudio");
        }

        public void CrouchHands()
        {
            if (!setCrouchHands && TakeWeaponInCrouchlMode && CanUseCrouchIK && !IsAimEnabled && !wallDetect)
            {
                TakeWeaponInCrouchlMode = false;
                setCrouchHands = true;
                
                WeaponsHelper.ChangeIKPosition(CurrentWeaponInfo[SettingsSlotIndex].LeftHandPosition, CurrentWeaponInfo[SettingsSlotIndex].RightHandPosition,
                    CurrentWeaponInfo[SettingsSlotIndex].LeftHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightHandRotation, this);
                
                StartCoroutine("StartSetCrouchHandsPosition");
                StartCoroutine("StopSetCrouchHandPosition");
                
            }
            else if(setCrouchHands && TakeWeaponInCrouchlMode && CanUseCrouchIK && !wallDetect && !IsAimEnabled)
            {
                TakeWeaponInCrouchlMode = false;
                setCrouchHands = false;
                
                WeaponsHelper.ChangeIKPosition(CurrentWeaponInfo[SettingsSlotIndex].LeftCrouchHandPosition, CurrentWeaponInfo[SettingsSlotIndex].RightCrouchHandPosition,
                    CurrentWeaponInfo[SettingsSlotIndex].LeftCrouchHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightCrouchHandRotation, this);
                
                StartCoroutine("StartSetCrouchHandsPosition");
                StartCoroutine("StopSetCrouchHandPosition");
            }
        }

        public void Aim(bool instantly, bool notSendToMultiplayer)
        {

            if (!isMultiplayerWeapon)
            {
                if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !ActiveAimTPS ||
                    Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && !ActiveAimFPS
                    || IsReloadEnabled || wallDetect || Controller.changeCameraTypeTimeout < 0.5f || Controller.anim.GetBool("Grenade") || !aimTimeout)
                    return;
            }
            
            if (CanUseAimIK)
            {
                var canAim = false;
                if (WeaponParent == Helper.Parent.Character && !isMultiplayerWeapon)
                {
                    if (!WeaponManager.creategrenade && Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown && !Controller.isPause 
                        && !Controller.thisCameraScript.Occlusion)
                        canAim = true;
                }
                else
                {
                    canAim = true;
                }
                
                
                if (!canAim) return;
                
                if (!IsAimEnabled & TakeWeaponInAimMode)
                {
                    if (WeaponParent == Helper.Parent.Character && !isMultiplayerWeapon)
                    {
                        Controller.thisCameraScript.Aim();

                        _scatter = Attacks[currentAttack].ScatterOfBullets / 2;
                        MultiplayerAim = true;
                        aimTimeout = false;
                    }

                    IsAimEnabled = true;
                    TakeWeaponInAimMode = false;
                    
                    characterAnimations.anim.SetBool("CanWalkWithWeapon", false);
                    if (!characterAnimations.anim.GetCurrentAnimatorStateInfo(2).IsName("Take Weapon") &&
                        !characterAnimations.anim.GetCurrentAnimatorStateInfo(3).IsName("Take Weapon"))
                    {
                        characterAnimations.anim.CrossFade("Idle", 0, 2);
                        characterAnimations.anim.CrossFade("Idle", 0, 3);
                    }

                    StartCoroutine("WalkWithWeaponTimeout");

                    WeaponsHelper.ChangeIKPosition(CurrentWeaponInfo[SettingsSlotIndex].LeftHandPosition, CurrentWeaponInfo[SettingsSlotIndex].RightHandPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].LeftHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightHandRotation, this);
                    
                    StartCoroutine("StartSetAimHandsPosition");
                    StartCoroutine("StopSetAimHandPosition");
                    StartCoroutine("AimTimeout");
                }
                else if (IsAimEnabled & TakeWeaponInAimMode)
                {
                    if (SwitchToFpCamera && Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !instantly && !isMultiplayerWeapon)
                    {
                        Controller.thisCameraScript.DeepAim();
                        return;
                    }

                    if (WeaponParent == Helper.Parent.Character && !isMultiplayerWeapon)
                    {
                        Controller.thisCameraScript.Aim();
                        _scatter = Attacks[currentAttack].ScatterOfBullets;
                        aimTimeout = false;
                        
                        if(!notSendToMultiplayer)
                            MultiplayerAim = true;
                    }

                    IsAimEnabled = false;
                    aimWasSetBeforeAttack = false;
                    TakeWeaponInAimMode = false;
                    
                    characterAnimations.anim.SetBool("CanWalkWithWeapon", false);
                    if (!characterAnimations.anim.GetCurrentAnimatorStateInfo(2).IsName("Take Weapon") &&
                        !characterAnimations.anim.GetCurrentAnimatorStateInfo(3).IsName("Take Weapon"))
                    {
                        characterAnimations.anim.CrossFade("Idle", 0, 2);
                        characterAnimations.anim.CrossFade("Idle", 0, 3);
                    }

                    StartCoroutine("WalkWithWeaponTimeout");

                    WeaponsHelper.ChangeIKPosition(CurrentWeaponInfo[SettingsSlotIndex].LeftAimPosition, CurrentWeaponInfo[SettingsSlotIndex].RightAimPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].LeftAimRotation, CurrentWeaponInfo[SettingsSlotIndex].RightAimRotation, this);
                    
                    StartCoroutine("StartSetAimHandsPosition");
                    StartCoroutine("StopSetAimHandPosition");
                    StartCoroutine("AimTimeout");
                }
            }
            else
            {
                Debug.LogWarning("You haven't set the position of your hands for aiming. Please click [Adjust weapon position & hands IK] and adjust the hands.",
                    gameObject);
            }
        }

        bool UpdateAttackDirection()
        {
            if (!wallDetect)
            {
                if (Mathf.Abs(Controller.anim.GetFloat("CameraAngle")) < 60)
                {
                    if (Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
                    {

                        Direction = Controller.thisCamera.transform.TransformDirection(
                            Vector3.forward + new Vector3(Random.Range(-_scatter, _scatter), Random.Range(-_scatter, _scatter), 0));

                        if (Physics.Raycast(Controller.thisCamera.transform.position, Direction, out Hit, 10000f,
                            Helper.layerMask()))
                            return true;
                    }
                    else
                    {
                        tempCamera.position = Controller.thisCamera.transform.position;
                        tempCamera.rotation = Quaternion.Euler(90, Controller.thisCamera.transform.eulerAngles.y, Controller.thisCamera.transform.eulerAngles.z);
                        Direction = tempCamera.TransformDirection(
                            Vector3.up + new Vector3(Random.Range(-_scatter, _scatter), Random.Range(-_scatter, _scatter), 0));

                        if (Physics.Raycast(Attacks[currentAttack].AttackSpawnPoint.position, Direction, out Hit, 10000f, Helper.layerMask()))
                            return true;
                    }
                }
                else
                {
                    Direction = Attacks[currentAttack].AttackSpawnPoint.transform.TransformDirection(
                        Attacks[currentAttack].AttackSpawnPoint.forward + new Vector3(Random.Range(-_scatter, _scatter), Random.Range(-_scatter, _scatter), 0));
                    
                    if (Physics.Raycast(Attacks[currentAttack].AttackSpawnPoint.transform.position, Direction, out Hit, 10000f, Helper.layerMask()))
                        return true;
                }
            }
            else
            {
                Direction = Attacks[currentAttack].AttackSpawnPoint.transform.TransformDirection(
                    Attacks[currentAttack].AttackSpawnPoint.forward + new Vector3(Random.Range(-_scatter, _scatter), Random.Range(-_scatter, _scatter), 0));
                

                if (Physics.Raycast(Attacks[currentAttack].AttackSpawnPoint.transform.position, Direction, out Hit, 10000f, Helper.layerMask()))
                    return true;
            }

            return false;
        }
        
        IEnumerator DisableAnimation()
        {
            yield return new WaitForSeconds(Attacks[currentAttack].WeaponReload.length / 2);

            characterAnimations.anim.SetBool("CanWalkWithWeapon", false);
            characterAnimations.anim.SetBool("Reload", false);
            StopCoroutine("DisableAnimation");
        }

        IEnumerator ReloadTimeout()
        {
            yield return new WaitForSeconds(0.5f);
            
            IsReloadEnabled = true;
            characterAnimations.anim.SetBool("Reload", true);
            PlayReloadAudio();
            StartCoroutine(DisableAnimation());
            StartCoroutine(ReloadProcess());

            StopCoroutine("ReloadTimeout");
        }

        IEnumerator ReloadProcess()
        {
            yield return new WaitForSeconds(Attacks[currentAttack].WeaponReload.length);
            
            IsReloadEnabled = false;

            if (Attacks[currentAttack].inventoryAmmo < Attacks[currentAttack].maxAmmo - Attacks[currentAttack].curAmmo)
            {
                Attacks[currentAttack].curAmmo += Attacks[currentAttack].inventoryAmmo;
                Attacks[currentAttack].inventoryAmmo = 0;
            }
            else
            {
                Attacks[currentAttack].inventoryAmmo -= Attacks[currentAttack].maxAmmo - Attacks[currentAttack].curAmmo;
                Attacks[currentAttack].curAmmo += Attacks[currentAttack].maxAmmo - Attacks[currentAttack].curAmmo;
            }

            AttackAudioPlay = false;
            
            StartCoroutine("WalkWithWeaponTimeout");
            StopCoroutine("ReloadProcess");
        }

        IEnumerator StartSetAimHandsPosition()
        {
            while (true)
            {
                if (!TakeWeaponInAimMode && CanUseIK && IsAimEnabled)
                {
                    WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftAimPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].LeftAimRotation, CurrentWeaponInfo[SettingsSlotIndex].RightAimPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].RightAimRotation, "aim");
                }
                else if (!TakeWeaponInAimMode && CanUseIK && !IsAimEnabled && (Controller.isCrouch && !CurrentWeaponInfo[SettingsSlotIndex].disableIkInCrouchState ||
                                                                               !Controller.isCrouch && !CurrentWeaponInfo[SettingsSlotIndex].disableIkInNormalState))
                {
                    if (!Controller.isCrouch || Controller.isCrouch && Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
                    {
                        WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].LeftHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].RightHandRotation, "aim");
                    }
                    else if(Controller.isCrouch && Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                    {
                        WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftCrouchHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].LeftCrouchHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightCrouchHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].RightCrouchHandRotation, "aim");
                    }
                }

                yield return 0;
            }
        }
        
        IEnumerator StartSetCrouchHandsPosition()
        {
            while (true)
            {
                
                if (!TakeWeaponInCrouchlMode && CanUseCrouchIK && setCrouchHands)
                {
                    WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftCrouchHandPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].LeftCrouchHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightCrouchHandPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].RightCrouchHandRotation, "crouch");
                }
                else if (!TakeWeaponInCrouchlMode && CanUseCrouchIK && !setCrouchHands && (Controller.isCrouch && !CurrentWeaponInfo[SettingsSlotIndex].disableIkInCrouchState ||
                                                                                           !Controller.isCrouch && !CurrentWeaponInfo[SettingsSlotIndex].disableIkInNormalState))
                {
//                    if (!IsAimEnabled)
//                    {
                        WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].LeftHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].RightHandRotation, "crouch");
//                    }
//                    else
//                    {
//                        WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftAimPosition,
//                            CurrentWeaponInfo[SettingsSlotIndex].LeftAimRotation, CurrentWeaponInfo[SettingsSlotIndex].RightAimPosition,
//                            CurrentWeaponInfo[SettingsSlotIndex].RightAimRotation, "aim");
//                    }
                }

                yield return 0;
            }
        }

        IEnumerator StartSetWallHandsPosition()
        {
            while (true)
            {
                if (!TakeWeaponInWallMode && CanUseIK && wallDetect)
                {
                    WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftHandWallPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].LeftHandWallRotation, CurrentWeaponInfo[SettingsSlotIndex].RightHandWallPosition,
                        CurrentWeaponInfo[SettingsSlotIndex].RightHandWallRotation, "wall");
                }
                else if (!TakeWeaponInWallMode && CanUseIK && !wallDetect && (Controller.isCrouch && !CurrentWeaponInfo[SettingsSlotIndex].disableIkInCrouchState ||
                                                                              !Controller.isCrouch && !CurrentWeaponInfo[SettingsSlotIndex].disableIkInNormalState))
                {
                    if (!IsAimEnabled && !Controller.isCrouch)
                    {
                        WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].LeftHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].RightHandRotation, "wall");
                    }
                    else if(IsAimEnabled)
                    {
                        WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftAimPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].LeftAimRotation, CurrentWeaponInfo[SettingsSlotIndex].RightAimPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].RightAimRotation, "wall");
                    }
                    else if(Controller.isCrouch && Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                    {
                        WeaponsHelper.MovingIKObject(IkObjects.RightObject, IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftCrouchHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].LeftCrouchHandRotation, CurrentWeaponInfo[SettingsSlotIndex].RightCrouchHandPosition,
                            CurrentWeaponInfo[SettingsSlotIndex].RightCrouchHandRotation, "aim");
                    }
                }

                yield return 0;
            }
        }

        IEnumerator StopSetAimHandPosition()
        {
            yield return new WaitForSeconds(0.5f);
            TakeWeaponInAimMode = true;
            StopCoroutine("StartSetAimHandsPosition");
            StopCoroutine("StopSetAimHandPosition");
        }
        
        IEnumerator StopSetCrouchHandPosition()
        {
            yield return new WaitForSeconds(1);
            TakeWeaponInCrouchlMode = true;
            StopCoroutine("StartSetCrouchHandsPosition");
            StopCoroutine("StopSetCrouchHandPosition");
        }

        IEnumerator StopSetWallHandPosition()
        {
            yield return new WaitForSeconds(0.5f);
            TakeWeaponInWallMode = true;
            StopCoroutine("StartSetWallHandsPosition");
            StopCoroutine("StopSetWallHandPosition");
        }

        public IEnumerator WalkWithWeaponTimeout()
        {
            yield return new WaitForSeconds(0.5f);
            characterAnimations.anim.SetBool("CanWalkWithWeapon", true);
            StopCoroutine("WalkWithWeaponTimeout");
        }

        public IEnumerator GrenadeFlying()
        {
            yield return new WaitForSeconds(GrenadeParameters.GrenadeExplosionTime);
            Explosion();
            StopCoroutine("GrenadeFlying");
        }

        public IEnumerator AimTimeout()
        {
            yield return new WaitForSeconds(1);
            aimTimeout = true;
            if (WasSetSwitchToFP)
            {
                SwitchToFpCamera = true;
                WasSetSwitchToFP = false;
            }

            StopCoroutine("AimTimeout");
        }

        private void OnCollisionEnter(Collision other)
        {
            if (Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                return;

            if (other.collider.CompareTag("Enemy"))
            {
                Explosion();
            }
            else if (other.collider.GetComponent<Controller>())
            {
                if (other.gameObject != Controller.gameObject)
                    Explosion();
            }
            else if (other.collider.GetComponent<Surface>() && GrenadeParameters.ExplodeWhenTouchGround)
            {
                Explosion();
            }
        }

        public void Explosion()
        {
            if (GrenadeParameters.GrenadeExplosion)
            {
                var explosion = Instantiate(GrenadeParameters.GrenadeExplosion, transform.position /*+ transform.TransformDirection(Vector3.forward) * 1.4f*/, transform.rotation);
                explosion.GetComponent<Explosion>().damage = Attacks[currentAttack].weapon_damage;
                if (Controller.CharacterName != null)
                    explosion.GetComponent<Explosion>().OwnerName = Controller.CharacterName;
                StopCoroutine("GrenadeFlying");
                Destroy(gameObject);
            }
        }
        
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if(Application.isPlaying || Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
                return;
            
            if (Attacks[currentAttack].ShellPoint)
            {
                Handles.zTest = CompareFunction.Less;
                Handles.color = new Color32(250, 170, 0, 255);
                Handles.SphereHandleCap(0, Attacks[currentAttack].ShellPoint.position, Quaternion.identity, 0.05f, EventType.Repaint);
                Handles.ArrowHandleCap(0, Attacks[currentAttack].ShellPoint.position, Quaternion.LookRotation(Attacks[currentAttack].ShellPoint.forward), 0.5f,
                    EventType.Repaint);

                Handles.zTest = CompareFunction.Greater;
                Handles.color = new Color32(250, 170, 0, 50);
                Handles.SphereHandleCap(0, Attacks[currentAttack].ShellPoint.position, Quaternion.identity, 0.05f, EventType.Repaint);
                Handles.ArrowHandleCap(0, Attacks[currentAttack].ShellPoint.position, Quaternion.LookRotation(Attacks[currentAttack].ShellPoint.forward), 0.5f,
                    EventType.Repaint);
            }

            if (Attacks[currentAttack].AttackSpawnPoint)
            {
                Handles.zTest = CompareFunction.Less;
                Handles.color = new Color32(250, 0, 0, 255);
                Handles.SphereHandleCap(0, Attacks[currentAttack].AttackSpawnPoint.position, Quaternion.identity, 0.05f, EventType.Repaint);
                Handles.ArrowHandleCap(0, Attacks[currentAttack].AttackSpawnPoint.position, Quaternion.LookRotation(Attacks[currentAttack].AttackSpawnPoint.forward),
                    0.5f, EventType.Repaint);

                Handles.zTest = CompareFunction.Greater;
                Handles.color = new Color32(250, 0, 0, 50);
                Handles.SphereHandleCap(0, Attacks[currentAttack].AttackSpawnPoint.position, Quaternion.identity, 0.05f, EventType.Repaint);
                Handles.ArrowHandleCap(0, Attacks[currentAttack].AttackSpawnPoint.position, Quaternion.LookRotation(Attacks[currentAttack].AttackSpawnPoint.forward),
                    0.5f, EventType.Repaint);
            }
        }
#endif
    }
}



	


				
	
	