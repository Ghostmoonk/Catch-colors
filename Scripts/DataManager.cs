using Godot;
using System;
using Newtonsoft.Json;

public class DataManager : Node
{
    private const string SAVE_PATH = "D:\\Projets\\Games\\Godot\\Catch Colors\\save.json";
    private static DataModel _data = new DataModel();
    public override void _Ready()
    {
        GD.Print("ReadSaveFile");
        ReadSaveFile();
    }

    public static void Save()
    {
        WriteSaveFile();
    }
    public void ReadSaveFile()
    {
        string jsonString = null;
        var saveFile = OpenSaveFile(File.ModeFlags.Read);
        if (saveFile != null)
        {
            jsonString = saveFile.GetLine();
            try
            {
                _data = Deserialize(jsonString);
                GD.Print(_data);
            }
            catch
            {
                _data = new DataModel();
            }
            saveFile.Close();
        }
    }

    public static void AddLevel()
    {
        _data.level += 1;
    }

    public static int GetLevel()
    {
        return _data.level;
    }

    public static void UpdateLevel(int level)
    {
        _data.level = Game.level;
    }
    private static void WriteSaveFile()
    {
        var saveFile = OpenSaveFile(File.ModeFlags.Write);
        if (saveFile != null)
        {
            var json = JsonConvert.SerializeObject(_data);
            saveFile.StoreLine(json);
            saveFile.Close();
        }
    }
    private static File OpenSaveFile(File.ModeFlags flag = File.ModeFlags.Read)
    {
        var saveFile = new File();
        var err = saveFile.Open(SAVE_PATH, (int)flag);
        GD.Print(err);
        if (err == 0)
        {
            return saveFile;
        }
        return null;
    }
    private static DataModel Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<DataModel>(json);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
