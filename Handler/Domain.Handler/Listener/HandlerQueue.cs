using Domain.Model;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Handler
{
    public class HandlerQueue : IHostedService
    {
        private readonly ISubscriber subscriber;

        public IHandlerRepository Handler { get; }

        public HandlerQueue(ISubscriber subscriber, IHandlerRepository handler)
        {
            this.subscriber = subscriber;
            Handler = handler;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<HandlerModel>(message);
            Handler.Update(response);
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
