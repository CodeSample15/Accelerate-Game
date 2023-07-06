using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCode : MonoBehaviour
{
    public bool DetectWalls = false;

    public float spikeSpeed;
    public float spikeDamage;

    void Update()
    {
        if(!PauseButton.IsPaused)
        {
            transform.Translate(transform.right * spikeSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player.staticReference.Health -= spikeDamage;
            gameObject.SetActive(false);
        }
        else if (DetectWalls && other.gameObject.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
    }
}
