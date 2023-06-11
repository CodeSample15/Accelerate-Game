using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float turnTime;
    [SerializeField] private float fireTime;
    [SerializeField] private float gunOffset;

    [SerializeField] private float minSlideOutSpeed;
    [SerializeField] private float maxSlideOutSpeed;

    private Player player;
    private Animator anims;
    private Rigidbody2D rb;
    private bool active;
    private bool smoothTracking;

    //where the turret should be pointing
    private Vector3 turnVel;

    //how much time has elapsed between each shot
    private float shotTimer;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        anims = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        active = false;
        smoothTracking = true;
    }

    void Start()
    {
        StartCoroutine(startup());
    }

    void Update()
    {
        if(!PauseButton.IsPaused && active && player.isAlive)
        {
            Vector2 diff = (player.transform.position - transform.position).normalized;

            if (smoothTracking)
            {
                Quaternion lookRot = Quaternion.LookRotation(Vector3.forward, diff);
                transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles, lookRot.eulerAngles, ref turnVel, turnTime));
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(Vector3.forward, diff);
            }

            shotTimer += Time.deltaTime;

            if(shotTimer >= fireTime)
            {
                anims.SetTrigger("Shoot");

                //spawn bullet
                BulletCode temp = Instantiate(bullet, transform.position + (transform.up * gunOffset), Quaternion.identity).GetComponent<BulletCode>();
                temp.type = BulletCode.Types.spike;
                temp.direction = diff;
                temp.ignoreDash = false;

                shotTimer = 0f;
            }
        }
        else if(!player.isAlive)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator startup()
    {
        transform.position = BossController.Static_Reference.Boss.transform.position;

        //create a random vector and apply the velocity to the turret to slide out from
        Vector2 rand = new Vector2(Random.Range(-1,1), Random.Range(-1,1)).normalized;
        rand *= Random.Range(minSlideOutSpeed, maxSlideOutSpeed);
        rb.velocity = rand;

        do {
            yield return new WaitForEndOfFrame();
        } while (rb.velocity.magnitude > 0.5f);

        anims.SetTrigger("Deploy");
        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        yield return new WaitForSeconds(1f);

        active = true;

        yield return new WaitForSeconds(0.5f);

        smoothTracking = false;
    }
}
