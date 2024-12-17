using System;
using System.Collections.Generic;

namespace MyAspShop.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int IdProduct { get; set; }

    public string? UserId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;
}
