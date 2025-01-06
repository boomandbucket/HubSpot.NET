namespace HubSpot.NET.Api.Properties
{
    using HubSpot.NET.Api.Properties.Dto;
    using HubSpot.NET.Core.Interfaces;
    using RestSharp;

    public class HubSpotCompaniesPropertiesApi(IHubSpotClient client) : IHubSpotCompanyPropertiesApi
    {
        public PropertiesListHubSpotModel<CompanyPropertyHubSpotModel> GetAll()
        {
            var path = $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}";

            return client.ExecuteList<PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>>(path, convertToPropertiesSchema: false);
        }

        public CompanyPropertyHubSpotModel Create(CompanyPropertyHubSpotModel property)
        {
            var path = $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}";

            return client.Execute<CompanyPropertyHubSpotModel>(path, property, Method.Post, convertToPropertiesSchema: false);
        }

        public CompanyPropertyHubSpotModel Update(CompanyPropertyHubSpotModel property)
        {
            var path = $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}/named/{property.Name}";

            return client.Execute<CompanyPropertyHubSpotModel>(path, property, Method.Put, convertToPropertiesSchema: false);
        }

        public void Delete(string propertyName)
        {
            var path = $"{new PropertiesListHubSpotModel<CompanyPropertyHubSpotModel>().RouteBasePath}/named/{propertyName}";

            client.Execute(path, method: Method.Delete, convertToPropertiesSchema: true);
        }
    }
}