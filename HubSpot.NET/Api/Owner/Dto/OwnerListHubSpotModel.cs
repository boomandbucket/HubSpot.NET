using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Owner.Dto
{

    /// <summary>
    /// Models a set of owners in HubSpot
    /// </summary>
    [DataContract]
    public class OwnerListHubSpotModel<T> : IHubSpotModel where T: OwnerHubSpotModel, new()
    {
        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; }

        [DataMember(Name = "results")]
        public IReadOnlyList<T> Owners { get; set; } = new List<T>();

        public string RouteBasePath => "/crm/v3";

        public bool IsNameValue => false;
        public virtual void ToHubSpotDataEntity(ref dynamic converted)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

    }
}
