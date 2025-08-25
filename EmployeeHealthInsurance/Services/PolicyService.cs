using EmployeeHealthInsurance.Data;
using EmployeeHealthInsurance.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly ApplicationDbContext _context;

        public PolicyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Policy>> GetAllPoliciesAsync()
        {
            return await _context.Policies.ToListAsync();
        }

        public async Task<Policy> GetPolicyByIdAsync(int id)
        {
            return await _context.Policies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PolicyId == id);
        }

        public async Task<bool> UpdatePolicyAsync(Policy policyToUpdate)
        {
            if (policyToUpdate == null)
                return false;

            var existingPolicy = await _context.Policies.FindAsync(policyToUpdate.PolicyId);
            if (existingPolicy == null)
                return false;

            existingPolicy.PolicyName = policyToUpdate.PolicyName;
            existingPolicy.CoverageAmount = policyToUpdate.CoverageAmount;
            existingPolicy.PremiumAmount = policyToUpdate.PremiumAmount;

            // Normalize enum for nvarchar column
            string normalizedPolicyType = policyToUpdate.PolicyType.ToString().ToUpper();
            if (Enum.TryParse<PolicyType>(normalizedPolicyType, out var parsedType))
            {
                existingPolicy.PolicyType = parsedType;
            }
            else
            {
                Console.WriteLine($"Invalid PolicyType: {policyToUpdate.PolicyType}");
                return false;
            }

            _context.Entry(existingPolicy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating policy: {ex.Message}");
                return false;
            }

            // ✅ Final fallback return (just in case)
            return false;
        }


        public async Task DeletePolicyAsync(int id)
        {
            var policy = await _context.Policies.FindAsync(id);
            if (policy != null)
            {
                _context.Policies.Remove(policy);
                await _context.SaveChangesAsync();
            }
        }
    }
}
