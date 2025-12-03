using System;
using System.Threading.Tasks;

namespace HubSpot.NET.Core.OAuth
{
    using HubSpot.NET.Core;
    using HubSpot.NET.Core.OAuth.Dto;
	using Newtonsoft.Json;
	using RestSharp;
    using System.Collections.Generic;
    using System.Text;

	public class HubSpotOAuthApi
    {
        public string ClientId { get; protected set; }
        private string _clientSecret;
        private readonly string _basePath;

        public virtual string MidRoute => "oauth/v1/token";

        private readonly Dictionary<OAuthScopes, string> OAuthScopeNameConversions = new()
        {
            { OAuthScopes.Automation , "automation" },
            { OAuthScopes.BusinessIntelligence, "business-intelligence" },
            { OAuthScopes.Contacts , "contacts" },
            { OAuthScopes.Content , "content" },
            { OAuthScopes.ECommerce , "e-commerce" },
            { OAuthScopes.Files , "files" },
            { OAuthScopes.Forms , "forms" },
            { OAuthScopes.HubDb , "hubdb" },
            { OAuthScopes.IntegrationSync , "integration-sync" },
            { OAuthScopes.Reports , "reports" },
            { OAuthScopes.Social , "social" },
            { OAuthScopes.Tickets , "tickets" },
            { OAuthScopes.Timeline , "timeline" },
            { OAuthScopes.TransactionalEmail , "transactional-email" }
        };


        public HubSpotOAuthApi(string basePath, string clientId, string clientSecret)
        {
            _basePath = basePath;
            ClientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<HubSpotToken> AuthorizeAsync(string authCode, string redirectUri)
        {
            var model = new RequestTokenHubSpotModel()
            {
                ClientId = ClientId,
                ClientSecret = _clientSecret,
                Code = authCode,
                RedirectUri = redirectUri
            };

            var token = await InitiateRequestAsync(model, _basePath);

            return token;
        }

        [Obsolete("Use AuthorizeAsync instead.")]
        public HubSpotToken Authorize(string authCode, string redirectUri) => AuthorizeAsync(authCode, redirectUri).GetAwaiter().GetResult();

        public async Task<HubSpotToken> RefreshAsync(string redirectUri, HubSpotToken token)
        {
            var model = new RequestRefreshTokenHubSpotModel()
            {
                ClientId = ClientId,
                ClientSecret = _clientSecret,
                RedirectUri = redirectUri,
                RefreshToken = token.RefreshToken
            };

            var refreshToken = await InitiateRequestAsync(model, _basePath);

            return refreshToken;
        }

        [Obsolete("Use RefreshAsync instead.")]
        public HubSpotToken Refresh(string redirectUri, HubSpotToken token) => RefreshAsync(redirectUri, token).GetAwaiter().GetResult();

        public void UpdateCredentials(string id, string secret)
        {
            ClientId = id;
            _clientSecret = secret;
        }

        private async Task<HubSpotToken> InitiateRequestAsync<K>(K model, string basePath, params OAuthScopes[] scopes)
        {
            var client = new RestClient(basePath);

            var builder = new StringBuilder();
            foreach (var scope in scopes)
            {
                if (builder.Length > 0)
                {
                    builder.Append("%20");
                }

                builder.Append(OAuthScopeNameConversions[scope]);
            }

            var request = new RestRequest(MidRoute);

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(model));

            var bodyBuilder = new StringBuilder();
            foreach(var pair in dict)
            {
                if (bodyBuilder.Length > 0)
                {
                    bodyBuilder.Append("&");
                }

                bodyBuilder.Append($"{pair.Key}={pair.Value}");
            }

            request.AddStringBody(bodyBuilder.ToString(), ContentType.FormUrlEncoded);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            if (builder.Length > 0)
            {
                request.AddQueryParameter("scope", builder.ToString());
            }

            var serverResponse = await client.ExecuteAsync<HubSpotToken>(request);

            if (serverResponse.ResponseStatus != ResponseStatus.Completed)
            {
                throw new HubSpotException("Server did not respond to authorization request. Content: " + serverResponse.Content, new HubSpotError(serverResponse.StatusCode, serverResponse.Content), serverResponse.Content);
            }

            if (serverResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new HubSpotException("Error generating authentication token.", JsonConvert.DeserializeObject<HubSpotError>(serverResponse.Content), serverResponse.Content);
            }

            return serverResponse.Data;
        }
    }
}