using System.Collections.Generic;
using HubSpot.NET.Api.Associations.Dto;

namespace HubSpot.NET.Core.Interfaces;

public interface IHubSpotAssociationsApi
{
    void AssociationToObject(string objectType, string objectId, string toObjectType, string toObjectId);

    void AssociationToObjectByLabel(string objectType, string objectId, string toObjectType, string toObjectId,
        string associationCategory, int associationTypeId);

    T GetAssociations<T>(string objectType, string objectId, string toObjectType) where T : AssociationListHubSpotModel, new();
}