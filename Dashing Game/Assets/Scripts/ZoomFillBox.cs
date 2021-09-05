using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZoomFillBox : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] public float refillTime;

    private ParticleSystem particleSystem;

    private bool refilled;
    private float timeSinceLastRefill;

    public int TimeSinceLastRefill
    {
        get { return Mathf.RoundToInt(timeSinceLastRefill); }
    }

    void Awake()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();

        refilled = false;
        timeSinceLastRefill = 0f;
    }

    void Update()
    {
        if(timeSinceLastRefill >= refillTime)
        {
            refilled = true;
            particleSystem.Play();
        }
        else
        {
            timeSinceLastRefill += Time.deltaTime;
            particleSystem.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (refilled && player.GetComponent<Player>().DashPower != 100)
            {
                timeSinceLastRefill = 0f;

                player.DashPower = 100;
                refilled = false;
            }
        }
    }
}