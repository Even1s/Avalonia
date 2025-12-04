using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CafeApp;

public partial class MainWindow : Window
{
    private readonly TextBox _loginTBox;
    private readonly TextBox _passwordTBox;
    private readonly TextBlock _errorTBlock;
    
    public MainWindow()
    {
        InitializeComponent();
        
        _loginTBox = this.FindControl<TextBox>("LoginTBox")!;
        _passwordTBox = this.FindControl<TextBox>("PasswordTBox")!;
        _errorTBlock = this.FindControl<TextBlock>("ErrorTBlock")!;
    }

    private void AuthFetch(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_loginTBox.Text) || string.IsNullOrWhiteSpace(_passwordTBox.Text))
        {
            _errorTBlock.Text = "Введите Логин и Пароль";
            _errorTBlock.IsVisible = true;
            return;
        }
        _errorTBlock.IsVisible = false;
        
    }
}