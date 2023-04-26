using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBossController : MonoBehaviour
{
    [Header("Functional")]
    [Tooltip("How close the player can get before the countdown begins to an attack")]
    [SerializeField] private float AttackRange;
    [SerializeField] private float ConsiderRange;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float AttackTime;

    [Tooltip("How much damage the boss can deal to the player")]
    [SerializeField] private float AttackStrength;
    [Tooltip("How close the player can get to the boss without being damaged while attacking")]
    [SerializeField] private float DamageRange;

    [Tooltip("How fast the boss moves while attacking")]
    [SerializeField] private float AttackMoveSpeed;

    [Header("Cosmetic")]
    [SerializeField] private float minBlinkTime;
    [SerializeField] private float maxBlinkTime;
    [SerializeField] private float bounciness;

    private Animator anims;
    private Player player;
    private Rigidbody2D rb;
    private ParticleSystem attackParticles;
    private float timeSinceLastBlink;

    private float nextBlinkTime;

    private Vector2 smoothStopVel; //for smoothly stopping the boss from moving

    //variables used to control whether or not the boss attacks
    private bool isAttacking;
    private float attackBuildup;
    private float maxAttackBuildup;

    private float attackTimeElapsed;

    //variables used while attacking
    private Vector2 attackVector;
    private float attackVel;
    private float curAttackSpeed;

    void Awake()
    {
        anims = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();

        attackParticles = GetComponentInChildren<ParticleSystem>();
        attackParticles.Stop();

        timeSinceLastBlink = 0;
        nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);

        smoothStopVel = Vector2.zero;

        isAttacking = false;
        attackBuildup = 0;
        maxAttackBuildup = 2f;

        attackTimeElapsed = 0;
        attackVel = 0;
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
        if(distanceToPlayer() < AttackRange + 1f)
            attackBuildup += Time.deltaTime;
        else
            attackBuildup = 0;

        if (attackBuildup > maxAttackBuildup && !isAttacking)
        {
            //start attack
            isAttacking = true;
            attackTimeElapsed = 0f;

            curAttackSpeed = 0;
            attackVel = 0;

            attackVector = (player.transform.position - transform.position);

            attackParticles.Play();
            attackParticles.gameObject.transform.rotation.eulerAngles.Set(0, 0, 0); //reset particle rotation
        }

        if(isAttacking)
        {
            //spin the particles to make a ring effect
            attackParticles.gameObject.transform.Rotate(Vector3.forward * 5f);

            if (distanceToPlayer() < DamageRange)
            {
                player.Health -= AttackStrength * Time.deltaTime;
            }

            attackTimeElapsed += Time.deltaTime;
            if(attackTimeElapsed >= AttackTime)
            {
                //stop attack
                isAttacking = false;
                attackBuildup = 0;

                attackParticles.Stop();
            }
        }

        anims.SetBool("Attacking", isAttacking);
    }

    void FixedUpdate()
    {
        //handle movements
        if (!isAttacking && distanceToPlayer() > AttackRange)
        {
            Vector2 movement = (player.transform.position - transform.position).normalized * (distanceToPlayer() > ConsiderRange ? MoveSpeed/2f : MoveSpeed);
            rb.velocity = movement;
        }
        else if(isAttacking)
        {
            curAttackSpeed = Mathf.SmoothDamp(curAttackSpeed, AttackMoveSpeed, ref attackVel, 2f);
            rb.velocity = attackVector * curAttackSpeed;
        }
        else
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref smoothStopVel, 0.8f);
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
                player.Health -= AttackStrength*1.5f; //do a little more damage to punish the player for thinking they can just touch the boss
            }
        }
        else if(other.collider.CompareTag("Ground") && isAttacking)
        {
            //bounce off of wall
            Vector2 direction = Vector2.Reflect(attackVector, other.contacts[0].normal);
            attackVector = direction.normalized;
        }
    }

    private float distanceToPlayer()
    {
        return Vector2.Distance(player.gameObject.transform.position, transform.position);
    }
}
