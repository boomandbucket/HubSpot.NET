using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Owner.Dto
{

    /// <summary>
    /// Models a set of owners in HubSpot
    /// </summary>
    [DataContract]
    public class OwnerListHubSpotModel<T> : IHubSpotModel, ICollection<T> where T: OwnerHubSpotModel, new()
    {
        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; }

        private List<T> Owners { get; } = new List<T>();

        public string RouteBasePath => "/crm/v3";

        public bool IsNameValue => false;
        public virtual void ToHubSpotDataEntity(ref dynamic converted)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Owners.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Owners.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            Owners.AddRange(items);
        }

        public void Clear()
        {
            Owners.Clear();
        }

        public bool Contains(T item)
        {
            return Owners.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Owners.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return Owners.Remove(item);
        }

        public int Count => Owners.Count;
        public bool IsReadOnly => false;
    }
}
