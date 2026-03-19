using Microsoft.Maui.Graphics;

namespace MauiGame.Models;

public class Enemy
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; } = 40;
    public float Height { get; set; } = 60;
    public float Speed { get; set; } = 5f;
    public EnemyStance Stance { get; set; }

    public PlayerAction RequiredAction => Stance switch
    {
        EnemyStance.SwordUp => PlayerAction.Kick,
        EnemyStance.SwordDown => PlayerAction.Punch,
        EnemyStance.SwordInFront => PlayerAction.Parry,
        _ => PlayerAction.None
    };

    public Color GetColor() => Stance switch
    {
        EnemyStance.SwordUp => Colors.Red,        // Threat from above
        EnemyStance.SwordDown => Colors.Blue,      // Threat from below
        EnemyStance.SwordInFront => Colors.Green,  // Threat straight on
        _ => Colors.Gray
    };
}
