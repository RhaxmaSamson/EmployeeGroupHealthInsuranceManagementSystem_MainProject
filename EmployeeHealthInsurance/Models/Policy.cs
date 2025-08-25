using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeHealthInsurance.Models
{
    public enum PolicyType
    {
        INDIVIDUAL,
        FAMILY
    }
    public class Policy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PolicyId { get; set; }
        [Required]
        public string PolicyName { get; set; }
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public PolicyType PolicyType { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
