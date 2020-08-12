using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Hosting.Internal;
using Payroll.Entity;
using Payroll.Models;
using Payroll.Services;
using Payroll.Services.Implementation;

namespace Payroll.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EmployeeController(EmployeeService employeeService , IWebHostEnvironment hostingEnvironment)
        {
            _employeeService = employeeService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var employees = _employeeService.GetAllEmployees().Select(employee => new EmployeeIndexViewModel
            {
                ID = employee.ID,
                EmployeeNo = employee.EmployeeNO,
                ImageUrl = employee.ImageURL,
                FullName = employee.FirstName +( string.IsNullOrEmpty(employee.MiddleName)?" ": " "+employee.MiddleName[0]+"." ).ToUpper()+ employee.LastName,
                Gender = employee.Gender,
                Designation = employee.Designation,
                EmployeeCity = employee.City,
                DateJoined = employee.DateJoined
            }).ToList() ; 
            return View(employees);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new EmployeeCreateViewModel();
            return View(model);
        
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = new Employee
                {
                    ID = model.ID,
                    EmployeeNO = model.EmployeeNO,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    Gender = model.Gender,
                    Email = model.Email,
                    DOB = model.DOB,
                    DateJoined = model.DateJoined,
                    NationalInsuranceNo = model.NationalInsuranceNo,
                    PaymentMethod = model.PaymentMethod,
                    StudentLoan = model.StudentLoan,
                    UnionMember = model.UnionMember,
                    Address = model.Address,
                    City = model.City,
                    Phone = model.Phone,
                    PostCode = model.PostCode,
                    Designation = model.Designation
                    

                };
                if (model.ImageURL != null && model.ImageURL.Length > 0)
                {
                    employee.ImageURL = await FileUrl(Path.GetFileNameWithoutExtension(model.ImageURL.FileName), Path.GetExtension(model.ImageURL.FileName), @"images/employee", _hostingEnvironment.WebRootPath, model.ImageURL);

                }
                await _employeeService.CreateAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View();

        }
        
        public IActionResult Edit(int EmployeeId)
        {
            var employee = _employeeService.GetEmployeebyId(EmployeeId);
            if (employee == null)
            {
                return NotFound();
            }
            var model = new EmployeeEditViewModel()
            {
                ID = employee.ID,
                EmployeeNO = employee.EmployeeNO,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MiddleName = employee.MiddleName,
                Gender = employee.Gender,
                Email = employee.Email,
                DOB = employee.DOB,
               
                NationalInsuranceNo = employee.NationalInsuranceNo,
                PaymentMethod = employee.PaymentMethod,
                StudentLoan = employee.StudentLoan,
                UnionMember = employee.UnionMember,
                Address = employee.Address,
                City = employee.City,
                Phone = employee.Phone,
                PostCode = employee.PostCode,
                Designation = employee.Designation

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = _employeeService.GetEmployeebyId(model.ID);
                if (employee == null)
                {
                    return NotFound();
                }

                employee.EmployeeNO = model.EmployeeNO;
                employee.FirstName = model.FirstName;
                employee.MiddleName = model.MiddleName;
                employee.LastName = model.LastName;
                employee.NationalInsuranceNo = model.NationalInsuranceNo;
                employee.Gender = model.Gender;
                employee.Email = model.Email;
                employee.DOB = model.DOB;
                employee.Phone = model.Phone;
                employee.Designation = model.Designation;
                employee.PaymentMethod = model.PaymentMethod;
                employee.StudentLoan = model.StudentLoan;
                employee.Address = model.Address;
                employee.City = model.City;
                employee.PostCode = model.PostCode;
                if (model.ImageURL != null && model.ImageURL.Length > 0)
                {
                    employee.ImageURL = await FileUrl(Path.GetFileNameWithoutExtension(model.ImageURL.FileName), Path.GetExtension(model.ImageURL.FileName), @"images/employee", _hostingEnvironment.WebRootPath, model.ImageURL);
                }
                await _employeeService.UpdateAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View();
        
        }
        [HttpGet]
        public IActionResult Detail(int ID)
        {
            var employee = _employeeService.GetEmployeebyId(ID);
            if (employee == null)
            {
                return NotFound();
            }
            EmployeeDetailViewModel model = new EmployeeDetailViewModel()
            {
                ID = employee.ID,
                EmployeeNO = employee.EmployeeNO,
                FullName = employee.FirstName + (string.IsNullOrEmpty(employee.MiddleName) ? " " : " " + employee.MiddleName[0] + ".").ToUpper() + employee.LastName,
                Gender = employee.Gender,
                Email = employee.Email,
                DOB = employee.DOB,

                NationalInsuranceNo = employee.NationalInsuranceNo,
                PaymentMethod = employee.PaymentMethod,
                StudentLoan = employee.StudentLoan,
                UnionMember = employee.UnionMember,
                Address = employee.Address,
                City = employee.City,
                Phone = employee.Phone,
                PostCode = employee.PostCode,
                Designation = employee.Designation,
                ImageURL = employee.ImageURL,
                DateJoined = employee.DateJoined

            };
            return View(model);
        }
        public static async Task<string> FileUrl(string FileName, string FileExtension , string UploadDir , string WebRootPath , IFormFile FileUrl)
        {
            FileName = DateTime.UtcNow.ToString("yymmssfff") + FileName + FileExtension;

             var path = Path.Combine(WebRootPath, UploadDir, FileName);
             await FileUrl.CopyToAsync(new FileStream(path, FileMode.Create));
            return "/" + UploadDir + "/" + FileName;
        }
        [HttpGet]
        public IActionResult Delete(int ID)
        {
            var employee = _employeeService.GetEmployeebyId(ID);
            if (employee == null)
            {
                return NotFound();
            }
            var model = new EmployeeDeleteViewModel()
            {

                ID = employee.ID,
                FullName = employee.FirstName + (string.IsNullOrEmpty(employee.MiddleName) ? " " : " " + employee.MiddleName[0] + ".").ToUpper() + employee.LastName

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete( EmployeeDeleteViewModel model)
        {
            await _employeeService.Delete(model.ID);
            return RedirectToAction(nameof(Index));
        }
    }
}
