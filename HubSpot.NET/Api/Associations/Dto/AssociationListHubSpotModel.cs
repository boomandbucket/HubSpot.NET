using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;

namespace HubSpot.NET.Api.Associations.Dto
{
    public class AssociationListHubSpotModel: IHubSpotModel
    {
        [DataMember(Name = "results"), JsonProperty(PropertyName = "results")]
        public IList<AssociationHubSpotModel> Associations { get; set; } = new List<AssociationHubSpotModel>();

        public bool IsNameValue => false;

        public string RouteBasePath => "/crm/v4/objects/{objectType}/{objectId}/associations/{toObjectType}";

        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }
    }

    public class AssociationHubSpotModel
    {
        [DataMember(Name = "toObjectId")]
        public Int64 ToObjectId { get; set; }
        [DataMember(Name = "associationTypes")]
        public IList<AssociationType> AssociationTypes { get; set; } = new List<AssociationType>();

    }

    public class AssociationType
    {
        [DataMember(Name = "category")]
        public AssociationCategory Category { get; set; }
        [DataMember(Name = "typeId")]
        public int TypeId { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
    }

    public enum AssociationCategory
    {
        HUBSPOT_DEFINED,
        USER_DEFINED
    }
}
