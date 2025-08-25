using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeHealthInsurance.Models
{
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganizationId { get; set; }

        [Required]
        [Display(Name = "Organization Name")]
        public string OrganizationName { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [EmailAddress]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [Phone]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
