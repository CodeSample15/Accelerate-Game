﻿using System.Collections;
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
        if(Input.GetKeyDown(KeyCode.Escape) && Player.staticReference.isAlive)
        {
            paused = !paused;
        }

        if (Input.GetButtonDown("joystick button 7") && Player.staticReference.isAlive)
        {
            paused = !paused;
        }
    }

    //for use with external buttons (graphical pause button)
    public void Pause()
    {
        paused = !paused;
    }

    public static void TogglePause()
    {
        paused = !paused;
    }
}
