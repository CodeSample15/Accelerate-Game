using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZoomFillBox : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] public float refillTime;
    [SerializeField] public int refillAmount;

    private bool refilled;
    private float timeSinceLastRefill;

    void Awake()
    {
        refilled = false;
        timeSinceLastRefill = 0f;
    }

    void Update()
    {
        if(timeSinceLastRefill >= refillTime)
        {
            refilled = true;
        }
        else
        {
            timeSinceLastRefill += Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (refilled)
            {
                timeSinceLastRefill = 0f;

                player.DashPower = 100;
                refilled = false;
            }
        }
    }
}
