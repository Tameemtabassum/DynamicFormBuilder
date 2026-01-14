using System.ComponentModel.DataAnnotations;

public class EmployeeChangeHistoriesModel
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string EmployeeId { get; set; }

    public string? PreviousData { get; set; }
    public string? UpdatedData { get; set; }

    public DateTime ChangedAt { get; set; }

  
}
