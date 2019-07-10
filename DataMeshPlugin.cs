using System;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.DataMesh
{
    public class DataMeshPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;

        public DataMeshPlugin(
            IWebHelper webHelper,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _settingService = settingService;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/MiscDataMesh/Configure";
        }

        public override void Install()
        {
            _settingService.SaveSetting(new Settings());
            base.Install();
        }

        public override void PreparePluginToUninstall()
        {
            base.PreparePluginToUninstall();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<Settings>();
            base.Uninstall();
        }
    }
}
