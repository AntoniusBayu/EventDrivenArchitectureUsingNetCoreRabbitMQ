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
        public IHandlerRepository Handler { get; }

        public HandlerController(IPublisher publisher, IHandlerRepository handler)
        {
            this.publisher = publisher;
            this.Handler = handler;
        }

        [HttpPost, Route("AddSingleData")]
        public IActionResult Add(Order data)
        {
            try
            {
                var datahandler = new HandlerModel();
                datahandler.OrderID = data.OrderID;
                datahandler.Result = false;
                datahandler.Message = "Pending";

                this.Handler.Insert(datahandler);
                data.IsSuccess = true;
                publisher.Publish(JsonConvert.SerializeObject(data), "order.event", null);

                return Ok("Sukses");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
