using TaskAPI.Repositories;

namespace TaskAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<Models.Task>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllTasksAsync();
        }

        public async Task<Models.Task> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetTaskByIdAsync(id);
        }

        public async Task<Models.Task> CreateTaskAsync(Models.Task task)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("El título no puede estar vacío");

            // Asignar fecha de creación en UTC (importante para PostgreSQL)
            task.CreatedAt = DateTime.UtcNow;

            return await _taskRepository.CreateTaskAsync(task);
        }

        public async Task<Models.Task> UpdateTaskAsync(int id, Models.Task task)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("El título no puede estar vacío");

            // Verificar si la tarea existe
            var existingTask = await _taskRepository.GetTaskByIdAsync(id);
            if (existingTask == null)
                throw new KeyNotFoundException($"Tarea con ID {id} no encontrada");

            // Actualizar propiedades
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            // No actualizamos CreatedAt, se mantiene el original

            return await _taskRepository.UpdateTaskAsync(existingTask);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            // Verificar si la tarea existe
            var existingTask = await _taskRepository.GetTaskByIdAsync(id);
            if (existingTask == null)
                throw new KeyNotFoundException($"Tarea con ID {id} no encontrada");

            return await _taskRepository.DeleteTaskAsync(id);
        }
    }
}
