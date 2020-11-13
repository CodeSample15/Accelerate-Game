using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //public
    [SerializeField] public Player player;
    [SerializeField] public Animator PlayerDamageAnimation;

    public GameObject GamePlayer;
    public EnemyController enemyController;

    public int Type;

    //Damage Data
    private float MeleeDamage;
    private float MeleeAttackSpeed;

    private float ShooterDamage; //TODO: SHOOTER TYPE ENEMY
    private float ShooterAttackSpeed;

    private float AttackSpeed;

    //Time Data
    public float timeSinceLastAttack;

    void Awake()
    {
        #region Stats
        //Melee:
        MeleeDamage = 10;
        MeleeAttackSpeed = 5;

        //Shooter:
        ShooterDamage = 4;
        ShooterAttackSpeed = 3;
        #endregion

        timeSinceLastAttack = 0f; //Starts off being able to attack right away
    }

    void Update()
    {
        if (!player.isAlive)
            Destroy(gameObject);

        //Controlling the stats of the enemy depending on what type it is
        switch(Type)
        {
            case 0:
                //Melee
                AttackSpeed = MeleeAttackSpeed;
                break;

            case 1:
                AttackSpeed = ShooterAttackSpeed;
                break;
        }

        //Attacking
        if(timeSinceLastAttack >= AttackSpeed)
        {
            //Attack
            switch (Type)
            {
                //melee type
                case 1:
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
                case 2:
                    break;
            }
        }
        else
        {
            //Refill attack cooldown
            timeSinceLastAttack += Time.deltaTime;
        }
    }

    private bool isTouching(Collider2D target)
    {
        Collider2D col = gameObject.GetComponent<Collider2D>();
        return col.IsTouching(target);
    }
}