using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script will control the level and the different difficulties as the game progresses. 
 */
public class LevelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> LevelModules;

    public static Vector3 velocity;
    public static float gravityScale;

    private Rigidbody2D rb;
    private GameObject playerGameObject;
    private WaveController waveController;
    private Player player;

    private List<GameObject> ActiveModules;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<Player>();
        playerGameObject = player.gameObject;
        waveController = FindObjectOfType<WaveController>();

        ActiveModules = new List<GameObject>();

        //load starting module
        GameObject temp = Instantiate(LevelModules[0], Vector2.zero, Quaternion.identity);
        temp.transform.parent = transform;
    }

    void Update()
    {
        velocity = rb.velocity;
        rb.gravityScale = -gravityScale;
    }
}
