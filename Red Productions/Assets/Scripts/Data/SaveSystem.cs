using UnityEngine;
using System.IO;


public static class SaveSystem
{
    private static string fileName = "gameSave.dat";
    public static void SerializeData(SaveData data)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(data);
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.Write(json);
        }
        Debug.Log("saved game to" + path);
    }

    public static SaveData DeserializeData()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("Save loaded from " + path);
                return saveData;
            }
        }
        else
        {
            Debug.Log("Save file not found " + path);
            return null;
        }
    }
}
