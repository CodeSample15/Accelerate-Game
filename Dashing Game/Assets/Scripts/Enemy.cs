using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    //Public
    [SerializeField] public Player player;
    [SerializeField] public BulletController bulletController;
    [SerializeField] public GameObject playerGameObject;
    [SerializeField] public Animator PlayerDamageAnimation;

    public GameObject GamePlayer;
    public EnemyController enemyController;

    public int Type;

    //Private
    private Rigidbody2D rb;
    private AIPath path;

    //Damage Data
    private float MeleeDamage;
    private float MeleeAttackSpeed;

    private float ShooterDamage; //TODO: SHOOTER TYPE ENEMY
    private float ShooterAttackSpeed;

    private float ShooterRange;

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

        #region Stats
        //Melee:
        MeleeDamage = 10;
        MeleeAttackSpeed = 5;

        //Shooter:
        ShooterDamage = 4;
        ShooterAttackSpeed = 3;
        ShooterRange = 8f;

        InRange = false;
        #endregion

        speed = 9;

        timeSinceLastAttack = 0f; //Starts off being able to attack right away
    }

    void Update()
    {
        if (!player.isAlive)
            Destroy(gameObject);

        //Controlling the stats and movement of the enemy depending on what type it is
        switch(Type)
        {
            case 0:
                //Melee
                AttackSpeed = MeleeAttackSpeed;
                break;

            case 1:
                AttackSpeed = ShooterAttackSpeed;
                projectileSpeed = 5f;

                if(distanceTo(playerGameObject) < ShooterRange)
                {
                    //rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    path.maxSpeed = 0;
                    InRange = true;
                }
                else
                {
                    //rb.constraints = RigidbodyConstraints2D.None;
                    path.maxSpeed = speed;
                    InRange = false;
                }

                break;
        }

        //Attacking
        if(timeSinceLastAttack >= AttackSpeed)
        {
            //Attack
            switch (Type)
            {
                //melee type
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

                //shooter type
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

    private bool isTouching(Collider2D target)
    {
        Collider2D col = gameObject.GetComponent<Collider2D>();
        return col.IsTouching(target);
    }
}