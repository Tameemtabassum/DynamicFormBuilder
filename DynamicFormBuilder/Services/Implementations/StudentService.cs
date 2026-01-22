using DocumentFormat.OpenXml.InkML;
using DynamicFormBuilder.Data.DBContext; 
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;

namespace DynamicFormBuilder.Services.Implementations
{
    public class StudentService : IStudentService
    {
        public readonly ApplicationDbContext _dbContext;
        public StudentService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }




        public StudentModel GetStudentById(int id)
        {
            var student = _dbContext.StudentModels.Find(id);
            return student;
        }

        public List<StudentModel> GetAllStudents()
        {
            // Only get students with valid DepartmentID
            var students = _dbContext.StudentModels
                .Include(s => s.Department)
                .Where(s => s.DepartmentID != null && s.DepartmentID > 0)
                .ToList();
            return students;
        }


        public void AddStudent(StudentModel student)
        {
            _dbContext.StudentModels.Add(student);
            _dbContext.SaveChanges();
        }

        public void UpdateStudent(StudentModel student)
        {
            _dbContext.StudentModels.Update(student);
            _dbContext.SaveChanges();
        }
        public void DeleteStudent(int id)
        {
            var student = new StudentModel { Id = id };
            _dbContext.StudentModels.Remove(student);
            _dbContext.SaveChanges();
        }



        public IEnumerable<DepartmentModel> GetAllDepartments()
        {
            return _dbContext.Departments.ToList();
        }
    }
}
