using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Open : MonoBehaviour
{
    [SerializeField]

    public GameObject DoorToOpen;
    public float doorSpeed = 1;
    public float delay = 0;
    public float duration = 5f;
    public moveDirection doorDirection;
    public bool needButton;
    public GameObject PressE_UI;
    public List<GameObject> ObjectsToActivate;

    public AudioClip SoundToPlay;
    public float Volume;
    AudioSource audio;
    public enum moveDirection
    {
        up,
        left,
        right
    }

    Transform targetPosition;
    bool doorOpening = false;
    bool doorFinishOpen = false;
    private void Start()
    {
        if(ObjectsToActivate.Count != 0)
        {
            for (int i = 0; i < ObjectsToActivate.Count; i++)
            {
                ObjectsToActivate[i].SetActive(false);
            }
        }
        PressE_UI.SetActive(false);
        audio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider col)
    {
        if (!doorOpening)
        {
            if (!needButton)
            {
                Invoke("ActivateDoor", delay);
                
            }                              
        }        
    }
    private void OnTriggerStay(Collider col)
    {
        if (needButton)
        {
            PressE_UI.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                Invoke("ActivateDoor", delay);
                needButton = false;
                PressE_UI.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        PressE_UI.SetActive(false);
    }
    private void ActivateDoor()
    {
        doorOpening = true;
        Invoke("StopDoor", duration);
        audio.PlayOneShot(SoundToPlay, Volume);
        if (ObjectsToActivate.Count != 0)
        {
            for (int i = 0; i < ObjectsToActivate.Count; i++)
            {
                ObjectsToActivate[i].SetActive(true);
            }
        }
    }
    private void StopDoor()
    {
        doorFinishOpen = true;
    }
    private void Update()
    {
        if (doorOpening)
        {
            /*
            if (!(Vector3.Distance(DoorToOpen.transform.position, Target.position) < 0.1f))
            {
                DoorToOpen.transform.Translate(Vector3.right * Time.deltaTime * doorSpeed);
            }
            */
            if (!doorFinishOpen)
            {
                switch (doorDirection)
                {
                    case moveDirection.up:
                        DoorToOpen.transform.Translate(Vector3.up * Time.deltaTime * doorSpeed);
                        break;
                    case moveDirection.left:
                        DoorToOpen.transform.Translate(Vector3.right * Time.deltaTime * doorSpeed);
                        break;
                    case moveDirection.right:
                        DoorToOpen.transform.Translate(Vector3.left * Time.deltaTime * doorSpeed);
                        break;
                    default:
                        break;
                }
            }
        }      
    }
}
