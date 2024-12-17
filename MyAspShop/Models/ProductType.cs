using System;
using System.Collections.Generic;

namespace MyAspShop.Models;

public partial class ProductType
{
    public int IdProductType { get; set; }

    public string? NameType { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
