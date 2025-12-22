using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.CookWindows;
using CafeApp.Helpers;
using CafeApp.Models;
using CafeApp.ViewModels;
using CafeApp.Views.Admin;
using CafeApp.WaiterWindows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp;

public partial class MainView : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    public MainView()
    {
        InitializeComponent();
    }
    
    private void AuthBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var mvm = DataContext as MainViewModel;

        if (string.IsNullOrWhiteSpace(mvm?.Login) || string.IsNullOrWhiteSpace(mvm.Password))
        {
            mvm?.ShowMessageCommand.Execute("Поля логина или пароля пустые");
            return;
        }
        
        var user = _db.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Login == mvm.Login);
        var passwordIsValid = PasswordCrypt.IsValid(mvm.Password, user?.PasswordCrypt);
        
        if (user == null || !passwordIsValid)
        {
            mvm.ShowMessageCommand.Execute("Введенные логин или пароль неверны");
            return;
        }

        if (user.Status == UserStatuses.USER_FIRED)
        {
            mvm.ShowMessageCommand.Execute("Вы уволены");
            return;
        }
        
        var shift = _db.Shifts
            .Include(x => x.Users)
            .Include(x => x.WaiterTables)
            .FirstOrDefault(x => 
                x.OpenAt <= DateTime.Now.ToLocalTime() && 
                x.CloseAt >= DateTime.Now.ToLocalTime() && 
                (x.Users.Contains(user) || user.Role.Name == Roles.ADMIN_ROLE));
        
        if (shift != null)
            App.CurrentShift = shift;
        App.CurrentUser = user;
        
        switch (user.Role.Name)
        {
            case Roles.ADMIN_ROLE:
                new AdminView { DataContext = new AdminViewModel() }.Show();
                break;
            case Roles.COOK_ROLE:
                if (shift == null)
                {
                    mvm.ShowMessageCommand.Execute("У вас не назначена смена");
                    return;
                }
                new CookWindow().Show();
                break;
            case Roles.WAITER_ROLE:
                if (shift == null)
                {
                    mvm.ShowMessageCommand.Execute("У вас не назначена смена");
                    return;
                }
                new WaiterWindow().Show();
                break;
        }
        Close();
    }
}