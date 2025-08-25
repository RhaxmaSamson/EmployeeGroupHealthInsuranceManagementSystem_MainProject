using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeHealthInsurance.Services; // Assuming you have a ReportService

[Authorize]
public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: Report/Index
    [Authorize(Roles = "Admin,HRManager")]
    public IActionResult Index()
    {
        return View();
    }

    // GET: Report/Employee
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> Employee()
    {
        var stream = await _reportService.GenerateEmployeeReportAsync();
        stream.Position = 0;
        return File(stream, "application/pdf", "EmployeeReport.pdf");
    }

    // GET: Report/Policy
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> Policy()
    {
        var stream = await _reportService.GeneratePolicyReportAsync();
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PolicyReport.xlsx");
    }

    // GET: Report/Enrollment
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> Enrollment()
    {
        var stream = await _reportService.GenerateEnrollmentReportAsync();
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EnrollmentReport.xlsx");
    }

    // GET: Report/Claims
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> Claims()
    {
        var stream = await _reportService.GenerateClaimReportAsync();
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ClaimReport.xlsx");
    }

    // GET: Report/Organizations
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> Organizations()
    {
        var stream = await _reportService.GenerateOrganizationReportAsync();
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrganizationReport.xlsx");
    }

    // GET: Report/PremiumCalculator
    [Authorize(Roles = "Admin,HRManager")]
    public async Task<IActionResult> PremiumCalculator()
    {
        var stream = await _reportService.GeneratePremiumCalculatorReportAsync();
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PremiumCalculatorReport.xlsx");
    }
}