using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TaskAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración adicional para la entidad Task
            modelBuilder.Entity<Task>()
                .Property(t => t.Title)
                .IsRequired();

            modelBuilder.Entity<Task>()
                .Property(t => t.Status)
                .IsRequired();
        }
    }
}