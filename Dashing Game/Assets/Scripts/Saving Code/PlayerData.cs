using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //data stored
    private bool newPlayer; // whether or not the player is new to the game
    private int money; // amount of in game currency the player has
    private int highScore;

    //Speed, MaxHealth, DashStamina, DashRecharge, JumpHeight
    private int Speed;
    private int MaxHealth;
    private int MaxDash;
    private int DashRecharge;
    private int JumpHeight;

    //Sound data
    private int Volume;
    private bool Music;

    //giving scripts access to the player data
    public bool isNewPlayer
    {
        get { return newPlayer; }
        set { newPlayer = value; }
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

    public PlayerData(bool newPlayer, int money, int highScore, 
        int Speed, int MaxHealth, int MaxDash, int DashRecharge, int JumpHeight,  //upgrades
        int Volume, bool Music) //sound
    {
        this.newPlayer = newPlayer;
        this.money = money;
        this.highScore = highScore;

        this.Speed = Speed;
        this.MaxHealth = MaxHealth;
        this.MaxDash = MaxDash;
        this.DashRecharge = DashRecharge;
        this.JumpHeight = JumpHeight;

        this.Volume = Volume;
        this.Music = Music;
    }
}
