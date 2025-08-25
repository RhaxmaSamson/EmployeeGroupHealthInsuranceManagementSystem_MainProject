using EmployeeHealthInsurance.Data;
using EmployeeHealthInsurance.DTOs;
using EmployeeHealthInsurance.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _context;

        public ClaimService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Claim> SubmitClaimAsync(ClaimDto claimDto)
        {
            var claim = new Claim
            {
                EnrollmentId = claimDto.EnrollmentId,
                ClaimAmount = claimDto.ClaimAmount,
                ClaimReason = claimDto.ClaimReason,
                ClaimDate = DateTime.Now, // Set the date in the service layer
                ClaimStatus = ClaimStatus.SUBMITTED
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
            return claim;
        }

        public async Task<Claim> GetClaimDetailsAsync(int claimId)
        {
            return await _context.Claims
                .Include(c => c.Enrollment)
                .FirstOrDefaultAsync(c => c.ClaimId == claimId);
        }

        public async Task<bool> UpdateClaimStatusAsync(int claimId, ClaimStatus status)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
                return false;

            claim.ClaimStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Claim>> ListAllClaimsAsync()
        {
            return await _context.Claims
                .Include(c => c.Enrollment)
                .ThenInclude(e => e.Employee)
                .ToListAsync();
        }
    }
}