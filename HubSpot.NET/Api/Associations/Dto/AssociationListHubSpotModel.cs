using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;

namespace HubSpot.NET.Api.Associations.Dto;

/// <summary>
/// Models the associations of a HubSpot Object to other HubSpot Objects
/// </summary>
[DataContract]
public class AssociationListHubSpotModel : IHubSpotModel
{
    [JsonProperty(PropertyName = "results")]
    public List<Association> Associations { get; set; } = new ();

    public class Association
    {
        [JsonProperty(PropertyName = "toObjectId")]
        public long AssociatedObjectId { get; set; }
        public List<AssociationType> AssociationTypes { get; set; } = new();
    }

    public class AssociationType
    {
        public string Category { get; set; } // HUBSPOT_DEFINED or USER_DEFINED
        public long TypeId { get; set; }
        public string Label { get; set; } // e.g. "Contact with Primary Company"
    }


    public bool IsNameValue => false;

    public void ToHubSpotDataEntity(ref dynamic dataEntity) { }
    
    public void FromHubSpotDataEntity(dynamic hubspotData) { }
    
    public string RouteBasePath => "/crm/v4/objects/";
}