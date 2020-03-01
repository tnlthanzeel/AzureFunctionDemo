using System;
using System.Threading.Tasks;
using GitHubMonitorApp.AppConstants;
using GitHubMonitorApp.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace GitHubMonitorApp
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [Table(TableTorage.Table.Name.ToDos, Connection = "AzureWebJobsStorage")]CloudTable todoTable,
             ILogger log)
        {
            var query = new TableQuery<Todo>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            var deleted = 0;

            foreach (var todo in segment)
            {
                if (todo.IsCompleted)
                {
                    await todoTable.ExecuteAsync(TableOperation.Delete(todo));
                    deleted++;
                }
            }
            log.LogInformation($"Deleted {deleted} items as {DateTime.Now}");
        }
    }
}
