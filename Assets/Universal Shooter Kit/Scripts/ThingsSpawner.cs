using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingsSpawner : MonoBehaviour
{
    public GameObject[] ThingsToSpawn;

    private void Start()
    {
        foreach (GameObject go in ThingsToSpawn)
        {
            go.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(GameObject go in ThingsToSpawn)
            {
                go.SetActive(true);
            }
        }

        enabled = false;
    }


}
