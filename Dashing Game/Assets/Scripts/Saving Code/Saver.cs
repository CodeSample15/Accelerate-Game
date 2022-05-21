using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Saver
{
    public static void SavePlayer(PlayerData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerData.dta";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayer(bool newPlayer, int money, int highScore, int Speed, int MaxHealth, int MaxDash, int DashRecharge, int JumpHeight, int Volume, bool Music)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerData.dta";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(newPlayer, money, highScore, Speed, MaxHealth, MaxDash, DashRecharge, JumpHeight, Volume, Music);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData loadData()
    {
        string path = Application.persistentDataPath + "/playerData.dta";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("File not created yet!");
            return null;
        }
    }
}