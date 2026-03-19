namespace MauiGame.Models;

public class ScoreEntry
{
    public string Name { get; set; } = "Player";
    public int Score { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}
