namespace TaskService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;
using TaskService.Domain.Outbox;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

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

        mb.Entity<OutboxMessage>(b =>
                {
                    b.ToTable("outbox_messages");
                    b.HasKey(o => o.Id);
                    b.Property(o => o.Id).HasColumnName("id");
                    b.Property(o => o.Type).HasColumnName("type").HasMaxLength(200).IsRequired();
                    b.Property(o => o.Payload).HasColumnName("payload").IsRequired();
                    b.Property(o => o.CreatedAt).HasColumnName("created_at");
                    b.Property(o => o.ProcessedAt).HasColumnName("processed_at");
                    b.HasIndex(o => o.ProcessedAt);  // índice para o polling ser eficiente
                });
    }
}
