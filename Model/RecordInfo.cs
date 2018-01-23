using System;

namespace SS.Payment.Model
{
	public class RecordInfo
	{
        public int Id { get; set; }

        public int PublishmentSystemId { get; set; }

        public string Message { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Fee { get; set; }

        public string OrderNo { get; set; }

        public string Channel { get; set; }

        public bool IsPaied { get; set; }

        public string UserName { get; set; }

        public DateTime AddDate { get; set; }
    }
}
