using Cirs.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Cirs.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Table and column names are spelled out so they match docs/database/schema.sql exactly.
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(u => u.Role).HasColumnName("role").HasMaxLength(20).IsRequired();
            entity.Property(u => u.IsActive).HasColumnName("is_active");
            entity.Property(u => u.FailedLoginCount).HasColumnName("failed_login_count");
            entity.Property(u => u.LockoutEnd).HasColumnName("lockout_end").HasColumnType("datetime");
            entity.Property(u => u.LastLoginAt).HasColumnName("last_login_at").HasColumnType("datetime");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at").HasColumnType("datetime");

            entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("uq_users_email");
        });
    }
}
