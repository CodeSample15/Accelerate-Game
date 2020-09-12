using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    #region Public Variables
    public Joystick joystick;
    public Slider health_bar;
    public Slider dash_meter;
    public TextMeshProUGUI scoreText;
    #endregion

    #region Private Variables
    private Rigidbody2D rb;

    private float movementSpeed;
    private float dashSpeed;
    private float jumpForce;
    private int minDashPower;
    private float dashRechargeRate;
    private float dashDischargeRate;
    private int enemyDamage;

    private float health;
    private float dashPower;
    private bool dashing;
    private int pointsPerKill;
    private int score;

    //movement variables
    private Vector2 movement; //for walking / dashing movement
    private Vector2 lastDashDir; //for dashing

    private Vector3 velocity = Vector3.zero;
    #endregion

    public int Score
    {
        get { return score; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        //adjustable variables
        movementSpeed = 10f;
        dashSpeed = 15f;
        jumpForce = 9f;
        minDashPower = 20;
        dashRechargeRate = 20f;
        dashDischargeRate = 40f;
        enemyDamage = 5;

        //fixed variables for things like health and the amount of dash ability left
        health = 100f;
        dashPower = 0f;
        dashing = false;
        score = 0;
        pointsPerKill = 15;

        lastDashDir = new Vector2(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //get user controll input
        movement.x = joystick.Horizontal;
        movement.y = joystick.Vertical;

        //only dashes if the dash meter is above a certain point
        if (dashPower >= minDashPower)
        {
            dashing = (CrossPlatformInputManager.GetAxis("Dash") == 1); //checking if the dash button is pressed or not
        }
        else if(dashing && dashPower > 0)
        {
            if (dashPower <= 0)
            {
                dashing = false;
                dashPower = 0;
            }
            else
            {
                dashing = (CrossPlatformInputManager.GetAxis("Dash") == 1);
            }
        }
        else
        {
            dashing = false;
        }


        //refilling the dash meter
        if(!dashing && dashPower < 100)
        {
            dashPower += Time.deltaTime * dashRechargeRate;
        }
        else if(dashPower > 100)
        {
            dashPower = 100;
        }
        else if(dashing)
        {
            dashPower -= dashDischargeRate * Time.deltaTime;
        }


        //update UI
        health_bar.value = health / 100;
        dash_meter.value = dashPower / 100;

        scoreText.text = score.ToString();
    }

    private void FixedUpdate()
    {
        if(!dashing)
        {
            //walking code (from Brackey's 2D character movement script)---------------------------------------------------
            rb.gravityScale = 2;

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
            // And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

            lastDashDir = new Vector2(0, 1); //when the player starts to dash, they will always start by going up first
        }
        else
        {
            //dashing code
            rb.gravityScale = 0;

            Vector3 targetVelocity;

            if (movement == Vector2.zero)
            {
                targetVelocity = lastDashDir;
            }
            else
            {
                targetVelocity = movement;
                lastDashDir = movement;
            }

            targetVelocity = targetVelocity.normalized;
            targetVelocity *= dashSpeed;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .02f);
        }
    }

    //public methods
    public void jump()
    {
        if (!dashing) {
            if (Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), 0.31f))
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }
    }

    //private methods
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            if (dashing)
            {
                Destroy(other.gameObject);
                score += pointsPerKill;
            }
            else
            {
                health -= enemyDamage;
            }
        }
    }
}