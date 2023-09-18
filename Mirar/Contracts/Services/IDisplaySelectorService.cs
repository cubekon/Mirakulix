using Windows.Devices.Display;
using Windows.Devices.Display.Core;
using Windows.Foundation;

namespace Mirar.Contracts.Services;
public interface IDisplaySelectorService
{
    DisplayMonitor CurrentDisplay
    {
        get;
    }

    event TypedEventHandler<object, DisplayMonitor> DisplayAdapterChanged;

    Task InitializeAsync();
    Task SetDisplayAsync(DisplayMonitor display);
}
