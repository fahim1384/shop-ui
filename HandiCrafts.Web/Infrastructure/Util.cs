using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace HandiCrafts.Web.Infrastructure
{
    public class Util
    {

    }

    public class HttpClientRequestHandler : IRequestHandler
    {
        public string GetReleases(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add(RequestConstants.UserAgent, RequestConstants.UserAgentValue);

                var response = httpClient.GetStringAsync(new Uri(url)).Result;

                return response;
            }
        }
    }

    public static class RequestConstants
    {
        public const string BaseUrl = "https://service.tabrizhandicrafts.com";
        public const string Url = "https://service.tabrizhandicrafts.com/api/Account/Customer_GetActivationCodeForLogin?userId=60&activationCode=1234";
        public const string UserAgent = "User-Agent";
        public const string UserAgentValue = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
    }

    public interface IRequestHandler
    {
        //Method to get the releases of the repo provided by the url
        string GetReleases(string url);
    }
}
