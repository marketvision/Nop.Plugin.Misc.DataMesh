using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Microsoft.Rest.Azure.Authentication;
using System;
using Microsoft.Rest;
using System.Threading;
using Microsoft.Azure.DataLake.Store;
using System.Text;
using Newtonsoft.Json;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Misc.DataMesh.Infrastructure
{
    public class DataLakeConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly Uri _dataLakeTokenAudience = new Uri(@"https://datalake.azure.net/");
        private readonly string _tenant = "d2e01622-5fd0-4c20-b85f-f7ed187e83c9";
        private readonly string _clientId = "e4c80a6a-16e9-4f97-94e7-14e453b4fe8e";
        private readonly string _secretKey = "ceaj+aHQr7esNWqctq3ZvIA2hw1whkbgG+RdnLhnKXY=";
        private static string _adlsg1AccountName = "alexp.azuredatalakestore.net";

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            var dataLakeCredentials = GetCredentials(_tenant, _dataLakeTokenAudience, _clientId, _secretKey);

            var client = AdlsClient.CreateClient(_adlsg1AccountName, dataLakeCredentials);

            try
            {
                var fileName = "/Test/" + eventMessage.Order.OrderGuid.ToString() + ".txt";

                using (var stream = client.CreateFile(fileName, IfExists.Overwrite))
                {
                    Order order = eventMessage.Order;
                    Customer customer = order.Customer;
                    var simplifiedOrder = new SimplifiedOrderData(order.OrderGuid.ToString(), "NopOrder", customer.Id.ToString(), customer.SystemName);

                    var json = JsonConvert.SerializeObject(simplifiedOrder, new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented,
                    });

                    var fileToWrite = Encoding.UTF8.GetBytes(json);

                    stream.Write(fileToWrite, 0, fileToWrite.Length);
                }
            }
            catch (AdlsException exception)
            {
                PrintAdlsException(exception);
            }
        }

        private static void PrintAdlsException(AdlsException exp)
        {
            Console.WriteLine("ADLException");
            Console.WriteLine($"   Http Status: {exp.HttpStatus}");
            Console.WriteLine($"   Http Message: {exp.HttpMessage}");
            Console.WriteLine($"   Remote Exception Name: {exp.RemoteExceptionName}");
            Console.WriteLine($"   Server Trace Id: {exp.TraceId}");
            Console.WriteLine($"   Exception Message: {exp.Message}");
            Console.WriteLine($"   Exception Stack Trace: {exp.StackTrace}");
            Console.WriteLine();
        }

        private static ServiceClientCredentials GetCredentials(string tenant, Uri tokenAudience, string clientId, string secretKey)
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var serviceSettings = ActiveDirectoryServiceSettings.Azure;
            serviceSettings.TokenAudience = tokenAudience;

            var credentials = ApplicationTokenProvider.LoginSilentAsync(tenant, clientId, secretKey, serviceSettings).GetAwaiter().GetResult();
            return credentials;
        }
    }
}