using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Implementations;
using DynamicFormBuilder.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


public class StudentsController : Controller
{
    //private readonly StudentRepository _studentRepository;
    private readonly IStudentService _studentService;


    public StudentsController(IStudentService studentService)
    {
        //_studentRepository = repository;
        _studentService = studentService;
    }

    // GET: Students/Index
    public IActionResult Index()
    {
        var students = _studentService.GetAllStudents();
        return View(students);
    }



    // GET: Students/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Students/Create
    [HttpPost]
    public IActionResult Create(StudentModel model)
    {
        if (ModelState.IsValid)
        {
            // Save the student to the database using repository
            
            _studentService.AddStudent(model);

            ViewBag.Message = "Student Registered Successfully!";
            return RedirectToAction("Index");
        }

        return View(model);
    }
    public IActionResult Edit(int id)
    {
        var student = _studentService.GetStudentById(id);

        if (student == null)
            return NotFound();

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

        return View(model);
    }
    // GET: Students/Delete/5
    public IActionResult Delete(int id)
    {
        // Optionally, fetch the student to show confirmation
        var student = _studentService.GetStudentById(id);

        if (student == null)
            return NotFound();

        return View(student); // pass student to confirmation view
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
