using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    
        public class EmployeesController : ControllerBase
        {
            private readonly IEmployeeService _employeeService;

            public EmployeesController(IEmployeeService employeeService)
            {
                _employeeService = employeeService;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
            {
                var employees = await _employeeService.GetAllAsync(cancellationToken);
                return Ok(employees);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
            {
                var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
                if (employee == null) return NotFound();
                return Ok(employee);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] CreateUpdateEmployeeDto dto, CancellationToken cancellationToken)
            {
                var created = await _employeeService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateEmployeeDto dto, CancellationToken cancellationToken)
            {
                var updated = await _employeeService.UpdateAsync(id, dto, cancellationToken);
                if (!updated) return NotFound();
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
            {
                var deleted = await _employeeService.DeleteAsync(id, cancellationToken);
                if (!deleted) return NotFound();
                return NoContent();
            }

            // Performance Test (Sequential vs Parallel)
            [HttpGet("performance-test")]
            public async Task<IActionResult> TestPerformance()
            {
                var swSeq = Stopwatch.StartNew();
                await Task.Delay(1500);
                await Task.Delay(1500);
                swSeq.Stop();

                var swPar = Stopwatch.StartNew();
                var task1 = Task.Delay(1500);
                var task2 = Task.Delay(1500);
                await Task.WhenAll(task1, task2);
                swPar.Stop();

                return Ok(new
                {
                    SequentialTimeMs = swSeq.ElapsedMilliseconds,
                    ParallelTimeMs = swPar.ElapsedMilliseconds,
                    Message = "Parallel execution executed independent tasks concurrently."
                });
            }
        }
}
