using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    public static TutorialController staticRef;
    
    [Header("UI Items")]
    [SerializeField] private GameObject HealthBar;

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

    private bool usingController;

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
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow))
            usingController = false;
        else if (Input.GetAxis("JoystickChecker") > 0)
            usingController = true;
    }

    public void Triggered(string name)
    {
        switch(name)
        {
            case "Start":
                ExplanationText.SetText("Use arrow keys or joystick to move");
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
                textAnimation.SetTrigger("Blink");
                break;

            case "Wall Jump":
                break;

            case "Health":
                break;

            case "Dash":
                break;

            case "Red Enemy":
                break;

            case "Green Enemy":
                break;

            case "Blue Enemy":
                break;

            case "Yellow Enemy":
                break;

            case "Recharge Box":
                break;

            case "Crystal":
                break;

            
        }
    }
}
