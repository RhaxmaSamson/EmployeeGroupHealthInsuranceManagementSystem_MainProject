using EmployeeHealthInsurance.Data;
using System.Threading.Tasks;
using EmployeeHealthInsurance.Models;

namespace EmployeeHealthInsurance.Services
{
    public class PremiumCalculatorService : IPremiumCalculatorService
    {
        private readonly ApplicationDbContext _context;

        public PremiumCalculatorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> CalculatePremiumAsync(int policyId, int employeeId)
        {
            // Backward-compatible default calculation
            var policy = await _context.Policies.FindAsync(policyId);
            if (policy == null) return 0;
            var basePremium = policy.PremiumAmount;
            var policyTypeFactor = policy.PolicyType == PolicyType.FAMILY ? 1.5m : 1.0m;
            return decimal.Round(basePremium * policyTypeFactor, 2);
        }

        public async Task<decimal> CalculatePremiumAsync(int policyId, int employeeId, int age, int dependents)
        {
            var policy = await _context.Policies.FindAsync(policyId);
            if (policy == null) return 0;

            var basePremium = policy.PremiumAmount;

            // Age factor bands
            decimal ageFactor = 1.0m;
            if (age >= 30 && age <= 45) ageFactor = 1.2m;
            else if (age > 45) ageFactor = 1.5m;

            // Dependents factor (applies mainly to FAMILY policies)
            var normalizedDependents = dependents < 0 ? 0 : dependents;
            var dependentsCap = 4; // cap to avoid runaway costs
            var effectiveDependents = normalizedDependents > dependentsCap ? dependentsCap : normalizedDependents;
            decimal dependentsFactor = policy.PolicyType == PolicyType.FAMILY ? (1.0m + (effectiveDependents * 0.15m)) : 1.0m;

            // Policy type factor
            decimal policyTypeFactor = policy.PolicyType == PolicyType.FAMILY ? 1.2m : 1.0m;

            var premium = basePremium * ageFactor * dependentsFactor * policyTypeFactor;
            return decimal.Round(premium, 2);
        }
    }
}