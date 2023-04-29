using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BossController : MonoBehaviour
{
    public static BossController Static_Reference;

    [Header("Things that need to be assigned by developer:")]
    [SerializeField] private TextMeshProUGUI DisplayText;

    [Space]

    [Header("The actual boss in the level")]
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject health_bar;

    [Header("Boss stats (change these per boss)")]
    [SerializeField] private float minAttackDelay;
    [SerializeField] private float maxAttackDelay;
    [SerializeField] private float startHealth;
    [Tooltip("How close the player needs to be before the boss becomes aggressive")]
    [SerializeField] private float angryRange;

    [Space]

    [SerializeField] private GameObject[] attacks;

    [Header("Stats that effect the player")]

    [Tooltip("Maximum amount of damage the player can do to the boss")]
    [SerializeField] private float playerDamage; //how much the player can damage the enemy

    [Tooltip("How much money the player can earn (based off of remaining health)")]
    [SerializeField] private float maxMoneyPossible;

    private Player player;
    private Animator bossAnims;
    private Slider health_bar_slider;

    private float _health;

    private float timeSinceLastAttack;
    private float nextAttackTime;
    private int bossType;

    private float player_power; //how powerful the player is

    public float Health
    {
        get { return _health; }
    }

    void Awake()
    {
        Static_Reference = this;

        switch (SceneManager.GetActiveScene().name)
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
        health_bar_slider = health_bar.GetComponentInChildren<Slider>();
        bossAnims = boss.GetComponent<Animator>();
        _health = startHealth;
        nextAttackTime = Random.Range(minAttackDelay, maxAttackDelay);

        //calculate how strong the player currently is
        PlayerData data = Saver.loadData();

        player_power = (data.SpeedUpgrade + data.MaxHealthUpgrade + data.MaxDashUpgrade + data.DashRechargeUpgrade + data.JumpHeightUpgrade) / 25f; //divided by 25 because that's the maximum value all the upgrades can add up to
        if (player_power == 0) //hah noob playing
            player_power += 0.1f;
    }

    void Update()
    {
        //handle attacks if the boss uses external attacks
        if(attacks.Length != 0 && _health > 0 && !PauseButton.IsPaused)
        {
            timeSinceLastAttack += Time.deltaTime;

            if(timeSinceLastAttack >= nextAttackTime)
            {
                int randomIndex = Random.Range(0, attacks.Length);
                Instantiate(attacks[randomIndex], player.gameObject.transform.position, Quaternion.identity);

                nextAttackTime = Random.Range(minAttackDelay, maxAttackDelay);
                timeSinceLastAttack = 0;
            }
        }
        else if(_health <= 0)
        {
            //give the player money


            //kill the player

            Destroy(gameObject);
        }

        //handle boss animations
        bossAnims.SetBool("IsAngry", Vector2.Distance(boss.transform.position, player.gameObject.transform.position) < angryRange);
    }

    void LateUpdate()
    {
        health_bar.transform.position = boss.transform.position;
        health_bar_slider.value = _health / startHealth;
    }

    public void Damage()
    {
        if (_health > 0)
        {
            _health -= player_power * playerDamage;

            bossAnims.SetTrigger("Damage");
        }
    }
}
