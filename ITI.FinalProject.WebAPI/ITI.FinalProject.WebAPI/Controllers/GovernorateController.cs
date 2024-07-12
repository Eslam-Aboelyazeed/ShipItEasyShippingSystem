using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces.ApplicationServices;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using ITI.FinalProject.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace ITI.FinalProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernorateController : ControllerBase
    {
        private readonly IPaginationService<Governorate, GovernorateDTO, GovernorateInsertDTO, GovernorateUpdateDTO, int> service;
        private readonly RoleManager<ApplicationRoles> roleManager;
        private readonly IDropDownOptionsService<Governorate, int> optionsService;

        public GovernorateController(
            IPaginationService<Governorate, GovernorateDTO, GovernorateInsertDTO, GovernorateUpdateDTO, int> service, RoleManager<ApplicationRoles> roleManager,
            IDropDownOptionsService<Governorate, int> optionsService
            )
        {
            this.service = service;
            this.roleManager = roleManager;
            this.optionsService = optionsService;
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns governorate options",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of options", Type = typeof(List<OptionDTO<int>>))]
        [HttpGet("/api/governorateOptions")]
        public async Task<ActionResult<List<OptionDTO<int>>>> GetOptions()
        {
            var roles = roleManager.Roles.ToList();
            if (roles.Where(r => r.Name == User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value).Count() == 0)
            {
                return Unauthorized();
            }

            var options = await optionsService.GetOptions(o => o.status == Status.Active);

            return Ok(options);
        }

        // GET: api/Governorate
        [SwaggerOperation(
        Summary = "This Endpoint returns a list of governorates",
            Description = ""
        )]
        [SwaggerResponse(404, "There weren't any governorates in the database", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of governorates", Type = typeof(List<GovernorateDTO>))]
        [HttpGet]
        public async Task<ActionResult<List<GovernorateDTO>>> GetAllGovernorates()
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var governorates = await service.GetAllObjects();

            if (governorates == null || governorates.Count == 0)
            {
                return NotFound();
            }

            return Ok(governorates);
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns a list of governorates with the specified page size",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of governorates", Type = typeof(PaginationDTO<GovernorateDTO>))]
        [HttpGet("/api/GovernoratePage")]
        public async Task<ActionResult<PaginationDTO<GovernorateDTO>>> GetPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = "")
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var paginationDTO = await service.GetPaginatedOrders(pageNumber, pageSize, g => g.name.Trim().ToLower().Contains(name.Trim().ToLower()));

            return Ok(paginationDTO);
        }

        // GET api/Governorate/5
        [SwaggerOperation(
        Summary = "This Endpoint returns the specified governorate",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns the specified governorate", Type = typeof(GovernorateDTO))]
        [HttpGet("{id}")]
        public async Task<ActionResult<GovernorateDTO>> GetGovernorateById(int id)
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var governorate = await service.GetObject(g => g.id == id);

            if (governorate == null)
            {
                return NotFound();
            }

            return Ok(governorate);
        }

        // POST api/Governorate
        [SwaggerOperation(
        Summary = "This Endpoint inserts a governorate element in the db",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the governorate was inserted successfully", Type = typeof(void))]
        [HttpPost]
        public async Task<IActionResult> PostGovernorate([FromBody] GovernorateInsertDTO governorateInsertDTO)
        {
            if (await CheckRole(PowerTypes.Create))
            {
                return Unauthorized();
            }

            var result = await service.InsertObject(governorateInsertDTO);

            if (result.Succeeded)
            {
                if (await service.SaveChangesForObject())
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = "Error saving changes" });
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = result.Message ?? "Something went wrong, please try again later" });
        }

        // PUT api/Governorate/5
        [SwaggerOperation(
        Summary = "This Endpoint updates the specified governorate",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(400, "The id that was given doesn't equal the id in the given governorate object", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the governorate was updated successfully", Type = typeof(void))]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGovernorate(int id, [FromBody] GovernorateUpdateDTO governorateUpdateDTO)
        {
            if (await CheckRole(PowerTypes.Update))
            {
                return Unauthorized();
            }

            if (id != governorateUpdateDTO.id)
            {
                return BadRequest(new ErrorDTO() { Message = "Id doesn't match the id in the object" });
            }

            var governorate = await service.GetObjectWithoutTracking(g => g.id == id);

            if (governorate == null)
            {
                return NotFound(new ErrorDTO() { Message = "Governorate doesn't exist in the db" });
            }

            var result = await service.UpdateObject(governorateUpdateDTO);

            if (result.Succeeded)
            {
                if (await service.SaveChangesForObject())
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = "Error saving changes" });
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = result.Message ?? "Something went wrong, please try again later" });
        }

        // DELETE api/Governorate/5
        [SwaggerOperation(
        Summary = "This Endpoint deletes the specified governorate from the db",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the governorate was deleted successfully", Type = typeof(void))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGovernorate(int id)
        {
            if (await CheckRole(PowerTypes.Delete))
            {
                return Unauthorized();
            }

            var governorate = await service.GetObjectWithoutTracking(g => g.id == id);

            if (governorate == null)
            {
                return NotFound(new ErrorDTO() { Message = "Governorate doesn't exist in the db" });
            }

            var result = await service.DeleteObject(id);

            if (result.Succeeded)
            {
                if (await service.SaveChangesForObject())
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = "Error saving changes" });
                }
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
