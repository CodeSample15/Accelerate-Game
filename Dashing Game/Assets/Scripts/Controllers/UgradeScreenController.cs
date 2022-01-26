using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UgradeScreenController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyDisplay;

    //private fields
    private int money;

    void Awake()
    {
        //load player data from file
        PlayerData data = Saver.loadData();

        money = data.Money;
    }

    void Update()
    {
        MoneyDisplay.SetText(money.ToString());
    }
}
