using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubMonitorApp.Models.ToDo
{
    public class TodoModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }
}
