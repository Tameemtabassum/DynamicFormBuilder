using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Implementations;
using DynamicFormBuilder.Services.Interfaces;
using DynamicFormBuilder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;


public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public IActionResult Index(string phone, int? divisionId, int page = 1, int pageSize = 10)
    {
        // Store filter values in ViewBag
        ViewBag.Phone = phone;
        ViewBag.DivisionId = divisionId;

        // divisions for dropdown
        var divisions = _customerService.GetAllDivision() ?? new List<DivisionModel>();
        ViewBag.DivisionList = divisions.Select(d => new SelectListItem
        {
            Value = d.DivisionID.ToString(),
            Text = d.DivisionName
        }).ToList();

        // filtered customers
        var allCustomers = _customerService.GetAllSearchCustomer(phone, divisionId);
        return View(allCustomers.ToPagedList(page, pageSize));
    }

    public IActionResult Create()
    {
        var divisions = _customerService.GetAllDivision();
        var districts = _customerService.GetAllDistrict();

        ViewBag.DivisionList = divisions.Select(d => new SelectListItem
        {
            Value = d.DivisionID.ToString(),
            Text = d.DivisionName
        }).ToList();

        ViewBag.DistrictList = districts.Select(d => new SelectListItem
        {
            Value = d.DistrictID.ToString(),
            Text = d.DistrictName
        }).ToList();

        return View();
    }

    [HttpPost]
    public IActionResult Create(CustomerModel model)
    {
        if (ModelState.IsValid)
        {
            _customerService.AddCustomer(model);
            return RedirectToAction("Index");
        }


        var divisions = _customerService.GetAllDivision();
        var districts = _customerService.GetAllDistrict();

        ViewBag.DivisionList = divisions.Select(d => new SelectListItem
        {
            Value = d.DivisionID.ToString(),
            Text = d.DivisionName
        }).ToList();

        ViewBag.DistrictList = districts.Select(d => new SelectListItem
        {
            Value = d.DistrictID.ToString(),
            Text = d.DistrictName
        }).ToList();

        return View(model);
    }

    public IActionResult Edit(int id)
    {
        var customer = _customerService.GetCustomerById(id);

        if (customer == null)
            return NotFound();


        var divisions = _customerService.GetAllDivision();
        ViewBag.DivisionList = divisions.Select(d => new SelectListItem
        {
            Value = d.DivisionID.ToString(),
            Text = d.DivisionName
        }).ToList();


        var districts = _customerService.GetAllDistrict()
                        .Where(d => d.DivisionID == customer.DivisionID)
                        .ToList();

        ViewBag.DistrictList = districts.Select(d => new SelectListItem
        {
            Value = d.DistrictID.ToString(),
            Text = d.DistrictName
        }).ToList();

        return View(customer);
    }

    [HttpPost]
    public IActionResult Edit(CustomerModel model)
    {
        if (ModelState.IsValid)
        {
            _customerService.UpdateCustomer(model);
            return RedirectToAction("Index");
        }

        var divisions = _customerService.GetAllDivision();
        ViewBag.DivisionList = divisions.Select(d => new SelectListItem
        {
            Value = d.DivisionID.ToString(),
            Text = d.DivisionName
        }).ToList();


        var districts = _customerService.GetAllDistrict()
                        .Where(d => d.DivisionID == model.DivisionID)
                        .ToList();

        ViewBag.DistrictList = districts.Select(d => new SelectListItem
        {
            Value = d.DistrictID.ToString(),
            Text = d.DistrictName
        }).ToList();

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var customer = _customerService.GetCustomerDetailsById(id);

        if (customer == null)
            return NotFound();
        return View("Details",customer);
    }
    [HttpPost]
    public IActionResult UpdateBalance(int CustomerID, decimal Balance)
    {
        if (Balance < 0)
        {
            var customer = _customerService.GetCustomerById(CustomerID);
            ModelState.AddModelError("Balance", "Balance cannot be negative");
            return View("Details", customer);
        }

        var existingCustomer = _customerService.GetCustomerById(CustomerID);
        if (existingCustomer == null)
            return NotFound();

        existingCustomer.Balance = Balance;
        _customerService.UpdateCustomer(existingCustomer);

        TempData["Success"] = "Balance updated successfully!";
        return RedirectToAction("Details", new { id = CustomerID });
    }


    public IActionResult Delete(int id)
    {
        var customer = _customerService.GetCustomerById(id);

        if (customer == null)
            return NotFound();

        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        _customerService.DeleteCustomer(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult AddBalance(int CustomerID, decimal AmountToAdd)
    {
        if (AmountToAdd <= 0)
        {
            TempData["Error"] = "Amount must be greater than zero!";
            return RedirectToAction("Details", new { id = CustomerID });
        }

        _customerService.AddBalance(CustomerID, AmountToAdd);

        TempData["Success"] = "Balance updated successfully!";
        return RedirectToAction("Details", new { id = CustomerID });
    }


    [HttpGet]
    public JsonResult GetDistricts(int divisionId)
    {

        var districts = _customerService.GetDistrictByDivisionId(divisionId)
            .Select(d => new
            {
                districtID = d.DistrictID,
                districtName = d.DistrictName
            })
            .ToList();

        return Json(districts);
    }
    public IActionResult Excel_Download(string phone, int? divisionId)
    {

        //================================FOR EXCEL EXPORT REPORT================================
        var sFileName = "Customer_Details" + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

        using (var workbook = new XLWorkbook())
        {
            //===FOR Login History EXCEL EXPORT REPORT===

            var data = _customerService.GetAllSearchCustomer(phone, divisionId);


            var ws = workbook.Worksheets.Add("Report");
            ws.RowHeight = 20;

            var range = ws.Range("A1:K1");
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            range.Style.Border.TopBorderColor = XLColor.Black;
            range.Style.Border.LeftBorderColor = XLColor.Black;
            range.Style.Border.RightBorderColor = XLColor.Black;
            range.Style.Border.BottomBorderColor = XLColor.Black;


            ws.Row(1).Style.Fill.PatternType = XLFillPatternValues.Solid;
            ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromArgb(200, 200, 198);
            ws.Row(1).Height = 20;
            ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Font.FontSize = 12;


            if (data != null)
            {
                var headerColumn = 1;
                ws.Cell(1, headerColumn++).Value = "ID";
                ws.Cell(1, headerColumn++).Value = "Full Name";
                ws.Cell(1, headerColumn++).Value = "Phone";
                ws.Cell(1, headerColumn++).Value = "Email";
                ws.Cell(1, headerColumn++).Value = "NID";
                ws.Cell(1, headerColumn++).Value = "Division";
                ws.Cell(1, headerColumn++).Value = "District";
                ws.Cell(1, headerColumn++).Value = "DOB";
                ws.Cell(1, headerColumn++).Value = "Profession";
                ws.Cell(1, headerColumn++).Value = "Balance";
             
            }

            if (data.Any())
            {
                int rowId = 2;

                foreach (var singleData in data)
                {
                    var DataColumn = 1;
                    ws.Cell(rowId, DataColumn++).Value = singleData.CustomerID;
                    ws.Cell(rowId, DataColumn++).Value = singleData.FullName;
                    ws.Cell(rowId, DataColumn++).Value = singleData.Phone;
                    ws.Cell(rowId, DataColumn++).Value = singleData.Email;
                    ws.Cell(rowId, DataColumn++).Value = singleData.NID;
                    ws.Cell(rowId, DataColumn++).Value = singleData.DivisionName;
                    ws.Cell(rowId, DataColumn++).Value = singleData.DistrictName;
                    ws.Cell(rowId, DataColumn++).Value = singleData.DOB?.ToString("yyyy-MM-dd");
                    ws.Cell(rowId, DataColumn++).Value = singleData.Profession;
                    ws.Cell(rowId, DataColumn++).Value = singleData.Balance;
                  
                   

                    rowId++;
                }
            }


            for (int i = 1; i <= 11; i++)
            {
                ws.Column(i).AdjustToContents();
                ws.Column(i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Column(i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
        }
    }
}

