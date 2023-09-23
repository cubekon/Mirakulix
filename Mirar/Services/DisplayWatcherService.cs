using System.Diagnostics;
using Mirar.Contracts.Services;
using Mirar.Helpers;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace Mirar.Services;
public class DisplayWatcherService : IDisplayWatcherService, IDisposable
{
    public DeviceWatcher _watcher;

    public event TypedEventHandler<object, DisplayMonitor>? DisplayAdaptersChanged;

    // Prevent displaying notification on first run / init
    private bool _initialized = false;

    private int _deviceReferenceCounter = 0;
    private int _deviceCounter = 0;

    public DisplayWatcherService()
    {
        _watcher = DeviceInformation.CreateWatcher(DisplayMonitor.GetDeviceSelector());
    }

    public async Task InitializeAsync()
    {
        var deviceSelector = DisplayMonitor.GetDeviceSelector();
        var test = await DeviceInformation.FindAllAsync(deviceSelector);

        _deviceReferenceCounter = test.Count;

        _watcher.EnumerationCompleted += Watcher_EnumerationCompleted;
        _watcher.Added += Watcher_Added;
        _watcher.Removed += Watcher_Removed;

        await StartWatcherAsync();
    }

    public async Task StartWatcherAsync()
    {
        if (_watcher is null) throw new Exception("Device Watcher is null!");
        if (_watcher.Status == DeviceWatcherStatus.Started) return;
        _watcher.Start();
        await Task.CompletedTask;
    }

    public async Task StopWatcherAsync()
    {
        if (_watcher is null) throw new Exception("Device Watcher is null!");
        if (_watcher.Status == DeviceWatcherStatus.Stopped) return;
        _watcher.Stop();
        await Task.CompletedTask;
    }

    private void Watcher_EnumerationCompleted(DeviceWatcher sender, object args)
    {
        // Handle EnumerationCompleted event here
        Debug.WriteLine("DisplayWatcher: EnumerationCompleted");
    }

    private async void Watcher_Added(DeviceWatcher sender, DeviceInformation args)
    {
        if (_deviceCounter < _deviceReferenceCounter)
        {
            _deviceCounter++;
        }
        else if (!_initialized) _initialized = true;


        DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(args.Id);
        if (_initialized) ShowChangedDisplayNotification(displayMonitor);
        // add displayMonitor to a collection here
        Debug.WriteLine($"DisplayWatcher: DisplayMonitor Added: {displayMonitor.DisplayName} ({displayMonitor.NativeResolutionInRawPixels.Width}x{displayMonitor.NativeResolutionInRawPixels.Height})");
    }

    private async void Watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
    {
        if (_deviceCounter >= _deviceReferenceCounter && !_initialized)
        {
            _initialized = true;
        }

        try
        {
            DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(args.Id);
            if (_initialized) ShowChangedDisplayNotification(displayMonitor);
            // remove displayMonitor from collection here
            Debug.WriteLine($"DisplayWatcher: DisplayMonitor Removed: {displayMonitor.DisplayName} ({displayMonitor.NativeResolutionInRawPixels.Width}x{displayMonitor.NativeResolutionInRawPixels.Height})");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.Message}");
        }
    }

    private void ShowChangedDisplayNotification(DisplayMonitor changedMonitor)
    {
        var notificationPayload = $"{ResourceExtensions.GetLocalized("DisplayAdapterUpdatedNotification")}";
        App.GetService<IAppNotificationService>().Show(notificationPayload);

        DisplayAdaptersChanged?.Invoke(this, changedMonitor);
    }

    public async void Dispose()
    {
        await StopWatcherAsync();
        Debug.WriteLine("DisplayWatcher -> Disposed!");
    }
}
