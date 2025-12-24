using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicFormBuilder.Models
{
    [Table("Division")]
    public class DivisionModel
    {
        [Key]
        public int DivisionID { get; set; }

        [Required]
        [StringLength(100)]
        public string DivisionName { get; set; }

        public List<DistrictModel> Districts { get; set; }
    }
}
