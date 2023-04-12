using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeScreenController : MonoBehaviour
{
    //All of the code for the buttons on home screen (other than the play button)
    [Header("Possible colors for player customization")]
    [SerializeField] public Color[] playerColors;
    public static Color[] PlayerColors;

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

    [Header("Store button animation")]
    public Animator StoreAnimation;

    private bool alreadySelectedButtons;

    private PlayerData data;

    public bool MenuIsShowing
    {
        get { return MenuShowing; }
    }

    void Awake()
    {
        //load colors for player skins
        PlayerColors = playerColors;

        MenuShowing = false;
        MenuSpeed = 6f;

        textSize = 10f;

        alreadySelectedButtons = false;

        //fetch player data and create file if there isn't any
        Debug.Log("Fetching player data...");
        if (Saver.loadData() == null || false) //the || false/true is for debugging purposes
        {
            //new player, create new player data file
            List<int> skins = new List<int>();
            skins.Add(0);

            PlayerData newPlayerData = new PlayerData(true, 0, 0,        //new player, money, and highscore
                                                     0, 0, 0, 0, 0,      //upgrades (reset all to zero)
                                                     0,                  //number of crystals unlocked (set to zero)
                                                     skins, 0,           //IDs of currently unlocked skins 
                                                     0, true);           //sound (medium volume, music turned on)

            Debug.Log("New player, creating player data file");
            Saver.SavePlayer(newPlayerData);
        }

        data = Saver.loadData();

        #region Upgrading player data files if necessary
        if (data.SkinsUnlocked == null)
        {
            List<int> skins = new List<int>();
            skins.Add(0);
            data.SkinsUnlocked = skins;
            data.SelectedSkin = 0;

            Saver.SavePlayer(data);
        }
        #endregion

        //load data
        money = data.Money;
        highScore = data.HighScore;

        //set button sprites depending on loaded data
        VolumeButton.GetComponent<Image>().sprite = VolumeSprites[data.VolumeLevel];
        MusicVolumeButton.GetComponent<Image>().sprite = MusicVolumeSprites[data.MusicPlaying ? 0 : 1];

        Debug.Log("Player data loaded!");

        CurrentSprite = data.VolumeLevel; //what sprite the volume level is on (also shows the level of the volume)
        MusicVolume = data.MusicPlaying;  //whether or not the music is playing in the background (on top of the sound effects)
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
        CurrentSprite--;
        if (CurrentSprite < 0)
            CurrentSprite = VolumeSprites.Length-1;

        VolumeButton.GetComponent<Image>().sprite = VolumeSprites[CurrentSprite];

        data.VolumeLevel = CurrentSprite;
        Saver.SavePlayer(data);
    }

    public void MusicClick()
    {
        MusicVolume = !MusicVolume;

        if (MusicVolume)
            MusicVolumeButton.GetComponent<Image>().sprite = MusicVolumeSprites[0];
        else
            MusicVolumeButton.GetComponent<Image>().sprite = MusicVolumeSprites[1];

        data.MusicPlaying = MusicVolume;
        Saver.SavePlayer(data);
    }

    public void AnimateStoreButton()
    {
        //animate the score button using the same animation that the play button uses
        StoreAnimation.SetTrigger("Clicked");
    }

    public void SettingsClick()
    {
        MenuShowing = !MenuShowing;
    }

    public void BackGroundClicked()
    {
        //if the setting menu is currently showing, hide it
        //this can also be used to "click off" of certain menus that can be added in the future
        if (MenuShowing)
            MenuShowing = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public static Color[] TempColors()
    {
        //for debugging the shop screen
        Color[] temp = new Color[4];
        temp[0] = Color.white;
        temp[1] = Color.yellow;
        temp[2] = Color.blue;
        temp[3] = Color.red;

        return temp;
    }
}
