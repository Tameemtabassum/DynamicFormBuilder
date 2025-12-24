namespace DynamicFormBuilder.Models
{
    public class FormViewModel
    {
        public int FormId { get; set; }
        public string FormTitle { get; set; }
        public DateTime CreatedDate { get; set; }

        // List of fields for edit
        public List<FormFieldModel> Fields { get; set; } = new List<FormFieldModel>();
    }
}