using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQL_Database_Tools.Core.Entities
{
    public sealed class ComparisonResult
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public Action Action { get; set; }

        public ComparisonResult(string schema, string name, Action action)
        {
            Schema = schema;
            Name = name;
            Action = action;
        }
    }
}
