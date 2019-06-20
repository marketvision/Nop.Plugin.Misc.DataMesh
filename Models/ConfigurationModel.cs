using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.DataMesh.Models
{
    public class DataStreamConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Misc.DataMesh.RabbitMqEndpoint")]
        public string RabbitMqEndpoint { get; set; }
    }
}