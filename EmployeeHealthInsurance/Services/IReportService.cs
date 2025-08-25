using System.IO;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public interface IReportService
    {
        Task<MemoryStream> GenerateEmployeeReportAsync();
        Task<MemoryStream> GeneratePolicyReportAsync();
        Task<MemoryStream> GenerateEnrollmentReportAsync();
        Task<MemoryStream> GenerateClaimReportAsync();
        Task<MemoryStream> GenerateOrganizationReportAsync();
        Task<MemoryStream> GeneratePremiumCalculatorReportAsync();
    }
}