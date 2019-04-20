using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Payment.Core;

namespace SS.Payment.Controllers.Pages
{
    [RoutePrefix("pages/test")]
    public class PagesTestController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            var request = Context.AuthenticatedRequest;
            var siteId = request.GetQueryInt("siteId");
            var type = request.GetQueryString("type");
            var apiUrl = Context.Environment.ApiUrl;

            if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId))
            {
                return Unauthorized();
            }

            var paymentApi = new PaymentApi(siteId);
            
            var redirectUrl = string.Empty;
            var wxPayQrCodeUrl = string.Empty;
            var wxPayOrderNo = string.Empty;

            if (Utils.EqualsIgnoreCase(type, "AliPay"))
            {
                redirectUrl = paymentApi.ChargeByAliPay("测试", 0.01M, Utils.GetShortGuid(), "https://www.alipay.com");
            }
            else if (Utils.EqualsIgnoreCase(type, "WxPay"))
            {
                wxPayOrderNo = Utils.GetShortGuid();
                var notifyUrl = UrlUtils.GetWxPayNotifyUrl(apiUrl, wxPayOrderNo, siteId);
                var url = HttpUtility.UrlEncode(paymentApi.ChargeByWxPay("测试", 0.01M, wxPayOrderNo, notifyUrl));
                wxPayQrCodeUrl = UrlUtils.GetWxPayQrCodeUrl(apiUrl, url);
            }

            return Ok(new
            {
                Value = redirectUrl,
                WxPayQrCodeUrl = wxPayQrCodeUrl,
                WxPayOrderNo = wxPayOrderNo
            });
        }
    }
}
