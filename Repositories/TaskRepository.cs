using Microsoft.EntityFrameworkCore;
using TaskAPI.Data;
using TaskAPI.Models;

namespace TaskAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly Data.ApplicationDbContext _context;

        public TaskRepository(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Task>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<Models.Task> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<Models.Task> CreateTaskAsync(Models.Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Models.Task> UpdateTaskAsync(Models.Task task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}