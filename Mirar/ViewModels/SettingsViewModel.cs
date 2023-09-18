using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Mirar.Contracts.Services;
using Mirar.Helpers;
using Mirar.Models;
using Windows.ApplicationModel;
using Windows.Devices.Display;
using Windows.Devices.Display.Core;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

namespace Mirar.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    private readonly IDisplaySelectorService _displaySelectorService;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty]
    private DisplayMonitor? _selectedDisplay;

    [ObservableProperty]
    private string? _selectedTheme;

    public ObservableCollection<ElementTheme> AvailableThemes { get; private set; } = new ObservableCollection<ElementTheme> ();

    public ObservableCollection<DisplayMonitor> AvailableDisplays { get; private set; } = new ObservableCollection<DisplayMonitor>();

    public SettingsViewModel(IThemeSelectorService themeSelectorService, IDisplaySelectorService displaySelectorService)
    {
        // Theme
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        GetAllThemes();

        // Display
        _displaySelectorService = displaySelectorService;
        _displaySelectorService.DisplayAdapterChanged += OnDisplayAdapterChanged;
        _selectedDisplay = _displaySelectorService.CurrentDisplay;
        GetAllDisplays();

        // Description
        _versionDescription = GetVersionDescription();
    }

    private void OnDisplayAdapterChanged(object sender, DisplayMonitor dm)
    {
        // TODO: Enhance addition / deletion of single DisplayMonitor
        GetAllDisplays();
    }

    private void GetAllDisplays()
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            AvailableDisplays.Clear();

            var deviceSelector = DisplayMonitor.GetDeviceSelector();
            var displays = await DeviceInformation.FindAllAsync(deviceSelector);

            foreach (DeviceInformation displayInfo in displays)
            {
                DisplayMonitor displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(displayInfo.Id);

                AvailableDisplays.Add(displayMonitor);
            }
        });
    }

    private void GetAllThemes()
    {
        AvailableThemes.Clear();

        foreach(ElementTheme theme in Enum.GetValues(typeof(ElementTheme)))
        {
            AvailableThemes.Add(theme);
        }
    }

    public async void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox cb = (ComboBox)sender;

        var newThemeString = (string)cb.SelectedValue;

        EnumToStringConverter enumToString = new EnumToStringConverter();

        ElementTheme newTheme = (ElementTheme)enumToString.ConvertBack(newThemeString, typeof(string), string.Empty, string.Empty);

        if (ElementTheme != newTheme)
        {
            ElementTheme = newTheme;
            await _themeSelectorService.SetThemeAsync(newTheme);
        }
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
