using Godot;
using System;

public class Menu : CanvasLayer
{
    //Ui
    private Button startButton;
    private Button leaveButton;

    //Scenes
    private PackedScene gameScene;


    public override void _Ready()
    {
        gameScene = (PackedScene)GD.Load("Scenes/Game.tscn");

        startButton = (Button)GetNode("StartButton");
        startButton.Connect("pressed", this, nameof(StartGame));


        leaveButton = (Button)GetNode("LeaveButton");
        leaveButton.Connect("pressed", this, nameof(LeaveGame));
    }

    void StartGame()
    {
        DisableMenu();
        gameScene.Instance();
        AddChild((Game)gameScene.Instance());
    }

    void DisableMenu()
    {
        foreach (Control child in GetChildren())
        {
            if (child != GetNode("LeaveButton"))
            {
                child.Hide();
            }
        }
    }
    void LeaveGame()
    {
        if (Game.level != 0)
        {
            DataManager.UpdateLevel(Game.level);
        }
        else
        {
            DataManager.UpdateLevel(1);
        }
        DataManager.Save();
        GetTree().Quit();
    }
}
