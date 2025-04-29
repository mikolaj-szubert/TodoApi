using System.ComponentModel.DataAnnotations;
using TodoApi.Attributes;
namespace TodoApi.Models.DTO
{
    /// <summary>
    /// DTO dodawania obiektu TodoItem do bazy danych
    /// </summary>
    public class PostTodoItemDto
    {
        /// <summary>
        /// Tytuł zadania do wykonania.
        /// Maksymalnie 100 znaków.
        /// </summary>
        [Required, StringLength(100, ErrorMessage = "Pole {0} nie może mieć więcej niż {1} znaków.")]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Opis zadania do wykonania.
        /// Maksymalnie 255 znaków.
        /// </summary>
        [Required, StringLength(255, ErrorMessage = "Pole {0} nie może mieć więcej niż {1} znaków.")]
        public string Description { get; set; } = default!;

        /// <summary>
        /// Procent wykonania zadania.
        /// </summary>
        [Range(0, 100, ErrorMessage = "Procent wykonania musi być w zakresie od {1} do {2}.")]
        public decimal PercentCompleted { get; set; }

        /// <summary>
        /// Data i czas zakończenia zadania.
        /// Tylko data i czas z przyszłości są akceptowane.
        /// </summary>
        [Required, FutureDate(ErrorMessage = "Czas zakończenia zadania musi być w przyszłości.")]
        public DateTime DueDateTime { get; set; } = default!;

        /// <summary>
        /// Konwersja obiektu DTO na obiekt TodoItem.
        /// </summary>
        /// <returns>TodoItem</returns>
        public TodoItem ToTodoItem()
        {
            return new TodoItem
            {
                Title = Title,
                Description = Description,
                PercentCompleted =  PercentCompleted,
                DueDateTime = DueDateTime
            };
        }
    }
    /// <summary>
    /// DTO edytowania obiektu TodoItem w bazie danych
    /// </summary>
    public class PutTodoItemDto
    {
        /// <summary>
        /// Tytuł zadania do wykonania.
        /// Maksymalnie 100 znaków.
        /// </summary>
        [StringLength(100, ErrorMessage = "Pole {0} nie może mieć więcej niż {1} znaków.")]
        public string Title { get; set; } = default!;

        /// <summary>
        /// Opis zadania do wykonania.
        /// Maksymalnie 255 znaków.
        /// </summary>
        [StringLength(255, ErrorMessage = "Pole {0} nie może mieć więcej niż {1} znaków.")]
        public string Description { get; set; } = default!;

        /// <summary>
        /// Procent wykonania zadania.
        /// </summary>
        [Range(0, 100, ErrorMessage = "Procent wykonania musi być w zakresie od {1} do {2}.")]
        public decimal? PercentCompleted { get; set; }

        /// <summary>
        /// Data i czas zakończenia zadania.
        /// </summary>
        public DateTime? DueDateTime { get; set; }
    }
}
