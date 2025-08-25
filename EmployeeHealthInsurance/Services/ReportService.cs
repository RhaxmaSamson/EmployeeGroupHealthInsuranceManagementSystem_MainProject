using EmployeeHealthInsurance.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;
using System.Linq;
using EmployeeHealthInsurance.Models;

namespace EmployeeHealthInsurance.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MemoryStream> GenerateEmployeeReportAsync()
        {
            var employees = await _context.Employees.Include(e => e.Organization).ToListAsync();

            var stream = new MemoryStream();

            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);

                    page.Header().Text("Employee Report").SemiBold().FontSize(20);

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Name").SemiBold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Email").SemiBold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Designation").SemiBold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Organization").SemiBold();
                            });

                            foreach (var e in employees)
                            {
                                table.Cell().Padding(5).Text(e.Name ?? string.Empty);
                                table.Cell().Padding(5).Text(e.Email ?? string.Empty);
                                table.Cell().Padding(5).Text(e.Designation ?? string.Empty);
                                table.Cell().Padding(5).Text(e.Organization?.OrganizationName ?? string.Empty);
                            }
                        });
                    });

                    page.Footer().AlignRight().Text(txt =>
                    {
                        txt.DefaultTextStyle(t => t.FontSize(10));
                        txt.Span("Generated on: ");
                        txt.Span(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    });
                });
            }).GeneratePdf(stream);

            stream.Position = 0;
            return stream;
        }

        public async Task<MemoryStream> GeneratePolicyReportAsync()
        {
            var policies = await _context.Policies
                .Include(p => p.Enrollments)
                .ThenInclude(e => e.Employee)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var wsPolicies = workbook.Worksheets.Add("Policies");
            wsPolicies.Cell(1, 1).Value = "Policy Name";
            wsPolicies.Cell(1, 2).Value = "Type";
            wsPolicies.Cell(1, 3).Value = "Coverage";
            wsPolicies.Cell(1, 4).Value = "Premium";
            wsPolicies.Cell(1, 5).Value = "Enrollments";

            int row = 2;
            foreach (var p in policies)
            {
                wsPolicies.Cell(row, 1).Value = p.PolicyName;
                wsPolicies.Cell(row, 2).Value = p.PolicyType.ToString();
                wsPolicies.Cell(row, 3).Value = p.CoverageAmount;
                wsPolicies.Cell(row, 4).Value = p.PremiumAmount;
                wsPolicies.Cell(row, 5).Value = p.Enrollments?.Count ?? 0;
                row++;
            }

            var wsEnrollments = workbook.Worksheets.Add("Enrollments");
            wsEnrollments.Cell(1, 1).Value = "Enrollment Id";
            wsEnrollments.Cell(1, 2).Value = "Policy";
            wsEnrollments.Cell(1, 3).Value = "Employee";
            wsEnrollments.Cell(1, 4).Value = "Status";
            wsEnrollments.Cell(1, 5).Value = "Date";

            row = 2;
            foreach (var p in policies)
            {
                if (p.Enrollments == null) continue;
                foreach (var e in p.Enrollments)
                {
                    wsEnrollments.Cell(row, 1).Value = e.EnrollmentId;
                    wsEnrollments.Cell(row, 2).Value = p.PolicyName;
                    wsEnrollments.Cell(row, 3).Value = e.Employee?.Name ?? string.Empty;
                    wsEnrollments.Cell(row, 4).Value = e.Status.ToString();
                    wsEnrollments.Cell(row, 5).Value = e.EnrollmentDate;
                    row++;
                }
            }

            wsPolicies.Columns().AdjustToContents();
            wsEnrollments.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<MemoryStream> GenerateEnrollmentReportAsync()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Employee)
                .Include(e => e.Policy)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Enrollments");
            ws.Cell(1, 1).Value = "Enrollment Id";
            ws.Cell(1, 2).Value = "Employee";
            ws.Cell(1, 3).Value = "Policy";
            ws.Cell(1, 4).Value = "Status";
            ws.Cell(1, 5).Value = "Enrollment Date";

            int row = 2;
            foreach (var e in enrollments)
            {
                ws.Cell(row, 1).Value = e.EnrollmentId;
                ws.Cell(row, 2).Value = e.Employee?.Name ?? string.Empty;
                ws.Cell(row, 3).Value = e.Policy?.PolicyName ?? string.Empty;
                ws.Cell(row, 4).Value = e.Status.ToString();
                ws.Cell(row, 5).Value = e.EnrollmentDate;
                row++;
            }

            ws.Columns().AdjustToContents();
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<MemoryStream> GenerateClaimReportAsync()
        {
            var claims = await _context.Claims
                .Include(c => c.Enrollment)
                .ThenInclude(e => e.Employee)
                .Include(c => c.Enrollment)
                .ThenInclude(e => e.Policy)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Claims");
            ws.Cell(1, 1).Value = "Claim Id";
            ws.Cell(1, 2).Value = "Employee";
            ws.Cell(1, 3).Value = "Policy";
            ws.Cell(1, 4).Value = "Amount";
            ws.Cell(1, 5).Value = "Reason";
            ws.Cell(1, 6).Value = "Date";
            ws.Cell(1, 7).Value = "Status";

            int row = 2;
            foreach (var c in claims)
            {
                ws.Cell(row, 1).Value = c.ClaimId;
                ws.Cell(row, 2).Value = c.Enrollment?.Employee?.Name ?? string.Empty;
                ws.Cell(row, 3).Value = c.Enrollment?.Policy?.PolicyName ?? string.Empty;
                ws.Cell(row, 4).Value = c.ClaimAmount;
                ws.Cell(row, 5).Value = c.ClaimReason ?? string.Empty;
                ws.Cell(row, 6).Value = c.ClaimDate;
                ws.Cell(row, 7).Value = c.ClaimStatus.ToString();
                row++;
            }

            ws.Columns().AdjustToContents();
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<MemoryStream> GenerateOrganizationReportAsync()
        {
            var orgs = await _context.Organizations
                .Include(o => o.Employees)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Organizations");
            ws.Cell(1, 1).Value = "Organization";
            ws.Cell(1, 2).Value = "Contact Person";
            ws.Cell(1, 3).Value = "Contact Email";
            ws.Cell(1, 4).Value = "Employee Count";

            int row = 2;
            foreach (var o in orgs)
            {
                ws.Cell(row, 1).Value = o.OrganizationName;
                ws.Cell(row, 2).Value = o.ContactPerson ?? string.Empty;
                ws.Cell(row, 3).Value = o.ContactEmail ?? string.Empty;
                ws.Cell(row, 4).Value = o.Employees?.Count ?? 0;
                row++;
            }

            ws.Columns().AdjustToContents();
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<MemoryStream> GeneratePremiumCalculatorReportAsync()
        {
            // This sample compiles a pricing matrix from Policies (as base premiums) and outputs for INDIVIDUAL vs FAMILY
            var policies = await _context.Policies.ToListAsync();

            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Premium Matrix");
            ws.Cell(1, 1).Value = "Policy";
            ws.Cell(1, 2).Value = "Type";
            ws.Cell(1, 3).Value = "Base Premium";
            ws.Cell(1, 4).Value = "Age 25 (Indiv)";
            ws.Cell(1, 5).Value = "Age 35 (Family, 1 Dep)";
            ws.Cell(1, 6).Value = "Age 50 (Family, 2 Dep)";

            int row = 2;
            foreach (var p in policies)
            {
                ws.Cell(row, 1).Value = p.PolicyName;
                ws.Cell(row, 2).Value = p.PolicyType.ToString();
                ws.Cell(row, 3).Value = p.PremiumAmount;

                decimal sample1 = p.PremiumAmount * (p.PolicyType == PolicyType.FAMILY ? 1.2m : 1.0m); // baseline
                decimal age25Ind = decimal.Round(sample1 * 1.0m, 2);
                decimal age35Fam1 = decimal.Round(p.PremiumAmount * 1.2m * 1.15m * (p.PolicyType == PolicyType.FAMILY ? 1.2m : 1.0m), 2);
                decimal age50Fam2 = decimal.Round(p.PremiumAmount * 1.5m * (1 + (2 * 0.15m)) * (p.PolicyType == PolicyType.FAMILY ? 1.2m : 1.0m), 2);

                ws.Cell(row, 4).Value = age25Ind;
                ws.Cell(row, 5).Value = age35Fam1;
                ws.Cell(row, 6).Value = age50Fam2;
                row++;
            }

            ws.Columns().AdjustToContents();
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}