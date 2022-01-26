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

    //private fields
    private PlayerData data;

    private int money;

    void Awake()
    {
        //load player data from file
        data = Saver.loadData();

        money = data.Money;
    }

    void Update()
    {
        MoneyDisplay.SetText(money.ToString());

        //update all of the bars
        SpeedUpgradeBar.UpdateDisplay(data);
        MaxHealthUpgradeBar.UpdateDisplay(data);
        MaxDashUpgradeBar.UpdateDisplay(data);
        DashRechargeUpgradeBar.UpdateDisplay(data);
        JumpHeightUpgradeBar.UpdateDisplay(data);
    }
}
