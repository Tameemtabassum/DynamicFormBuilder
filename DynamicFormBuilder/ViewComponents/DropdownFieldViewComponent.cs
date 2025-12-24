using Microsoft.AspNetCore.Mvc;
using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.ViewComponents
{
    public class DropdownFieldViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(FormFieldModel field)
        {
            return View(field);
        }
    }
}