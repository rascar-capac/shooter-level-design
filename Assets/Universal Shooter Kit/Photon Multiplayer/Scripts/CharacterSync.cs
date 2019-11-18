// GercStudio
// © 2018-2019

using System;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GercStudio.USK.Scripts
{

    public enum PhotonEventCodes
    {
        ChangeHealth = 0,
        ChangeWeapon = 1,
        PickUp = 2,
        Grenade = 3,
        DropWeapon = 4,
        Attack = 5,
        Fire = 6,
        Rocket = 7,
        BulletHit = 8,
        ChangeCameraType = 9,
        ChangeEnemyHealth = 10,
        PlayerDeath = 11,
        Reload = 12,
        Aim = 13, 
        ChangeAttack = 14, 
        Crouch = 15,
        HideAllWeapons = 16
    }

    public class CharacterSync : MonoBehaviourPun, IPunObservable
    {
        public TextMesh PlayerStatsText;
        
        [HideInInspector]
        public float currentHealth;
        
        private Controller controller;
        private InventoryManager weaponManager;
        private WeaponController weaponController; 
        
        private GameObject grenade;

       // private Vector3 CharacterPositions;
        private Vector3 CameraPosition;
        private Vector3 CameraRotation;
        
        private float destroyTimeOut;

        private bool hasTimerStarted;
        private bool sendEvent = true;
    
        #region StartMethods
    
            private void OnEnable()
            {
                PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
            }
        
            private void OnDisable()
            {
                PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
            }
        
            private void Awake()
            {
                controller = GetComponent<Controller>();
                weaponManager = GetComponent<InventoryManager>();
                if (!photonView.IsMine & PhotonNetwork.IsConnected)
                {
                    controller.isMultiplayerCharacter = true;
                }
            }
        
            void Start()
            {
                weaponManager.ActiveDropWeapon = false;
                
                currentHealth = controller.PlayerHealth;
                controller.CharacterName = photonView.Owner.NickName;
                if (PhotonNetwork.InRoom)
                {
                    if ((ModeOfGame) PhotonNetwork.CurrentRoom.CustomProperties["gm"] == ModeOfGame.Deathmatch)
                    {
                        weaponManager.Coop = false;
                    }
                    else if ((ModeOfGame) PhotonNetwork.CurrentRoom.CustomProperties["gm"] == ModeOfGame.Cooperative)
                    {
                        weaponManager.Coop = true;
                    }
                }

                if (!photonView.IsMine & PhotonNetwork.IsConnected)
                {
                    if (Application.isMobilePlatform & controller.UiButtonsGameObject)
                        controller.UiButtonsGameObject.SetActive(false);
                    controller.thisCamera.SetActive(false);
                    var _audio = controller.gameObject.GetComponent<AudioSource>();
                    _audio.spatialBlend = 1;
                    _audio.minDistance = 1;
                    _audio.maxDistance = 500;
                  //  controller.isMultiplayerCharacter = true;
                   // controller.Health_Text.gameObject.SetActive(false);
                   // WeaponManager.AmmoUI.gameObject.SetActive(false);
                   // WeaponManager.GrenadeAmmoUI.gameObject.SetActive(false);
                   // controller.thisCameraScript.Crosshair.gameObject.SetActive(false);
                   // controller.thisCameraScript.PickUpCrosshair.gameObject.SetActive(false);
                    Helper.ChangeLayersRecursively(gameObject.transform, "Default");

                    PlayerStatsText.transform.parent = controller.BodyObjects.Head;
                    PlayerStatsText.transform.localPosition = Vector3.up * 2;
                    
                    controller.thisCameraScript.LayerCamera = Helper.NewCamera("LayerCamera", transform, "Sync").gameObject;
                    controller.thisCameraScript.LayerCamera.SetActive(false);
                    
                    if (PlayerStatsText)
                        PlayerStatsText.text = photonView.Owner.NickName + "\n" + currentHealth.ToString("F0");

                }
                else
                {
                    if (Application.isMobilePlatform)
                    {
                        var room = FindObjectOfType<RoomManager>();
                        if (room)
                            controller.uiButtons[9].onClick.AddListener(room.Pause);
                    }

                    controller.thisCamera.SetActive(true);
                   
                    if(Application.isMobilePlatform & controller.UiButtonsGameObject)
                        controller.UiButtonsGameObject.SetActive(true);
                    
                    //controller.Health_Text.gameObject.SetActive(true);
                    //WeaponManager.AmmoUI.gameObject.SetActive(true);
                   // WeaponManager.GrenadeAmmoUI.gameObject.SetActive(true);
                    //controller.thisCamera.GetComponent<CameraController>().crosshair.gameObject.SetActive(true);
                    //Helper.ChangeLayersRecursively(gameObject.transform, "Default");
                  
                    if(PlayerStatsText)
                        PlayerStatsText.gameObject.SetActive(false);
                }
            }
    
        #endregion
    
        #region UpdateSynchElements

        void Update()
        {
            weaponController = weaponManager.weaponController;
           
            if (!photonView.IsMine)
            {
                if (Math.Abs(currentHealth - controller.PlayerHealth) > 0.1f)
                {
                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.AddToRoomCache,
                        Receivers = ReceiverGroup.Others
                    };
                    object[] content =
                    {
                        photonView.ViewID, controller.PlayerHealth, controller.KillerName, controller.KillMethod
                    };
                    PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.ChangeHealth, content, options,
                        SendOptions.SendReliable);
                    currentHealth = controller.PlayerHealth;
                }
            }
            else
            {
                if (weaponManager.MultiplayerUseHealth)
                {
                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.AddToRoomCache,
                        Receivers = ReceiverGroup.Others
                    };
                    
                    object[] content = { photonView.ViewID, controller.PlayerHealth };
                    
                    PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.ChangeHealth, content, options, SendOptions.SendReliable);

                    weaponManager.MultiplayerUseHealth = false;
                }
            }


            if (!photonView.IsMine && PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                if (PlayerStatsText)
                    PlayerStatsText.text = photonView.Owner.NickName + "\n" + controller.PlayerHealth.ToString("F0");

                controller.thisCamera.transform.position = CameraPosition;
                controller.thisCamera.transform.eulerAngles = CameraRotation;
            }
        }

        void LateUpdate()
        {
            if (!photonView.IsMine & PhotonNetwork.IsConnected & PhotonNetwork.InRoom)
            {
                controller.BodyObjects.TopBody.localEulerAngles = controller.BodyLocalEulerAngles;

//                    transform.rotation = controller.CurrentRotation;

//                    if (controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
//                    {
//                        transform.position = Vector3.MoveTowards(transform.position, CharacterPositions, 10 * Time.deltaTime);
//                    }

                if (PlayerStatsText & FindObjectOfType<RoomManager>().Player)
                {
                    PlayerStatsText.transform.LookAt(FindObjectOfType<RoomManager>().Player.GetComponent<Controller>().thisCamera.transform);
                    PlayerStatsText.transform.RotateAround(PlayerStatsText.transform.position, PlayerStatsText.transform.up, 180);
                }

                if (weaponController)
                {
                    if (!weaponController.isMultiplayerWeapon)
                        weaponController.isMultiplayerWeapon = true;

                    if (weaponController.gameObject.layer == 8)
                        Helper.ChangeLayersRecursively(weaponController.transform, "Default");

//                        if (controller.anim.GetBool("Reload"))
//                        {
//                            controller.WeaponManager.weaponController.IsReloadEnabled = true;
//                            controller.WeaponManager.SmoothIKSwitch = 0;
//                        }
//                        else
//                        {
//                            controller.WeaponManager.weaponController.IsReloadEnabled = false;
//                            controller.WeaponManager.SmoothIKSwitch = 1;
//                        }
                }
            }
            else if (photonView.IsMine & PhotonNetwork.IsConnected & PhotonNetwork.InRoom)
            {
                if (controller.changeCameraType)
                {
                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.AddToRoomCache,
                        Receivers = ReceiverGroup.Others
                    };
                    RaiseEventSender(controller.TypeOfCamera, PhotonEventCodes.ChangeCameraType, options);
                    controller.changeCameraType = false;
                }

                if (weaponController)
                {
                    if (weaponController.gameObject.layer != 8)
                        Helper.ChangeLayersRecursively(weaponController.transform, "Character");

                    if (weaponController.isMultiplayerWeapon)
                        weaponController.isMultiplayerWeapon = false;

                    if (weaponController.MultiplayerAim)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(null, PhotonEventCodes.Aim, options);
                        weaponController.MultiplayerAim = false;
                    }

                    if (weaponManager.MultiplayerHideAllWeapons)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.AddToRoomCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(null, PhotonEventCodes.HideAllWeapons, options);
                        weaponManager.MultiplayerHideAllWeapons = false;
                    }

                    if (weaponController.MultiplayerChangeAttack)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.AddToRoomCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(null, PhotonEventCodes.ChangeAttack, options);
                        weaponController.MultiplayerChangeAttack = false;
                    }

                    if (controller.multiplayerCrouch)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.AddToRoomCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(null, PhotonEventCodes.Crouch, options);
                        controller.multiplayerCrouch = false;
                    }

                    if (weaponController.MultiplayerReload)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(null, PhotonEventCodes.Reload, options);
                        weaponController.MultiplayerReload = false;
                    }

                    if (weaponController.MultiplayerAttack)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(0, PhotonEventCodes.Attack, options);
                        weaponController.MultiplayerAttack = false;
                    }

                    if (weaponController.MultiplayerFire)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(true, PhotonEventCodes.Fire, options);

                        weaponController.MultiplayerFire = false;
                        sendEvent = false;
                    }
                    else
                    {
                        if (!sendEvent)
                        {
                            var options = new RaiseEventOptions
                            {
                                CachingOption = EventCaching.DoNotCache,
                                Receivers = ReceiverGroup.Others
                            };

                            RaiseEventSender(false, PhotonEventCodes.Fire, options);
                            sendEvent = true;
                        }
                    }

                    if (weaponController.MultiplayerRocket)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        object[] content =
                        {
                            photonView.ViewID,
                            weaponController.Hit.point,
                            weaponController.MultiplayerRocketRaycast,
                            controller.thisCamera.transform.position,
                            controller.thisCamera.transform.rotation
                        };
                        PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.Rocket, content, options,
                            SendOptions.SendReliable);
                        weaponController.MultiplayerRocket = false;
                    }

                    if (weaponController.MultiplayerBulletHit)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        object[] content =
                        {
                            photonView.ViewID,
                            weaponController.Direction,
                            controller.thisCamera.transform.position
                        };
                        PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.BulletHit, content, options, SendOptions.SendReliable);

                        weaponController.MultiplayerBulletHit = false;
                    }
                }

                if (weaponManager.MultiplayerLaunchGrenade)
                {
                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.DoNotCache,
                        Receivers = ReceiverGroup.Others
                    };
                    RaiseEventSender(0, PhotonEventCodes.Grenade, options);
                    weaponManager.MultiplayerLaunchGrenade = false;
                }

                if (weaponManager.isMultiplayerPickUp)
                {

                    var room = FindObjectOfType<PickUpManager>();
                    if(room) room.PickUp(photonView.ViewID, weaponManager.currentPickUpId);
                    else Debug.LogError("You should add the [PickUp Manger] script in your scene.");


//                        RaiseEventOptions options = new RaiseEventOptions()
//                        {
//                            CachingOption = EventCaching.AddToRoomCacheGlobal,
//                            Receivers = ReceiverGroup.Others
//                        };
//                        RaiseEventSender(WeaponManager.PickUpIdMultiplayer, PhotonEventCodes.PickUp, options);
//                        
                    weaponManager.isMultiplayerPickUp = false;
                }

                if (weaponManager.MultiplayerChangedWeapon)
                {
                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.AddToRoomCache,
                        Receivers = ReceiverGroup.Others
                    };
                    object[] content =
                    {
                        photonView.ViewID,
                        weaponManager.currentSlot,
                        weaponManager.slots[weaponManager.currentSlot].currentWeaponInSlot,
                    };

                    PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.ChangeWeapon, content, options,
                        SendOptions.SendReliable);

                    weaponManager.MultiplayerChangedWeapon = false;
                }

                if (weaponManager.hasWeaponDropped)
                {
                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.AddToRoomCache,
                        Receivers = ReceiverGroup.Others
                    };

                    object[] content =
                    {
                        photonView.ViewID,
                        weaponManager.DropIdMultiplayer,
                        weaponManager.DropDirection
                    };
                    PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.DropWeapon, content, options,
                        SendOptions.SendReliable);

                    weaponManager.hasWeaponDropped = false;
                }

//                    if (currentWeapon != WeaponManager.current_Weapon)
//                    {
//                        RaiseEventOptions options = new RaiseEventOptions()
//                        {
//                            CachingOption = EventCaching.AddToRoomCache,
//                            Receivers = ReceiverGroup.Others
//                        };
//                        RaiseEventSender(WeaponManager.current_Weapon, PhotonEventCodes.ChangeWeapon, options);
//                        currentWeapon = WeaponManager.current_Weapon;
//                    }
//                    else if (currentWeapon != WeaponManager.current_Weapon &
//                             WeaponManager.weapons[WeaponManager.current_Weapon].GetComponent<PhotonPickUp>())
//                    {
//                        currentWeapon = WeaponManager.current_Weapon;
//                        if (photonView.IsMine)
//                        {
//                            Helper.ChangeLayersRecursively(gameObject.transform, "Character");
//                        }
//                        else
//                        {
//                            Helper.ChangeLayersRecursively(gameObject.transform, "Default");
//                        }
//        
//                        Destroy(WeaponManager.weapons[WeaponManager.current_Weapon].GetComponent<PhotonPickUp>());
//                    }



                destroyTimeOut += Time.deltaTime;

                if (controller.PlayerHealth <= 0 & !hasTimerStarted)
                {
                    var PlayersList = PhotonNetwork.PlayerList;
                    var customValues = new Hashtable();

                    //update player death count
                    int deaths = (int) photonView.Owner.CustomProperties["d"] + 1;
                    customValues.Clear();
                    customValues.Add("d", deaths);
                    photonView.Owner.SetCustomProperties(customValues);


                    for (int i = 0; i < PlayersList.Length; i++)
                    {
                        //update player kills count
                        if (controller.KillerName == PlayersList[i].NickName)
                        {
                            int kills = (int) PlayersList[i].CustomProperties["k"] + 1;
                            customValues.Clear();
                            customValues.Add("k", kills);
                            PlayersList[i].SetCustomProperties(customValues);
                        }
                    }

                    FindObjectOfType<RoomManager>().ClearPlayerListings();
                    FindObjectOfType<RoomManager>().PlayerList();

                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.DoNotCache,
                        Receivers = ReceiverGroup.Others
                    };
                    RaiseEventSender(null, PhotonEventCodes.PlayerDeath, options);

                    var roomManager = FindObjectOfType<RoomManager>();
                    roomManager.StatsContent.gameObject.SetActive(true);
                    roomManager.StartCoroutine(roomManager.StatsEnabledTimer());
                    var tempListing = Instantiate(roomManager.StatsText, roomManager.StatsContent.GetChild(0).GetChild(0));
                    tempListing.text = controller.KillerName + " - " + controller.KillMethod + " - " + photonView.Owner.NickName;

                    hasTimerStarted = true;
                }

                if (hasTimerStarted & destroyTimeOut >= 0.1f)
                {
                    FindObjectOfType<RoomManager>().Player = null;
                    Destroy(controller.thisCamera.transform.parent.gameObject);
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }

        #endregion
        
        #region ProcessingSynchElements
    
        void RaiseEventSender(object value, PhotonEventCodes code, RaiseEventOptions options)
        {
            object[] content =
            {
                photonView.ViewID, value
            };
            PhotonNetwork.RaiseEvent((byte) code, content, options, SendOptions.SendReliable);
        }

        void OnEvent(EventData photonEvent)
        {
            PhotonEventCodes eventCode = (PhotonEventCodes) photonEvent.Code;
            object[] data = photonEvent.CustomData as object[];
            if (data != null)
                if ((int) data[0] == photonView.ViewID)
                {
                    if (eventCode == PhotonEventCodes.ChangeHealth)
                    {
                        if (data.Length == 4)
                        {
                            controller.PlayerHealth = (float) data[1];
                            currentHealth = controller.PlayerHealth;
                            
                            if (controller.PlayerHealth <= 0)
                            {
                                controller.KillerName = (string) data[2];
                                controller.KillMethod = (string) data[3];
                            }
                        }
                        else if (data.Length == 2)
                        {
                            controller.PlayerHealth = (float) data[1];
                            currentHealth = controller.PlayerHealth;
                        }
                        
                    }
                    else if (eventCode == PhotonEventCodes.PlayerDeath)
                    {
                        if (data.Length == 2)
                        {
                            if (controller.Ragdoll)
                            {
                                Transform dead =
                                    Instantiate(controller.Ragdoll, transform.position, transform.rotation);
                                Helper.CopyTransformsRecurse(transform, dead);
                            }

                            var room = FindObjectOfType<RoomManager>();
                            room.ClearPlayerListings();
                            room.PlayerList();
                            room.StatsContent.gameObject.SetActive(true);
                            room.StartCoroutine(room.StatsEnabledTimer());
                            var tempListing = Instantiate(room.StatsText, room.StatsContent.GetChild(0).GetChild(0));
                            tempListing.text = controller.KillerName + " - " + controller.KillMethod + " - " +
                                               photonView.Owner.NickName;
                        }
                    }
                    else if (eventCode == PhotonEventCodes.ChangeWeapon)
                    {
                        if (data.Length == 3)
                        {
                            weaponManager.currentSlot = (int) data[1];
                            weaponManager.slots[weaponManager.currentSlot].currentWeaponInSlot = (int) data[2];
                            
                            weaponManager.Switch(weaponManager.currentSlot);
                            // WeaponManager.current_Weapon = (int) data[1];
                            // WeaponManager.Switch();

                            var _audio = weaponManager.slots[weaponManager.currentSlot]
                                .weaponsInInventory[weaponManager.slots[weaponManager.currentSlot].currentWeaponInSlot]
                                .weapon.GetComponent<AudioSource>();
                            if (_audio.spatialBlend != 1)
                            {
                                _audio.spatialBlend = 1;
                                _audio.minDistance = 1;
                                _audio.maxDistance = 200;
                            }
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Aim)
                    {
                        if (data.Length == 2)
                        {
                            weaponController.Aim(false, false);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.HideAllWeapons)
                    {
                        if (data.Length == 2)
                        {
                            weaponManager.hideAllWeapons = true;
                            weaponManager.SwitchNewWeapon();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Crouch)
                    {
                        controller.isCrouch = !controller.isCrouch;
                        
                        if (controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                        {
                            if (controller.WeaponManager.hasAnyWeapon)
                                weaponManager.weaponController.CrouchHands();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Grenade)
                    {
                        if (data.Length == 2)
                        {
//                            WeaponManager.Grenades[WeaponManager.currentGrenade].grenadeAmmo -= 1;
//                            WeaponManager.creategrenade = true;
//                            controller.anim.SetBool("Grenade", true);
//                            weaponManager.StartCoroutine("CreateGrenade");
//                            weaponManager.BeginningLaunchingGrenade();
                            weaponManager.LaunchGrenade();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.ChangeAttack)
                    {
                        if (data.Length == 2)
                        {
                            weaponController.ChangeAttack();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Reload)
                    {
                        if (data.Length == 2)
                        {
                            weaponController.Reload();
//                            if (weaponController.Attacks[weaponController.currentAttack].ReloadAudio)
//                                weaponController.GetComponent<AudioSource>().PlayOneShot(weaponController.Attacks[weaponController.currentAttack].ReloadAudio);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Attack)
                    {
                        if (data.Length == 2)
                        {
                            if (weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Knife)
                                if (weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint & weaponController.Attacks[weaponController.currentAttack].MuzzleFlash)
                                {
                                    var Flash = Instantiate(weaponController.Attacks[weaponController.currentAttack].MuzzleFlash, weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.position,
                                        weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.rotation);
                                    Flash.transform.parent = weaponController.transform;
                                    Flash.transform.localRotation = Quaternion.Euler(0, 0, 0);
//                                    Helper.ChangeLayersRecursively(Flash, "Character");
                                    Flash.gameObject.AddComponent<DestroyObject>().destroy_time = 0.17f;
                                }

                            if (weaponController.Attacks[weaponController.currentAttack].Shell &&
                                weaponController.Attacks[weaponController.currentAttack].ShellPoint)
                            {
                                var _shell = Instantiate(weaponController.Attacks[weaponController.currentAttack].Shell,
                                    weaponController.Attacks[weaponController.currentAttack].ShellPoint.position, 
                                    weaponController.Attacks[weaponController.currentAttack].ShellPoint.localRotation);
                                _shell.AddComponent<ShellControll>().ShellPoint = weaponController.Attacks[weaponController.currentAttack].ShellPoint;
                            }

                            if (weaponController.Attacks[weaponController.currentAttack].AttackAudio)
                                weaponController.GetComponent<AudioSource>().PlayOneShot(weaponController.Attacks[weaponController.currentAttack].AttackAudio);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Fire)
                    {
                        if (data.Length == 2)
                        {
                            if (!(bool) data[1])
                            {
                                weaponController.GetComponent<AudioSource>().Stop();
                                weaponController.AttackAudioPlay = false;
                            }
                            else
                            {
                                Instantiate(weaponController.Attacks[weaponController.currentAttack].Fire, weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.position,
                                    weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.rotation);
                                if (!weaponController.AttackAudioPlay)
                                    if (weaponController.Attacks[weaponController.currentAttack].AttackAudio)
                                    {
                                        weaponController.GetComponent<AudioSource>().clip = weaponController.Attacks[weaponController.currentAttack].AttackAudio;
                                        weaponController.GetComponent<AudioSource>().Play();
                                        weaponController.AttackAudioPlay = true;
                                    }
                            }
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Rocket)
                    {
                        if (data.Length == 5)
                        {
                            var rocket = Instantiate(weaponController.Attacks[weaponController.currentAttack].Rocket,
                                weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.position,
                                weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.rotation);
                            
                            controller.thisCamera.transform.rotation = (Quaternion) data[4];
                            controller.thisCamera.transform.position = (Vector3) data[3];
                            rocket.GetComponent<Rocket>().isRaycast = (bool) data[2];
                            rocket.GetComponent<Rocket>().TargetPoint = (Vector3) data[1];
                            rocket.GetComponent<Rocket>().Camera = controller.thisCamera.transform;
                            
                            if (weaponController.Attacks[weaponController.currentAttack].AttackAudio)
                                weaponController.gameObject.GetComponent<AudioSource>().PlayOneShot(weaponController.Attacks[weaponController.currentAttack].AttackAudio);
                        }

                    }
                    else if (eventCode == PhotonEventCodes.ChangeCameraType)
                    {
                        if (data.Length == 2)
                        {
                            CharacterHelper.SwitchCamera(controller.TypeOfCamera, (CharacterHelper.CameraType) data[1], controller);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.DropWeapon)
                    {
                        if (data.Length == 3)
                        {
                            weaponManager.DropIdMultiplayer = (string) data[1];
//                            WeaponManager.DropDirection = (Vector3) data[2];
                            weaponManager.DropWeapon();
                        }
                    }
//                    else if (eventCode == PhotonEventCodes.PickUp)
//                    {
//                        if (data.Length == 2)
//                        {
//                            var foundObjects = FindObjectsOfType<PickUp>();
//                            var id = (string) data[1];
//                            foreach (var obj in foundObjects)
//                            {
//                                if (obj.pickUpId == id)
//                                {
//                                    obj.PickUpObject(gameObject);
//                                }
//                            }
//                        }
//                    }
                    else if (eventCode == PhotonEventCodes.BulletHit)
                    {
                        if (data.Length == 3)
                        {
                            var direction = (Vector3) data[1];
                            var origin = (Vector3) data[2];
                            RaycastHit hitPoint;
                            
                            if (Physics.Raycast(origin, direction, out hitPoint, 10000))
                            {
                                var HitRotation = Quaternion.FromToRotation(Vector3.up, hitPoint.normal);
                                Surface surface;

                                if (hitPoint.collider.GetComponent<Surface>())
                                {
                                    surface = hitPoint.collider.GetComponent<Surface>();

                                    Instantiate(surface.Sparks, hitPoint.point + hitPoint.normal * 0.01f, HitRotation);
                                    var hitGO = Instantiate(surface.Hit, hitPoint.point + hitPoint.normal * 0.001f, HitRotation).transform;
                                    if (surface.HitAudio)
                                    {
                                        var _audio = hitGO.gameObject.AddComponent<AudioSource>();
                                        _audio.spatialBlend = 1;
                                        _audio.minDistance = 1;
                                        _audio.maxDistance = 50;
                                        _audio.PlayOneShot(hitGO.gameObject.GetComponent<AudioSource>().clip);
                                    }

                                    hitGO.parent = hitPoint.transform;
                                }
                                if (hitPoint.collider.GetComponent<WeaponController>())
                                {
                                    if (hitPoint.collider.GetComponent<WeaponController>().Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
                                    {
                                        hitPoint.collider.GetComponent<WeaponController>().Explosion();
                                    }
                                }
//                                else if (hitPoint.collider.GetComponent<Controller>())
//                                {
//                                    hitPoint.collider.GetComponent<Controller>().Damage(2, "2");
//                                }
                            }
                        }
                    }
                }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
         {
    
             if (stream.IsWriting)
             {
                 stream.SendNext(controller.BodyLocalEulerAngles);
                 stream.SendNext(controller.CurrentRotation);
                 stream.SendNext(controller.SmoothIKSwitch);
                 stream.SendNext(controller.currentGravity);
                 //stream.SendNext(transform.position);
                 stream.SendNext(controller.thisCamera.transform.position);
                 stream.SendNext(controller.thisCamera.transform.eulerAngles);
                 stream.SendNext(controller.currentCharacterControllerCenter);
             }
             else
             {
                 controller.BodyLocalEulerAngles = (Vector3) stream.ReceiveNext();
                 controller.CurrentRotation = (Quaternion) stream.ReceiveNext();
                 controller.SmoothIKSwitch = (float) stream.ReceiveNext();
                 controller.currentGravity = (float) stream.ReceiveNext();
                 
                 //CharacterPositions = (Vector3) stream.ReceiveNext();
                 CameraPosition = (Vector3) stream.ReceiveNext();
                 CameraRotation = (Vector3) stream.ReceiveNext();
                 controller.CharacterController.center =
                     new Vector3(controller.CharacterController.center.x, (float) stream.ReceiveNext(), controller.CharacterController.center.z);
             }
         }
    
         #endregion
         
    }
}


