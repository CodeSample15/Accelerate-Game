using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PauseButton : MonoBehaviour
{
    [SerializeField] public Animator PauseAnimations;
    [SerializeField] private bool paused;

    public bool IsPaused
    {
        get { return paused; }
    }

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
    }

    public void Pause()
    {
        paused = !paused;
    }
}
