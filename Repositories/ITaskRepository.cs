namespace TaskAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Models.Task>> GetAllTasksAsync();
        Task<Models.Task> GetTaskByIdAsync(int id);
        Task<Models.Task> CreateTaskAsync(Models.Task task);
        Task<Models.Task> UpdateTaskAsync(Models.Task task);
        Task<bool> DeleteTaskAsync(int id);
    }
}
