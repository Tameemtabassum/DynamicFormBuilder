using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Services.Interfaces
{
    public interface IStudentService
    {
        StudentModel GetStudentById(int id);
        List<StudentModel> GetAllStudents();
        void AddStudent(StudentModel student);
        void UpdateStudent(StudentModel student);
        void DeleteStudent(int id);

    }
}
