using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Experimental.Rendering.Universal;

/*
 * Types of enemies:
 * 0: Melee (red)
 * 1: Shooter (green)
 * 2: Bomber (blue)
 * 3: Lobber (yellow) shoots explosive charges, no idea why I named it lobber lmao
 * 3: Ghost (clear) <-- I want to delete this one it sucks beyond compare lol
 */

public class Enemy : MonoBehaviour
{
    //Public
    [SerializeField] public Player player;
    [SerializeField] public BulletController bulletController;
    [SerializeField] public GameObject playerGameObject;
    [SerializeField] public Animator PlayerDamageAnimation;
    
    public WaveController enemyController;
    public ParticleController particleController;

    public List<Color32> Colors;

    public int Type;

    //Private
    private Rigidbody2D rb;
    private AIPath path;
    private SpriteRenderer sprite;

    #region Enemy Data
    //Melee
    private float MeleeDamage;
    private float MeleeAttackSpeed;

    //Shooter
    private float ShooterDamage;
    private float ShooterAttackSpeed;

    private float ShooterRange;
    private float ShooterBulletSpeed;

    //Bomber
    private float BomberDamage;
    private float Radius;
    private float BomberDamageDampener;

    private bool Detonating;

    private int framesPerColorChange;
    private int currentFrame;
    private bool normalColor;

    private Color32 detonatingColor;
    public ParticleSystem explosionEffect; // belongs in the public variables but it's put here for better organization
    public CameraShake cameraShake;

    private float bomberSpeedChange;
    private float detonationTime;
    private float timePassed;

    //Lobber
    #endregion

    private float AttackSpeed;

    private bool InRange;

    //Time Data
    public float timeSinceLastAttack;

    //Movement Data
    private float speed;

    void Awake()
    {
        path = GetComponent<AIPath>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        #region Stats
        //Melee:
        MeleeDamage = 15;
        MeleeAttackSpeed = 2;

        //Shooter:
        ShooterDamage = 5;
        ShooterAttackSpeed = 0.8f;
        ShooterRange = 8f;
        ShooterBulletSpeed = 8f;

        //Bomber:
        BomberDamage = 25; //Damage at the center of the explosion (damage decreases with distance)
        Radius = 4.7f; //Blast radius
        BomberDamageDampener = 6; //Controlling how limited the bomber's damage is to the distance of the player

        Detonating = false; //Telling the enemy how to move when detonating

        framesPerColorChange = 10; //Changing the colors of the enemy rapidly to tell the player that the enemy will explode
        currentFrame = 0;
        normalColor = true;

        detonatingColor = new Color32(0, 0, 255, 255);

        detonationTime = 0.9f; //How much time it takes for the enemy to explode
        timePassed = 0;
        bomberSpeedChange = 1.3f;

        //Lobber:

        #endregion

        speed = 9;

        initColors();

        timeSinceLastAttack = 0f; //Starts off being able to attack right away
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!player.isAlive || player == null)
            Destroy(gameObject);

        //Controlling the stats and movement of the enemy depending on what type it is---------------------------------------------------------------------------------
        if (!PauseButton.IsPaused) //will only move if the game is unpaused
        {
            //setting the speed of the enemy back to normal (if it was paused)
            path.maxSpeed = speed;

            switch (Type)
            {
                case 0:
                    //Melee
                    AttackSpeed = MeleeAttackSpeed;
                    break;

                case 1:
                    //Shooter
                    {
                        AttackSpeed = ShooterAttackSpeed;

                        //calculate if the player is in the enemy's line of sight
                        Vector3 dir = (transform.position - player.transform.position).normalized * -1;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distanceTo(playerGameObject) - 1f);

                        if (distanceTo(playerGameObject) < ShooterRange && hit.collider == null)
                        {
                            path.maxSpeed = 0;
                            InRange = true;
                        }
                        else
                        {
                            path.maxSpeed = speed;
                            InRange = false;
                        }
                    }
                    break;

                case 2:
                    //Bomber
                    if (Detonating)
                    {
                        path.maxSpeed = speed / bomberSpeedChange;

                        if (currentFrame < framesPerColorChange)
                        {
                            currentFrame++;
                        }
                        else
                        {
                            //swap colors
                            if (!normalColor)
                                sprite.color = Colors[Type];
                            else
                                sprite.color = detonatingColor;

                            normalColor = !normalColor;

                            currentFrame = 0;
                        }
                    }
                    else
                    {
                        path.maxSpeed = speed;
                    }

                    break;

                case 3:
                    break;
            }
        }
        else
        {
            path.maxSpeed = 0;
        }

        //Attacking-----------------------------------------------------------------------------------------------------
        if((timeSinceLastAttack >= AttackSpeed) && !PauseButton.IsPaused) //will always attack if the enemy is a ghost and won't attack if the game is paused
        {
            //Attack
            switch (Type)
            {
                //Melee type
                case 0:
                    Collider2D playerCol = playerGameObject.gameObject.GetComponent<Collider2D>();

                    if (isTouching(playerCol) && !playerGameObject.GetComponent<Player>().isDashing)
                    {
                        //the player isn't dashing, so the enemy can attack
                        PlayerDamageAnimation.SetTrigger("Damage");
                        playerGameObject.GetComponent<Player>().Health -= MeleeDamage;

                        timeSinceLastAttack = 0f; //attacked, so the cooldown restarts
                    }
                    else
                    {
                        timeSinceLastAttack = AttackSpeed - 0.5f;
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
                        bulletController.shoot(transform.position, shootDir, playerGameObject.transform.position, ShooterDamage, ShooterBulletSpeed, ShooterRange + 10, 1);

                        timeSinceLastAttack = 0f; //restart cooldown
                    }
                    break;

                //bomber type
                case 2:
                    if(distanceTo(playerGameObject) <= Radius && !Detonating)
                    {
                        Detonating = true;
                    }

                    if (Detonating)
                    {
                        if(timePassed > detonationTime)
                        {
                            //explode-----------------------------------------------------------------------

                            //make new explosion particles and move them to the location of the enemy
                            particleController.AddParticles(Instantiate(explosionEffect, transform.position, Quaternion.Euler(new Vector3(90,0,0))));

                            //damage place based off of distance from explosion location
                            float damage = Mathf.Clamp(BomberDamage - (distanceTo(playerGameObject) * BomberDamageDampener), 0, BomberDamage); //calculating damage

                            if (damage > 0 && distanceTo(playerGameObject) < Radius/2)
                            {
                                PlayerDamageAnimation.SetTrigger("Damage");
                                playerGameObject.GetComponent<Player>().Health -= damage;
                            }

                            cameraShake.Shake(); //some visual feedback that an explosion just happened

                            Destroy(gameObject); //destroy the bomber
                        }
                        else
                        {
                            timePassed += Time.deltaTime;
                        }
                    }
                    break;

                //lobber type
                case 3:
                    break;
            }
        }
        else
        {
            //Refill attack cooldown
            if(!PauseButton.IsPaused)
                timeSinceLastAttack += Time.deltaTime;
        }
    }

    //public method for other scripts to access the different colors the enemies have
    public Color getColor(int num)
    {
        initColors();

        if (num < 0 || num >= Colors.Count)
            return new Color(0,0,0); //return black if the color is not found

        return Colors[num];
    }

    public void Colorize()
    {
        //Changing the color of the sprite and light of the enemy
        sprite.color = Colors[Type];
        GetComponentInChildren<Light2D>().color = Colors[Type];
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
        Colors.Add(new Color32(255, 255, 0, 255));   //yellow             (Lobber Enemy)
        Colors.Add(new Color32(200, 200, 200, 150)); //Transparent White  (Ghost Enemy)
    }

    private bool isTouching(Collider2D target)
    {
        Collider2D col = gameObject.GetComponent<Collider2D>();
        return col.IsTouching(target);
    }
}