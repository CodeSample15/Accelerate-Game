using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveController : MonoBehaviour
{
    //public
    [SerializeField] public GameObject player;
    [Header("Size of map")]
    [Tooltip("Should be TOP LEFT corner")]
    [SerializeField] public Vector2 PointOne;
    [Tooltip("Should be BOTTOM RIGHT corner")]
    [SerializeField] public Vector2 PointTwo;
    [Tooltip("Spawn radius around player")]
    [SerializeField] public float SpawnRadius;

    public GameObject enemy;
    public ParticleSystem spawnParticles;
    public ParticleController particleController;
    public TextMeshProUGUI waveText; //to tell the player what wave they're currently on
    public Animator waveTextAnimation;
    public PauseButton pauseButton;

    [Header("Enemy spawning")]
    public int GreenEnemyWave = 5; //five waves for the green enemies to start spawning
    public int BlueEnemyWave = 10; //ten waves for the blue enemies to start spawning
    public int GhostEnemyWave = 15; //fifteeen waves for the ghost enemies to start spawning

    [Header("Difficulty settings")]
    [Tooltip("How much the amount of random time before an enemy spawns in decreased")]
    public float difficultyIncrease;

    public bool spawning; //for other scripts to stop or start the spawning process

    //private
    private List<GameObject> Enemies; //keeping track of the enemies so that they can be killed at the end of the round

    private int startEnemyCount;
    private int enemyIncreasePerWave;

    //time per each enemy to spawn in for each wave
    private float minTimePerEnemySpawn;
    private float maxTimePerEnemySpawn;
    private float nextEnemyWait;
    private float timeSinceLastEnemySpawn;

    //how many enemies are left to spawn in
    private int enemiesToSpawn;
    private int enemiesSpawned; //how many enemies are in the wave

    private int wave; //current wave

    public List<GameObject> ActiveEnemies
    {
        get { return Enemies; }
    }

    public int getWave
    {
        get { return wave; }
    }

    public bool Spawning
    {
        get { return spawning; }
        set { spawning = value; }
    }

    void Awake()
    {
        Enemies = new List<GameObject>();
        spawning = true;

        startEnemyCount = 3;
        enemyIncreasePerWave = 2;
        enemiesSpawned = 0;
        timeSinceLastEnemySpawn = 0f;
        wave = 1;

        enemiesToSpawn = startEnemyCount;

        minTimePerEnemySpawn = 1.5f;
        maxTimePerEnemySpawn = 2.5f;
        nextEnemyWait = Random.Range(minTimePerEnemySpawn, maxTimePerEnemySpawn);

        GreenEnemyWave = 2;
        BlueEnemyWave = 4;
        GhostEnemyWave = 7;

        difficultyIncrease = 0.1f;
    }

    void Start()
    {
        //play wave animation
        waveText.SetText("Wave: " + wave.ToString());
        StartCoroutine(animateWaveText());
    }

    void Update()
    {
        if (!PauseButton.IsPaused) {
            if (player.GetComponent<Player>().isAlive)
            {
                //looping through the enemies array to see if any of them are dead, and removing the ones that are to save memory
                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (Enemies[i].gameObject == null)
                    {
                        Enemies.RemoveAt(i);
                        i--;
                    }
                }

                //detecting if it's time to spawn yet or not
                if (timeSinceLastEnemySpawn >= nextEnemyWait)
                {
                    if (enemiesSpawned < enemiesToSpawn)
                    {
                        timeSinceLastEnemySpawn = 0;
                        StartCoroutine(spawnNewEnemy());
                    }
                    else
                    {
                        //wait until the player has killed all of the enemies in the wave
                        if (Enemies.Count == 0)
                        {
                            //start a new wave
                            enemiesSpawned = 0;
                            timeSinceLastEnemySpawn = 0; //adding some more wait time when a new wave starts
                            wave++;
                            enemiesToSpawn += enemyIncreasePerWave;

                            //increase difficulty by lowering the amount of time it takes an enemy to spawn
                            if(minTimePerEnemySpawn > 0)
                            {
                                minTimePerEnemySpawn -= difficultyIncrease;
                                maxTimePerEnemySpawn -= difficultyIncrease;
                            }

                            minTimePerEnemySpawn = Mathf.Max(0, minTimePerEnemySpawn);
                            nextEnemyWait = Random.Range(minTimePerEnemySpawn, maxTimePerEnemySpawn);

                            //give the player more money for completing a round
                            player.GetComponent<Player>().earnMoney(5 * (wave-1));

                            //play animations and stuff
                            waveText.SetText("Next wave: " + wave.ToString());
                            StartCoroutine(animateWaveText());
                        }
                    }
                }
                else
                {
                    timeSinceLastEnemySpawn += Time.deltaTime;
                }
            }
            else
            {
                waveText.SetText(""); //clear the text
            }
        }
    }

    IEnumerator spawnNewEnemy()
    {
        //spawning a new enemy in a random position----------------------------------------------------------
        Vector2 topLeftBound = new Vector2(player.transform.position.x - SpawnRadius, player.transform.position.y + SpawnRadius);
        Vector2 bottomRightBound = new Vector2(player.transform.position.x + SpawnRadius, player.transform.position.y - SpawnRadius);

        float x = Random.Range(Mathf.Max(PointOne.x, topLeftBound.x), Mathf.Min(PointTwo.x, bottomRightBound.x));
        float y = Random.Range(Mathf.Min(PointOne.y, topLeftBound.y), Mathf.Max(PointTwo.y, bottomRightBound.y));

        Vector2 spawnPosition = new Vector2(x, y);

        //increasing number of possible enemies to spawn depending on wave
        int maxEnemy = 1;
        if (wave >= GreenEnemyWave)
            maxEnemy++;
        if (wave >= BlueEnemyWave)
            maxEnemy++;
        if (wave >= GhostEnemyWave)
            maxEnemy++;

        int t = Random.Range(0, maxEnemy); //getting a random enemy type to spawn

        Enemies.Add(Instantiate(enemy, spawnPosition, Quaternion.identity)); //create the enemy object
        particleController.AddParticles(Instantiate(spawnParticles, Enemies[Enemies.Count - 1].transform.position, Quaternion.identity));

        //converting the color32 of the enemy color to regular color
        Color enemyColor = Enemies[Enemies.Count - 1].GetComponent<Enemy>().getColor(t);
        ParticleSystem.MainModule settings = particleController.Particles[particleController.Particles.Count - 1].main;
        settings.startColor = enemyColor;

        yield return new WaitForSeconds(0.3f); //letting the particles play before spawning the enemy

        while (PauseButton.IsPaused)
        {
            yield return new WaitForSeconds(0.01f);
        }

        if (spawning)
        {
            Enemies[Enemies.Count - 1].SetActive(true);
            Enemies[Enemies.Count - 1].GetComponent<Enemy>().Type = t;
            Enemies[Enemies.Count - 1].GetComponent<Enemy>().Colorize();

            enemiesSpawned++; //update the wave status
        }
    }

    IEnumerator animateWaveText()
    {
        //start the animation
        waveTextAnimation.SetTrigger("Show");

        yield return new WaitForSeconds(1.5f);

        //hide the text
        waveTextAnimation.SetTrigger("Hide");

        yield return new WaitForSeconds(0.5f);
        waveText.SetText("");
    }


    //public method for killing an enemy
    public void clearEnemy(int number)
    {
        Destroy(Enemies[number].gameObject);
        Enemies.RemoveAt(number);
    }
}
