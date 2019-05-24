using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSSQL_Database_Tools.Core.Entities;

namespace MSSQL_Database_Tools.Test
{
    [TestClass]
    public class DatabaseServiceTest
    {
        #region Functions
        [TestMethod]
        public void AddSchema_function_changes()
        {
            string expected = @"CREATE FUNCTION [dbo].[FY] ( @date datetime)  returns integer  as  begin   declare @retval integer, @fystartmonth tinyint   set @fystartmonth=7     if @date is null    set @retval=null   else    set @retval=(year(@date) +      case when month(@date)>=@fystartmonth      then 1 else 0 end)   return @retval  end";

            string text = @"  CREATE function FY ( @date datetime)  returns integer  as  begin   declare @retval integer, @fystartmonth tinyint   set @fystartmonth=7     if @date is null    set @retval=null   else    set @retval=(year(@date) +      case when month(@date)>=@fystartmonth      then 1 else 0 end)   return @retval  end        ";
            
            DBObjectType objectType = DBObjectType.FUNCTION;
            string schema = "dbo";
            string name = "FY";


            var result = MSSQL_Database_Tools.Core.Services.DatabaseService.AddSchema(text, objectType, schema, name);

            Assert.AreEqual(expected, result);

        }

        [TestMethod]
        public void AddSchema_function_nochanges()
        {
            string expected = @"CREATE FUNCTION [dbo].[FY] ( @date datetime)  returns integer  as  begin   declare @retval integer, @fystartmonth tinyint   set @fystartmonth=7     if @date is null    set @retval=null   else    set @retval=(year(@date) +      case when month(@date)>=@fystartmonth      then 1 else 0 end)   return @retval  end";

            string text = @"  CREATE function dbo.FY ( @date datetime)  returns integer  as  begin   declare @retval integer, @fystartmonth tinyint   set @fystartmonth=7     if @date is null    set @retval=null   else    set @retval=(year(@date) +      case when month(@date)>=@fystartmonth      then 1 else 0 end)   return @retval  end        ";

            DBObjectType objectType = DBObjectType.FUNCTION;
            string schema = "dbo";
            string name = "FY";


            var result = MSSQL_Database_Tools.Core.Services.DatabaseService.AddSchema(text, objectType, schema, name);

            Assert.AreEqual(expected, result);

        }


        [TestMethod]
        public void AddSchema_function_changes_brackets()
        {
            string expected = @"CREATE FUNCTION [dbo].[FY] ( @date datetime)  returns integer  as  begin   declare @retval integer, @fystartmonth tinyint   set @fystartmonth=7     if @date is null    set @retval=null   else    set @retval=(year(@date) +      case when month(@date)>=@fystartmonth      then 1 else 0 end)   return @retval  end";

            string text = @"  CREATE function [FY] ( @date datetime)  returns integer  as  begin   declare @retval integer, @fystartmonth tinyint   set @fystartmonth=7     if @date is null    set @retval=null   else    set @retval=(year(@date) +      case when month(@date)>=@fystartmonth      then 1 else 0 end)   return @retval  end        ";

            DBObjectType objectType = DBObjectType.FUNCTION;
            string schema = "dbo";
            string name = "FY";


            var result = MSSQL_Database_Tools.Core.Services.DatabaseService.AddSchema(text, objectType, schema, name);

            Assert.AreEqual(expected, result);

        }


        [TestMethod]
        public void AddSchema_function_nochanges_brackets()
        {
            string expected = @"CREATE FUNCTION [dbo].[FY] ( @date datetime)  returns integer  as  begin   declare @retval integer, @fystartmonth tinyint   set @fystartmonth=7     if @date is null    set @retval=null   else    set @retval=(year(@date) +      case when month(@date)>=@fystartmonth      then 1 else 0 end)   return @retval  end";

            string text = @"  CREATE function [dbo].[FY] ( @date datetime)  returns integer  as  begin   declare @retval integer, @fystartmonth tinyint   set @fystartmonth=7     if @date is null    set @retval=null   else    set @retval=(year(@date) +      case when month(@date)>=@fystartmonth      then 1 else 0 end)   return @retval  end        ";

            DBObjectType objectType = DBObjectType.FUNCTION;
            string schema = "dbo";
            string name = "FY";


            var result = MSSQL_Database_Tools.Core.Services.DatabaseService.AddSchema(text, objectType, schema, name);

            Assert.AreEqual(expected, result);

        }

        #endregion
    }
}
