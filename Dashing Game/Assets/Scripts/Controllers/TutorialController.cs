using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public static TutorialController staticRef;
    public Vector2 checkpoint;
    
    [Header("UI Items")]
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private Animator FadeAnimation;

    [Space]

    [SerializeField] private GameObject DashBar;

    [Space]

    [SerializeField] private GameObject Minimap;
    [SerializeField] private GameObject MinimapBorder;

    [SerializeField] private GameObject WaveProgressBar;
    [SerializeField] private GameObject ScoreText;

    [Space]

    [Header("Explanations")]
    [SerializeField] private TextMeshProUGUI ExplanationText;
    [SerializeField] private Animator textAnimation;

    [Space]

    [Header("Enemy")]
    [SerializeField] private GameObject Enemy;

    [Space]

    [Header("Items that need to be shown / hidden")]
    [SerializeField] private ZoomFillBox RechargeBox;
    [SerializeField] private ZoomFillBox RechargeBox2;

    private List<GameObject> EnemyPool;

    private bool usingController;
    private bool unlockedDash;
    private bool enemiesSpawned;

    //event flags
    private bool endOfFightMessageShown; //after the player fights the enemies in the tutorial, this variable is used to flag whether or not the controller updated the tutorial text box
    private bool rechargeBoxMessageShown;
    private bool secondRechargeBoxMessageShown;

    public bool UnlockedDash
    {
        get { return unlockedDash; }
    }

    public List<GameObject> TutorialEnemyPool
    {
        get { return EnemyPool; }
    }

    void Awake()
    {
        HealthBar.SetActive(false);
        DashBar.SetActive(false);
        Minimap.SetActive(false);
        MinimapBorder.SetActive(false);
        WaveProgressBar.SetActive(false);
        ScoreText.SetActive(false);
        
        ExplanationText.SetText("");

        staticRef = this;

        usingController = false;
        unlockedDash = false;
        enemiesSpawned = false;

        endOfFightMessageShown = false;
        rechargeBoxMessageShown = false;
        secondRechargeBoxMessageShown = false;

        EnemyPool = new List<GameObject>();
        RechargeBox.gameObject.SetActive(false);
    }

    void Start()
    {
        RechargeBox.gameObject.SetActive(false);
        RechargeBox2.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow))
            usingController = false;
        else if (Input.GetAxis("JoystickChecker") > 0)
            usingController = true;


        //event triggers
        if(enemiesSpawned && EnemyPool.Count == 0 && !endOfFightMessageShown)
        {
            //all of the enemies in the tutorial are dead
            ExplanationText.SetText("Use recharge boxes to refill health and dash");
            ExplanationText.rectTransform.position = new Vector2(-153.3f, -1.25f);
            textAnimation.SetTrigger("Blink");

            Player.staticReference.Health = 50f;
            Player.staticReference.transform.position = new Vector2(-151.15f, -6.86f);

            RechargeBox.gameObject.SetActive(true);

            endOfFightMessageShown = true;
        }

        if(!rechargeBoxMessageShown && !RechargeBox.Refilled)
        {
            ExplanationText.SetText("Refill boxes only recharge once a different refill box is used");
            ExplanationText.rectTransform.position = new Vector2(-153.3f, -1.25f);
            textAnimation.SetTrigger("Blink");

            RechargeBox2.gameObject.SetActive(true);

            rechargeBoxMessageShown = true;
        }

        if(!secondRechargeBoxMessageShown && !RechargeBox2.Refilled)
        {
            ExplanationText.SetText("You're ready");
            ExplanationText.rectTransform.position = new Vector2(-153.3f, -1.25f);
            textAnimation.SetTrigger("Blink");

            StartCoroutine(EndOfTutorial());

            secondRechargeBoxMessageShown = true;
        }
    }

    //world triggers
    public void Triggered(string name)
    {
        switch(name)
        {
            case "Start":
                ExplanationText.SetText("Use arrow keys, W/D keys, or joystick to move");
                ExplanationText.rectTransform.position = new Vector2(-3.9f, 46.4f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Jump":
                if (usingController)
                    ExplanationText.SetText("Press \"A\" to jump");
                else
                    ExplanationText.SetText("Press space to jump");
                ExplanationText.rectTransform.position = new Vector2(21.49f, 46.04f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Double Jump":
                ExplanationText.SetText("Press jump while in the air to double jump");
                ExplanationText.rectTransform.position = new Vector2(37.65f, 46.6f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Wall Jump":
                ExplanationText.SetText("Wall jump by walking into the wall while repeatedly pressing jump");
                ExplanationText.rectTransform.position = new Vector2(58.42f, 58.38f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Health":
                HealthBar.SetActive(true);
                ExplanationText.SetText("Health is important. Try not to lose it");
                ExplanationText.rectTransform.position = new Vector2(52.32f, 83.56f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Dash 1":
                ExplanationText.SetText("The key feature of this game is ACCELERATION...");
                ExplanationText.rectTransform.position = new Vector2(26.12f, 83.56f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Dash 2":
                ExplanationText.SetText("You have to DASH to ACCELERATE...");
                ExplanationText.rectTransform.position = new Vector2(10.01f, 83.56f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Dash 3":
                DashBar.SetActive(true);
                unlockedDash = true;

                if (usingController)
                    ExplanationText.SetText("Press the B button to DASH. Use the joystick to steer");
                else
                    ExplanationText.SetText("Press shift to DASH. Use arrow keys or WSAD to steer");

                ExplanationText.rectTransform.position = new Vector2(-11.04f, 83.56f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Dash 4":
                ExplanationText.SetText("Dashing is used to attack and damage things");
                ExplanationText.rectTransform.position = new Vector2(-56.67f, 84.87f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Crystal":
                ExplanationText.SetText("Use your dash to break the crystal");
                ExplanationText.rectTransform.position = new Vector2(-87.46f, 84.87f);
                textAnimation.SetTrigger("Blink");
                break;

            case "Enemies":
                ExplanationText.SetText("You can also use your dash to kill enemies:");
                ExplanationText.rectTransform.position = new Vector2(-153.3f, -1.25f);
                textAnimation.SetTrigger("Blink");

                SpawnEnemy(-157.64f, 5.95f);
                SpawnEnemy(-154.72f, 7.86f);
                SpawnEnemy(-146.67f, 8.55f);
                SpawnEnemy(-142.63f, 5.73f);

                enemiesSpawned = true;
                break;
        }
    }

    private void SpawnEnemy(float x, float y)
    {
        Vector2 pos = new Vector2(x, y);

        Enemy enemyHolder = Instantiate(Enemy, pos, Quaternion.identity).GetComponent<Enemy>();
        enemyHolder.gameObject.SetActive(false);
        enemyHolder.player = Player.staticReference;
        enemyHolder.playerGameObject = Player.staticReference.gameObject;
        enemyHolder.GetComponent<AIDestinationSetter>().target = Player.staticReference.gameObject.transform;

        enemyHolder.gameObject.SetActive(true);
        enemyHolder.Type = 0;
        enemyHolder.Colorize();

        EnemyPool.Add(enemyHolder.gameObject);
    }

    private IEnumerator EndOfTutorial()
    {
        FadeAnimation.SetTrigger("Fade");

        //player is no longer a new player
        PlayerData data = Saver.loadData();
        data.isNewPlayer = false;
        Saver.SavePlayer(data);

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(9);
    }
}
