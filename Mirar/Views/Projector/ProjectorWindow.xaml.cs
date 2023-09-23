using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mirar.ViewModels;

namespace Mirar.Views.Projector;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProjectorWindow : WindowEx
{
    private bool _isInitialized;

    public ProjectorViewModel ViewModel
    {
        get;
    }

    public ProjectorWindow()
    {
        ViewModel = App.GetService<ProjectorViewModel>();

        InitializeComponent();

        Root.Loaded += Root_Loaded;
    }

    private async void Root_Loaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null && _isInitialized == false)
        {
            await ViewModel.InitializeAsync();
            _isInitialized = true;
        }
    }
}
