using System.Collections.Generic;
using System.Net;
using HubSpot.NET.Core;
using RestSharp;
using HubSpot.NET.Api.Owner.Dto;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Owner
{

    public class HubSpotOwnerApi : IHubSpotOwnerApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotOwnerApi(IHubSpotClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets all owners within your HubSpot account
        /// </summary>
        /// <returns>The set of owners</returns>
        public OwnerListHubSpotModel<T> GetAll<T>(OwnerGetAllRequestOptions opts = null)
            where T : OwnerHubSpotModel, new()
        {
            string path = $"{new OwnerHubSpotModel().RouteBasePath}/owners";

            path = path.SetQueryParam("limit", 100);
            if (opts != null)
            {
                if (opts.IncludeInactive)
                    path = path.SetQueryParam("includeInactive", "true");
                if (!string.IsNullOrWhiteSpace(opts.EmailAddress))
                    path = path.SetQueryParam("email", opts.EmailAddress);
            }

            var owners = new List<T>();
            var currentPage = new OwnerListHubSpotModel<T>();

            do
            {
                var pagedPath = currentPage.Paging != null ? $"{path}&after={currentPage.Paging.Next.After}" : path;
                currentPage = _client.ExecuteList<OwnerListHubSpotModel<T>>(pagedPath, convertToPropertiesSchema: false);
                owners.AddRange(currentPage.Owners);
            } while (currentPage.Paging?.Next != null);

            var ownersListHubSpotModel = new OwnerListHubSpotModel<T>
            {
                Owners = owners
            };
            return ownersListHubSpotModel;
        }

        /// <summary>
        /// Gets a single owner by ID
        /// </summary>
        /// <param name="ownerId">ID of the owner</param>
        /// <typeparam name="T">Implementation of OwnerHubSpotModel</typeparam>
        /// <returns>The owner entity or null if the owner does not exist</returns>
        public T GetById<T>(long ownerId) where T: OwnerHubSpotModel, new()
        {
            var path = $"{new OwnerHubSpotModel().RouteBasePath}/owners/{ownerId}";

            try
            {
                var data = _client.Execute<T>(path, Method.GET, convertToPropertiesSchema: false);
                return data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }
    }
}