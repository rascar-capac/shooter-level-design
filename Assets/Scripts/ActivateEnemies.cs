using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GercStudio.USK.Scripts;

public class ActivateEnemies : MonoBehaviour
{
    public EnemyMove[] enemiesToActivate;

    private void Start()
    {
        
        foreach (EnemyMove enemy in enemiesToActivate)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(EnemyMove enemy in enemiesToActivate)
            {
                enemy.target = other.GetComponent<Controller>();
                enemy.gameObject.SetActive(true);
            }
        }
        enabled = false;
    }


}
