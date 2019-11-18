// GercStudio
// © 2018-2019

using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GercStudio.USK.Scripts
{

    public enum ModeOfGame
    {
        Deathmatch = 0,
        Cooperative = 1
    }

    public class Lobby : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
//        public List<GameObject> CharactersPrefabs;
//        public List<GameObject> CharactersImages;

        public List<CharacterHelper.PhotonCharacter> Characters = new List<CharacterHelper.PhotonCharacter>{new CharacterHelper.PhotonCharacter()};
        public List<CharacterHelper.PhotonLevel> Levels = new List<CharacterHelper.PhotonLevel>{new CharacterHelper.PhotonLevel()};
        
        public GameObject RoomListingPrefab;
        public GameObject FindRooms;
        public GameObject MainMenu;
        public GameObject CreateRooms;
        public Transform roomsPanel;

        private List<GameObject> Players;
        //public List<RoomInfo> RoomListings;

//        public int CharactersCount;
        //public int LevelsCount;
       // public List<string> LevelsNames;

        public InputField RoomName;
        public InputField PlayerNameText;

        public Text MaxPlayersText;
        public Text MinPlayersText;
        public Text TimeValueText;

        public Slider MaxPlayersSlider;
        public Slider MinPlayersSlider;
        public Slider TimeSlider;

        public Toggle EnemiesToggle;

        public Button FindRoomsButton;
        public Button CreateRoomButton;
        public Button RandomRoomButton;
        public Button FindRoomsBackButton;
        public Button CreateRoomsBackButton;
        public Button Create;
        public Button UpCharacterCount;
        public Button DownCharacterCount;
       // public List<Button> LevelsButtons;

        public Dropdown GameModeDropdown;

        public int inspectorTab;

        private int currentLevel;
        private int currentPlayer;
        
        private List<RoomInfo> AllRooms = new List<RoomInfo>();

        private bool UnlimetedTime;

        void Start()
        {
            #region Player Name Update

            string DefaultName = String.Empty;
            
            var tempCharacters = new List<CharacterHelper.PhotonCharacter>();
			
            foreach (var character in Characters)
            {
                if (character.Character && !character.Character.GetComponent<CharacterSync>() || !character.Character)
                {
                    tempCharacters.Add(character);
                }
            }

            foreach (var tempCharacter in tempCharacters)
            {
                Characters.Remove(tempCharacter);
            }
            
            var tempLevels = new List<CharacterHelper.PhotonLevel>();
            foreach (var level in Levels)
            {
                if (!level.LevelButton)
                {
                    tempLevels.Remove(level);
                }
            }

            foreach (var tempLevel in tempLevels)
            {
                Levels.Remove(tempLevel);
            }
          
            if (PlayerNameText != null)
                if (PlayerPrefs.HasKey("PlayerName"))
                {
                    DefaultName = PlayerPrefs.GetString("PlayerName");
                    PlayerNameText.text = DefaultName;
                }

            PhotonNetwork.NickName = DefaultName;

            #endregion
            
            if(PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
            
            currentLevel = 0;
            if (Characters.Count > 0)
            {
                currentPlayer =  PlayerPrefs.GetInt("curCharacter");
                PlayerSelection();
            }

            MainMenu.SetActive(true);
            CreateRooms.SetActive(false);
            FindRooms.SetActive(false);

            UpCharacterCount.onClick.AddListener(NextPlayer);
            DownCharacterCount.onClick.AddListener(PreviousPlayer);
            FindRoomsButton.onClick.AddListener(FindRoomsClick);
            CreateRoomButton.onClick.AddListener(CreateRoomsClick);
            RandomRoomButton.onClick.AddListener(RandomRoomClick);
            FindRoomsBackButton.onClick.AddListener(FindRoomsBackButtonClick);
            TimeSlider.onValueChanged.AddListener(TimeSliderChange);
            MaxPlayersSlider.onValueChanged.AddListener(MaxPlayersChange);
            MinPlayersSlider.onValueChanged.AddListener(MinPlayersChange);
            PlayerNameText.onValueChanged.AddListener(SetName);

            CreateRoomsBackButton.onClick.AddListener(CreateRoomsBackButtonClick);
            Create.onClick.AddListener(CreateRoom);
            
            foreach (var button in Levels)
            {
                if (button.LevelButton)
                {
                    button.LevelButton.onClick.AddListener(ChoiceLevel);
                    button.LevelButton.GetComponent<Outline>().enabled = false;
                    button.LevelButton.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            
            if (!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();
        }

        void ListRoom(RoomInfo room)
        {
            if (room.IsVisible)
            {
                GameObject tempListing = Instantiate(RoomListingPrefab, roomsPanel);
                RoomButton tempButton = tempListing.GetComponent<RoomButton>();
                tempButton.RoomName = room.Name;
                tempButton.CurrentPlayer = room.PlayerCount;
                tempButton.RoomSize = room.MaxPlayers;
                tempButton.Enemies = (bool) room.CustomProperties["e"];
                tempButton.LevelName = Levels[(int) room.CustomProperties["m"]].LevelName;
                tempButton.GameMode = Enum.GetName(typeof(ModeOfGame), (ModeOfGame) room.CustomProperties["gm"]);
                tempButton.SetRoom();
            }
        }

        void PlayerSelection()
        {
            for (var i = 0; i < Characters.Count; i++)
            {
                if(Characters[i].Character)
                    Characters[i].Image.SetActive(false);
            }

            Characters[currentPlayer].Image.SetActive(true);
            PlayerPrefs.SetString("CharacterPrefabName", Characters[currentPlayer].Character.name);
            PlayerPrefs.SetInt("curCharacter", currentPlayer);
        }

        public void NextPlayer()
        {
            currentPlayer++;
            if (currentPlayer > Characters.Count - 1)
                currentPlayer = 0;
            PlayerSelection();
        }

        public void PreviousPlayer()
        {
            currentPlayer--;
            if (currentPlayer < 0)
                currentPlayer = Characters.Count - 1;
            PlayerSelection();
        }

        public void CreateRoom()
        {

            var customValues = new Hashtable();

            if (Levels.Count > 0)
                customValues.Add("m", currentLevel);
            else customValues.Add("m", 1);

            if (GameModeDropdown)
                customValues.Add("gm", GameModeDropdown.value);
            else customValues.Add("gm", 0);

            if (TimeSlider && !UnlimetedTime)
                customValues.Add("mt", (double) TimeSlider.value);

            if (MinPlayersSlider)
                customValues.Add("mp", Convert.ToInt32(MinPlayersSlider.value));
            else customValues.Add("mp", 0);

            if (EnemiesToggle)
                customValues.Add("e", EnemiesToggle.isOn);
            else customValues.Add("e", false);

            customValues.Add("gs", false);

            RoomOptions roomOpt = new RoomOptions();

            if (MaxPlayersSlider)
                roomOpt.MaxPlayers = (byte) Convert.ToInt32(MaxPlayersSlider.value);
            else roomOpt.MaxPlayers = 10;
            
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true;
            roomOpt.CustomRoomProperties = customValues;

            string[] value = new string[3];
            value[0] = "m";
            value[1] = "gm";
            value[2] = "e";
            roomOpt.CustomRoomPropertiesForLobby = value;
            PhotonNetwork.CreateRoom(RoomName.text, roomOpt);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("Player name is empty");
                return;
            }

            PhotonNetwork.NickName = name;
            PlayerPrefs.SetString("PlayerName", name);
        }

        static Predicate<RoomInfo> ByName(string name)
        {
            return delegate(RoomInfo room) { return room.Name == name; };
        }

        #region UIManaged

        void ChoiceLevel()
        {
            for (var i = 0; i < Levels.Count; i++)
            {
                Levels[i].LevelButton.GetComponent<Outline>().enabled = false;
                Levels[i].LevelButton.transform.localScale = new Vector3(1, 1, 1);
                if (Levels[i].LevelButton.name == EventSystem.current.currentSelectedGameObject.name)
                {
                    currentLevel = i;
                    Levels[i].LevelButton.GetComponent<Outline>().enabled = true;
                    Levels[i].LevelButton.transform.localScale = new Vector3(Levels[i].LevelButton.transform.localScale.x * 1.1f,
                        Levels[i].LevelButton.transform.localScale.y * 1.1f, 1);
                }
            }
        }

        void FindRoomsClick()
        {
            MainMenu.SetActive(false);
            FindRooms.SetActive(true);
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby(TypedLobby.Default);
            }
        }

        void CreateRoomsClick()
        {
            MainMenu.SetActive(false);
            CreateRooms.SetActive(true);
        }

        void RandomRoomClick()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        void FindRoomsBackButtonClick()
        {
            MainMenu.SetActive(true);
            FindRooms.SetActive(false);
        }

        void CreateRoomsBackButtonClick()
        {
            MainMenu.SetActive(true);
            CreateRooms.SetActive(false);
        }

        void TimeSliderChange(float value)
        {
            TimeValueText.text = "Match Time: " + value + " min";
            if (TimeSlider.value > TimeSlider.maxValue - 1)
            {
                UnlimetedTime = true;
                TimeValueText.text = "Match Time: ∞";
            }
            else
            {
                UnlimetedTime = false;
            }
        }

        void MinPlayersChange(float value)
        {
            MinPlayersText.text = "Min Players: " + value;

        }
        
        void MaxPlayersChange(float value)
        {
            MaxPlayersText.text = "Max Players: " + value;
        }

        #endregion

        #region PhotonCallBacks

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var room in roomList)
            {
                if (AllRooms.Count > 0)
                {
                    if (!AllRooms.Exists(_room => _room.Name == room.Name))
                        AllRooms.Add(room);
                    else
                    {
                        if (room.PlayerCount <= 0)
                            AllRooms.Remove(room);
                        else
                        {
                             var _roomInfo = AllRooms.Find(_room => _room.Name == room.Name);
                            _roomInfo.PlayerCount = room.PlayerCount;
                        }
                    }
                }
                else AllRooms.Add(room);
            }
            UpdateRooms();
        }
        
        void UpdateRooms()
        {
            foreach (Transform child in roomsPanel)
            {
                Destroy(child.gameObject);
            }

            foreach (var room in AllRooms)
            {
                ListRoom(room);
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connect to Master");
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            print("Joined to lobby");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            print("Failed create room: " + returnCode);
        }

        public override void OnCreatedRoom()
        {
            print("RoomManager is created");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            print("Random room didn't found, but we created new");
            CreateRoom();
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel(Levels[(int) PhotonNetwork.CurrentRoom.CustomProperties["m"]].LevelName);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Error Connect");
        }

        #endregion
    }

}




