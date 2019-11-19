using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Open : MonoBehaviour
{
    [SerializeField]

    public GameObject DoorToOpen;
    public Transform Target;
    public float doorSpeed = 1;
    public float delay = 0;
    public float duration = 5f;

    Transform targetPosition;
    bool doorOpening = false;
    bool doorFinishOpen = false;
    private void OnTriggerEnter(Collider col)
    {
        if (!doorOpening)
        {
            Invoke("ActivateDoor", delay);           
        }        
    }
    private void ActivateDoor()
    {
        doorOpening = true;
        Invoke("StopDoor", duration);
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
                DoorToOpen.transform.Translate(Vector3.right * Time.deltaTime * doorSpeed);
            }
        }      
    }
}
