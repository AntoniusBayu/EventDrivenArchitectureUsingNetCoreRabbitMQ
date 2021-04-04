using Domain.Model;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Order
{
    public class InventoryResponseListener : IHostedService
    {
        private readonly IPublisher publisher;
        private readonly ISubscriber subscriber;
        private readonly IOrderRepository orderRepository;

        public InventoryResponseListener(IPublisher publisher, ISubscriber subscriber, IOrderRepository orderRepository)
        {
            this.publisher = publisher;
            this.subscriber = subscriber;
            this.orderRepository = orderRepository;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<Domain.Model.Inventory>(message);
            if (!response.IsSuccess)
            {
                orderRepository.Delete(response.OrderID);
                publisher.Publish(JsonConvert.SerializeObject(
                        new Inventory { ProductID = response.ProductID, IsSuccess = false }
                        ), "order.event", null);
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
