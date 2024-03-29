﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * This script handles the visuals of the player customization page, while the UpgradeScreenController handles all of the data related to this page
 */

public class PlayerCustomization : MonoBehaviour
{
    [Header("Preview Animation")]
    [SerializeField] private float TurnSpeedNormal;
    [SerializeField] private float TurnSpeedMax;
    [SerializeField] private float TurnSpeedDecay;

    [Space]

    [SerializeField] private GameObject playerPrev;
    [SerializeField] private GameObject currentPrev;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject BuyButton;

    private Color[] colors;

    //for the animation
    private float curSpeed;
    private float targetSpeed;

    private float curVel;
    private float targetVel;

    //for the text display
    private TextMeshProUGUI buttonText;

    private int skinCost;

    public int SkinCost
    {
        get { return skinCost; }
        set { skinCost = Mathf.Max(0, value); }
    }

    public Color[] PlayerColors
    {
        get { return colors; }
    }

    void Awake()
    {
        buttonText = BuyButton.GetComponentInChildren<TextMeshProUGUI>();

        curSpeed = 0;
        targetSpeed = TurnSpeedNormal;

        curVel = 0;
        targetVel = 0;

        if (HomeScreenController.PlayerColors != null)
            colors = HomeScreenController.PlayerColors;
        else
            colors = HomeScreenController.TempColors(); //temporarily create some testing colors for when the main screen hasn't been run yet

        //store should open on selected and owned skin
        BuyButton.SetActive(false);
        priceText.SetText("Selected");

        try
        {
            currentPrev.GetComponent<Image>().color = colors[Saver.loadData().SelectedSkin];
        }
        catch
        {
            Debug.LogError("Colors not initialized yet!");
        }
    }

    void Update()
    {
        //updating the player preview with a spin animation
        curSpeed = Mathf.SmoothDamp(curSpeed, targetSpeed, ref curVel, 0.5f);
        targetSpeed = Mathf.SmoothDamp(targetSpeed, TurnSpeedNormal, ref targetVel, TurnSpeedDecay);

        playerPrev.transform.Rotate(new Vector3(0, 0, curSpeed) * Time.deltaTime);
    }

    public void Spin()
    {
        curSpeed = TurnSpeedMax;
    }

    public void setColor(int col, bool purchased, bool selected)
    {
        if (col < 0 || col >= colors.Length)
            return;

        playerPrev.GetComponent<Image>().color = colors[col];

        priceText.SetText(purchased ? "" : skinCost.ToString());
        priceText.color = colors[col];

        if (!purchased)
        {
            BuyButton.SetActive(true);
            buttonText.SetText("Buy");
        }
        else
        {
            buttonText.SetText("Select");

            if (selected)
            {
                BuyButton.SetActive(false);
                priceText.SetText("Selected");
            }
            else
                BuyButton.SetActive(true);
        }
    }

    public void UpdateCurrent(int color)
    {
        currentPrev.GetComponent<Image>().color = colors[color];
    }
}
