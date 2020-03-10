using System;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                msg.RequestUri = new Uri(msg.RequestUri + "/City?where="+ HttpUtility.UrlEncode("{\"objectId\":\"ETDFsnewML\"}"));

                HttpResponseMessage responseMessage = GetResponse(httpClient,msg).Result;
                    
                result = responseMessage.Content.ToString();
            }
            Console.WriteLine(result);
            Console.ReadKey();
        }

        static async Task<HttpResponseMessage> GetResponse(HttpClient httpClient, HttpRequestMessage msg)
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

            return message;
        }
    }
}