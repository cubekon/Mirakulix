using Microsoft.UI.Windowing;

namespace Mirar.Views.Projector;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProjectorWindow : WindowEx
{
    public ProjectorWindow()
    {
        InitializeComponent();

        Content = null;
    }
}
