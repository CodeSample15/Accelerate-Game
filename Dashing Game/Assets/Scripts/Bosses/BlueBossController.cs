using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBossController : MonoBehaviour
{
    [Header("Functional")]
    [SerializeField] private float AttackRange;

    [Header("Cosmetic")]
    [SerializeField] private float blinkTime;
    [SerializeField] private float bounciness;

    private Animator anims;
    private Player player;
    private float timeSinceLastBlink;

    void Awake()
    {
        anims = GetComponent<Animator>();
        player = FindObjectOfType<Player>();

        timeSinceLastBlink = 0;
    }

    void Update()
    {
        //handle animations
        timeSinceLastBlink += Time.deltaTime;

        if(timeSinceLastBlink >= blinkTime)
        {
            anims.SetTrigger("Blink");
            timeSinceLastBlink = 0;
        }

        //handle attacks

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
