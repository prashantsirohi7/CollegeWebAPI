using System.ComponentModel.DataAnnotations;

namespace StudentWebAPIProject.Validators
{
    public class DateCheckAttribute : ValidationAttribute 
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var date = (DateTime?)value;

            if (date < DateTime.Today)
                return new ValidationResult("Admission date is less than today date");

            return ValidationResult.Success;
        }
    }
}
