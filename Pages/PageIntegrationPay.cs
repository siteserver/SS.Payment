using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Payment.Core;

namespace SS.Payment.Pages
{
    public class PageIntegrationPay : Page
    {
        public Literal LtlAlipayPc;
        public Literal LtlAlipayMobi;
        public Literal LtlWeixin;
        public Literal LtlJdpay;
        public Literal LtlScript;

        public string PageIntegrationPayAlipayMobiUrl => PageIntegrationPayAlipayMobi.GetRedirectUrl(_siteId);
        public string PageIntegrationPayAlipayPcUrl => PageIntegrationPayAlipayPc.GetRedirectUrl(_siteId);
        public string PageIntegrationPayJdpayUrl => PageIntegrationPayJdpay.GetRedirectUrl(_siteId);
        public string PageIntegrationPayWeixinUrl => PageIntegrationPayWeixin.GetRedirectUrl(_siteId);

        private int _siteId;

        public static string GetRedirectUrl(int siteId)
        {
            return $"{nameof(PageIntegrationPay)}.aspx?siteId={siteId}";
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var request = SiteServer.Plugin.Context.GetCurrentRequest();
            _siteId = request.GetQueryInt("siteId");
            if (!request.AdminPermissions.HasSitePermissions(_siteId, Main.PluginId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (IsPostBack) return;

            var paymentApi = new PaymentApi(_siteId);

            if (!string.IsNullOrEmpty(Request.QueryString["alipayPc"]))
            {
                LtlScript.Text = paymentApi.ChargeByAlipayPc("测试", 0.01M, Utils.GetShortGuid(), "https://www.alipay.com");
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["alipayMobi"]))
            {
                LtlScript.Text = paymentApi.ChargeByAlipayMobi("测试", 0.01M, Utils.GetShortGuid(), "https://www.alipay.com");
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["weixin"]))
            {
                try
                {
                    var url = HttpUtility.UrlEncode(paymentApi.ChargeByWeixin("测试", 0.01M, Utils.GetShortGuid(), "https://pay.weixin.qq.com"));
                    LtlScript.Text = $@"<div style=""display: none""><img id=""weixin_test"" src=""{GetRedirectUrl(_siteId)}&qrcode={url}"" width=""200"" height=""200"" /></div><script>{Utils.SwalDom(Page, "微信支付测试", "weixin_test")}</script>";
                }
                catch (Exception ex)
                {
                    LtlScript.Text = $"<script>{Utils.SwalError(Page, "测试报错", ex.Message)}</script>";
                }
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["qrcode"]))
            {
                Response.BinaryWrite(QrCodeUtils.GetBuffer(Request.QueryString["qrcode"]));
                Response.End();
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["jdpay"]))
            {
                LtlScript.Text = paymentApi.ChargeByJdpay("测试", 0.01M, Utils.GetShortGuid(), "https://www.jdpay.com");
            }

            var configInfo = Main.GetConfigInfo(_siteId);

            LtlAlipayPc.Text = configInfo.IsAlipayPc ? $@"
                <span class=""label label-primary"">已开通</span>
                <a class=""m-l-10"" href=""{GetRedirectUrl(_siteId)}&alipayPc=true"">测试</a>" : "未开通";

            LtlAlipayMobi.Text = configInfo.IsAlipayMobi ? $@"
                <span class=""label label-primary"">已开通</span>
                <a class=""m-l-10"" href=""{GetRedirectUrl(_siteId)}&alipayMobi=true"">测试</a>" : "未开通";

            LtlWeixin.Text = configInfo.IsWeixin ? $@"
                <span class=""label label-primary"">已开通</span>
                <a class=""m-l-10"" href=""{GetRedirectUrl(_siteId)}&weixin=true"">测试</a>" : "未开通";

            LtlJdpay.Text = configInfo.IsJdpay ? $@"
                <span class=""label label-primary"">已开通</span>
                <a class=""m-l-10"" href=""{GetRedirectUrl(_siteId)}&jdpay=true"">测试</a>" : "未开通";
        }
    }
}
