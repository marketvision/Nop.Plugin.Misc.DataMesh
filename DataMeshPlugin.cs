using System;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.DataMesh
{
    public class DataMeshPlugin : IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public PluginDescriptor PluginDescriptor { get; set; }

        public DataMeshPlugin(
            IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        public string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/MiscDataMesh/Configure";
        }

        public void Install()
        {

        }

        public void PreparePluginToUninstall()
        {

        }

        public void Uninstall()
        {

        }
    }
}
