using RestSharp;

namespace HubSpot.NET.Core
{
    public static class RestSharpExtensions
    {
        public static bool IsSuccessful(this RestResponse response)
        {
            return (int) response.StatusCode >= 200 
                   && (int) response.StatusCode <= 299 
                   && response.ResponseStatus == ResponseStatus.Completed;
        }
    }
}
