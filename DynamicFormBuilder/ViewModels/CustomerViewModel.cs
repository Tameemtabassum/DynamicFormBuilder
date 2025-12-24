using DynamicFormBuilder.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicFormBuilder.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(30)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "NID number is required")]
        public string NID { get; set; }


        public int? DivisionID { get; set; }
        public int? DistrictID { get; set; }


        public DivisionModel? Division { get; set; }

        public DistrictModel? District { get; set; }

        [Required(ErrorMessage = "DOB is required")]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Profession is required")]
        [StringLength(150)]
        public string Profession { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }


        
        [DisplayName("Division Name")]
        [StringLength(100)]
        public string? DivisionName { get; set; }

        public List<DistrictModel> Districts { get; set; }

        [Required(ErrorMessage = "Balance is required")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }



      
        [StringLength(100)]
        public string? DistrictName { get; set; }


    }
}
