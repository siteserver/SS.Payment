namespace SS.Payment.Core
{
    public class ConfigInfo
    {
        public bool IsAliPay { get; set; }
        public bool AliPayIsMApi { get; set; }
        public string AliPayPid { get; set; }
        public string AliPayMd5 { get; set; }
        public string AliPayAppId { get; set; }
        public string AliPayPublicKey { get; set; }
        public string AliPayPrivateKey { get; set; }

        public bool IsWxPay { get; set; }
        public string WxPayAppId { get; set; }
        public string WxPayAppSecret { get; set; }
        public string WxPayMchId { get; set; }
        public string WxPayKey { get; set; }
    }
}