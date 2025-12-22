using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CafeApp.ViewModels;

public partial class AddTableViewModel : ViewModelBase
{
    [ObservableProperty] private string _tablesNumber = String.Empty;
    [ObservableProperty] private string _errorText = String.Empty;
    [ObservableProperty] private bool _isVisibleError = false;
    
    [RelayCommand]
    private void ShowMessage(string message)
    {
        ErrorText = message;
        IsVisibleError = true;
    }
}