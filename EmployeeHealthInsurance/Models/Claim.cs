using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeHealthInsurance.Models
{
    public enum ClaimStatus
    {
        SUBMITTED,
        APPROVED,
        REJECTED
    }
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }
        public decimal ClaimAmount { get; set; }
        public string ClaimReason { get; set; }
        public DateTime ClaimDate { get; set; }
        public ClaimStatus ClaimStatus { get; set; }

        [ForeignKey("Enrollment")]
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
    }
}
