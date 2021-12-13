using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZoomFillBox : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] public float refillTime;

    private ParticleSystem particles;

    private bool refilled;
    private float timeSinceLastRefill;

    public int TimeSinceLastRefill
    {
        get { return Mathf.RoundToInt(timeSinceLastRefill); }
    }

    void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();

        //start out with the boxes being filled
        refilled = true;
        timeSinceLastRefill = refillTime;
    }

    void Update()
    {
        if (!PauseButton.IsPaused)
        {
            if (timeSinceLastRefill >= refillTime)
            {
                refilled = true;
                particles.Play();
            }
            else
            {
                timeSinceLastRefill += Time.deltaTime;
                particles.Stop();
            }
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