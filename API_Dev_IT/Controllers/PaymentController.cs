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
               IPayment payment, ILogger<PaymentController> logger)
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
                _logger.LogInformation($"All payments retrieved successfully");
                return Ok(pay);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while retrieving all payments");
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
                _logger.LogInformation($"Payment {id} retrieved successfully");
                return Ok(pay);
            }
            catch (Exception x)
            {
                _logger.LogError(x, $"Error occurred while retrieving payment {id}");
                return BadRequest(x.Message);
            }
        }

        [HttpPost("Payment")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Post(Payment pay)
        {
            try
            {
                var payment = await _payment.Create<Payment>(pay);
                _logger.LogInformation($"Payment {payment.PaymentID} created successfully");
                return Ok(payment);

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x, $"Error occurred while creating payment");
                return BadRequest(x.Message);
            }
        }

        [HttpPut("Payment/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Put(Payment pay, int id)
        {
            try
            {
                var payment = await _payment.Update<Payment>(pay, id);
                _logger.LogInformation($"Payment {payment.PaymentID} updated successfully");
                return Ok(payment);
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x, $"Error occurred while updating payment {id}");
                return BadRequest(x.Message);
            }
        }
    }
}
