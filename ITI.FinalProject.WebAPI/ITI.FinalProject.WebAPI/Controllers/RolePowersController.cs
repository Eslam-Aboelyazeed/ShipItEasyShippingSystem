using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces.ApplicationServices;
using Application.Services;
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

namespace ITI.FinalProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePowersController : ControllerBase
    {
        private readonly IPaginationService<RolePowers, RolePowersDTO, RolePowersInsertDTO, RolePowersUpdateDTO, string> service;
        private readonly RoleManager<ApplicationRoles> roleManager;
        private readonly IDropDownOptionsService<ApplicationRoles, string> optionsService;

        public RolePowersController(IPaginationService<RolePowers, RolePowersDTO, RolePowersInsertDTO, RolePowersUpdateDTO, string> service, RoleManager<ApplicationRoles> roleManager,
            IDropDownOptionsService<ApplicationRoles, string> optionsService 
            )
        {
            this.service = service;
            this.roleManager = roleManager;
            this.optionsService = optionsService;
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns role options",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of options", Type = typeof(List<OptionDTO<string>>))]
        [HttpGet("/api/roleOptions")]
        public async Task<ActionResult<List<OptionDTO<string>>>> GetOptions()
        {
            var roles = roleManager.Roles.ToList();
            if (roles.Where(r => r.Name == User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value).Count() == 0)
            {
                return Unauthorized();
            }

            var options = await optionsService.GetOptions(o => o.Name != "Admin" && o.Name != "Merchant" && o.Name != "Representative");

            return Ok(options);
        }

        // GET: api/RolePowers
        [SwaggerOperation(
        Summary = "This Endpoint returns a list of rolePowers",
            Description = ""
        )]
        [SwaggerResponse(404, "There weren't any rolePowers in the database", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of rolePowers", Type = typeof(List<RolePowersDTO>))]
        [HttpGet]
        public async Task<ActionResult<List<RolePowersDTO>>> GetAllRolePowers()
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var rolePowers = await service.GetAllObjects();

            if (rolePowers == null || rolePowers.Count == 0)
            {
                return NotFound();
            }

            return Ok(rolePowers);
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns a list of rolePowers with the specified page size",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of rolePowers", Type = typeof(PaginationDTO<RolePowersDTO>))]
        [HttpGet("/api/RolePowerPage")]
        public async Task<ActionResult<PaginationDTO<RolePowersDTO>>> GetPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = "")
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var paginationDTO = await service.GetPaginatedOrders(pageNumber, pageSize, rp => (rp.ApplicationRoles.Name != null) ? rp.ApplicationRoles.Name.Trim().ToLower().Contains(name.Trim().ToLower()) : 1 == 1);

            return Ok(paginationDTO);
        }

        // GET api/RolePowers/fkmc4a2wkmfkmq
        [SwaggerOperation(
        Summary = "This Endpoint returns the specified rolePower",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns the specified rolePower", Type = typeof(RolePowersDTO))]
        [HttpGet("{id}")]
        public async Task<ActionResult<RolePowersDTO>> GetRolePowerById(string id)
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var rolePower = await service.GetObject(rp => rp.RoleId == id);

            if (rolePower == null)
            {
                return NotFound();
            }

            return Ok(rolePower);
        }

        // POST api/RolePowers
        [SwaggerOperation(
        Summary = "This Endpoint inserts a rolePower element in the db",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the rolePower was inserted successfully", Type = typeof(void))]
        [HttpPost]
        public async Task<IActionResult> PostRolePower([FromBody] RolePowersInsertDTO rolePowersInsertDTO)
        {
            if (await CheckRole(PowerTypes.Create))
            {
                return Unauthorized();
            }

            var result = await service.InsertObject(rolePowersInsertDTO);

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

        // PUT api/RolePowers/fkmc4a2wkmfkmq
        [SwaggerOperation(
        Summary = "This Endpoint updates the specified rolePower",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(400, "The id that was given doesn't equal the id in the given rolePower object", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the rolePower was updated successfully", Type = typeof(void))]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRolePower(string id, [FromBody] RolePowersUpdateDTO rolePowersUpdateDTO)
        {
            if (await CheckRole(PowerTypes.Update))
            {
                return Unauthorized();
            }

            if (id != rolePowersUpdateDTO.RoleId)
            {
                return BadRequest(new ErrorDTO() { Message = "Id doesn't match the id in the object" });
            }

            var rolePower = await service.GetObjectWithoutTracking(rp => rp.RoleId == id);

            if (rolePower == null)
            {
                return NotFound(new ErrorDTO() { Message = "Role Power doesn't exist in the db" });
            }

            var result = await service.UpdateObject(rolePowersUpdateDTO);

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

        // DELETE api/RolePowers/fkmc4a2wkmfkmq
        [SwaggerOperation(
        Summary = "This Endpoint deletes the specified rolePower from the db",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the rolePower was deleted successfully", Type = typeof(void))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRolePower(string id)
        {
            if (await CheckRole(PowerTypes.Delete))
            {
                return Unauthorized();
            }

            var rolePower = await service.GetObjectWithoutTracking(rp => rp.RoleId == id);

            if (rolePower == null)
            {
                return NotFound(new ErrorDTO() { Message = "Role Power doesn't exist in the db" });
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
