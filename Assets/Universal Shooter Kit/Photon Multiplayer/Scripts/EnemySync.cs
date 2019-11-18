// GercStudio
// © 2018-2019

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace GercStudio.USK.Scripts
{

    public class EnemySync : MonoBehaviourPun
    {
        private EnemyHealth enemyHealth;
        private float currentHealth;
        private float destroyTimeOut;
        private bool isTimerStarted;

        void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth>();
            currentHealth = enemyHealth.Enemy_health;
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        private void OnEvent(EventData photonEvent)
        {
            PhotonEventCodes eventCode = (PhotonEventCodes) photonEvent.Code;
            object[] data = photonEvent.CustomData as object[];
            if (eventCode == PhotonEventCodes.ChangeEnemyHealth)
            {
                if ((int) data[0] == photonView.ViewID)
                {
                    if (data.Length == 2)
                    {
                        enemyHealth.Enemy_health = (float) data[1];
                    }
                }
            }
        }

        void Update()
        {
            if (GetComponent<EnemyMove>().targets.Count > 1)
            {
                if (enemyHealth.Enemy_health_text & FindObjectOfType<RoomManager>().Player)
                {
                    enemyHealth.Enemy_health_text.LookAt(FindObjectOfType<RoomManager>().Player.GetComponent<Controller>()
                        .thisCamera.transform);
                    enemyHealth.Enemy_health_text.RotateAround(enemyHealth.Enemy_health_text.position,
                        enemyHealth.Enemy_health_text.up, 180);
                }
            }

            if (currentHealth != enemyHealth.Enemy_health)
            {
                RaiseEventOptions options = new RaiseEventOptions()
                {
                    CachingOption = EventCaching.AddToRoomCache,
                    Receivers = ReceiverGroup.All
                };
                object[] content = new object[]
                {
                    photonView.ViewID, enemyHealth.Enemy_health
                };
                PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.ChangeEnemyHealth, content, options,
                    SendOptions.SendReliable);
                currentHealth = enemyHealth.Enemy_health;
            }

            destroyTimeOut += Time.deltaTime;

            if (enemyHealth.Enemy_health <= 0)
            {
                if (PhotonNetwork.IsMasterClient & !isTimerStarted)
                {
                    destroyTimeOut = 0;
                    isTimerStarted = true;
                }

                if (isTimerStarted & destroyTimeOut >= 0.05f)
                {
                    PhotonNetwork.Destroy(PhotonView.Get(gameObject));
                }
            }
        }
    }

}



