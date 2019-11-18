// GercStudio
// © 2018-2019

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public List<Transform> SpawnAreas;

        public List<Helper.EnemiesInGameManager> Enemies = new List<Helper.EnemiesInGameManager>();

        public UI Ui;
        
        public int inspectorTab;
        public int CurrentCharacter;
        public int LastCharacter;

        public Button Exit;
        public Button Resume;
        public Button Restart;
        
        public Transform Canvas;

        public Text GrenadeAmmo;
        public Text WeaponAmmo;
        public Text Health;

        public Image pickUpIcon;

        public GameObject defaultCrosshair;

      //  public CharacterHelper.CrosshairType CrosshairType;

        public GameObject pauseBackground;
        public List<GameObject> Characters = new List<GameObject>(1);
        public List<Transform> CharactersSpawnPoints = new List<Transform>(1);
        
        public List<Controller> controller;

        public bool UsePause;

        public List<InventoryManager> inventoryManager;
        public List<CameraController> cameraController;

        private bool selectMenuItemWithGamepad;
        private bool isPause;
        private bool cameraFlyingStep1;
        private bool cameraFlyingStep2;
        private bool switchingCamera;
        private int _quantity;
        private int currentMenuItem;

        public Inputs inputs;

        private Camera cameraForSwitching;
        
        private Color[] normButtonsColors = new Color[3];
        private Sprite[] normButtonsSprites = new Sprite[3];

        private float _timeout;

        private bool setbutton;

        void Awake()
        {
            if (Ui)
            {
                if(Ui.defaultCrosshair) defaultCrosshair = Instantiate(Ui.defaultCrosshair, Canvas.transform);
                if(Ui.pickUpIcon) pickUpIcon = Instantiate(Ui.pickUpIcon, Canvas.transform);
                if(Ui.Health) Health = Instantiate(Ui.Health, Canvas.transform);
                if(Ui.WeaponAmmo) WeaponAmmo = Instantiate(Ui.WeaponAmmo, Canvas.transform);
                if(Ui.GrenadeAmmo) GrenadeAmmo = Instantiate(Ui.GrenadeAmmo, Canvas.transform);
            }
            
            cameraForSwitching = Helper.NewCamera("Camera for switching", transform, "GameManager");
            Destroy(cameraForSwitching.GetComponent<AudioListener>());
            
            cameraForSwitching.gameObject.SetActive(false);
            
            if (Characters.Count > 0)
            {
                for (var i = 0; i < Characters.Count; i++)
                {
                    var character = Characters[i];
                    if (!character) continue;

                    var position = Vector3.zero;
                    
                    if (CharactersSpawnPoints[i])
                        position = CharactersSpawnPoints[i].position;

                    var instantiateChar = Instantiate(character, position, Quaternion.identity);
                    
                    controller.Add(instantiateChar.GetComponent<Controller>());
                    inventoryManager.Add(instantiateChar.GetComponent<InventoryManager>());
                    cameraController.Add(instantiateChar.GetComponent<Controller>().thisCameraScript);

                    instantiateChar.GetComponent<InventoryManager>().Coop = true;
                    
                    if (WeaponAmmo) instantiateChar.GetComponent<InventoryManager>().AmmoUI = WeaponAmmo;

                    if (GrenadeAmmo) instantiateChar.GetComponent<InventoryManager>().GrenadeAmmoUI = GrenadeAmmo;

                    if (Health)
                    {
                        instantiateChar.GetComponent<Controller>().Health_Text = Health;
                        instantiateChar.GetComponent<Controller>().Health_Text.gameObject.SetActive(true);
                    }

                    if (pickUpIcon) instantiateChar.GetComponent<Controller>().thisCameraScript.PickUpCrosshair = pickUpIcon.transform;

                    if (defaultCrosshair) instantiateChar.GetComponent<Controller>().thisCameraScript.Crosshair = defaultCrosshair.transform;
                    

                    instantiateChar.GetComponent<Controller>().ActiveCharacter = i == 0;
                    
                    if (instantiateChar.GetComponent<Controller>().thisCamera.GetComponent<AudioListener>())
                        instantiateChar.GetComponent<Controller>().thisCamera.GetComponent<AudioListener>().enabled = i == 0;
                }
            }
            
            CurrentCharacter = 0;

            if (defaultCrosshair)
            {
                defaultCrosshair.transform.SetParent(controller[CurrentCharacter].transform.Find("Canvas"));
                defaultCrosshair.name = "Crosshair";
            }

            var foundObjects = GameObject.FindGameObjectsWithTag("Spawn");

            foreach (var obj in foundObjects)
            {
                SpawnAreas.Add(obj.transform);
            }

            if (SpawnAreas.Count == 0)
                Debug.LogWarning(
                    "(Game Control) <color=yellow>Not found</color> spawn points with tag [Spawn]. Add them, otherwise the enemies won't spawn");

            StartCoroutine("SpawnEnemies");

            if (!UsePause) return;
            
            if (pauseBackground)
                pauseBackground.SetActive(false);


            if (Exit)
            {
                Exit.onClick.AddListener(ExitGame);
                Exit.gameObject.SetActive(false);
                switch (Exit.transition)
                {
                    case Selectable.Transition.ColorTint:
                        normButtonsColors[0] = Exit.colors.normalColor;
                        break;
                    case Selectable.Transition.SpriteSwap:
                        normButtonsSprites[0] = Exit.GetComponent<Image>().sprite;
                        break;
                }
            }
            else
                Debug.LogWarning(
                    "<color=yellow>Missing component</color>: [Exit Button] in the [Game Manger] script. PLease add it.",
                    gameObject);

            if (Restart)
            {
                Restart.onClick.AddListener(RestartScene);
                Restart.gameObject.SetActive(false);
                switch (Restart.transition)
                {
                    case Selectable.Transition.ColorTint:
                        normButtonsColors[1] = Restart.colors.normalColor;
                        break;
                    case Selectable.Transition.SpriteSwap:
                        normButtonsSprites[1] = Restart.GetComponent<Image>().sprite;
                        break;
                }
            }
            else
                Debug.LogWarning(
                    "<color=yellow>Missing component</color>: [Restart Button] in the [Game Manger] script. PLease add it.",
                    gameObject);

            if (Resume)
            {
                Resume.onClick.AddListener(Pause);
                Resume.gameObject.SetActive(false);
                switch (Resume.transition)
                {
                    case Selectable.Transition.ColorTint:
                        normButtonsColors[2] = Resume.colors.normalColor;
                        break;
                    case Selectable.Transition.SpriteSwap:
                        normButtonsSprites[2] = Resume.GetComponent<Image>().sprite;
                        break;
                }
            }
            else
                Debug.LogWarning("<color=yellow>Missing component</color>: [Resume Button] in the [Game Manger] script. PLease add it.",
                    gameObject);
        }

        public void SwitchCharacter()
        {
            if(switchingCamera)
                return;

            var newCharacterIndex = CurrentCharacter;

            newCharacterIndex++;
            if (newCharacterIndex > Characters.Count - 1)
                newCharacterIndex = 0;

            if (CurrentCharacter != newCharacterIndex)
            {
                switchingCamera = true;
                controller[CurrentCharacter].ActiveCharacter = false;
            
                controller[CurrentCharacter].anim.SetBool("Attack", false);
                controller[CurrentCharacter].anim.SetBool("Move", false);
                controller[CurrentCharacter].anim.SetFloat("Horizontal", 0);
                controller[CurrentCharacter].anim.SetFloat("Vertical", 0);
                
                if(controller[CurrentCharacter].thisCamera.GetComponent<AudioListener>())
                    controller[CurrentCharacter].thisCamera.GetComponent<AudioListener>().enabled = false;
                
                if(controller[CurrentCharacter].WeaponManager.weaponController && controller[CurrentCharacter].WeaponManager.weaponController.IsAimEnabled)
                    controller[CurrentCharacter].WeaponManager.weaponController.Aim(true, false);

                cameraForSwitching.gameObject.SetActive(true);
                cameraForSwitching.transform.SetPositionAndRotation(controller[CurrentCharacter].thisCamera.transform.position,
                    controller[CurrentCharacter].thisCamera.transform.rotation);
                
                LastCharacter = CurrentCharacter;
                CurrentCharacter = newCharacterIndex;
                StartCoroutine(FlyCamera());
            }
            
        }

        IEnumerator FlyCamera()
        {
            while (true)
            {
                var dist = Vector3.Distance(controller[LastCharacter].transform.position, controller[CurrentCharacter].transform.position);

                var currentHeight = controller[CurrentCharacter].transform.position + Vector3.up * dist / 3;
                var lastHeight = controller[CurrentCharacter].transform.position + Vector3.up * dist / 3;

                var checkTopDown = controller[LastCharacter].thisCamera.transform.position.y < currentHeight.y &&
                                   controller[CurrentCharacter].thisCamera.transform.position.y < lastHeight.y;
                
                if (dist > 25 && checkTopDown)
                {
                    
                    //var lastCameraScript = controller[LastCharacter].GetComponent<Controller>().thisCameraScript;
                    var currentCameraScript = controller[CurrentCharacter].GetComponent<Controller>().thisCameraScript;

                    if (!cameraFlyingStep1 && !cameraFlyingStep2)
                    {
                        cameraForSwitching.transform.position = Helper.MoveObjInNewPosition(cameraForSwitching.transform.position,
                            controller[LastCharacter].transform.position + Vector3.up * dist / 3, 5 * Time.deltaTime);

                        var rot = controller[LastCharacter].thisCamera.transform.eulerAngles;
                        cameraForSwitching.transform.rotation =
                            Quaternion.Slerp(cameraForSwitching.transform.rotation, Quaternion.Euler(90, rot.y, rot.z), 0.5f);
                        
//                        if(lastCameraScript.crosshair)
//                            lastCameraScript.crosshair.gameObject.SetActive(false);
                        
                        if(currentCameraScript.Crosshair)
                            currentCameraScript.Crosshair.gameObject.SetActive(false);
                        

//                        if(lastCameraScript.PickUpCrosshair)
//                            lastCameraScript.PickUpCrosshair.gameObject.SetActive(false);
                        
                        if(currentCameraScript.PickUpCrosshair)
                            currentCameraScript.PickUpCrosshair.gameObject.SetActive(false);

                        cameraFlyingStep1 = Helper.ReachedPositionAndRotation(cameraForSwitching.transform.position,
                            controller[LastCharacter].transform.position + Vector3.up * dist / 3);
                    }

                    if (cameraFlyingStep1 && !cameraFlyingStep2)
                    {
                        if (defaultCrosshair)
                            defaultCrosshair.transform.SetParent(controller[CurrentCharacter].transform.Find("Canvas"));
                        
                        cameraForSwitching.transform.position = Helper.MoveObjInNewPosition(cameraForSwitching.transform.position,
                            controller[CurrentCharacter].transform.position + Vector3.up * dist / 3, 3 * Time.deltaTime);

//                    var rot = controller[CurrentCharacter].thisCamera.transform.eulerAngles;
//                    cameraForSwitching.transform.rotation =
//                        Quaternion.Slerp(cameraForSwitching.transform.rotation, Quaternion.Euler(90, rot.y, rot.z), 0.5f);

                        cameraFlyingStep2 = Helper.ReachedPositionAndRotation(cameraForSwitching.transform.position,
                            controller[CurrentCharacter].transform.position + Vector3.up * dist / 3);

                    }

                    if (cameraFlyingStep2)
                    {
                        cameraForSwitching.transform.position = Helper.MoveObjInNewPosition(cameraForSwitching.transform.position,
                            controller[CurrentCharacter].thisCamera.transform.position, 5 * Time.deltaTime);

                        cameraForSwitching.transform.rotation = Quaternion.Slerp(cameraForSwitching.transform.rotation,
                            controller[CurrentCharacter].thisCamera.transform.rotation, 2.5f * Time.deltaTime);
                        
                        if(currentCameraScript.Crosshair &&  controller[CurrentCharacter].TypeOfCamera != CharacterHelper.CameraType.ThirdPerson)
                            currentCameraScript.Crosshair.gameObject.SetActive(true);
                        

                        if (Helper.ReachedPositionAndRotation(cameraForSwitching.transform.position, controller[CurrentCharacter].thisCamera.transform.position,
                            cameraForSwitching.transform.eulerAngles, controller[CurrentCharacter].thisCamera.transform.eulerAngles))
                        {
                            controller[CurrentCharacter].ActiveCharacter = true;
                            controller[CurrentCharacter].thisCamera.GetComponent<Camera>().enabled = true;

                            cameraFlyingStep1 = false;
                            cameraFlyingStep2 = false;
                            switchingCamera = false;
                            
                            if(currentCameraScript.gameObject.GetComponent<AudioListener>())
                                currentCameraScript.gameObject.GetComponent<AudioListener>().enabled = true;
                            
                            StopCoroutine(FlyCamera());
                            break;
                        }
                    }
                }
                else
                {
                    cameraForSwitching.transform.position = Helper.MoveObjInNewPosition(cameraForSwitching.transform.position,
                        controller[CurrentCharacter].thisCamera.transform.position, 5 * Time.deltaTime);

                    cameraForSwitching.transform.rotation = Quaternion.Slerp(cameraForSwitching.transform.rotation,
                        controller[CurrentCharacter].thisCamera.transform.rotation, 2.5f * Time.deltaTime);

                    if (Helper.ReachedPositionAndRotation(cameraForSwitching.transform.position, controller[CurrentCharacter].thisCamera.transform.position,
                        cameraForSwitching.transform.eulerAngles, controller[CurrentCharacter].thisCamera.transform.eulerAngles))
                    {
                        controller[CurrentCharacter].ActiveCharacter = true;
                        controller[CurrentCharacter].thisCamera.GetComponent<Camera>().enabled = true;

                        cameraFlyingStep1 = false;
                        cameraFlyingStep2 = false;
                        switchingCamera = false;

                        StopCoroutine(FlyCamera());
                        break;
                    }
                }

                yield return 0;
            }
        }

//        public void ChangeInput()
//        {
//            setbutton = true;
//        }

//        private void OnGUI()
//        {
//            if(setbutton)
//                if (Input.GetAxis("Gamepad Vertical") > 0)
//                {
//                    controller[CurrentCharacter].GetComponent<Controller>()._gamepadAxes[1] = "Gamepad 3rd axis";
//                    setbutton = false;
//                }
//
//            var _event = Event.current;
//            if (_event.type == EventType.KeyDown && setbutton)
//                if (_event.isKey)
//                {
//                    controller[CurrentCharacter].GetComponent<Controller>()._keyboardCodes[12] = _event.keyCode;
//                    setbutton = false;
//                }
//
//        }

        void Update()
        {
            if(Characters.Count == 0 || !controller[CurrentCharacter] || !inventoryManager[CurrentCharacter])
                return;
            
            if (Input.GetKeyDown(controller[CurrentCharacter]._gamepadCodes[16]) || Input.GetKeyDown(controller[CurrentCharacter]._keyboardCodes[18]) ||
                    Helper.CheckGamepadAxisButton(16, controller[CurrentCharacter]._gamepadButtonsAxes, controller[CurrentCharacter].hasAxisButtonPressed, 
                        "GetKeyDown", controller[CurrentCharacter].inputs.AxisButtonValues[16]))
            {
                SwitchCharacter();
            }
//
//            if (!inventoryManager[CurrentCharacter].hasAnyWeapon)
//            {
//                if (controller[CurrentCharacter].thisCameraScript.crosshair)
//                    controller[CurrentCharacter].thisCameraScript.crosshair.gameObject.SetActive(false);
//            }
//            else
//            {
//                
//            }


            if (controller[CurrentCharacter].PlayerHealth <= 0)
            {
                if(UsePause && Restart)
                {
                    Restart.gameObject.SetActive(true);
                    
                    if(inventoryManager[CurrentCharacter].GamepadConnect)
                        StartCoroutine("GamepadInput");
                    
                    if (pauseBackground)
                        pauseBackground.SetActive(true);

                    if (Exit)
                        Exit.gameObject.SetActive(true);
                }
                else StartCoroutine("FastRestart");
            }
            else
            {
//                if (inventoryManager)
//                {
//                    if (inventoryManager.grenadeDebug & !isPause)
//                        Pause();
//
////                    if (inventoryManager.weaponController)
////                        if (inventoryManager.weaponController.ActiveDebug & !isPause)
////                            Pause();
//                }
                
//                if(inventoryManager.inventoryWheel.activeSelf & isPause)
//                    Pause();

//                if ((controller.DebugMode || cameraController.cameraDebug) & !isPause)
//                    Pause();

                if (!UsePause) return;
                
                if (Input.GetKeyDown(controller[CurrentCharacter]._gamepadCodes[10]) || Input.GetKeyDown(controller[CurrentCharacter]._keyboardCodes[10]) ||
                    Helper.CheckGamepadAxisButton(10, controller[CurrentCharacter]._gamepadButtonsAxes, controller[CurrentCharacter].hasAxisButtonPressed, 
                        "GetKeyDown", controller[CurrentCharacter].inputs.AxisButtonValues[10]))
                {
                    if (inventoryManager[CurrentCharacter].inventoryWheel)
                        if (inventoryManager[CurrentCharacter].inventoryWheel.activeSelf)
                            return;

                    Pause();
                }

//                    if (controller.DebugMode || cameraController.cameraDebug) return;

//                    if (inventoryManager)
//                    {
////                        if (inventoryManager.weaponController)
////                        {
//////                            if (inventoryManager.weaponController.ActiveDebug || inventoryManager.grenadeDebug) return;
////                            Pause();
////                        }
////                        else
////                        {
//////                            if (inventoryManager.grenadeDebug) return;
////                            Pause();
////                        }
//                    }
//                    else
//                    {
//                        Pause();
//                    }
            }
            
        }

        public void Pause()
        {
//            if (!inventoryManager.grenadeDebug & !controller.DebugMode & !cameraController.cameraDebug)
//            {
//                if (inventoryManager.weaponController)
//                {
//                    if (!inventoryManager.weaponController.ActiveDebug)
//                    {
//                        ShowGUI();
//                    }
//                }
//                else
//                {
//                    ShowGUI();
//                }
//            }

            ShowGUI();

            if (!isPause)
            {
                if (inventoryManager[CurrentCharacter].GamepadConnect)
                    StartCoroutine("GamepadInput");
            }
            else
                StopCoroutine("GamepadInput");


//            if (cameraController[CurrentCharacter].crosshair)
//                cameraController[CurrentCharacter].crosshair.gameObject.SetActive(isPause);

//            if (Application.isMobilePlatform)
//            {
//                controller[CurrentCharacter].uiButtons[11].gameObject.SetActive(isPause);
//            }

//            if (cameraController[CurrentCharacter].PickUpCrosshair)
//                cameraController[CurrentCharacter].PickUpCrosshair.gameObject.SetActive(isPause);


            controller[CurrentCharacter].isPause = !isPause;

            if (!Application.isMobilePlatform & !inventoryManager[CurrentCharacter].GamepadConnect)
            {
                Cursor.visible = !isPause;
                Cursor.lockState = !isPause ? CursorLockMode.None : CursorLockMode.Locked;
            }

            isPause = !isPause;

        }

        void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void ExitGame()
        {
            Application.Quit();
        }

        void ShowGUI()
        {
            DeactivateAllButtons();

            if (pauseBackground)
                pauseBackground.SetActive(!isPause);

            //if (!Application.isEditor)
                Time.timeScale = !isPause ? 0 : 1;

            if (Application.isMobilePlatform)
                controller[CurrentCharacter].UiButtonsGameObject.SetActive(isPause);

            if (Exit)
                Exit.gameObject.SetActive(!isPause);

            if (Resume)
                Resume.gameObject.SetActive(!isPause);
        }

        IEnumerator GamepadInput()
        {
            while (true)
            {
                var vector = new Vector2(Input.GetAxis(controller[CurrentCharacter]._gamepadAxes[0]),
                    Input.GetAxis(controller[CurrentCharacter]._gamepadAxes[1]));

                vector.Normalize();

                if (Math.Abs(vector.y - 1) < 0.4f & !selectMenuItemWithGamepad)
                {
                    DeactivateAllButtons();
                    selectMenuItemWithGamepad = true;
                    currentMenuItem++;
                    if (currentMenuItem > 1)
                        currentMenuItem = 1;
                }
                else if (Math.Abs(vector.y + 1) < 0.4f & !selectMenuItemWithGamepad)
                {
                    DeactivateAllButtons();
                    selectMenuItemWithGamepad = true;
                    currentMenuItem--;
                    if (currentMenuItem < 0)
                        currentMenuItem = 0;
                }
                else if (Math.Abs(vector.y) < 0.1f)
                {
                    selectMenuItemWithGamepad = false;
                }

                if (currentMenuItem == 0)
                {
                    if (Restart)
                    {
                        Helper.ChangeColor(Restart, Restart.colors.highlightedColor,
                            Restart.spriteState.highlightedSprite);

                        if (controller[CurrentCharacter].PlayerHealth <= 0)
                        {
                            if (Input.GetKey(KeyCode.Joystick1Button1))
                                Helper.ChangeColor(Restart, Restart.colors.pressedColor,
                                    Restart.spriteState.pressedSprite);

                            if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                                RestartScene();
                        }
                    }

                    if (Resume)
                    {
                        Helper.ChangeColor(Resume, Resume.colors.highlightedColor,
                            Resume.spriteState.highlightedSprite);
                        
                        if (controller[CurrentCharacter].PlayerHealth > 0)
                        {
                            if (Input.GetKey(KeyCode.Joystick1Button1))
                                Helper.ChangeColor(Resume, Resume.colors.pressedColor,
                                    Resume.spriteState.pressedSprite);

                            if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                                Pause();
                        }
                    }
                }
                else if (currentMenuItem == 1)
                {
                    if (Exit)
                    {

                        Helper.ChangeColor(Exit, Exit.colors.highlightedColor, Exit.spriteState.highlightedSprite);

                        if (Input.GetKey(KeyCode.Joystick1Button1))
                            Helper.ChangeColor(Exit, Exit.colors.pressedColor, Exit.spriteState.pressedSprite);


                        if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                            ExitGame();
                    }
                }

                yield return 0;
            }
        }

        void DeactivateAllButtons()
        {
            if(Exit)
                Helper.ChangeColor(Exit,normButtonsColors[0],normButtonsSprites[0]);
            
            if(Restart)
                Helper.ChangeColor(Restart,normButtonsColors[1],normButtonsSprites[1]);
            
            if(Resume)
                Helper.ChangeColor(Resume,normButtonsColors[2],normButtonsSprites[2]);
        }

//        IEnumerator DebugPause()
//        {
//            while (true)
//            {
//                if (!inventoryManager.grenadeDebug & !controller.DebugMode & !cameraController.cameraDebug)
//                {
//                    if (inventoryManager.weaponController)
//                    {
//                        if (!inventoryManager.weaponController.ActiveDebug)
//                        {
//                            if (pauseBackground)
//                                pauseBackground.SetActive(true);
//
//                            if (Exit)
//                                Exit.gameObject.SetActive(true);
//
//                            if (Resume)
//                                Resume.gameObject.SetActive(true);
//                        }
//                        else
//                        {
//                            if (pauseBackground)
//                                pauseBackground.SetActive(false);
//
//                            if (Exit)
//                                Exit.gameObject.SetActive(false);
//
//                            if (Resume)
//                                Resume.gameObject.SetActive(false);
//                        }
//                    }
//                    else
//                    {
//                        if (pauseBackground)
//                            pauseBackground.SetActive(true);
//
//                        if (Exit)
//                            Exit.gameObject.SetActive(true);
//
//                        if (Resume)
//                            Resume.gameObject.SetActive(true);
//                    }
//
//
//                }
//                else
//                {
//                    if (pauseBackground)
//                        pauseBackground.SetActive(false);
//
//                    if (Exit)
//                        Exit.gameObject.SetActive(false);
//
//                    if (Resume)
//                        Resume.gameObject.SetActive(false);
//                }
//
//                yield return 0;
//            }
//        }

        IEnumerator FastRestart()
        {
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            StopCoroutine("FastRestart");
        }

        IEnumerator SpawnEnemies()
        {
            while (true)
            {
                foreach (var enemy in Enemies)
                {
                    enemy.CurrentTime += Time.deltaTime;
                    
                    if (enemy.Count > enemy.CurrentSpawnCount && SpawnAreas.Count > 0 && enemy.enemyPrefab) //&& enemy.WaypointBehavior)
                    {
                        if (enemy.CurrentTime >= enemy.SpawnTimeout)
                        {
                            enemy.CurrentTime = 0;
                            
                            var spawn = enemy.CurrentSpawnMethodIndex == 0 ? SpawnAreas[Random.Range(0, SpawnAreas.Count)] : enemy.SpawnArea.transform;

                            if (spawn)
                            {
                                Instantiate(enemy.enemyPrefab, spawn.position, Quaternion.identity);
                               // instantiate.GetComponent<AIController>().movementBehavior = enemy.WaypointBehavior;
                            }

                            enemy.CurrentSpawnCount++;
                        }
                    }
                }

                yield return 0;
            }
        }
        
    }
}