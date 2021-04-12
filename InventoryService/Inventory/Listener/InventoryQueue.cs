using Domain.Model;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory
{
    public class InventoryQueue : IHostedService
    {
        private readonly IPublisher publisher;
        private readonly ISubscriber subscriber;
        private readonly IInventoryRepository invRepository;

        public InventoryQueue(IPublisher publisher, ISubscriber subscriber, IInventoryRepository invRepository)
        {
            this.publisher = publisher;
            this.subscriber = subscriber;
            this.invRepository = invRepository;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<Domain.Model.Inventory>(message);

            try
            {
                if (response.IsSuccess)
                {
                    var hdl = new HandlerModel();
                    hdl.Message = "Berhasil disimpan";

                    invRepository.Update(response.ProductID, response.Stock).GetAwaiter().GetResult();
                    hdl.Result = true;
                    hdl.OrderID = response.OrderID;
                    publisher.Publish(JsonConvert.SerializeObject(hdl), "handler.event", null);
                }
            }
            catch
            {
                var order = new Order();
                order.OrderID = response.OrderID;
                order.IsSuccess = false;

                publisher.Publish(JsonConvert.SerializeObject(order), "order.event", null);
            }

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
