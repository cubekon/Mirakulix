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
using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;

namespace Mirar.Services;
public class DisplaySelectorService : IDisplaySelectorService, IDisposable
{
    private const string SettingsKey = "ProjectorDisplay";

    public DisplayModel CurrentDisplay { get; set; } = new DisplayModel();

    public ObservableCollection<DisplayModel> AvailableDisplays { get; set; } = new ObservableCollection<DisplayModel>();

    public event TypedEventHandler<object, DisplayMonitor>? DisplayAdapterChanged;

    public event TypedEventHandler<object, DisplayMonitor>? ProjectorDisplayChanged;

    private readonly ILocalSettingsService _localSettingsService;

    private readonly IDisplayWatcherService _displayWatcherService;

    public DisplaySelectorService(ILocalSettingsService localSettingsService, IDisplayWatcherService displayWatcherService)
    {
        _localSettingsService = localSettingsService;
        _displayWatcherService = displayWatcherService;
        _displayWatcherService.DisplayAdapterChanged += OnDisplayAdapterChanged;
    }

    private async Task GetDisplaysAsync()
    {
        
        AvailableDisplays.Clear();

        var deviceSelector = DisplayMonitor.GetDeviceSelector();
        var displays = await DeviceInformation.FindAllAsync(deviceSelector);

        foreach (DeviceInformation displayInfo in displays)
        {
            DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(displayInfo.Id);

            DisplayModel displayModel = new(displayMonitor);

            AvailableDisplays.Add(displayModel);
        }

        // prevent losing display selection in settings
        await RestorePreviousDisplayAsync();

        await Task.CompletedTask;
    }

    public async Task UpdateAvailableDisplaysAsync()
    {
        await GetDisplaysAsync();
    }

    private void OnDisplayAdapterChanged(object sender, DisplayMonitor dm)
    {
        Debug.WriteLine("DisplaySelectorService: DisplayAdapterChanged triggered");
        DisplayAdapterChanged?.Invoke(sender, dm);
    }

    private async Task RestorePreviousDisplayAsync()
    {
        // Load Previous Display Setting
        DisplayModel? loadedDisplay = await LoadDisplayFromSettingsAsync();

        if (AvailableDisplays.Count < 1) return;
        if (loadedDisplay == null) return;


        var lastDisplay = AvailableDisplays.Where(x => x.DeviceId == loadedDisplay.DeviceId).FirstOrDefault();
        if (lastDisplay == null) return;

        await SetDisplayAsync(lastDisplay);
    }

    public async Task InitializeAsync()
    {
        await GetDisplaysAsync();

        await RestorePreviousDisplayAsync();

        await Task.CompletedTask;
    }

    public async Task SetDisplayAsync(DisplayModel display)
    {
        CurrentDisplay = display;

        DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(display.DeviceId);
        ProjectorDisplayChanged?.Invoke(this, displayMonitor);

        await Task.CompletedTask;
    }

    private async Task<DisplayModel?> LoadDisplayFromSettingsAsync()
    {
        var displayIdFromSettings = await _localSettingsService.ReadSettingAsync<string>(SettingsKey);

        if (displayIdFromSettings == null) return null;

        try
        {
            DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(displayIdFromSettings);
            return new DisplayModel(displayMonitor);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }

    }

    public async Task SaveDisplayInSettingsAsync(DisplayModel displayToSave)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKey, displayToSave.DeviceId);
    }

    public void Dispose()
    {
        Console.WriteLine("DisplaySelectorService Dispose");
    }
}
