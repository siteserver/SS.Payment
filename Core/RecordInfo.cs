using Datory;
using System;

namespace SS.Payment.Core
{
    [Table("ss_payment_record")]
	public class RecordInfo : Entity
	{
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string Message { get; set; }

        [TableColumn]
        public string ProductId { get; set; }

        [TableColumn]
        public string ProductName { get; set; }

        [TableColumn]
        public decimal Fee { get; set; }

        [TableColumn]
        public string OrderNo { get; set; }

        [TableColumn]
        public string Channel { get; set; }

        [TableColumn]
        public bool IsPayed { get; set; }

        [TableColumn]
        public string UserName { get; set; }

        [TableColumn]
        public DateTime AddDate { get; set; }
    }
}
