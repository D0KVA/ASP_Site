using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyAspShop.Models;

public partial class MyShopContext : DbContext
{
    public MyShopContext()
    {
    }

    public MyShopContext(DbContextOptions<MyShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PositionOrder> PositionOrders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DOKVA\\SQLEXPRESS;Initial Catalog=MyShop;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC074724870D");

            entity.Property(e => e.IdProduct).HasColumnName("ID_Product");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItems__ID_Pr__5070F446");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("PK__Orders__EC9FA9553B898772");

            entity.Property(e => e.IdOrder).HasColumnName("ID_Order");
            entity.Property(e => e.CommentOrder).HasColumnType("text");
            entity.Property(e => e.SumOrder).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UsersId).HasColumnName("Users_ID");

            entity.HasOne(d => d.Users).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Users_ID__44FF419A");
        });

        modelBuilder.Entity<PositionOrder>(entity =>
        {
            entity.HasKey(e => e.IdPosOrder).HasName("PK__Position__D77BC1CB4471DE98");

            entity.Property(e => e.IdPosOrder).HasColumnName("ID_PosOrder");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.PositionOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PositionO__Order__47DBAE45");

            entity.HasOne(d => d.Product).WithMany(p => p.PositionOrders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PositionO__Produ__48CFD27E");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Products__522DE496034154F6");

            entity.HasIndex(e => e.NameProduct, "UQ__Products__DF0C7103121CF0FD").IsUnique();

            entity.Property(e => e.IdProduct).HasColumnName("ID_Product");
            entity.Property(e => e.DescriptionProduct).HasColumnType("text");
            entity.Property(e => e.ImageProduct).HasColumnType("text");
            entity.Property(e => e.NameProduct)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PriceProduct).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductType_ID");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("FK__Products__Produc__4222D4EF");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.IdProductType).HasName("PK__ProductT__BF692758463102F9");

            entity.Property(e => e.IdProductType).HasColumnName("ID_ProductType");
            entity.Property(e => e.NameType)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.IdReview).HasName("PK__Reviews__E39E9647ABB9520B");

            entity.Property(e => e.IdReview).HasColumnName("ID_Review");
            entity.Property(e => e.CommentReview).HasColumnType("text");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.UsersId).HasColumnName("Users_ID");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Product__4D94879B");

            entity.HasOne(d => d.Users).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Users_I__4CA06362");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK__Roles__43DCD32D5515F640");

            entity.HasIndex(e => e.NameRole, "UQ__Roles__7EF6AFD64DBC4704").IsUnique();

            entity.Property(e => e.IdRole).HasColumnName("ID_Role");
            entity.Property(e => e.NameRole)
                .HasMaxLength(45)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__ED4DE44249F9C413");

            entity.HasIndex(e => e.LoginUser, "UQ__Users__4156FB31D4077A34").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053431E6B89F").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("ID_User");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.LoginUser)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.PasswordUser)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__Role_ID__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
