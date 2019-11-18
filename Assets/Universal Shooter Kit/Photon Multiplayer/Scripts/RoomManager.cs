// GercStudio
// © 2018-2019

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{

    [RequireComponent(typeof(PickUpManager))]
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        public List<Transform> SpawnPoints;
        public List<Transform> SpawnAreas;
        public Transform PlayersPanel;
        public Transform StatsContent;
        public List<GameObject> Enemies;

        public Transform Canvas;
        
        public GameObject StartMenu;
        public GameObject PlayerListingPrefab;
        public GameObject DefaultCamera;
        public GameObject Player;

        public UI Ui;
        
        public Image pickUpIcon;

        public GameObject defaultCrosshair;

        public Button StartButton;
        public Button BackButton;
        public Button ResumeButton;

        public Text StatsText;
        public Text TimerText;
        public Text RestartTimer;
        
        public Text GrenadeAmmo;
        public Text WeaponAmmo;
        public Text Health;

        public float RestartTime = 5;
        public float timeout = 10;
        public int quantity = 4;
        
        public int inspectorTab;

        private float _timeout;
        private float _restartTime;
        private double time;
        private double StartTime;
        private double currentTime;
        private double LeftTime;
        private int _quantity;

        private bool IsTimer;
        private bool GameStarted;
        private bool isRestartTimer;
        private bool isPause;
        private bool isEnemies;
        
        
        void Awake()
        {
            if (!PhotonNetwork.InRoom)
            {
                Debug.LogWarning("You aren't in the Photon.RoomManager - Connect to this scene in the Lobby ");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                if (Ui)
                {
                    if(Ui.defaultCrosshair) defaultCrosshair = Instantiate(Ui.defaultCrosshair, Canvas.transform);
                    if(Ui.pickUpIcon) pickUpIcon = Instantiate(Ui.pickUpIcon, Canvas.transform);
                    if(Ui.Health) Health = Instantiate(Ui.Health, Canvas.transform);
                    if(Ui.WeaponAmmo) WeaponAmmo = Instantiate(Ui.WeaponAmmo, Canvas.transform);
                    if(Ui.GrenadeAmmo) GrenadeAmmo = Instantiate(Ui.GrenadeAmmo, Canvas.transform);
                }

                Hashtable customValues = new Hashtable();
                customValues.Add("k", 0);
                customValues.Add("d", 0);
                PhotonNetwork.LocalPlayer.SetCustomProperties(customValues);
                CheckCountPlayers();
                CheckPlayersNames();
                ClearPlayerListings();
                PlayerList();
            }
        }

        void Start()
        {

            if(StartMenu)
                StartMenu.SetActive(true);
            else
            {
                Debug.LogError("<color=red>Missing component</color>: [Start Menu] in the [RoomManager] script. Please add it; " +
                    "otherwise, the start screen won't be displayed.", gameObject);
                Debug.Break();
            }
            
            if(DefaultCamera)
                DefaultCamera.SetActive(true);
            else
            {
                Debug.LogError(
                    "<color=red>Missing component</color>: [Default Camera] in the [RoomManager] script. Please add it; " +
                    "this camera is needed for the main menu.", gameObject);
                Debug.Break();
            }
            
            if(TimerText)
                TimerText.gameObject.SetActive(false);
            else
            {
                Debug.LogWarning(
                    "<color=yellow>Missing component</color>: [Timer Text] in the [RoomManager] script. Please add it; " +
                    "otherwise, the timer won't be shown during the match.", gameObject);
            }
            
            if(RestartTimer)
                RestartTimer.gameObject.SetActive(false);
            else
            {
                Debug.LogWarning(
                    "<color=yellow>Missing component</color>: [Restart Timer Text] in the [RoomManager] script. Please add it; " +
                    "otherwise, the restart timer won't be shown.", gameObject);
            }
            
            if(StartButton)
                StartButton.onClick.AddListener(StartButtonClick);
            else
            {
                Debug.LogError(
                    "<color=red>Missing component</color>: [Start Button] in the [RoomManager] script. Please add it; " +
                    "otherwise, you will not be able to run the game.", gameObject);
                Debug.Break();
            }
            
            if(BackButton)
                BackButton.onClick.AddListener(BackButtonClick);
            else
            {
                Debug.LogWarning(
                    "<color=yellow>Missing component</color>: [Back Button] in the [RoomManager] script. Please add it; " +
                    "otherwise, you won't be able to go to the main menu.", gameObject);
            }

            if (ResumeButton)
            {
                ResumeButton.onClick.AddListener(Pause);
                ResumeButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning(
                    "<color=yellow>Missing component</color>: [Back Button] in the [RoomManager] script. Please add it; " +
                    "this button is needed to continue the game.", gameObject);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                StartButton.gameObject.SetActive(true);
                if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["e"])
                    isEnemies = true;
            }
            else
            {
                StartButton.gameObject.SetActive(false);
            }

            if (isEnemies)
            {
                var foundObjects = GameObject.FindGameObjectsWithTag("Spawn");
                foreach (var obj in foundObjects)
                {
                    SpawnPoints.Add(obj.transform);
                }

                if (SpawnPoints.Count == 0)
                    Debug.LogError("Not found spawn points with tag [Spawn]. Add them, otherwise the enemies won't spawn");
                
                if(Enemies.Count == 0)
                {
                    Debug.LogWarning("There is not any enemy in the [Room Manager] script.");
                }
            }
        }

        void LateUpdate()
        {
            #region RestartTimer Update

            _restartTime -= Time.deltaTime;
            RestartTimer.text = "Restart: " + _restartTime.ToString("00");
            
            if (!Player & !isRestartTimer & GameStarted)
            {
                RestartTimer.gameObject.SetActive(true);
                isRestartTimer = true;
                _restartTime = RestartTime;
                StopGame();
            }

            if (isRestartTimer & _restartTime <= 0)
            {
                RestartTimer.gameObject.SetActive(false);
                LaunchGame();
                isRestartTimer = false;
            }

            #endregion

            StatsContent.GetChild(1).GetComponent<Scrollbar>().value = 0;

            if (Player)
            {
                var controller = Player.GetComponent<Controller>();
                if ((Input.GetKeyDown(controller._gamepadCodes[10]) ||
                     Input.GetKeyDown(controller._keyboardCodes[10]) ||
                     Helper.CheckGamepadAxisButton(10, controller._gamepadButtonsAxes,
                         controller.hasAxisButtonPressed, "GetKeyDown", controller.inputs.AxisButtonValues[10])) & GameStarted)
                {
                    Pause();
                }
            }

            if (!GameStarted & PhotonNetwork.InRoom)
            {
                if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"])
                {
                    LaunchGame();
                    GameStarted = true;
                    StartTime = (double) PhotonNetwork.CurrentRoom.CustomProperties["t"];
                    if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("mt"))
                    {
                        time = (double) PhotonNetwork.CurrentRoom.CustomProperties["mt"] * 60;
                        IsTimer = true;
                    }
                    else IsTimer = false;
                }
            }

            if (GameStarted & IsTimer)
            {
                currentTime = PhotonNetwork.Time - StartTime;
                LeftTime = time - currentTime;

                TimerText.text = "Time left: " + FormatTime(LeftTime);
                if (LeftTime <= 0)
                    StopGame();
            }

            SpawnEnemies();
        }

        private string FormatTime(double time)
        {
            int minutes = (int) time / 60;
            int seconds = (int) time - 60 * minutes;

            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public void ClearPlayerListings()
        {
            for (int i = PlayersPanel.childCount - 1; i >= 0; i--)
            {
                Destroy(PlayersPanel.GetChild(i).gameObject);
            }
        }

        public void PlayerList()
        {
            if (PhotonNetwork.InRoom)
            {
                var _players = PhotonNetwork.PlayerList;
                var sortPlayers = _players.OrderByDescending(t => t.CustomProperties["k"])
                    .ThenBy(t => t.CustomProperties["d"]).ToArray();

                foreach (var player in sortPlayers)
                {
                    var tempListing = Instantiate(PlayerListingPrefab, PlayersPanel);
                    var tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                    tempText.text = player.NickName + " | Kills: " + player.CustomProperties["k"] + " | Deaths: " + player.CustomProperties["d"];
                }
            }
        }

        void SpawnEnemies()
        {
            if (isEnemies & PhotonNetwork.IsMasterClient)
            {
                _timeout += Time.deltaTime;
                if (_timeout >= timeout & _quantity < quantity & SpawnPoints.Count > 0)
                {
                    if (Enemies.Count > 0)
                    {
                        PhotonNetwork.InstantiateSceneObject(
                            Enemies[Random.Range(0, Enemies.Count - 1)].name,
                            SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity, 0);
                        _timeout = 0;
                        _quantity++;
                    }
                }
            }
        }

        void StartGame()
        {
            Hashtable customValues = new Hashtable();
            customValues.Add("gs", true);
            customValues.Add("t", PhotonNetwork.Time);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customValues);
        }

        void StopGame()
        {
            if (LeftTime <= 0)
            {
                ResumeButton.gameObject.SetActive(false);

                if (PhotonNetwork.IsMasterClient)
                {
                    StartButton.gameObject.SetActive(true);
                    var customValues = new Hashtable();
                    customValues.Add("gs", false);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(customValues);
                }
                else StartButton.gameObject.SetActive(false);

                isPause = false;
                GameStarted = false;
                IsTimer = false;
                isRestartTimer = false;
                _quantity = 0;
                _timeout = 0;

                if (Player.GetComponent<Controller>().thisCamera)
                    Destroy(Player.GetComponent<Controller>().thisCamera);

                PhotonNetwork.Destroy(Player);
                StartMenu.SetActive(true);
                DefaultCamera.SetActive(true);
                TimerText.gameObject.SetActive(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                isPause = false;
                StartButton.gameObject.SetActive(false);
                StartMenu.SetActive(true);
                DefaultCamera.SetActive(true);
                TimerText.gameObject.SetActive(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        void LaunchGame()
        {
            StartMenu.SetActive(false);
            DefaultCamera.SetActive(false);
            TimerText.gameObject.SetActive(true);
            var foundObjects = GameObject.FindGameObjectsWithTag("SpawnArea");
            foreach (var area in foundObjects)
            {
                SpawnAreas.Add(area.transform);
            }

            if (SpawnAreas.Count > 0)
            {
                var spawnPoint = SpawnAreas[Random.Range(0, SpawnAreas.Count - 1)];
                Player = PhotonNetwork.Instantiate(PlayerPrefs.GetString("CharacterPrefabName"),
                    new Vector3(spawnPoint.position.x + Random.Range(0, 10), spawnPoint.position.y + 3, spawnPoint.position.z + Random.Range(0, 10)), Quaternion.identity,
                    0);

                Player.GetComponent<Controller>().ActiveCharacter = true;

                if (WeaponAmmo) Player.GetComponent<InventoryManager>().AmmoUI = WeaponAmmo;

                if (GrenadeAmmo) Player.GetComponent<InventoryManager>().GrenadeAmmoUI = GrenadeAmmo;

                if (Health)
                {
                    Player.GetComponent<Controller>().Health_Text = Health;
                    Player.GetComponent<Controller>().Health_Text.gameObject.SetActive(true);
                }

                if (pickUpIcon) Player.GetComponent<Controller>().thisCameraScript.PickUpCrosshair = pickUpIcon.transform;

                if (defaultCrosshair) Player.GetComponent<Controller>().thisCameraScript.Crosshair = defaultCrosshair.transform;

            }
            else Debug.LogError("<Color=Red>Missing</Color> [PlayerPrefab] or [SpawnAreas]. Please set it up in GameObject [RoomManager Options] ", this);
        }
        
        public void Pause()
        {
            var value = !isPause;

            Controller controller = Player.GetComponent<Controller>();
            
            if(StartButton)
                StartButton.gameObject.SetActive(false);
            
            if(ResumeButton)
                ResumeButton.gameObject.SetActive(value);
            
            controller.isPause = value;
            if (Application.isMobilePlatform)
                controller.UiButtonsGameObject.SetActive(!value);

            if (value)
            {
                if(controller.thisCameraScript.Crosshair)
                    controller.thisCameraScript.Crosshair.gameObject.SetActive(false);

                if (Application.isMobilePlatform)
                {
                    controller.uiButtons[4].gameObject.SetActive(false);
                }
                else
                {
                    if ( controller.thisCameraScript.PickUpCrosshair)
                        controller.thisCameraScript.PickUpCrosshair.gameObject.SetActive(false);
                }
            }

            StartMenu.SetActive(value);
            Cursor.visible = value;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
            isPause = value;
        }

        void CheckCountPlayers()
        {
            var room = PhotonNetwork.CurrentRoom;
            StartButton.GetComponent<Button>().interactable = room.PlayerCount >= (int) room.CustomProperties["mp"];
        }

        void CheckPlayersNames()
        {
            var PlayersList = PhotonNetwork.PlayerListOthers;
            for (int i = 0; i < PlayersList.Length; i++)
            {
                if (PhotonNetwork.NickName == PlayersList[i].NickName)
                    PhotonNetwork.NickName = PhotonNetwork.NickName + " (" + Random.Range(100, 10000) + ")";
            }
        }

        #region UIManaged

        void StartButtonClick()
        {
            if (PhotonNetwork.InRoom & PhotonNetwork.IsMasterClient)
                StartGame();
        }

        void BackButtonClick()
        {
            if (GameStarted & Player)
            {
                Destroy(Player.GetComponent<Controller>().thisCameraScript.CameraPos.gameObject);
                PhotonNetwork.Destroy(Player);
            }

            //SceneManager.LoadScene(0);
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PhotonCallBacks

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (newMasterClient.IsLocal)
                StartButton.gameObject.SetActive(true);
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            CheckCountPlayers();
            StartCoroutine(UpdatePlayerList());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckCountPlayers();
            ClearPlayerListings();
            PlayerList();
        }

        #endregion

        IEnumerator UpdatePlayerList()
        {
            yield return new WaitForSeconds(3);
            CheckPlayersNames();
            ClearPlayerListings();
            PlayerList();
            StopCoroutine(UpdatePlayerList());
        }

        public IEnumerator StatsEnabledTimer()
        {
            yield return new WaitForSeconds(5);
            StatsContent.gameObject.SetActive(false);
            StopCoroutine(StatsEnabledTimer());
        }
    }

}


