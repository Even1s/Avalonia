using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CafeApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _login = String.Empty;
    [ObservableProperty] private string _password = String.Empty;

    [ObservableProperty] private string _error = String.Empty;
    [ObservableProperty] private bool _isVisible = false;
    
    [RelayCommand]
    private void ShowMessage(string message)
    {
        Error = message;
        IsVisible = true;
    }
}