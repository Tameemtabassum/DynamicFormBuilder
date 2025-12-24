using Microsoft.AspNetCore.Mvc;
using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;
using Newtonsoft.Json;

namespace DynamicFormBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormApiController : ControllerBase
    {
        private readonly FormRepository _repository;

        public FormApiController(FormRepository repository)
        {
            _repository = repository;
        }

        // POST: api/FormApi/Create
        [HttpPost("Create")]
        public IActionResult CreateForm([FromBody] FormSubmitRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FormTitle))
                {
                    return BadRequest(new { success = false, message = "Form title is required." });
                }

                if (request.Fields == null || request.Fields.Count == 0)
                {
                    return BadRequest(new { success = false, message = "Please add at least one field." });
                }

                // Insert form
                var form = new FormModel { FormTitle = request.FormTitle };
                int formId = _repository.InsertForm(form);

                // Insert fields
                for (int i = 0; i < request.Fields.Count; i++)
                {
                    request.Fields[i].FormId = formId;
                    request.Fields[i].FieldOrder = i + 1;
                    _repository.InsertFormField(request.Fields[i]);
                }

                return Ok(new { success = true, message = "Form created successfully!", formId = formId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error: " + ex.Message });
            }
        }

        // GET: api/FormApi/GetForm/5
        [HttpGet("GetForm/{id}")]
        public IActionResult GetForm(int id)
        {
            try
            {
                var form = _repository.GetFormById(id);

                if (form == null)
                {
                    return NotFound(new { success = false, message = "Form not found." });
                }

                return Ok(new { success = true, data = form });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error: " + ex.Message });
            }
        }
    }

    public class FormSubmitRequest
    {
        public string FormTitle { get; set; }
        public List<FormFieldModel> Fields { get; set; }
    }
}