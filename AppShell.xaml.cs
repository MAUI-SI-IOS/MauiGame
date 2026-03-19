using MauiGame.Views;

﻿namespace MauiGame
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("HallOfFame", typeof(HallOfFamePage));
        }
    }
}
