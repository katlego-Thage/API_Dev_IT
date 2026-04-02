using API_Dev_IT.Context;
using API_Dev_IT.Helper;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Dev_IT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly ITenant _tenant;
        public TenantController(BookingContext context,
               ILogger<UserController> logger,
               ITenant tenant)
        {
            _context = context;
            _logger = logger;
            _tenant = tenant;
        }

        [HttpGet("GetTenant")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var tenant = await _context.tenant
                            .AsNoTracking()
                            .ToListAsync();
                return Ok(tenant);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("GetTenant/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var tenant = await _context.tenant
                            .FirstOrDefaultAsync(x => 
                            x.TenantID == id);
                return Ok(tenant);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPost("Tenant")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Post(Tenant ten)
        {
            try
            {
                var userRole = UserRoleHelper.GetRole();
                if (userRole is not "Manager" || userRole is not "Admin")
                {
                    return Unauthorized("Your not authorized add rooms");
                }
                var tenant = await _tenant.Create(ten);
                return Ok(tenant);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPut("Tenant/{id}")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Put(Tenant ten, int id)
        {
            try
            {
                var userRole = UserRoleHelper.GetRole();
                if (userRole is not "Manager" || userRole is not "Admin")
                {
                    return Unauthorized("Your not authorized add rooms");
                }
                var tenant = await _tenant.Update(ten, id);
                return Ok(tenant);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userRole = UserRoleHelper.GetRole();
                if (userRole is not "Manager" || userRole is not "Admin")
                {
                    return Unauthorized("Your not authorized add rooms");
                }
                var tenant = await _tenant.Delete(id);
                return Ok(tenant);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }
    }
}
