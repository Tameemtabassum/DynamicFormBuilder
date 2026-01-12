using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicFormBuilder.ViewModels
{
    public class EmployeeViewModel
    {
        [Key]

        public string? Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Employee ID is required")]
        public string EmployeeId { get; set; }

        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [StringLength(150)]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Designation cannot contain numbers")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Status is required")]


        public bool IsActive { get; set; }

        [NotMapped]
        [ValidateNever]
        public SelectList StatusList { get; internal set; }
    }
}
