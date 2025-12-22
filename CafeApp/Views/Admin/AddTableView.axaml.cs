using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CafeApp.Models;
using CafeApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CafeApp.Views.Admin;

public partial class AddTableView : Window
{
    private readonly CafeDbContext _db = App.Current.Services.GetRequiredService<CafeDbContext>();
    
    public AddTableView()
    {
        InitializeComponent();
    }

    private void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var allTables = _db.Tables.ToList();
        var atvm = DataContext as AddTableViewModel;

        int number;
        try
        {
            number = Convert.ToInt32(atvm?.TablesNumber);
        }
        catch (FormatException)
        {
            atvm?.ShowMessageCommand.Execute("Неправильный формат ввода");
            return;
        }

        if (allTables.Any(x => x.Number == number))
        {
            atvm?.ShowMessageCommand.Execute("Такой столик уже существует");
            return;
        }
        
        _db.Add(new Table { Number = number });
        _db.SaveChanges();
        
        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e) => Close();
}