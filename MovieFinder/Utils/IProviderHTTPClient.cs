using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieFinder.Utils
{
    public interface IProviderHTTPClient
    {
        List<HttpClientExtended> GetProviderHTTPClients();
    }
}
