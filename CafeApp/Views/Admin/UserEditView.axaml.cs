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

public partial class UserEditView : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    private readonly ComboBox _roleCBox;
    private readonly User _editUser;
    
    public UserEditView(User editUser)
    {
        InitializeComponent();
        _roleCBox = this.FindControl<ComboBox>("RoleCBox")!;
        _editUser = editUser;
    }
    
    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        _roleCBox!.ItemsSource = _db.Roles.ToList();
        _roleCBox.SelectedItem = _db.Roles.FirstOrDefault(x => x.Name == CafeApp.Helpers.Roles.WAITER_ROLE);
        var uevm = DataContext as UserEditViewModel;
        uevm.SetUserCommand.Execute(_editUser);
    }

    private async void UserPhotoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = await GetImageAsync();
        if (file == null) return;
        
        var uevm = DataContext as UserEditViewModel;
        uevm?.AddPhotoCommand.Execute(file);
    }

    private async void ContractPhotoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = await GetImageAsync();
        if (file == null) return;
        
        var uevm = DataContext as UserEditViewModel;
        uevm?.AddContractCommand.Execute(file);
    }
    
    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var uevm = DataContext as UserEditViewModel;

        User saveUser = new User
        {
            Login = uevm.Login,
            PasswordCrypt = !string.IsNullOrWhiteSpace(uevm.Password)
                ? PasswordCrypt.CryptPassword(uevm.Password)
                : _editUser.PasswordCrypt,
            FirstName = uevm.FirstName,
            LastName = uevm.LastName,
            Patronymic = uevm.Patronymic,
            Birthday = DateOnly.FromDateTime(uevm.Birthday.DateTime),
            Role = uevm.Role,
            Status = UserStatuses.USER_WORKING,
            Photo = uevm.Photo,
            Contract = uevm.Contract
        };
        
        if (!string.IsNullOrWhiteSpace(saveUser.Login) &&
            !string.IsNullOrWhiteSpace(saveUser.FirstName) &&
            !string.IsNullOrWhiteSpace(saveUser.LastName) &&
            saveUser.Role != null &&
            saveUser.Photo != null &&
            saveUser.Contract != null)
        {
            _db.Users.Update(saveUser);
            _db.SaveChanges();
            Close();
        }
        else
        {
            uevm?.ShowMessageCommand.Execute("Не все поля заполнены");
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