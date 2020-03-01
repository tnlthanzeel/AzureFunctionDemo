using GitHubMonitorApp.AppConstants;
using GitHubMonitorApp.Models.ToDo;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubMonitorApp.Entities
{
    public class Todo : TableEntity
    {
        public DateTime CreatedTime { get; set; }
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }


    public static class Mappings
    {
        public static Todo ToTableEntity(this TodoModel todo)
        {
            return new Todo()
            {
                PartitionKey = TableTorage.Table.PartitionKeys.ToDos,
                RowKey = todo.Id,
                CreatedTime = todo.CreatedTime,
                IsCompleted = todo.IsCompleted,
                TaskDescription = todo.TaskDescription
            };
        }

        public static TodoModel ToDoModel(this Todo todo)
        {
            return new TodoModel()
            {
                Id = todo.RowKey,
                CreatedTime = todo.CreatedTime,
                IsCompleted = todo.IsCompleted,
                TaskDescription = todo.TaskDescription
            };
        }
    }
}
