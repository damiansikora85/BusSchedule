using CommunityToolkit.Maui.Views;

namespace BusSchedule.Popups;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class FavoritesInfoPopup : Popup
{
    public FavoritesInfoPopup()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}