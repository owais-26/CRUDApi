using CRUDApi.Data;
using CRUDApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDApi.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class EmployeesController : Controller
    {
        private readonly FullStackDbContext _fullStackDbContext;

        public EmployeesController(FullStackDbContext fullStackDbContext)
        {
            _fullStackDbContext = fullStackDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(
           [FromQuery] DateTime? createdOn,
           [FromQuery] DateTime? lastModifiedOn,
           [FromQuery] string lastModifiedBy,
           [FromQuery] string createdBy)
        {
            var query = _fullStackDbContext.Employees.AsQueryable();

            // Apply filters based on parameters
            if (createdOn.HasValue)
            {
                query = query.Where(e => e.CreatedOn.Date == createdOn.Value.Date);
            }

            if (lastModifiedOn.HasValue)
            {
                query = query.Where(e => e.LastModifiedOn.Date == lastModifiedOn.Value.Date);
            }

            if (!string.IsNullOrEmpty(lastModifiedBy))
            {
                query = query.Where(e => e.LastModifiedBy == lastModifiedBy);
            }

            if (!string.IsNullOrEmpty(createdBy))
            {
                query = query.Where(e => e.CreatedBy == createdBy);
            }

            var employees = await query.ToListAsync();
            return Ok(employees);
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employeeRequest)
        {
            employeeRequest.Id = Guid.NewGuid();
            employeeRequest.CreatedOn = DateTime.UtcNow; // Set created date/time
            employeeRequest.LastModifiedOn = DateTime.UtcNow; // Set last modified date/time
            await _fullStackDbContext.Employees.AddAsync(employeeRequest);
            await _fullStackDbContext.SaveChangesAsync();
            return Ok(employeeRequest);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetEmployee([FromRoute] Guid id)
        {
           var employee = await _fullStackDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if(employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPut]
        [Route("{id:Guid}")]

        public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id , Employee updateEmployeeRequest)
        {
            var employee = await _fullStackDbContext.Employees.FindAsync(id);
            if (employee == null)

            {
                return NotFound();

                
            }
            employee.Name = updateEmployeeRequest.Name;
            employee.Email = updateEmployeeRequest.Email;
            employee.Phone = updateEmployeeRequest.Phone;
            employee.Salary = updateEmployeeRequest.Salary;
            employee.Department = updateEmployeeRequest.Department;
            employee.LastModifiedOn = DateTime.UtcNow; // Update last modified date/time
            employee.LastModifiedBy = updateEmployeeRequest.LastModifiedBy;

           await _fullStackDbContext.SaveChangesAsync();

            return Ok(employee);

        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)

        {
            var employee = await _fullStackDbContext.Employees.FindAsync(id);
            if (employee == null)

            {
                return NotFound();


            }
            _fullStackDbContext.Employees.Remove(employee);
            await _fullStackDbContext.SaveChangesAsync();
            return
            Ok(employee);
        }
    }
}
