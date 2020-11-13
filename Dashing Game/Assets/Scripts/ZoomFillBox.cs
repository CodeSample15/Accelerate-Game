using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class ZoomFillBox : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] public ParticleSystem particleSystem;
    [SerializeField] public Light2D light;
    [SerializeField] public float refillTime;
    [SerializeField] public int refillAmount;

    private bool refilled;
    private float timeSinceLastRefill;

    public int TimeSinceLastRefill
    {
        get { return Mathf.RoundToInt(timeSinceLastRefill); }
    }

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
            light.enabled = true;
            particleSystem.Play();
        }
        else
        {
            timeSinceLastRefill += Time.deltaTime;
            light.enabled = false;
            particleSystem.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
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