using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Mirar.Contracts.Services;
using WindowsDisplayAPI;

namespace Mirar.ViewModels;
public partial class ProjectorViewModel : ObservableRecipient
{
    private readonly IDisplayService _displayService;
    private WindowEx? _projectorWindow;

    [ObservableProperty]
    private UIElement? _contentFrame;

    public ProjectorViewModel(IDisplayService displayService)
    {
        _displayService = displayService;
        _displayService.ActiveDisplayChanged += OnActiveDisplayChanged;
    }

    public async Task InitializeAsync()
    {
        _projectorWindow = App.ProjectorWindow;

        await UpdateDisplayPosition(_displayService.ActiveDisplay);

        await Task.CompletedTask;
    }

    private async Task UpdateDisplayPosition(Display? display)
    {
        if (display == null) return;

        if (_projectorWindow == null) return;

        _projectorWindow.Restore();

        var displayPosX = display.CurrentSetting.Position.X;
        var displayPosY = display.CurrentSetting.Position.Y;

        var displayWidth = 100;
        var displayHeight = 100;

        _projectorWindow.MoveAndResize(displayPosX, displayPosY, displayWidth, displayHeight);

        _projectorWindow.Maximize();

        await Task.CompletedTask;
    }

    private async void OnActiveDisplayChanged(object sender, Display? display)
    {
        await Task.Run(() =>
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                await UpdateDisplayPosition(display);
            });
        });
    }
}
