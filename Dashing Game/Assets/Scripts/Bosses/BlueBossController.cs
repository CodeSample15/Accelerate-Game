using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBossController : MonoBehaviour
{
    [Header("Functional")]
    [SerializeField] private float AttackRange;
    [SerializeField] private float ConsiderRange;
    [SerializeField] private float MoveSpeed;

    [Tooltip("How much damage the boss can deal to the player")]
    [SerializeField] private float AttackStrength;

    [Header("Cosmetic")]
    [SerializeField] private float minBlinkTime;
    [SerializeField] private float maxBlinkTime;
    [SerializeField] private float bounciness;

    private Animator anims;
    private Player player;
    private Rigidbody2D rb;
    private float timeSinceLastBlink;

    private float nextBlinkTime;

    private bool isAttacking;
    private float attackBuildup;
    private float maxAttackBuildup;

    void Awake()
    {
        anims = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();

        timeSinceLastBlink = 0;
        nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);

        isAttacking = false;
        attackBuildup = 0;
        maxAttackBuildup = 0.7f;
    }

    void Update()
    {
        //handle animations
        timeSinceLastBlink += Time.deltaTime;

        if(timeSinceLastBlink >= nextBlinkTime)
        {
            anims.SetTrigger("Blink");
            timeSinceLastBlink = 0;
            nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);
        }

        //handle attacks
        if()
    }

    void FixedUpdate()
    {
        //handle movements
        if (!isAttacking && Vector2.Distance(player.gameObject.transform.position, transform.position) > AttackRange)
        {
            Vector2 movement = (player.transform.position - transform.position).normalized * MoveSpeed;
            rb.velocity = movement;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Player"))
        {
            if(player.isDashing)
            {
                BossController.Static_Reference.Damage();

                //calculate bounce-back vector
                Vector2 bounce = (player.gameObject.transform.position - transform.position).normalized * bounciness;

                player.DashPower = 0;
                player.gameObject.GetComponent<Rigidbody2D>().velocity = bounce;
                player.KnockBackPlayer(bounce.x);
            }
            else
            {
                //damage player
            }
        }
    }
}
