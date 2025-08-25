using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace EmployeeHealthInsurance.Models
{

    public enum EnrollmentStatus
    {
        ACTIVE,
        CANCELLED
    }
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public EnrollmentStatus Status { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey("Policy")]
        public int PolicyId { get; set; }
        public Policy Policy { get; set; }

        public ICollection<Claim> Claims { get; set; }
        public ICollection<EnrollmentDependent> Dependents { get; set; } = new List<EnrollmentDependent>();
    }
}
