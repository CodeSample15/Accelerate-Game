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
    [SerializeField] private ParticleSystem BossDeathParticle;

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
    [Tooltip("How long the attack animation is before the boss can attack")]
    [SerializeField] private float attackAnimationTime;

    [Space]

    [SerializeField] private GameObject[] attacks;

    [Header("Stats that effect the player")]

    [Tooltip("Maximum amount of damage the player can do to the boss")]
    [SerializeField] private float playerDamage; //how much the player can damage the enemy

    [Tooltip("How much money the player can earn (based off of remaining health)")]
    [SerializeField] private float maxMoneyPossible;

    private CameraShake CamShake;

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

    public GameObject Boss
    {
        get { return boss; }
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

        CamShake = Camera.main.GetComponent<CameraShake>();

        player = FindObjectOfType<Player>();
        health_bar_slider = health_bar.GetComponentInChildren<Slider>();
        bossAnims = boss.GetComponent<Animator>();
        _health = startHealth;
        nextAttackTime = Random.Range(minAttackDelay, maxAttackDelay) / 2; //have half as much time before the first attack

        //calculate how strong the player currently is
        PlayerData data = Saver.loadData();

        player_power = (data.SpeedUpgrade + data.MaxHealthUpgrade + data.MaxDashUpgrade + data.DashRechargeUpgrade + data.JumpHeightUpgrade) / 25f; //divided by 25 because that's the maximum value all the upgrades can add up to
        if (player_power == 0) //hah noob playing
            player_power += 0.1f;
    }

    void Update()
    {
        //handle attacks if the boss uses external attacks
        if((attacks.Length != 0 || (bossType==4 && !boss.GetComponent<RedBossController>().IsAttacking)) && _health > 0 && !PauseButton.IsPaused)
        {
            timeSinceLastAttack += Time.deltaTime;

            if(timeSinceLastAttack >= nextAttackTime)
            {
                StartCoroutine(spawnAttack());

                nextAttackTime = Random.Range(minAttackDelay, maxAttackDelay);
                timeSinceLastAttack = 0;
            }
        }
        else if(_health <= 0)
        {
            //give the player money
            int moneyAdded = (int)(maxMoneyPossible * (player.Health / Player.MaxHealth));

            PlayerData data = Saver.loadData();
            data.Money += moneyAdded;
            
            //update player data
            switch(bossType)
            {
                case 0:
                    data.BlueBossDefeated = true;
                    break;

                case 1:
                    data.GreenBossDefeated = true;
                    break;

                case 2:
                    data.OrangeBossDefeated = true;
                    break;

                case 3:
                    data.PinkBossDefeated = true;
                    break;

                case 4:
                    data.RedBossDefeated = true;
                    break;
            }

            Saver.SavePlayer(data);

            //kill the boss
            BossDeathParticle.gameObject.transform.position = boss.transform.position;
            BossDeathParticle.Play();

            boss.SetActive(false);
            health_bar.SetActive(false);

            //kill the player
            DisplayText.fontSize = 42f;
            DisplayText.SetText("Boss Defeated!");
            player.scoreText.SetText("");

            player.gameObject.SetActive(false);
            Cursor.visible = true;

            player.menu_animations.SetTrigger("FadeIn");
            player.score_animation.SetTrigger("Move");

            //enable the menu buttons
            MenuLogic.buttonsActive = true;

            //start the animation for the amount of money being added the player's balance
            player.money_add_animation.runAnimation(0.7f, (int)(moneyAdded * 0.6f), moneyAdded);

            Destroy(gameObject);
        }

        //handle boss animations
        bossAnims.SetBool("IsAngry", Vector2.Distance(boss.transform.position, player.gameObject.transform.position) < angryRange);
    }

    void LateUpdate()
    {
        float yOffset = 0;

        if (bossType == 1)
            yOffset = 0.6f;
        else if (bossType == 2 || bossType == 3)
            yOffset = 0.9f;
        else if (bossType == 4)
            yOffset = 3.5f;

        health_bar.transform.position = new Vector2(boss.transform.position.x, boss.transform.position.y + yOffset);
        health_bar_slider.value = _health / startHealth;
    }

    public float Damage()
    {
        if (_health > 0)
        {
            float damage = player_power * playerDamage;
            _health -= damage;
            CamShake.Shake();

            bossAnims.SetTrigger("Damage");

            return damage;
        }

        return 0;
    }

    //for red boss to gradually lose health
    public float Damage(float amount)
    {
        if (_health > 0)
        {
            float damage = player_power * amount;
            _health -= damage;

            return damage;
        }

        return 0;
    }

    IEnumerator spawnAttack()
    {
        bossAnims.SetTrigger("Attack");

        yield return new WaitForSeconds(attackAnimationTime);

        if (bossType != 4)
        {
            int randomIndex = Random.Range(0, attacks.Length);
            Instantiate(attacks[randomIndex], player.gameObject.transform.position, Quaternion.identity);

            if (bossType == 2)
            {
                boss.GetComponent<OrangeBossController>().attack();
            }
            else if (bossType == 3)
            {
                boss.GetComponent<PinkBossController>().attack();
            }
        }
        else
        {
            boss.GetComponent<RedBossController>().attack(); //all attacks are being handled by the red boss controller script
        }
    }
}
