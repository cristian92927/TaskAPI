using Microsoft.EntityFrameworkCore;
using TaskAPI.Models;

namespace TaskAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
       
            // Inicializador estático
            static ApplicationDbContext()
            {
                // Esto permite usar DateTimeKind.Local con PostgreSQL
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            }

            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Models.Task> Tasks { get; set; }
            public DbSet<User> Users { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            { 
            base.OnModelCreating(modelBuilder);

            // Ajustar nombres de tabla para PostgreSQL (sensible a mayúsculas/minúsculas)
            modelBuilder.Entity<Models.Task>().ToTable("Tasks");
            modelBuilder.Entity<User>().ToTable("Users");

            // Configuración para la entidad Task
            modelBuilder.Entity<Models.Task>()
                .Property(t => t.Title)
                .IsRequired();

            modelBuilder.Entity<Models.Task>()
                .Property(t => t.Status)
                .IsRequired();

            // Configuración para la entidad User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}