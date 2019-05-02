using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieFinder.Models
{
    /// <summary>
    /// Model class to store Provider details
    /// </summary>
    public class MovieProviderAPIURL
    {
        public string ProviderName { get; set; }
        public string Url { get; set; }
        public string AccessToken { get; set; }
    }

    public class MovieProvidersAPIURL
    {

        public List<MovieProviderAPIURL> MovieProviders_APIURL { get; set; }
    }


}
