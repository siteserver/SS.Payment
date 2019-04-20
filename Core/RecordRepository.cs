using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;

namespace SS.Payment.Core
{
    public class RecordRepository : Repository<RecordInfo>
    {
        public RecordRepository() : base(Context.Environment.DatabaseType, Context.Environment.ConnectionString)
        {
        }

        public void UpdateIsPayed(string orderNo)
        {
            Update(Q
                .Set(nameof(RecordInfo.IsPayed), true)
                .Where(nameof(RecordInfo.OrderNo), orderNo)
                );
        }

        public bool IsPayed(string orderNo)
        {
            return Get<bool>(Q
                .Select(nameof(RecordInfo.IsPayed))
                .Where(nameof(RecordInfo.OrderNo), orderNo)
                );
        }

        public int GetPayedCount(int siteId)
        {
            return Count(Q
                .Where(nameof(RecordInfo.SiteId), siteId)
                .Where(nameof(RecordInfo.IsPayed), true)
            );
        }

        public IList<RecordInfo> GetPayedRecordInfoList(int siteId, int page, int perPage)
        {
            return GetAll(Q
                .Where(nameof(RecordInfo.SiteId), siteId)
                .Where(nameof(RecordInfo.IsPayed), true)
                .OrderByDesc(nameof(RecordInfo.Id))
                .ForPage(page, perPage)
                );
        }
    }
}
