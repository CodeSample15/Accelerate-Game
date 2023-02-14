using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * This script will control the level and the different difficulties as the game progresses. 
 */
public class LevelController : MonoBehaviour
{
    [Header("Settings for gameplay")]
    [SerializeField] private float level_width;
    [SerializeField] private float level_height;
    [SerializeField] private float spawn_radius;

    [Header("Variables used by other scripts")]
    //temp variables for level transitioning
    public static int tempScore;
    public static float tempHealth;
    public static bool saved;

    private Rigidbody2D rb;
    private GameObject playerGameObject;
    private WaveController waveController;
    private Player player;

    private Vector2 levelPoint1;
    private Vector2 levelPoint2;

    void Awake()
    {
        tempScore = 0;
        tempHealth = 100;
        saved = false;

        player = FindObjectOfType<Player>();
        playerGameObject = player.gameObject;
        waveController = FindObjectOfType<WaveController>();

        levelPoint1 = new Vector2(transform.position.x - (level_width/2), transform.position.y + (level_height/2));
        levelPoint2 = new Vector2(transform.position.x + (level_width/2), transform.position.y - (level_height/2));
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Main")
        {
            if(waveController.waveOver())
            {
                waveController.startWave(levelPoint1, levelPoint2, spawn_radius);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(new Vector2(transform.position.x - (level_width / 2), transform.position.y + (level_height / 2)), "", true);
        Gizmos.DrawIcon(new Vector2(transform.position.x + (level_width / 2), transform.position.y - (level_height / 2)), "", true);
        Gizmos.DrawIcon(new Vector2(transform.position.x - (level_width / 2), transform.position.y - (level_height / 2)), "", true);
        Gizmos.DrawIcon(new Vector2(transform.position.x + (level_width / 2), transform.position.y + (level_height / 2)), "", true);
    }
}
