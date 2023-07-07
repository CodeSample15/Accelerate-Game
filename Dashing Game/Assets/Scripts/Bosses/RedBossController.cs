using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RedBossController : MonoBehaviour
{
    public static List<GameObject> EnemyPool;

    [Header("Cosmetics")]
    [SerializeField] private float neutralTurnSpeed;
    [SerializeField] private float attackTurnSpeed;

    [Header("Non cosmetics")]
    [SerializeField] private float healthDecay;
    [SerializeField] private float attackTime;
    [SerializeField] private GameObject EnemyObject;
    [SerializeField] private SpikeAttack spikeAttack;
    [SerializeField] private GameObject laserAttack;

    [Space]

    [Header("Enemy Spawning")]
    [SerializeField] private Vector2 posOne;
    [SerializeField] private Vector2 posTwo;
    [SerializeField] private ParticleSystem spawnParticles;
    [SerializeField] private int EnemiesToSpawn;
    [SerializeField] private int MaxEnemies;

    private Player player;

    private float turnVel;
    private float turnSpeed;

    private bool isAttacking;

    public bool IsAttacking
    {
        get { return isAttacking; }
    }

    void Awake()
    {
        EnemyPool = new List<GameObject>();
        player = Player.staticReference;

        laserAttack.SetActive(false);

        isAttacking = false;
        turnSpeed = neutralTurnSpeed;

        StartCoroutine(AstarRescan()); //rescan the map to update the pathfinding (DELETE IF LAG IS CAUSED)
    }

    void Update()
    {
        if(!PauseButton.IsPaused)
        {
            transform.Rotate(new Vector3(0,0,1) * turnSpeed * Time.deltaTime);
            BossController.Static_Reference.Damage(healthDecay*Time.deltaTime);

            if(isAttacking)
                turnSpeed = Mathf.SmoothDamp(turnSpeed, attackTurnSpeed, ref turnVel, 1f);
            else
                turnSpeed = Mathf.SmoothDamp(turnSpeed, neutralTurnSpeed, ref turnVel, 0.1f);
        }

        //remove stale enemies from list
        for (int i = 0; i < EnemyPool.Count; i++)
        {
            if (EnemyPool[i] == null)
            {
                EnemyPool.RemoveAt(i);
                i--;
            }
        }
    }

    public void attack()
    {
        StartCoroutine(RunAttack());
    }

    private IEnumerator AstarRescan()
    {
        while(true)
        {
            if(!PauseButton.IsPaused)
                AstarPath.active.Scan();

            yield return new WaitForSeconds(0.5f); //rescan every half a second
        }
    }

    IEnumerator SpawnEnemies()
    {
        for(int i=0; i<EnemiesToSpawn; i++)
        {
            if (EnemyPool.Count < MaxEnemies)
            {
                bool posGood = false;
                int layerMask = 1 << 8;
                Vector2 pos;

                int tries = 0;

                do
                {
                    pos = new Vector2(Random.Range(posOne.x, posTwo.x), Random.Range(posOne.y, posTwo.y));
                    RaycastHit2D hit = Physics2D.Raycast(pos, transform.right, 0.01f, layerMask);

                    if (hit.collider == null)
                        posGood = true;

                    yield return new WaitForSeconds(0.01f);

                    tries++;
                } while (!posGood && tries < 30);

                if (tries >= 30)
                    continue;

                Enemy enemyHolder = Instantiate(EnemyObject, pos, Quaternion.identity).GetComponent<Enemy>();
                enemyHolder.gameObject.SetActive(false);
                enemyHolder.player = Player.staticReference;
                enemyHolder.playerGameObject = Player.staticReference.gameObject;
                enemyHolder.GetComponent<AIDestinationSetter>().target = Player.staticReference.gameObject.transform;
                EnemyPool.Add(enemyHolder.gameObject);

                ParticleSystem particleHolder = Instantiate(spawnParticles, pos, Quaternion.identity);

                //converting the color32 of the enemy color to regular color
                Color enemyColor = enemyHolder.getColor(3);
                ParticleSystem.MainModule settings = particleHolder.main;
                settings.startColor = enemyColor;

                while (PauseButton.IsPaused)
                    yield return new WaitForEndOfFrame();

                //spawn the enemy
                if (player.isAlive && BossController.Static_Reference.Health > 0)
                {
                    enemyHolder.gameObject.SetActive(true);
                    enemyHolder.Type = 3;
                    enemyHolder.Colorize();
                }
                else
                    break;
            }
        }
    }

    IEnumerator RunAttack()
    {
        isAttacking = true;

        //wait for spin speed to reach max speed
        yield return new WaitForSeconds(1.9f);

        //50% chance of spike attack spawning
        if (Random.Range(0, 2) == 1)
        {
            spikeAttack.transform.position = player.transform.position;
            spikeAttack.BeginSpawning();
        }

        //50% chance of enemies spawning
        if (Random.Range(0, 2) == 1)
            StartCoroutine(SpawnEnemies());

        //100% chance of lasers shooting from boss
        laserAttack.SetActive(true);

        //wait for attack to finish
        yield return new WaitForSeconds(attackTime);

        //clean up
        isAttacking = false;
        laserAttack.SetActive(false);
    }
}
