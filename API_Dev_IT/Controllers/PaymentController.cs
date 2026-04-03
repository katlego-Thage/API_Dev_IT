using API_Dev_IT.Context;
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
    public class PaymentController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly IPayment _payment;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(BookingContext context, 
               IPayment payment,
               ILogger<PaymentController> logger)
        {
            _context = context;
            _payment = payment;
            _logger = logger;
        }

        [HttpGet("GetPayment")]
        [Authorize(Roles = "Manager, Admin, Receptionist")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var pay = await _context.payment
                        .AsNoTracking()
                        .ToListAsync();
                return Ok(pay);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpGet("GetPayment/{id}")]
        [Authorize(Roles = "Manager, Admin, Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pay = await _context.payment
                         .FirstOrDefaultAsync(x =>
                         x.PaymentID == id);
                return Ok(pay);
            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPost("Payment")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Post(Payment pay)
        {
            try
            {
                var payment = await _payment.Create(pay);

                return Ok(payment);

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }

        [HttpPut("Payment/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Put(Payment pay, int id)
        {
            try
            {
                var payment = await _payment.Update(pay, id);

                return Ok(payment);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }
        // might have to remove this, a payment can be made or updated
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var payment = await _payment.Delete(id);

                return Ok(payment);

            }
            catch (Exception x)
            {
                _logger.LogError($"{x.Message}");
                return BadRequest(x.Message);
            }
        }
    }
}
