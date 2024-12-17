using System;
using System.Collections.Generic;

namespace MyAspShop.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string NameProduct { get; set; } = null!;

    public decimal PriceProduct { get; set; }

    public int? ProductTypeId { get; set; }

    public string? DescriptionProduct { get; set; }

    public string? ImageProduct { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<PositionOrder> PositionOrders { get; set; } = new List<PositionOrder>();

    public virtual ProductType? ProductType { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
