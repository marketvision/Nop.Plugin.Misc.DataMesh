using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.StaticFiles;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.DataMesh;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.DataMesh.Infrastructure
{
    /// <summary>
    /// Represents oAuth2.0 authentication event consumer (used for saving customer fields on registration)
    /// </summary>
    public partial class EventConsumer : IConsumer<CustomerAutoRegisteredByExternalMethodEvent>
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPictureService _pictureService;
        private readonly Settings _oAuth2AuthenticationSettings;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public EventConsumer(CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IPictureService pictureService,
            Settings oAuth2AuthenticationSettings,
            ICustomerService customerService)
        {
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _pictureService = pictureService;
            _oAuth2AuthenticationSettings = oAuth2AuthenticationSettings;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle customer auto-registered by external method event 
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(CustomerAutoRegisteredByExternalMethodEvent eventMessage)
        {
            if (eventMessage?.Customer == null || eventMessage.AuthenticationParameters == null)
                return;

            //handle event only for this authentication method
            if (!eventMessage.AuthenticationParameters.ProviderSystemName.Equals(OAuth2AuthenticationDefaults.SystemName))
                return;

            HandleCustomerRoles(eventMessage);

            //store some of the customer fields
            var firstName = eventMessage.AuthenticationParameters.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(firstName))
                _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.FirstNameAttribute, firstName);

            //upload avatar
            var avatarUrl = eventMessage.AuthenticationParameters.Claims.FirstOrDefault(claim => claim.Type == OAuth2AuthenticationDefaults.AvatarClaimType)?.Value;
            if (string.IsNullOrEmpty(avatarUrl))
                return;

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return;

            try
            {
                //try to get byte array of the user avatar image
                byte[] customerPictureBinary;
                using (var webClient = new WebClient())
                    customerPictureBinary = webClient.DownloadData(avatarUrl);

                if (customerPictureBinary.Length > _customerSettings.AvatarMaximumSizeBytes)
                {
                    _logger.Error(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), _customerSettings.AvatarMaximumSizeBytes));
                    return;
                }

                //save avatar
                new FileExtensionContentTypeProvider().TryGetContentType(avatarUrl, out string mimeType);
                var customerAvatar = _pictureService.InsertPicture(customerPictureBinary, mimeType ?? MimeTypes.ImagePng, null);
                _genericAttributeService.SaveAttribute(eventMessage.Customer, NopCustomerDefaults.AvatarPictureIdAttribute, customerAvatar.Id);
            }
            catch { }
        }

        private void HandleCustomerRoles(CustomerAutoRegisteredByExternalMethodEvent eventMessage)
        {
            IEnumerable<string> roles = eventMessage.AuthenticationParameters.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(x => x.Value);
            string[] configuredAdminRoles = _oAuth2AuthenticationSettings.AdministratorsRoles.Split(' ');
            foreach (var role in roles)
            {
                if (configuredAdminRoles.Contains(role))
                {
                    var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
                    foreach (var customerRole in allCustomerRoles)
                    {
                        if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName)
                            eventMessage.Customer.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = customerRole });

                        if (customerRole.SystemName == NopCustomerDefaults.ForumModeratorsRoleName)
                            eventMessage.Customer.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = customerRole });
                    }
                    _customerService.UpdateCustomer(eventMessage.Customer);

                    break;
                }
            }
        }

        #endregion
    }
}