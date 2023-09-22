using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Mirar.ViewModels;

namespace Mirar.Views.Projector;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProjectorWindow : WindowEx
{
    public ProjectorViewModel ViewModel
    {
        get;
    }

    public ProjectorWindow()
    {
        ViewModel = App.GetService<ProjectorViewModel>();

        InitializeComponent();

        Content = null;

        this.Activated += ProjectorWindow_Activated;

        this.SizeChanged += ProjectorWindow_SizeChanged;
    }

    private void ProjectorWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        var width = args.Size.Width;
        var height = args.Size.Height;
        var dpi = this.GetDpiForWindow();

        var desiredSize = new Windows.Foundation.Size(((float)2560 * 96.0f / dpi), ((float)1440 * 96.0f / dpi));

        //Debug.WriteLine($"Width: {width} | Height: {height} | DPI: {dpi} | DS: {desiredSize.Width} | DS: {desiredSize.Height}");
    }

    private async void ProjectorWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        await ViewModel.InitializeAsync();
    }
}
