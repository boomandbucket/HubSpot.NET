using System.Collections.Generic;
using System.Linq;
using HubSpot.NET.Api.Associations.Dto;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.Associations;

public class HubSpotAssociationsApi(IHubSpotClient client) : IHubSpotAssociationsApi
{
    public AssociationTypeListHubSpotModel ListTypes(string fromObjectType, string toObjectType)
    {
        var associationPath =
            $"/crm/v4/associations/{fromObjectType}/{toObjectType}/labels";
        return client.ExecuteList<AssociationTypeListHubSpotModel>(associationPath, null, Method.Get, convertToPropertiesSchema: false);

    }

    public AssociationListHubSpotModel List(string objectType, string objectId, string toObjectType)
    {
        var associationPath =
            $"/crm/v4/objects/{objectType}/{objectId}/associations/{toObjectType}";
        return client.ExecuteList<AssociationListHubSpotModel>(associationPath, null, Method.Get, convertToPropertiesSchema: false);

    }

    public void AssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId)
    {
        var associationPath =
            $"/crm/v4/objects/{objectType}/{objectId}/associations/default/{toObjectType}/{toObjectId}";
        client.Execute(associationPath, null, Method.Get, convertToPropertiesSchema: false);
        
    }

    public void AssociationToObjectByLabel(string objectType, string objectId, string toObjectType, string toObjectId, string associationCategory, int associationTypeId)
    {
        var associationPath =
            $"/crm/v4/objects/{objectType}/{objectId}/associations/{toObjectType}/{toObjectId}";
        var label = new
        {
            associationCategory,
            associationTypeId
        };
        var body = new[] {label};
        client.Execute(associationPath, body, Method.Put, convertToPropertiesSchema: false);
        
    }

    public void DeleteAssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId)
    {
        var associationPath =
            $"/crm/v4/objects/{objectType}/{objectId}/associations/{toObjectType}/{toObjectId}";
        client.Execute(associationPath, null, Method.Delete, convertToPropertiesSchema: false);
    }

    public void DeleteAssociationToObjectByLabel(string objectType, string objectId, string toObjectType, string toObjectId, (string associationCategory, int associationTypeId)[] associations)
    {
        var associationPath =
            $"/crm/v4/associations/{objectType}/{toObjectType}/batch/labels/archive";
        var body = new DeleteAssociationToObjectByLabelRequest(objectId, toObjectId, associations);
        client.Execute(associationPath, body, Method.Post, convertToPropertiesSchema: false);
    }
}