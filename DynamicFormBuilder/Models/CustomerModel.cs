using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicFormBuilder.Models
{
    [Table("Customer")]
    public class CustomerModel
    {
        [Key]
        public int CustomerID { get; set; }
        
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^01\d{9}$", ErrorMessage = "Phone number must start with 01 and be exactly 11 digits")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "NID number is required")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "NID must contain exactly 12 digits")]
        public string NID { get; set; }



        public int? DivisionID { get; set; }

       
        public int? DistrictID { get; set; }

        

       
        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CustomerModel), nameof(ValidateDOB))]
        public DateTime? DOB { get; set; }

        public static ValidationResult ValidateDOB(DateTime? dob, ValidationContext context)
        {
            if (dob.HasValue && dob.Value.Date > DateTime.Today)
            {
                return new ValidationResult("Date of Birth cannot be in the future");
            }
            return ValidationResult.Success;
        }

        [Required(ErrorMessage = "profession is required")]
        [StringLength(150)]
        public string Profession { get; set; }

        [Required(ErrorMessage = "balance is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
    }
}
