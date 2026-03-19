using MauiGame.Models;
using MauiGame.ViewModels;
using Microsoft.Maui.Graphics;

namespace MauiGame.Graphics;

public class GameDrawable : IDrawable
{
    private GameViewModel _viewModel;
    private float _screenWidth;
    private float _screenHeight;
    private float _groundY;

    // Assets simulation
    private Color PlayerColor = Colors.Blue;
    private Color GroundColor = Colors.DarkGray;
    private Color BackgroundColor = Colors.SkyBlue;

    public GameDrawable(GameViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        _screenWidth = dirtyRect.Width;
        _screenHeight = dirtyRect.Height;
        _groundY = _screenHeight - 100; // Ground level

        // Draw Background
        canvas.FillColor = BackgroundColor;
        canvas.FillRectangle(dirtyRect);

        // Draw Ground
        canvas.FillColor = GroundColor;
        canvas.FillRectangle(0, _groundY, _screenWidth, 100);

        if (_viewModel.IsGameOver)
        {
            DrawGameOver(canvas);
            return;
        }

        DrawPlayer(canvas);
        DrawEnemies(canvas);
        DrawHUD(canvas);
    }

    private void DrawPlayer(ICanvas canvas)
    {
        float pX = _viewModel.Player.X;
        float pY = _groundY - _viewModel.Player.Height;

        // Player Body
        canvas.FillColor = PlayerColor;
        canvas.FillRectangle(pX, pY, _viewModel.Player.Width, _viewModel.Player.Height);

        // Draw Action Indicator if player is currently attacking
        if (_viewModel.Player.CurrentAction != PlayerAction.None && _viewModel.Player.ActionTimer > 0)
        {
            canvas.StrokeColor = Colors.Yellow;
            canvas.StrokeSize = 4;

            switch (_viewModel.Player.CurrentAction)
            {
                case PlayerAction.Punch:
                    // Middle attack
                    canvas.DrawLine(pX + _viewModel.Player.Width, pY + 20, pX + _viewModel.Player.Width + 30, pY + 20);
                    break;
                case PlayerAction.Kick:
                    // Low attack
                    canvas.DrawLine(pX + _viewModel.Player.Width, pY + _viewModel.Player.Height - 10, pX + _viewModel.Player.Width + 40, pY + _viewModel.Player.Height - 10);
                    break;
                case PlayerAction.Parry:
                    // Shield in front
                    canvas.DrawLine(pX + _viewModel.Player.Width + 5, pY, pX + _viewModel.Player.Width + 5, pY + _viewModel.Player.Height);
                    break;
            }
        }
    }

    private void DrawEnemies(ICanvas canvas)
    {
        foreach (var enemy in _viewModel.Enemies.ToList()) // ToList to avoid concurrent modification issues
        {
            float eX = enemy.X;
            float eY = _groundY - enemy.Height;

            // Enemy Body
            canvas.FillColor = enemy.GetColor();
            canvas.FillRectangle(eX, eY, enemy.Width, enemy.Height);

            // Draw Stance Indicator
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 3;

            switch (enemy.Stance)
            {
                case EnemyStance.SwordUp:
                    // Sword pointing UP (requires low kick to bypass)
                    canvas.DrawLine(eX, eY - 20, eX, eY);
                    break;
                case EnemyStance.SwordDown:
                    // Sword pointing DOWN (requires punch to head)
                    canvas.DrawLine(eX, eY + enemy.Height, eX, eY + enemy.Height + 20);
                    break;
                case EnemyStance.SwordInFront:
                    // Sword pointing FRONT (requires parry)
                    canvas.DrawLine(eX - 20, eY + 30, eX, eY + 30);
                    break;
            }
        }
    }

    public void UpdateScreenWidth(float width)
    {
        _screenWidth = width;
    }

    private void DrawHUD(ICanvas canvas)
    {
        canvas.FontColor = Colors.Black;
        canvas.FontSize = 24;
        canvas.DrawString($"Score: {_viewModel.Score}", 20, 30, HorizontalAlignment.Left);
        canvas.DrawString($"Level: {_viewModel.Level}", 20, 60, HorizontalAlignment.Left);
    }

    private void DrawGameOver(ICanvas canvas)
    {
        canvas.FontColor = Colors.Red;
        canvas.FontSize = 48;
        canvas.DrawString(_viewModel.GameOverMessage, _screenWidth / 2, _screenHeight / 2 - 50, HorizontalAlignment.Center);
        canvas.FontSize = 24;
        canvas.DrawString("Press Start to Play", _screenWidth / 2, _screenHeight / 2 + 20, HorizontalAlignment.Center);
    }
}
