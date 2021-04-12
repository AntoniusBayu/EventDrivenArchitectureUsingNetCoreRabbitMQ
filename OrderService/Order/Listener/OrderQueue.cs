using Domain.Model;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Order
{
    public class OrderQueue : IHostedService
    {
        private readonly IPublisher publisher;
        private readonly ISubscriber subscriber;
        private readonly IOrderRepository orderRepository;

        public OrderQueue(IPublisher publisher, ISubscriber subscriber, IOrderRepository orderRepository)
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
            var response = JsonConvert.DeserializeObject<Domain.Model.Order>(message);
            var hdl = new HandlerModel();

            try
            {
                if (response.IsSuccess)
                {
                    var inv = new Inventory();
                    inv.ProductID = response.ProductID;
                    inv.Stock = response.Quantity;
                    inv.OrderID = response.OrderID;

                    this.orderRepository.Create(response).GetAwaiter().GetResult();
                    inv.IsSuccess = true;
                    publisher.Publish(JsonConvert.SerializeObject(inv), "inventory.event", null);

                    hdl.Result = false;
                    hdl.Message = "Create udah berhasil, tinggal masuk inventory";
                    hdl.OrderID = response.OrderID;

                    publisher.Publish(JsonConvert.SerializeObject(hdl), "handler.event", null);
                }
                else
                {
                    hdl.Result = false;
                    hdl.Message = "Inventory Gagal Euuuyyy";
                    hdl.OrderID = response.OrderID;

                    this.orderRepository.Delete(response.OrderID).GetAwaiter().GetResult();
                    publisher.Publish(JsonConvert.SerializeObject(hdl), "handler.event", null);
                }
            }
            catch (Exception ex)
            {
                hdl.Result = false;
                hdl.Message = ex.Message;
                hdl.OrderID = response.OrderID;

                publisher.Publish(JsonConvert.SerializeObject(hdl), "handler.event", null);
            }

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
