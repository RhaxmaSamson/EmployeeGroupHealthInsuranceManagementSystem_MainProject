using System.ComponentModel.DataAnnotations;

namespace EmployeeHealthInsurance.DTOs
{
    public class ClaimDto
    {
        [Required]
        public int EnrollmentId { get; set; }
        [Required]
        public decimal ClaimAmount { get; set; }
        [Required]
        public string ClaimReason { get; set; }
    }
}
