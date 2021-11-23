﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Animator FadeAnimation;
    [Tooltip("Where the player will be moved as an animation for the next level to be loaded.")]
    [SerializeField] private Vector2 PlayerMovePosition;
    [SerializeField] public float AnimationDelay;
    [SerializeField] private int NumberOfStages;

    public int StageCount
    {
        get { return NumberOfStages; }
    }

    /// <summary>
    /// Load a new level and play the game
    /// </summary>
    public void Play()
    {
        int nextLevel = Random.Range(1, NumberOfStages+1);

        StartCoroutine(loadScene(nextLevel));
    }

    public void Play(int levelNum)
    {
        StartCoroutine(loadScene(levelNum));
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