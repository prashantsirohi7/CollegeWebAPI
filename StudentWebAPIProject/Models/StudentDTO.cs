using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using StudentWebAPIProject.Validators;
using System.ComponentModel.DataAnnotations;

namespace StudentWebAPIProject.Models
{
    public class StudentDTO
    {
        [ValidateNever]
        public int id { get; set; }
        [Required(ErrorMessage = "Please provide Student name")]
        [StringLength(30)]
        public string name { get; set; }
        [Required]
        [Range(18,30)]
        public int age { get; set; }
        [EmailAddress(ErrorMessage = "Please provide valid email")]
        public string email { get; set; }
        [Required]
        [StringLength(150)]
        public string address { get; set; }
        [DateCheck]
        public DateTime addmissionDate { get; set; }
    }
}
