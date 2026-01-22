using DynamicFormBuilder.Models;
using System.Data.SqlClient;

namespace DynamicFormBuilder.Data
{
    public class StudentRepository
    {
        private readonly string _connectionString;

        public StudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<StudentModel> GetAllStudents()
        {
            var students = new List<StudentModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Students", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new StudentModel
                    {
                        Id = (int)reader["Id"],
                        StudentName = reader["StudentName"].ToString(),
                        Age = (int)reader["Age"],
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString(),
                        DepartmentID = reader["DepartmentID"] != DBNull.Value ? (int)reader["DepartmentID"] : 0

                    });
                }
            }

            return students;
        }
        public void AddStudent(StudentModel student)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand(
                    @"INSERT INTO Students 
              (StudentName, Age, PhoneNumber, Email, Address, Department) 
              VALUES 
              (@StudentName, @Age, @PhoneNumber, @Email, @Address, @Department)", connection);

                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@StudentName", student.StudentName);
                command.Parameters.AddWithValue("@Age", student.Age);
                command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                command.Parameters.AddWithValue("@Email", student.Email);
                command.Parameters.AddWithValue("@Address", (object)student.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@Department", (object)student.Department ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateStudent(StudentModel student)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand(
                    @"UPDATE Students
              SET StudentName = @StudentName,
                  Age = @Age,
                  PhoneNumber = @PhoneNumber,
                  Email = @Email,
                  Address = @Address,
                  Department = @Department
              WHERE Id = @Id", connection);

                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Id", student.Id);
                command.Parameters.AddWithValue("@StudentName", student.StudentName);
                command.Parameters.AddWithValue("@Age", student.Age);
                command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                command.Parameters.AddWithValue("@Email", student.Email);
                command.Parameters.AddWithValue("@Address", (object)student.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@Department", (object)student.Department ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }
        public void DeleteStudent(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand(
                    "DELETE FROM Students WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);

                command.ExecuteNonQuery();
            }
        }



    }
}
