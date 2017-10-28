using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http.Formatting;

namespace tackapi
{
    public static class UsageChange
    {
        [FunctionName("usageChange")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            return req.CreateResponse(
                HttpStatusCode.OK, 
                new CurrentUsage() { Speed = 100},
                JsonMediaTypeFormatter.DefaultMediaType);
        }
    }

    internal class CurrentUsage
    {
        public int Speed { get; set; }
        public CurrentUsage()
        {
        }
    }
}
