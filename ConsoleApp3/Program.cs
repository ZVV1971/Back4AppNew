using System;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ConsoleApp3
{
    class Program
    {
        private static readonly Uri baseUri = new Uri("https://parseapi.back4app.com/classes");
        private static string result = null;

        static void Main(string[] args)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = baseUri;
                HttpRequestMessage msg = new HttpRequestMessage
                {
                    RequestUri = new Uri(httpClient.BaseAddress,""),
                    Method = HttpMethod.Get,
                    Headers =
                    {
                        //{ HttpRequestHeader.Authorization.ToString(), "Basic " + "VWxhZHppbWlyX1pha2hhcmVua2E6am1sNDIxX0pNTDQyMTM="},
                        //{ HttpRequestHeader.Accept.ToString(), "application/json" }
                        { "X-Parse-Application-Id", "XW0rXBZxcGJcl3zkoWrh3w6bryBpkXnG36CvzOPV" },
                        { "X-Parse-REST-API-Key", "eOMGmD5HW5sJ0dQqlFYS7sQbVV2cYKJMU8eSWbI9" }
                    }
                };
                msg.RequestUri = new Uri(msg.RequestUri + "/City");// +
                    //"?where="+ HttpUtility.UrlEncode("{\"objectId\":\"ETDFsnewML\"}"));

                string responseMessage = GetResponse(httpClient, msg).Result;

                JObject resp_JSON = JObject.Parse(responseMessage);
                JToken resp = resp_JSON["results"];

                if (String.IsNullOrEmpty(resp.ToString()))
                {
                    result = "Response does not contain any results";
                }

                JArray arr = JArray.Parse(resp.ToString());
                result = responseMessage;
                foreach (JToken item in arr)
                {
                    foreach (JProperty tkn in item.Children())
                    {
                        Console.WriteLine("Name:\t{0}\tValue:{1}", tkn.Name, tkn.Value.ToString());
                    }
                }
            }
            Console.WriteLine(result);
            Console.ReadKey();
        }

        static async Task<string> GetResponse(HttpClient httpClient, HttpRequestMessage msg)
        {
            HttpResponseMessage message = null;

            try
            {
                message = await httpClient.SendAsync(msg);
            }
            catch (Exception ex)
            {
                message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Headers = { { HttpResponseHeader.ContentType.ToString(), "text/plain" } },
                    Content = new StringContent(ex.Message)
                };
            }
            string s = await message.Content.ReadAsStringAsync();
            return s;
        }
    }
}