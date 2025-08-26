using EmployeeHealthInsurance.Models;
using EmployeeHealthInsurance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    // GET: Policy/Delete/5 (Admin only)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var policy = await _policyService.GetPolicyByIdAsync(id);
        if (policy == null)
        {
            return NotFound();
        }
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