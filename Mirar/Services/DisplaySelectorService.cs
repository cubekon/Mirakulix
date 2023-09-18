using Mirar.Contracts.Services;
using Mirar.Models;
using Windows.UI.ViewManagement;
using Windows.Devices.Display;
using Windows.Devices.Display.Core;
using Windows.Media.Protection.PlayReady;
using Windows.Foundation;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing;
using Windows.Devices.Enumeration;
using System.Diagnostics;
using Mirar.Helpers;
using CommunityToolkit.WinUI;

namespace Mirar.Services;
public class DisplaySelectorService : IDisplaySelectorService, IDisposable
{
    private const string SettingsKey = "ProjectorDisplay";

    public DisplayMonitor? CurrentDisplay { get; set; } = null;

    public event TypedEventHandler<object, DisplayMonitor>? DisplayAdapterChanged;

    private readonly ILocalSettingsService _localSettingsService;

    private readonly IDisplayWatcherService _displayWatcherService;

    public DisplaySelectorService(ILocalSettingsService localSettingsService, IDisplayWatcherService displayWatcherService)
    {
        _localSettingsService = localSettingsService;
        _displayWatcherService = displayWatcherService;
        _displayWatcherService.DisplayAdapterChanged += OnDisplayAdapterChanged;
    }

    private void OnDisplayAdapterChanged(object sender, DisplayMonitor dm)
    {
        Debug.WriteLine("DisplaySelectorService: DisplayAdapterChanged triggered");
        DisplayAdapterChanged?.Invoke(sender, dm);
    }

    public async Task InitializeAsync()
    {
        CurrentDisplay = await LoadDisplayFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetDisplayAsync(DisplayMonitor display)
    {
        CurrentDisplay = display;
        await SaveDisplayInSettingsAsync(display);
    }

    private async Task<DisplayMonitor> LoadDisplayFromSettingsAsync()
    {
        var displayIdFromSettings = await _localSettingsService.ReadSettingAsync<string>(SettingsKey);

        // TODO: Restore DisplayMonitor Instance from displayIdFromSettings

        return DisplayMonitor.FromInterfaceIdAsync(displayIdFromSettings).GetResults();
    }

    private async Task SaveDisplayInSettingsAsync(DisplayMonitor displayToSave)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKey, displayToSave.DisplayAdapterDeviceId);
    }

    public void Dispose()
    {
        Console.WriteLine("DisplaySelectorService Dispose");
    }
}
