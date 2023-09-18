using Windows.Graphics;

namespace Mirar.Models;
public class Display
{
    public string? DeviceName;
    public SizeInt32 Resolution;

    // override ToString
    public override string ToString()
    {
        return $"{DeviceName} ({Resolution.Width}x{Resolution.Height})";
    }
}
