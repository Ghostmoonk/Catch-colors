using Godot;
using System;

public class Joueur : Area2D
{
    [Export]
    private int SPEED = 400;
    private Color actualColor;
    private AnimatedSprite animatedSprite;
    private Sprite colorSprite;
    private CollisionShape2D collisionShape2D;
    private AnimationPlayer animationPlayer;
    private Particles2D particles;
    private Vector2 velocity;
    private Vector2 screenSize;
    private Timer changeColorTimer;
    private Random colorRandom;

    private Timer AnimationColorTimer;

    public override void _Ready()
    {
        screenSize = GetViewportRect().Size;

        colorRandom = new Random();
        animatedSprite = (AnimatedSprite)GetNode("AnimatedSprite");
        colorSprite = (Sprite)GetNode("ColorSprite");
        actualColor = colorSprite.Modulate;
        collisionShape2D = (CollisionShape2D)GetNode("CollisionShape2D");
        particles = (Particles2D)GetNode("Particles2D");
        animationPlayer = (AnimationPlayer)GetNode("Control/AnimationPlayer");

        changeColorTimer = (Timer)GetNode("ChangeColorTimer");
        changeColorTimer.Start();

        Connect("body_entered", this, nameof(OnBodyEntered));
        changeColorTimer.Connect("timeout", this, nameof(ChangeColor));
        AddUserSignal("scoreUp");
        AddUserSignal("hit");

        SetPosition(GetViewportRect().Size / 2);
        colorSprite.Modulate = Colors.colors[colorRandom.Next(0, Colors.colors.Length)];
        actualColor = colorSprite.Modulate;
        AnimationColorTimer = (Timer)GetNode("AnimationColorTimer");
    }

    public override void _Process(float delta)
    {
        velocity = new Vector2();
        if (Input.IsActionPressed("ui_right"))
        {
            velocity.x += 1;
        }
        if (Input.IsActionPressed("ui_left"))
        {
            velocity.x -= 1;
        }
        if (Input.IsActionPressed("ui_up"))
        {
            velocity.y -= 1;
        }
        if (Input.IsActionPressed("ui_down"))
        {
            velocity.y += 1;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * SPEED;
            animatedSprite.Play();
            particles.SetEmitting(true);
        }
        else
        {
            particles.SetEmitting(false);
            animatedSprite.Animation = "idle";
        }
        Position += velocity * delta;

        Position = new Vector2(
            Mathf.Clamp(Position.x, 0, screenSize.x),
            Mathf.Clamp(Position.y, 0, screenSize.y)
        );

        if (velocity.x != 0)
        {
            animatedSprite.Animation = "walk";
            animatedSprite.FlipH = velocity.x < 0;
            animatedSprite.FlipV = false;
            colorSprite.Rotation = Mathf.Deg2Rad(45f);
        }
        else if (velocity.y != 0)
        {
            animatedSprite.Animation = "walk";
            animatedSprite.FlipV = velocity.y > 0;

            if (velocity.y > 0)
            {
                colorSprite.Rotation = Mathf.Deg2Rad(225f);
            }
            else
            {
                colorSprite.Rotation = Mathf.Deg2Rad(45f);
            }
        }
    }

    void OnBodyEntered(PhysicsBody2D body)
    {
        if (body is Asteroid)
        {
            Asteroid asteroid = (Asteroid)body;
            if (asteroid.sprite.Modulate == this.actualColor)
            {
                EmitSignal("scoreUp");
                body.QueueFree();
            }
            else
            {
                EmitSignal("hit");
            }
        }
    }

    async private void ChangeColor()
    {
        animationPlayer.PlayBackwards("Flash");

        AnimationColorTimer.Start();
        await ToSignal(AnimationColorTimer, "timeout");
        animationPlayer.Play("Flash");
        colorSprite.Modulate = Colors.colors[colorRandom.Next(0, Colors.colors.Length)];
        actualColor = colorSprite.Modulate;
    }

}
