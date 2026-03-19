using MauiGame.Graphics;
using MauiGame.Models;
using MauiGame.ViewModels;

﻿namespace MauiGame
{
    public partial class MainPage : ContentPage
    {
        private GameViewModel _viewModel;
        private GameDrawable _gameDrawable;
        private bool _isGameLoopRunning;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new GameViewModel();
            _gameDrawable = new GameDrawable(_viewModel);
            GameCanvas.Drawable = _gameDrawable;
            BindingContext = _viewModel;
        }

        private void OnGameCanvasSizeChanged(object sender, EventArgs e)
        {
            _viewModel.ScreenWidth = (float)GameCanvas.Width;
            _gameDrawable.UpdateScreenWidth((float)GameCanvas.Width);
        }

        private void OnStartGameClicked(object sender, EventArgs e)
        {
            _viewModel.StartGame();
            if (!_isGameLoopRunning)
            {
                StartGameLoop();
            }
        }

        private async void OnHallOfFameClicked(object sender, EventArgs e)
        {
            // Will navigate to HallOfFamePage when it's ready
            await Shell.Current.GoToAsync("HallOfFame");
        }

        private void StartGameLoop()
        {
            _isGameLoopRunning = true;

            // Running at roughly 60 FPS (16ms delay)
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
            {
                if (!_viewModel.IsGameOver)
                {
                    _viewModel.UpdateGameLogic(0.016f); // Passing roughly the deltaTime
                    GameCanvas.Invalidate(); // Request redraw
                }

                if (_viewModel.IsGameOver)
                {
                    GameCanvas.Invalidate(); // One last draw for Game Over screen
                    _isGameLoopRunning = false;
                    return false; // Stop timer
                }

                return true; // Continue timer
            });
        }

        private void OnPunchClicked(object sender, EventArgs e) => _viewModel.PerformAction(PlayerAction.Punch);
        private void OnParryClicked(object sender, EventArgs e) => _viewModel.PerformAction(PlayerAction.Parry);
        private void OnKickClicked(object sender, EventArgs e) => _viewModel.PerformAction(PlayerAction.Kick);
    }
}
