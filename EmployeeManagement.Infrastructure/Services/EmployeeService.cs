using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Entites ;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
namespace EmployeeManagement.Infrastructure.Services
{
    public class EmployeeService : IEmployeeService
    {
      
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .AsNoTracking()
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Position = e.Position,
                    Salary = e.Salary
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var e = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (e == null) return null;

            return new EmployeeDto { Id = e.Id, Name = e.Name, Position = e.Position, Salary = e.Salary };
        }

        public async Task<EmployeeDto> CreateAsync(CreateUpdateEmployeeDto dto, CancellationToken cancellationToken = default)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Position = dto.Position,
                Salary = dto.Salary
            };

            await _context.Employees.AddAsync(employee, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Position = employee.Position,
                Salary = employee.Salary
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateUpdateEmployeeDto dto, CancellationToken cancellationToken = default)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (employee == null) return false;

            employee.Name = dto.Name;
            employee.Position = dto.Position;
            employee.Salary = dto.Salary;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}

