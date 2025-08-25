
using EmployeeHealthInsurance.DTOs;
using EmployeeHealthInsurance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace EmployeeHealthInsurance.Services
{
    public interface IEmployeeService
    {
        Task<Employee> RegisterEmployeeAsync(EmployeeDto employeeDto);
        Task<Employee> GetEmployeeDetailsAsync(int employeeId);
        Task<Employee> UpdateEmployeeProfileAsync(int employeeId, EmployeeDto employeeDto);
        Task<IEnumerable<Employee>> ListAllEmployeesAsync();
        Task<bool> DeleteEmployeeAsync(int employeeId); // ✅ New method
    }
}