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

    public class DOBCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var date = (DateTime?)value;
            var dateDiff = DateTime.Today.Year - date.Value.Year;

            if (date >= DateTime.Today)
                return new ValidationResult("DOB cannot be today or future date");

            if(dateDiff < 18 || dateDiff > 30)
                return new ValidationResult("DOB should be betenn 18 and 30 years");

            return ValidationResult.Success;
        }
    }
}
