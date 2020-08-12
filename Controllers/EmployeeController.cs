using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payroll.Models;
using Payroll.Services;
using Payroll.Services.Implementation;

namespace Payroll.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(EmployeeService employeeService )
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            var employees = _employeeService.GetAllEmployees().Select(employee => new EmployeeIndexViewModel
            {
                ID = employee.ID,
                EmployeeNo = employee.EmployeeNO,
                ImageUrl = employee.ImageURL,
                FullName = employee.FirstName + employee.MiddleName + employee.LastName,
                Gender = employee.Gender,
                Designation = employee.Designation,
                EmployeeCity = employee.City,
                DateJoined = employee.DateJoined
            }).ToList() ; 
            return View(employees);
        }
    }
}
