using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //for pressing the button
    public Animator PlayButtonAnimation;
    public MenuLogic transition;

    void Awake()
    {
        CurrentSprite = 0; //change this to information loaded from player file
        MusicVolume = true; //same with this one

        MenuShowing = false;
        MenuSpeed = 4.1f;
    }

    void Update()
    {
        Vector2 menuPosition = Menu.GetComponent<RectTransform>().anchoredPosition;

        if (MenuShowing)
        {
            if (menuPosition.x > -123.8f)
            {
                Menu.GetComponent<RectTransform>().anchoredPosition = new Vector2((menuPosition.x - MenuSpeed), menuPosition.y);
            }

            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
            {
                //user is using controller or arrow keys to select settings

            }
        }
        else
        {
            if (menuPosition.x < 126.3f)
            {
                Menu.GetComponent<RectTransform>().anchoredPosition = new Vector2((menuPosition.x + MenuSpeed), menuPosition.y);
            }
        }

        //A button on controller to start
        if(Input.GetButtonDown("joystick button 0"))
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
