using EmployeeHealthInsurance.DTOs;
using EmployeeHealthInsurance.Models;
using EmployeeHealthInsurance.Services;
using EmployeeHealthInsurance.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IOrganizationService _organizationService;
    private readonly UserManager<IdentityUser> _userManager;

    public EmployeeController(
        IEmployeeService employeeService,
        IOrganizationService organizationService,
        UserManager<IdentityUser> userManager)
    {
        _employeeService = employeeService;
        _organizationService = organizationService;
        _userManager = userManager;
    }

    // GET: Employee/Register
    [Authorize(Roles = "HRManager")]
    public async Task<IActionResult> Register()
    {
        var orgs = await _organizationService.GetAllOrganizationsAsync();
        ViewBag.OrganizationList = orgs.Select(o => new SelectListItem
        {
            Value = o.OrganizationId.ToString(),
            Text = o.OrganizationName
        }).ToList();

        return View();
    }

    // POST: Employee/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "HRManager")]
    public async Task<IActionResult> Register(EmployeeDto employeeDto)
    {
        if (!ModelState.IsValid)
        {
            var orgsInvalid = await _organizationService.GetAllOrganizationsAsync();
            ViewBag.OrganizationList = orgsInvalid.Select(o => new SelectListItem
            {
                Value = o.OrganizationId.ToString(),
                Text = o.OrganizationName
            }).ToList();

            return View(employeeDto);
        }

        try
        {
            var employee = await _employeeService.RegisterEmployeeAsync(employeeDto);
            if (employee != null)
            {
                var existingUser = await _userManager.FindByEmailAsync(employee.Email);
                if (existingUser == null)
                {
                    var identityUser = new IdentityUser
                    {
                        UserName = employee.Email,
                        Email = employee.Email,
                        EmailConfirmed = true
                    };
                    var tempPassword = $"Emp#{Guid.NewGuid().ToString("N").Substring(0, 8)}!";
                    var createResult = await _userManager.CreateAsync(identityUser, tempPassword);
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(identityUser, "Employee");
                        TempData["NewEmployeeTempPassword"] = tempPassword;
                    }
                    else
                    {
                        TempData["NewEmployeeTempPassword"] = "Account creation failed";
                    }
                }

                return RedirectToAction("Details", new { id = employee.EmployeeId });
            }

            ModelState.AddModelError("", "Failed to register employee.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        var orgs = await _organizationService.GetAllOrganizationsAsync();
        ViewBag.OrganizationList = orgs.Select(o => new SelectListItem
        {
            Value = o.OrganizationId.ToString(),
            Text = o.OrganizationName
        }).ToList();

        return View(employeeDto);
    }

    // GET: Employee/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetEmployeeDetailsAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var viewModel = new EmployeeViewModel
        {
            EmployeeId = employee.EmployeeId,
            Name = employee.Name,
            Email = employee.Email,
            Phone = employee.Phone,
            Address = employee.Address,
            Designation = employee.Designation,
            OrganizationName = employee.Organization?.OrganizationName
        };

        return View(viewModel);
    }

    // GET: Employee/Edit/{id}
    [Authorize(Roles = "HRManager")]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetEmployeeDetailsAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var dto = new EmployeeDto
        {
            Name = employee.Name,
            Email = employee.Email,
            Phone = employee.Phone,
            Address = employee.Address,
            Designation = employee.Designation,
            OrganizationId = employee.OrganizationId
        };
        return View(dto);
    }

    // POST: Employee/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "HRManager")]
    public async Task<IActionResult> Edit(int id, EmployeeDto employeeDto)
    {
        if (!ModelState.IsValid)
        {
            return View(employeeDto);
        }

        var updated = await _employeeService.UpdateEmployeeProfileAsync(id, employeeDto);
        if (updated == null)
        {
            return NotFound();
        }

        return RedirectToAction("Details", new { id = updated.EmployeeId });
    }

    // GET: Employee/List
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> List()
    {
        var employees = await _employeeService.ListAllEmployeesAsync();
        var employeeViewModels = employees.Select(e => new EmployeeViewModel
        {
            EmployeeId = e.EmployeeId,
            Name = e.Name,
            Email = e.Email,
            Designation = e.Designation,
            OrganizationName = e.Organization?.OrganizationName
        }).ToList();

        return View(employeeViewModels);
    }

    // GET: Employee/Delete/{id}
    [Authorize(Roles = "HRManager")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _employeeService.GetEmployeeDetailsAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var viewModel = new EmployeeViewModel
        {
            EmployeeId = employee.EmployeeId,
            Name = employee.Name,
            Email = employee.Email,
            Designation = employee.Designation,
            OrganizationName = employee.Organization?.OrganizationName
        };

        return View(viewModel);
    }

    // POST: Employee/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "HRManager")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _employeeService.GetEmployeeDetailsAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var identityUser = await _userManager.FindByEmailAsync(employee.Email);
        if (identityUser != null)
        {
            await _userManager.DeleteAsync(identityUser);
        }

        await _employeeService.DeleteEmployeeAsync(id);

        TempData["SuccessMessage"] = $"Employee '{employee.Name}' deleted successfully.";
        return RedirectToAction("List");
    }
}