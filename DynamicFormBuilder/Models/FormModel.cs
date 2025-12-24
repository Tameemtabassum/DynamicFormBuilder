namespace DynamicFormBuilder.Models
{
    public class FormModel
    {
        public int FormId { get; set; }
        public string FormTitle { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<FormFieldModel> Fields { get; set; } = new List<FormFieldModel>();
        

    }

    public class FormFieldModel
    {
        public int FieldId { get; set; }
        public int FormId { get; set; }
        public string FieldLabel { get; set; }
        public string SelectedOption { get; set; }
        public bool IsRequired { get; set; }
        public int FieldOrder { get; set; }
    }

    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableSearch Search { get; set; }
    }

    public class DataTableSearch
    {
        public string Value { get; set; }
    }

    public class DataTableResponse
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<FormModel> Data { get; set; }
    }
}