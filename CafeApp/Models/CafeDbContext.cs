using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.Models;

public class CafeDbContext : DbContext
{
    public CafeDbContext() { }

    public CafeDbContext(DbContextOptions<CafeDbContext> options) : base(options) { }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<User> Users { get; set; }
    
    public virtual DbSet<WaiterTable> WaiterTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString =
            "Host=localhost;Port=5432;Database=cafe;Username=postgres;Password=post98";
        
        optionsBuilder.UseNpgsql(connectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pk");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("order_id");
            entity.Property(e => e.ClientsNumber)
                .HasDefaultValue(1)
                .HasColumnName("clients_number");
            entity.Property(e => e.CloseAt).HasColumnName("close_at");
            entity.Property(e => e.Dishes)
                .HasColumnName("dishes")
                .HasMaxLength(1024);
            entity.Property(e => e.OpenAt).HasColumnName("open_at");
            entity.Property(e => e.Payment)
                .HasColumnName("payment")
                .HasColumnType("money");
            entity.Property(e => e.ShiftId).HasColumnName("shift_id");
            entity.Property(e => e.Status)
                .HasMaxLength(16)
                .HasColumnName("status");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(48)
                .HasColumnName("payment_method");

            entity.HasOne(d => d.Shift).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("orders_shifts_fk");
            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("orders_tables_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pk");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shifts_pkey");

            entity.ToTable("shifts");

            entity.Property(e => e.Id).HasColumnName("shift_id");
            entity.Property(e => e.CloseAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("close_at");
            entity.Property(e => e.OpenAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("open_at");

            entity.HasMany(d => d.Users).WithMany(p => p.Shifts)
                .UsingEntity<Dictionary<string, object>>(
                    "ShiftEmployee",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("shift_employees_users_fk"),
                    l => l.HasOne<Shift>().WithMany()
                        .HasForeignKey("ShiftId")
                        .HasConstraintName("shift_employees_shifts_fk"),
                    j =>
                    {
                        j.HasKey("ShiftId", "UserId").HasName("shift_employees_pk");
                        j.ToTable("shift_employees");
                        j.IndexerProperty<int>("ShiftId").HasColumnName("shift_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Id).HasColumnName("table_id");
            entity.Property(e => e.Number).HasColumnName("table_number");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("user_id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.Contract).HasColumnName("contract");
            entity.Property(e => e.FirstName)
                .HasMaxLength(48)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(48)
                .HasColumnName("last_name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(48)
                .HasColumnName("patronymic");
            entity.Property(e => e.Login)
                .HasMaxLength(16)
                .HasColumnName("login");
            entity.Property(e => e.PasswordCrypt)
                .HasMaxLength(48)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasMaxLength(16)
                .HasColumnName("status");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_roles_fk");
        });

        modelBuilder.Entity<WaiterTable>(entity =>
        {
            entity.HasKey(e => new { e.ShiftId, e.UserId, e.TableId }).HasName("waiter_tables_pk");

            entity.ToTable("waiter_tables");

            entity.Property(e => e.ShiftId).HasColumnName("shift_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");

            entity.HasOne(d => d.Shift).WithMany(p => p.WaiterTables)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("waiter_table_shifts_fk");

            entity.HasOne(d => d.Table).WithMany(p => p.WaiterTables)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("waiter_table_tables_fk");

            entity.HasOne(d => d.User).WithMany(p => p.WaiterTables)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("waiter_table_users_fk");
        });
    }
}