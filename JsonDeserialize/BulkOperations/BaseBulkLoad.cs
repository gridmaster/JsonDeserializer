using System;
using System.Data;
using System.Data.SqlClient;
using JsonDeserialize.Core;

namespace JsonDeserialize.BulkOperations
{
    public class BaseBulkLoad
    {
        private readonly string ConnString;
        private readonly string[] ColumnNames;

        public BaseBulkLoad(string connString, string[] columnNames)
        {
            ConnString = connString;
            ColumnNames = columnNames;
        }

        public DataTable ConfigureDataTable()
        {
            var dt = new DataTable();

            for (int i = 0; i < ColumnNames.Length; i++)
            {
                dt.Columns.Add(new DataColumn());
                dt.Columns[i].ColumnName = ColumnNames[i];
            }
            return dt;
        }

        public void BulkCopy<T>(DataTable dt)
        {
            string tableName = typeof(T).Name;

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConnString))
            {
                for (int i = 0; i < ColumnNames.Length; i++)
                    bulkCopy.ColumnMappings.Add(i, ColumnNames[i]);

                bulkCopy.BulkCopyTimeout = 600; // in seconds 
                bulkCopy.DestinationTableName = tableName;
                try
                {
                    bulkCopy.WriteToServer(dt);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(new LogEvent("BulkLoadSector - BulkCopy", string.Format("<{0}> - Bulk load error: {1}", tableName, ex.Message)));
                }
                bulkCopy.Close();
            }
        }
    }
}
