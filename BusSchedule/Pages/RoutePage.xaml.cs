using BusSchedule.Core.Model;
using Microsoft.Maui.Controls;

namespace BusSchedule.Pages;

public partial class RoutePage : TabbedPage
{
	public RoutePage()
	{
		InitializeComponent();
	}

    public RoutePage(Routes route, string destinationName, int? direction)
    {
    }
}