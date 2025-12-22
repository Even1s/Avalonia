using System;
using System.Collections.Generic;

namespace CafeApp.Models;

public class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordCrypt { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly Birthday { get; set; }

    public string Status { get; set; } = null!;

    public byte[]? Photo { get; set; }

    public byte[]? Contract { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<WaiterTable> WaiterTables { get; set; } = new List<WaiterTable>();
    
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
