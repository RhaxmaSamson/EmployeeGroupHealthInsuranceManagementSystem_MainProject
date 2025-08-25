using System.ComponentModel.DataAnnotations;

namespace EmployeeHealthInsurance.DTOs
{
    public class EmployeeDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Designation { get; set; }
        public int OrganizationId { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}

