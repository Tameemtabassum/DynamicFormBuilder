using Microsoft.AspNetCore.Mvc;
using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;
using Newtonsoft.Json;


namespace DynamicFormBuilder.Controllers
{
    public class FormController : Controller
    {
        private readonly FormRepository _repository;


        public FormController(FormRepository repository)
        {
            _repository = repository;
        }

        // GET: Form/Index - List all forms
        public IActionResult Index()
        {
            return View();
        }

        // GET: Form/Create - Create new form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Form/Create - Save form
        [HttpPost]
        public IActionResult Create(string formTitle, string fieldsJson)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(formTitle))
                {
                    return Json(new { success = false, message = "Form title is required." });
                }

                var fields = JsonConvert.DeserializeObject<List<FormFieldModel>>(fieldsJson);

                if (fields == null || !fields.Any())
                {
                    return Json(new { success = false, message = "Please add at least one field." });
                }

                // Insert form
                var form = new FormModel { FormTitle = formTitle };
                int formId = _repository.InsertForm(form);

                // Insert fields
                for (int i = 0; i < fields.Count; i++)
                {
                    fields[i].FormId = formId;
                    fields[i].FieldOrder = i + 1;
                    _repository.InsertFormField(fields[i]);
                }

                return Json(new { success = true, message = "Form created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Form/GetForms - DataTable server-side processing
        [HttpPost]
        public IActionResult GetForms([FromBody] DataTableRequest request)
        {
            try
            {
                int pageNumber = (request.Start / request.Length) + 1;
                int pageSize = request.Length;
                string searchValue = request.Search?.Value ?? "";

                var response = _repository.GetAllForms(pageNumber, pageSize, searchValue);
                response.Draw = request.Draw;

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // GET: Form/Preview/5
        public IActionResult Preview(int id)
        {
            var form = _repository.GetFormById(id);

            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }
        public IActionResult Edit(int id)
        {
            // Get
            // 
            var form = _repository.GetFormById(id);

            if (form == null)
                return NotFound();

            // Get all fields for this form
            var fields = _repository.GetFormById(id);

            // Map to ViewModel
            var viewModel = new FormViewModel
            {
                FormId = form.FormId,
                FormTitle = form.FormTitle,
                CreatedDate = form.CreatedDate,
                //Fields = fields
            };

            return View(viewModel);
        }


        //[HttpPost]
        //public IActionResult Edit(int id, string formTitle, string fieldsJson)
        //{
        //    try
        //    {
        //        // Deserialize the fields JSON
        //        var fields = System.Text.Json.JsonSerializer.Deserialize<List<FormFieldModel>>(fieldsJson);

        //        if (string.IsNullOrWhiteSpace(formTitle))
        //            return Json(new { success = false, message = "Form title is required." });

        //        if (fields == null || fields.Count == 0)
        //            return Json(new { success = false, message = "At least one field is required." });

        //        // Update form title
        //        _repository.UpdateForm(id, formTitle);

        //        // Remove old fields
        //        //_repository.DeleteFormFields(id);

        //        // Insert updated fields
        //        for (int i = 0; i < fields.Count; i++)
        //        {
        //            fields[i].FormId = id;
        //            fields[i].FieldOrder = i + 1;
        //            _repository.InsertFormField(fields[i]);
        //        }

        //        return Json(new { success = true, message = "Form updated successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error: " + ex.Message });
        //    }
        //}



    }
}