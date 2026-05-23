namespace UserService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id);

            b.Property(u => u.Id).HasColumnName("id");
            b.Property(u => u.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            b.Property(u => u.CreatedAt).HasColumnName("created_at");

            // Value Object mapeado como owned (coluna "email" na mesma tabela)
            b.OwnsOne(u => u.Email, vo =>
            {
                vo.Property(e => e.Value)
                    .HasColumnName("email")
                    .HasMaxLength(320)
                    .IsRequired();

                vo.HasIndex(e => e.Value).IsUnique();
            });
        });
    }
}
