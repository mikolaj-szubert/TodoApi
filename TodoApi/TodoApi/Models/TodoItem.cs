using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TodoApi.Attributes;

namespace TodoApi.Models;

/// <summary>
/// Obiekt zadań do wykonania.
/// </summary>
public class TodoItem
{
    /// <summary>
    /// Identyfikator
    /// </summary>
    public int Id { get; init; }
    /// <summary>
    /// Tytuł zadania do wykonania.
    /// </summary>
    public string Title { get; set; } = default!;
    /// <summary>
    /// Opis zadania do wykonania.
    /// </summary>
    public string Description { get; set; } = default!;
    /// <summary>
    /// Procent wykonania zadania.
    /// </summary>
    public decimal PercentCompleted { get; set; }
    /// <summary>
    /// Data i czas zakończenia zadania.
    /// </summary>
    public DateTime DueDateTime { get; set; } = default!;
}