using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Graphics.Display;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Mirar.Contracts.Services;
using Mirar.Views.Projector;
using Newtonsoft.Json.Linq;
using Windows.Devices.Display;
using Windows.Devices.Display.Core;
using WindowsDisplayAPI;
using WinUIEx;

namespace Mirar.ViewModels;
public class ProjectorViewModel : ObservableRecipient
{
    private readonly IDisplaySelectorService _displaySelectorService;
    private WindowEx? _projectorWindow;


    public ProjectorViewModel(IDisplaySelectorService displaySelectorService)
    {
        _displaySelectorService = displaySelectorService;
        _displaySelectorService.ProjectorDisplayChanged += OnProjectorDisplayChanged;
    }

    public async Task InitializeAsync()
    {
        _projectorWindow = App.ProjectorWindow;
        await Task.CompletedTask;
    }

    private void OnProjectorDisplayChanged(object sender, DisplayMonitor display)
    {
        if (_projectorWindow == null) return;

        _projectorWindow.Restore();

        IReadOnlyList<DisplayArea> areas = DisplayArea.FindAll();

        //using (var mgr = DisplayManager.Create(DisplayManagerOptions.None))
        //{
        //    mgr.TryAcquireTarget(DisplayTarget.)
        //}

        for (int i = 0; i < areas.Count; i++)
        {
            var area = areas[i];

            var test = Win32Interop.GetMonitorFromDisplayId(area.DisplayId);

            DisplayInformation display1 = DisplayInformation.CreateForDisplayId(area.DisplayId);

            var displayInfo = DisplayInformation.CreateForDisplayId(area.DisplayId);

            Debug.WriteLine(area.ToString());
        }

        foreach (Display display2 in Display.GetDisplays())
        {
            var DisplayName = display2.DisplayName;
            string Friendly = Regex.Replace(display2.DisplayName, @"[^A-Za+Z0-9 ]", "");
            string Resolution = display2.CurrentSetting.Resolution.Width.ToString() + " x " + display2.CurrentSetting.Resolution.Height.ToString();
            string Position = display2.CurrentSetting.Position.X.ToString() + " x " + display2.CurrentSetting.Position.Y.ToString();
        }

        // Random area
        Random random = new Random();
        var areaNr = random.Next(areas.Count);

        // TODO: Set ProjectorWindow to selected Output Projector Display
        float displayWidth = display.NativeResolutionInRawPixels.Width;
        float displayHeight = display.NativeResolutionInRawPixels.Height;

        var dpi = _projectorWindow.GetDpiForWindow();

        var desiredSize = new Windows.Foundation.Size((displayWidth * 96.0f / dpi), (displayHeight * 96.0f / dpi));

        _projectorWindow.MoveAndResize(areas[areaNr].WorkArea.X, areas[areaNr].WorkArea.Y, desiredSize.Width, desiredSize.Height);

        _projectorWindow.Maximize();

        Debug.WriteLine($"ProjectorDisplay Changed: {display.DeviceId}");
        Debug.WriteLine($"Width: {displayWidth} | Height: {displayHeight} | DPI: {dpi} | DS: {desiredSize.Width} | DS: {desiredSize.Height}");
    }
}
