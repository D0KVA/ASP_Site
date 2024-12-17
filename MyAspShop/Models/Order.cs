using System;
using System.Collections.Generic;

namespace MyAspShop.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public DateOnly DateOrder { get; set; }

    public decimal SumOrder { get; set; }

    public string? CommentOrder { get; set; }

    public int UsersId { get; set; }

    public virtual ICollection<PositionOrder> PositionOrders { get; set; } = new List<PositionOrder>();

    public virtual User Users { get; set; } = null!;
}
