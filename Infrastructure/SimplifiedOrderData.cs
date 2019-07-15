namespace Nop.Plugin.Misc.DataMesh.Infrastructure
{
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