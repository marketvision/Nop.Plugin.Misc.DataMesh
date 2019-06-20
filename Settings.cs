using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.DataMesh
{
    public class Settings : ISettings
    {
        public string RabbitMqEndpoint { get; set; }
    }
}