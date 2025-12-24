using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicFormBuilder.Models
{
    [Table("District")]
    public class DistrictModel
    {
        [Key]
        public int DistrictID { get; set; }

        [Required]
        [StringLength(100)]
        public string DistrictName { get; set; }

        public int DivisionID { get; set; }

        [ForeignKey("DivisionID")]
        public DivisionModel Division { get; set; }
    }
}
