using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CafeApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _login = String.Empty;
}