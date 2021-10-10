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

    [Header("Enemy spawning")]
    public int GreenEnemyWave = 5; //five waves for the green enemies to start spawning
    public int BlueEnemyWave = 10; //ten waves for the blue enemies to start spawning

    public bool spawning; //for other scripts to stop or start the spawning process

    //private
    private List<GameObject> Enemies; //keeping track of the enemies so that they can be killed at the end of the round

    private int startEnemyCount;
    private int enemyIncreasePerWave;

    //time per each enemy to spawn in for each wave
    private const float timePerEnemySpawn = 2.5f;
    private float timeSinceLastEnemySpawn;

    //how many enemies are left to spawn in
    private int enemiesToSpawn;

    private int wave; //current wave

    void Awake()
    {
        startEnemyCount = 3;
        enemyIncreasePerWave = 3;
        timeSinceLastEnemySpawn = 0f;
        wave = 1;

        enemiesToSpawn = startEnemyCount;

        GreenEnemyWave = 5;
        BlueEnemyWave = 10;
    }

    void Start()
    {
        //play wave animation
        waveText.SetText("Wave: " + wave.ToString());
        StartCoroutine(animateWaveText());
    }

    void Update()
    {
        //detecting if it's time to spawn yet or not
        if(timeSinceLastEnemySpawn >= timePerEnemySpawn)
        {
            if (enemiesToSpawn > 0) //making sure the wave is not over
            {
                timeSinceLastEnemySpawn = 0;
                StartCoroutine(spawnNewEnemy());
            }
            else
            {
                //start a new wave
                timeSinceLastEnemySpawn = 0; //just some more wait time
                wave++;
                enemiesToSpawn += enemyIncreasePerWave;

                //play animations and stuff
                waveText.SetText("Wave Completed!\nWave: " + wave.ToString());
                StartCoroutine(animateWaveText());
            }
        }
        else
        {
            timeSinceLastEnemySpawn += Time.deltaTime;
        }
    }

    IEnumerator spawnNewEnemy()
    {
        //spawning a new enemy in a random position----------------------------------------------------------
        float x;
        float y;
        do
        {
            x = Random.Range(player.transform.position.x + SpawnRadius, player.transform.position.x - SpawnRadius);
            y = Random.Range(player.transform.position.y + SpawnRadius, player.transform.position.y - SpawnRadius);
        } while (x < PointOne.x || x > PointTwo.x || y > PointOne.y || y < PointTwo.y); //making sure the enemy spawns in level and the player can actually kill it

        Vector2 spawnPosition = new Vector2(x, y);

        //increasing number of possible enemies to spawn depending on wave
        int maxEnemy = 1;
        if (wave >= GreenEnemyWave)
            maxEnemy++;
        if (wave >= BlueEnemyWave)
            maxEnemy++;

        int t = Random.Range(0, maxEnemy); //getting a random enemy type to spawn

        Enemies.Add(Instantiate(enemy, spawnPosition, Quaternion.identity)); //create the enemy object
        particleController.AddParticles(Instantiate(spawnParticles, spawnPosition, Quaternion.identity));

        //converting the color32 of the enemy color to regular color
        Color enemyColor = Enemies[Enemies.Count - 1].GetComponent<Enemy>().getColor(t);
        ParticleSystem.MainModule settings = particleController.Particles[particleController.Particles.Count - 1].main;
        settings.startColor = enemyColor;

        yield return new WaitForSeconds(0.3f); //letting the particles play before spawning the enemy

        if (spawning)
        {
            Enemies[Enemies.Count - 1].SetActive(true);
            Enemies[Enemies.Count - 1].GetComponent<Enemy>().Type = t;
            Enemies[Enemies.Count - 1].GetComponent<Enemy>().Colorize();

            enemiesToSpawn--; //update the wave status
        }
    }

    IEnumerator animateWaveText()
    {
        //start the animation
        waveTextAnimation.SetTrigger("Show");

        yield return new WaitForSeconds(0.5f);

        //hide the text
        waveTextAnimation.SetTrigger("Hide");

        yield return new WaitForSeconds(0.2f);
        waveText.SetText("");
    }
}
