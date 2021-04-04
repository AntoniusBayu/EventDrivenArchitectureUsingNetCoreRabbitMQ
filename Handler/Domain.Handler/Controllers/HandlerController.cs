using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plain.RabbitMQ;

namespace Domain.Handler.Controllers
{
    [Route("api/handler")]
    [ApiController]
    public class HandlerController : ControllerBase
    {
        private readonly IPublisher publisher;

        public HandlerController(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        [HttpPost, Route("AddSingleData")]
        public IActionResult Add(Order data)
        {
            try
            {
                publisher.Publish(JsonConvert.SerializeObject(data), "handler.published", null);

                return Ok("Sukses");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
