using Domain;

namespace Application.Services;

public interface IEmployeeService
{
    Task<Employee> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken);
    Task<Employee> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken);
    Task<int> DeleteEmployeeAsync(Employee employee, CancellationToken cancellationToken);
    Task<Employee> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<Employee>> GetEmployeeListAsync(CancellationToken cancellationToken);
}