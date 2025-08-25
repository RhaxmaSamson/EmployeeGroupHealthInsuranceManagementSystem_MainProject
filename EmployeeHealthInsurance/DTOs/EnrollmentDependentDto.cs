using EmployeeHealthInsurance.Models;

namespace EmployeeHealthInsurance.DTOs
{
    public class EnrollmentDependentDto
    {
        public int EnrollmentDependentId { get; set; }
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
        public string Name { get; set; }
        public string Relationship { get; set; } // Spouse, Child
        public DateTime DateOfBirth { get; set; }
    }
}
