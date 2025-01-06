using HubSpot.NET.Api.Associations.Dto;

namespace HubSpot.NET.Core.Interfaces;

public interface IHubSpotAssociationsApi
{
    AssociationListHubSpotModel List(string objectType, string objectId, string toObjectType);
    void AssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId);

    void AssociationToObjectByLabel(string objectType, string objectId, string toObjectType, string toObjectId,
        string associationCategory, int associationTypeId);
}