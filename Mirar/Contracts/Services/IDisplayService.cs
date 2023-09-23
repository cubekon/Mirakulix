using System.Collections.ObjectModel;
using Windows.Foundation;
using WindowsDisplayAPI;

namespace Mirar.Contracts.Services;
public interface IDisplayService
{
    // Summary:
    //     Gets the active display.
    Display? ActiveDisplay { get; }

    // Summary:
    //     Gets the display loaded from settings.
    Display? SavedDisplay { get; }

    // Summary:
    //     Gets all connected displays.
    List<Display> AvailableDisplays { get; }

    // Summary:
    //     Occurs when the list of available displays has changed.
    event TypedEventHandler<object, List<Display>> DisplayAdaptersChanged;

    // Summary:
    //     Occurs when the active display has changed.
    event TypedEventHandler<object, Display?> ActiveDisplayChanged;

    // Summary:
    //     Initializes the display service.
    Task InitializeAsync();
    
    // Summary:
    //     Sets the active display.
    Task SetActiveDisplayAsync(Display? display);

    // Summary:
    //     Gets the list of available displays.
    // Parameters:
    //   useCache:
    //     If true, the cached list of displays will be returned if available.
    Task<List<Display>> GetDisplaysAsync(bool useCache = true);

    // Summary:
    //     Gets the display by display path.
    Task<Display?> GetDisplayByPathAsync(string displayPath);
}
