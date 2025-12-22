using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CafeApp.Helpers;
using CafeApp.Models;
using CafeApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.Views.Admin;

public partial class UserCreateView : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    private readonly ComboBox _roleCBox;
    
    public UserCreateView()
    {
        InitializeComponent();
        _roleCBox = this.FindControl<ComboBox>("RoleCBox")!;
    }
    
    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        _roleCBox!.ItemsSource = _db.Roles.ToList();
        _roleCBox.SelectedItem = _db.Roles.FirstOrDefault(x => x.Name == CafeApp.Helpers.Roles.WAITER_ROLE);
    }

    private async void UserPhotoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = await GetImageAsync();
        if (file == null) return;
        
        var ucvm = DataContext as UserCreateViewModel;
        ucvm?.AddPhotoCommand.Execute(file);
    }

    private async void ContractPhotoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = await GetImageAsync();
        if (file == null) return;
        
        var ucvm = DataContext as UserCreateViewModel;
        ucvm?.AddContractCommand.Execute(file);
    }
    
    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var ucvm = DataContext as UserCreateViewModel;

        User saveUser = new User
        {
            Login = ucvm.Login,
            PasswordCrypt = PasswordCrypt.CryptPassword(ucvm.Password),
            FirstName = ucvm.FirstName,
            LastName = ucvm.LastName,
            Patronymic = ucvm.Patronymic,
            Birthday = DateOnly.FromDateTime(ucvm.Birthday.DateTime),
            Role = ucvm.Role,
            Status = UserStatuses.USER_WORKING,
            Photo = ucvm.Photo,
            Contract = ucvm.Contract
        };
        
        if (!string.IsNullOrWhiteSpace(saveUser.Login) &&
            !string.IsNullOrWhiteSpace(saveUser.PasswordCrypt) &&
            !string.IsNullOrWhiteSpace(saveUser.FirstName) &&
            !string.IsNullOrWhiteSpace(saveUser.LastName) &&
            saveUser.Role != null &&
            saveUser.Photo != null &&
            saveUser.Contract != null)
        {
            User creatingUser = new User
            {
                Login = ucvm.Login,
                PasswordCrypt = PasswordCrypt.CryptPassword(ucvm.Password),
                FirstName = ucvm.FirstName,
                LastName = ucvm.LastName,
                Patronymic = ucvm.Patronymic,
                Birthday = DateOnly.FromDateTime(ucvm.Birthday.DateTime),
                Role = _roleCBox.SelectedItem as Role,
                Status = UserStatuses.USER_WORKING,
                Photo = ucvm.Photo,
                Contract = ucvm.Contract
            };
            
            _db.Users.Add(creatingUser);
            _db.SaveChanges();
            Close();
        }
        else
        {
            ucvm?.ShowMessageCommand.Execute("Не все поля заполнены");
        }
    }
    
    private void BackBtn_OnClick(object? sender, RoutedEventArgs e) => Close();

    private async Task<IStorageFile?> GetImageAsync()
    {
        var topLevel = GetTopLevel(this);
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Выберите фото", AllowMultiple = false, FileTypeFilter = [FilePickerFileTypes.ImageAll] });

        return files.FirstOrDefault();
    }
}