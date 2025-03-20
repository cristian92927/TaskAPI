using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskAPI.Services;
using TaskAPI.Models;
using System.Security.Claims;


namespace TaskAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }


        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            return Ok(new { message = "API de tareas funcionando correctamente" });
        }


        [HttpGet("auth-info")]
        [Authorize] 
        public IActionResult GetAuthInfo()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var claims = User.Claims.Select(c => new { type = c.Type, value = c.Value }).ToList();

            return Ok(new
            {
                message = "Información de autenticación",
                username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                authHeader = authHeader,
                claims = claims,
                isAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }


        // GET: api/tasks
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetAllTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recuperar las tareas: {ex.Message}");
            }
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Models.Task>> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Models.Task>> CreateTask(Models.Task task)
        {
            try
            {
                Console.WriteLine($"Usuario autenticado: {User.Identity?.IsAuthenticated}");
                Console.WriteLine($"Usuario: {User.Identity?.Name}");

                var createdTask = await _taskService.CreateTaskAsync(task);
                return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la tarea: {ex.Message}");
            }
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask(int id, Models.Task task)
        {
            try
            {
                var updatedTask = await _taskService.UpdateTaskAsync(id, task);
                return Ok(updatedTask);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                return Ok(new { Success = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}