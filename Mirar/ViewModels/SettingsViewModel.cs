using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mirar.Contracts.Services;
using Mirar.Helpers;
using Mirar.Views.Projector;
using Windows.ApplicationModel;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;
using WindowsDisplayAPI;

namespace Mirar.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    private readonly IDisplayService _displayService;

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
    private Display? _selectedDisplay;

    public ObservableCollection<Display> AvailableDisplays { get; private set; } = new ObservableCollection<Display>();

    // Theme
    [ObservableProperty]
    private string? _selectedTheme;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    public ObservableCollection<ElementTheme> AvailableThemes { get; private set; } = new ObservableCollection<ElementTheme>();

    // Version Description
    [ObservableProperty]
    private string _versionDescription;

    // ----- Properties -----

    [ObservableProperty]
    private bool _isInDemoMode = App.ProjectorWindow.ViewModel.ContentFrame is DemoFrame;

    public SettingsViewModel(IThemeSelectorService themeSelectorService, IDisplayService displayService)
    {
        // Projector Display
        _displayService = displayService;
        _displayService.DisplayAdaptersChanged += OnDisplayAdaptersChanged;
        _displayService.ActiveDisplayChanged += OnActiveDisplayChanged;

        // Theme
        _themeSelectorService = themeSelectorService;
        AvailableThemes = _themeSelectorService.ThemeList;
        _elementTheme = _themeSelectorService.Theme;

        // Version Description
        _versionDescription = GetVersionDescription();
    }
    public async Task InitializeDisplaysAsync()
    {
        var displays = await _displayService.GetDisplaysAsync();

        foreach (var display in displays)
        {
            AvailableDisplays.Add(display);
        }

        SelectedDisplay = SelectedDisplay = AvailableDisplays.Where(d => d.DevicePath == _displayService.ActiveDisplay?.DevicePath).FirstOrDefault();

        await UpdateDisplayHeader();
    }

    // ----- Relay Commands -----

    [RelayCommand]
    private void ActivateDemo()
    {
        _isInDemoMode = !_isInDemoMode;

        if (_isInDemoMode)
        {
            App.ProjectorWindow.ViewModel.ContentFrame = App.GetService<DemoFrame>();
        }
        else
        {
            App.ProjectorWindow.ViewModel.ContentFrame = App.GetService<PictureFrame>();
        }   
    }

    public async void Display_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;

        Display newSelection = (Display)comboBox.SelectedValue;

        if (newSelection == null) return;

        if (SelectedDisplay != newSelection)
        {
            SelectedDisplay = newSelection;
            await UpdateDisplayHeader();

            await _displayService.SetActiveDisplayAsync(newSelection);
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

    // Future functionality: if display gets changed elsewhere, update the UI
    private async void OnActiveDisplayChanged(object sender, Display? display)
    {
        await Task.Run(() =>
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                if (SelectedDisplay != display) 
                {
                    SelectedDisplay = AvailableDisplays.Where(d => d.DevicePath == _displayService.ActiveDisplay?.DevicePath).FirstOrDefault();
                    // Update Header
                    await UpdateDisplayHeader();
                }
            });
        });
    }

    private async void OnDisplayAdaptersChanged(object sender, List<Display> availableDisplays)
    {
        await Task.Run(() =>
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                AvailableDisplays.Clear();


                foreach (var display in availableDisplays)
                {
                    AvailableDisplays.Add(display);
                }

                SelectedDisplay = AvailableDisplays.Where(d => d.DevicePath == _displayService.ActiveDisplay?.DevicePath).FirstOrDefault();
            });
        });
    }

    private Task UpdateDisplayHeader()
    {
        Display? display = SelectedDisplay;

        if (display == null)
        {
            DisplayName = "No Display Selected";
            DisplayOutputDevice = string.Empty;
            DisplayResolution = string.Empty;
            DisplayFrequency = string.Empty;
        }
        else
        {
            DisplayName = display.DeviceName;
            DisplayOutputDevice = $"Output Device: {display.Adapter.DeviceName}";
            DisplayResolution = $"Resolution: {display.CurrentSetting.Resolution.Width} x {display.CurrentSetting.Resolution.Height}";
            DisplayFrequency = $"Frequency: {display.CurrentSetting.Frequency} Hz";
        }

        return Task.CompletedTask;
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
