namespace MauiGame.Models;

public class Player
{
    public float X { get; set; } = 50;
    public float Y { get; set; }
    public float Width { get; set; } = 40;
    public float Height { get; set; } = 60;

    // Animation/State info
    public PlayerAction CurrentAction { get; set; } = PlayerAction.None;
    public float ActionTimer { get; set; } = 0; // How long to display the action
    public const float MaxActionTime = 0.5f; // Seconds
}
