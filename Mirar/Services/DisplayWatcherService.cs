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

    public event TypedEventHandler<object, DisplayMonitor>? DisplayAdapterChanged;

    // Prevent displaying notification on first run / init
    private bool _initialized = false;

    public DisplayWatcherService()
    {
        _watcher = DeviceInformation.CreateWatcher(DisplayMonitor.GetDeviceSelector());
        _watcher.EnumerationCompleted += Watcher_EnumerationCompleted;
        _watcher.Added += Watcher_Added;
        _watcher.Removed += Watcher_Removed;
        _watcher.Start();
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
        if(_watcher is null) throw new Exception("Device Watcher is null!");
        if (_watcher.Status == DeviceWatcherStatus.Stopped) return;
        _watcher.Stop();
        await Task.CompletedTask;
    }

    private void Watcher_EnumerationCompleted(DeviceWatcher sender, object args)
    {
        // Handle EnumerationCompleted event here
        if(!_initialized) _initialized = true;

        Debug.WriteLine("DisplayWatcher: EnumerationCompleted");
    }

    private async void Watcher_Added(DeviceWatcher sender, DeviceInformation args)
    {
        DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(args.Id);
        if (_initialized) ShowChangedDisplayNotification(displayMonitor);
        // add displayMonitor to a collection here
        Debug.WriteLine($"DisplayWatcher: DisplayMonitor Added: {displayMonitor.DisplayName} ({displayMonitor.NativeResolutionInRawPixels.Width}x{displayMonitor.NativeResolutionInRawPixels.Height})");
    }

    private async void Watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
    {
        DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(args.Id);
        if(_initialized) ShowChangedDisplayNotification(displayMonitor);
        // remove displayMonitor from collection here
        Debug.WriteLine($"DisplayWatcher: DisplayMonitor Removed: {displayMonitor.DisplayName} ({displayMonitor.NativeResolutionInRawPixels.Width}x{displayMonitor.NativeResolutionInRawPixels.Height})");
    }

    private void ShowChangedDisplayNotification(DisplayMonitor changedMonitor)
    {
        var notificationPayload = $"{ResourceExtensions.GetLocalized("DisplayAdapterUpdatedNotification")}";
        App.GetService<IAppNotificationService>().Show(notificationPayload);

        DisplayAdapterChanged?.Invoke(this, changedMonitor);
    }

    public async void Dispose()
    {
        await StopWatcherAsync();
        Debug.WriteLine("DisplayWatcher -> Disposed!");
    }
}
