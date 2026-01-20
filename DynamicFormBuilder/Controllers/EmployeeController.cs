using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using DynamicFormBuilder.Services.Interfaces;
using DynamicFormBuilder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Utilities;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;
using System.ComponentModel;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.IO.Compression;
using System.Security;
using System.Transactions;
using X.PagedList.Extensions;
using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Controllers
{
    [Authorize]

    public class EmployeeController : Controller
    {

        private IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index(string email, int? designationId, int page = 1, int pageSize = 10)
        {
            var employees = _employeeService.GetAll();

            // Apply filters
            if (!string.IsNullOrEmpty(email))
            {
                employees = employees.Where(e =>
                    e.Email.Contains(email, StringComparison.OrdinalIgnoreCase) ||
                    e.FullName.Contains(email, StringComparison.OrdinalIgnoreCase) ||
                    e.EmployeeId.Contains(email, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (designationId.HasValue && designationId.Value > 0)
            {
                // employees = employees.Where(e => e.DesignationID == designationId);
            }

            // Map to ViewModel
            var employeeViewModels = employees.Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                EmployeeId = e.EmployeeId,
                FullName = e.FullName,
                Email = e.Email,
                DOB = e.DOB,
                Designation = e.Designation,
                IsActive = e.IsActive
            });

            var pagedList = employeeViewModels.ToPagedList(page, pageSize);

            ViewBag.SearchEmail = email;
            ViewBag.DesignationId = designationId;

            //  AJAX request, return the same view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return View(pagedList);
            }

            return View(pagedList);
        }

        // CREATE - GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeModel model)
        {
            model.Id = Guid.NewGuid().ToString();

            if (!ModelState.IsValid)
                return View(model);

            _employeeService.Create(model);
            TempData["success"] = "Employee created successfully";
            return RedirectToAction("Index");
        }

        // EDIT - GET (Updated to support AJAX/Modal)
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var employee = _employeeService.GetById(id);
            if (employee == null)
                return NotFound();

            // Check if it's an AJAX request - return partial view for modal
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Edit", employee);
            }

            // For direct browser access, return full view with layout
            return View(employee);
        }

        // EDIT - POST (Updated to support AJAX/Modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                // Check if it's an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("Edit", model);
                }
                return View(model);
            }

            _employeeService.Update(model);
            TempData["success"] = "Employee updated successfully";

            // For AJAX requests, return JSON with redirect info
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, redirectTo = Url.Action("Index") });
            }

            return RedirectToAction("Index");
        }

        // DETAILS - GET
        public IActionResult Details(string employeeId)
        {
            if (employeeId == null)
            {
                // For AJAX requests, return error message in partial view
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    ViewBag.Message = "Employee not found.";
                    return PartialView("Details", new List<EmployeeChangeHistoriesViewModel>());
                }
                return NotFound();
            }

            Thread.Sleep(500); // TEST 

            var history = _employeeService.GetEmployeeChangeHistoryRecord(employeeId);

            if (history == null || !history.Any())
            {
                ViewBag.Message = "No change history found for this employee.";

                // For AJAX requests (modal), return partial view without layout
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("Details", new List<EmployeeChangeHistoriesViewModel>());
                }

                // For direct browser access, return full view with layout
                return View(new List<EmployeeChangeHistoriesViewModel>());
            }

            // Check if it's an AJAX request - return ONLY the partial view (no layout/nav)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Details", history);
            }

            // For direct browser access, return the full view with layout
            return View(history);
        }

        // DELETE - GET (Updated to support AJAX/Modal)
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            // Check if it's an AJAX request - return partial view for modal
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Delete", employee);
            }

            // For direct browser access, return full view with layout
            return View(employee);
        }

        // DELETE - POST (Updated to support AJAX/Modal)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            _employeeService.Delete(id);

            TempData["success"] = "Employee deleted successfully";

            // For AJAX requests, return JSON with redirect info
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, redirectTo = Url.Action("Index") });
            }

            return RedirectToAction("Index");
        }

        // SEARCH - 
        [HttpGet]
        public IActionResult Search(string searchTerm, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Json(new { success = false, data = new List<object>() });
            }

            var employees = _employeeService.GetAll()
                .Where(e => e.EmployeeId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            e.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(e => new
                {
                    id = e.Id,
                    employeeId = e.EmployeeId,
                    fullName = e.FullName,
                    email = e.Email,
                    designation = e.Designation,
                    dob = e.DOB.HasValue ? e.DOB.Value.ToString("yyyy-MM-dd") : "",
                    isActive = e.IsActive
                })
                .ToList();

            return Json(new { success = true, data = employees });
        }

        #region Employee Status Update
        public ActionResult EmployeeStatusUpdate(bool searchStatus, string others, string searchEmployeeId, int page = 1, int pageSize = 30)
        {
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            var model = new EmployeeModel()
            {
                IsActive = searchStatus,
                Id = searchEmployeeId,

                StatusList = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem {Text = "--Select--", Value = ""},
                    new SelectListItem {Value = "ACTIVE" , Text = @"ACTIVE"},
                    new SelectListItem {Value = "INACTIVE", Text = @"INACTIVE"},
                }, "Value", "Text", 1)
            };
            return PartialView(model);
        }

        public ActionResult SearchForEmployeeStatusUpdate(string searchStatus, string others, int searchEmployeeId, int page = 1, int pageSize = 30)
        {
            ViewBag.searchEmployeeId = searchEmployeeId;
            ViewBag.status = searchStatus;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            ViewBag.others = others;

            return View();
        }

        [HttpGet]
        public ActionResult EmployeeStatusUpdateBulkUpload(int page = 1, int pageSize = 30)
        {
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            return PartialView();
        }

        #region Upload Post
        [HttpPost]
        public async Task<ActionResult> EmployeeStatusUpdateBulkUpload(IFormFile fileUpload = null, int page = 1, int pageSize = 30)
        {
            var message = "File upload failed";
            var path = "";

            try
            {
                // 1️⃣ Validate file
                if (fileUpload == null || fileUpload.Length == 0)
                {
                    message = "Please select a file to upload";
                    return CreateJsonResult(page, pageSize, message);
                }

                var filename = Path.GetFileName(fileUpload.FileName);
                var fileExtension = Path.GetExtension(filename).ToLower();
                var allowedExtensions = new[] { ".csv", ".xls", ".xlsx" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    message = "Invalid file type. Only CSV and Excel files are allowed";
                    return CreateJsonResult(page, pageSize, message);
                }

                // 2️⃣ Save uploaded file
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "EmployeeStatusBulk");

                if (!CreateFolderIfNeeded(uploadFolder))
                {
                    message = "Failed to create upload directory";
                    return CreateJsonResult(page, pageSize, message);
                }

                path = Path.Combine(uploadFolder, filename);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                // 3️⃣ Read file into DataTable
                DataTable dt;

                if (fileExtension == ".csv")
                {
                    dt = ReadCsvFileToDataTable(path);
                }
                else // Excel file
                {
                    dt = ReadExcelFileToDataTable(path);
                }

                if (dt.Rows.Count == 0)
                {
                    message = "The uploaded file contains no data";
                    return CreateJsonResult(page, pageSize, message);
                }

                // 4️⃣ Validate data
                var empList = new List<EmployeeModel>();
                var errorList = new List<string>();
                var processedEmployeeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var validStatuses = new[] { "Active", "Inactive" };
                int serial = 0;

                foreach (DataRow row in dt.Rows)
                {
                    serial++;

                    try
                    {
                        var employeeId = GetColumnValue(row, "Employee ID")?.Trim();

                        if (string.IsNullOrWhiteSpace(employeeId))
                        {
                            errorList.Add($"Line {serial}: Employee ID is required");
                            continue;
                        }

                        if (processedEmployeeIds.Contains(employeeId))
                        {
                            errorList.Add($"Line {serial}: Duplicate Employee ID ({employeeId}) in file");
                            continue;
                        }

                        if (!_employeeService.IsEmployeeIdExist(employeeId))
                        {
                            errorList.Add($"Line {serial}: Employee not found ({employeeId})");
                            continue;
                        }

                        var status = GetColumnValue(row, "Status")?.Trim();

                        if (string.IsNullOrWhiteSpace(status))
                        {
                            errorList.Add($"Line {serial}: Status is required for Employee ID ({employeeId})");
                            continue;
                        }

                        if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
                        {
                            errorList.Add($"Line {serial}: Invalid status '{status}'. Allowed: {string.Join(", ", validStatuses)}");
                            continue;
                        }

                        bool isActive = status.Equals("Active", StringComparison.OrdinalIgnoreCase);

                        empList.Add(new EmployeeModel
                        {
                            EmployeeId = employeeId,
                            IsActive = isActive
                        });

                        processedEmployeeIds.Add(employeeId);
                    }
                    catch (Exception exRow)
                    {
                        errorList.Add($"Line {serial}: Error - {exRow.Message}");
                    }
                }

                // 5️⃣ If errors exist, return
                if (errorList.Any())
                {
                    message = "Errors found: " + string.Join("; ", errorList);
                    return CreateJsonResult(page, pageSize, message);
                }

                // 6️⃣ Update database inside a transaction
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var now = DateTime.Now;
                    int updatedCount = 0;

                    foreach (var emp in empList)
                    {
                        var data = _employeeService.GetEmployeeDataByEmployeeId(emp.EmployeeId);

                        if (data == null)
                            throw new Exception($"Employee data not found for ID: {emp.EmployeeId}");

                        data.IsActive = emp.IsActive;
                        _employeeService.Update(data);

                        updatedCount++;
                    }

                    scope.Complete();
                    message = $"✅ Data updated successfully! {updatedCount} employee(s) updated.";
                }
            }
            catch (Exception ex)
            {
                message = $"❌ File upload failed: {ex.Message}";
            }
            finally
            {
                // 7️⃣ Clean up uploaded file
                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                {
                    try
                    {
                        System.IO.File.Delete(path);
                    }
                    catch
                    {
                        // Suppress cleanup errors
                    }
                }
            }

            return CreateJsonResult(page, pageSize, message);
        }

        private DataTable ReadExcelFileToDataTable(string path)
        {
            var dt = new DataTable();

            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                var ws = package.Workbook.Worksheets.FirstOrDefault();

                if (ws == null || ws.Dimension == null)
                    return dt;

                // Add columns from first row
                for (int col = 1; col <= ws.Dimension.End.Column; col++)
                {
                    var headerValue = ws.Cells[1, col].Text?.Trim();
                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        dt.Columns.Add(headerValue);
                    }
                }

                // Add data rows (skip empty rows)
                for (int rowNum = 2; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    // Check if row is empty
                    bool isEmptyRow = true;
                    for (int col = 1; col <= ws.Dimension.End.Column; col++)
                    {
                        if (!string.IsNullOrWhiteSpace(ws.Cells[rowNum, col].Text))
                        {
                            isEmptyRow = false;
                            break;
                        }
                    }

                    if (isEmptyRow)
                        continue;

                    var row = dt.NewRow();

                    for (int col = 1; col <= ws.Dimension.End.Column; col++)
                    {
                        row[col - 1] = ws.Cells[rowNum, col].Text?.Trim() ?? string.Empty;
                    }

                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        private DataTable ReadCsvFileToDataTable(string path)
        {
            var dt = new DataTable();

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                BadDataFound = null
            }))
            {
                // Read header
                csv.Read();
                csv.ReadHeader();

                foreach (var header in csv.HeaderRecord)
                {
                    dt.Columns.Add(header?.Trim() ?? string.Empty);
                }

                // Read rows
                while (csv.Read())
                {
                    // Skip empty rows
                    bool isEmptyRow = true;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(csv.GetField(i)))
                        {
                            isEmptyRow = false;
                            break;
                        }
                    }

                    if (isEmptyRow)
                        continue;

                    var row = dt.NewRow();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        row[i] = csv.GetField(i)?.Trim() ?? string.Empty;
                    }

                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        private string GetColumnValue(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row.IsNull(columnName))
                return null;

            return row[columnName]?.ToString()?.Trim();
        }

        private bool CreateFolderIfNeeded(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private JsonResult CreateJsonResult(int page, int pageSize, string message)
        {
            return Json(new
            {
                redirectTo = Url.Action("Index", "Employee", new { page, pageSize }),
                message,
                position = "mainContent"
            });
        }
        #endregion

        #endregion
    }
}