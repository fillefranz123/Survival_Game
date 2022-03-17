using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class SaveSystem: MonoBehaviour
{
    [SerializeField] string dataPath = "/gameData";
    [SerializeField] string inventoryPath = "/inventory.data";

    FileStream datastream;
    IFormatter formatter;

    private void Start()
    {
        formatter = new BinaryFormatter();
        if (!Directory.Exists(string.Concat(Application.persistentDataPath, dataPath)))
        {
            Directory.CreateDirectory(string.Concat(Application.persistentDataPath, dataPath));
        }

    }
   
}
