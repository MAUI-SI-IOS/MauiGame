namespace MauiGame.Models;

public enum PlayerAction
{
    Punch,
    Kick,
    Parry,
    None
}

public enum EnemyStance
{
    SwordUp,     // Requires Kick (Low Kick)
    SwordDown,   // Requires Punch
    SwordInFront // Requires Parry
}
