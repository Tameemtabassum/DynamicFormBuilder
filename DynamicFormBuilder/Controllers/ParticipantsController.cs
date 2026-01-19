using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Implementations;
using DynamicFormBuilder.Services.Interfaces;
using DynamicFormBuilder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace DynamicFormBuilder.Controllers
{
    [Authorize]
    public class ParticipantsController : Controller
    {
        private readonly IParticipantsService _participantsService;

        public ParticipantsController(IParticipantsService participantsService)
        {
            _participantsService = participantsService;
        }

        public IActionResult Index(int? page, int pageSize = 10)
        {
            var participants = _participantsService.GetAllParticipants();
            var participantsList = participants
                .Select(p => new ParticipantsViewModel
                {
                    Name = p.Name,
                    Address = p.Address,
                    Country = p.Country,
                    Email = p.Email,
                    Phone = p.Phone,
                    Id = p.Id
                });
            var pagedList = participantsList.ToPagedList(page ?? 1, pageSize);

            // Handle AJAX requests 
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return View(pagedList);
            }

            return View(pagedList);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(ParticipantsModel model)
        {
            if (ModelState.IsValid)
            {
                _participantsService.AddParticipant(model);
                ViewBag.Message = "Participant Added Successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }


        public IActionResult Edit(int id)
        {
            var participant = _participantsService.GetParticipantById(id);
            if (participant == null)
                return NotFound();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Edit", participant);
            }

            return View(participant);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ParticipantsModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("Edit", model);
                }
                return View(model);
            }

            _participantsService.UpdateParticipant(model);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var participant = _participantsService.GetParticipantById(id);
            if (participant == null)
                return NotFound();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Delete", participant);
            }

            return View(participant);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _participantsService.DeleteParticipant(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }


            return RedirectToAction(nameof(Index));
        }


        public IActionResult Details(int id)
        {
            var participant = _participantsService.GetParticipantById(id);
            if (participant == null)
                return NotFound();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Details", participant);
            }

            return View(participant);
        }


        public IActionResult ExcelDownload()
        {
            // File name
            var sFileName = "Participants_List_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

            // Fetch all participants
            var data = _participantsService.GetAllParticipants() ?? new List<ParticipantsModel>();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Participants");
                ws.RowHeight = 20;

                // Header row style
                var range = ws.Range("A1:F1");
                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Row(1).Style.Fill.PatternType = XLFillPatternValues.Solid;
                ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromArgb(200, 200, 198);
                ws.Row(1).Style.Font.Bold = true;
                ws.Row(1).Style.Font.FontSize = 12;
                ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Headers
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Name";
                ws.Cell(1, 3).Value = "Email";
                ws.Cell(1, 4).Value = "Phone";
                ws.Cell(1, 5).Value = "Address";
                ws.Cell(1, 6).Value = "Country";

                // Fill data
                if (data.Any())
                {
                    int rowId = 2;
                    foreach (var p in data)
                    {
                        ws.Cell(rowId, 1).Value = p.Id;
                        ws.Cell(rowId, 2).Value = p.Name;
                        ws.Cell(rowId, 3).Value = p.Email;
                        ws.Cell(rowId, 4).Value = p.Phone;
                        ws.Cell(rowId, 5).Value = p.Address;
                        ws.Cell(rowId, 6).Value = p.Country;
                        rowId++;
                    }
                }

                // Adjust column widths and alignment
                for (int i = 1; i <= 6; i++)
                {
                    ws.Column(i).AdjustToContents();
                    ws.Column(i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Column(i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }

                // Return file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
                }
            }
        }
        
        public IActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Message"] = "No file selected!";
                return RedirectToAction("Index");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
            {
                TempData["Message"] = "Please upload a valid Excel file (.xlsx)";
                return RedirectToAction("Index");
            }

            int updated = 0;
            int notFound = 0;

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(stream);
                var sheet = package.Workbook.Worksheets.FirstOrDefault();

                if (sheet?.Dimension == null)
                {
                    TempData["Message"] = "Excel file is empty or invalid!";
                    return RedirectToAction("Index");
                }

                var allParticipants = _participantsService.GetAllParticipants()?.ToList();
                if (allParticipants == null || !allParticipants.Any())
                {
                    TempData["Message"] = "No participants in database!";
                    return RedirectToAction("Index");
                }

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    string nameFromExcel = sheet.Cells[row, 1].GetValue<string>()?.Trim();
                    string countryFromExcel = sheet.Cells[row, 2].GetValue<string>()?.Trim();

                    if (string.IsNullOrWhiteSpace(nameFromExcel))
                        continue;

                    var normalizedExcelName = nameFromExcel.Replace("  ", " ").ToLower();

                    var participant = allParticipants.FirstOrDefault(p =>
                        !string.IsNullOrWhiteSpace(p.Name) &&
                        p.Name.Replace("  ", " ").ToLower() == normalizedExcelName
                    );

                    if (participant != null)
                    {
                        // Get fresh instance from DB and update
                        var dbParticipant = _participantsService.GetParticipantById(participant.Id);
                        if (dbParticipant != null)
                        {
                            dbParticipant.Country = countryFromExcel;
                            _participantsService.UpdateParticipant(dbParticipant);
                            updated++;
                        }
                    }
                    else
                    {
                        notFound++;
                    }
                }

                TempData["Message"] = $"✓ {updated} participants updated. {notFound} names not found.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
