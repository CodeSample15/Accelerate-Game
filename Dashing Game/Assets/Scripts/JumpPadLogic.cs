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
            Vector2 force = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector2.up;

            playerRB.AddForce(force * boostStrength, ForceMode2D.Impulse);
        }
    }
}
