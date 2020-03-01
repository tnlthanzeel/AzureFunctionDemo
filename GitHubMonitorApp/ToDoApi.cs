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

namespace GitHubMonitorApp
{
    public static class ToDoApi
    {
        static List<TodoModel> todoModelsList = new List<TodoModel>();

        [FunctionName("CreateToDo")]
        public static async Task<IActionResult> CreateToDo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("creating a new todo list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<ToDoCreateModel>(requestBody);

            var todo = new TodoModel { TaskDescription = input.TaskDescription };
            todoModelsList.Add(todo);
            return new OkObjectResult(todo);
        }

        [FunctionName("GetToDos")]
        public static IActionResult GetToDos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("getting todo list items");
            return new OkObjectResult(todoModelsList);
        }

        [FunctionName("GetToDoById")]
        public static IActionResult GetToDoById(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
          ILogger log, string id)
        {
            var todo = todoModelsList.FirstOrDefault(f => f.Id == id);

            if (todo is null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(todo);
        }

        [FunctionName("UpdateToDo")]
        public static async Task<IActionResult> UpdateToDo(
         [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
         ILogger log, string id)
        {
            var todo = todoModelsList.FirstOrDefault(f => f.Id == id);

            if (todo is null)
            {
                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<ToDoUpdateModel>(requestBody);

            todo.IsCompleted = updated.IsCompleted;

            if (!string.IsNullOrEmpty(updated.TaskDescription))
            {
                todo.TaskDescription = updated.TaskDescription;
            }

            return new OkObjectResult(todo);
        }


        [FunctionName("DeleteToDo")]
        public static IActionResult DeleteToDo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
        ILogger log, string id)
        {
            var todo = todoModelsList.FirstOrDefault(f => f.Id == id);

            if (todo is null)
            {
                return new NotFoundResult();
            }

            todoModelsList.Remove(todo);

            return new OkResult();
        }
    }
}
