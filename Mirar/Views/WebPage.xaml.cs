using Microsoft.UI.Xaml.Controls;

using Mirar.ViewModels;

namespace Mirar.Views;

// To learn more about WebView2, see https://docs.microsoft.com/microsoft-edge/webview2/.
public sealed partial class WebPage : Page
{
    public WebViewModel ViewModel
    {
        get;
    }

    public WebPage()
    {
        ViewModel = App.GetService<WebViewModel>();
        InitializeComponent();

        ViewModel.WebViewService.Initialize(WebView);
    }
}
