using SiteServer.Plugin;

namespace SS.Payment.Provider
{
    public class Dao
    {
        private readonly string _connectionString;
        private readonly IDataApi _dataApi;

        public Dao(string connectionString, IDataApi dataApi)
        {
            _connectionString = connectionString;
            _dataApi = dataApi;
        }

        public int GetIntResult(string sqlString)
        {
            var count = 0;

            using (var conn = _dataApi.GetConnection(_connectionString))
            {
                conn.Open();
                using (var rdr = _dataApi.ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        count = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }
    }
}