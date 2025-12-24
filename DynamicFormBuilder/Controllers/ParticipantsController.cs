using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Linq;


namespace DynamicFormBuilder.Controllers
{
    public class ParticipantsController : Controller
    {
        private readonly IParticipantsService _participantsService;

        public ParticipantsController(IParticipantsService participantsService)
        {
            _participantsService = participantsService;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            int total = _participantsService.Count();
            var data = _participantsService.GetPage(page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            return View(data);
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

            return View(participant);
        }

       
        [HttpPost]
        public IActionResult Edit(ParticipantsModel model)
        {
            if (ModelState.IsValid)
            {
                _participantsService.UpdateParticipant(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Participants/Delete/5
        public IActionResult Delete(int id)
        {
            var participant = _participantsService.GetParticipantById(id);

            if (participant == null)
                return NotFound();

            return View(participant); 
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _participantsService.DeleteParticipant(id);
            return RedirectToAction("Index");
        }

       
        public IActionResult Details(int id)
        {
            var participant = _participantsService.GetParticipantById(id);
            if (participant == null)
                return NotFound();

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
    }

}
