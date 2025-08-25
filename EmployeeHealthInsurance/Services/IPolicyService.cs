using EmployeeHealthInsurance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public interface IPolicyService
    {

        Task<IEnumerable<Policy>> GetAllPoliciesAsync();
        Task<Policy> GetPolicyByIdAsync(int id);
        // Changed return type to Task<bool> to indicate success/failure of update
        Task<bool> UpdatePolicyAsync(Policy policy);
        Task DeletePolicyAsync(int id);

    }
}