using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("Boss stats (change these per boss)")]
    [SerializeField] private float minAttackDelay;
    [SerializeField] private float maxAttackDelay;
    [SerializeField] private float startHealth;

    [Header("Boss attack gameobjects (DONT CHANGE)")]
    [SerializeField] private GameObject[] BlueBossAttacks;
    [SerializeField] private GameObject[] GreenBossAttacks;
    [SerializeField] private GameObject[] OrangeBossAttacks;
    [SerializeField] private GameObject[] PinkBossAttacks;
    [SerializeField] private GameObject[] RedBossAttacks;

    private GameObject[][] attacks;

    private float timeSinceLastAttack;
    private int bossType;

    void Awake()
    {
        //prepare attacks
        attacks = new GameObject[5][];
        attacks[0] = BlueBossAttacks;
        attacks[1] = GreenBossAttacks;
        attacks[2] = OrangeBossAttacks;
        attacks[3] = PinkBossAttacks;
        attacks[4] = RedBossAttacks;

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
    }

    void Update()
    {
        
    }
}
