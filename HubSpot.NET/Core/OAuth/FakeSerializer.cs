using RestSharp;

namespace HubSpot.NET.Core.OAuth
{
	using RestSharp.Serializers;

	internal class FakeSerializer : ISerializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public ContentType ContentType { get; set; }

        internal FakeSerializer()
        {
            ContentType = ContentType.FormUrlEncoded;
        }
        public string Serialize(object obj)
        {
            return obj.ToString();
        }
    }
}