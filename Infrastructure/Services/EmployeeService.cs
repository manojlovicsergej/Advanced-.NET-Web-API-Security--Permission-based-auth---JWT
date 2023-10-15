using Application.Services;
using Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;

    public EmployeeService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Employee> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<Employee> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<int> DeleteEmployeeAsync(Employee employee, CancellationToken cancellationToken)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee.Id;
    }

    public async Task<Employee> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return employee;
    }

    public async Task<List<Employee>> GetEmployeeListAsync(CancellationToken cancellationToken)
    {
        return await _context.Employees.ToListAsync(cancellationToken);
    }
}