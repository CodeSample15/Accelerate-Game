using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;

public class NormalCharacterController : MonoBehaviour
{
    #region Private Variables
    private Rigidbody2D rb;
    private Collider2D col;

    private bool detectingEnemies; //whether the DetectEnemies coroutine is still running

    private float movementSpeed; // how fast the player travels at maximum speed
    private float walkingSpeed; // where the animation will switch from walking to running
    private float dashSpeed; // how fast the player travels when dashing
    private float jumpForce; // how much force is applied to the character when it jumps
    private float sideJumpForce; // how much sideways force is applied to the character when it jumps off of a wall
    private float sideJumpVelocity; // so that the side jumping doesn't interfere with the main movement code, the player will have a seperate variable to control how much it's moving sideways for a side jump
    private float sideJumpSlowRate; // the rate in which the player slows down from a side jump
    private int minDashPower; // the minimum amount of dash power in the dash bar allowed for the user to dash
    private float dashRechargeRate; // how fast the dash bar refills
    private float dashDischargeRate; // how fast the dash bar empties when the player dashes
    private float sideDetectionLength; // how far the raycast for the side detection is shot out. Can be used to adjust how steep of an angle the player can run up

    private float scoreAnimationSpeed;
    private float scoreNormalSize;
    private float scoreGrowSize;
    private float scoreShrinkSpeed;
    private float scoreGrowSpeed;

    private float health;
    private float dashPower;
    private bool dashing;
    private int pointsPerKill;
    private int score;

    //movement variables
    private Vector2 movement; //for walking / dashing movement
    private Vector2 lastDashDir; //for dashing

    private float feetDistance; //for casting rays from the player's feet rather than the center of the sprite

    private bool doubleJumped; //test if the player has already double jumped

    private Vector3 velocity = Vector3.zero;

    //player data
    private PlayerData data;

    private int moneyAdded; //will keep track of the amount of currency the player will earn at the end of the game

    //upgrade variables (will store the actual additions made to the stats, not how many upgrades the player has made to a category)
    private float speedUpgrade;
    private float maxHealthUpgrade;
    private float maxDashUpgrade;
    private float dashRechargeUpgrade;
    private float jumpHeightUpgrade;
    #endregion

    #region Public versions of private variables
    public float DashPower
    {
        get { return dashPower; }
        set { dashPower = value; }
    }

    public int Score
    {
        get { return score; }
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public bool isDashing
    {
        get { return dashing; }
    }

    public bool isAlive
    {
        get { return health > 0; }
    }

    public float MaxDash
    {
        get { return maxDashUpgrade + 100; }
    }
    #endregion

    #region Applying upgrades to the player
    /*
     *  Any line of code with the comment "UPGRADE" next to it uses one of the following upgrade variables to upgrade a stat
    */

    private void applyUpgrades()
    {
        //setting all of the appropriate values to the amount of upgrades on a given stat times a modifier
        speedUpgrade = (data.SpeedUpgrade * 0.15f) + 1;
        maxHealthUpgrade = data.MaxHealthUpgrade * 15f;
        maxDashUpgrade = data.MaxDashUpgrade * 15f;
        dashRechargeUpgrade = data.DashRechargeUpgrade * 0.7f;
        jumpHeightUpgrade = data.JumpHeightUpgrade * 0.3f;
    }
    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        detectingEnemies = false;

        //adjustable variables
        movementSpeed = 6f;
        walkingSpeed = 0.3f;
        dashSpeed = 15f;
        jumpForce = 16f;
        sideJumpForce = 1000;
        sideJumpVelocity = 0f;
        sideJumpSlowRate = 0.9f;
        minDashPower = 20;
        dashRechargeRate = 3.2f;
        dashDischargeRate = 35f;
        sideDetectionLength = 0.4f;

        scoreAnimationSpeed = 0.04f;
        scoreGrowSize = 0.55f;
        scoreShrinkSpeed = 0.009f;
        scoreGrowSpeed = 0.06f;

        //fixed starting value variables for things like health and the amount of dash ability left
        dashing = false;
        score = 0;
        pointsPerKill = 15;

        doubleJumped = false;

        feetDistance = 0.5f;

        lastDashDir = new Vector2(0, 1);

        //load player data and apply upgrades
        data = Saver.loadData();

        //new players get 15 "dollars" to start with
        if (data.isNewPlayer)
            moneyAdded = 15;
        else
            moneyAdded = 0;

        applyUpgrades();

        health = 100f + maxHealthUpgrade;
        dashPower = 100f + maxDashUpgrade;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            //only run the main code if the game isn't paused
            if (!PauseButton.IsPaused)
            {
                rb.simulated = true;
                MenuLogic.buttonsActive = false; //turning off the buttons

                /*
                 * MOVEMENT CODE FOR PC ONLY (FOR NOW)
                */
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0"))
                    jump();
                /*
                 * MOVEMENT CODE FOR PC ONLY (FOR NOW)
                 */

                //only dashes if the dash meter is above a certain point
                if (dashPower >= minDashPower)
                {
                    dashing = (CrossPlatformInputManager.GetAxis("Dash") == 1) || Input.GetKey(KeyCode.LeftShift) || Input.GetButton("joystick button 1"); //checking if the dash button is pressed or not
                }
                else if (dashing && dashPower > 0)
                {
                    if (dashPower <= 0)
                    {
                        dashing = false;
                        dashPower = 0;
                    }
                    else
                    {
                        dashing = (CrossPlatformInputManager.GetAxis("Dash") == 1) || Input.GetKey(KeyCode.LeftShift) || Input.GetButton("joystick button 1");
                    }
                }
                else
                {
                    dashing = false;
                }


                //refilling the dash meter--------------------------------------
                if (!dashing && dashPower < 100 + maxDashUpgrade) //UPGRADE
                {
                    //if the player doesn't have enough dash ability and isn't dashing, recharge
                    dashPower += Time.deltaTime * (dashRechargeRate + dashRechargeUpgrade); // UPGRADE
                }
                else if (dashPower > 100 + maxDashUpgrade) //UPGRADE
                {
                    //if the player has more dash ability than the max, reset it to the max
                    dashPower = 100 + maxDashUpgrade; // UPGRADE
                }
                else if (dashing)
                {
                    //if dashing, subtract the proper amount of dash ability
                    dashPower -= dashDischargeRate * Time.deltaTime;
                }

                //handle animations--------------------------------------------------------------------- (Animations)
                //set the speed of the running animation
                //-------------------------------------------------------------------------------------- (Animations)
            }
            else
            {
                //game is paused
                rb.simulated = false;
            }
        }
        else
        {
            //If the player is dead:
            //save data-----------------------------------------
            if (data.HighScore < score)
            {
                //new high score
                data.HighScore = score;
            }

            data.isNewPlayer = false; //no longer a new player now that the player has finished a game (CHANGE TO AFTER TUTORIAL IS PLAYED)

            //calculate the amount of money the player should get
            data.Money += moneyAdded;

            Saver.SavePlayer(data); //save changes to player files
            //--------------------------------------------------

            //enable the menu buttons
            MenuLogic.buttonsActive = true;

            //start the animation for the amount of money being added the player's balance

            gameObject.SetActive(false); //"kill" the player
        }

        //update UI-----------------------------------
        //--------------------------------------------
    }

    private void FixedUpdate()
    {
        if (!dashing)
        {
            //walking code
            sideJumpVelocity *= sideJumpSlowRate;
            rb.gravityScale = 2;

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2((movement.x * movementSpeed) * speedUpgrade, rb.velocity.y); //UPGRADE

            // And then smoothing it out and applying it to the character
            if (!Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1f), transform.right, sideDetectionLength)) //detecting if the player is on a wall or not
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
            rb.AddForce(Vector2.right * sideJumpVelocity);

            lastDashDir = new Vector2(0, 1); //when the player starts to dash, they will always start by going up first

            if (onGround(false))
            {
                doubleJumped = false;
            }
        }
        else
        {
            //dashing code
            rb.gravityScale = 0;
            sideJumpVelocity = 0;

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
            targetVelocity *= dashSpeed * speedUpgrade; //UPGRADE
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .02f);
        }
    }

    //public methods
    public void jump()
    {
        if (!dashing)
        {
            bool rightWall = false;
            bool leftWall = false;

            if (onGround(ref leftWall, ref rightWall))
            {
                //determining if the player is jumping off of a wall
                if (!(leftWall && rightWall))
                {
                    if (leftWall)
                        sideJumpVelocity = sideJumpForce;
                    else if (rightWall)
                        sideJumpVelocity = -sideJumpForce;
                }

                //applying upwards force
                rb.velocity = new Vector2(rb.velocity.x, jumpForce + jumpHeightUpgrade); //UPGRADE

                //setting animations and particles
                
            }
            else if (!doubleJumped)
            {
            }

            /* // Other animation code for side jumps:
            else if (right.collider != null || left.collider != null)
            {
                if(right.collider.CompareTag("Ground") || left.collider.CompareTag("Ground"))
                //player is on a slope, do a different animation
                character_animations.SetTrigger("Flip");

                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
            */
        }
    }

    //for adding earned currency for the player to get at the end of the game
    public void earnMoney(int amount)
    {
        moneyAdded += amount;
    }

    /// <summary>
    /// Controlls how much money the player will get for each enemy kill (depending on the type)
    /// All currency gain amounts are here (they are not stored in external variables)
    /// </summary>
    private void givePlayerMoneyForKill(int type)
    {
        if (type == 0)          //Normal
            moneyAdded += 1;

        else if (type == 1)     //Shooter
            moneyAdded += 1;

        else if (type == 2)     //Bomber
            moneyAdded += 2;    //slightly more since they have the chance to explode before you kill them

        else if (type == 3)     //Laser
            moneyAdded += 2;
    }

    /// <summary>
    /// Helper method for determining if the player is on the ground and not falling or dashing
    /// </summary>
    private bool onGround(bool checkWall)
    {
        int groundLayerMask = 1 << 8;

        RaycastHit2D right = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - feetDistance), transform.TransformDirection(new Vector2(1, -1)), 0.5f, groundLayerMask);
        RaycastHit2D left = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - feetDistance), transform.TransformDirection(new Vector2(-1, -1)), 0.5f, groundLayerMask);

        bool rightCol = false;
        bool leftCol = false;

        if (right.collider != null)
            if (right.collider.CompareTag("Ground"))
                rightCol = true;
        if (left.collider != null)
            if (left.collider.CompareTag("Ground"))
                leftCol = true;

        if (checkWall)
            return Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), 1f, groundLayerMask) || rightCol || leftCol;
        else
            return Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), 1f, groundLayerMask);
    }

    /// <summary>
    /// Overloaded method of onGround that also uses a referenced value of whether or not the user is wall jumping (left or right side of player)
    /// </summary>
    /// <param name="leftCol"></param>
    /// <param name="rightCol"></param>
    /// <returns></returns>
    private bool onGround(ref bool leftCol, ref bool rightCol)
    {
        int groundMask = 1 << 8; //bitmask to only accept the 8th layer (ground)        1 0 0 0 0 0 0 0

        RaycastHit2D right = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - feetDistance), transform.TransformDirection(new Vector2(1, -1)), 0.5f, groundMask);
        RaycastHit2D left = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - feetDistance), transform.TransformDirection(new Vector2(-1, -1)), 0.5f, groundMask);

        rightCol = false;
        leftCol = false;

        if (right.collider != null)
            if (right.collider.CompareTag("Ground"))
                rightCol = true;
        if (left.collider != null)
            if (left.collider.CompareTag("Ground"))
                leftCol = true;

        RaycastHit2D bottom = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), 1f, groundMask);
        bool ground = bottom.collider != null && bottom.collider.CompareTag("Ground");

        return ground || rightCol || leftCol;
    }

    /// <summary>
    /// Detecting if the player is touching any enemies and destroying them if the player is dashing
    /// 
    /// The script will loop through each active enemy gameobject in the EnemyController's active enemies array to search for collisions between the two gameobjects
    /// </summary>

    /*
     * Method to update score size
     * This is used for animating the score display whenever the user gets additional points
    */

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Lava"))
            health = 0; //kill the player
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("JumpPad"))
        {
            float jumpBoost = other.gameObject.GetComponent<JumpPadLogic>().Boost;

            //Boosting the player upwards if they are not dashing
            if (!dashing)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(transform.up * jumpBoost, ForceMode2D.Impulse);
            }
        }
    }
}