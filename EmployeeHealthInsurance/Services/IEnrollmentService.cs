using EmployeeHealthInsurance.Models;
using EmployeeHealthInsurance.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public interface IEnrollmentService
    {
        Task<Enrollment> EnrollInPolicyAsync(EnrollmentDto enrollmentDto);
        Task<IEnumerable<Enrollment>> GetEnrolledPoliciesAsync(int employeeId);
        Task<bool> CancelEnrollmentAsync(int enrollmentId);
        Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync(); // Add this line
    }
}