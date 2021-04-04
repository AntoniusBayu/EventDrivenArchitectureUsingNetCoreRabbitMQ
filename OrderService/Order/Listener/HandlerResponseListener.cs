using Domain.Model;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Order
{
    public class HandlerResponseListener : IHostedService
    {
        private readonly ISubscriber subscriber;

        public IPublisher Publisher { get; }
        public IOrderRepository OrderRepository { get; }

        public HandlerResponseListener(IPublisher publisher, ISubscriber subscriber, IOrderRepository orderRepository)
        {
            this.Publisher = publisher;
            this.subscriber = subscriber;
            this.OrderRepository = orderRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<Domain.Model.Order>(message);

            try
            {
                OrderRepository.Create(response);
                Publisher.Publish(JsonConvert.SerializeObject(
                        new Inventory { ProductID = response.ProductID, Stock = response.Quantity, IsSuccess = true }
                        ), "order.event", null);
            }
            catch (System.Exception)
            {
                Publisher.Publish(JsonConvert.SerializeObject(
                        new Inventory { ProductID = response.ProductID, Stock = response.Quantity, IsSuccess = false }
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
