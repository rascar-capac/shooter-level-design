using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GercStudio.USK.Scripts;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private List<EnemyMove> zombiePrefabs;
    [SerializeField] private float period;

    private float timer;

    private void Start()
    {
        timer = period;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            EnemyMove enemyPrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Count)];
            EnemyMove enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            enemy.isAngryZombie = true;
            enemy.target = GameObject.FindWithTag("Player").GetComponent<Controller>();
            timer = period;
        }
    }
}
