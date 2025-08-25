
using EmployeeHealthInsurance.Data;
using EmployeeHealthInsurance.DTOs;
using EmployeeHealthInsurance.Models;
using EmployeeHealthInsurance.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace EmployeeHealthInsurance.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> RegisterEmployeeAsync(EmployeeDto employeeDto)
        {
            if (employeeDto == null)
                throw new ArgumentNullException(nameof(employeeDto));
            if (string.IsNullOrWhiteSpace(employeeDto.Name) || string.IsNullOrWhiteSpace(employeeDto.Email))
                throw new ArgumentException("Name and Email are required");

            var employee = new Employee
            {
                Name = employeeDto.Name,
                Email = employeeDto.Email,
                Phone = employeeDto.Phone,
                Address = employeeDto.Address,
                Designation = employeeDto.Designation,
                OrganizationId = employeeDto.OrganizationId
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> GetEmployeeDetailsAsync(int employeeId)
        {
            return await _context.Employees
                                 .Include(e => e.Organization)
                                 .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee> UpdateEmployeeProfileAsync(int employeeId, EmployeeDto employeeDto)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return null;

            employee.Name = employeeDto.Name;
            employee.Email = employeeDto.Email;
            employee.Phone = employeeDto.Phone;
            employee.Address = employeeDto.Address;
            employee.Designation = employeeDto.Designation;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<IEnumerable<Employee>> ListAllEmployeesAsync()
        {
            return await _context.Employees
                                 .Include(e => e.Organization)
                                 .ToListAsync();
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}