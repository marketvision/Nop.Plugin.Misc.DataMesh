﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.DataMesh.Infrastructure
{
    /// <summary>
    /// Represents registrar of OAuth2 external authentication
    /// </summary>
    public class OAuth2AuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            //add the OAuth2 authentication
            builder.AddOAuth(OAuth2AuthenticationDefaults.AuthenticationScheme, options =>
            {
                //configure the OAuth2 Client ID and Client Secret
                var settings = EngineContext.Current.Resolve<Settings>();
                string[] scopes = settings.Scopes?.Split(' ') ?? new string[] { };
                foreach (var scope in scopes)
                {
                    options.Scope.Add(scope);
                }
                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret;

                options.CallbackPath = new PathString(OAuth2AuthenticationDefaults.CallbackPath);
                options.ClaimsIssuer = OAuth2AuthenticationDefaults.ClaimsIssuer;
                options.SaveTokens = true;
                options.AuthorizationEndpoint = $"{settings.Authority}/connect/authorize";
                options.TokenEndpoint = $"{settings.Authority}/connect/token";
                options.UserInformationEndpoint = $"{settings.Authority}/connect/userinfo";

                options.Events = new OAuthEvents
                {
                    // The OnCreatingTicket event is called after the user has been authenticated and the OAuth middleware has
                    // created an auth ticket. We need to manually call the UserInformationEndpoint to retrieve the user's information,
                    // parse the resulting JSON to extract the relevant information, and add the correct claims.
                    OnCreatingTicket = async context =>
                    {
                        //try to retrieve user info
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        var responseBody = await response.Content.ReadAsStringAsync();
                        if (!response.IsSuccessStatusCode)
                        {
                            var logger = EngineContext.Current.Resolve<ILogger>();
                            logger.Error($"An error occurred while retrieving the OAuth2 user profile: {response.Headers.ToString()} {responseBody}.");
                            return;
                        }

                        //extract the user info object
                        var user = JObject.Parse(responseBody);

                        //set external identifier of the user as a claim with type ClaimTypes.NameIdentifier
                        var externalIdentifier = user?.Value<string>("sub");
                        if (!string.IsNullOrEmpty(externalIdentifier))
                            context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, externalIdentifier));

                        //set username of the user
                        var name = user?.Value<string>("given_name");
                        if (!string.IsNullOrEmpty(name))
                            context.Identity.AddClaim(new Claim(ClaimTypes.Name, name));

                        //set email
                        var email = user?.Value<string>("email");
                        if (!string.IsNullOrEmpty(email))
                            context.Identity.AddClaim(new Claim(ClaimTypes.Email, email));

                        //try to set avatar URL
                        var avatar = user?.Value<string>("picture");
                        if (!string.IsNullOrEmpty(avatar))
                            context.Identity.AddClaim(new Claim(OAuth2AuthenticationDefaults.AvatarClaimType, avatar));

                        //try to set avatar URL
                        var roles = user?.SelectToken("role")?.ToObject<string[]>();
                        if (roles != null)
                        {
                            foreach (var role in roles)
                            {
                                context.Identity.AddClaim(new Claim(context.Identity.RoleClaimType, role));
                            }
                        }
                    }
                };
            });
        }
    }
}