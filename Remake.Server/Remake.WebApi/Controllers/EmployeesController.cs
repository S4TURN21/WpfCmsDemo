using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Remake.WebApi.Entities.Models;

namespace Remake.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public EmployeesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetEmployees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return employees;
        }

        [HttpGet("{id}", Name = "GetEmployeeById")]
        public async Task<ActionResult<Employee>> GetEmployee(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        [HttpPost(Name = "AddEmployee")]
        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpPut(Name = "UpdateEmployee")]
        public async Task<ActionResult<Employee>> UpdateEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpDelete("{id}", Name = "DeleteEmployee")]
        public async Task<ActionResult> DeleteEmployee(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> TestError()
        {
            await Task.Delay(1000);
            return BadRequest();
        }
    }
}
