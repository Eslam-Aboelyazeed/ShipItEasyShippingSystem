using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
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

namespace ITI.FinalProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        IPaginationService<Branch, BranchDisplayDTO, BranchInsertDTO, BranchUpdateDTO,int> branchServ;
        private readonly RoleManager<ApplicationRoles> roleManager;
        private readonly IDropDownOptionsService<Branch, int> optionsService;

        public BranchesController(
            IPaginationService<Branch, BranchDisplayDTO, BranchInsertDTO, BranchUpdateDTO,int> _branchServ, RoleManager<ApplicationRoles> roleManager,
            IDropDownOptionsService<Branch, int> optionsService
            )
        {
            branchServ = _branchServ;
            this.roleManager = roleManager;
            this.optionsService = optionsService;
        }


        [SwaggerOperation(
        Summary = "This Endpoint returns branch options",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of options", Type = typeof(List<OptionDTO<int>>))]
        [HttpGet("/api/branchOptions")]
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

        [SwaggerOperation(
        Summary = "This Endpoint returns a list of branches",
            Description = ""
        )]
        [SwaggerResponse(404, "There weren't any branches in the database", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of branches", Type = typeof(List<BranchDisplayDTO>))]
        [HttpGet]
        public async Task<ActionResult> GetAllBranches()
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }
            
            List<BranchDisplayDTO> branches = await branchServ.GetAllObjects();
            if (branches.Count == 0)
            {
                return NotFound();
            }
            return Ok(branches);

        }

        [SwaggerOperation(
        Summary = "This Endpoint returns a list of branches with the specified page size",
            Description = ""
        )]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of branches", Type = typeof(PaginationDTO<BranchDisplayDTO>))]
        [HttpGet("/api/BranchPage")]
        public async Task<ActionResult<PaginationDTO<BranchDisplayDTO>>> GetBranches([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = "")
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var paginationDTO = await branchServ.GetPaginatedOrders(pageNumber, pageSize, b => b.name.Trim().ToLower().Contains(name.Trim().ToLower()));

            return Ok(paginationDTO);
        }

        [SwaggerOperation(
        Summary = "This Endpoint returns the specified branch",
          Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns the specified branch", Type = typeof(BranchDisplayDTO))]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            BranchDisplayDTO? branch = await branchServ.GetObject(p=>p.id==id);
            if (branch == null)
                return NotFound();
            return Ok(branch);

        }

        [SwaggerOperation(
        Summary = "This Endpoint inserts a city element in the db",
          Description = ""
        )]
        [SwaggerResponse(400, "Something went wrong, please check your request", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Confirms that the city was inserted successfully", Type = typeof(ModificationResultDTO))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [HttpPost]
        public async Task<ActionResult> AddBranch(BranchInsertDTO branch)
        {
            if (await CheckRole(PowerTypes.Create))
            {
                return Unauthorized();
            }

            if (branch == null)
                return BadRequest();
            var result =await branchServ.InsertObject(branch);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = result.Message ?? "Something went wrong, please try again later" });
        }

        [SwaggerOperation(
        Summary = "This Endpoint inserts a city element in the db",
          Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Confirms that the city was deleted successfully", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]

        [HttpDelete]
        public async Task<ActionResult> DeleteBranch(int id)
        {
            if (await CheckRole(PowerTypes.Delete))
            {
                return Unauthorized();
            }

            BranchDisplayDTO? branch = await branchServ.GetObjectWithoutTracking(c => c.id == id);
            if (branch == null)
            {
                return NotFound();
            }
            var result =await branchServ.DeleteObject(id);
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Message = result.Message ?? "Something went wrong, please try again later" });
        }

        [SwaggerOperation(
        Summary = "This Endpoint updates the specified city",
           Description = ""
        )]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(400, "The id that was given doesn't equal the id in the given city object", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(200, "Confirms that the city was updated successfully", Type = typeof(BranchUpdateDTO))]

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBranch(int id,BranchUpdateDTO branch)
        {
            if (await CheckRole(PowerTypes.Update))
            {
                return Unauthorized();
            }

            if (branch == null || id != branch.id)
            {
                return BadRequest();
            }

            BranchDisplayDTO? branchDisplay = await branchServ.GetObjectWithoutTracking(c => c.id == id);
            if (branch == null)
            {
                return NotFound();
            }

            var result=await branchServ.UpdateObject(branch);
            if(result.Succeeded)
            {
                return Ok(branch);
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
