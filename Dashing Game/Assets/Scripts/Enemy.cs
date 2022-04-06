﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Experimental.Rendering.Universal;

/*
 * Types of enemies:
 * 0: Melee (red)
 * 1: Shooter (green)
 * 2: Bomber (blue)
 * 3: Laser (yellow)
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

    private float timePerColorChange;
    private float currentTime;
    private bool normalColor;

    private Color32 detonatingColor;
    public ParticleSystem explosionEffect; // belongs in the public variables but it's put here for better organization
    public CameraShake cameraShake;

    private float bomberSpeedChange;
    private float detonationTime;
    private float timePassed;

    //Laser
    private bool ShootingLaser;
    private float LaserRange; //how far the laser enemy has to be from the player until it fires
    private float LaserChargeTime; //how long it takes the laser chare particles to charge
    private float LaserChargeTimeElapsed;

    private float LaserDuration;
    private float LaserDurationElapsed; 

    private float LaserCooldownTime;
    private float LaserCooldownElapsed;
    public GameObject LaserGameObject;
    private GameObject LaserHolder; //for holding the newly instantiated laserholder gameobject
    private bool LaserLocationPicked;

    private Vector3 LaserEndPosition = Vector3.zero; //where the laser will point to 
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

        timePerColorChange = 0.1f; //Changing the colors of the enemy rapidly to tell the player that the enemy will explode
        currentTime = 0;
        normalColor = true;

        detonatingColor = new Color32(0, 0, 255, 255);

        detonationTime = 0.9f; //How much time it takes for the enemy to explode
        timePassed = 0;
        bomberSpeedChange = 1.3f;

        //Laser:
        ShootingLaser = false;
        LaserRange = 7;
        LaserChargeTime = 0.5f;
        LaserChargeTimeElapsed = 0f;

        LaserDuration = 0.2f;
        LaserDurationElapsed = 0f;

        LaserCooldownTime = 1.2f;
        LaserCooldownElapsed = 0;

        LaserLocationPicked = false;
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

                        if (currentTime < timePerColorChange)
                        {
                            currentTime+=Time.deltaTime;
                        }
                        else
                        {
                            //swap colors
                            if (!normalColor)
                                sprite.color = Colors[Type];
                            else
                                sprite.color = detonatingColor;

                            normalColor = !normalColor;

                            currentTime = 0;
                        }
                    }
                    else
                    {
                        path.maxSpeed = speed;
                    }

                    break;

                case 3:
                    //Laser
                    if(distanceTo(playerGameObject) < LaserRange)
                    {
                        path.maxSpeed = 0;
                    }
                    else if(!ShootingLaser)
                    {
                        path.maxSpeed = speed;
                    }
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

                //Laser type
                case 3:
                    /*
                     *     private bool ShootingLaser;
    private float LaserRange; //how far the laser enemy has to be from the player until it fires
    private float LaserChargeTime; //how long it takes the laser chare particles to charge
    private float LaserChargeTimeElapsed;

    private float LaserDuration;
    private float LaserDurationElapsed; 

    private float LaserCooldownTime;
    private float LaserCooldownElapsed;
    public GameObject LaserLine;
    private GameObject LaserHolder; //for holding the newly instantiated laserholder
    private bool LaserLocationPicked;

    private Vector3 LaserEndPosition = Vector3.zero; //where the laser will point to 
                     */

                    if(distanceTo(playerGameObject) < LaserRange && LaserCooldownElapsed > LaserCooldownTime && !ShootingLaser)
                    {
                        //shooting the laser if the player is close enough to the enemy and the laser isn't on cooldown
                        ShootingLaser = true;

                        LaserCooldownElapsed = 0;    //putting the laser on cooldown again
                        LaserChargeTimeElapsed = 0;  //making the laser go through another recharge cycle (play particles)
                        LaserDurationElapsed = 0;    //laser has not started firing yet
                    }
                    else
                    {
                        //cooldown the laser
                        LaserCooldownElapsed += Time.deltaTime;
                    }

                    //shooting code is seperate for better readability
                    if(ShootingLaser)
                    {
                        if(LaserChargeTimeElapsed > LaserChargeTime)
                        {
                            //if the laser is done charging, shoot until the laser duration is done
                            if(LaserDurationElapsed < LaserDuration)
                            {
                                //fire the laser
                                if (LaserHolder == null)
                                    LaserHolder = Instantiate(LaserGameObject, Vector3.zero, );

                                LaserDurationElapsed += Time.deltaTime;
                            }
                            else
                            {
                                ShootingLaser = false; //laser is done firing
                            }
                        }
                        else
                        {
                            //charge the laser (play particles)
                            LaserChargeTimeElapsed += Time.deltaTime;
                        }
                    }

                    /*
                    if(ShootingLaser)
                    {
                        //charge the laser, stop the enemy from moving
                        path.maxSpeed = 0;
                        Debug.Log("reeee");
                        if(LaserChargeTimeElapsed < LaserChargeTime)
                        {
                            Debug.Log("doin the thing");
                            //charge laser (allow time for particle effect to finish playing)
                            LaserChargeTimeElapsed += Time.deltaTime;
                            LaserLocationPicked = false; //calculate a new position
                            LaserDurationElapsed = 0; //reset the duration of the laser because it hasn't started yet
                        }
                        else
                        {
                            //play particle animations

                            //shoot the laser
                            if(LaserHolder == null)
                                LaserHolder = Instantiate(LaserLine, Vector2.zero, Quaternion.identity); //make a new laser object if one doesn't already exist

                            //calculate where the player is only if there isn't a location picked already
                            if (!LaserLocationPicked)
                            {
                                Vector3 dir = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, 0);
                                //dir = dir.normalized;

                                //calculate where the lazer should stop using raycasting
                                //RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
                                //dir *= hit.distance;
                                LaserEndPosition = new Vector3(dir.x + transform.position.x, dir.y + transform.position.y, 0) * 1000;
                            }
                            Debug.Log("Firing");
                            //keep the laser firing until the duration ends
                            if (LaserDurationElapsed < LaserDuration)
                            {
                                LaserHolder.SetPosition(0, transform.position);
                                LaserHolder.SetPosition(1, LaserEndPosition);
                                LaserDurationElapsed += Time.deltaTime; //the laser keeps firing
                            }
                            else
                            {
                                ShootingLaser = false; //stop firing the laser

                                Debug.Log("done");
                                LaserHolder.SetPosition(0, Vector3.zero);
                                LaserHolder.SetPosition(0, Vector3.zero);
                            }
                        }
                    }
                    else
                    {
                        //reset laser duration so it can fire to its entirety next time it's fired
                        LaserDurationElapsed = 0;

                        //laser cooldown, keeps the enemy from firing over and over again
                        if(distanceTo(playerGameObject) <= LaserRange && LaserCooldownTime >= LaserCooldownElapsed)
                        {
                            ShootingLaser = true;
                            LaserChargeTimeElapsed = 0; //make the laser charge up again

                            //start particle animations for charging
                        }
                        else
                        {
                            LaserCooldownElapsed += Time.deltaTime; //recharge the laser as the enemy is free to move around again (cooldown)
                        }

                        //stop the player from moving if it's a certain distance from the player, otherwise keep moving
                        if(distanceTo(playerGameObject) <= LaserRange)
                            path.maxSpeed = 0;
                        else
                            path.maxSpeed = speed;
                    }
                    */
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
        Colors.Add(new Color32(255, 255, 0, 255));   //yellow             (Laser Enemy)
    }

    private bool isTouching(Collider2D target)
    {
        Collider2D col = gameObject.GetComponent<Collider2D>();
        return col.IsTouching(target);
    }

    void OnDestroy()
    {
        //get rid of objects that are no longer needed
        if(LaserHolder != null)
            Destroy(LaserHolder);
    }
}