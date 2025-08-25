using EmployeeHealthInsurance.Models;
using EmployeeHealthInsurance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[Authorize(Roles = "Admin,HRManager,Employee")]
public class PolicyController : Controller
{
    private readonly IPolicyService _policyService;

    public PolicyController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    // GET: Policy/Index
    public async Task<IActionResult> Index()
    {
        var policies = await _policyService.GetAllPoliciesAsync();
        return View(policies);
    }

    // GET: Policy/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var policy = await _policyService.GetPolicyByIdAsync(id);

        if (policy == null)
        {
            return NotFound();
        }

        return View(policy);
    }

    // GET: Policy/Edit/5 (Admin only)

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var policy = await _policyService.GetPolicyByIdAsync(id);
        if (policy == null)
        {
            return NotFound();
        }
        return View(policy);
    }

    // POST: Policy/Edit/5 (Admin only)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Policy policy)
    {
        if (!ModelState.IsValid)
        {
            return View(policy);
        }

        var updateResult = await _policyService.UpdatePolicyAsync(policy);
        if (updateResult)
        {
            TempData["SuccessMessage"] = "Policy updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to update policy.");
        return View(policy);
    }


    // POST: Policy/Delete (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _policyService.DeletePolicyAsync(id);
        return RedirectToAction(nameof(Index));
    }
}