using System.ComponentModel.DataAnnotations;

namespace DynamicFormBuilder.ViewModels
{
    public class StudentViewModel
    {

        [Required]
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Student name is required")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name must contain only letters and spaces")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, 150, ErrorMessage = "Age must be a valid number")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string Address { get; set; }

        public string Department { get; set; }
    }
}
