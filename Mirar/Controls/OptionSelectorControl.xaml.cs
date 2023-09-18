using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mirar.Controls;
public sealed partial class OptionSelectorControl : UserControl
{
    public OptionSelectorControl()
    {
        InitializeComponent();
    }

    public IconElement? Icon
    {
        get => GetValue(IconProperty) as IconElement;
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(OptionSelectorControl), new PropertyMetadata(null));

    public string? Label
    {
        get => GetValue(LabelProperty) as string;
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(OptionSelectorControl), new PropertyMetadata(null));

    public string? Description
    {
        get => GetValue(DescriptionProperty) as string;
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(OptionSelectorControl), new PropertyMetadata(null));


    public object? Container
    {
        get => GetValue(OptionContainerProperty);
        set => SetValue(OptionContainerProperty, value);
    }

    public static readonly DependencyProperty OptionContainerProperty = DependencyProperty.Register("Container", typeof(object), typeof(OptionSelectorControl), new PropertyMetadata(null));
}
