using MSSQL_Database_Tools.Core.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Action = MSSQL_Database_Tools.Core.Entities.Action;

namespace MSSQL_Database_Tools.Core.Services
{
    public static class DatabaseService
    {
        /// <summary>
        /// Downloads the database objects (not data!) to the specified directory.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// database
        /// or
        /// filePath
        /// </exception>
        public static bool DownloadDatabase(string database, string filePath)
        {
            if (string.IsNullOrEmpty(database))
            {
                throw new ArgumentNullException("database");
            }
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            //make the folders
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            //If the file path does not contain a closing slash, add it.
            if (filePath.EndsWith("\\") == false)
            {
                filePath += "\\";
            }

            var functionFilePath = filePath + "Functions";
            var procedureFilePath = filePath + "Stored Procedures";
            var tableTypeFilePath = filePath + "Table Types";
            var triggerFilePath = filePath + "Triggers";
            var viewFilePath = filePath + "Views";

            if (!Directory.Exists(functionFilePath))
            {
                Directory.CreateDirectory(functionFilePath);
            }
            if (!Directory.Exists(procedureFilePath))
            {
                Directory.CreateDirectory(procedureFilePath);
            }
            if (!Directory.Exists(tableTypeFilePath))
            {
                Directory.CreateDirectory(tableTypeFilePath);
            }
            if (!Directory.Exists(triggerFilePath))
            {
                Directory.CreateDirectory(triggerFilePath);
            }
            if (!Directory.Exists(viewFilePath))
            {
                Directory.CreateDirectory(viewFilePath);
            }

            //Functions
            var functions = GetFunctions(database);
            foreach (var function in functions)
            {
                FileWriterService.WriteToFile(functionFilePath, function);
            }
            functions.Clear();

            var procedures = GetProcedures(database);
            foreach (var procedure in procedures)
            {
                FileWriterService.WriteToFile(procedureFilePath, procedure);
            }
            procedures.Clear();

            //TODO: user defined Table Types

            var triggers = GetTriggers(database);
            foreach (var trigger in triggers)
            {
                FileWriterService.WriteToFile(triggerFilePath, trigger);
            }
            triggers.Clear();

            var views = GetViews(database);
            foreach (var view in views)
            {
                FileWriterService.WriteToFile(viewFilePath, view);
            }
            views.Clear();


            return true;
        }

        /// <summary>
        /// Gets all functions in a given database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public static List<SqlObject> GetFunctions(string database)
        {
            var query = @"SELECT DISTINCT s.name AS 'Schema', OBJECT_NAME(m.object_id) AS 'Name', definition AS 'ProcedureText' FROM sys.sql_modules m WITH(NOLOCK) inner join sys.objects o WITH(NOLOCK) on m.object_id = o.object_id inner join sys.schemas s WITH(NOLOCK) on s.schema_id = o.schema_id WHERE OBJECTPROPERTY(m.object_id, 'IsScalarFunction') = 1 OR OBJECTPROPERTY(m.object_id, 'IsTableFunction') = 1";
            return GetSqlObjects(query, database, DBObjectType.FUNCTION);
        }

        /// <summary>
        /// Gets all procedures in a given database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public static List<SqlObject> GetProcedures(string database)
        {
            var query = @"SELECT DISTINCT s.name AS 'Schema', OBJECT_NAME(m.object_id) AS 'Name', definition AS 'ProcedureText' FROM sys.sql_modules m WITH(NOLOCK) inner join sys.objects o WITH(NOLOCK) on m.object_id = o.object_id inner join sys.schemas s WITH(NOLOCK) on s.schema_id = o.schema_id WHERE OBJECTPROPERTY(m.object_id, 'IsProcedure') = 1";
            return GetSqlObjects(query, database, DBObjectType.PROCEDURE);
        }

        /// <summary>
        /// Gets all views in a given database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public static List<SqlObject> GetViews(string database)
        {
            var query = @"SELECT s.name AS 'Schema', OBJECT_NAME(m.object_id) AS 'Name', definition AS 'ProcedureText' FROM sys.sql_modules m WITH(NOLOCK) inner join sys.objects o WITH(NOLOCK) on m.object_id = o.object_id inner join sys.schemas s WITH(NOLOCK) on s.schema_id = o.schema_id WHERE OBJECTPROPERTY(m.object_id, 'IsView') = 1";
            return GetSqlObjects(query, database, DBObjectType.VIEW);
        }

        /// <summary>
        /// Gets all triggers in a given database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public static List<SqlObject> GetTriggers(string database)
        {
            var query = @"SELECT s.name AS 'Schema', OBJECT_NAME(m.object_id) AS 'Name', definition AS 'ProcedureText' FROM sys.sql_modules m WITH(NOLOCK) inner join sys.objects o WITH(NOLOCK) on m.object_id = o.object_id inner join sys.schemas s WITH(NOLOCK) on s.schema_id = o.schema_id WHERE OBJECTPROPERTY(m.object_id, 'IsTrigger') = 1";
            return GetSqlObjects(query, database, DBObjectType.TRIGGER);
        }

        /// <summary>
        /// Gets the SQL objects.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        private static List<SqlObject> GetSqlObjects(string query, string database, DBObjectType objectType)
        {
            var result = new List<SqlObject>();

            var dbName = DatabaseService.GetCurrentDatabase(database);

            using (var dtObjectNames = DatabaseService.GetDataTable(query, database))
            {
                foreach (DataRow row in dtObjectNames.Rows)
                {
                    var s = new SqlObject();
                    s.Name = row["Name"].ToString().Trim();
                    s.Schema = row["Schema"].ToString().Trim();
                    s.Text = row["ProcedureText"].ToString();
                    s.DBName = dbName;
                    s.ObjectType = objectType;

                    s.Text = AddSchema(s.Text, s.ObjectType, s.Schema, s.Name);


                    result.Add(s);
                }
            }

            return result;
        }

        public static string AddSchema(string text, DBObjectType objectType, string schema, string name)
        {
            text = text.Trim();

            string operationPrefix = $"CREATE {objectType.ToString()}";

            if (text.StartsWith(operationPrefix, StringComparison.InvariantCultureIgnoreCase))
            {

                if (text.StartsWith($"{operationPrefix} {schema}.{name}", StringComparison.InvariantCultureIgnoreCase))
                {
                    // add brackets.
                    text = Regex.Replace(text, $"{operationPrefix} {schema}.{name}", $"{operationPrefix} [{schema}].[{name}]", RegexOptions.IgnoreCase);
                }
                else if (text.StartsWith($"{operationPrefix} {name}", StringComparison.InvariantCultureIgnoreCase))
                {
                    // add schema and brackets.
                    text = Regex.Replace(text, $"{operationPrefix} {name}", $"{operationPrefix} [{schema}].[{name}]", RegexOptions.IgnoreCase);
                }
                else if (text.StartsWith($"{operationPrefix} [{name}]", StringComparison.InvariantCultureIgnoreCase))
                {
                    // add schema.
                    text = Regex.Replace(text, $"{operationPrefix} \\[{name}\\]", $"{operationPrefix} [{schema}].[{name}]", RegexOptions.IgnoreCase);
                }
                else if (text.StartsWith($"{operationPrefix} [{schema}].[{name}]", StringComparison.InvariantCultureIgnoreCase))
                {
                    // add schema.
                    text = Regex.Replace(text, $"{operationPrefix} \\[{schema}\\].\\[{name}\\]", $"{operationPrefix} [{schema}].[{name}]", RegexOptions.IgnoreCase);
                }
            }

            return text;
        }

        /// <summary>
        /// Gets a list of stored procedures that are different.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns></returns>
        public static List<ComparisonResult> GetDifferences(List<SqlObject> source, List<SqlObject> destination)
        {
            var result = new List<ComparisonResult>();

            foreach (var procedure in destination)
            {
                var target = source.FirstOrDefault(x => x.Name == procedure.Name && x.Schema == procedure.Schema);
                if (target == null)
                {
                    //it does not exist, add it.
                    result.Add(new ComparisonResult(procedure.Schema, procedure.Name, Action.New));
                }
                else
                {
                    //it exists, see if it needs to be updated.
                    if (target.Text != procedure.Text)
                    {
                        result.Add(new ComparisonResult(procedure.Schema, procedure.Name, Action.Change));
                    }
                }
            }

            //if the procedure is in destination but not in source, delete it.
            foreach (var procedure in source)
            {
                if (destination.Any(x => x.Name == procedure.Name && x.Schema == procedure.Schema) == false)
                {
                    result.Add(new ComparisonResult(procedure.Schema, procedure.Name, Action.Delete));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the current database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public static string GetCurrentDatabase(string database)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[database].ToString().Trim();            

            if (connectionString.IndexOf("DATABASE=", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                connectionString = connectionString.Substring(connectionString.IndexOf("DATABASE=", StringComparison.InvariantCultureIgnoreCase) + 9);
            }

            if (connectionString.Contains(";"))
            {
                connectionString = connectionString.Remove(connectionString.IndexOf(';'));
            }

            return connectionString;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="sqlText">The SQL text.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// DataTable object
        /// </returns>
        public static DataTable GetDataTable(string sqlText, string connection)
        {
            using (var cnn = new SqlConnection(GetConnectionString(connection)))
            {
                using (var dt = new DataTable())
                {
                    using (var cmd = new SqlCommand(sqlText, cnn) { CommandType = CommandType.Text })
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        return dt;
                    }
                }
            }
        }

        private static string GetConnectionString(string connection)
        {
            return ConfigurationManager.ConnectionStrings[connection].ToString().Trim();
        }
    }
}
