using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadLogic : MonoBehaviour
{
    [SerializeField] private float boostStrength;

    private Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !player.isDashing)
        {
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();

            playerRB.velocity = new Vector2(0, 0);
            Vector2 force = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * Vector2.up;
            force *= boostStrength;

            playerRB.AddForce(force, ForceMode2D.Impulse);
            player.KnockBackPlayer(force.x);
        }
    }
}
