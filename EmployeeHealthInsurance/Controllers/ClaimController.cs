using EmployeeHealthInsurance.DTOs;
using EmployeeHealthInsurance.Models;
using EmployeeHealthInsurance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Authorize]
public class ClaimController : Controller
{
    private readonly IClaimService _claimService;
    private readonly IEnrollmentService _enrollmentService; // To get enrollment details

    public ClaimController(IClaimService claimService, IEnrollmentService enrollmentService)
    {
        _claimService = claimService;
        _enrollmentService = enrollmentService;
    }

    // GET: Claim/Submit (Employees only)
    [Authorize(Roles = "Employee")]
    public IActionResult Submit()
    {
        return View();
    }

    // POST: Claim/Submit (Employees only)
    [Authorize(Roles = "Employee")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ClaimDto claimDto)
    {
        if (ModelState.IsValid)
        {
            var claim = await _claimService.SubmitClaimAsync(claimDto);
            if (claim != null)
            {
                return RedirectToAction("List"); // Redirect to claims list or success page
            }
            ModelState.AddModelError("", "Failed to submit claim.");
        }
        return View(claimDto);
    }

    // GET: Claim/List (Employees, HR, and Admin can view)
    [Authorize(Roles = "Employee,HRManager,Admin")]
    public async Task<IActionResult> List()
    {
        var claims = await _claimService.ListAllClaimsAsync();
        return View(claims);
    }

    // GET: Claim/Details/{id} (Employees, HR, and Admin can view)
    [Authorize(Roles = "Employee,HRManager,Admin")]
    public async Task<IActionResult> Details(int id)
    {
        var claim = await _claimService.GetClaimDetailsAsync(id);
        if (claim == null)
        {
            return NotFound();
        }
        return View(claim);
    }

    // POST: Claim/UpdateStatus/{id} (HR only)
    [Authorize(Roles = "HRManager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ClaimStatus status)
    {
        var updated = await _claimService.UpdateClaimStatusAsync(id, status);
        if (!updated)
        {
            return NotFound();
        }
        return RedirectToAction("Details", new { id });
    }
}