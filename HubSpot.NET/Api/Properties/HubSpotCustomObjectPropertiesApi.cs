namespace HubSpot.NET.Api.Properties
{
    using Dto;
    using Core.Interfaces;
    using RestSharp;

    public class HubSpotCustomObjectPropertiesApi(IHubSpotClient client) : IHubSpotCustomObjectPropertiesApi
    {
        public T GetProperty<T>(string customObjectName, string customObjectProperty) where T : CustomObjectPropertyHubSpotModel, new()
        {
            var path = $"{new PropertiesListHubSpotModel<CustomObjectPropertyHubSpotModel>().RouteBasePath}/{customObjectName}/{customObjectProperty}";
            var result = client.Execute<T>(path, null, Method.Get, convertToPropertiesSchema: false);
            return result;
        }

        public T UpdateProperty<T>(string customObjectName, string customObjectProperty, CustomObjectPropertyHubSpotModel property) where T : CustomObjectPropertyHubSpotModel, new()
        {
            var path = $"{new PropertiesListHubSpotModel<CustomObjectPropertyHubSpotModel>().RouteBasePath}/{customObjectName}/{customObjectProperty}";
            return client.Execute<T>(path, property, Method.Patch, convertToPropertiesSchema: false);
        }
    }
}