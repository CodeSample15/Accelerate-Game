using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //data stored
    private bool newPlayer; // whether or not the player is new to the game
    private bool visitedStore; //whether or not the player has visited the store before in this game (to trigger tutorial)
    private int money; // amount of in game currency the player has
    private int highScore;

    //Speed, MaxHealth, DashStamina, DashRecharge, JumpHeight
    private int Speed;
    private int MaxHealth;
    private int MaxDash;
    private int DashRecharge;
    private int JumpHeight;

    //Crystals unlocked
    private int crystalsUnlocked;

    //Skins unlocked
    private List<int> skinsUnlocked;
    private int selectedSkin; //ID of the currently selected skin

    //Bosses defeated
    private bool blueBossDefeated;
    private bool greenBossDefeated;
    private bool orangeBossDefeated;
    private bool pinkBossDefeated;
    private bool redBossDefeated;

    //Sound data
    private int Volume;
    private bool Music;

    //giving scripts access to the player data
    public bool isNewPlayer
    {
        get { return newPlayer; }
        set { newPlayer = value; }
    }

    public bool VisitedStore
    {
        get { return visitedStore; }
        set { visitedStore = value; }
    }

    public int Money
    {
        get { return money; }
        set { money = Mathf.Max(0, value); }
    }

    public int HighScore
    {
        get { return highScore; }
        set { highScore = value; }
    }

    //upgrades
    public int SpeedUpgrade
    {
        get { return Speed; }
        set { Speed = Mathf.Min(Mathf.Max(value, 0), 5); } //making sure the number is between 0 and 5
    }
    public int MaxHealthUpgrade
    {
        get { return MaxHealth; }
        set { MaxHealth = Mathf.Min(Mathf.Max(value, 0), 5); } //making sure the number is between 0 and 5
    }
    public int MaxDashUpgrade
    {
        get { return MaxDash; }
        set { MaxDash = Mathf.Min(Mathf.Max(value, 0), 5); } //making sure the number is between 0 and 5
    }
    public int DashRechargeUpgrade
    {
        get { return DashRecharge; }
        set { DashRecharge = Mathf.Min(Mathf.Max(value, 0), 5); } //making sure the number is between 0 and 5
    }
    public int JumpHeightUpgrade
    {
        get { return JumpHeight; }
        set { JumpHeight = Mathf.Min(Mathf.Max(value, 0), 5); } //making sure the number is between 0 and 5
    }

    public int CrystalsUnlocked
    {
        get { return crystalsUnlocked; }
        set { crystalsUnlocked = Mathf.Min(Mathf.Max(value, 0), 5); }
    }

    public List<int> SkinsUnlocked
    {
        get { return skinsUnlocked; }
        set { skinsUnlocked = value; }
    }

    public int SelectedSkin
    {
        get { return selectedSkin; }
        set { selectedSkin = Mathf.Min(value, HomeScreenController.PlayerColors.Length); }
    }
    
    //defeated bosses
    public bool BlueBossDefeated
    {
        get { return blueBossDefeated; }
        set { blueBossDefeated = value; }
    }

    public bool GreenBossDefeated
    {
        get { return greenBossDefeated; }
        set { greenBossDefeated = value; }
    }

    public bool OrangeBossDefeated
    {
        get { return orangeBossDefeated; }
        set { orangeBossDefeated = value; }
    }

    public bool PinkBossDefeated
    {
        get { return pinkBossDefeated; }
        set { pinkBossDefeated = value; }
    }

    public bool RedBossDefeated
    {
        get { return redBossDefeated; }
        set { redBossDefeated = value; }
    }

    //sound
    public int VolumeLevel
    {
        get { return Volume; }
        set { Volume = value; }
    }
    public bool MusicPlaying
    {
        get { return Music; }
        set { Music = value; }
    }

    public PlayerData(bool newPlayer, bool visitedStore, int money, int highScore, //basic stats
        int Speed, int MaxHealth, int MaxDash, int DashRecharge, int JumpHeight, //upgrades
        int crystalsUnlocked,
        List<int> skinsUnlocked, int selectedSkin,
        bool blueBossDefeated, bool greenBossDefeated, bool orangeBossDefeated, bool pinkBossDefeated, bool redBossDefeated,
        int Volume, bool Music) //sound
    {
        this.newPlayer = newPlayer;
        this.visitedStore = visitedStore;
        this.money = money;
        this.highScore = highScore;

        this.Speed = Speed;
        this.MaxHealth = MaxHealth;
        this.MaxDash = MaxDash;
        this.DashRecharge = DashRecharge;
        this.JumpHeight = JumpHeight;

        this.crystalsUnlocked = crystalsUnlocked;

        this.skinsUnlocked = skinsUnlocked;
        this.selectedSkin = selectedSkin;

        this.blueBossDefeated = blueBossDefeated;
        this.greenBossDefeated = greenBossDefeated;
        this.orangeBossDefeated = orangeBossDefeated;
        this.pinkBossDefeated = pinkBossDefeated;
        this.redBossDefeated = redBossDefeated;

        this.Volume = Volume;
        this.Music = Music;
    }
}
