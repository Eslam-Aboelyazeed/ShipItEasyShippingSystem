using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces.ApplicationServices;
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
    public class ProductsController : ControllerBase
    {
        private readonly IGenericService<Product, DisplayProductDTO, InsertProductDTO, UpdateProductDTO, int> _productService;
        private readonly RoleManager<ApplicationRoles> roleManager;

        public ProductsController(IGenericService<Product, DisplayProductDTO, InsertProductDTO, UpdateProductDTO, int> productService, RoleManager<ApplicationRoles> roleManager)
        {
            _productService = productService;
            this.roleManager = roleManager;
        }

        // GET: api/Products
        [SwaggerOperation(Summary = "This Endpoint returns a list of products", Description = "")]
        [SwaggerResponse(404, "There weren't any products in the database", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns A list of products", Type = typeof(List<DisplayProductDTO>))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayProductDTO>>> GetProducts()
        {
            if (await CheckRole(PowerTypes.Read, true, true))
            {
                return Unauthorized();
            }

            var products = await _productService.GetAllObjects();

            if (products == null || products.Count == 0)
            {
                return NotFound();
            }

            return Ok(products);
        }

        // GET: api/Products/5
        [SwaggerOperation(Summary = "This Endpoint returns the specified product")]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(void))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(200, "Returns the specified product", Type = typeof(DisplayProductDTO))]
        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayProductDTO>> GetProduct(int id)
        {
            if (await CheckRole(PowerTypes.Read, true, true))
            {
                return Unauthorized();
            }

            var product = await _productService.GetObject(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Products
        [SwaggerOperation(Summary = "This Endpoint inserts a new product in the db", Description = "")]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the product was inserted successfully", Type = typeof(void))]
        [HttpPost]
        public async Task<IActionResult> PostProduct(InsertProductDTO productDTO)
        {
            if (await CheckRole(PowerTypes.Create, true, false))
            {
                return Unauthorized();
            }

            var result = await _productService.InsertObject(productDTO);

            if (result.Succeeded)
            {
                if (await _productService.SaveChangesForObject())
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

        // PUT: api/Products/5
        [SwaggerOperation(Summary = "This Endpoint updates the specified product", Description = "")]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(400, "The id that was given doesn't equal the id in the given product object", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the product was updated successfully", Type = typeof(void))]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, UpdateProductDTO productDTO)
        {
            if (await CheckRole(PowerTypes.Update, true, true))
            {
                return Unauthorized();
            }

            if (id != productDTO.Id)
            {
                return BadRequest(new ErrorDTO() { Message = "Id doesn't match the id in the object" });
            }

            var success = await _productService.GetObjectWithoutTracking(p => p.Id == id);

            if (success == null)
            {
                return NotFound(new ErrorDTO() { Message = "Product doesn't exist in the db" });
            }

            var result = await _productService.UpdateObject(productDTO);

            if (result.Succeeded)
            {
                if (await _productService.SaveChangesForObject())
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

        // DELETE: api/Products/5
        [SwaggerOperation(Summary = "This Endpoint deletes the specified product", Description = "")]
        [SwaggerResponse(404, "The id that was given doesn't exist in the db", Type = typeof(ErrorDTO))]
        [SwaggerResponse(401, "Unauthorized", Type = typeof(void))]
        [SwaggerResponse(500, "Something went wrong, please try again later", Type = typeof(ErrorDTO))]
        [SwaggerResponse(204, "Confirms that the product was deleted successfully", Type = typeof(void))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (await CheckRole(PowerTypes.Delete, true, false))
            {
                return Unauthorized();
            }

            var success = await _productService.GetObjectWithoutTracking(p => p.Id == id);

            if (success == null)
            {
                return NotFound(new ErrorDTO() { Message = "Product doesn't exist in the db" });
            }

            var result = await _productService.DeleteObject(id);

            if (result.Succeeded)
            {
                if (await _productService.SaveChangesForObject())
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

        private async Task<bool> CheckRole(PowerTypes powerType, bool isAdminAllowed, bool isRepresentativeAllowed)
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == null)
            {
                return true;
            }

            if (role == "Merchant")
            {
                return false;
            }

            if (isAdminAllowed)
            {
                if (role == "Admin")
                {
                    return false;
                }
            }

            if (isRepresentativeAllowed)
            {
                if (role == "Representative")
                {
                    return false;
                }
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
