using System.Collections;
using UnityEngine;

public class RedBossController : MonoBehaviour
{
    [Header("Cosmetics")]
    [SerializeField] private float neutralTurnSpeed;
    [SerializeField] private float attackTurnSpeed;

    [Header("Non cosmetics")]
    [SerializeField] private float healthDecay;
    [SerializeField] private float attackTime;
    [SerializeField] private SpikeAttack spikeAttack;
    [SerializeField] private GameObject laserAttack;

    private Player player;

    private float turnVel;
    private float turnSpeed;

    private bool isAttacking;

    public bool IsAttacking
    {
        get { return isAttacking; }
    }

    void Awake()
    {
        player = Player.staticReference;

        laserAttack.SetActive(false);

        isAttacking = false;
        turnSpeed = neutralTurnSpeed;
    }

    void Update()
    {
        if(!PauseButton.IsPaused)
        {
            transform.Rotate(new Vector3(0,0,1) * turnSpeed * Time.deltaTime);
            BossController.Static_Reference.Damage(healthDecay*Time.deltaTime);

            if(isAttacking)
                turnSpeed = Mathf.SmoothDamp(turnSpeed, attackTurnSpeed, ref turnVel, 1f);
            else
                turnSpeed = Mathf.SmoothDamp(turnSpeed, neutralTurnSpeed, ref turnVel, 0.1f);
        }
    }

    public void attack()
    {
        StartCoroutine(RunAttack());
    }

    IEnumerator RunAttack()
    {
        isAttacking = true;

        //wait for spin speed to reach max speed
        yield return new WaitForSeconds(1.9f);

        //50% chance of spike attack spawning
        if (Random.Range(0, 2) == 1)
        {
            spikeAttack.transform.position = player.transform.position;
            spikeAttack.BeginSpawning();
        }

        //100% chance of lasers shooting from boss
        laserAttack.SetActive(true);

        //wait for attack to finish
        yield return new WaitForSeconds(attackTime);

        //clean up
        isAttacking = false;
        laserAttack.SetActive(false);
    }
}
