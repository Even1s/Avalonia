using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CafeApp.Helpers;
using CafeApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.ViewModels;

public partial class UserEditViewModel : ViewModelBase
{
    [ObservableProperty] private string _login = String.Empty;
    [ObservableProperty] private string _password = String.Empty;
    [ObservableProperty] private string _firstName = String.Empty;
    [ObservableProperty] private string _lastName = String.Empty;
    [ObservableProperty] private string _patronymic = String.Empty;
    [ObservableProperty] private DateTimeOffset _birthday = new DateTimeOffset(DateTime.Now);
    [ObservableProperty] private Role? _role;
    
    [ObservableProperty] private byte[]? _photo;
    [ObservableProperty] private Bitmap? _photoSource;
    [ObservableProperty] private byte[]? _contract;
    [ObservableProperty] private Bitmap? _contractSource;
    
    public List<Role> RolesList { get; set; } = [];
    
    [ObservableProperty] private string _error = String.Empty;
    [ObservableProperty] private bool _isVisibleError = false;
    
    [RelayCommand]
    private void ShowMessage(string message)
    {
        Error = message;
        IsVisibleError = true;
    }

    [RelayCommand]
    private async Task AddPhoto(IStorageFile file)
    {
        Photo = await file.ReadAllBytesAsync();
        PhotoSource = Photo.ToBitmap();
    }
    [RelayCommand]
    private async Task AddContract(IStorageFile file)
    {
        Contract = await file.ReadAllBytesAsync();
        ContractSource = Contract.ToBitmap();
    }
    [RelayCommand]
    private void GetRoles(DbSet<Role> roles)
    {
        Role = roles.FirstOrDefault(x => x.Name == Roles.WAITER_ROLE);
        Console.WriteLine(Role?.Name);
        RolesList = roles.ToList();
        Console.WriteLine(RolesList.Count);
    }
    
    [RelayCommand]
    private void SetUser(User editUser)
    {
        Login = editUser.Login;
        Password = String.Empty;
        FirstName = editUser.FirstName;
        LastName = editUser.LastName;
        Patronymic = editUser.Patronymic ?? String.Empty;
        Birthday = new DateTimeOffset(editUser.Birthday.ToDateTime(new TimeOnly(0)), TimeSpan.Zero);
        Role = editUser.Role;

        Photo = editUser.Photo;
        PhotoSource = editUser.Photo!.ToBitmap();
        Contract = editUser.Contract;
        ContractSource = editUser.Contract!.ToBitmap();
    }
}