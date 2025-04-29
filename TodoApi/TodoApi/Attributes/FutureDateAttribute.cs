using System.ComponentModel.DataAnnotations;

namespace TodoApi.Attributes
{
    /// <summary>
    /// Atrybut walidacji który sprawdza czy podana data jest w przyszłości.
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// Metoda wyniku walidacji.
        /// </summary>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is DateTime dateTime)
            {
                if (dateTime < DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            else
            {
                return new ValidationResult("Invalid data format.");
            }
            return ValidationResult.Success;
        }
    }
}
