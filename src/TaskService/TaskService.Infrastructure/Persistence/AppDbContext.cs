namespace TaskService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Task> Tasks => Set<Task>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Task>(b =>
        {
            b.ToTable("tasks");
            b.HasKey(t => t.Id);

            b.Property(t => t.Id).HasColumnName("id");
            b.Property(t => t.UserId).HasColumnName("user_id");
            b.Property(t => t.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            b.Property(t => t.Description).HasColumnName("description");

            b.Property(t => t.Status).HasColumnName("status")
                .HasConversion<string>();   // persiste como texto: "Pending", "Completed"

            b.Property(t => t.CreatedAt).HasColumnName("created_at");
            b.Property(t => t.CompletedAt).HasColumnName("completed_at");

            b.HasIndex(t => t.UserId);

            // EF Core não persiste domain events — são transitórios
            b.Ignore(t => t.DomainEvents);
        });
    }
}
