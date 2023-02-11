using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script will control the level and the different difficulties as the game progresses. 
 */
public class LevelController : MonoBehaviour
{
    //temp variables for level transitioning
    public static int tempScore;
    public static float tempHealth;
    public static bool saved;

    private Rigidbody2D rb;
    private GameObject playerGameObject;
    private WaveController waveController;
    private Player player;

    private List<GameObject> ActiveModules;

    void Awake()
    {
        tempScore = 0;
        tempHealth = 100;
        saved = false;

        player = FindObjectOfType<Player>();
        playerGameObject = player.gameObject;
        waveController = FindObjectOfType<WaveController>();
    }
}
