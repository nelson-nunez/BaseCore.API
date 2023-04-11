using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace BaseRest.Core.API.LogsConfiguration
{
    public class SerilogConfiguration
    {
        public static ColumnOptions GetColumnOptions()
        {
            var columnOptions = new ColumnOptions();
            // Override the default Primary Column of Serilog by custom column name
            columnOptions.Id.ColumnName = "LogId";
            // Removing all the default column
            //columnOptions.Store.Remove(StandardColumn.TimeStamp);
            //columnOptions.Store.Remove(StandardColumn.Message);
            //columnOptions.Store.Remove(StandardColumn.Level);
            //columnOptions.Store.Remove(StandardColumn.Exception);
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            //columnOptions.Store.Remove(StandardColumn.Properties);

            // Adding all the custom columns
            columnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "UserId", DataLength = 50, AllowNull = true},
                new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "CreatedBy",DataLength = 50, AllowNull = true },
                new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "ThreadId",DataLength = 50, AllowNull = true },
                new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "StackTrace", DataLength = 500, AllowNull = true },
            };
            return columnOptions;
        }
        public static MSSqlServerSinkOptions GetsSinkOptions()
        {
            var sinkOptions = new MSSqlServerSinkOptions
            {
                TableName = "LogEvents",
                SchemaName = "dbo",
                AutoCreateSqlTable = true,
                BatchPostingLimit = 1,
            };
            return sinkOptions;
        }
    }
}
