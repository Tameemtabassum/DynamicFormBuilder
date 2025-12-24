using System.Data;
using System.Data.SqlClient;
using DynamicFormBuilder.Models;
using Newtonsoft.Json;


namespace DynamicFormBuilder.Data
{
    public class FormRepository
    {
        private readonly string _connectionString;

        public FormRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int InsertForm(FormModel form)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertForm", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FormTitle", form.FormTitle);

                    SqlParameter formIdParam = new SqlParameter("@FormId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(formIdParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return (int)formIdParam.Value;
                }
            }
        }

        public void InsertFormField(FormFieldModel field)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertFormField", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FormId", field.FormId);
                    cmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                    cmd.Parameters.AddWithValue("@SelectedOption", field.SelectedOption);
                    cmd.Parameters.AddWithValue("@IsRequired", field.IsRequired);
                    cmd.Parameters.AddWithValue("@FieldOrder", field.FieldOrder);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTableResponse GetAllForms(int pageNumber, int pageSize, string searchValue)
        {
            DataTableResponse response = new DataTableResponse();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllForms", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@SearchValue", searchValue ?? "");

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        response.Data = new List<FormModel>();

                        while (reader.Read())
                        {
                            response.Data.Add(new FormModel
                            {
                                FormId = (int)reader["FormId"],
                                FormTitle = reader["FormTitle"].ToString(),
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });

                            if (response.RecordsTotal == 0)
                            {
                                response.RecordsTotal = (int)reader["TotalRecords"];
                                response.RecordsFiltered = (int)reader["TotalRecords"];
                            }
                        }
                    }
                }
            }

            return response;
        }

        public FormModel GetFormById(int formId)
        {
            FormModel form = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetFormById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FormId", formId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            form = new FormModel
                            {
                                FormId = (int)reader["FormId"],
                                FormTitle = reader["FormTitle"].ToString(),
                                CreatedDate = (DateTime)reader["CreatedDate"],
                                Fields = new List<FormFieldModel>()
                            };
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                form.Fields.Add(new FormFieldModel
                                {
                                    FieldId = (int)reader["FieldId"],
                                    FormId = (int)reader["FormId"],
                                    FieldLabel = reader["FieldLabel"].ToString(),
                                    SelectedOption = reader["SelectedOption"].ToString(),
                                    IsRequired = (bool)reader["IsRequired"],
                                    FieldOrder = (int)reader["FieldOrder"]
                                });
                            }
                        }
                    }
                }
            }

            return form;
        }
    }
}