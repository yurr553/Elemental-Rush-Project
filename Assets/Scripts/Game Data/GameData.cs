using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;





[Serializable]
public class SavaData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
}


public class GameData : MonoBehaviour
{

    public static GameData gameData;
    public SavaData saveData;


    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();
    }

    public void Save()
    {
        //Creating a binary formatter which can read the binary file
        BinaryFormatter formatter = new BinaryFormatter();

        //Create a route from the program to the file
        FileStream file = File.Open(Application.persistentDataPath + "/player.data1", FileMode.Create);
        //Create a copy of the data for the player
        SavaData data = new SavaData();
        data = saveData;
        //Saves the data in the file
        formatter.Serialize(file, data);
        //Closes the data stream
        file.Close();

        Debug.Log("Saved");
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.data1"))
        {
            //Create a Binary Formatter
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.data1", FileMode.Open);
            saveData = formatter.Deserialize(file) as SavaData;
            file.Close();
            Debug.Log("Loaded");
        }
        else
        {
            
        }
    }

    private void Start()
    {
        
    }

    private void OnDisable()
    {
        
       Save();
        
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
