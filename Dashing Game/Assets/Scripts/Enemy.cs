using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //public
    [SerializeField] public Animator PlayerDamageAnimation;
    [SerializeField] public float AttackSpeed;

    public GameObject GamePlayer;
    public EnemyController enemyController;

    public int Type;

    private bool isAttacking;

    private float MeleeDamage = 10;
    private float ShooterDamage = 4; //TODO: SHOOTER TYPE ENEMY

    void Awake()
    {
        isAttacking = false;
    }

    void Update()
    {
        if(!isAttacking)
        {
            StartCoroutine(attack());
            isAttacking = true;
        }
    }

    IEnumerator attack()
    {
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
                }

                break;

            //shooter type
            case 2:
                break;
        }

        yield return new WaitForSeconds(AttackSpeed);

        isAttacking = false;
    }

    private bool isTouching(Collider2D target)
    {
        Collider2D col = gameObject.GetComponent<Collider2D>();
        return col.IsTouching(target);
    }
}