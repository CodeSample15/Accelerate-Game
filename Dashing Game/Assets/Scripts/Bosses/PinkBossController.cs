using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkBossController : MonoBehaviour
{
    [Space]
    [Header("Cosmetic")]
    [SerializeField] private GameObject eye;
    [SerializeField] private float minBlinkTime;
    [SerializeField] private float maxBlinkTime;
    [SerializeField] private float eyeFocusDistance;
    [SerializeField] private float maxEyeOffset;

    private Player player;
    private SpikeAttack spiralAttack;

    private bool isAttacking;

    //animation related things
    private Animator anims;
    private float timeSinceLastBlink;
    private float nextBlinkTime;

    private Vector2 eyeOffset;
    private Vector2 eyeOffsetVel;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        anims = GetComponent<Animator>();
        spiralAttack = GetComponentInChildren<SpikeAttack>();

        isAttacking = false;

        timeSinceLastBlink = 0;
        nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);

        eyeOffset = Vector2.zero;
        eyeOffsetVel = Vector2.zero;
    }

    void Update()
    {
        if (!PauseButton.IsPaused)
        {
            //update the position of the eye
            Vector2 target = Vector2.zero;
            if (distanceToPlayer() > eyeFocusDistance)
                target = (player.transform.position - transform.position).normalized * maxEyeOffset;
            eyeOffset = Vector2.SmoothDamp(eyeOffset, target, ref eyeOffsetVel, 0.2f);

            eye.transform.position = (Vector2)transform.position + eyeOffset;

            //update the blinking animation
            timeSinceLastBlink += Time.deltaTime;

            if (timeSinceLastBlink >= nextBlinkTime && !isAttacking)
            {
                anims.SetTrigger("Blink");
                timeSinceLastBlink = 0;
                nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);
            }
        }
    }

    public void attack()
    {
        spiralAttack.BeginSpawning();
    }

    //helper method
    private float distanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TurretController.DamageDone += BossController.Static_Reference.Damage();
            Destroy(other.gameObject);
        }
    }
}
