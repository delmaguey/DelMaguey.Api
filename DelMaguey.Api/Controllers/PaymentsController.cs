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
        public async Task<IActionResult> Transfer([FromBody] object request)
        {
            //if (request.Amount <= 0)
            //{
            //    return BadRequest("Invalid Amount");
            //}

            var transaction = new
            {
                TransactionId = Guid.NewGuid().ToString(),
                Type = "Transfer",
                //FromAccount = request.FromAccount,
                //ToAccount = request.ToAccount,
                //Amount = request.Amount,
                TimeStamp = DateTime.UtcNow,
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
                transaction.TransactionId,
                Status = "Processing"
            });


        }


    }
}
