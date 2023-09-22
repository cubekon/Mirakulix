using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mirar.Contracts.Services;
using Mirar.Helpers;
using Mirar.Models;
using Windows.ApplicationModel;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;

namespace Mirar.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    private readonly IDisplaySelectorService _displaySelectorService;

    // ----- Header -----

    [ObservableProperty]
    private string displayName = string.Empty;

    [ObservableProperty]
    private string displayOutputDevice = string.Empty;

    [ObservableProperty]
    private string displayResolution = string.Empty;

    [ObservableProperty]
    private string displayFrequency = string.Empty;


    // ----- OptionSelectors -----

    // Projector Display
    [ObservableProperty]
    private DisplayModel? _selectedDisplay;

    public ObservableCollection<DisplayModel> AvailableDisplays { get; private set; } = new ObservableCollection<DisplayModel>();

    // Theme
    [ObservableProperty]
    private string? _selectedTheme;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    public ObservableCollection<ElementTheme> AvailableThemes { get; private set; } = new ObservableCollection<ElementTheme>();

    // Version Description
    [ObservableProperty]
    private string _versionDescription;


    public SettingsViewModel(IThemeSelectorService themeSelectorService, IDisplaySelectorService displaySelectorService)
    {
        // Projector Display
        _displaySelectorService = displaySelectorService;
        AvailableDisplays = _displaySelectorService.AvailableDisplays;
        _selectedDisplay = _displaySelectorService.CurrentDisplay;
        _displaySelectorService.DisplayAdapterChanged += OnDisplayAdapterChanged;

        // Header
        // -- references Projector Display --

        // Theme
        _themeSelectorService = themeSelectorService;
        AvailableThemes = _themeSelectorService.ThemeList;
        _elementTheme = _themeSelectorService.Theme;

        // Version Description
        _versionDescription = GetVersionDescription();
    }

    private void OnDisplayAdapterChanged(object sender, DisplayMonitor dm)
    {
        // TODO: Enhance addition / deletion of single DisplayMonitor

        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            await _displaySelectorService.UpdateAvailableDisplaysAsync();
            SelectedDisplay = _displaySelectorService.CurrentDisplay;
        });
        
    }

    public async void Display_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;

        DisplayModel newSelection = (DisplayModel)comboBox.SelectedValue;

        if(newSelection == null) return;

        if(SelectedDisplay != newSelection)
        {
            SelectedDisplay = newSelection;
            await _displaySelectorService.SetDisplayAsync(newSelection);
            await _displaySelectorService.SaveDisplayInSettingsAsync(newSelection);
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
