using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;

namespace Mirar.Contracts.Services;

public interface IThemeSelectorService
{
    ElementTheme Theme
    {
        get;
    }

    ObservableCollection<ElementTheme> ThemeList
    {
        get;
    }

    Task InitializeAsync();

    Task SetThemeAsync(ElementTheme theme);

    Task SetRequestedThemeAsync();
}
