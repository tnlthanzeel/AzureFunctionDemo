using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GitHubMonitorApp.Models.ToDo;
using System.Collections.Generic;
using System.Linq;
using GitHubMonitorApp.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using GitHubMonitorApp.AppConstants;
using Microsoft.WindowsAzure.Storage;
using System.Net;

namespace GitHubMonitorApp
{
    public static class ToDoApi
    {
        [FunctionName("CreateToDo")]
        public static async Task<IActionResult> CreateToDo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table(TableTorage.Table.Name.ToDos, Connection = "AzureWebJobsStorage")]IAsyncCollector<Todo> todoTable,
            ILogger log)
        {
            log.LogInformation("creating a new todo list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<ToDoCreateModel>(requestBody);

            var todo = new TodoModel { TaskDescription = input.TaskDescription };

            await todoTable.AddAsync(todo.ToTableEntity());

            return new OkObjectResult(todo);
        }

        [FunctionName("GetToDos")]
        public static async Task<IActionResult> GetToDos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            [Table(TableTorage.Table.Name.ToDos, Connection = "AzureWebJobsStorage")]CloudTable todoTable,
           ILogger log)
        {
            log.LogInformation("getting todo list items");
            var query = new TableQuery<Todo>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            return new OkObjectResult(segment.Select(Mappings.ToDoModel));
        }

        [FunctionName("GetToDoById")]
        public static IActionResult GetToDoById(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,

           [Table(TableTorage.Table.Name.ToDos, TableTorage.Table.PartitionKeys.ToDos/*partiotion key*/, "{id}" /*row key*/, Connection = "AzureWebJobsStorage")]Todo todoTable,
          ILogger log, string id)
        {

            if (todoTable is null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(todoTable.ToDoModel());
        }

        [FunctionName("UpdateToDo")]
        public static async Task<IActionResult> UpdateToDo(
         [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
          [Table(TableTorage.Table.Name.ToDos, Connection = "AzureWebJobsStorage")]CloudTable todoTable,
         ILogger log, string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<ToDoUpdateModel>(requestBody);

            var findOperation = TableOperation.Retrieve<Todo>(TableTorage.Table.PartitionKeys.ToDos, id);
            var findResult = await todoTable.ExecuteAsync(findOperation);

            if (findResult.Result is null)
            {
                return new NotFoundResult();
            }

            var existingRow = findResult.Result as Todo;

            existingRow.IsCompleted = updated.IsCompleted;
            if (!string.IsNullOrEmpty(updated.TaskDescription))
            {
                existingRow.TaskDescription = updated.TaskDescription;
            }

            var replaceOperation = TableOperation.Replace(existingRow);
            await todoTable.ExecuteAsync(replaceOperation);

            return new OkObjectResult(existingRow.ToDoModel());
        }


        [FunctionName("DeleteToDo")]
        public static async Task<IActionResult> DeleteToDo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
         [Table(TableTorage.Table.Name.ToDos, Connection = "AzureWebJobsStorage")]CloudTable todoTable,
        ILogger log, string id)
        {

            var deleteOperation = TableOperation.Delete(new TableEntity() { PartitionKey = TableTorage.Table.PartitionKeys.ToDos, RowKey = id, ETag = "*" });

            try
            {
                var deleteResult = await todoTable.ExecuteAsync(deleteOperation);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {

                return new NotFoundResult();
            }

            return new OkResult();
        }
    }
}
