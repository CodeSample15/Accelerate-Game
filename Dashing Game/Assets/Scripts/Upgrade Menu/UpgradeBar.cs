using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBar : MonoBehaviour
{
    private enum StatTypes { Speed, MaxHealth, MaxDash, DashRecharge, JumpHeight };

    [Header("Select Stat:")]
    [SerializeField] private StatTypes statType;

    [SerializeField] private GameObject EmptyBar;
    [SerializeField] private GameObject FullBar;


    private List<GameObject> bars;
    private PlayerData loaded_data;

    //adjustable variables
    private float barDistance;

    void Awake()
    {
        //load data from save file
        loaded_data = Saver.loadData();
        bars = new List<GameObject>();

        barDistance = 11f;

        UpdateDisplay(loaded_data);
    }

    //get the amount of upgrades that the player has put on a stat
    private int getAmountOfBars(PlayerData data)
    {
        int output = 0;

        switch (statType) 
        {
            case StatTypes.Speed:
                output = data.SpeedUpgrade;
                break;

            case StatTypes.MaxHealth:
                output = data.MaxHealthUpgrade;
                break;

            case StatTypes.MaxDash:
                output = data.MaxDashUpgrade;
                break;

            case StatTypes.DashRecharge:
                output = data.DashRechargeUpgrade;
                break;

            case StatTypes.JumpHeight:
                output = data.JumpHeightUpgrade;
                break;
        }

        return output;
    }

    public void UpdateDisplay(PlayerData data)
    {
        //clear preexisting bars
        foreach(GameObject bar in bars)
        {
            Destroy(bar.gameObject);
        }
        bars.Clear();

        //start position for the progress bar
        float x = transform.position.x;

        for (int i = 0; i < getAmountOfBars(data); i++) {
            bars.Add(Instantiate(FullBar, new Vector2(x, transform.position.y), Quaternion.identity));
            bars[bars.Count-1].transform.parent = gameObject.transform;
            x += barDistance;
        }
        for(int i=0; i<5-getAmountOfBars(data); i++)
        {
            bars.Add(Instantiate(EmptyBar, new Vector2(x, transform.position.y), Quaternion.identity));
            bars[bars.Count - 1].transform.parent = gameObject.transform;
            x += barDistance;
        }
    }
}
