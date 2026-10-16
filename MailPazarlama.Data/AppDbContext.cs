using MailPazarlama.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailPazarlama.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<MailTemplate> MailTemplates { get; set; }
    public DbSet<MailSetting> MailSettings { get; set; }
    public DbSet<MailSendLog> MailSendLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tablo İsimleri
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Subscriber>().ToTable("Subscribers");
        modelBuilder.Entity<MailTemplate>().ToTable("MailTemplates");
        modelBuilder.Entity<MailSetting>().ToTable("MailSettings");
        modelBuilder.Entity<MailSendLog>().ToTable("MailSendLogs");

        // Unique Indexler
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Subscriber>()
            .HasIndex(s => s.Email)
            .IsUnique();

        // MailSendLogs İlişkileri ve DeleteBehavior.Restrict
        modelBuilder.Entity<MailSendLog>(entity =>
        {
            entity.HasOne(m => m.Subscriber)
                .WithMany(s => s.MailSendLogs)
                .HasForeignKey(m => m.SubscriberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.MailTemplate)
                .WithMany(t => t.MailSendLogs)
                .HasForeignKey(m => m.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.User)
                .WithMany(u => u.MailSendLogs)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
