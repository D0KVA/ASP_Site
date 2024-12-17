using System;
using System.Collections.Generic;

namespace MyAspShop.Models;

public partial class Review
{
    public int IdReview { get; set; }

    public int? Rating { get; set; }

    public string? CommentReview { get; set; }

    public int UsersId { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User Users { get; set; } = null!;
}
