using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBossController : MonoBehaviour
{
    [Header("Functional")]
    [Tooltip("How close the player can get before the countdown begins to an attack")]
    [SerializeField] private float AttackRange;
    [SerializeField] private float ConsiderRange;
    [SerializeField] private float maxAttackBuildup;
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
    private Vector2 moveVel; //for smoothly moving the player around

    //variables used to control whether or not the boss attacks
    private bool isAttacking;
    private float attackBuildup;

    private float attackTimeElapsed;

    //variables used while attacking
    private Vector2 attackVector;
    private float attackVel;
    private float curAttackSpeed;

    //variables for pausing
    private bool tempVelSet;
    private Vector2 tempVel;

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

        attackTimeElapsed = 0;
        attackVel = 0;

        tempVelSet = false;
    }

    void Update()
    {
        if (!PauseButton.IsPaused && BossController.Static_Reference.Health > 0)
        {
            //unpause velocity if that's required
            if (tempVelSet)
            {
                rb.velocity = tempVel;
                tempVelSet = false;
            }

            //handle animations
            timeSinceLastBlink += Time.deltaTime;

            if (timeSinceLastBlink >= nextBlinkTime && !isAttacking)
            {
                anims.SetTrigger("Blink");
                timeSinceLastBlink = 0;
                nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);
            }

            //handle attacks
            if (distanceToPlayer() < AttackRange + 1f)
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

            if (isAttacking)
            {
                //spin the particles to make a ring effect
                attackParticles.gameObject.transform.Rotate(Vector3.forward * 5f);

                if (distanceToPlayer() < DamageRange)
                {
                    player.Health -= AttackStrength * Time.deltaTime;
                }

                attackTimeElapsed += Time.deltaTime;
                if (attackTimeElapsed >= AttackTime)
                {
                    //stop attack
                    isAttacking = false;
                    attackBuildup = 0;

                    attackParticles.Stop();
                }
            }

            anims.SetBool("Attacking", isAttacking);
        }
        else if (PauseButton.IsPaused)
        {
            if (!tempVelSet)
            {
                tempVel = rb.velocity;
                tempVelSet = true;
            }

            rb.velocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (!PauseButton.IsPaused && BossController.Static_Reference.Health > 0)
        {
            //handle movements
            if (!isAttacking && distanceToPlayer() > AttackRange)
            {
                Vector2 movement = (player.transform.position - transform.position).normalized * (distanceToPlayer() > ConsiderRange ? MoveSpeed / 2f : MoveSpeed);
                rb.velocity = Vector2.SmoothDamp(rb.velocity, movement, ref moveVel, 0.1f);
            }
            else if (isAttacking)
            {
                curAttackSpeed = Mathf.SmoothDamp(curAttackSpeed, AttackMoveSpeed, ref attackVel, 1.7f);
                rb.velocity = attackVector * curAttackSpeed;
            }
            else
            {
                rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref smoothStopVel, 0.8f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!PauseButton.IsPaused && BossController.Static_Reference.Health > 0)
        {
            if (other.collider.CompareTag("Player"))
            {
                if (player.isDashing)
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
                    if (!player.isDashing)
                        player.Health -= AttackStrength * 1.3f;

                    if (isAttacking)
                    {
                        //bounce off player and bounce the player as well
                        Vector2 bounce = (player.gameObject.transform.position - transform.position).normalized * bounciness;

                        attackVector = -bounce.normalized;
                        player.gameObject.GetComponent<Rigidbody2D>().velocity = bounce;
                        player.KnockBackPlayer(bounce.x);
                    }
                }
            }
            else if (other.collider.CompareTag("Ground") && isAttacking)
            {
                //bounce off of wall
                Vector2 direction = Vector2.Reflect(attackVector, other.contacts[0].normal);
                attackVector = direction.normalized;
            }
        }
    }

    private float distanceToPlayer()
    {
        return Vector2.Distance(player.gameObject.transform.position, transform.position);
    }
}
