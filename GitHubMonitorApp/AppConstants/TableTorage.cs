using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubMonitorApp.AppConstants
{
    public static class TableTorage
    {
        public static class Table
        {
            public static class Name
            {

            public const string ToDos = "todos";
            }

            public static class PartitionKeys
            {

                public const string ToDos = "TODO";
            }
        }
    }
}
