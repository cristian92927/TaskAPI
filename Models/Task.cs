using System.ComponentModel.DataAnnotations;
using System;

namespace TaskAPI.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public TaskStatus Status { get; set; }

        // Cambio para asegurar que siempre sea UTC
        private DateTime _createdAt = DateTime.UtcNow;

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }

    public enum TaskStatus
    {
        Pending,
        Completed
    }
}

