using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeHealthInsurance.DTOs;
using System.Collections.Generic;

namespace EmployeeHealthInsurance.ViewModels
{
    public class RegisterEmployeeViewModel
    {
        public EmployeeDto Employee { get; set; }
        public List<SelectListItem> Organizations { get; set; }
    }
}