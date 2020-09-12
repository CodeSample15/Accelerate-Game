using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //public
    public GameObject player;
    public bool clone;

    //private
    private Rigidbody2D rb;
    private Vector2 direction;

    private float moveSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 5f;
        clone = false;
    }

    private void FixedUpdate()
    {
        if (clone)
        {
            direction = player.transform.position - transform.position;
            direction = direction.normalized;
            rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
        }
    }
}