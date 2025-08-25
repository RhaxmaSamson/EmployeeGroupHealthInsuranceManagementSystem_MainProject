using EmployeeHealthInsurance.Models;
using EmployeeHealthInsurance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Authorize(Roles = "Admin,HRManager")]
public class OrganizationController : Controller
{
    private readonly IOrganizationService _organizationService;

    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    // GET: Organization/Index
    public async Task<IActionResult> Index()
    {
        var organizations = await _organizationService.GetAllOrganizationsAsync();
        return View(organizations);
    }

    // GET: Organization/Create
    [Authorize(Roles = "Admin")] // Only Admin can create organizations
    public IActionResult Create()
    {
        return View();
    }

    // POST: Organization/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    // Ensured ContactPhone and Address are included in the bind properties
    public async Task<IActionResult> Create([Bind("OrganizationName,ContactPerson,ContactEmail,ContactPhone,Address")] Organization organization)
    {
        if (ModelState.IsValid)
        {
            await _organizationService.AddOrganizationAsync(organization);
            TempData["SuccessMessage"] = "Organization created successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(organization);
    }

    // GET: Organization/Edit/5
    [Authorize(Roles = "Admin")] // Only Admin can edit organizations
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var organization = await _organizationService.GetOrganizationByIdAsync(id.Value);
        if (organization == null)
        {
            return NotFound();
        }
        return View(organization);
    }

    // POST: Organization/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    // Ensured ContactPhone and Address are included in the bind properties
    public async Task<IActionResult> Edit(int id, [Bind("OrganizationId,OrganizationName,ContactPerson,ContactEmail,ContactPhone,Address")] Organization organization)
    {
        if (id != organization.OrganizationId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _organizationService.UpdateOrganizationAsync(organization);
                TempData["SuccessMessage"] = "Organization updated successfully!";
            }
            catch (Exception) // Catch a more specific exception if needed, e.g., DbUpdateConcurrencyException
            {
                if (await _organizationService.GetOrganizationByIdAsync(organization.OrganizationId) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(organization);
    }

    // GET: Organization/Delete/5
    [Authorize(Roles = "Admin")] // Only Admin can delete organizations
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var organization = await _organizationService.GetOrganizationByIdAsync(id.Value);
        if (organization == null)
        {
            return NotFound();
        }

        return View(organization);
    }

    // POST: Organization/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _organizationService.DeleteOrganizationAsync(id);
        TempData["SuccessMessage"] = "Organization deleted successfully!";
        return RedirectToAction(nameof(Index));
    }
}
