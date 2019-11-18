// GercStudio
// © 2018-2019

using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{

    public class RoomButton : MonoBehaviour
    {
        public Text NameText;
        public Text SizeText;
        public Text IndexText;
        public Text GameModeText;
        public Text EnemiesText;

        [HideInInspector] public int RoomSize;
        [HideInInspector] public int CurrentPlayer;

        [HideInInspector] public string LevelName;
        [HideInInspector] public string RoomName;
        [HideInInspector] public string GameMode;

        [HideInInspector] public bool Enemies;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(JoinRoomClick);
        }

        public void SetRoom()
        {
            IndexText.text = LevelName;
            NameText.text = RoomName;
            GameModeText.text = GameMode;
            EnemiesText.text = Enemies ? "Yes" : "No";
            SizeText.text = CurrentPlayer + " / " + RoomSize;
        }

        public void JoinRoomClick()
        {
            PhotonNetwork.JoinRoom(RoomName);
        }
    }

}


