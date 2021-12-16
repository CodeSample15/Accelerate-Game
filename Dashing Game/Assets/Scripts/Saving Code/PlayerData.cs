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


    public PlayerData(bool newPlayer, int money, int highScore)
    {
        this.newPlayer = newPlayer;
        this.money = money;
        this.highScore = highScore;
    }
}
