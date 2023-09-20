using System.Text.RegularExpressions;
using Windows.Devices.Display;
using Windows.Graphics;
using Windows.Graphics.Display;

namespace Mirar.Models;

public static class DisplayExtensions
{
    public static string NullOrEmptyNA(this string? str)
    {
        return string.IsNullOrEmpty(str) ? "-NA-" : str;
    }
}

public class DisplayModel
{
    public string? DeviceId;
    public string? DeviceName;
    public string? DeviceType;
    public string? DeviceModel;
    public string? DeviceInterface;
    public string? DeviceVendorId;
    public SizeInt32 Resolution;

    public DisplayModel(DisplayMonitor? displayMonitor = null)
    {
        if(displayMonitor == null) return;

        DeviceId = displayMonitor.DeviceId;
        DeviceName = displayMonitor.DisplayName;
        Resolution = displayMonitor.NativeResolutionInRawPixels;

        ProcessDevicePath(displayMonitor.DeviceId, out DeviceType, out DeviceModel);
        ProcessDevicePath(displayMonitor.DisplayAdapterDeviceId, out DeviceInterface, out DeviceVendorId);
    }

    private Task ProcessDevicePath(string path, out string? aGroup, out string? bGroup)
    {
        aGroup = null;
        bGroup = null;

        if (path == null) return Task.CompletedTask;

        Regex regex = new Regex(@"\\\?\\(.*?)#(.*?)\#");
        Match match = regex.Match(path);

        if (!match.Success) throw new Exception($"No matches found!");

        if (match.Groups.Count != 3) throw new Exception($"Three Groups were expected -> Got: {match.Groups.Count}");
        {
            aGroup = match.Groups[1].Value;
            bGroup = match.Groups[2].Value;
        }

        return Task.CompletedTask;
    }

    // override ToString
    public override string ToString()
    {
        return $"Name: {DeviceName.NullOrEmptyNA()} | Model: {DeviceModel.NullOrEmptyNA()} | Type: {DeviceType.NullOrEmptyNA()} | Interface: {DeviceInterface.NullOrEmptyNA()} | Resolution: {Resolution.Width}x{Resolution.Height}";
    }
}
