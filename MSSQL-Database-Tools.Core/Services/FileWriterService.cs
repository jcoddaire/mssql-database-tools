using MSSQL_Database_Tools.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MSSQL_Database_Tools.Core.Services
{
    public static class FileWriterService
    {
        /// <summary>
        /// Writes the given SQL object to a file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="sqlObject">The SQL object.</param>
        public static void WriteToFile(string filePath, SqlObject sqlObject)
        {
            var path = GetFullPath(sqlObject.Schema.ToLower(), sqlObject.Name, filePath);
            if (!string.IsNullOrWhiteSpace(path))
            {
                //Set the USE statement to the current database.
                var useDatabase = $"USE [{sqlObject.DBName}]{Environment.NewLine}GO{Environment.NewLine}{Environment.NewLine}";
                string ANSI_NULLS = $"SET ANSI_NULLS ON{Environment.NewLine}GO{Environment.NewLine}";
                string QUOTED_IDENTIFIER = $"SET QUOTED_IDENTIFIER ON{Environment.NewLine}GO{Environment.NewLine}{Environment.NewLine}";

                //By default everything will be a CREATE statement. In order to have source control (VSTS / CI) auto deploy these, we need to have a DROP
                //statement first, so the script can be re-run without needing to consider context.

                //IF OBJECT_ID('MYPROC') IS NOT NULL DROP PROCEDURE MYPROC
                //GO

                var dropStatement = $"IF OBJECT_ID('[{sqlObject.Schema}].[{sqlObject.Name}]') IS NOT NULL DROP {sqlObject.ObjectType.ToString()} [{sqlObject.Schema}].[{sqlObject.Name}]{Environment.NewLine}GO{Environment.NewLine}{Environment.NewLine}";

                File.WriteAllText(path, $"{useDatabase}{ANSI_NULLS}{QUOTED_IDENTIFIER}{dropStatement}{sqlObject.Text}");
            }
        }

        /// <summary>
        /// Gets the save location on disk for the given sql object.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
        private static string GetFullPath(string schema, string name, string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (schema.Contains("\\"))
            {
                schema = schema.Replace("\\", "-");
            }

            return $@"{directory}\{schema}.{name}.sql";
        }
    }
}
