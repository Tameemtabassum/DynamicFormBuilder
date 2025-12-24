using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicFormBuilder.Models
{
    [Table("Participants")]
    public class ParticipantsModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [StringLength(255)]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^01\d{9}$", ErrorMessage = "Phone number must start with 01 and be exactly 11 digits")]
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }

    }
}










