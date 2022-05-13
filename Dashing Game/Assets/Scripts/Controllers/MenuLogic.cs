using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuLogic : MonoBehaviour
{
    [SerializeField] HomeScreenController HomeScreenController;
    [SerializeField] private GameObject HomeButton;
    [SerializeField] private GameObject RestartButton;
    [SerializeField] private GameObject ExitButton;
    [SerializeField] private Animator   PressedButtonAnimation;

    [SerializeField] private GameObject player;
    [SerializeField] private Animator FadeAnimation;
    [Tooltip("Where the player will be moved as an animation for the next level to be loaded.")]
    [SerializeField] private Vector2 PlayerMovePosition;
    [SerializeField] public float AnimationDelay;
    [SerializeField] private int NumberOfStages;

    public static bool buttonsActive;

    public int StageCount
    {
        get { return NumberOfStages; }
    }

    void Awake()
    {
        //making sure the player can't accidentally press these buttons when they're hidden
        if (HomeButton != null && RestartButton != null)
        {
            HomeButton.SetActive(false);
            RestartButton.SetActive(false);
            ExitButton.SetActive(false);
            buttonsActive = false;
        }
    }

    void Update()
    {
        if (HomeButton != null && RestartButton != null)
        {
            HomeButton.SetActive(buttonsActive);
            RestartButton.SetActive(buttonsActive);
            ExitButton.SetActive(buttonsActive);
        }

        if(buttonsActive && PauseButton.IsPaused)
        {
            //restart button becomes resume (prevents people from accidentally restarting their game)
            RestartButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Resume");
        }
        else if(RestartButton != null)
        {
            RestartButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Restart");
        }
    }

    /// <summary>
    /// Load a new level and play the game
    /// </summary>
    public void Play()
    {
        int nextLevel = Random.Range(2, NumberOfStages+2); //offset by two for the home and store scenes

        //playing button animation if there is one
        if (PressedButtonAnimation != null)
            if(!HomeScreenController.MenuIsShowing)
                PressedButtonAnimation.SetTrigger("Clicked");

        StartCoroutine(loadScene(nextLevel));
    }

    public void Play(int levelNum)
    {
        StartCoroutine(loadScene(levelNum));
    }

    //for the restart button, since it will change function if the game is paused or not
    public void ResumeOrRestart()
    {
        //will resume if the game is paused, will load a new level if the game isn't
        if (PauseButton.IsPaused)
        {
            PauseButton.TogglePause();
        }
        else
        {
            Play();
        }
    }

    //quit the game application
    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator loadScene(int sceneNum)
    {
        //move the player off the level and load the next level
        if(player != null)
            player.transform.position = PlayerMovePosition;

        FadeAnimation.SetTrigger("Fade"); //start the black fade animation

        yield return new WaitForSeconds(AnimationDelay);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNum);

        //after waiting the right anount of time for the animations to finish playing, wait until the next scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
