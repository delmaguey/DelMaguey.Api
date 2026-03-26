using DelMaguey.Api.Models;
using DelMaguey.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace DelMaguey.Api.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ServiceBusPublisher _publisher;

        public PaymentsController(ServiceBusPublisher publisher)
        {
            _publisher = publisher;
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
                Id = request.Id,
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
                transaction.Id,
                Status = "Processing"
            });


        }


    }
}
