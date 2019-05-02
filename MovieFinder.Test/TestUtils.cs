using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MovieFinder.Test
{
    class TestUtils
    {
        //Url to connect API
        static string WEB_API_URL = "http://localhost:43268/";
        static IDictionary testParams;
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            //WEB_API_URL = testContext.Properties["webApiUrl"].ToString();
            testParams = testContext.Properties as IDictionary;
        }
        public static string GetParamValue(string name)
        {
            return testParams[name].ToString();
        }
        public static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(WEB_API_URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        public static StringContent GetJsonObjectAsContent<T>(T data)
        {
            var content = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            return stringContent;
        }
    }
}

