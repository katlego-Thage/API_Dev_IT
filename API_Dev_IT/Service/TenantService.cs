using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace API_Dev_IT.Service
{
    public class TenantService : ITenant
    {
        private readonly BookingContext _context;
        private readonly ILogger<TenantService> _logger;
        public TenantService(BookingContext context, ILogger<TenantService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<T> Create<T>(Tenant ten)
        {
            try
            {
                var tenant = await _context.tenant
                             .FirstOrDefaultAsync(x => 
                             x.Email == ten.Email);

                if (tenant?.Email == ten.Email)
                {
                    _logger.LogWarning($"Failed: email {ten.Email}" +
                                       $" already exists");
                    throw new InvalidOperationException("Tenant already exist");
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

                _logger.LogInformation(
                $"Tenant {insert.TenantID} created successfully");
                return (T)(object)insert;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during tenant registration");
                throw;
            }
        }

        public async Task<T> Update<T>(Tenant ten, int id)
        {
            try
            {
                var tenant = await _context.tenant
                             .FirstOrDefaultAsync(x =>
                             x.TenantID == id);

                if (tenant?.TenantID != ten.TenantID)
                {
                    _logger.LogWarning($" email {ten.Email}" +
                                       $" doesnt exists");
                    throw new InvalidOperationException(
                              "Invalid Tenant");
                }

                if (tenant == null)
                {
                    _logger.LogWarning($"Update failed: tenant {id} not found");
                    throw new InvalidOperationException("Tenant not found");
                }

                tenant.FullName = ten.FullName;
                tenant.Email = ten.Email;
                tenant.Phone = ten.Phone;
                tenant.Address = ten.Address;

                _context.Update(tenant);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tenant {id} updated successfully");
                return (T)(object)tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Tenant error occurred while updating {id}");
                throw;
            }
        }

        public async Task<T> Delete<T>(int id)
        {
            try
            {
                var tenant = await _context.tenant
                             .FirstOrDefaultAsync(x =>
                             x.TenantID == id);

                if (tenant is null || tenant.TenantID != id)
                {
                    _logger.LogWarning($"delete failed: tenant {id} not found");
                    throw new InvalidOperationException(
                              "Tenant doesnt exist");
                }
                _context.tenant.Remove(tenant);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tenant {id} deleted successfully");
                return (T)(object)tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Tenant error occurred while deleting {id}");
                throw;
            }
        }
    }
}
