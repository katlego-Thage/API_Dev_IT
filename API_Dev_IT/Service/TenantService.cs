using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_Dev_IT.Service
{
    public class TenantService : ITenant
    {
        private readonly BookingContext _context;
        private readonly ILogger<TenantService> _logger;
        public TenantService(BookingContext context,
               ILogger<TenantService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Tenant> Create(Tenant ten)
        {
            try
            {
                var tenant = await _context.tenant
                             .FirstOrDefaultAsync(x => 
                             x.Email == ten.Email);

                if (tenant?.Email == ten.Email)
                {
                    _logger.LogError($"Tenant {tenant} " +
                              $"already exist");
                    throw new InvalidOperationException(
                              "Tenant already exist");
                }

                var insert = new Tenant
                {
                    FullName = ten.FullName,
                    Phone = ten.Phone,
                    Email = ten.Email,
                    Address = ten.Address
                };

                _context.tenant.Add(insert);
                await _context.SaveChangesAsync();

                return insert;

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }

        public async Task<Tenant> Update(Tenant ten, int id)
        {
            try
            {
                var tenant = await _context.tenant
                             .FirstOrDefaultAsync(x =>
                             x.TenantID == id);

                if (tenant?.TenantID != ten.TenantID)
                {
                    _logger.LogError($"{tenant} " +
                              $"Invalid exist");
                    throw new InvalidOperationException(
                              "Invalid Tenant");
                }

                tenant.FullName = ten.FullName;
                tenant.Email = ten.Email;
                tenant.Phone = ten.Phone;
                tenant.Address = ten.Address;

                _context.Update(tenant);
                await _context.SaveChangesAsync();

                return tenant;
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }

        public async Task<Tenant> Delete(int id)
        {
            try
            {
                var tenant = await _context.tenant
                             .FirstOrDefaultAsync(x =>
                             x.TenantID == id);

                if (tenant is null || tenant.TenantID != id)
                {
                    _logger.LogError($"{tenant} " +
                                  $"Tenant doesnt exist");
                    throw new InvalidOperationException(
                              "Tenant doesnt exist");
                }
                _context.tenant.Remove(tenant);
                await _context.SaveChangesAsync();

                return tenant;
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }
    }
}
