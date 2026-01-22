using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Implementations;
using DynamicFormBuilder.Services.Interfaces;
using DynamicFormBuilder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;

[Authorize]
public class StudentsController : Controller
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // GET: Students/Index
    public IActionResult Index(string name, string email, int page = 1, int pageSize = 10)
    {
        ViewBag.Name = name;
        ViewBag.Email = email;

        IEnumerable<StudentModel> students = _studentService.GetAllStudents();

        if (!string.IsNullOrEmpty(name))
        {
            students = students.Where(s => s.StudentName.Contains(name));
        }
        if (!string.IsNullOrEmpty(email))
        {
            students = students.Where(s => s.Email.Contains(email));
        }

        var studentViewModels = students.Select(s => new StudentViewModel
        {
            Id = s.Id,
            StudentName = s.StudentName,
            Age = s.Age,
            PhoneNumber = s.PhoneNumber,
            Email = s.Email,
            Address = s.Address,
            Department = s.Department?.DepartmentName
        }).ToList();

        return View(studentViewModels.ToPagedList(page, pageSize));
    }

    // GET: Students/Create
    public IActionResult Create()
    {
        var departments = _studentService.GetAllDepartments();

        ViewBag.DepartmentList = departments.Select(d => new SelectListItem
        {
            Value = d.DepartmentID.ToString(),
            Text = d.DepartmentName
        }).ToList();

        return View();
    }


    // POST: Students/Create
    [HttpPost]
    public IActionResult Create(StudentModel model)
    {
        if (ModelState.IsValid)
        {
            _studentService.AddStudent(model);
            ViewBag.Message = "Student Registered Successfully!";
            return RedirectToAction("Index");
        }

        var departments = _studentService.GetAllDepartments();

        ViewBag.DepartmentList = departments.Select(d => new SelectListItem
        {
            Value = d.DepartmentID.ToString(),
            Text = d.DepartmentName
        }).ToList();

        return View(model);
    }

    public IActionResult Edit(int id)
    {
        var student = _studentService.GetStudentById(id);
        if (student == null)
            return NotFound();

        var departments = _studentService.GetAllDepartments();

        ViewBag.DepartmentList = departments.Select(d => new SelectListItem
        {
            Value = d.DepartmentID.ToString(),
            Text = d.DepartmentName
        }).ToList();

        return View(student);
    }

    [HttpPost]
    public IActionResult Edit(StudentModel model)
    {
        if (ModelState.IsValid)
        {
            _studentService.UpdateStudent(model);
            return RedirectToAction("Index");
        }

        var departments = _studentService.GetAllDepartments();

        ViewBag.DepartmentList = departments.Select(d => new SelectListItem
        {
            Value = d.DepartmentID.ToString(),
            Text = d.DepartmentName
        }).ToList();

        return View(model);
    }

    // GET: Students/Delete/5
    public IActionResult Delete(int id)
    {
        var student = _studentService.GetStudentById(id);
        if (student == null)
            return NotFound();

        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _studentService.DeleteStudent(id);
        return RedirectToAction("Index");
    }
}