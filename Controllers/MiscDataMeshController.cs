using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.DataMesh.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.DataMesh.Controllers
{
    public class MiscDataMeshController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly Settings _dataStreamSettings;

        public MiscDataMeshController(ISettingService settingService,
            Settings oAuth2AuthenticationSettings)
        {
            _settingService = settingService;
            _dataStreamSettings = oAuth2AuthenticationSettings;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new DataStreamConfigurationModel
            {
                RabbitMqEndpoint = _dataStreamSettings.RabbitMqEndpoint,
            };

            return View("~/Plugins/Misc.DataMesh/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(DataStreamConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            _dataStreamSettings.RabbitMqEndpoint = model.RabbitMqEndpoint;
            _settingService.SaveSetting(_dataStreamSettings);

            return Configure();
        }
    }
}