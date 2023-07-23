using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UgradeScreenController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyDisplay;

    [Header("Different upgrade bars")]
    [SerializeField] private UpgradeBar SpeedUpgradeBar;
    [SerializeField] private UpgradeBar MaxHealthUpgradeBar;
    [SerializeField] private UpgradeBar MaxDashUpgradeBar;
    [SerializeField] private UpgradeBar DashRechargeUpgradeBar;
    [SerializeField] private UpgradeBar JumpHeightUpgradeBar;

    [Space]

    [Header("Button text for the prices of each upgrade")]
    [SerializeField] private TextMeshProUGUI SpeedPrice;
    [SerializeField] private TextMeshProUGUI MaxHealthPrice;
    [SerializeField] private TextMeshProUGUI MaxDashPrice;
    [SerializeField] private TextMeshProUGUI DashRechargePrice;
    [SerializeField] private TextMeshProUGUI JumpHeightPrice;

    [Space]

    [Header("Screens")]
    [SerializeField] private GameObject Screen1;
    [SerializeField] private GameObject Screen2;

    [Space]

    [Header("Crystal sprites")]
    [SerializeField] private Sprite[] CrystalSprites;
    [SerializeField] private Image CrystalImage;

    [Space]

    [SerializeField] private TextMeshProUGUI CrystalCostDisplay;
    [SerializeField] private GameObject CrystalBuyButton;

    [Space]
    [SerializeField] private PlayerCustomization playerCustomization;

    [Space]
    [Header("GameObjects that need to be shown / hidden")]
    [SerializeField] private GameObject RightCustomizationArrow;
    [SerializeField] private GameObject LeftCustomizationArrow;

    [SerializeField] private GameObject RightCrystalArrow;
    [SerializeField] private GameObject LeftCrystalArrow;

    [Space]
    [SerializeField] private GameObject CustomizationBuyButton;


    //private fields
    private PlayerData data;

    //start prices of all upgrades
    private int SpeedStartPrice;
    private int MaxHealthStartPrice;
    private int MaxDashStartPrice;
    private int DashRechargeStartPrice;
    private int JumpHeightStartPrice;

    //amount the prices are multiplied by each time the player buys
    private double increaseRate;

    //for the seperate screens
    private Animator screen1Anims;
    private Animator screen2Anims;

    //for buying crystals
    private int curCrystal;

    private int CrystalBaseCost;
    private int CrystalCostMultiplier;

    //for buying player customizations
    private int curColor;

    void Awake()
    {
        //load player data from file
        data = Saver.loadData();

        SpeedStartPrice = 35;
        MaxHealthStartPrice = 50;
        MaxDashStartPrice = 50;
        DashRechargeStartPrice = 60;
        JumpHeightStartPrice = 30;

        increaseRate = 2.4f;

        screen1Anims = Screen1.GetComponent<Animator>();
        screen2Anims = Screen2.GetComponent<Animator>();

        Screen1.SetActive(true); //make sure screen is showing in case it was hidden during editing
        Screen2.SetActive(true); 

        screen1Anims.SetTrigger("Show");

        curCrystal = 0;

        CrystalBaseCost = 100;
        CrystalCostMultiplier = 8;

        updateCrystalSprite();

        curColor = data.SelectedSkin;
        playerCustomization.SkinCost = 300; //keeping all the set prices in this file for organization

        //update arrows that should be showing or not
        if (curCrystal == 4)
            RightCrystalArrow.SetActive(false);
        else if (curCrystal == 0)
            LeftCrystalArrow.SetActive(false);

        if (curColor == playerCustomization.PlayerColors.Length - 1)
            RightCustomizationArrow.SetActive(false);
        else if (curColor == 0)
            LeftCustomizationArrow.SetActive(false);
    }

    //private methods
    private void updatePrices()
    {
        int SpeedAddition = (int)(SpeedStartPrice * data.SpeedUpgrade * increaseRate);
        int MaxHealthAddition = (int)(MaxHealthStartPrice * data.MaxHealthUpgrade * increaseRate);
        int MaxDashAddition = (int)(MaxDashStartPrice * data.MaxDashUpgrade * increaseRate);
        int DashRechargeAddition = (int)(DashRechargeStartPrice * data.DashRechargeUpgrade * increaseRate);
        int JumpHeightAddition = (int)(JumpHeightStartPrice * data.JumpHeightUpgrade * increaseRate);

        SpeedPrice.SetText(data.SpeedUpgrade == 5 ? "Max" : "$" + (SpeedStartPrice + SpeedAddition).ToString());
        MaxHealthPrice.SetText(data.MaxHealthUpgrade == 5 ? "Max" : "$" + (MaxHealthStartPrice + MaxHealthAddition).ToString());
        MaxDashPrice.SetText(data.MaxDashUpgrade == 5 ? "Max" : "$" + (MaxDashStartPrice + MaxDashAddition).ToString());
        DashRechargePrice.SetText(data.DashRechargeUpgrade == 5 ? "Max" : "$" + (DashRechargeStartPrice + DashRechargeAddition).ToString());
        JumpHeightPrice.SetText(data.JumpHeightUpgrade == 5 ? "Max" : "$" + (JumpHeightStartPrice + JumpHeightAddition).ToString());
    }


    private void updateAllBars()
    {
        SpeedUpgradeBar.UpdateDisplay(data);
        MaxHealthUpgradeBar.UpdateDisplay(data);
        MaxDashUpgradeBar.UpdateDisplay(data);
        DashRechargeUpgradeBar.UpdateDisplay(data);
        JumpHeightUpgradeBar.UpdateDisplay(data);
    }


    void Update()
    {
        MoneyDisplay.SetText(data.Money.ToString());

        //update the prices
        updatePrices();
    }

    #region Button methods for upgrading
    //button methods for upgrading
    public void UpgradeSpeed()
    {
        //calculate amount to buy
        int price = SpeedStartPrice + (int)(SpeedStartPrice * data.SpeedUpgrade * increaseRate);

        //if the player has enough, upgrade and save to file
        if(data.Money >= price && data.SpeedUpgrade < 5)
        {
            //spawn particles
            data.Money -= price;
            data.SpeedUpgrade += 1;

            playerCustomization.Spin();
        }

        //save
        Saver.SavePlayer(data);
        updateAllBars();
    }

    public void UpgradeMaxHealth()
    {
        //calculate amount to buy
        int price = MaxHealthStartPrice + (int)(MaxHealthStartPrice * data.MaxHealthUpgrade * increaseRate);

        //if the player has enough, upgrade and save to file
        if (data.Money >= price && data.MaxHealthUpgrade < 5)
        {
            //spawn particles
            data.Money -= price;
            data.MaxHealthUpgrade += 1;

            playerCustomization.Spin();
        }

        //save
        Saver.SavePlayer(data);
        updateAllBars();
    }

    public void UpgradeMaxDash()
    {
        //calculate amount to buy
        int price = MaxDashStartPrice + (int)(MaxDashStartPrice * data.MaxDashUpgrade * increaseRate);

        //if the player has enough, upgrade and save to file
        if (data.Money >= price && data.MaxDashUpgrade < 5)
        {
            //spawn particles
            data.Money -= price;
            data.MaxDashUpgrade += 1;

            playerCustomization.Spin();
        }

        //save
        Saver.SavePlayer(data);
        updateAllBars();
    }

    public void UpgradeDashRecharge()
    {
        //calculate amount to buy
        int price = DashRechargeStartPrice + (int)(DashRechargeStartPrice * data.DashRechargeUpgrade * increaseRate);

        //if the player has enough, upgrade and save to file
        if (data.Money >= price && data.DashRechargeUpgrade < 5)
        {
            //spawn particles
            data.Money -= price;
            data.DashRechargeUpgrade += 1;

            playerCustomization.Spin();
        }

        //save
        Saver.SavePlayer(data);
        updateAllBars();
    }

    public void UpgradeJump()
    {
        //calculate amount to buy
        int price = JumpHeightStartPrice + (int)(JumpHeightStartPrice * data.JumpHeightUpgrade * increaseRate);

        //if the player has enough, upgrade and save to file
        if (data.Money >= price && data.JumpHeightUpgrade < 5)
        {
            //spawn particles
            data.Money -= price;
            data.JumpHeightUpgrade += 1;

            playerCustomization.Spin();
        }

        //save
        Saver.SavePlayer(data);
        updateAllBars();
    }
    #endregion

    #region Button methods for changing screens
    public void ToScreen1()
    {
        curCrystal = 0;

        screen1Anims.SetTrigger("SlideInRight");
        screen2Anims.SetTrigger("SlideOutRight");
    }

    public void ToScreen2()
    {
        screen1Anims.SetTrigger("SlideOutLeft");
        screen2Anims.SetTrigger("SlideInLeft");
    }
    #endregion

    #region Methods for crystals
    public void NextCrystal()
    {
        //right arrow pressed
        curCrystal++;

        LeftCrystalArrow.SetActive(true);
        if (curCrystal == 4)
            RightCrystalArrow.SetActive(false);

        updateCrystalSprite();
    }

    public void LastCrystal()
    {
        //left arrow pressed
        curCrystal--;

        RightCrystalArrow.SetActive(true);
        if (curCrystal == 0)
            LeftCrystalArrow.SetActive(false);

        updateCrystalSprite();
    }

    public void BuyCrystal()
    {
        if(data.CrystalsUnlocked == curCrystal && data.Money >= CalcCrystalPrice())
        {
            data.CrystalsUnlocked++;
            data.Money -= CalcCrystalPrice();
            Saver.SavePlayer(data);

            updateCrystalSprite();
        }
    }

    private int CalcCrystalPrice()
    {
        return (CrystalBaseCost * CrystalCostMultiplier * (curCrystal + 1));
    }

    private void updateCrystalSprite()
    {
        CrystalImage.sprite = CrystalSprites[curCrystal];
        CrystalCostDisplay.SetText("$" + CalcCrystalPrice());
        Color temp = CrystalImage.color;

        //also update the buy button
        if (data.CrystalsUnlocked >= curCrystal)
        {
            CrystalBuyButton.SetActive(true);
            CrystalBuyButton.GetComponentInChildren<TextMeshProUGUI>().SetText(data.CrystalsUnlocked == curCrystal ? "Buy" : "Purchased");
            CrystalBuyButton.GetComponent<Button>().interactable = data.CrystalsUnlocked == curCrystal;

            if(data.CrystalsUnlocked != curCrystal)
                CrystalCostDisplay.SetText("");

            temp.a = 1f;
        }
        else
        {
            CrystalBuyButton.SetActive(false);
            temp.a = 0.5f;
        }
        CrystalImage.color = temp;
    }
    #endregion

    #region Methods for character customization
    public void NextColor()
    {
        //right button pressed
        curColor++;

        LeftCustomizationArrow.SetActive(true);
        if (curColor == playerCustomization.PlayerColors.Length - 1)
            RightCustomizationArrow.SetActive(false);

        playerCustomization.setColor(curColor, data.SkinsUnlocked.Contains(curColor), data.SelectedSkin == curColor);
    }

    public void LastColor()
    {
        //left button pressed
        curColor--;

        RightCustomizationArrow.SetActive(true);
        if (curColor == 0)
            LeftCustomizationArrow.SetActive(false);

        playerCustomization.setColor(curColor, data.SkinsUnlocked.Contains(curColor), data.SelectedSkin == curColor);
    }

    public void BuySkin()
    {
        if (!data.SkinsUnlocked.Contains(curColor) && data.Money >= playerCustomization.SkinCost)
        {
            data.SkinsUnlocked.Add(curColor);
            data.Money -= playerCustomization.SkinCost;

            data.SelectedSkin = curColor;

            Saver.SavePlayer(data);
        }
        else if (data.SkinsUnlocked.Contains(curColor) && data.SelectedSkin != curColor)
        {
            data.SelectedSkin = curColor;
            Saver.SavePlayer(data);
        }

        playerCustomization.setColor(curColor, data.SkinsUnlocked.Contains(curColor), data.SelectedSkin == curColor);
    }
    #endregion
}
