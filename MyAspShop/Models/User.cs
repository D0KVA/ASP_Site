using System;
using System.Collections.Generic;

namespace MyAspShop.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string LoginUser { get; set; } = null!;

    public string PasswordUser { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? RoleId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }
}
