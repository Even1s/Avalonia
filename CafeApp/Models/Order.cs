using System;
using System.Collections.Generic;

namespace CafeApp.Models;

public class Order
{
    public int Id { get; set; }

    public int ShiftId { get; set; }

    public int TableId { get; set; }
    
    public string? Status { get; set; }
    
    public int ClientsNumber { get; set; }

    public string? Dishes { get; set; }

    public decimal? Payment { get; set; }
    
    public string? PaymentMethod { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CloseAt { get; set; }
    

    public virtual Shift? Shift { get; set; }

    public virtual Table? Table { get; set; }
}