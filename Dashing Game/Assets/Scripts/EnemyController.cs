using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //public
    [SerializeField] public Vector2 PointOne;
    [SerializeField] public Vector2 PointTwo;
    public GameObject enemy;

    //private
    private List<GameObject> Enemies; //keeping track of the enemies so that they can be killed at the end of the round
    private float minTimeUntilNextSpawn;
    private float maxTimeUntilNextSpawn;

    //the minimum that the max and min times until next spawn are allowed to go
    private float minTimePossible;
    private float maxTimePossible;

    private float difficultyIncrease; //how fast the game gets harder to play

    private bool spawning;

    // Start is called before the first frame update
    void Start()
    {
        Enemies = new List<GameObject>();
        minTimeUntilNextSpawn = 3f;
        maxTimeUntilNextSpawn = 10f;

        minTimePossible = 1f;
        maxTimePossible = 3f;

        difficultyIncrease = 0.1f;

        spawning = true;

        StartCoroutine(spawnEnemies());
    }

    IEnumerator spawnEnemies()
    {
        while(spawning)
        {
            //spawning a new enemy in a random position----------------------------------------------------------
            float x = Random.Range(PointOne.x, PointTwo.x);
            float y = Random.Range(PointOne.y, PointTwo.y);
            Vector2 spawnPosition = new Vector2(x, y);

            Enemies.Insert(0, Instantiate(enemy, spawnPosition, Quaternion.identity));
            Enemies[0].SetActive(true);

            //dictating how long the program should wait until the next enemy can be spawned---------------------
            float randomTime = Random.Range(minTimeUntilNextSpawn, maxTimeUntilNextSpawn);
            updateTimes();

            yield return new WaitForSeconds(randomTime);
        }
    }

    //private methods
    private void updateTimes()
    {
        if(maxTimeUntilNextSpawn > maxTimePossible)
        {
            maxTimeUntilNextSpawn -= difficultyIncrease;
        }
        else if(minTimeUntilNextSpawn > minTimePossible)
        {
            minTimeUntilNextSpawn -= difficultyIncrease;
        }
    }
}