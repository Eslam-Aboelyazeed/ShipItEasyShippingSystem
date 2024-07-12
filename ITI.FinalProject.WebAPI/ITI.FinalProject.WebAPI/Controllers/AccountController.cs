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
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITI.FinalProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRoles> roleManager;
        private readonly IPaginationService<RolePowers, RolePowersDTO, RolePowersInsertDTO, RolePowersUpdateDTO, string> service;
        private readonly IConfiguration configuration;

        public AccountController
            (
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IConfiguration configuration, 
            RoleManager<ApplicationRoles> roleManager,
            IPaginationService<RolePowers, RolePowersDTO, RolePowersInsertDTO, RolePowersUpdateDTO, string> service
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
            this.service = service;
        }

        [SwaggerOperation(
        Summary = "This Endpoint logs the user in the system",
            Description = ""
        )]
        [SwaggerResponse(400, "The user name or email or password weren't given", Type = typeof(ErrorDTO))]
        [SwaggerResponse(200, "Confirms that the user was loggedin successfully", Type = typeof(LoginResponseDTO))]
        [HttpPost("/api/login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO userLoginDTO)
        {
            if (userLoginDTO == null)
            {
                return BadRequest(new ErrorDTO() { Message = "Please enter valid user name or email and vaild password" });
            }

            var user = await userManager.FindByEmailAsync(userLoginDTO.EmailOrUserName);

            if (user == null)
            {
                user = await userManager.FindByNameAsync(userLoginDTO.EmailOrUserName);

                if (user == null)
                {
                    return BadRequest(new ErrorDTO() { Message = "Please enter valid user name or email" });
                }
            }

            var result = await userManager.CheckPasswordAsync(user, userLoginDTO.Password);

            if (result == false)
            {
                return BadRequest(new ErrorDTO() { Message = "Plaese enter valid password" });
            }

            if (user.Status == Status.Inactive)
            {
                return BadRequest(new ErrorDTO() { Message = "User is inactive" });
            }

            var claims = await userManager.GetClaimsAsync(user);

            var cl = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            IdentityResult identityRes = new IdentityResult();

            if (cl != null)
            {
                identityRes = await userManager.RemoveClaimAsync(user, cl);
            }

            var r = await userManager.GetRolesAsync(user);

            identityRes = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, r[0]));

            cl = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (cl != null)
            {
                identityRes = await userManager.RemoveClaimAsync(user, cl);
            }

            var id = await userManager.GetUserIdAsync(user);

            identityRes = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, id));

            foreach (Tables power in Enum.GetValues(typeof(Tables)))
            {
                cl = claims.FirstOrDefault(c => c.Type == $"RolePowers{power}");

                if (cl != null)
                {
                    identityRes = await userManager.RemoveClaimAsync(user, cl);
                }
            }

            var roles = await service.GetAllObjects();

            var role = roles.FirstOrDefault(ro => ro.RoleName == r[0]);

            if (role != null)
            {
                var rps = await service.GetObject(rp => rp.RoleId == role.RoleId);

                if (rps != null && rps.Powers != null)
                {                    
                    foreach (var power in rps.Powers)
                    {
                        var validtions = new StringBuilder();

                        if (power.Create)
                        {
                            validtions.Append("Create ");
                        }

                        if (power.Update)
                        {
                            validtions.Append("Update ");
                        }

                        if (power.Delete)
                        {
                            validtions.Append("Delete ");
                        }

                        if (power.Read)
                        {
                            validtions.Append("Read ");
                        }

                        identityRes = await userManager.AddClaimAsync(user, new Claim($"RolePowers{power}", validtions.ToString()));
                    }
                }
            }


            cl = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (cl != null)
            {
                identityRes = await userManager.RemoveClaimAsync(user, cl);
            }

            var userName = await userManager.GetUserNameAsync(user);

            identityRes = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, userName ?? ""));

            cl = claims.FirstOrDefault(c => c.Type == "ExpireDate");

            if (cl != null)
            {
                identityRes = await userManager.RemoveClaimAsync(user, cl);
            }

            identityRes = await userManager.AddClaimAsync(user, new Claim("ExpireDate", DateTime.Now.AddDays(1).ToString("f")));

            cl = claims.FirstOrDefault(c => c.Type == "UserType");

            if (cl != null)
            {
                identityRes = await userManager.RemoveClaimAsync(user, cl);
            }

            identityRes = await userManager.AddClaimAsync(user, new Claim("UserType", user.UserType.ToString()));

            claims = await userManager.GetClaimsAsync(user);

            var sKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("SKey").Value??""));

            var signingCreds = new SigningCredentials(sKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signingCreds
                );

            var givenToken = new JwtSecurityTokenHandler().WriteToken(token);


            await signInManager.SignInAsync(user, false);

            return Ok(new LoginResponseDTO()
            {
                Token = givenToken
            });

        }

        [SwaggerOperation(
        Summary = "This Endpoint logs the user out of the system",
            Description = ""
        )]
        [SwaggerResponse(404, "The user id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(204, "Confirms that the user was loggedout successfully", Type = typeof(void))]
        [HttpGet("/api/logout")]
        public async Task<IActionResult> Logout([FromQuery] string UserId)
        {
            var roleList = await roleManager.Roles.ToListAsync();

            if (roleList.FirstOrDefault(r => r.Name == User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value) == null)
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return NotFound();
            }

            await signInManager.SignOutAsync();

            return NoContent();
        }
    }
}
