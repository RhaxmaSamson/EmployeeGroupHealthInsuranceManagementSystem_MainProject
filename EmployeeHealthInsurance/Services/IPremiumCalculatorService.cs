using EmployeeHealthInsurance.Services;
using System.Threading.Tasks;
using EmployeeHealthInsurance.Models;

namespace EmployeeHealthInsurance.Services
{
    public interface IPremiumCalculatorService
    {
        Task<decimal> CalculatePremiumAsync(int policyId, int employeeId);
        Task<decimal> CalculatePremiumAsync(int policyId, int employeeId, int age, int dependents);
    }
}