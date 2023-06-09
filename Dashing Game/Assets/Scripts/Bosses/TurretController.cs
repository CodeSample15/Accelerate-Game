using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float turnTime;
    [SerializeField] private float fireTime;

    private Player player;
    private Animator anims;
    private bool active;

    //where the turret should be pointing
    private Vector2 turn;
    private Vector2 turnVel;

    //how much time has elapsed between each shot
    private float shotTimer;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        anims = GetComponent<Animator>();
        active = false;
    }

    void Start()
    {
        StartCoroutine(startup());
    }

    void Update()
    {
        if(PauseButton.IsPaused && active)
        {
            Vector2 target = player.transform.position - transform.position;
            turn = Vector2.SmoothDamp(turn, target, ref turnVel, turnTime);

            shotTimer += Time.deltaTime;

            if(shotTimer >= fireTime)
            {
                anims.SetTrigger("Fire");

                //spawn bullet

                shotTimer = 0f;
            }
        }
    }

    IEnumerator startup()
    {
        yield return new WaitForSeconds(1f);

        active = true;
    }
}
