using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DynamicFormBuilder.Models
{
    [Table("Departments")]
    public class DepartmentModel
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required]
        public string DepartmentName { get; set; }
    }
}