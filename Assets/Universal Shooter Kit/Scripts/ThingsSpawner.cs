using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GercStudio.USK.Scripts;

public class ThingsSpawner : MonoBehaviour
{
    public EnemyMove[] EnemyToSpawn;

    private void Start()
    {
        foreach (EnemyMove enemy in EnemyToSpawn)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(EnemyMove enemy in EnemyToSpawn)
            {
                enemy.target = other.GetComponent<Controller>();
                enemy.gameObject.SetActive(true);
            }
        }

        enabled = false;
    }


}
