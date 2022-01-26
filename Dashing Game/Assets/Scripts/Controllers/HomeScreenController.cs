using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeScreenController : MonoBehaviour
{
    //All of the code for the buttons on home screen (other than the play button)
    [Header("Volume Button")]
    public GameObject VolumeButton;
    public Sprite[] VolumeSprites;
    private int CurrentSprite;

    [Header("Music Volume Button")]
    public GameObject MusicVolumeButton;
    public Sprite[] MusicVolumeSprites;
    private bool MusicVolume;

    //for showing and hiding the menu
    [Header("Menu")]
    public GameObject Menu;
    public Animator MenuAnimation; //for controller input (when touch screen or mouse isn't available) 
    public float MenuSpeed;
    private bool MenuShowing;

    [Header("Money Amount")]
    public GameObject CurrencySprite;
    public TextMeshProUGUI MoneyDisplay; //shows the user how much money they have
    private float textSize; //how big the text is (for moving the currency sprite)
    private int money; //how much money the user has (loaded from save file)

    [Header("HighScore")]
    public TextMeshProUGUI HighScoreText;
    private int highScore;

    [Header("Play button animation and scene transition")]
    //for pressing the button
    public Animator PlayButtonAnimation;
    public MenuLogic transition;

    private bool alreadySelectedButtons;

    public bool MenuIsShowing
    {
        get { return MenuShowing; }
    }

    void Awake()
    {
        CurrentSprite = 0; //change this to information loaded from player file
        MusicVolume = true; //same with this one

        MenuShowing = false;
        MenuSpeed = 6f;

        textSize = 10f;

        alreadySelectedButtons = false;

        //fetch player data and create file if there isn't any
        Debug.Log("Fetching player data...");
        if (Saver.loadData() == null)
        {
            //new player, create new player data file
            PlayerData newPlayerData = new PlayerData(true, 0, 0, 0, 0, 0, 0, 0);

            Debug.Log("New player, creating player data file");
            Saver.SavePlayer(newPlayerData);
        }

        //load data
        PlayerData data = Saver.loadData();

        money = data.Money;
        highScore = data.HighScore;

        Debug.Log("Player data loaded!");
    }

    void Update()
    {
        Vector2 menuPosition = Menu.GetComponent<RectTransform>().anchoredPosition;
        Vector2 targetPosition;

        if (MenuShowing)
        {
            //enable buttons
            VolumeButton.SetActive(true);
            MusicVolumeButton.SetActive(true);

            if (menuPosition.x > -123.8f)
            {
                targetPosition = new Vector2(-123.8f, Menu.GetComponent<RectTransform>().anchoredPosition.y);
                Menu.GetComponent<RectTransform>().anchoredPosition = transform.localPosition = Vector2.Lerp(transform.localPosition, targetPosition, Time.deltaTime * MenuSpeed);
            }

            //code for when menu is showing
            if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0 && !alreadySelectedButtons)
            {
                alreadySelectedButtons = true;

                VolumeButton.GetComponent<Button>().Select();
            }
        }
        else
        {
            if (menuPosition.x < 126.3f)
            {
                targetPosition = new Vector2(126.3f, Menu.GetComponent<RectTransform>().anchoredPosition.y);
                Menu.GetComponent<RectTransform>().anchoredPosition = transform.localPosition = Vector2.Lerp(transform.localPosition, targetPosition, Time.deltaTime * MenuSpeed);
            }

            //disable buttons
            VolumeButton.SetActive(false);
            MusicVolumeButton.SetActive(false);
        }

        //A button on controller to start
        if(Input.GetButtonDown("joystick button 0") && !MenuShowing)
        {
            PlayButtonAnimation.SetTrigger("Clicked");
            transition.Play();
        }

        //menu button on controller to open menu
        if(Input.GetButtonDown("joystick button 7"))
        {
            SettingsClick();
            MenuAnimation.SetTrigger("Pressed");
        }

        //update money amount display
        MoneyDisplay.SetText(money.ToString());
        int moneyLength = money.ToString().Length; //used to determing how many spaces the icon should move back from the center
        CurrencySprite.GetComponent<RectTransform>().anchoredPosition = new Vector2(moneyLength * -textSize, CurrencySprite.GetComponent<RectTransform>().anchoredPosition.y);

        //update high score display
        HighScoreText.SetText("Highscore: " + highScore.ToString());
    }

    public void VolumeClick()
    {
        CurrentSprite++;
        if (CurrentSprite >= VolumeSprites.Length)
            CurrentSprite = 0;

        VolumeButton.GetComponent<Image>().sprite = VolumeSprites[CurrentSprite];
    }

    public void MusicClick()
    {
        MusicVolume = !MusicVolume;

        if (MusicVolume)
            MusicVolumeButton.GetComponent<Image>().sprite = MusicVolumeSprites[0];
        else
            MusicVolumeButton.GetComponent<Image>().sprite = MusicVolumeSprites[1];
    }

    public void SettingsClick()
    {
        MenuShowing = !MenuShowing;
    }
}
