using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.ContactList.Dto
{
    [DataContract]
    public class ContactListUpdateResponseModel : IHubSpotModel
    {
        [DataMember(Name = "discarded")] 
        public List<long> Discarded = new();

        [DataMember(Name = "invalidVids")] 
        public List<long> InvalidContactIds = new();

        [DataMember(Name = "updated")] 
        public List<long> UpdatedContactIds = new();

        [IgnoreDataMember]
        public bool IsNameValue => false;
        
        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }

        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        [IgnoreDataMember]
        public string RouteBasePath => "/contacts/v1/lists"; 
    }
}