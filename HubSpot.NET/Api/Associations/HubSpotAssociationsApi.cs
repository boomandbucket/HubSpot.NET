using System.Collections.Generic;
using System.Linq;
using HubSpot.NET.Api.Associations.Dto;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.Associations;

public class HubSpotAssociationsApi : IHubSpotAssociationsApi
{
    private readonly IHubSpotClient _client;
    public HubSpotAssociationsApi(IHubSpotClient client)
    {
        _client = client;
    }
    
    /// <summary>
    /// Adds the ability to associate via the default association
    /// See the PUT documentation here: https://developers.hubspot.com/docs/api/crm/associations
    /// </summary>
    /// <param name="objectType">the type of the object you're associating (e.g. contact).</param>
    /// <param name="objectId"> the ID of the record to associate.</param>
    /// <param name="toObjectType"> the ID of the record to associate.</param>
    /// <param name="toObjectId"> the ID of the record to associate.</param>
    public void AssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId)
    {
        var associationPath =
            $"/crm/v4/objects/{objectType}/{objectId}/associations/default/{toObjectType}/{toObjectId}";
        _client.Execute(associationPath, null, Method.Put, convertToPropertiesSchema: false);
        
    }

    /// <summary>
    /// Adds the ability to associate via the label association association
    /// See the PUT documentation here: https://developers.hubspot.com/docs/api/crm/associations
    /// </summary>
    /// <param name="objectType">the type of the object you're associating (e.g. contact).</param>
    /// <param name="objectId"> the ID of the record to associate.</param>
    /// <param name="toObjectType"> the ID of the record to associate.</param>
    /// <param name="toObjectId"> the ID of the record to associate.</param>
    /// <param name="associationCategory">Category type: HUBSPOT_DEFINED, INTEGRATOR_DEFINED, USER_DEFINED</param>
    /// <param name="associationTypeId">This is the ID of the label, can be hard to find, the url of the label in your settings is a good place to look</param>
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
        _client.Execute(associationPath, body, Method.Put, convertToPropertiesSchema: false);
        
    }

    /// <summary>
    /// Retrieves alls associations of a given object to the specified object type
    /// </summary>
    /// <param name="objectType">Object type id of the object whose associations you're retrieving (e.g. "0-2" for company)</param>
    /// <param name="objectId">Object id of the object whose associations you're retrieving</param>
    /// <param name="toObjectType">Object type id of the associations to retrieve (e.g. "0-1" for contact)</param>
    /// <returns></returns>
    public T GetAssociations<T>(string objectType, string objectId, string toObjectType) where T : AssociationListHubSpotModel, new()

    {
        var associationPath = $"/crm/v4/objects/{objectType}/{objectId}/associations/{toObjectType}";
        var associations = _client.ExecuteList<T>(associationPath, Method.Get, convertToPropertiesSchema: false);

        return associations;
    }

    /// <summary>
    /// Adds the ability to batch associate via the default association
    /// </summary>
    /// <param name="objectType">the type of the object you're associating (e.g. contact)</param>
    /// <param name="toObjectType">the type of the object you're associating to(e.g. contact)</param>
    /// <param name="associations">pairs of object ids to associate</param>
    /// <returns></returns>
    public void BatchCreateAssociations(string objectType, string toObjectType, IEnumerable<(string objectId, string toObjectId)> associations)
    {
        var associationPath =
            $"/crm/v4/associations/{objectType}/{toObjectType}/batch/associate/default";
        
        var body = new
        {
            inputs = associations.Select(a => new
            {
                from = new { id = a.objectId }, 
                to = new { id = a.toObjectId }
            })
        };
        _client.Execute(associationPath, body, Method.Post, convertToPropertiesSchema: false);
    }

    /// <summary>
    /// Adds the ability to batch remove associations
    /// </summary>
    /// <param name="objectType">the type of the object you're associating (e.g. contact)</param>
    /// <param name="toObjectType">the type of the object you're associating to(e.g. contact)</param>
    /// <param name="associations">pairs of object ids to associate</param>
    /// <returns></returns>
    public void BatchDeleteAssociations(string objectType, string toObjectType, IEnumerable<(string objectId, string toObjectId)> associations)
    {
        var associationPath =
            $"/crm/v4/associations/{objectType}/{toObjectType}/batch/archive";
        
        var body = new
        {
            inputs = associations.Select(a => new
            {
                from = new { id = a.objectId }, 
                to = new[] { new { id = a.toObjectId } }
            })
        };
        _client.Execute(associationPath, body, Method.Post, convertToPropertiesSchema: false);
    }
}