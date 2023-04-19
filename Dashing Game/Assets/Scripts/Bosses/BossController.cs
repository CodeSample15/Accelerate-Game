using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("The actual boss in the level")]
    [SerializeField] private GameObject boss;

    [Header("Boss stats (change these per boss)")]
    [SerializeField] private float minAttackDelay;
    [SerializeField] private float maxAttackDelay;
    [SerializeField] private float startHealth;

    [Space]

    [SerializeField] private GameObject[] attacks;

    private Player player;
    private Animator bossAnims;

    private float health;

    private float timeSinceLastAttack;
    private float nextAttackTime;
    private int bossType;

    public float Health
    {
        get { return health; }
    }

    void Awake()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "Blue Boss":
                bossType = 0;
                break;

            case "Green Boss":
                bossType = 1;
                break;

            case "Orange Boss":
                bossType = 2;
                break;

            case "Pink Boss":
                bossType = 3;
                break;

            case "Red Boss":
                bossType = 4;
                break;
        }

        player = FindObjectOfType<Player>();
        bossAnims = boss.GetComponent<Animator>();
        health = startHealth;
        nextAttackTime = Random.Range(minAttackDelay, maxAttackDelay);
    }

    void Update()
    {
        if(health > 0 && !PauseButton.IsPaused)
        {
            timeSinceLastAttack += Time.deltaTime;

            if(timeSinceLastAttack >= nextAttackTime)
            {
                if(attacks.Length != 0) //shouldn't be the case when game is released, but it's useful for testing
                {
                    int randomIndex = Random.Range(0, attacks.Length);
                    Instantiate(attacks[randomIndex], player.gameObject.transform.position, Quaternion.identity);
                }

                nextAttackTime = Random.Range(minAttackDelay, maxAttackDelay);
                timeSinceLastAttack = 0;
            }
        }
    }
}
