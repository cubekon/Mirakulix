using Mirar.Contracts.Services;
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
using WindowsDisplayAPI;
using Newtonsoft.Json.Linq;

namespace Mirar.Services;
public class DisplayService : IDisplayService, IDisposable
{
    private const string SettingsKey = "ProjectorDisplay";

    // Services
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IDisplayWatcherService _displayWatcherService;

    public Display? SavedDisplay { get; private set; }
    public Display? ActiveDisplay { get; private set; }

    public List<Display> AvailableDisplays { get; private set; } = new List<Display>();

    // Events
    public event TypedEventHandler<object, Display?>? ActiveDisplayChanged;
    public event TypedEventHandler<object, List<Display>>? DisplayAdaptersChanged;

    public DisplayService(ILocalSettingsService localSettingsService, IDisplayWatcherService displayWatcherService)
    {
        _localSettingsService = localSettingsService;
        _displayWatcherService = displayWatcherService;
        _displayWatcherService.DisplayAdaptersChanged += OnDisplayAdapterChanged;
    }

    public async Task InitializeAsync()
    {
        await GetDisplaysAsync(useCache: false);
        await RestoreSavedDisplayAsync();
        await Task.CompletedTask;
    }

    private async void OnDisplayAdapterChanged(object sender, DisplayMonitor displayMonitor)
    {
        Debug.WriteLine("DisplayService: DisplayAdaptersChanged triggered");

        // refresh AvailableDisplays
        await GetDisplaysAsync(useCache: false);

        // in case of disconnect, check if ActiveDisplay was disconnected
        // if so, set ActiveDisplay to null
        Display? changedDisplay = await GetDisplayByPathAsync(displayMonitor.DeviceId);

        if (changedDisplay == null) Debug.WriteLine("DisplayService: A DisplayMonitor was disconnected");

        if (changedDisplay == null && displayMonitor.DeviceId == ActiveDisplay?.DevicePath)
        {
            // Active Display was disconnected
            await SetActiveDisplayAsync(null);
        }

        // reselect SavedDisplay
        if (changedDisplay != null && displayMonitor.DeviceId == SavedDisplay?.DevicePath)
        {
            await SetActiveDisplayAsync(changedDisplay);
        }

        // notify listeners
        DisplayAdaptersChanged?.Invoke(sender, AvailableDisplays);
    }

    public async Task<List<Display>> GetDisplaysAsync(bool useCache = true)
    {
        if (useCache && AvailableDisplays.Count > 0) return AvailableDisplays;

        AvailableDisplays.Clear();

        await Task.Run(() =>
        {
            foreach (Display display in Display.GetDisplays())
            {
                AvailableDisplays.Add(display);
            }
        });

        return AvailableDisplays;
    }

    public async Task SetActiveDisplayAsync(Display? display)
    {
        if(ActiveDisplay == display) return;

        ActiveDisplay = display;

        // Save Display
        await SaveDisplayInSettingsAsync(display);

        ActiveDisplayChanged?.Invoke(this, display);

        await Task.CompletedTask;
    }

    private async Task RestoreSavedDisplayAsync()
    {
        // Load Previous Display Setting
        Display? loadedDisplay = await LoadDisplayFromSettingsAsync();

        if (AvailableDisplays.Count < 1) return;

        if (loadedDisplay == null) return;

        var lastDisplay = AvailableDisplays.Where(x => x.DevicePath == loadedDisplay.DevicePath).FirstOrDefault();
        if (lastDisplay == null) return;

        await SetActiveDisplayAsync(lastDisplay);
        await Task.CompletedTask;
    }

    private async Task<Display?> LoadDisplayFromSettingsAsync(bool useCache = true)
    {
        if (useCache && SavedDisplay != null) return SavedDisplay;

        var savedDisplayPath = await _localSettingsService.ReadSettingAsync<string>(SettingsKey);

        if (savedDisplayPath == null) return null;

        Display? savedDisplay = await GetDisplayByPathAsync(savedDisplayPath);

        return savedDisplay;
    }

    public async Task<Display?> GetDisplayByPathAsync(string displayPath)
    {
        return await Task.Run(async () =>
        {
            foreach (var display in await GetDisplaysAsync())
            {
                if (display.DevicePath == displayPath)
                {
                    return display;
                }
            }

            return null;
        });
    }

    private async Task SaveDisplayInSettingsAsync(Display? displayToSave)
    {
        if (displayToSave == null || SavedDisplay == displayToSave) return;

        await _localSettingsService.SaveSettingAsync(SettingsKey, displayToSave.DevicePath);

        SavedDisplay = displayToSave;

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Console.WriteLine("DisplayService Dispose");
    }
}
