using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.DataMesh.Components
{
    /// <summary>
    /// Represents a view component to render 'Sign In with oAuth2.0' button on the login page
    /// </summary>
    [ViewComponent(Name = Default.ViewComponentName)]
    public class DataMeshViewComponent : NopViewComponent
    {
        /// <summary>
        /// Invoke the external authentication view component
        /// </summary>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Misc.DataMesh/Views/PublicInfo.cshtml");
        }
    }
}