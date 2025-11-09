using System.Windows;
using System.Windows.Controls;

namespace StudentsManager_EF_Demo.Desktop.Components;

public partial class InputComponent : UserControl
{
    public static readonly DependencyProperty LabelProperty = 
        DependencyProperty.Register(nameof(Label), typeof(object), typeof(InputComponent));
    public object Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    
    public static readonly DependencyProperty ValueProperty = 
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(InputComponent));
    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public static readonly DependencyProperty IsReadOnlyProperty = 
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(InputComponent));
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }
    
    public InputComponent()
    {
        InitializeComponent();
    }
}