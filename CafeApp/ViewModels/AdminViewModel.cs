using System;
using System.Collections.Generic;
using System.Linq;
using CafeApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.ViewModels;

public partial class AdminViewModel : ViewModelBase
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    [ObservableProperty] private List<User> _usersList = [];
    [ObservableProperty] private List<Shift> _shiftsList = [];
    [ObservableProperty] private List<Table> _tablesList = [];
    [ObservableProperty] private List<Order> _ordersList = [];
    
    [ObservableProperty] private User? _selectedUser;
    [ObservableProperty] private Shift? _selectedShift;
    [ObservableProperty] private Table? _selectedTable;
    [ObservableProperty] private Order? _selectedOrder;

    [RelayCommand]
    private void LoadUsers()
    {
        UsersList = _db.Users
            .Include(x => x.Role)
            .ToList();
    }

    [RelayCommand]
    private void LoadShifts(CafeDbContext db) =>
        ShiftsList = db.Shifts
            .Include(x => x.Users)
            .Include(x => x.Orders)
            .Include(x => x.WaiterTables)
            .ToList();
    
    [RelayCommand]
    private void LoadTables(CafeDbContext db) => TablesList = db.Tables.ToList();
    
    [RelayCommand]
    private void LoadOrders(CafeDbContext db) =>
        OrdersList = db.Orders
            .Include(x => x.Shift)
            .Include(x => x.Table)
            .OrderBy(x => x.Shift)
            .ToList();
    
    [ObservableProperty] private bool _isEnabledUserEditBtn = false;
    [ObservableProperty] private bool _isEnabledShiftEditBtn = false;
    [ObservableProperty] private bool _isEnabledUserDeleteBtn = false;
    [ObservableProperty] private bool _isEnabledShiftDeleteBtn = false;
    [ObservableProperty] private bool _isEnabledDeleteTableBtn = false;
    [ObservableProperty] private bool _isEnabledEditOrderBtn = false;
    
    [RelayCommand]
    private void SetIsEnabledUserEditBtn(bool newValue) => IsEnabledUserEditBtn = newValue;
    [RelayCommand]
    private void SetIsEnabledShiftEditBtn(bool newValue) => IsEnabledUserEditBtn = newValue;
    [RelayCommand]
    private void SetIsEnabledUserDeleteBtn(bool newValue) => IsEnabledUserEditBtn = newValue;
    [RelayCommand]
    private void SetIsEnabledShiftDeleteBtn(bool newValue) => IsEnabledUserEditBtn = newValue;
    [RelayCommand]
    private void SetIsEnabledDeleteTableBtn(bool newValue) => IsEnabledUserEditBtn = newValue;
    [RelayCommand]
    private void SetIsEnabledEditOrderBtn(bool newValue) => IsEnabledUserEditBtn = newValue;
    
    
    [ObservableProperty] private string _error = String.Empty;
    [ObservableProperty] private bool _isVisibleError = false;
    
    [RelayCommand]
    private void ShowMessage(string message)
    {
        Error = message;
        IsVisibleError = true;
    }
}