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
    public float MenuSpeed;
    private bool MenuShowing;

    void Awake()
    {
        CurrentSprite = 0; //change this to information loaded from player file
        MusicVolume = true; //same with this one

        MenuShowing = false;
        MenuSpeed = 1f;
    }

    void Update()
    {
        float menuPosition = Menu.GetComponent<RectTransform>().position.x;

        /*
        if (MenuShowing)
        {
            while (menuPosition > -101.7f)
                Menu.transform.Translate(Vector2.left * MenuSpeed * Time.deltaTime);
        }
        else
        {
            while (menuPosition < 104f)
                Menu.transform.Translate(Vector2.right * MenuSpeed * Time.deltaTime);
        }
        */
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
