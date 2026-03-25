//using DelMaguey.Api.Models;
using DelMaguey.Api.Models;
using DelMaguey.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace DelMaguey.Api.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ServiceBusPublisher _publisher;
        private readonly FinanceDbContext _db;

        public PaymentsController(ServiceBusPublisher publisher, FinanceDbContext db)
        {
            _db= db;
            _publisher = publisher;
        }


        [HttpGet("getTransaction/{id}")]
        public async Task<Models.Transaction?> Get(int id)
        {
            // Use FirstOrDefaultAsync to retrieve the transaction by id asynchronously
            var tr = await _db.Transactions.FirstOrDefaultAsync(x => x.TransId == id);
            return tr;
        }

        
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] Models.Transaction request)
        {
            if (request.Amt <= 0)
            {
                return BadRequest("Invalid Amount");
            }

            Models.Transaction? transaction = new ()
            {
                TransId = request.TransId,
                Amt = request.Amt,
                TransDateTransTime = DateTime.UtcNow,
                Job = request.Job,
                City = request.City,
                Last = request.Last,
                Email = request.Email,
                First = request.First,
                State = request.State,
                Gender = request.Gender,
                Street = request.Street,
                Category = request.Category,
                Merchant = request.Merchant,
                TransNum = request.TransNum
            };


            try
            {
                await _publisher.SendTransactionAsync(transaction);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            

            return Accepted(new
            {
                transaction.TransId,
                Status = "Processing"
            });


        }


    }
}
