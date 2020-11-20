using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletCode : MonoBehaviour
{
    [SerializeField] Player player;

    //public
    public Vector2 direction;

    public float damage;
    public float speed;
    public float range;

    //private
    private Rigidbody2D rb;
    private float distanceTraveled;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.rotation.SetLookRotation(direction);

        distanceTraveled = 0;
    }

    void Update()
    {
        if(distanceTraveled > range)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = direction * speed;
        distanceTraveled += speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("Player"))
        {
            player.Health -= damage;
            Destroy(gameObject);
        }
    }
}
