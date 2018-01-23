using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SiteServer.Plugin.Apis;
using SS.Payment.Model;

namespace SS.Payment.Provider
{
    public class RecordDao
    {
        public const string TableName = "ss_payment_record";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.PublishmentSystemId),
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

        private readonly string _connectionString;
        private readonly IDataApi _helper;

        public RecordDao(IContext context)
        {
            _connectionString = context.Environment.ConnectionString;
            _helper = context.DataApi;
        }

        public int Insert(RecordInfo recordInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(RecordInfo.PublishmentSystemId)},
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
    @{nameof(RecordInfo.PublishmentSystemId)}, 
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
                _helper.GetParameter(nameof(recordInfo.PublishmentSystemId), recordInfo.PublishmentSystemId),
                _helper.GetParameter(nameof(recordInfo.Message), recordInfo.Message),
                _helper.GetParameter(nameof(recordInfo.ProductId), recordInfo.ProductId),
                _helper.GetParameter(nameof(recordInfo.ProductName), recordInfo.ProductName),
                _helper.GetParameter(nameof(recordInfo.Fee), recordInfo.Fee),
                _helper.GetParameter(nameof(recordInfo.OrderNo), recordInfo.OrderNo),
                _helper.GetParameter(nameof(recordInfo.Channel), recordInfo.Channel),
                _helper.GetParameter(nameof(recordInfo.IsPaied), recordInfo.IsPaied),
                _helper.GetParameter(nameof(recordInfo.UserName), recordInfo.UserName),
                _helper.GetParameter(nameof(recordInfo.AddDate), recordInfo.AddDate)
            };

            return _helper.ExecuteNonQueryAndReturnId(TableName, nameof(RecordInfo.Id), _connectionString, sqlString, parameters);
        }

        public void UpdateIsPaied(string orderNo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(RecordInfo.IsPaied)} = @{nameof(RecordInfo.IsPaied)} WHERE
                {nameof(RecordInfo.OrderNo)} = @{nameof(RecordInfo.OrderNo)}";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(RecordInfo.IsPaied), true),
                _helper.GetParameter(nameof(RecordInfo.OrderNo), orderNo)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parameters.ToArray());
        }

        public bool IsPaied(string orderNo)
        {
            var isPaied = false;

            string sqlString = $@"SELECT {nameof(RecordInfo.IsPaied)} FROM {TableName} WHERE {nameof(RecordInfo.OrderNo)} = @{nameof(RecordInfo.OrderNo)}";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(RecordInfo.OrderNo), orderNo)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parameters.ToArray()))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isPaied = rdr.GetBoolean(0);
                }
                rdr.Close();
            }

            return isPaied;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            return $@"SELECT {nameof(RecordInfo.Id)}, 
            {nameof(RecordInfo.PublishmentSystemId)}, 
            {nameof(RecordInfo.Message)},
            {nameof(RecordInfo.ProductId)},
            {nameof(RecordInfo.ProductName)},
            {nameof(RecordInfo.Fee)},
            {nameof(RecordInfo.OrderNo)},
            {nameof(RecordInfo.Channel)},
            {nameof(RecordInfo.IsPaied)},
            {nameof(RecordInfo.UserName)},
            {nameof(RecordInfo.AddDate)}
            FROM {TableName} WHERE {nameof(RecordInfo.PublishmentSystemId)} = {publishmentSystemId} ORDER BY Id DESC";
        }

        public void Delete(List<int> deleteIdList)
        {
            string sqlString =
                $"DELETE FROM {TableName} WHERE Id IN ({string.Join(",", deleteIdList)})";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }
    }
}
