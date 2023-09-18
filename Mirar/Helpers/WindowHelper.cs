namespace Mirar.Helpers;

public static class WindowHelper
{
    public static void CloseProjectorWindow()
    {
        if (App.ProjectorWindow is not null)
        {
            App.ProjectorWindow.Close();
        }
    }

    public static void CloseMainWindow()
    {
        if (App.MainWindow is not null)
        {
            App.MainWindow.Close();
        }
    }
}
