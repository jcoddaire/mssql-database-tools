using System;

namespace MSSQL_Database_Tools.Core.Entities
{
    public sealed class SqlObject
    {
        public string DBName { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DBObjectType ObjectType { get; set; }

        public SqlObject()
        {
            Schema = "dbo";
            Name = string.Empty;
            Text = string.Empty;
            DBName = string.Empty;
            ObjectType = DBObjectType.PROCEDURE;
        }
    }
}
