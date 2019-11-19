using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Open : MonoBehaviour
{
    [SerializeField]

    public GameObject DoorToOpen;
    public Transform Target;
    public float doorSpeed = 1;

    Transform targetPosition;
    bool doorOpening = false;
    private void OnTriggerEnter(Collider col)
    {
        if (!doorOpening)
        {
            doorOpening = true;
        }        
    }
    private void Update()
    {
        if (doorOpening)
        {           
            if (!(Vector3.Distance(transform.position, Target.position) < 0.001f))
            {
                DoorToOpen.transform.Translate(Vector3.right * Time.deltaTime * doorSpeed);
            }
        }
    }
}
