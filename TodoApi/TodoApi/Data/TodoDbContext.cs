using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

/// <summary>
/// Kontekst bazy danych
/// </summary>
/// <param name="options"></param>
public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Tabela zadań do wykonania
    /// </summary>
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}