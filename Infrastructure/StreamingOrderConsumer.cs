using System.Text;
using Newtonsoft.Json;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using RabbitMQ.Client;

namespace Nop.Plugin.Misc.DataMesh.Infrastructure
{
    public partial class StreamingOrderConsumer : IConsumer<OrderPlacedEvent>
    {
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "nopOrder", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var orderData = new SimplifiedOrderData(eventMessage.Order.OrderGuid.ToString(), "NopOrder", eventMessage.Order.CustomerId.ToString(), eventMessage.Order.Customer.Username);

                    var message = JsonConvert.SerializeObject(orderData);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "", routingKey: "nopOrder", basicProperties: null, body: body);
                }
            }
        }

        public class SimplifiedOrderData
        {
            public SimplifiedOrderData(string orderId, string orderName, string customerId, string customerName)
            {
                OrderId = orderId;
                OrderName = orderName;
                CustomerId = customerId;
                CustomerName = customerName;
            }

            public string OrderId { get; set; }
            public string OrderName { get; set; }
            public string CustomerId { get; set; }
            public string CustomerName { get; set; }
        }
    }
}