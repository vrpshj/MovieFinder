using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieFinder.Utils
{
    /// <summary>
    /// HTTP client extension
    /// </summary>
    public class HttpClientExtended:HttpClient
    {
        public string ProviderName { get; set; }
    }
}
