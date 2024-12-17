using System;
using System.Collections.Generic;

namespace MyAspShop.Models;

public partial class PositionOrder
{
    public int IdPosOrder { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
