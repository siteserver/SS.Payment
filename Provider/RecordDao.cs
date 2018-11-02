using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Payment.Model;

namespace SS.Payment.Provider
{
    public static class RecordDao
    {
        public const string TableName = "ss_payment_record";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Message),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.ProductId),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.ProductName),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Fee),
                DataType = DataType.Decimal
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.OrderNo),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Channel),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.IsPaied),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.UserName),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.AddDate),
                DataType = DataType.DateTime
            }
        };

        public static int Insert(RecordInfo recordInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(RecordInfo.SiteId)},
    {nameof(RecordInfo.Message)},
    {nameof(RecordInfo.ProductId)},
    {nameof(RecordInfo.ProductName)},
    {nameof(RecordInfo.Fee)},
    {nameof(RecordInfo.OrderNo)},
    {nameof(RecordInfo.Channel)},
    {nameof(RecordInfo.IsPaied)},
    {nameof(RecordInfo.UserName)},
    {nameof(RecordInfo.AddDate)}
) VALUES (
    @{nameof(RecordInfo.SiteId)}, 
    @{nameof(RecordInfo.Message)},
    @{nameof(RecordInfo.ProductId)},
    @{nameof(RecordInfo.ProductName)},
    @{nameof(RecordInfo.Fee)},
    @{nameof(RecordInfo.OrderNo)},
    @{nameof(RecordInfo.Channel)},
    @{nameof(RecordInfo.IsPaied)},
    @{nameof(RecordInfo.UserName)},
    @{nameof(RecordInfo.AddDate)}
)";

            var parameters = new[]
            {
                Context.DatabaseApi.GetParameter(nameof(recordInfo.SiteId), recordInfo.SiteId),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.Message), recordInfo.Message),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.ProductId), recordInfo.ProductId),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.ProductName), recordInfo.ProductName),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.Fee), recordInfo.Fee),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.OrderNo), recordInfo.OrderNo),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.Channel), recordInfo.Channel),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.IsPaied), recordInfo.IsPaied),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.UserName), recordInfo.UserName),
                Context.DatabaseApi.GetParameter(nameof(recordInfo.AddDate), recordInfo.AddDate)
            };

            return Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(RecordInfo.Id), Context.ConnectionString, sqlString, parameters);
        }

        public static void UpdateIsPaied(string orderNo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(RecordInfo.IsPaied)} = @{nameof(RecordInfo.IsPaied)} WHERE
                {nameof(RecordInfo.OrderNo)} = @{nameof(RecordInfo.OrderNo)}";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(RecordInfo.IsPaied), true),
                Context.DatabaseApi.GetParameter(nameof(RecordInfo.OrderNo), orderNo)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters.ToArray());
        }

        public static bool IsPaied(string orderNo)
        {
            var isPaied = false;

            string sqlString = $@"SELECT {nameof(RecordInfo.IsPaied)} FROM {TableName} WHERE {nameof(RecordInfo.OrderNo)} = @{nameof(RecordInfo.OrderNo)}";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(RecordInfo.OrderNo), orderNo)
            };

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters.ToArray()))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isPaied = rdr.GetBoolean(0);
                }
                rdr.Close();
            }

            return isPaied;
        }

        public static string GetSelectString(int siteId)
        {
            return $@"SELECT {nameof(RecordInfo.Id)}, 
            {nameof(RecordInfo.SiteId)}, 
            {nameof(RecordInfo.Message)},
            {nameof(RecordInfo.ProductId)},
            {nameof(RecordInfo.ProductName)},
            {nameof(RecordInfo.Fee)},
            {nameof(RecordInfo.OrderNo)},
            {nameof(RecordInfo.Channel)},
            {nameof(RecordInfo.IsPaied)},
            {nameof(RecordInfo.UserName)},
            {nameof(RecordInfo.AddDate)}
            FROM {TableName} WHERE {nameof(RecordInfo.SiteId)} = {siteId} ORDER BY Id DESC";
        }

        public static void Delete(List<int> deleteIdList)
        {
            string sqlString =
                $"DELETE FROM {TableName} WHERE Id IN ({string.Join(",", deleteIdList)})";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }
    }
}
