using EmployeeHealthInsurance.DTOs;

using EmployeeHealthInsurance.Models;

using EmployeeHealthInsurance.Services;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;

using System.Collections.Generic;

using System.Linq;

using System.Threading.Tasks;

[Authorize]

public class EnrollmentController : Controller

{

    private readonly IEnrollmentService _enrollmentService;

    private readonly IEmployeeService _employeeService;

    private readonly IPolicyService _policyService;

    public EnrollmentController(IEnrollmentService enrollmentService, IEmployeeService employeeService, IPolicyService policyService)

    {

        _enrollmentService = enrollmentService;

        _employeeService = employeeService;

        _policyService = policyService;

    }

    // GET: Enrollment/Enroll (Employee only)

    [Authorize(Roles = "Employee")]

    public async Task<IActionResult> Enroll()

    {

        var policies = await _policyService.GetAllPoliciesAsync();

        ViewBag.PolicyList = policies.Select(p => new SelectListItem

        {

            Value = p.PolicyId.ToString(),

            Text = p.PolicyName

        }).ToList();

        return View();

    }

    // POST: Enrollment/Enroll (Employee only)

    [HttpPost]

    [ValidateAntiForgeryToken]

    [Authorize(Roles = "Employee")]

    public async Task<IActionResult> Enroll(EnrollmentDto enrollmentDto)

    {

        if (ModelState.IsValid)

        {

            var enrollment = await _enrollmentService.EnrollInPolicyAsync(enrollmentDto);

            if (enrollment != null)

            {

                return RedirectToAction("List");

            }

            ModelState.AddModelError("", "Failed to enroll in policy.");

        }

        return View(enrollmentDto);

    }

    // GET: Enrollment/List (HR and Admin only)

    [Authorize(Roles = "HRManager,Admin")]

    public async Task<IActionResult> List()

    {

        var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();

        return View(enrollments);

    }

    // GET: Enrollment/ByEmployee/{employeeId} (HR and Admin only)

    [Authorize(Roles = "HRManager,Admin")]

    public async Task<IActionResult> ByEmployee(int employeeId)

    {

        var enrollments = await _enrollmentService.GetEnrolledPoliciesAsync(employeeId);

        return View("List", enrollments);

    }

    // POST: Enrollment/Cancel/{id} (Employee only)

    [HttpPost]

    [ValidateAntiForgeryToken]

    [Authorize(Roles = "Employee")]

    public async Task<IActionResult> Cancel(int id)

    {

        var result = await _enrollmentService.CancelEnrollmentAsync(id);

        if (!result)

        {

            return NotFound();

        }

        return RedirectToAction("List");

    }

}

