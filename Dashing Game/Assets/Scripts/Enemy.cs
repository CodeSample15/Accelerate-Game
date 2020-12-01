using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Experimental.Rendering.LWRP;

public class Enemy : MonoBehaviour
{
    //Public
    [SerializeField] public Player player;
    [SerializeField] public BulletController bulletController;
    [SerializeField] public GameObject playerGameObject;
    [SerializeField] public Animator PlayerDamageAnimation;

    public GameObject GamePlayer;
    public EnemyController enemyController;

    public List<Color32> Colors;

    public int Type;

    //Private
    private Rigidbody2D rb;
    private AIPath path;
    private SpriteRenderer sprite;
    private Light2D light;

    #region Enemy Data
    //Melee
    private float MeleeDamage;
    private float MeleeAttackSpeed;

    //Shooter
    private float ShooterDamage;
    private float ShooterAttackSpeed;

    private float ShooterRange;

    //Bomber
    private float BomberDamage;
    private float Radius;

    private bool Detonating;

    private int framesPerColorChange;
    private int currentFrame;

    private float detonationTime;
    private float timePassed;

    //Ghost
    private float GhostDamage;
    private float GhostAttackSpeed;
    #endregion

    private float AttackSpeed;

    private bool InRange;
    private float projectileSpeed;

    //Time Data
    public float timeSinceLastAttack;

    //Movement Data
    private float speed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        light = GetComponentInChildren<Light2D>();

        #region Stats
        //Melee:
        MeleeDamage = 15;
        MeleeAttackSpeed = 2;

        //Shooter:
        ShooterDamage = 5;
        ShooterAttackSpeed = 1f;
        ShooterRange = 8f;

        //Bomber:
        BomberDamage = 30; //Damage at the center of the explosion (damage decreases with distance)
        Radius = 10; //Blast radius

        Detonating = false; //Telling the enemy how to move when detonating

        framesPerColorChange = 3; //Changing the colors of the enemy rapidly to tell the player that the enemy will explode
        currentFrame = 0;

        detonationTime = 0; //How much time it takes for the enemy to explode
        timePassed = 0;

        //Ghost:
        GhostDamage = 1;
        GhostAttackSpeed = 0.1f;

        InRange = false;
        #endregion

        speed = 9;

        initColors();

        timeSinceLastAttack = 0f; //Starts off being able to attack right away
    }

    void Update()
    {
        if (!player.isAlive)
            Destroy(gameObject);

        //Changing the color of the sprite and light of the enemy
        sprite.color = Colors[Type];
        light.color = Colors[Type];

        //Controlling the stats and movement of the enemy depending on what type it is---------------------------------------------------------------------------------
        switch(Type)
        {
            case 0:
                //Melee
                AttackSpeed = MeleeAttackSpeed;
                break;

            case 1:
                //Shooter
                AttackSpeed = ShooterAttackSpeed;
                projectileSpeed = 5f;

                if(distanceTo(playerGameObject) < ShooterRange)
                {
                    path.maxSpeed = 0;
                    InRange = true;
                }
                else
                {
                    path.maxSpeed = speed;
                    InRange = false;
                }
                break;

            case 2:
                //Bomber
                if (Detonating)
                {
                    path.maxSpeed = speed / 3;

                    if(currentFrame < framesPerColorChange)
                    {
                        //TODO: COLOR CHANGES AND EXPLOSION
                        currentFrame++;
                    }
                    else
                    {
                        currentFrame = 0;
                    }
                }
                else
                {
                    path.maxSpeed = speed;
                }

                break;
        }

        //Attacking-----------------------------------------------------------------------------------------------------
        if(timeSinceLastAttack >= AttackSpeed)
        {
            //Attack
            switch (Type)
            {
                //Melee type
                case 0:
                    Collider2D playerCol = GamePlayer.gameObject.GetComponent<Collider2D>();

                    if (isTouching(playerCol) && !GamePlayer.GetComponent<Player>().isDashing)
                    {
                        //the player isn't dashing, so the enemy can attack
                        PlayerDamageAnimation.SetTrigger("Damage");
                        GamePlayer.GetComponent<Player>().Health -= MeleeDamage;

                        timeSinceLastAttack = 0f; //attacked, so the cooldown restarts
                    }

                    break;

                //Shooter type
                case 1:
                    if (InRange)
                    {
                        //Calculating what direction the bullet should travel in
                        Vector2 shootDir = new Vector2(transform.position.x - playerGameObject.transform.position.x, transform.position.y - playerGameObject.transform.position.y);
                        shootDir *= -1;
                        shootDir = shootDir.normalized;

                        //Using the Bullet Controller script to instantiate a new bullet
                        bulletController.shoot(transform.position, shootDir, playerGameObject.transform.position, ShooterDamage, projectileSpeed, ShooterRange + 10, 1);

                        timeSinceLastAttack = 0f; //restart cooldown
                    }
                    break;

                //Bomber type
                case 2:
                    if(distanceTo(playerGameObject) <= Radius)
                    {
                        Detonating = true;
                    }
                    break;
            }
        }
        else
        {
            //Refill attack cooldown
            timeSinceLastAttack += Time.deltaTime;
        }
    }

    private float distanceTo(GameObject other)
    {
        float x1 = other.transform.position.x;
        float y1 = other.transform.position.y;
        float x2 = transform.position.x;
        float y2 = transform.position.y;

        x1 = x1 - x2;
        y1 = y1 - y2;

        x1 *= x1;
        y1 *= y1;

        return Mathf.Sqrt(x1 + y1);
    }

    private void initColors()
    {
        Colors = new List<Color32>();

        Colors.Add(new Color32(255, 0, 0, 255));     //Red                (Melee Enemy)
        Colors.Add(new Color32(0, 255, 0, 255));     //Green              (Shooter Enemy)
        Colors.Add(new Color32(22, 174, 250, 255));  //Blue               (Bomber Enemy)
        Colors.Add(new Color32(255, 255, 255, 150)); //Transparent White  (Ghost Enemy)
    }

    private bool isTouching(Collider2D target)
    {
        Collider2D col = gameObject.GetComponent<Collider2D>();
        return col.IsTouching(target);
    }
}