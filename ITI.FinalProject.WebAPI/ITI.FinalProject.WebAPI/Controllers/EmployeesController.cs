using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces;
using Application.Interfaces.ApplicationServices;
using Domain.Entities;
using Domain.Enums;
using ITI.FinalProject.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Transactions;

namespace ITI.FinalProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IPaginationService<Employee, EmployeeReadDto, EmployeeAddDto, EmployeeupdateDto, string> employeeService;
        private readonly RoleManager<ApplicationRoles> roleManager;

        public EmployeesController(IPaginationService<Employee, EmployeeReadDto, EmployeeAddDto, EmployeeupdateDto, string> employeeService, RoleManager<ApplicationRoles> roleManager)
        {
            this.employeeService = employeeService;
            this.roleManager = roleManager;
        }
        //GET
        [SwaggerOperation(
        Summary = "This Endpoint returns a list of Employees",
        Description = ""
        )]
        [SwaggerResponse(404, "There weren't any employees in the database", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns a list of employees", Type = typeof(IEnumerable<EmployeeReadDto>))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetAll()
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var employees = await employeeService.GetAllObjects(e => e.user);

            if (employees == null || !employees.Any())
            {
                return NotFound();
            }

            return Ok(employees);
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns a list of employees with the specified page size",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of employees", Type = typeof(PaginationDTO<EmployeeReadDto>))]
        [HttpGet("/api/EmployeePage")]
        public async Task<ActionResult<PaginationDTO<EmployeeReadDto>>> GetPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = "")
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var paginationDTO = await employeeService.GetPaginatedOrders(pageNumber, pageSize, e =>  1 == 1 );
            paginationDTO.List = paginationDTO.List.Where(e => e.FullName.Trim().ToLower().Contains(name.Trim().ToLower())).ToList();
            
            return Ok(paginationDTO);
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns the specified employee",
        Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns the specified employee", Type = typeof(EmployeeReadDto))]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<EmployeeReadDto>> GetById(string id)
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            EmployeeReadDto? employeeReadDto = await employeeService.GetObject(e => e.userId == id, e => e.user);
            if (employeeReadDto == null)
            {
                return NotFound();
            }
            return Ok(employeeReadDto);
        }
        
        [SwaggerOperation(
        Summary = "This Endpoint inserts an employee element in the db",
        Description = ""
        )]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(204, "Confirms that the employee was inserted successfully", Type = typeof(void))]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] EmployeeAddDto employeeAddDto)
        {
            if (await CheckRole(PowerTypes.Create))
            {
                return Unauthorized();
            }

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                var result = await employeeService.InsertObject(employeeAddDto);
                transaction.Complete();

                if (result.Succeeded)
                {
                    return NoContent();
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = result.Message ?? "Something went wrong, please try again later" });
            }

        }
        
        [SwaggerOperation(
        Summary = "This Endpoint deletes the specified employee from the db",
        Description = ""
        )]
        [SwaggerResponse(400, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(204, "Confirms that the employee was deleted successfully", Type = typeof(void))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if (await CheckRole(PowerTypes.Delete))
            {
                return Unauthorized();
            }

            try
            {
                await employeeService.DeleteObject(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Message = ex.Message });
            }
        }

        [SwaggerOperation(
        Summary = "This Endpoint updates the specified employee",
        Description = ""
        )]
        [SwaggerResponse(400, "The id that was given doesn't equal the id in the given employee object", Type = typeof(ErrorDTO))]
        [SwaggerResponse(404, "Employee doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the employee was updated successfully", Type = typeof(void))]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] EmployeeupdateDto employeeupdateDto)
        {
            if (await CheckRole(PowerTypes.Update))
            {
                return Unauthorized();
            }

            if (id != employeeupdateDto.Id)
            {
                return BadRequest(new ErrorDTO() { Message = "Id doesn't match the id in the object" });
            }

            var employee = await employeeService.GetObjectWithoutTracking(e => e.userId == id, e => e.user);

            if (employee == null)
            {
                return NotFound(new ErrorDTO() { Message = "Employee doesn't exist in the db" });
            }

            var result = await employeeService.UpdateObject(employeeupdateDto);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = result.Message ?? "Something went wrong, please try again later" });
        }

        private async Task<bool> CheckRole(PowerTypes powerType)
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == null)
            {
                return true;
            }

            if (role == "Admin")
            {
                return false;
            }

            var rolePowers = await roleManager.Roles.Include(r => r.RolePowers).Where(r => r.Name == role).FirstOrDefaultAsync();

            if (rolePowers == null)
            {
                return true;
            }

            string controllerName = ControllerContext.ActionDescriptor.ControllerName;

            switch (powerType)
            {
                case PowerTypes.Create:
                    if ((!rolePowers.RolePowers.FirstOrDefault(rp => rp.TableName.ToString() == controllerName)?.Create) ?? true)
                    {
                        return true;
                    }
                    break;
                case PowerTypes.Read:
                    if ((!rolePowers.RolePowers.FirstOrDefault(rp => rp.TableName.ToString() == controllerName)?.Read) ?? true)
                    {
                        return true;
                    }
                    break;
                case PowerTypes.Update:
                    if ((!rolePowers.RolePowers.FirstOrDefault(rp => rp.TableName.ToString() == controllerName)?.Update) ?? true)
                    {
                        return true;
                    }
                    break;
                case PowerTypes.Delete:
                    if ((!rolePowers.RolePowers.FirstOrDefault(rp => rp.TableName.ToString() == controllerName)?.Delete) ?? true)
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}
