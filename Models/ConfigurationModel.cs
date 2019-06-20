using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.DataMesh.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Misc.DataMesh.ClientId")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("Plugins.Misc.DataMesh.ClientSecret")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string ClientSecret { get; set; }

        [NopResourceDisplayName("Plugins.Misc.DataMesh.Authority")]
        public string Authority { get; set; }

        [NopResourceDisplayName("Plugins.Misc.DataMesh.Scopes")]
        public string Scopes { get; set; }

        [NopResourceDisplayName("Plugins.Misc.DataMesh.AdministratorsRoles")]
        public string AdministratorsRoles { get; set; }
    }
}