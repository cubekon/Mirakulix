using Microsoft.UI.Xaml.Controls;

using Mirar.ViewModels;

namespace Mirar.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class TablePage : Page
{
    public TableViewModel ViewModel
    {
        get;
    }

    public TablePage()
    {
        ViewModel = App.GetService<TableViewModel>();
        InitializeComponent();
    }
}
