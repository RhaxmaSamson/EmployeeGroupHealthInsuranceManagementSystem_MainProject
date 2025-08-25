using EmployeeHealthInsurance.DTOs;
using EmployeeHealthInsurance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public interface IClaimService
    {
        Task<Claim> SubmitClaimAsync(ClaimDto claimDto);
        Task<Claim> GetClaimDetailsAsync(int claimId);
        Task<bool> UpdateClaimStatusAsync(int claimId, ClaimStatus status);
        Task<IEnumerable<Claim>> ListAllClaimsAsync();
    }
}