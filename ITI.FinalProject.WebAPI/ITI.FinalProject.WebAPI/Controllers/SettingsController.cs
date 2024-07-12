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
using System.Text;

namespace ITI.FinalProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IGenericService<Settings, SettingsDTO, SettingsInsertDTO, SettingsUpdateDTO, int> service;
        private readonly RoleManager<ApplicationRoles> roleManager;
        public SettingsController(IGenericService<Settings, SettingsDTO, SettingsInsertDTO, SettingsUpdateDTO, int> service, RoleManager<ApplicationRoles> roleManager)
        {
            this.service = service;
            this.roleManager = roleManager;
        }

        // GET: api/Settings
        [SwaggerOperation(Summary = "This Endpoint returns a list of settings", Description = "")]
        [SwaggerResponse(404, "There weren't any settings in the database", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of settings", Type = typeof(List<SettingsDTO>))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettingsDTO>>> GetSettingsList()
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var settingsList = await service.GetAllObjects();

            if (settingsList == null || settingsList.Count == 0)
            {
                return NotFound();
            }

            return Ok(settingsList);
        }


        // GET: api/Settings/5
        [SwaggerOperation(Summary = "This Endpoint returns the specified settings")]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns the specified settings", Type = typeof(SettingsDTO))]
        [HttpGet("{id}")]
        public async Task<ActionResult<SettingsDTO>> GetSettings(int id)
        {
            if (await CheckRole(PowerTypes.Read))
            {
                return Unauthorized();
            }

            var settings = await service.GetObject(s => s.Id == id);

            if (settings == null)
            {
                return NotFound();
            }

            return Ok(settings);
        }

        // POST: api/Settings
        [SwaggerOperation(Summary = "This Endpoint inserts a new settings in the db", Description = "")]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the settings was inserted successfully", Type = typeof(void))]
        [HttpPost]
        public async Task<IActionResult> PostSettings([FromBody] SettingsInsertDTO settingsInsertDTO)
        {
            if (await CheckRole(PowerTypes.Create))
            {
                return Unauthorized();
            }

            var result = await service.InsertObject(settingsInsertDTO);

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

        // PUT: api/Settings/5
        [SwaggerOperation(Summary = "This Endpoint updates the specified settings", Description = "")]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(400, "The id that was given doesn't equal the id in the given settings object", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the settings was updated successfully", Type = typeof(void))]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSettings(int id, SettingsUpdateDTO settingsUpdateDTO)
        {
            if (await CheckRole(PowerTypes.Update))
            {
                return Unauthorized();
            }

            if (id != settingsUpdateDTO.Id)
            {
                return BadRequest(new ErrorDTO() { Message = "Id doesn't match the id in the object" });
            }

            var success = await service.GetObjectWithoutTracking(s => s.Id == id);

            if (success == null)
            {
                return NotFound(new ErrorDTO() { Message = "Settings doesn't exist in the db" });
            }

            var result = await service.UpdateObject(settingsUpdateDTO);

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

        // DELETE: api/Settings/5
        [SwaggerOperation(Summary = "This Endpoint deletes the specified settings", Description = "")]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the settings was deleted successfully", Type = typeof(void))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSettings(int id)
        {
            if (await CheckRole(PowerTypes.Delete))
            {
                return Unauthorized();
            }

            var success = await service.GetObjectWithoutTracking(s => s.Id == id);

            if (success == null)
            {
                return NotFound(new ErrorDTO() { Message = "Settings doesn't exist in the db" });
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
