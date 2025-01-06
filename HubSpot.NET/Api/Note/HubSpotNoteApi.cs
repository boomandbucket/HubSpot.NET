namespace HubSpot.NET.Api.Note
{
    using System;
    using System.Net;
    using HubSpot.NET.Api.Note.Dto;
    using HubSpot.NET.Core;
    using HubSpot.NET.Core.Extensions;
    using HubSpot.NET.Core.Interfaces;
    using RestSharp;

    public class HubSpotNoteApi(IHubSpotClient client) : IHubSpotNoteApi
    {
        /// <summary>
        /// Creates a note
        /// </summary>
        /// <param name="entity">The note to create</param>
        /// <returns>The created note (with ID set)</returns>
        public NoteHubSpotResponseModel Create(NoteHubSpotRequestModel entity)
        {
            var path = $"{entity.RouteBasePath}";
            var data = client.Execute<NoteHubSpotResponseModel>(path, entity, Method.Post, false);
            return data;
        }
    }
}
