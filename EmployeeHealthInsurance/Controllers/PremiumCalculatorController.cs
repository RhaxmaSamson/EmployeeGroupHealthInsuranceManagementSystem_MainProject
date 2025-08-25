using EmployeeHealthInsurance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize(Roles = "Admin,HRManager")]
public class PremiumCalculatorController : Controller
{
    private readonly IPremiumCalculatorService _premiumCalculatorService;

    public PremiumCalculatorController(IPremiumCalculatorService premiumCalculatorService)
    {
        _premiumCalculatorService = premiumCalculatorService;
    }

    // GET: PremiumCalculator/Calculate
    [HttpGet]
    public IActionResult Calculate()
    {
        return View();
    }

    // POST: PremiumCalculator/CalculateResult
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CalculateResult(int policyId, int employeeId, int? age, int? dependents)
    {
        decimal premium;
        if (age.HasValue || dependents.HasValue)
        {
            var resolvedAge = age ?? 30;
            var resolvedDependents = dependents ?? 0;
            premium = await _premiumCalculatorService.CalculatePremiumAsync(policyId, employeeId, resolvedAge, resolvedDependents);
        }
        else
        {
            premium = await _premiumCalculatorService.CalculatePremiumAsync(policyId, employeeId);
        }
        ViewBag.Premium = premium;
        return View("Calculate");
    }
}