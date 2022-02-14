using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    void Awake()
    {
        //load player data from file
        data = Saver.loadData();

        SpeedStartPrice = 35;
        MaxHealthStartPrice = 50;
        MaxDashStartPrice = 50;
        DashRechargeStartPrice = 60;
        JumpHeightStartPrice = 30;

        increaseRate = 0.5f;
    }

    //private methods
    private void updatePrices()
    {
        int SpeedAddition = (int)(SpeedStartPrice * data.SpeedUpgrade * increaseRate);
        int MaxHealthAddition = (int)(MaxHealthStartPrice * data.MaxHealthUpgrade * increaseRate);
        int MaxDashAddition = (int)(MaxDashStartPrice * data.MaxDashUpgrade * increaseRate);
        int DashRechargeAddition = (int)(DashRechargeStartPrice * data.DashRechargeUpgrade * increaseRate);
        int JumpHeightAddition = (int)(JumpHeightStartPrice * data.JumpHeightUpgrade * increaseRate);

        SpeedPrice.SetText("$" + (SpeedStartPrice + SpeedAddition).ToString());
        MaxHealthPrice.SetText("$" + (MaxHealthStartPrice + MaxHealthAddition).ToString());
        MaxDashPrice.SetText("$" + (MaxDashStartPrice + MaxDashAddition).ToString());
        DashRechargePrice.SetText("$" + (DashRechargeStartPrice + DashRechargeAddition).ToString());
        JumpHeightPrice.SetText("$" + (JumpHeightStartPrice + JumpHeightAddition).ToString());
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
        }

        //save
        Saver.SavePlayer(data);
        updateAllBars();
    }
    #endregion
}
