using Microsoft.Extensions.Options;
using MovieFinder.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MovieFinder.Utils
{
    public class ProviderHTTPClient: IProviderHTTPClient
    {
        private readonly IOptions<List<MovieProviderAPIURL>> providerDetails;
        private List<HttpClientExtended> providerClients;
        public ProviderHTTPClient(IOptions<List<MovieProviderAPIURL>> providerConfig)
        {
            providerDetails = providerConfig;
            CreateClients();
        }
        /// <summary>
        /// method to create HTTP client
        /// </summary>
        private void CreateClients()
        {
            providerClients = new List<HttpClientExtended>();
            foreach(var provider in providerDetails.Value)
            {
                var client = new HttpClientExtended();
                client.ProviderName = provider.ProviderName;
                client.BaseAddress = new Uri(provider.Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("x-access-token", provider.AccessToken);
                providerClients.Add(client);
            }
        }
        public List<HttpClientExtended>  GetProviderHTTPClients()
        {
            return providerClients;
        }



    }
}
