using MauiGame.ViewModels;

namespace MauiGame.Views;

public partial class HallOfFamePage : ContentPage
{
    public HallOfFamePage()
    {
        InitializeComponent();
        BindingContext = new GameViewModel(); // Simple reuse for fetching scores
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
