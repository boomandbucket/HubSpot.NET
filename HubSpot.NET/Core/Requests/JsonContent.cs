using System.Net.Http;
using System.Text;

namespace HubSpot.NET.Core.Requests
{
    public class JsonContent(string json, Encoding encoding) : StringContent(json, encoding, "application/json")
    {
        public JsonContent(string json) : this(json, Encoding.UTF8)
        {
        }
    }
}