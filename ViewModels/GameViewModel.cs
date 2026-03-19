using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using MauiGame.Models;
using Microsoft.Maui.Graphics;

namespace MauiGame.ViewModels;

public class GameViewModel : INotifyPropertyChanged
{
    private int _score;
    private int _level = 1;
    private bool _isGameOver = true;
    private string _gameOverMessage = "Press Start to Play";

    public int Score
    {
        get => _score;
        set { _score = value; OnPropertyChanged(); CheckLevelProgression(); }
    }

    public int Level
    {
        get => _level;
        set { _level = value; OnPropertyChanged(); }
    }

    public bool IsGameOver
    {
        get => _isGameOver;
        set { _isGameOver = value; OnPropertyChanged(); }
    }

    public string GameOverMessage
    {
        get => _gameOverMessage;
        set { _gameOverMessage = value; OnPropertyChanged(); }
    }

    public ObservableCollection<ScoreEntry> HighScores { get; set; } = new();

    public Player Player { get; set; } = new();
    public List<Enemy> Enemies { get; set; } = new();

    public float GameSpeed => 5f + (Level * 1.5f);
    public float SpawnRate => Math.Max(0.5f, 2.5f - (Level * 0.2f)); // Seconds between spawns

    public GameViewModel()
    {
        LoadHighScores();
    }

    public void StartGame()
    {
        Score = 0;
        Level = 1;
        IsGameOver = false;
        Enemies.Clear();
        Player.CurrentAction = PlayerAction.None;
        Player.ActionTimer = 0;
        GameOverMessage = string.Empty;
    }

    public void UpdateGameLogic(float deltaTime)
    {
        if (IsGameOver) return;

        // Decrease action timer
        if (Player.ActionTimer > 0)
        {
            Player.ActionTimer -= deltaTime;
            if (Player.ActionTimer <= 0)
            {
                Player.ActionTimer = 0;
                Player.CurrentAction = PlayerAction.None;
            }
        }

        // Spawn Enemies
        TrySpawnEnemy(deltaTime);

        // Move Enemies and Check Collisions
        for (int i = Enemies.Count - 1; i >= 0; i--)
        {
            var enemy = Enemies[i];
            enemy.X -= enemy.Speed;

            // Collision range (when enemy reaches player)
            float hitRangeEnd = Player.X + Player.Width + 10;
            float hitRangeStart = Player.X;

            if (enemy.X <= hitRangeEnd && enemy.X >= hitRangeStart)
            {
                // Has the player performed the correct action?
                if (Player.CurrentAction == enemy.RequiredAction && Player.ActionTimer > 0)
                {
                    // Defeated!
                    Score++;
                    Enemies.RemoveAt(i);
                }
                else
                {
                    // Hit the player! Game Over
                    HandleGameOver();
                    return; // Stop processing further enemies
                }
            }
            else if (enemy.X < hitRangeStart) // Passed without hitting?
            {
                // This shouldn't happen unless player speed > enemy speed, but just in case
                Enemies.RemoveAt(i);
            }
        }
    }

    public float ScreenWidth { get; set; } = 800; // Will be updated by view

    private float _spawnTimer = 0;
    private void TrySpawnEnemy(float deltaTime)
    {
        _spawnTimer -= deltaTime;
        if (_spawnTimer <= 0)
        {
            var random = new Random();
            var stance = (EnemyStance)random.Next(0, 3);

            Enemies.Add(new Enemy
            {
                X = ScreenWidth > 0 ? ScreenWidth : 800,
                Speed = GameSpeed,
                Stance = stance
            });
            _spawnTimer = SpawnRate;
        }
    }

    public void PerformAction(PlayerAction action)
    {
        if (IsGameOver) return;

        Player.CurrentAction = action;
        Player.ActionTimer = Player.MaxActionTime;
    }

    private void CheckLevelProgression()
    {
        // Level up every 5 enemies defeated
        if (Score > 0 && Score % 5 == 0)
        {
            Level++;
        }
    }

    public void HandleGameOver()
    {
        IsGameOver = true;
        GameOverMessage = $"Game Over! Score: {Score}";
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        if (Score > 0)
        {
            HighScores.Add(new ScoreEntry { Score = Score });

            var sortedScores = HighScores.OrderByDescending(s => s.Score).Take(10).ToList();
            HighScores.Clear();
            foreach (var score in sortedScores)
            {
                HighScores.Add(score);
            }

            string json = JsonSerializer.Serialize(HighScores.ToList());
            Preferences.Default.Set("HighScores", json);
        }
    }

    private void LoadHighScores()
    {
        string json = Preferences.Default.Get("HighScores", string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                var scores = JsonSerializer.Deserialize<List<ScoreEntry>>(json);
                if (scores != null)
                {
                    HighScores.Clear();
                    foreach (var s in scores) HighScores.Add(s);
                }
            }
            catch
            {
                // Ignored
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
