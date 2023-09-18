using CommunityToolkit.WinUI.UI.Controls;

using Microsoft.UI.Xaml.Controls;

using Mirar.ViewModels;

namespace Mirar.Views;

public sealed partial class ListPage : Page
{
    public ListViewModel ViewModel
    {
        get;
    }

    public ListPage()
    {
        ViewModel = App.GetService<ListViewModel>();
        InitializeComponent();
    }

    private void OnViewStateChanged(object sender, ListDetailsViewState e)
    {
        if (e == ListDetailsViewState.Both)
        {
            ViewModel.EnsureItemSelected();
        }
    }
}
