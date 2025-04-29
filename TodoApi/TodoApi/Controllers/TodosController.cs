using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Models.DTO;

namespace TodoApi.Controllers;

/// <summary>
/// Kontroler zadań
/// </summary>
/// <param name="context"></param>
[Route("api/[controller]")]
[ApiController]
public class TodosController(TodoDbContext context) : ControllerBase
{
    private readonly TodoDbContext _context = context;

    /// <summary>
    /// Pobiera wszystkie zadania.
    /// </summary>
    /// <returns>Zadania do wykonania</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
    {
        return await _context.TodoItems.ToListAsync();
    }

    /// <summary>
    /// Pobiera wybranie zadanie.
    /// </summary>
    /// <returns>Zadanie do wykonania</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodo(int id)
    {
        var todo = await _context.TodoItems.FindAsync(id);
        return todo == null ? NotFound() : Ok(todo);
    }

    /// <summary>
    /// Pobiera zadania do wykonania w określonym okresie.
    /// </summary>
    /// <param name="period">Okres czasu (today, tomorrow, week)</param>
    [HttpGet("incoming/{period}")]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetIncoming(string period)
    {
        var now = DateTime.UtcNow;
        DateTime start, end;

        switch (period.ToLower())
        {
            case "today":
                start = now.Date;
                end = now.Date.AddDays(1).AddTicks(-1);
                break;
            case "tomorrow":
                start = now.Date.AddDays(1);
                end = start.AddDays(1).AddTicks(-1);
                break;
            case "week":
                start = now.Date.AddDays(-(int)now.DayOfWeek);
                end = start.AddDays(7).AddTicks(-1);
                break;
            default:
                return BadRequest("Invalid period. Use: today, tomorrow, week");
        }

        return await _context.TodoItems
            .Where(t => t.DueDateTime >= start && t.DueDateTime <= end)
            .ToListAsync();
    }

    /// <summary>
    /// Dodaje zadanie.
    /// </summary>
    /// <returns>Nowo utworzone zadanie</returns>
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodo(PostTodoItemDto todo)
    {
        var entity = todo.ToTodoItem();
        _context.TodoItems.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTodo), new { id = entity.Id }, todo);
    }

    /// <summary>
    /// Aktualizuje zadanie.
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <param name="todo">Dane zadania</param>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodo(int id, PutTodoItemDto todo)
    {
        var entity = await _context.TodoItems.FindAsync(id);
        if (entity == null) return BadRequest();
        if (todo.DueDateTime != null && todo.DueDateTime != entity.DueDateTime)
        {
            if (todo.DueDateTime >= DateTime.Now)
                entity.DueDateTime = (DateTime)todo.DueDateTime;
            else
                return BadRequest();

        }
        entity.Title = todo.Title ?? entity.Title;
        entity.Description = todo.Description ?? entity.Description;
        entity.PercentCompleted = todo.PercentCompleted ?? entity.PercentCompleted;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Ustawia procent wykonania zadania.
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <param name="percent">Procent wykonania zadania</param>
    [HttpPatch("{id}/percent")]
    public async Task<IActionResult> SetPercentComplete(int id, [FromBody] decimal percent)
    {
        if (percent < 0 || percent > 100)
            return BadRequest("Procent musi być 0-100");

        var todo = await _context.TodoItems.FindAsync(id);
        if (todo == null) return NotFound();

        todo.PercentCompleted = percent;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Ustawia zadanie jako ukończone (100% ukończenia)
    /// </summary>
    /// <param name="id">Todo item ID</param>
    [HttpPatch("{id}/done")]
    public async Task<IActionResult> MarkAsDone(int id)
    {
        var todo = await _context.TodoItems.FindAsync(id);
        if (todo == null) return NotFound();

        todo.PercentCompleted = 100;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Usuwa zaplanowane zadanie
    /// </summary>
    /// <param name="id">ID zadania</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.TodoItems.FindAsync(id);
        if (todo == null) return NotFound();
        _context.TodoItems.Remove(todo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}