using EmployeeHealthInsurance.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeeHealthInsurance.DTOs
{
    public class EnrollmentDto
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int PolicyId { get; set; }
    }
}
