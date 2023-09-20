using System.Collections.ObjectModel;
using Mirar.Models;
using Windows.Devices.Display;
using Windows.Devices.Display.Core;
using Windows.Foundation;

namespace Mirar.Contracts.Services;
public interface IDisplaySelectorService
{
    DisplayModel CurrentDisplay
    {
        get;
    }

    ObservableCollection<DisplayModel> AvailableDisplays
    {
        get;
    }

    event TypedEventHandler<object, DisplayMonitor> DisplayAdapterChanged;

    Task InitializeAsync();
    Task SetDisplayAsync(DisplayModel display);

    Task UpdateAvailableDisplaysAsync();
}
