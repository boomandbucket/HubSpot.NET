using HubSpot.NET.Api.Associations.Dto;

namespace HubSpot.NET.Core.Interfaces;

public interface IHubSpotAssociationsApi
{
    /// <summary>
    /// Gets association types between 2 object types
    /// </summary>
    /// <param name="fromObjectType">the type of the object you're fetching associations (e.g. contact) from.</param>
    /// <param name="toObjectType"> the type of the object you are fetching associations to.</param>
    AssociationTypeListHubSpotModel ListTypes(string fromObjectType, string toObjectType);

    /// <summary>
    /// Gets associations to a specific object type
    /// </summary>
    /// <param name="objectType">the type of the object you're fetching associations (e.g. contact).</param>
    /// <param name="objectId"> the ID of the record to find associations for.</param>
    /// <param name="toObjectType"> the type of the object you are fetching associations for.</param>
    AssociationListHubSpotModel List(string objectType, string objectId, string toObjectType);

    /// <summary>
    /// Adds the ability to associate via the default association
    /// See the PUT documentation here: https://developers.hubspot.com/docs/api/crm/associations
    /// </summary>
    /// <param name="objectType">the type of the object you're associating (e.g. contact).</param>
    /// <param name="objectId"> the ID of the record to associate.</param>
    /// <param name="toObjectType"> the ID of the record to associate.</param>
    /// <param name="toObjectId"> the ID of the record to associate.</param>
    void AssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId);

    /// <summary>
    /// Adds the ability to associate via the label
    /// See the PUT documentation here: https://developers.hubspot.com/docs/api/crm/associations.
    /// Important - this will overwrite existing associations between the objects - see https://developers.hubspot.com/docs/guides/api/crm/associations/associations-v4#update-record-association-labels
    /// </summary>
    /// <param name="objectType">the type of the object you're associating (e.g. contact).</param>
    /// <param name="objectId"> the ID of the record to associate.</param>
    /// <param name="toObjectType"> the ID of the record to associate.</param>
    /// <param name="toObjectId"> the ID of the record to associate.</param>
    /// <param name="associationCategory">Category type: HUBSPOT_DEFINED, INTEGRATOR_DEFINED, USER_DEFINED</param>
    /// <param name="associationTypeId">This is the ID of the label, can be hard to find, the url of the label in your settings is a good place to look</param>
    void AssociationToObjectByLabel(string objectType, string objectId, string toObjectType, string toObjectId,
        string associationCategory, int associationTypeId);

    /// <summary>
    /// Deletes an association between 2 objects
    /// See the DELETE documentation here: https://developers.hubspot.com/docs/guides/api/crm/associations/associations-v4#remove-record-associations
    /// </summary>
    /// <param name="objectType">the type of the object that is associated from (e.g. contact).</param>
    /// <param name="objectId"> the ID of the record that is associated from.</param>
    /// <param name="toObjectType"> the type of the object that is associated to.</param>
    /// <param name="toObjectId"> the ID of the record that is associated to.</param>
    void DeleteAssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId);

    /// <summary>
    /// Deletes a specific label for an association leaving the association itself including other labels when applicable 
    /// See the PUT documentation here: https://developers.hubspot.com/docs/guides/api/crm/associations/associations-v4#remove-record-associations
    /// </summary>
    /// <param name="objectType">the type of the object that is associated from (e.g. contact).</param>
    /// <param name="objectId"> the ID of the record that is associated from.</param>
    /// <param name="toObjectType"> the type of the object that is associated to.</param>
    /// <param name="toObjectId"> the ID of the record that is associated to.</param>
    /// <param name="associations">A tuple with all the associationTypes that are to be removed</param>
    void DeleteAssociationToObjectByLabel(string objectType, string objectId, string toObjectType, string toObjectId, (string associationCategory, int associationTypeId)[] associations);
}