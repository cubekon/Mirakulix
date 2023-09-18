using Microsoft.UI.Xaml.Controls;

using Mirar.ViewModels;

namespace Mirar.Views;

public sealed partial class GridPage : Page
{
    public GridViewModel ViewModel
    {
        get;
    }

    public GridPage()
    {
        ViewModel = App.GetService<GridViewModel>();
        InitializeComponent();
    }
}
