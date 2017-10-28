using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Polly;
using System.Collections.Generic;
using System.Collections;

namespace tackapi
{
    public static class GetUseage
    {
        [FunctionName("GetUseage")]
        public static async void RunAsync([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            var httpClient = new HttpClient();
            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);

            await retryPolicy.ExecuteAsync(async () =>
            {
                var response = await httpClient
                  .DeleteAsync("https://emoncms.org/feed/timevalue.json?id=143908&apikey=f1dc4bad1c391025566a290a8f61f27a");
                response.EnsureSuccessStatusCode(); 
                var result = await response.Content.ReadAsStringAsync();
                log.Info($"usage data: {result}");
                Queue myQ = new Queue();
                myQ.Enqueue("Hello");
                myQ.Enqueue("World");
                myQ.Enqueue("!");
            });
        }
    }
}
