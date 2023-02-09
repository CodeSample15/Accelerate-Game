using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script will control the level and the different difficulties as the game progresses. 
 */
public class LevelController : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject playerGameObject;
    private WaveController waveController;
    private Player player;

    private List<GameObject> ActiveModules;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        playerGameObject = player.gameObject;
        waveController = FindObjectOfType<WaveController>();
    }
}
