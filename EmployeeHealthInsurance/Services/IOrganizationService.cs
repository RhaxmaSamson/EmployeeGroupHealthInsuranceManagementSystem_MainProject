using EmployeeHealthInsurance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetAllOrganizationsAsync();
        Task<Organization> GetOrganizationByIdAsync(int id);
        Task AddOrganizationAsync(Organization organization); // Added for creating an organization
        Task UpdateOrganizationAsync(Organization organization); // Added for updating an organization
        Task DeleteOrganizationAsync(int id); // Added for deleting an organization
    }
}
