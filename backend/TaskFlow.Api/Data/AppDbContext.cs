using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();

                // Add unique constraint on email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Task entity
            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Priority).HasConversion<int>();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                // Configure relationship with User
                entity
                    .HasOne(t => t.Owner)
                    .WithMany()
                    .HasForeignKey(t => t.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TaskAssignment entity - FIXED CASCADE ISSUE
            modelBuilder.Entity<TaskAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Role).HasConversion<int>();
                entity.Property(e => e.AssignedAt).IsRequired();

                // Configure relationships - FIXED: Use NO ACTION for User relationships
                entity
                    .HasOne(ta => ta.Task)
                    .WithMany()
                    .HasForeignKey(ta => ta.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(ta => ta.User)
                    .WithMany()
                    .HasForeignKey(ta => ta.UserId)
                    .OnDelete(DeleteBehavior.NoAction); // CHANGED: No cascade for User

                entity
                    .HasOne(ta => ta.AssignedBy)
                    .WithMany()
                    .HasForeignKey(ta => ta.AssignedByUserId)
                    .OnDelete(DeleteBehavior.NoAction); // CHANGED: No cascade for AssignedBy

                // Unique constraint: User can only be assigned to a task once
                entity.HasIndex(ta => new { ta.TaskId, ta.UserId }).IsUnique();
            });
        }
    }
}
