using EmployeeHealthInsurance.Data;
using EmployeeHealthInsurance.DTOs;
using EmployeeHealthInsurance.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment> EnrollInPolicyAsync(EnrollmentDto enrollmentDto)
        {
            var enrollment = new Enrollment
            {
                EmployeeId = enrollmentDto.EmployeeId,
                PolicyId = enrollmentDto.PolicyId,
                EnrollmentDate = DateTime.Now, // Set the date in the service layer
                Status = EnrollmentStatus.ACTIVE
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<IEnumerable<Enrollment>> GetEnrolledPoliciesAsync(int employeeId)
        {
            return await _context.Enrollments
                .Where(e => e.EmployeeId == employeeId)
                .Include(e => e.Policy)
                .Include(e => e.Employee)
                .ToListAsync();
        }

        public async Task<bool> CancelEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _context.Enrollments.FindAsync(enrollmentId);
            if (enrollment == null)
                return false;

            enrollment.Status = EnrollmentStatus.CANCELLED;
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
        {
            // Use .Include() to fetch related data for display in the view
            return await _context.Enrollments
                                 .Include(e => e.Employee)
                                 .Include(e => e.Policy)
                                 .ToListAsync();
        }
    }
}