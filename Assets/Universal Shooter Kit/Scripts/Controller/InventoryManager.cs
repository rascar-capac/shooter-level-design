// GercStudio
// © 2018-2019

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;


namespace GercStudio.USK.Scripts
{

    public class InventoryManager : MonoBehaviour
    {
        [Serializable]
        public class Weapon
        {
            public GameObject weapon;
            public List<Kit> WeaponAmmoKits;
        }

        [Serializable]
        public class InventorySlot
        {
            public bool hideAllWeaponsSlot;
            
            public int currentWeaponInSlot;
            public int weaponsCount;
            public Button SlotButton;
            public List<WeaponsHelper.WeaponSlotInInventory> weaponSlots = new List<WeaponsHelper.WeaponSlotInInventory>{new WeaponsHelper.WeaponSlotInInventory()};
            public List<Weapon> weaponsInInventory;
            public Text curAmmoText;
            public RawImage SlotImage;
        }

        [SerializeField] public InventorySlot[] slots;

        [Serializable]
        public class Kit
        {
            public int AddedValue;
            public Texture Image;

            public string PickUpId;
            public string ammoType;
        }

        [SerializeField] public List<Kit> HealthKits;
        [SerializeField] public List<Kit> ReserveAmmo;

        public GameObject inventoryWheel;
        public GameObject gun;
        public GameObject grenadeClone;

        public List<WeaponsHelper.GrenadeSlot> Grenades = new List<WeaponsHelper.GrenadeSlot>{new WeaponsHelper.GrenadeSlot()};
       
        public int currentSlot;
        public int currentAmmoKit;
        public int currentHealthKit;
        public int currentGrenade;
        
       
        public float SmoothIKSwitch;
        public float SmoothHeadIKSwitch;

        //inspector variables
        //public int inspectorTab;
        
        public int inventoryTabUp;
        public int inventoryTabMiddle;
        public int inventoryTabDown;
        public int currentInventorySlot;

        public string DropIdMultiplayer;
        public string currentPickUpId;
        
        public bool Coop;
        public bool GamepadConnect;
        public bool creategrenade;
        public bool MultiplayerLaunchGrenade;
        public bool hasWeaponTaken;
        public bool grenadeDebug;
        public bool hasAnyWeapon;
        public bool isPickUp;
        public bool pickUpUiButton;
        public bool isMultiplayerPickUp;
        public bool MultiplayerChangedWeapon;
        public bool MultiplayerHideAllWeapons;
        public bool hasWeaponDropped;
        public bool pressInventoryButton;
        public bool ActiveDropWeapon;
        public bool hideAllWeapons;
        public bool MultiplayerUseHealth;

        public Canvas canvas;
        
        public Color[] normButtonsColors = new Color[10];
        
        public Sprite[] normButtonsSprites = new Sprite[10];
        public Texture HideAllWeaponImage;

        public Text GrenadeAmmoUI;
        public Text AmmoUI;
        
        public Text CurrWeaponText;
        public Text currHealthKitCount;
        public Text currentHealthKitAddedValue;
        public Text currAmmoKitCount;
        public Text currentAmmoKitAddedValue;

        public Button UpWeaponButton;
        public Button DownWeaponButton;
        public Button UpHealthButton;
        public Button DownHealthButton;
        public Button UpAmmoButton;
        public Button DownAmmoButton;
        public Button AmmoButton;
        public Button HealthButton;

        public RawImage HealthImage;
        public RawImage AmmoImage;
        public RawImage aimTextureImage;
        
        public RenderTexture ScopeScreenTexture;

        public Vector3 DropDirection;

        public Controller controller;
        public WeaponController weaponController;
        public WeaponController grenadeController;

        private int weaponId;

        private bool canChangeWeaponInSlot;
        private bool tempIsAim;
        private bool tempIsGrenade;
        private bool closeInventory;
        private bool hasWeaponChanged;
        private bool pressedUIInventoryButton;
        private bool canDropWeapon = true;
        private bool gamepadInfo;
        private bool canLaunchGrenadeAgain;
        private bool setWeaponLayer = true;
        private bool firstLayerSet;

        private GameObject currentDropWeapon;

        private RaycastHit wallHitInfoRH;

        private void Awake()
        {
            controller = GetComponent<Controller>();
            FindWeapons();
        }

        void Start()
        {
            ActiveDropWeapon = true;

            //!
            currentGrenade = 0;
            //
            
            if (Grenades.Count > 0)
            {
                var hasAnyGrenade = false;
                foreach (var grenade in Grenades)
                {
                    if (grenade.Grenade)
                    {
                        hasAnyGrenade = true;
                        grenade.GrenadeScript = grenade.Grenade.GetComponent<WeaponController>();
                        grenade.grenadeAmmo = (int)grenade.GrenadeScript.Attacks[grenade.GrenadeScript.currentAttack].inventoryAmmo;
                    }
                }
                
                if(GrenadeAmmoUI && hasAnyGrenade)
                    GrenadeAmmoUI.gameObject.SetActive(true);
                else if(GrenadeAmmoUI && !hasAnyGrenade)
                    GrenadeAmmoUI.gameObject.SetActive(false);
               
                StartCoroutine("TimeoutAfterLaunchGrenade");
            }
                

            for (var i = 0; i < 8; i++)
            {
                var slot = i;

                if (!slots[i].SlotButton)
                    continue;

                slots[i].SlotButton.onClick.AddListener(delegate { SelectWeapon(slot); });
                
                switch (slots[i].SlotButton.transition)
                {
                    case Selectable.Transition.ColorTint:
                        normButtonsColors[i] = slots[i].SlotButton.colors.normalColor;
                        break;
                    case Selectable.Transition.SpriteSwap:
                        normButtonsSprites[i] = slots[i].SlotButton.GetComponent<Image>().sprite;
                        break;
                }
            }
            
            ScopeScreenTexture = new RenderTexture(1024, 1024, 24);

            if (HealthButton)
            {
                normButtonsColors[8] = HealthButton.colors.normalColor;
                normButtonsSprites[8] = HealthButton.GetComponent<Image>().sprite;
            }
            
            if (AmmoButton)
            {
                normButtonsColors[9] = AmmoButton.colors.normalColor;
                normButtonsSprites[9] = AmmoButton.GetComponent<Image>().sprite;
            }

            if (UpWeaponButton)
                UpWeaponButton.onClick.AddListener(delegate { UpInventoryValue("weapon"); });

            if (DownWeaponButton)
                DownWeaponButton.onClick.AddListener(delegate { DownInventoryValue("weapon"); });

            if (UpHealthButton)
                UpHealthButton.onClick.AddListener(delegate { UpInventoryValue("health"); });

            if (DownHealthButton)
                DownHealthButton.onClick.AddListener(delegate { DownInventoryValue("health"); });

            if (UpAmmoButton)
                UpAmmoButton.onClick.AddListener(delegate { UpInventoryValue("ammo"); });

            if (DownAmmoButton)
                DownAmmoButton.onClick.AddListener(delegate { DownInventoryValue("ammo"); });

            if (HealthButton)
                HealthButton.onClick.AddListener(delegate { UseKit("health"); });

            if (AmmoButton)
                AmmoButton.onClick.AddListener(delegate { UseKit("ammo"); });

            for (var i = 0; i < 8; i++)
            {
                if (slots[i].weaponsInInventory.Count > 0)
                {
                    if (AmmoUI && !controller.AdjustmentScene)
                        AmmoUI.gameObject.SetActive(true);
                    
                    controller.anim.SetBool("NoWeapons", false);
                    if (!controller.isMultiplayerCharacter)
                    {
                        MultiplayerChangedWeapon = true;
                    }

                    Switch(i);
                    break;
                }

//                if (AmmoUI)
//                    AmmoUI.gameObject.SetActive(false);

                controller.anim.SetBool("NoWeapons", true);
            }

            DeactivateInventory();

            if (Grenades.Count > 0)
            {
                foreach (var grenadeScript in Grenades)
                {
                    if(!grenadeScript.Grenade) continue;
                    
                    if (grenadeScript.GrenadeScript.GrenadeParameters.GrenadeThrow_FPS)
                    {
                        controller.ClipOverrides["GrenadeFPS"] = grenadeScript.GrenadeScript.GrenadeParameters.GrenadeThrow_FPS;
                        controller.newController.ApplyOverrides(controller.ClipOverrides);
                    }
                    else Debug.LogWarning("<color=yellow>Missing Component</color> [GrenadeThrow] animation", gameObject);
                    
                    if (grenadeScript.GrenadeScript.GrenadeParameters.GrenadeThrow_TPS_TDS)
                    {
                        controller.ClipOverrides["GrenadeTPS"] = grenadeScript.GrenadeScript.GrenadeParameters.GrenadeThrow_TPS_TDS;
                        controller.newController.ApplyOverrides(controller.ClipOverrides);
                    }
                    else Debug.LogWarning("<color=yellow>Missing Component</color> [GrenadeThrow] animation", gameObject);
                }
            }
        }

        void Update()
        {
            if (controller.isMultiplayerCharacter || !controller.ActiveCharacter)
                return;

            if (!controller.inputs.ForcedControllerDisconect)
                GamepadConnect = Input.GetJoystickNames().Length > 0;
            else GamepadConnect = false;

            if (!gamepadInfo & GamepadConnect)
            {
                Debug.Log("<color=green>Gamepad connected</color> Keyboard and mouse are inactive");

                gamepadInfo = true;
            }
            
            if (!controller.AdjustmentScene && !gamepadInfo && !GamepadConnect)
            {
                Debug.Log("<color=green>Gamepad disconnected</color> Keyboard and mouse are active");
                gamepadInfo = true;
            }
            

            CheckPickUp();
            
            if (Input.GetKeyDown(controller._gamepadCodes[9])||Input.GetKeyDown(controller._keyboardCodes[9])||
                Helper.CheckGamepadAxisButton(9, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown",
                    controller.inputs.AxisButtonValues[9]))
                DropWeapon();

            if (Input.GetKeyDown(controller._gamepadCodes[14])||Input.GetKeyDown(controller._keyboardCodes[16])||
                Helper.CheckGamepadAxisButton(14, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown",
                    controller.inputs.AxisButtonValues[14]))
                WeaponUp();

            if (Input.GetKeyDown(controller._gamepadCodes[15])||Input.GetKeyDown(controller._keyboardCodes[17])||
                Helper.CheckGamepadAxisButton(15, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown",
                    controller.inputs.AxisButtonValues[15]))
                WeaponDown();

            if (inventoryWheel)
            {
                if (!Application.isMobilePlatform)
                {
                    if (pressInventoryButton)
                    {
                        if (Input.GetKey(controller._gamepadCodes[7]) || Input.GetKey(controller._keyboardCodes[7]) ||
                            Helper.CheckGamepadAxisButton(7, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKey",
                                controller.inputs.AxisButtonValues[7]))
                        {
                            ActivateInventory();
                        }
                        else
                        {
                            if (!closeInventory)
                            {
                                DeactivateInventory();
                                closeInventory = true;
                            }
                        }
                    }
                    else
                    {
                        if (!inventoryWheel.activeSelf &
                            (Input.GetKeyDown(controller._gamepadCodes[7]) || Input.GetKeyDown(controller._keyboardCodes[7]) || 
                             Helper.CheckGamepadAxisButton(7, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown",
                                 controller.inputs.AxisButtonValues[7])))
                        {
                            ActivateInventory();
                        }
                        else if (inventoryWheel.activeSelf &
                                 (Input.GetKeyDown(controller._gamepadCodes[7]) ||
                                  Input.GetKeyDown(controller._keyboardCodes[7]) ||
                                  Helper.CheckGamepadAxisButton(7, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown",
                                      controller.inputs.AxisButtonValues[7])))
                        {
                            if (!closeInventory)
                            {
                                DeactivateInventory();
                                closeInventory = true;
                            }
                        }
                    }
                }
                else
                {
                    if (pressInventoryButton)
                    {
                        if (pressedUIInventoryButton)
                            ActivateInventory();
                        else
                        {
                            if (!closeInventory)
                            {
                                DeactivateInventory();
                                closeInventory = true;
                            }
                        }
                    }
                }

                if (!controller.AdjustmentScene && inventoryWheel.activeSelf)
                    CheckInventoryButtons();
            }

            if (AmmoUI & gun & weaponController)
            {
                AmmoUI.color = weaponController.Attacks[weaponController.currentAttack].curAmmo > 0 ? Color.white : Color.red;

                if (weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Knife)
                {
                    if (weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                        AmmoUI.text = weaponController.Attacks[weaponController.currentAttack].curAmmo.ToString("F0") + "/" + 
                                      weaponController.Attacks[weaponController.currentAttack].inventoryAmmo.ToString("F0");
                }
                else AmmoUI.text = "Knife";
            }

            if (Grenades.Count > 0)
            {
                if (GrenadeAmmoUI)
                {
                    GrenadeAmmoUI.color = Grenades[currentGrenade].grenadeAmmo > 0 ? Color.white : Color.red;

                    GrenadeAmmoUI.text = Grenades[currentGrenade].grenadeAmmo + " ";
                }

                if (Input.GetKeyDown(controller._gamepadCodes[6])||Input.GetKeyDown(controller._keyboardCodes[6])||
                    Helper.CheckGamepadAxisButton(6, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown",
                        controller.inputs.AxisButtonValues[6]))
                    LaunchGrenade();
            }
            else
            {
                if (GrenadeAmmoUI)
                    GrenadeAmmoUI.gameObject.SetActive(false);
            }
        }

        public void WeaponUp()
        {
            if (controller.AdjustmentScene) return;

            if (weaponController && (weaponController.IsAimEnabled || weaponController.IsReloadEnabled) || creategrenade || controller.isPause || !hasWeaponTaken ||
                controller.anim.GetBool("Attack")) return;
            
            if (slots[currentSlot].weaponsInInventory.Count < 2)
            {
                Helper.ChangeButtonColor(this, currentSlot, "norm");

                currentSlot++;
                if (currentSlot > 7)
                    currentSlot = 0;
            }

            var curIndex = slots[currentSlot].currentWeaponInSlot;

            if (hasAnyWeapon)
                ChoiceNewWeapon(curIndex, "up", weaponController.gameObject.GetInstanceID());
            else ChoiceNewWeapon(curIndex, "up", 0);
        }

        public void WeaponDown()
        {
            if (controller.AdjustmentScene) return;

            if (weaponController && (weaponController.IsAimEnabled || weaponController.IsReloadEnabled) || creategrenade || controller.isPause || !hasWeaponTaken ||
                controller.anim.GetBool("Attack")) return;
            
            
            if (slots[currentSlot].weaponsInInventory.Count < 2)
            {
                Helper.ChangeButtonColor(this, currentSlot, "norm");
                
                currentSlot--;
                if (currentSlot < 0)
                    currentSlot = 7;
            }

            var curIndex = slots[currentSlot].currentWeaponInSlot;

            if (hasAnyWeapon)
                ChoiceNewWeapon(curIndex, "down", weaponController.gameObject.GetInstanceID());
            else ChoiceNewWeapon(curIndex, "down", 0);
        }

        void InventoryGamepadInputs()
        {
            var vector = controller.inputs._Stick == Inputs.Stick.MovementStick
                ? new Vector2(Input.GetAxis(controller._gamepadAxes[0]), Input.GetAxis(controller._gamepadAxes[1]))
                : new Vector2(Input.GetAxis(controller._gamepadAxes[2]), Input.GetAxis(controller._gamepadAxes[3]));

            vector.y *= -1;
            
            vector.Normalize();

            if (Math.Abs(vector.x) < 0.4f & Math.Abs(vector.y - 1) < 0.4f)
            {
                if (slots[1].weaponsInInventory.Count > 0)
                {
                    SelectWeapon(1);
                    DeselectAllSlots(1);
                    Helper.ChangeButtonColor(this, 1, "high");
                }
            }
            else if (Math.Abs(vector.x - 0.707f) < 0.4f & Math.Abs(vector.y - 0.707f) < 0.4f)
            {
                if (slots[2].weaponsInInventory.Count > 0)
                {
                    SelectWeapon(2);
                    DeselectAllSlots(2);
                    Helper.ChangeButtonColor(this, 2, "high");
                }
            }
            else if (Math.Abs(vector.x - 1) < 0.4f & Math.Abs(vector.y) < 0.4f)
            {
                if (slots[3].weaponsInInventory.Count > 0)
                {
                    SelectWeapon(3);
                    DeselectAllSlots(3);
                    Helper.ChangeButtonColor(this, 3, "high");
                }
            }
            else if (Math.Abs(vector.x - 0.707f) < 0.4f & Math.Abs(vector.y + 0.707f) < 0.4f)
            {
                if (slots[4].weaponsInInventory.Count > 0)
                {
                    SelectWeapon(4);
                    DeselectAllSlots(4);
                    Helper.ChangeButtonColor(this, 4, "high");
                }
            }
            else if (Math.Abs(vector.x ) < 0.4f & Math.Abs(vector.y + 1) < 0.4f)
            {
                if (slots[5].weaponsInInventory.Count > 0)
                {
                    SelectWeapon(5);
                    DeselectAllSlots(5);
                    Helper.ChangeButtonColor(this, 5, "high");
                }
            }
            else if (Math.Abs(vector.x + 0.707f) < 0.4f & Math.Abs(vector.y + 0.707f) < 0.4f)
            {
                if (slots[6].weaponsInInventory.Count > 0)
                {
                    SelectWeapon(6);
                    DeselectAllSlots(6);
                    Helper.ChangeButtonColor(this, 6, "high");
                }
            }
            else if (Math.Abs(vector.x + 1) < 0.4f & Math.Abs(vector.y) < 0.4f)
            {
                if (slots[7].weaponsInInventory.Count > 0)
                {
                    SelectWeapon(7);
                    DeselectAllSlots(7);
                    Helper.ChangeButtonColor(this, 7, "high");
                }
            }
            else if (Math.Abs(vector.x + 0.707f) < 0.4f & Math.Abs(vector.y - 0.707f) < 0.4f)
            {
                if (slots[0].weaponsInInventory.Count > 0)
                {
                    SelectWeapon(0);
                    DeselectAllSlots(0);
                    Helper.ChangeButtonColor(this, 0, "high");
                }
            }
            
            var axis = Input.GetAxis(controller._gamepadAxes[4]);

            if (Math.Abs(axis + 1) < 0.1f)
            {
                if (canChangeWeaponInSlot)
                {
                    DownInventoryValue("weapon");
                    canChangeWeaponInSlot = false;
                }
            }
            else if (Math.Abs(axis - 1) < 0.1f)
            {
                if (canChangeWeaponInSlot)
                {
                    UpInventoryValue("weapon");
                    canChangeWeaponInSlot = false;
                }
            }
            else if (Math.Abs(axis) < 0.1f)
            {
                if(!canChangeWeaponInSlot)
                    canChangeWeaponInSlot = true;
            }


            if (Input.GetKeyDown(controller._gamepadCodes[12])|| 
                Helper.CheckGamepadAxisButton(12, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown",
                    controller.inputs.AxisButtonValues[12]))
                UseKit("health");
            
            
            if (Input.GetKeyDown(controller._gamepadCodes[13]) || 
                Helper.CheckGamepadAxisButton(13, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown",
                    controller.inputs.AxisButtonValues[13]))
                if(slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count > 0)
                    UseKit("ammo");
        }
        

        void DeselectAllSlots(int curSlot)
        {
            for (var i = 0; i < 8; i++)
            {
                if (i != curSlot)
                    Helper.ChangeButtonColor(this, i, "norm");
            }
            
            Helper.ChangeColor(HealthButton, normButtonsColors[8], normButtonsSprites[8]);
            Helper.ChangeColor(AmmoButton, normButtonsColors[9], normButtonsSprites[9]);
        }

        void CheckInventoryButtons()
        {
            if (GamepadConnect)
                InventoryGamepadInputs();
            
            if (HealthKits.Count > 1 & !GamepadConnect)
            {
                if (UpHealthButton)
                    UpHealthButton.gameObject.SetActive(true);

                if (DownHealthButton)
                    DownHealthButton.gameObject.SetActive(true);
            }
            else
            {
                if (UpHealthButton)
                    UpHealthButton.gameObject.SetActive(false);

                if (DownHealthButton)
                    DownHealthButton.gameObject.SetActive(false);
            }

            if (slots[currentSlot].weaponsInInventory.Count > 0 & !GamepadConnect)
            {
                if (slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count > 1)
                {
                    if (UpAmmoButton)
                        UpAmmoButton.gameObject.SetActive(true);
                    if (DownAmmoButton)
                        DownAmmoButton.gameObject.SetActive(true);
                }
                else
                {
                    if (UpAmmoButton)
                        UpAmmoButton.gameObject.SetActive(false);
                    if (DownAmmoButton)
                        DownAmmoButton.gameObject.SetActive(false);
                }

            }
            else
            {
                if (UpAmmoButton)
                    UpAmmoButton.gameObject.SetActive(false);
                if (DownAmmoButton)
                    DownAmmoButton.gameObject.SetActive(false);
            }

            if (slots[currentSlot].weaponsInInventory.Count > 1)
            {
                if (UpWeaponButton)
                    UpWeaponButton.gameObject.SetActive(true);

                if (DownWeaponButton)
                    DownWeaponButton.gameObject.SetActive(true);

                if (CurrWeaponText)
                {
                    CurrWeaponText.gameObject.SetActive(true);

                    CurrWeaponText.text = (slots[currentSlot].currentWeaponInSlot + 1) + "/" +
                                          (slots[currentSlot].weaponsInInventory.Count);
                }
            }
            else
            {
                if (UpWeaponButton)
                    UpWeaponButton.gameObject.SetActive(false);

                if (DownWeaponButton)
                    DownWeaponButton.gameObject.SetActive(false);

                if (CurrWeaponText)
                    CurrWeaponText.gameObject.SetActive(false);
            }
        }


        public void DropWeapon()
        {
            if(controller.AdjustmentScene || !ActiveDropWeapon || !canDropWeapon || !hasAnyWeapon || slots[currentSlot].hideAllWeaponsSlot || slots[currentSlot].weaponsInInventory.Count <= 0 || 
               weaponController.IsAimEnabled || weaponController.IsReloadEnabled || creategrenade || !hasWeaponTaken)
                return;

            if (weaponController.DropWeaponAudio)
            {
                GetComponent<AudioSource>().PlayOneShot(weaponController.DropWeaponAudio);
            }

            Helper.ChangeLayersRecursively(weaponController.transform, "Default");

             weaponController = null;
             gun = null;

            var curIndex = slots[currentSlot].currentWeaponInSlot;

            var curWeapon = slots[currentSlot].weaponsInInventory[curIndex];

            curWeapon.weapon.GetComponent<WeaponController>().enabled = false;

            if (!curWeapon.weapon.GetComponent<PickUp>())
            {
                var pickUpScript = curWeapon.weapon.AddComponent<PickUp>();
                pickUpScript.PickUpType = PickUp.TypeOfPickUp.Weapon;
                pickUpScript.distance = 10;
                pickUpScript.Slots = currentSlot + 1;
                pickUpScript.Method = PickUp.PickUpMethod.Raycast;

                if (!controller.isMultiplayerCharacter)
                {
                    if (pickUpScript.pickUpId == null)
                    {
                        pickUpScript.pickUpId = Helper.GeneratePickUpId();
                        DropIdMultiplayer = pickUpScript.pickUpId;
                    }
                }
                else
                {
                    if (pickUpScript.pickUpId == null)
                    {
                        pickUpScript.pickUpId = DropIdMultiplayer;
                    }
                }
            }

            DropDirection = controller.TypeOfCamera != CharacterHelper.CameraType.TopDown ? 
                controller.thisCamera.transform.forward * 5 : controller.thisCamera.transform.up * 5;
            
            
            curWeapon.weapon.GetComponent<BoxCollider>().isTrigger = false;
            
            curWeapon.weapon.transform.parent = null;

            var rigidbody = curWeapon.weapon.GetComponent<Rigidbody>();
            
            rigidbody.velocity = DropDirection;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;

            foreach (var kit in slots[currentSlot].weaponsInInventory[curIndex].WeaponAmmoKits)
            {
                ReserveAmmo.Add(kit);
            }

            var id = slots[currentSlot].weaponsInInventory[curIndex].weapon.GetInstanceID();
            slots[currentSlot].weaponsInInventory.Remove(curWeapon);

            ChoiceNewWeapon(curIndex, "up", id);

            if (!controller.isMultiplayerCharacter)
            {
                hasWeaponDropped = true;
                canDropWeapon = false;
                StartCoroutine(DropTimeOut(curWeapon));
            }
        }
        
        void ChoiceNewWeapon(int curIndex, string type, int id)
        {
            if (slots[currentSlot].weaponsInInventory.Count > 0 || slots[currentSlot].hideAllWeaponsSlot)
            {
                if (slots[currentSlot].weaponsInInventory.Count > 0 && !slots[currentSlot].hideAllWeaponsSlot)
                {
                    if (type == "up")
                    {
                        curIndex++;
                        if (curIndex > slots[currentSlot].weaponsInInventory.Count - 1)
                            curIndex = 0;
                    }
                    else
                    {
                        curIndex--;
                        if (curIndex < 0)
                            curIndex = slots[currentSlot].weaponsInInventory.Count - 1;
                    }

                    slots[currentSlot].currentWeaponInSlot = curIndex;

                    if (!controller.isMultiplayerCharacter)
                        MultiplayerChangedWeapon = true;

                    Switch(currentSlot);
                }
                else if(slots[currentSlot].hideAllWeaponsSlot)
                {
                    hideAllWeapons = true;
                    SwitchNewWeapon();
                }
            }
            else
            {
                hasAnyWeapon = false;
                
                for (var i = 0; i < 8; i++)
                {
                    if (type == "up")
                    {
                        currentSlot++;
                        if (currentSlot > 7)
                            currentSlot = 0;
                    }
                    else
                    {
                        currentSlot--;
                        if (currentSlot < 0)
                            currentSlot = 7;
                    }

                    if (slots[currentSlot].hideAllWeaponsSlot)
                    {
                        hideAllWeapons = true;
                        SwitchNewWeapon();
                        break;
                    }

                    if (slots[currentSlot].weaponsInInventory.Count > 0)
                    {
                        if (id != slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].weapon.GetInstanceID())
                        {
                            if (!controller.isMultiplayerCharacter)
                                MultiplayerChangedWeapon = true;

                            Switch(currentSlot);
                            hasAnyWeapon = true;
                            break;
                        }

                        hasAnyWeapon = true;
                    }
                }

                if (!hasAnyWeapon)
                {
                    if (AmmoUI)
                        AmmoUI.gameObject.SetActive(false);
                    
                    //controller.anim.SetBool("NoWeapons", true);
                    
                    hideAllWeapons = true;
                    SwitchNewWeapon();
                    
//                    if (controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
//                        StartCoroutine(ChangeAnimatorLayers(0));
                }
            }

            ShowImage("weapon");
            SelectWeapon(currentSlot);
        }

        void ActivateInventory()
        {
            if (weaponController)
                if (weaponController.IsAimEnabled && weaponController.UseAimTexture ||
                    weaponController.IsReloadEnabled || creategrenade || !hasWeaponTaken)
                    return;

            if (controller.isPause || inventoryWheel.activeSelf || controller.AdjustmentScene)
                return;

            CheckInventoryButtons();

            Helper.ChangeButtonColor(this, currentSlot, "high");
            ShowImage("weapon");
            ShowImage("health");
            ShowImage("ammo");

            inventoryWheel.SetActive(true);

            controller.hasMoveButtonPressed = false;

            controller.isPause = true;
            
            if (!GamepadConnect)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

//            if (cameraController.crosshair)
//                cameraController.crosshair.gameObject.SetActive(false);

            if (Application.isMobilePlatform)
            {
                for (var i = 0; i < controller.uiButtons.Length; i++)
                {
                    if (i != 10)
                    {
                        if (controller.uiButtons[i])
                            controller.uiButtons[i].gameObject.SetActive(false);
                    }
                }
            }
            closeInventory = false;
        }

        void DeactivateInventory()
        {
            if (!inventoryWheel)
                return;

            inventoryWheel.SetActive(false);
            controller.isPause = false;

            Helper.ChangeButtonColor(this, currentSlot, "high");
            
            Helper.ChangeColor(AmmoButton, normButtonsColors[9], normButtonsSprites[9]);
            Helper.ChangeColor(HealthButton, normButtonsColors[8], normButtonsSprites[8]);

            if (Application.isMobilePlatform)
            {
                for (var i = 0; i < controller.uiButtons.Length; i++)
                {
                    var button = controller.uiButtons[i];
                    if (button)
                        button.gameObject.SetActive(true);
                }
            }

            if (weaponController)
            {
                if (weaponController.IsAimEnabled)
                {
                    weaponController.Aim(true, false);
                    StartCoroutine("SwitchWeaponTimeOut");
                }
                else
                {
                    SwitchNewWeapon();
                }
            }
        }

        public void SwitchNewWeapon()
        {
            if (hasWeaponChanged)
            {
                if (!controller.isMultiplayerCharacter)
                {
                    MultiplayerChangedWeapon = true;
                }

                Switch(currentSlot);
                hasWeaponChanged = false;
            }
            else if(hideAllWeapons)
            {
                if (!controller.isMultiplayerCharacter)
                    MultiplayerHideAllWeapons = true;
                
                if (AmmoUI)
                    AmmoUI.gameObject.SetActive(false);
                    
                Null_Weapons();
                controller.anim.SetBool("NoWeapons", true);
                
                if (controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                    StartCoroutine(ChangeAnimatorLayers(0));
                
                hideAllWeapons = false;
                hasAnyWeapon = false;
            }
        }
        

        IEnumerator ChangeAnimatorLayers(int value)
        {
            while (true)
            {
                controller.anim.SetLayerWeight(3, Mathf.Lerp(controller.anim.GetLayerWeight(3), value, 10 * Time.deltaTime));
                
                    if (Math.Abs(controller.anim.GetLayerWeight(3) - value) < 0.1f)
                    {
                        controller.anim.SetLayerWeight(3, value);
                        StopCoroutine("ChangeAnimatorLayers");
                        break;
                    }

                    yield return 0;
            }
        }

        IEnumerator SwitchWeaponTimeOut()
        {
            yield return new WaitForSeconds(0.5f);
            SwitchNewWeapon();
            StopCoroutine("SwitchWeaponTimeOut");
        }
        
        void ShowImage(string type)
        {
            switch (type)
            {
                case "weapon":
                {
                    for (var i = 0; i < 8; i++)
                    {
                        if (slots[i].weaponsInInventory.Count <= 0 && !slots[i].hideAllWeaponsSlot)
                        {
                            var slotButton = slots[i].SlotButton;

                            if (!slotButton)
                                continue;

                            slotButton.interactable = false;

                            Helper.ChangeButtonColor(this, i, "norm");

                            if (slots[i].SlotImage)
                            {
                                var img = slots[i].SlotImage;
                                img.color = new Color(1, 1, 1, 0);
                            }

                            if (slots[i].curAmmoText)
                                slots[i].curAmmoText.gameObject.SetActive(false);

                            continue;
                        }

                        if (slots[i].SlotButton)
                            slots[i].SlotButton.interactable = true;

                        if (!slots[i].hideAllWeaponsSlot)
                        {
                            var weaponController = slots[i].weaponsInInventory[slots[i].currentWeaponInSlot].weapon
                                .GetComponent<WeaponController>();

                            if (!weaponController.WeaponImage || !slots[i].SlotImage)
                                continue;

                            var image = slots[i].SlotImage;

                            image.texture = weaponController.WeaponImage;

                            image.color = new Color(1, 1, 1, 1);

                            if (slots[i].curAmmoText)
                            {
                                slots[i].curAmmoText.gameObject.SetActive(true);
                                if (weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Knife)
                                {
                                    slots[i].curAmmoText.text = weaponController.Attacks[weaponController.currentAttack].curAmmo.ToString("F0") + "/" +
                                                                weaponController.Attacks[weaponController.currentAttack].inventoryAmmo;
                                }
                                else
                                {
                                    slots[i].curAmmoText.text = "Knife";
                                }
                            }
                        }
                        else
                        {
                            if (!slots[i].SlotImage)
                                continue;
                            
                            var image = slots[i].SlotImage;

                            image.texture = HideAllWeaponImage;

                            image.color = new Color(1, 1, 1, 1);
                            
                            slots[i].curAmmoText.text = " ";
                        }
                    }

                    break;
                }
                case "health":

                    if (HealthKits.Count > 0)
                    {
                        if (HealthButton)
                            HealthButton.interactable = true;

                        if (UpHealthButton)
                            UpHealthButton.interactable = true;

                        if (DownHealthButton)
                            DownHealthButton.interactable = true;

                        if (currHealthKitCount)
                            currHealthKitCount.text = currentHealthKit + 1 + "/" + HealthKits.Count;

                        if (currentHealthKitAddedValue)
                        {
                            currentHealthKitAddedValue.gameObject.SetActive(true);
                            currentHealthKitAddedValue.text = "+ " + HealthKits[currentHealthKit].AddedValue;
                        }

                        if (HealthImage)
                            HealthImage.color = new Color(1, 1, 1, 1);
                    }
                    else
                    {
                        if (HealthButton)
                            HealthButton.interactable = false;

                        if (UpHealthButton)
                            UpHealthButton.interactable = false;

                        if (DownHealthButton)
                            DownHealthButton.interactable = false;

                        if (currHealthKitCount)
                            currHealthKitCount.text = "0";

                        if (currentHealthKitAddedValue)
                            currentHealthKitAddedValue.gameObject.SetActive(false);

                        if (HealthImage)
                            HealthImage.color = new Color(1, 1, 1, 0);
                    }

                    foreach (var kit in HealthKits)
                    {
                        if (HealthKits.IndexOf(kit) == currentHealthKit)
                            if (HealthImage)
                                HealthImage.texture = kit.Image;
                    }

                    break;

                case "ammo":

                    if (slots[currentSlot].weaponsInInventory.Count > 0)
                    {
                        var weaponController = slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();
                            
                        if (slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count > 0 &&
                            weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Knife)
                        {

                            if (AmmoButton)
                                AmmoButton.interactable = true;

                            if (UpAmmoButton)
                                UpAmmoButton.interactable = true;

                            if (DownAmmoButton)
                                DownAmmoButton.interactable = true;

                            if (currAmmoKitCount)
                                currAmmoKitCount.text = currentAmmoKit + 1 + "/" + slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count;

                            if (currentAmmoKitAddedValue)
                            {
                                currentAmmoKitAddedValue.gameObject.SetActive(true);
                                currentAmmoKitAddedValue.text = "+ " + slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits[currentAmmoKit].AddedValue;
                            }


                            if (AmmoImage)
                                AmmoImage.color = new Color(1, 1, 1, 1);
                            AmmoImage.texture = slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits[currentAmmoKit].Image;

                        }
                        else
                        {
                            NotActiveAmmoKits();
                        }
                    }
                    else
                    {
                        NotActiveAmmoKits();
                    }

                    break;
            }
        }

        private void NotActiveAmmoKits()
        {
            if (AmmoButton)
                AmmoButton.interactable = false;

            if (UpAmmoButton)
                UpAmmoButton.interactable = false;

            if (DownAmmoButton)
                DownAmmoButton.interactable = false;

            if (currAmmoKitCount)
                currAmmoKitCount.text = "0";

            if (currentAmmoKitAddedValue)
                currentAmmoKitAddedValue.gameObject.SetActive(false);

            if (AmmoImage)
                AmmoImage.color = new Color(1, 1, 1, 0);
        }

        private void SelectWeapon(int slot)
        {
            if (slots[slot].weaponsInInventory.Count <= 0 && !slots[slot].hideAllWeaponsSlot)
                return;

            if (!slots[slot].hideAllWeaponsSlot)
            {
                weaponId = slots[slot].weaponsInInventory[slots[slot].currentWeaponInSlot].weapon.GetInstanceID();

                hideAllWeapons = false;

                if (hasAnyWeapon)
                {
                    hasWeaponChanged = gun.GetInstanceID() != weaponId & slots[slot].weaponsInInventory.Count > 0 &
                                       slots[slot].weaponsInInventory[slots[slot].currentWeaponInSlot].weapon;
                }
                else hasWeaponChanged = true;
            }
            else
            {
                hideAllWeapons = true;
                hasWeaponChanged = false;
            }

            if (!GamepadConnect)
            {
                if (currentSlot != slot)
                {
                    Helper.ChangeButtonColor(this, currentSlot, "norm");
                }
            }

            currentSlot = slot;

            Helper.ChangeButtonColor(this, currentSlot, "high");
        }

        private void UpInventoryValue(string type)
        {
            switch (type)
            {
                case "weapon":
                {
                    var curWeapon = slots[currentSlot].currentWeaponInSlot;
                    curWeapon++;

                    if (curWeapon > slots[currentSlot].weaponsInInventory.Count - 1)
                        curWeapon = 0;

                    slots[currentSlot].currentWeaponInSlot = curWeapon;

                    ShowImage("weapon");
                    SelectWeapon(currentSlot);
                    break;
                }
                case "health":

                    var curKit = currentHealthKit;
                    curKit++;
                    if (curKit > HealthKits.Count - 1)
                        curKit = 0;
                    currentHealthKit = curKit;

                    ShowImage("health");
                    break;

                case "ammo":

                    curKit = currentAmmoKit;
                    curKit++;
                    if (curKit > slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot]
                            .WeaponAmmoKits.Count - 1)
                        curKit = 0;
                    currentAmmoKit = curKit;

                    ShowImage("ammo");
                    break;
            }
        }

        void DownInventoryValue(string type)
        {
            switch (type)
            {
                case "weapon":
                {
                    var curWeapon = slots[currentSlot].currentWeaponInSlot;
                    curWeapon--;

                    if (curWeapon < 0)
                        curWeapon = slots[currentSlot].weaponsInInventory.Count - 1;

                    slots[currentSlot].currentWeaponInSlot = curWeapon;

                    ShowImage("weapon");
                    SelectWeapon(currentSlot);
                    break;
                }
                case "health":

                    var curKit = currentHealthKit;
                    curKit--;
                    if (curKit < 0)
                        curKit = HealthKits.Count - 1;
                    currentHealthKit = curKit;

                    ShowImage("health");
                    break;
                case "ammo":
                    curKit = currentAmmoKit;
                    curKit--;
                    if (curKit < 0)
                        curKit = slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot]
                                     .WeaponAmmoKits.Count - 1;
                    currentAmmoKit = curKit;

                    ShowImage("ammo");
                    break;
            }
        }

        void UseKit(string type)
        {
            switch (type)
            {
                case "health":
                    if (HealthKits.Count <= 0)
                        return;

                    controller.PlayerHealth += HealthKits[currentHealthKit].AddedValue;
                    HealthKits.Remove(HealthKits[currentHealthKit]);
                    var curIndex = currentHealthKit;
                    curIndex++;
                    if (curIndex > HealthKits.Count - 1)
                        curIndex = 0;
                    currentHealthKit = curIndex;
                    ShowImage("health");
                    MultiplayerUseHealth = true;
                    
                    break;
                case "ammo":
                    if (slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count <= 0)
                        return;

                    var ammoKit = slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits[currentAmmoKit];
                    var weaponController = slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();

                    foreach (var attack in weaponController.Attacks)
                    {
                        if (attack.AmmoType == ammoKit.ammoType)
                        {
                            weaponController.Attacks[weaponController.currentAttack].inventoryAmmo += ammoKit.AddedValue;
                            break;
                        }
                    }
                    
                    for (var i = 0; i < 8; i++)
                    {
                        foreach (var weapon in slots[i].weaponsInInventory)
                        {
                            if (weapon.WeaponAmmoKits.Exists(x => x.PickUpId == ammoKit.PickUpId))
                            {
                                var kit = weapon.WeaponAmmoKits.Find(x => x.PickUpId == ammoKit.PickUpId);
                                weapon.WeaponAmmoKits.Remove(kit);
                            }
                        }
                    }

                    curIndex = currentAmmoKit;
                    curIndex++;
                    if (curIndex > slots[currentSlot].weaponsInInventory[slots[currentSlot].currentWeaponInSlot]
                            .WeaponAmmoKits.Count - 1)
                        curIndex = 0;
                    currentAmmoKit = curIndex;

                    ShowImage("ammo");
                    break;
            }
        }

        void CheckPickUp()
        {
            if (controller.isPause) //|| this.cameraController.aim)
                return;

            if (weaponController)
                if (controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && weaponController.IsAimEnabled || weaponController.IsReloadEnabled || creategrenade ||
                    !hasWeaponTaken)
                {
                    isPickUp = false;
                    return;
                }

            var Hit = new RaycastHit();

            if (controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
            {
                var direction = controller.thisCamera.transform.TransformDirection(Vector3.forward);
                if (!Physics.Raycast(controller.thisCamera.transform.position, direction, out Hit, 100)) return;

            }
            else
            {
                if (!Physics.Raycast(controller.BodyObjects.Head.position + transform.forward * 2, Vector3.down * 3, out Hit,
                    100)) return;
            }

            if (Hit.collider.GetComponent<PickUp>())
            {
                if (!Hit.collider.GetComponent<PickUp>().isActiveAndEnabled) return;
                
                var pickUp = Hit.collider.GetComponent<PickUp>();

                if (pickUp.Method == PickUp.PickUpMethod.Raycast)
                {
                    if (Hit.distance <= pickUp.distance)
                    {
                        isPickUp = true;

                        if (Input.GetKeyDown(controller._gamepadCodes[8]) || Input.GetKeyDown(controller._keyboardCodes[8]) || pickUpUiButton ||
                            Helper.CheckGamepadAxisButton(8, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown", controller.inputs.AxisButtonValues[8]))
                        {
                            pickUp.PickUpObject(gameObject);
                            currentPickUpId = pickUp.pickUpId;
                            isMultiplayerPickUp = true;
                            pickUpUiButton = false;
                        }
                    }
                    else
                    {
                        isPickUp = false;
                    }
                }
            }
            else
            {
                isPickUp = false;
            }

        }
        
        void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PickUp>())
            {
                var pickUp = other.GetComponent<PickUp>();
                if (pickUp.Method == PickUp.PickUpMethod.Collider && pickUp.enabled)
                {
                    pickUp.PickUpObject(gameObject);
                    currentPickUpId = pickUp.pickUpId;
                    isMultiplayerPickUp = true;
                }
            }
        }

        public void Null_Weapons()
        {
            for (var i = 0; i < 8; i++)
            {
                foreach (var weapon in slots[i].weaponsInInventory)
                {
                    weapon.weapon.SetActive(false);
                }
            }
        }

        public void Switch(int slot)
        {
            StopCoroutine("TakeWeapon");

            Null_Weapons();
            slots[slot].weaponsInInventory[slots[slot].currentWeaponInSlot].weapon.SetActive(true);
            weaponController = slots[slot].weaponsInInventory[slots[slot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();
            weaponController.canAttack = false;
            
            weaponController.Controller = controller;
            weaponController.characterAnimations.anim = controller.GetComponent<Animator>();
            
            weaponController.CurrentWeaponInfo.Clear();

            for (var i = 0; i < weaponController.WeaponInfos.Count; i++)
            {
                var info = new WeaponsHelper.WeaponInfo();
                info.Clone(weaponController.WeaponInfos[i]);
                weaponController.CurrentWeaponInfo.Add(info);
            }
            
            ResetAnimatorParameters();
            SetWeaponAnimations(false);

            if (controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson && !controller.isMultiplayerCharacter
                && (!controller.isCrouch && !weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState ||
                controller.isCrouch && !weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState))
                StartCoroutine(ChangeAnimatorLayers(1));

            gun = slots[slot].weaponsInInventory[slots[slot].currentWeaponInSlot].weapon;

            Helper.ChangeLayersRecursively(gun.transform, "Character");

            if (weaponController)
            {
                if (weaponController.UseScope)
                    if (weaponController.ScopeScreen)
                    {
                        weaponController.ScopeScreen.GetComponent<MeshRenderer>().material.mainTexture = ScopeScreenTexture;
                    }

                if (weaponController.UseAimTexture)
                    aimTextureImage.texture = weaponController.AimCrosshairTexture;
            }


//            if (controller.isCrouch && controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
//                weaponController.setCrouchHands = true;
            
//            var rigidbody = gun.GetComponent<Rigidbody>();
//            rigidbody.useGravity = false;
//            rigidbody.isKinematic = true;
//            gun.GetComponent<BoxCollider>().isTrigger = true;
            
            firstLayerSet = false;
            hasWeaponTaken = false;
            
            if (!controller.AdjustmentScene)
            {
                controller.anim.Play("Take Weapon", 2);
                controller.anim.Play("Take Weapon", 3);
                StartCoroutine("TakeWeapon");
            }
            else
            {
                controller.anim.Play("Idle", 2);
                controller.anim.Play("Idle", 3);
            }
            

            controller.anim.SetBool("CanWalkWithWeapon", true);

            controller.anim.SetBool("NoWeapons", false);
            
            hasAnyWeapon = true;
            
            if (controller.isMultiplayerCharacter)
                return;

            if (!controller.AdjustmentScene && AmmoUI && !controller.AdjustmentScene)
                AmmoUI.gameObject.SetActive(true);
            
            if(controller.thisCameraScript.CameraAim)
                controller.thisCameraScript.Aim();
        }

        public void ResetAnimatorParameters()
        {
            foreach (var parameter in controller.anim.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    if (parameter.name == "Aim" || parameter.name == "TakeWeapon" || parameter.name == "Attack" || parameter.name == "Reload" 
                        || parameter.name == "Grenade" || parameter.name == "HasWeaponTaken" || parameter.name == "CanWalkWithWeapon" || parameter.name == "NoWeapons") 
                        controller.anim.SetBool(parameter.name, parameter.name == "NoWeapons");
                }

            }
        }

        public void SetWeaponAnimations(bool changeAttack)
        {
            if (weaponController.Attacks[weaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets)
            {
                if (weaponController.Attacks[weaponController.currentAttack].currentBulletType == 0)
                {
                    if (weaponController.Attacks[weaponController.currentAttack].WeaponAttack)
                        controller.ClipOverrides["WeaponAttack"] = weaponController.Attacks[weaponController.currentAttack].WeaponAttack;
                    else
                        Debug.LogWarning("<color=yellow>Missing Component</color> [Single Shoot] animation.", weaponController.gameObject);
                }
                else
                {
                    if (weaponController.Attacks[weaponController.currentAttack].WeaponAutoShoot)
                        controller.ClipOverrides["WeaponAttack"] = weaponController.Attacks[weaponController.currentAttack].WeaponAutoShoot;
                    else
                        Debug.LogWarning("<color=yellow>Missing Component</color> [Auto Shoot] animation.", weaponController.gameObject);
                }
            }
            else
            {
                if (weaponController.Attacks[weaponController.currentAttack].WeaponAttack)
                    controller.ClipOverrides["WeaponAttack"] = weaponController.Attacks[weaponController.currentAttack].WeaponAttack;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon attack] animation.", weaponController.gameObject);
            }

            if (weaponController.characterAnimations.WeaponIdle)
                controller.ClipOverrides["WeaponIdle"] = weaponController.characterAnimations.WeaponIdle;
            else
                Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon idle] animation.",
                    weaponController.gameObject);

            if (weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Knife)
            {
                if (weaponController.Attacks[weaponController.currentAttack].WeaponReload)
                    controller.ClipOverrides["WeaponReload"] = weaponController.Attacks[weaponController.currentAttack].WeaponReload;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon reload] animation.",
                        weaponController.gameObject);
            }

            if (!changeAttack)
            {
                if (weaponController.characterAnimations.WeaponWalk)
                    controller.ClipOverrides["WeaponWalk"] = weaponController.characterAnimations.WeaponWalk;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon walk] animation.",
                        weaponController.gameObject);

                if (weaponController.characterAnimations.WeaponRun)
                    controller.ClipOverrides["WeaponRun"] = weaponController.characterAnimations.WeaponRun;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon run] animation.",
                        weaponController.gameObject);

                if (weaponController.characterAnimations.TakeWeapon)
                    controller.ClipOverrides["TakeWeapon"] = weaponController.characterAnimations.TakeWeapon;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Take weapon] animation.",
                        weaponController.gameObject);
            }

            controller.newController.ApplyOverrides(controller.ClipOverrides);

            StartCoroutine("SetAnimParameters");
        }

        public void FindWeapons()
        {
            hasAnyWeapon = false;
            
            var adj = FindObjectOfType<Adjustment>();
            
            if (adj)
            {
                controller.AdjustmentScene = true;
                return;
            }
            
            var allWeapons = new List<GameObject>();

            for (var i = 0; i < 8; i++)
            {
                foreach (var slot in slots[i].weaponSlots)
                {
                    var weapon = slot.weapon;

                    if (!weapon) continue;
                    if (!weapon.GetComponent<WeaponController>()) continue;

                    var instantiatedWeapon = Instantiate(weapon);

                    slots[i].currentWeaponInSlot = 0;
                    slots[i].weaponsInInventory.Add(new Weapon() {weapon = instantiatedWeapon, WeaponAmmoKits = new List<Kit>()});
                    slot.weapon = instantiatedWeapon;
                    allWeapons.Add(instantiatedWeapon);

                    hasAnyWeapon = true;

                    WeaponsHelper.SetWeaponController(instantiatedWeapon, weapon, this, controller, transform);
                }
            }
        }

        #region Grenades

        public void LaunchGrenade()
        {
            if (weaponController)
                if (weaponController.IsReloadEnabled)
                    return;

            if (controller.isPause || controller.isCrouch)
                return;
            
            if(!controller.isMultiplayerCharacter)
                MultiplayerLaunchGrenade = true;

            if (weaponController)
            {
                if (weaponController.IsAimEnabled)
                {
                    weaponController.Aim(true, true);
                    StartCoroutine("GrenadeTimeout");
                }
                else
                {
                    BeginningLaunchingGrenade();
                }
            }
            else
            {
                BeginningLaunchingGrenade();
            }

        }

        public void BeginningLaunchingGrenade()
        {
            if (Grenades.Count > 0)
            {
                if (!(Grenades[currentGrenade].grenadeAmmo > 0 && canLaunchGrenadeAgain)) return;


                //                if (!GrenadePrefabs.GetComponent<Grenade>())
                //                    Debug.LogError(
                //                        "(Weapon Manager) The script [Grenade] for the grenade was not found. Add it, otherwise the grenade won't explode",
                //                        GrenadePrefabs);

//                if (controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
//                {
//                    if (Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeThrow_FPS)
//                    {
//                        controller.ClipOverrides["Grenade"] = Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeThrow_FPS;
//                        controller.newController.ApplyOverrides(controller.ClipOverrides);
//                    }
//                    else
//                    {
//                        Debug.LogWarning("<color=yellow>Missing Component</color> [GrenadeThrow FPS] animation", gameObject);
//                        return;
//                    }
//                }
//                else
//                {
//                    if (Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeThrow_TPS_TDS)
//                    {
//                        controller.ClipOverrides["Grenade"] = Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeThrow_TPS_TDS;
//                        controller.newController.ApplyOverrides(controller.ClipOverrides);
//                    }
//                    else
//                    {
//                        Debug.LogWarning("<color=yellow>Missing Component</color> [GrenadeThrow TPS/TDS] animation", gameObject);
//                        return;
//                    }
//                }


                controller.anim.SetBool("Grenade", true);

                if (hasAnyWeapon)
                    StartCoroutine("LaunchGrenadeTimeOut");

                
                StartCoroutine("DisableWeaponLayers");
            }
            else
            {
                Debug.LogError("(Weapon Manager) <color=red>Missing component</color>: [Grenade_prefab]. Add it, otherwise the grenade won't be created.", gameObject);
                Debug.Break();
            }
        }
        

        IEnumerator DisableWeaponLayers()
        {
            while (true)
            {
                if (controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                {
                    SmoothIKSwitch = 0;
                    controller.anim.SetLayerWeight(3, Mathf.Lerp(controller.anim.GetLayerWeight(3), 0, 6 * Time.deltaTime));
                    
                    if (Math.Abs(controller.anim.GetLayerWeight(3)) < 0.1f)
                    {
                        controller.anim.SetLayerWeight(3, 0);
                        
                        //if(Mathf.Abs(controller.anim.GetFloat("CameraAngle")) < 45)
                        controller.anim.SetBool("LaunchGrenade", true);

                        if (controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Grenade_Throw"))
                        {
                            Grenades[currentGrenade].grenadeAmmo -= 1;
                            creategrenade = true;
                            canLaunchGrenadeAgain = false;
                            
                            StartCoroutine("CreateGrenade");
                            StopCoroutine("DisableWeaponLayers");
                            break;
                        }
                    }
                }
                else
                {
                    StartCoroutine("CreateGrenade");
                    StopCoroutine("DisableWeaponLayers");
                    break;
                }
                
                yield return 0;
            }
        }

        IEnumerator ActivateWeaponLayers()
        {
            while (true)
            {
                if (!controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState || 
                    controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState)
                {
                    StopCoroutine("ActivateWeaponLayers");
                    break;
                }
                controller.anim.SetLayerWeight(3, Mathf.Lerp(controller.anim.GetLayerWeight(3), 1, 2 * Time.deltaTime));
                if (Math.Abs(controller.anim.GetLayerWeight(3) - 1) < 0.1f)
                {
                    controller.anim.SetLayerWeight(3, 1);
                    StopCoroutine("ActivateWeaponLayers");
                    break;
                }

                yield return 0;
            }
        }

        IEnumerator GrenadeTimeout()
        {
            while (true)
            {
                if (controller.anim.GetBool("CanWalkWithWeapon"))
                {
                    BeginningLaunchingGrenade();
                    StopCoroutine("GrenadeTimeout");
                }

                yield return 0;
            }
        }

        IEnumerator CreateGrenade()
        {
            yield return new WaitForSeconds(controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson ? 
                Grenades[currentGrenade].GrenadeScript.WeaponInfos[ Grenades[currentGrenade].GrenadeScript.SettingsSlotIndex].timeBeforeCreating_FPS :
                Grenades[currentGrenade].GrenadeScript.WeaponInfos[ Grenades[currentGrenade].GrenadeScript.SettingsSlotIndex].timeBeforeCreating_TPS - 0.25f);
            
            var curWeaponInfo = Grenades[currentGrenade].GrenadeScript.WeaponInfos[Grenades[currentGrenade].GrenadeScript.SettingsSlotIndex];

            grenadeClone = Instantiate(Grenades[currentGrenade].Grenade, curWeaponInfo.WeaponPosition, Quaternion.Euler(curWeaponInfo.WeaponRotation),
                controller.BodyObjects.RightHand);

            Helper.ChangeLayersRecursively(grenadeClone.transform, "Character");

            if (curWeaponInfo.WeaponSize != Vector3.zero)
                grenadeClone.transform.localScale = curWeaponInfo.WeaponSize;

            //takeGrenade = true;
            grenadeController = grenadeClone.GetComponent<WeaponController>();

            WeaponsHelper.PlaceWeapon(Grenades[currentGrenade].GrenadeScript.WeaponInfos[Grenades[currentGrenade].GrenadeScript.SettingsSlotIndex], grenadeClone.transform);

            StartCoroutine("GrenadeInHand");

            if (hasAnyWeapon)
                controller.anim.SetBool("CanWalkWithWeapon", false);

            var grenadeScript = grenadeController;
            grenadeScript.enabled = false;
            //grenadeScript.SettingsSlotIndex = Grenades[currentGrenade].saveSlotIndex;

            grenadeClone.GetComponent<Rigidbody>().useGravity = false;

            if (controller.BodyObjects.LeftHand && grenadeClone.transform.parent != controller.BodyObjects.LeftHand)
            {
                grenadeClone.transform.parent = controller.BodyObjects.LeftHand;
                grenadeClone.transform.localPosition = Vector3.zero;
            }

            StartCoroutine("LaunchingGrenade");
            StopCoroutine("CreateGrenade");


        }

        IEnumerator LaunchGrenadeTimeOut()
        {
            yield return new WaitForSeconds(Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeThrow_FPS
                ? Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeThrow_FPS.length : 5);
            StartCoroutine(weaponController.WalkWithWeaponTimeout());
            StopCoroutine(LaunchGrenadeTimeOut());
        }

        IEnumerator GrenadeInHand()
        {
            while (true)
            {
                grenadeClone.transform.localPosition =
                    grenadeController.WeaponInfos[grenadeController.SettingsSlotIndex].WeaponPosition;
                grenadeClone.transform.localEulerAngles =
                    grenadeController.WeaponInfos[grenadeController.SettingsSlotIndex].WeaponRotation;
                yield return 0;
            }
        }

        IEnumerator LaunchingGrenade()
        {
            yield return new WaitForSeconds(controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson ? 
                Grenades[currentGrenade].GrenadeScript.WeaponInfos[grenadeController.SettingsSlotIndex].timeInHand_FPS :
                Grenades[currentGrenade].GrenadeScript.WeaponInfos[grenadeController.SettingsSlotIndex].timeInHand_TPS - 0.1f);

            StopCoroutine("GrenadeInHand");
            if (Grenades[currentGrenade].GrenadeScript.GrenadeParameters.ThrowAudio)
                grenadeClone.GetComponent<AudioSource>().PlayOneShot(Grenades[currentGrenade].GrenadeScript.GrenadeParameters.ThrowAudio);
            else Debug.LogWarning("(Weapon Manager) <color=yellow>Missing</color> grenade throw audio");

            controller.anim.SetBool("Grenade", false);
            controller.anim.SetBool("LaunchGrenade", false);

            if (controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                StartCoroutine("ActivateWeaponLayers");

            //takeGrenade = false;
            grenadeController = null;

            grenadeClone.GetComponent<Rigidbody>().useGravity = true;
            grenadeClone.GetComponent<Rigidbody>().isKinematic = false;

            Helper.ChangeLayersRecursively(grenadeClone.transform, "Default");

            var grenadeScript = grenadeClone.GetComponent<WeaponController>();
            grenadeScript.enabled = true;
            grenadeScript.StartCoroutine("GrenadeFlying");
            grenadeScript.Controller = controller;

            StartCoroutine("TimeoutAfterLaunchGrenade");

            grenadeClone.transform.parent = null;

            if (Mathf.Abs(controller.anim.GetFloat("CameraAngle")) < 45)
            {
                switch (controller.TypeOfCamera)
                {
                    case CharacterHelper.CameraType.FirstPerson:
                        grenadeClone.GetComponent<Rigidbody>().velocity = controller.thisCamera.transform.TransformDirection(
                            Vector3.forward * Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeSpeed);
                        break;
                    case CharacterHelper.CameraType.ThirdPerson:
                        grenadeClone.GetComponent<Rigidbody>().velocity = controller.thisCamera.transform.TransformDirection(
                            Vector3.forward * Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeSpeed);
                        break;
                    case CharacterHelper.CameraType.TopDown:
                        grenadeClone.GetComponent<Rigidbody>().velocity = controller.thisCamera.transform.TransformDirection(
                            Vector3.up * Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeSpeed);
                        break;
                }
            }
            else
            {
                grenadeClone.GetComponent<Rigidbody>().velocity = transform.forward * Grenades[currentGrenade].GrenadeScript.GrenadeParameters.GrenadeSpeed;
            }

            creategrenade = false;
            StopCoroutine("LaunchingGrenade");
        }

        IEnumerator TimeoutAfterLaunchGrenade()
        {
            yield return new WaitForSeconds(2);
            canLaunchGrenadeAgain = true;
            StopCoroutine("TimeoutAfterLaunchGrenade");
        }

        #endregion

        IEnumerator TakeWeapon()
        {
            var time = 5f;
            if (controller.isCrouch ||// && !weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState ||
                !controller.isCrouch && !weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState)
                time = weaponController.characterAnimations.TakeWeapon.length;

            else if (!controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState)
                time = weaponController.characterAnimations.TakeWeapon.length / 2;
            
            yield return new WaitForSeconds(time);
            hasWeaponTaken = true;
            SmoothIKSwitch = 0;
            
            if (controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState ||
                !controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState)
            {
                firstLayerSet = true;
            }
            else
            {
                firstLayerSet = false;
            }
            
            controller.anim.SetBool("HasWeaponTaken", false);
            StartCoroutine("ShootingTimeout"); 
            StopCoroutine("TakeWeapon");
        }

        IEnumerator DropTimeOut(Weapon curWeapon)
        {
            yield return new WaitForSeconds(1);
            curWeapon.weapon.GetComponent<PickUp>().enabled = true;
            canDropWeapon = true;
            StopCoroutine("DropTimeOut");

        }

        IEnumerator ShootingTimeout()
        {
            yield return new WaitForSeconds(1);
            if (weaponController)
                weaponController.canAttack = true;
            StopCoroutine("ShootingTimeout");
        }

        IEnumerator SetAnimParameters()
        {
            yield return new WaitForSeconds(0.01f);
            controller.anim.SetBool("TakeWeapon", true);

            StopCoroutine("SetAnimParameters");
        }


        #region MobileUI

        public void UIAttack()
        {
            weaponController.UIButtonAttack = true;
        }

        public void UIEndAttack()
        {
            weaponController.UIButtonAttack = false;
        }

        public void UIAim()
        {
            weaponController.Aim(false, false);
        }

        public void UIReload()
        {
            weaponController.Reload();
        }

        public void UIChangeAttackType()
        {
            weaponController.ChangeAttack();
        }

        public void UIPickUp()
        {
            pickUpUiButton = true;
        }

        public void UIInventory()
        {
            if (!inventoryWheel) return;

            if (inventoryWheel.activeSelf)
            {
                DeactivateInventory();
            }
            else
            {
                ActivateInventory();
            }
        }

        public void UIActivateInventory()
        {
            pressedUIInventoryButton = true;
        }

        public void UIDeactivateInventory()
        {
            pressedUIInventoryButton = false;
        }

        #endregion
        

        #region HandsIK

        private void OnAnimatorIK(int layerIndex)
        {
            if (weaponController && hasAnyWeapon)
            {
                if (grenadeController)
                {
                    Helper.FingersRotate(grenadeController.WeaponInfos[grenadeController.SettingsSlotIndex], controller.anim, "Grenade");
                }
                else if (weaponController.IsReloadEnabled)
                {
                    Helper.FingersRotate(null, controller.anim, "Null");
                }
                else
                {
                    Helper.FingersRotate(weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex], controller.anim, "Weapon");
                }

                if (!weaponController.ActiveDebug)
                {
                    if (weaponController.IsReloadEnabled || controller.anim.GetBool("Grenade") ||
                        !controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState &&
                        !weaponController.IsAimEnabled && !weaponController.wallDetect ||
                        controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState 
                                            && !weaponController.IsAimEnabled && !weaponController.wallDetect)
                    {
                        if (SmoothIKSwitch > 0)
                            SmoothIKSwitch -= 1 * Time.deltaTime;
                        else
                        {
                            SmoothIKSwitch = 0;
                            controller.anim.SetBool("HasWeaponTaken", false);
                        }
                    }
                    else
                    {
                        if (SmoothIKSwitch < 1)
                        {
                            SmoothIKSwitch += 1 * Time.deltaTime;
                        }
                        else
                        {
                            firstLayerSet = true;
                            SmoothIKSwitch = 1;
                            controller.anim.SetBool("HasWeaponTaken", true);
                        }
                    }

                    if (!controller.isMultiplayerCharacter)
                    {
                        if (weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState ||
                            weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState)
                        {
                            if (setWeaponLayer && firstLayerSet)
                            {
                                controller.anim.SetLayerWeight(3, SmoothIKSwitch);
                            }
                        }

                        if (controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson &&
                            !controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState ||
                            controller.isCrouch && weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState)
                        {
                            if (weaponController.IsReloadEnabled || !hasWeaponTaken)
                            {
                                setWeaponLayer = false;
                                controller.anim.SetLayerWeight(3, Mathf.Lerp(controller.anim.GetLayerWeight(3), 1, 2 * Time.deltaTime));
                            }
                            else if (!weaponController.IsReloadEnabled && !setWeaponLayer && hasWeaponTaken)
                            {
                                if (!weaponController.IsAimEnabled)
                                {
                                    controller.anim.SetLayerWeight(3, Mathf.Lerp(controller.anim.GetLayerWeight(3), 0, 2 * Time.deltaTime));

                                    if (Math.Abs(controller.anim.GetLayerWeight(3)) < 0.1f)
                                        setWeaponLayer = true;
                                }
                                else
                                {
                                    if (SmoothIKSwitch > 0.9f)
                                        setWeaponLayer = true;
                                }
                            }
                        }
                    }


                    if (layerIndex == controller.currentAnimatorLayer)
                    {
                        if (weaponController.IkObjects.RightObject & weaponController.IkObjects.LeftObject)
                        {
                            if (weaponController.CanUseIK & hasWeaponTaken)
                            {
                                Helper.HandIK(controller, weaponController, this, weaponController.IkObjects.LeftObject,
                                    weaponController.IkObjects.RightObject, controller.BodyObjects.LeftHand, controller.BodyObjects.RightHand,
                                    SmoothIKSwitch);
                                
                                if(weaponController.IsAimEnabled)
                                {
                                    if (controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson 
                                        && weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Knife)
                                    {
                                        SmoothHeadIKSwitch = Mathf.Lerp(SmoothHeadIKSwitch, 1, 0.7f * Time.deltaTime);
                                        controller.anim.SetLookAtWeight(SmoothHeadIKSwitch);
                                    }
                                }
                                else
                                {
                                    SmoothHeadIKSwitch = Mathf.Lerp(SmoothHeadIKSwitch, 0, 5 * Time.deltaTime);
                                    controller.anim.SetLookAtWeight(SmoothHeadIKSwitch);
                                }
                                
                                controller.anim.SetLookAtPosition(weaponController.transform.position);
                            }
                        }
                    }
                }
                else if (weaponController.ActiveDebug)
                {
                    // Elbows rotation 
                    controller.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
                    controller.anim.SetIKHintPosition(AvatarIKHint.LeftElbow,
                        weaponController.IkObjects.LeftElbowObject.position);

                    controller.anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
                    controller.anim.SetIKHintPosition(AvatarIKHint.RightElbow,
                        weaponController.IkObjects.RightElbowObject.position);
                    //

                    switch (weaponController.DebugMode)
                    {
                        case WeaponsHelper.IkDebugMode.Aim:
                            weaponController.hasAimIKChanged = true;
                            Helper.HandIK(controller, weaponController, this, weaponController.IkObjects.LeftAimObject,
                                weaponController.IkObjects.RightAimObject, controller.BodyObjects.TopBody, controller.BodyObjects.TopBody, 1);
                            break;
                        case WeaponsHelper.IkDebugMode.Wall:
                            weaponController.hasWallIKChanged = true;
                            Helper.HandIK(controller, weaponController, this, weaponController.IkObjects.LeftWallObject,
                                weaponController.IkObjects.RightWallObject, controller.BodyObjects.TopBody, controller.BodyObjects.TopBody, 1);
                            break;
                        case WeaponsHelper.IkDebugMode.Norm:
                            if (!weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInNormalState)
                            {
                                if(controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                                    controller.anim.SetLayerWeight(3, 1);
                                
                                Helper.HandIK(controller, weaponController, this, weaponController.IkObjects.LeftObject,
                                    weaponController.IkObjects.RightObject, controller.BodyObjects.TopBody, controller.BodyObjects.TopBody, 1);
                            }
                            else
                            {
                                controller.anim.SetLayerWeight(3, 0);
                                Helper.HandIK(controller, weaponController, this, weaponController.IkObjects.LeftObject,
                                    weaponController.IkObjects.RightObject, controller.BodyObjects.TopBody, controller.BodyObjects.TopBody, 0);
                            }
                            break;
                        case WeaponsHelper.IkDebugMode.Crouch:
                            if (!weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState)
                            {
                                if (controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                                    controller.anim.SetLayerWeight(3, 1);
                                
                                weaponController.hasCrouchIKChanged = true;
                                
                                Helper.HandIK(controller, weaponController, this, weaponController.IkObjects.LeftCrouchObject,
                                    weaponController.IkObjects.RightCrouchObject, controller.BodyObjects.TopBody, controller.BodyObjects.TopBody, 1);
                            }
                            else
                            {
                                controller.anim.SetLayerWeight(3, 0);
                                Helper.HandIK(controller, weaponController, this, weaponController.IkObjects.LeftCrouchObject,
                                    weaponController.IkObjects.RightCrouchObject, controller.BodyObjects.TopBody, controller.BodyObjects.TopBody, 0);
                            }

                            break;
                    }
                }
            }
            else 
            {
                if (grenadeController && grenadeController.CurrentWeaponInfo.Count >= 1)
                {
                    if (controller.AdjustmentScene)
                    {
                        Helper.FingersRotate(grenadeController.CurrentWeaponInfo[grenadeController.SettingsSlotIndex], controller.anim, "Grenade"); 
                    }
                    else if(!controller.AdjustmentScene)
                    {
                        Helper.FingersRotate(grenadeController.WeaponInfos[grenadeController.SettingsSlotIndex], controller.anim, "Grenade");
                    }
                }
                else
                {
                    Helper.FingersRotate(null, controller.anim, "Null");
                }
            }
        }
        #endregion
    }
}





