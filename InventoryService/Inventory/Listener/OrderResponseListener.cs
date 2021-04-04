using Domain.Model;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory
{
    public class OrderResponseListener : IHostedService
    {
        private readonly IPublisher publisher;
        private readonly ISubscriber subscriber;
        private readonly IInventoryRepository invRepository;

        public OrderResponseListener(IPublisher publisher, ISubscriber subscriber, IInventoryRepository invRepository)
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
            var response = JsonConvert.DeserializeObject<Domain.Model.Order>(message);
            if (!response.IsSuccess)
            {
                invRepository.Update(response.ProductID, response.Quantity);
                publisher.Publish(JsonConvert.SerializeObject(
                        new Order { OrderID = response.OrderID,ProductID = response.ProductID, IsSuccess = false }
                        ), "inventory.event", null);
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
