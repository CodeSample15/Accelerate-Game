using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public static float DamageDone = 0;
    public bool SlideOut = true;

    [SerializeField] private GameObject bullet;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private float turnTime;
    [SerializeField] private float fireTime;
    [SerializeField] private float gunOffset;

    [SerializeField] private float minSlideOutSpeed;
    [SerializeField] private float maxSlideOutSpeed;

    [Space]
    [SerializeField] private float lifeSpan;
    [SerializeField] private float maxDamage;

    private Player player;
    private Animator anims;
    private Rigidbody2D rb;
    private bool active;
    private bool smoothTracking;

    //where the turret should be pointing
    private Vector3 turnVel;

    //how much time has elapsed between each shot
    private float shotTimer;

    //how much time has elapsed since being spawned
    private float lifeElapsed;

    //how many turrets currently exist in the scene
    private static int turretCount = 0;

    void Awake()
    {
        turretCount++;

        if (turretCount > 1)
            Destroy(gameObject);

        DamageDone = 0;

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

            lifeElapsed += Time.deltaTime;

            if(lifeElapsed > lifeSpan || DamageDone > maxDamage)
            {
                StartCoroutine(despawn());
            }
        }
        else if(!player.isAlive)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator startup()
    {
        if (SlideOut)
        {
            transform.position = BossController.Static_Reference.Boss.transform.position;

            //create a random vector and apply the velocity to the turret to slide out from
            Vector2 rand = Random.insideUnitCircle.normalized;
            rand *= Random.Range(minSlideOutSpeed, maxSlideOutSpeed);
            rb.velocity = rand;
        }

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

    IEnumerator despawn()
    {
        anims.SetTrigger("Despawn");
        active = false;

        yield return new WaitForSeconds(4f);

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player") && player.isDashing)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        turretCount--;
    }
}
