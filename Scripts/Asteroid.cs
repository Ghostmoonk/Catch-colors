using Godot;
using System;

public class Asteroid : RigidBody2D
{
    public Sprite sprite;
    public static float speed = 50f;
    private VisibilityNotifier2D visibility;

    //private static string[] colors = { "ff0000", "00ff00", "0000ff" };

    private Random colorRandom;
    public override void _Ready()
    {
        colorRandom = new Random();
        visibility = (VisibilityNotifier2D)GetNode("VisibilityNotifier2D");
        visibility.Connect("screen_exited", this, nameof(OnScreenExited));
        sprite = (Sprite)GetNode("Sprite");

        sprite.Modulate = Colors.colors[colorRandom.Next(0, Colors.colors.Length)];
    }
    public override void _Process(float delta)
    {
        sprite.Rotate(Mathf.Deg2Rad(1f));
    }


    private void OnScreenExited()
    {
        QueueFree();
    }

}
