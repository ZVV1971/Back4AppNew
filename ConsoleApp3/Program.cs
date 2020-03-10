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
        private static readonly Uri baseUri = new Uri("http://ecsc00a0129f.epam.com:6443/api/public/tdm/v1/");
        private static string result = null;

        static void Main(string[] args)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = baseUri;
                HttpRequestMessage msg = new HttpRequestMessage
                {
                    RequestUri = new Uri(httpClient.BaseAddress, "projects"),
                    Method = HttpMethod.Get,
                    Headers =
                    {
                        { HttpRequestHeader.Authorization.ToString(), "Basic " + "VWxhZHppbWlyX1pha2hhcmVua2E6am1sNDIxX0pNTDQyMTM="},
                        { HttpRequestHeader.Accept.ToString(), "application/json" }
                    }
                };
                msg.RequestUri = new Uri(msg.RequestUri + HttpUtility.UrlPathEncode("?name=" + "OQ_PROJECT"));

                HttpResponseMessage responseMessage = GetResponse(httpClient,msg).Result;
                    
                result = responseMessage.Content.ToString();
            }


            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine(result);
            Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
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
