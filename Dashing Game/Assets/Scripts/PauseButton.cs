using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PauseButton : MonoBehaviour
{
    [SerializeField] public Animator PauseAnimations;
    [SerializeField] private static bool paused;

    public static bool IsPaused
    {
        get { return paused; }
    }

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
    }

    void Update()
    {
        //pause if escape key is pressed
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
        }

        if (Input.GetButtonDown("joystick button 7"))
        {
            paused = !paused;
        }
    }

    //for use with external buttons (graphical pause button)
    public void Pause()
    {
        paused = !paused;
    }
}
