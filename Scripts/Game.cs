using Godot;
using System;

public class Game : Node2D
{
    private Joueur joueur;
    private PathFollow2D path;
    private PackedScene asteroidScene;
    //Values
    private int score;
    public static int level;
    private int objective;
    private bool succesLevel;
    //Randoms
    private static Random randomAsteroidPos;
    private static Random randomAsteroidRotation;
    //Timers
    private Timer asteroidSpawnTimer;
    //Ui
    private Label scoreLabel;
    private Label message;
    private Button restartButton;

    private const int OBJECTIVE_PER_LEVEL = 1;

    //Save

    public override void _Ready()
    {
        level = DataManager.GetLevel();
        if (level < 1)
        {
            level = 1;
        }
        objective = level * OBJECTIVE_PER_LEVEL;

        joueur = (Joueur)GetNode("Joueur");
        joueur.Connect("hit", this, nameof(GameOver));
        joueur.Connect("scoreUp", this, nameof(IncreaseScore));

        randomAsteroidPos = new Random();
        randomAsteroidRotation = new Random();

        asteroidSpawnTimer = (Timer)GetNode("AsteroidSpawnTimer");
        asteroidSpawnTimer.Connect("timeout", this, nameof(OnAsteroidSpawn));

        path = (PathFollow2D)GetNode("Path2D/PathFollow2D");
        asteroidScene = (PackedScene)GD.Load("Scenes/Asteroid.tscn");

        asteroidSpawnTimer.Start();

        //UI
        scoreLabel = (Label)GetNode("CanvasLayer/ScoreLabel");
        message = (Label)GetNode("CanvasLayer/MessageLabel");
        restartButton = (Button)GetNode("CanvasLayer/RestartButton");
        scoreLabel.Text = score.ToString() + " / " + objective;
        restartButton.Connect("pressed", this, nameof(ChooseContinue));

        //Saves
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    private void OnAsteroidSpawn()
    {
        path.SetOffset(randomAsteroidPos.Next());
        Vector2 pos = path.GetPosition();

        float rotation = path.GetRotation() + Mathf.Deg2Rad(90 + randomAsteroidRotation.Next(-45, 46));
        Asteroid asteroid = (Asteroid)asteroidScene.Instance();
        asteroid.SetPosition(pos);
        asteroid.SetRotation(rotation);
        asteroid.SetLinearVelocity(new Vector2(10 * Asteroid.speed, 0).Rotated(rotation));
        AddChild(asteroid);

    }
    private void GameOver()
    {
        joueur.Hide();
        asteroidSpawnTimer.Stop();
        ClearAsteroids();
        message.Text = "Game over !";
        restartButton.Text = "Try again";
        message.Show();
        restartButton.Show();
        succesLevel = false;
    }

    void SuccessLevel()
    {
        asteroidSpawnTimer.Stop();
        ClearAsteroids();
        restartButton.Text = "Next Level";
        message.Text = "Well Played !";
        message.Show();
        restartButton.Show();
        succesLevel = true;
    }
    void ClearAsteroids()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Asteroid)
            {
                child.QueueFree();
            }
        }
    }

    void ChooseContinue()
    {
        Restart(succesLevel);
    }

    private void IncreaseScore()
    {
        score += 1;
        scoreLabel.Text = score.ToString() + " / " + objective;
        if (score == objective)
        {
            SuccessLevel();
        }
    }

    private void Restart(bool successPrevLevel)
    {
        //Hide end level UI
        message.Hide();
        restartButton.Hide();

        //Reset score
        score = 0;

        if (successPrevLevel)
        {
            level += 1;
            objective = level * OBJECTIVE_PER_LEVEL;
        }
        else
        {
            joueur.Show();
        }
        scoreLabel.Text = score.ToString() + " / " + objective;
        //Relaunch asteroids
        asteroidSpawnTimer.Start();
    }

}
