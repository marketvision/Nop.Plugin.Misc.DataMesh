using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.DataMesh.Infrastructure
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            System.Console.WriteLine(eventMessage.Order.CustomOrderNumber);
        }
    }
}