using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SettingManager
{
    public class SettingData
    {
        public string remoteAddress = "43.249.193.233";

        public ushort remotePort = 55217;

        public int targetFrameRate = 120;
    }

    public static SettingData Data = null;

    private static string FileDirectory = Application.persistentDataPath;

    private static string FileName = "settings.json";


    public static void Load()
    {
        string path = Path.Combine(FileDirectory, FileName);
        if (File.Exists(path))
        {
            Data = JsonConvert.DeserializeObject<SettingData>(File.ReadAllText(path));
        }
        else
        {
            Data = new SettingData();
            Save();
        }
    }


    public static void Save()
    {
        File.WriteAllText(Path.Combine(FileDirectory, FileName),
            JsonConvert.SerializeObject(Data, Formatting.Indented));
    }
}