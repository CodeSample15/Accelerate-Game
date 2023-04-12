using System.Collections;
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

    [Header("Character customization colors")]
    [SerializeField] public Color[] PlayerColors;

    //for the animation
    private GameObject playerPrev;

    private float curSpeed;
    private float targetSpeed;

    private float curVel;
    private float targetVel;

    //for the text display
    private TextMeshProUGUI priceText;

    private int skinCost;

    public int SkinCost
    {
        get { return skinCost; }
        set { skinCost = Mathf.Max(0, value); }
    }

    void Awake()
    {
        playerPrev = transform.Find("Player Preview").gameObject;
        priceText = transform.Find("Price Text").GetComponent<TextMeshProUGUI>();

        curSpeed = 0;
        targetSpeed = TurnSpeedNormal;

        curVel = 0;
        targetVel = 0;
    }

    void Start()
    {
        priceText.SetText("$" + skinCost);
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

    public void setColor(int col)
    {
        if (col < 0 || col >= PlayerColors.Length)
            return;

        playerPrev.GetComponent<Image>().color = PlayerColors[col];
        priceText.color = PlayerColors[col];
    }
}
