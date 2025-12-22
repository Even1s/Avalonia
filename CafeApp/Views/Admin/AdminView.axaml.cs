using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CafeApp.AdminWindows;
using CafeApp.Helpers;
using CafeApp.Models;
using CafeApp.ViewModels;
using CafeApp.WaiterWindows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.Views.Admin;

public partial class AdminView : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public AdminView()
    {
        InitializeComponent();
    }
    
    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        avm?.LoadUsersCommand.Execute(_db);
        avm?.LoadShiftsCommand.Execute(_db);
        avm?.LoadTablesCommand.Execute(_db);
        avm?.LoadOrdersCommand.Execute(_db);
    }

    private void LogOutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        App.CurrentUser = null;
        App.CurrentShift = null;
        new MainView { DataContext = new MainViewModel() }.Show();
        Close();
    }

    private void UsersDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        avm?.SetIsEnabledUserEditBtnCommand.Execute(avm.SelectedUser != null);
        avm?.SetIsEnabledUserDeleteBtnCommand.Execute(avm.SelectedUser != null);
    }
    
    private async void UserDeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        if (avm?.SelectedUser == null)
            return;
        
        _db.Users.Remove(avm.SelectedUser);
        await _db.SaveChangesAsync();
        avm.LoadUsersCommand.Execute(_db);
    }

    private async void UserEditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        if (avm?.SelectedUser != null)
            await new UserEditView(avm.SelectedUser) { DataContext = new UserEditViewModel() }.ShowDialog(this);
    }
    
    private void AddUserBtn_OnClick(object? sender, RoutedEventArgs e) => 
        new UserCreateView { DataContext = new UserCreateViewModel() }.ShowDialog(this);

    private void ShiftsDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        avm?.SetIsEnabledShiftEditBtnCommand.Execute(avm.SelectedOrder != null);
        avm?.SetIsEnabledShiftDeleteBtnCommand.Execute(avm.SelectedOrder != null);
    }
    
    private async void ShiftDeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        if (avm?.SelectedShift == null)
            return;
        
        _db.Shifts.Remove(avm.SelectedShift);
        await _db.SaveChangesAsync();
        avm.LoadShiftsCommand.Execute(_db);
    }

    private async void ShiftEditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        if (avm?.SelectedShift != null)
            await new ShiftEditWindow(avm.SelectedShift).ShowDialog(this);
    }
    private void AddShiftBtn_OnClick(object? sender, RoutedEventArgs e) => new ShiftEditWindow().ShowDialog(this);

    private void TablesDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        avm?.SetIsEnabledDeleteTableBtnCommand.Execute(avm.SelectedTable != null);
    }

    private async void DeleteTableBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        if (avm?.SelectedTable == null)
            return;
        
        _db.Tables.Remove(avm.SelectedTable);
        await _db.SaveChangesAsync();
        avm.LoadTablesCommand.Execute(_db);
    }

    private void AddTableBtn_OnClick(object? sender, RoutedEventArgs e) =>
        new AddTableView { DataContext = new AddTableViewModel() }.ShowDialog(this);

    private void OrdersDataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        avm?.SetIsEnabledEditOrderBtnCommand.Execute(avm?.SelectedOrder != null && avm.SelectedOrder.Status != OrderStatuses.PAID);
    }
    
    private void RefreshUsersBtn_OnClick(object? sender, RoutedEventArgs e) => 
        (DataContext as AdminViewModel)?.LoadUsersCommand.Execute(_db);

    private void RefreshShiftsBtn_OnClick(object? sender, RoutedEventArgs e) => 
        (DataContext as AdminViewModel)?.LoadShiftsCommand.Execute(_db);
    
    private void RefreshTablesBtn_OnClick(object? sender, RoutedEventArgs e) => 
        (DataContext as AdminViewModel)?.LoadTablesCommand.Execute(_db);
    
    private void RefreshOrdersBtn_OnClick(object? sender, RoutedEventArgs e) => 
        (DataContext as AdminViewModel)?.LoadOrdersCommand.Execute(_db);
    
    private void EditOrderBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var avm = DataContext as AdminViewModel;
        if (avm?.SelectedOrder?.Shift != null)
            new OrderEditWindow(avm.SelectedOrder, avm.SelectedOrder.Shift).ShowDialog(this);
    }
    

    private async void AllOrdersReportXlsxBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var orders = GetOrders();

        var pathToSave = await GetPathToSaveAsync(App.CurrentShift);

        if (pathToSave != null)
            await ReportFactory.MakeReport(orders, pathToSave);
    }

    private async void AllOrdersReportPdfBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var orders = GetOrders();

        var pathToSavePdf = await GetPathToSaveAsync(App.CurrentShift, pdf: true);
        var pathToSaveExcel = pathToSavePdf?.Replace("pdf", "xlsx");

        if (pathToSaveExcel != null)
            await ReportFactory.MakeReport(orders, pathToSaveExcel, isPdf: true);
    }

    private async void PaidOrdersReportXlsxBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var orders = GetOrders(paid: true);

        var pathToSave = await GetPathToSaveAsync(App.CurrentShift);

        if (pathToSave != null)
            await ReportFactory.MakeReport(orders, pathToSave);
    }

    private async void PaidOrdersReportPdfBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var orders = GetOrders(paid: true);

        var pathToSavePdf = await GetPathToSaveAsync(App.CurrentShift, pdf: true);
        var pathToSaveExcel = pathToSavePdf?.Replace("pdf", "xlsx");

        if (pathToSaveExcel != null)
            await ReportFactory.MakeReport(orders, pathToSaveExcel, isPdf: true);
    }

    private List<Order> GetOrders(bool paid = false)
    {
        var orders = _db.Orders
            .Include(x => x.Shift)
            .Include(x => x.Table)
            .Where(x => x.Shift == App.CurrentShift)
            .ToList();
        
        if (paid)
            orders = orders.Where(x => x.Status == OrderStatuses.PAID).ToList();

        return orders;
    }
    
    private async Task<string?> GetPathToSaveAsync(Shift? shift, bool pdf = false)
    {
        if (shift == null)
        {
            var avm = DataContext as AdminViewModel;
            avm?.ShowMessageCommand.Execute("Смена отсутствует");
            return null;
        }
        
        var type = pdf ? "pdf" : "xlsx";
        var filesType = pdf ? "Pdf Files" : "Excel Files";
        var patterns = pdf ? "*.pdf" : "*.xlsx";
        
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранить отчет",
            SuggestedFileName = $"Отчет_№{shift.Id}_{DateTime.Now.ToLocalTime().ToString("dd_MM_yyyy")}.{type}",
            FileTypeChoices = new[] { new FilePickerFileType(filesType) { Patterns = [patterns] } }
        });
        
        return file?.TryGetLocalPath();
    }
}