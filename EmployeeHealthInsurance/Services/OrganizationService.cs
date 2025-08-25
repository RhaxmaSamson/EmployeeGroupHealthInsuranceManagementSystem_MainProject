using EmployeeHealthInsurance.Data;
using EmployeeHealthInsurance.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeHealthInsurance.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly ApplicationDbContext _context;

        public OrganizationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
        {
            return await _context.Organizations.ToListAsync();
        }

        public async Task<Organization> GetOrganizationByIdAsync(int id)
        {
            return await _context.Organizations.FindAsync(id);
        }

        // Implementation for adding a new organization
        public async Task AddOrganizationAsync(Organization organization)
        {
            _context.Add(organization);
            await _context.SaveChangesAsync();
           
        }

        // Implementation for updating an existing organization
        public async Task UpdateOrganizationAsync(Organization organization)
        {
            _context.Update(organization);
            await _context.SaveChangesAsync();
        }

        // Implementation for deleting an organization
        public async Task DeleteOrganizationAsync(int id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization != null)
            {
                _context.Organizations.Remove(organization);
                await _context.SaveChangesAsync();
            }
        }
    }
}
