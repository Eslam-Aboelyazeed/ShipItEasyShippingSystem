using Application.DTOs;
using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces.ApplicationServices;
using Domain.Entities;
using Domain.Enums;
using ITI.FinalProject.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Transactions;

namespace ITI.FinalProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepresentativeController : ControllerBase
    {
        private readonly IPaginationService<Representative, RepresentativeDisplayDTO, RepresentativeInsertDTO, RepresentativeUpdateDTO, string> service;
        private readonly RoleManager<ApplicationRoles> roleManager;
        private readonly IDropDownOptionsService<Representative, string> optionsService;

        public RepresentativeController(
            IPaginationService<Representative,RepresentativeDisplayDTO,RepresentativeInsertDTO,RepresentativeUpdateDTO,string> service, RoleManager<ApplicationRoles> roleManager,
            IDropDownOptionsService<Representative, string> optionsService
            )
        {
            this.service = service;
            this.roleManager = roleManager;
            this.optionsService = optionsService;
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns representative options",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of options", Type = typeof(List<OptionDTO<string>>))]
        [HttpGet("/api/representativeOptions")]
        public async Task<ActionResult<List<OptionDTO<string>>>> GetOptions()
        {
            var roles = roleManager.Roles.ToList();
            if (roles.Where(r => r.Name == User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value).Count() == 0)
            {
                return Unauthorized();
            }

            var options = await optionsService.GetOptions(o => o.user.Status == Status.Active, o => o.user, o => o.governorates);

            return Ok(options);
        }

        // GET: api/Representative
        [SwaggerOperation(
        Summary = "This Endpoint returns a list of representative",
            Description = ""
        )]
        [SwaggerResponse(404, "There weren't any representative in the database", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of representative", Type = typeof(List<RepresentativeDisplayDTO>))]
        [HttpGet]
        public async Task<ActionResult<List<RepresentativeDisplayDTO>>> GetAllRepresentative()
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var Representatives=await service.GetAllObjects(r => r.user, r => r.governorates);

            if (Representatives == null || Representatives.Count == 0)
            {
                return NotFound();
            }

            return Ok(Representatives);
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns a list of representatives with the specified page size",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of representatives", Type = typeof(PaginationDTO<RepresentativeDisplayDTO>))]
        [HttpGet("/api/RepresentativePage")]
        public async Task<ActionResult<PaginationDTO<RepresentativeDisplayDTO>>> GetPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = "")
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var paginationDTO = await service.GetPaginatedOrders(pageNumber, pageSize, r => 1 == 1);
            paginationDTO.List = paginationDTO.List.Where(r => r.UserFullName.Trim().ToLower().Contains(name.Trim().ToLower())).ToList();

            return Ok(paginationDTO);
        }

        // GET: api/Representative/owcmwmece51cwe5
        [SwaggerOperation(
        Summary = "This Endpoint returns the specified representative",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns the specified representative", Type = typeof(RepresentativeDisplayDTO))]
        [HttpGet("{id}")]
        public async Task<ActionResult<RepresentativeDisplayDTO>> GetRepresentativeById(string id)
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var representative = await service.GetObject(r => r.userId == id, r => r.user, r => r.governorates);

            if (representative == null)
            {
                return NotFound();
            }

            return Ok(representative);
        }


        // POST api/Representative
        [SwaggerOperation(
        Summary = "This Endpoint inserts a representative element in the db",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the representative was inserted successfully", Type = typeof(void))]
        [HttpPost]
        public async Task<ActionResult> AddRepresentative([FromBody] RepresentativeInsertDTO RepresentativeInsertDTO)
        {
            if (await CheckRole(PowerTypes.Create))
            {
                return Unauthorized();
            }

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                var result = await service.InsertObject(RepresentativeInsertDTO);
                transaction.Complete();

                if (result.Succeeded)
                {
                    return NoContent();
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = result.Message ?? "Something went wrong, please try again later" });
            }

        }

        // PUT api/Representative/owcmwmece51cwe5
        [SwaggerOperation(
        Summary = "This Endpoint updates the specified representative",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(400, "The id that was given doesn't equal the id in the given representative object", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the representative was updated successfully", Type = typeof(void))]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepresentative(string id, [FromBody] RepresentativeUpdateDTO representativeUpdateDTO)
        {
            if (await CheckRole(PowerTypes.Update))
            {
                return Unauthorized();
            }

            if (id != representativeUpdateDTO.Id)
            {
                return BadRequest(new ErrorDTO() { Message = "Id doesn't match the id in the object" });
            }

            var representative = await service.GetObjectWithoutTracking(r => r.userId == id, r => r.user, r => r.governorates);

            if (representative == null)
            {
                return NotFound(new ErrorDTO() { Message = "Representative doesn't exist in the db" });
            }

            var result = await service.UpdateObject(representativeUpdateDTO);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = result.Message ?? "Something went wrong, please try again later" });
        }

        // DELETE api/Representative/owcmwmece51cwe5
        [SwaggerOperation(
        Summary = "This Endpoint deletes the specified representative from the db",
            Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the representative was deleted successfully", Type = typeof(void))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRepresentative(string id)
        {
            if (await CheckRole(PowerTypes.Delete))
            {
                return Unauthorized();
            }

            var representative = await service.GetObjectWithoutTracking(r => r.userId == id, r => r.user, r => r.governorates);

            if (representative == null)
            {
                return NotFound(new ErrorDTO() { Message = "Representative doesn't exist in the db" });
            }

            var result = await service.DeleteObject(id);

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
