using System;
using System.Threading.Tasks;
using GitHubMonitorApp.AppConstants;
using GitHubMonitorApp.Models.ToDo;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GitHubMonitorApp
{
    public static class QueueListeners
    {
        [FunctionName("QueueListeners")]
        public static async Task Run([QueueTrigger(TableTorage.Table.Name.ToDos, Connection = "AzureWebJobsStorage")]TodoModel todoModel,
            [Blob(TableTorage.Table.Name.ToDos, Connection = "AzureWebJobsStorage")]CloudBlobContainer container,
             ILogger log)
        {
            // create blob container if does not eists
            await container.CreateIfNotExistsAsync();
            
            var blob = container.GetBlockBlobReference($"{todoModel.Id}.txt");
            await blob.UploadTextAsync($"created a new task: {todoModel.TaskDescription}");
            log.LogInformation($"C# Queue trigger function processed: {todoModel.TaskDescription}");
        }
    }
}
