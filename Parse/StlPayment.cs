using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.HtmlControls;
using SiteServer.Plugin;
using SS.Payment.Core;
using SS.Payment.Model;
using SS.Payment.Provider;
using ThoughtWorks.QRCode.Codec;

namespace SS.Payment.Parse
{
    public class StlPayment
    {
        private StlPayment() { }
        public const string ElementName = "stl:payment";

        public const string AttributeProductId = "productId";
        public const string AttributeProductName = "productName";
        public const string AttributeFee = "fee";
        public const string AttributeLoginUrl = "loginUrl";
        public const string AttributeRedirectUrl = "redirectUrl";
        public const string AttributeWeixinName = "weixinName";

        public static void ApiRedirect(string successUrl)
        {
            Utils.Redirect(successUrl);
        }

        public static object ApiGet(IRequest request)
        {
            var siteId = request.GetPostInt("siteId");

            var configInfo = Main.ApiCollection.ConfigApi.GetConfig<ConfigInfo>(siteId);

            return new
            {
                request.IsUserLoggin,
                IsForceLogin = configInfo != null && configInfo.IsForceLogin
            };
        }

        public static object ApiPay(IRequest request)
        {
            var siteId = request.GetPostInt("siteId");
            var productId = request.GetPostString("productId");
            var productName = request.GetPostString("productName");
            var fee = request.GetPostDecimal("fee");
            var channel = request.GetPostString("channel");
            var message = request.GetPostString("message");
            var isMobile = request.GetPostBool("isMobile");
            var successUrl = request.GetPostString("successUrl");
            var orderNo = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            successUrl += "&orderNo=" + orderNo;

            var paymentApi = new PaymentApi(siteId);

            var recordInfo = new RecordInfo
            {
                PublishmentSystemId = siteId,
                Message = message,
                ProductId = productId,
                ProductName = productName,
                Fee = fee,
                OrderNo = orderNo,
                Channel = channel,
                IsPaied = false,
                UserName = request.UserName,
                AddDate = DateTime.Now
            };
            RecordDao.Insert(recordInfo);

            if (channel == "alipay")
            {
                return isMobile
                    ? paymentApi.ChargeByAlipayMobi(productName, fee, orderNo, successUrl)
                    : paymentApi.ChargeByAlipayPc(productName, fee, orderNo, successUrl);
            }
            if (channel == "weixin")
            {
                var notifyUrl = Main.ApiCollection.PluginApi.GetPluginApiUrl(nameof(ApiWeixinNotify), orderNo);
                var url = HttpUtility.UrlEncode(paymentApi.ChargeByWeixin(productName, fee, orderNo, notifyUrl));
                var qrCodeUrl =
                    $"{Main.ApiCollection.PluginApi.GetPluginApiUrl(nameof(ApiQrCode))}?qrcode={url}";
                return new
                {
                    qrCodeUrl,
                    orderNo
                };
            }
            if (channel == "jdpay")
            {
                return paymentApi.ChargeByJdpay(productName, fee, orderNo, successUrl);
            }

            return null;
        }

        public static HttpResponseMessage ApiQrCode(IRequest request)
        {
            var response = new HttpResponseMessage();

            var qrcode = request.GetQueryString("qrcode");
            var qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M,
                QRCodeVersion = 0,
                QRCodeScale = 4
            };

            //将字符串生成二维码图片
            var image = qrCodeEncoder.Encode(qrcode, Encoding.Default);

            //保存为PNG到内存流  
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);

            response.Content = new ByteArrayContent(ms.GetBuffer());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public static HttpResponseMessage ApiWeixinNotify(IRequest request, string orderNo)
        {
            var siteId = request.GetPostInt("siteId");

            var response = new HttpResponseMessage();

            var paymentApi = new PaymentApi(siteId);

            bool isPaied;
            string responseXml;
            paymentApi.NotifyByWeixin(request.HttpRequest, out isPaied, out responseXml);
            if (isPaied)
            {
                RecordDao.UpdateIsPaied(orderNo);
            }

            response.Content = new StringContent(responseXml);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public static object ApiPaySuccess(IRequest request)
        {
            var orderNo = request.GetPostString("orderNo");
            
            RecordDao.UpdateIsPaied(orderNo);

            return null;
        }

        public static object ApiWeixinInterval(IRequest request)
        {
            var orderNo = request.GetPostString("orderNo");

            var isPaied = RecordDao.IsPaied(orderNo);

            return new
            {
                isPaied
            };
        }

        public static string Parse(IParseContext context)
        {
            var stlAnchor = new HtmlAnchor();

            var productId = string.Empty;
            var productName = string.Empty;
            decimal fee = 0;
            var loginUrl = string.Empty;
            var redirectUrl = string.Empty;
            var weixinName = string.Empty;

            foreach (var name in context.Attributes.Keys)
            {
                var value = context.Attributes[name];
                if (Utils.EqualsIgnoreCase(name, AttributeProductId))
                {
                    productId = Main.ApiCollection.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeProductName))
                {
                    productName = Main.ApiCollection.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeFee))
                {
                    value = Main.ApiCollection.ParseApi.ParseAttributeValue(value, context);
                    decimal.TryParse(value, out fee);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeLoginUrl))
                {
                    loginUrl = Main.ApiCollection.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeRedirectUrl))
                {
                    redirectUrl = Main.ApiCollection.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeWeixinName))
                {
                    weixinName = Main.ApiCollection.ParseApi.ParseAttributeValue(value, context);
                }
                else
                {
                    stlAnchor.Attributes.Add(name, value);
                }
            }
            if (string.IsNullOrEmpty(loginUrl))
            {
                loginUrl = "/home/#/login";
            }
            var currentUrl = Main.ApiCollection.ParseApi.GetCurrentUrl(context);
            var loginToPaymentUrl = $"{loginUrl}?redirectUrl={HttpUtility.UrlEncode(currentUrl)}";

            if (string.IsNullOrEmpty(productName) || fee <= 0) return string.Empty;

            if (!string.IsNullOrEmpty(weixinName))
            {
                weixinName = $@"<p style=""text-align: center"">{weixinName}</p>";
            }

            string template = $@"
<div class=""mask1_bg mask1_bg_cut"" v-show=""isPayment || isWxQrCode || isPaymentSuccess"" @click=""isPayment = isWxQrCode = isPaymentSuccess = false""></div>
<div class=""detail_alert detail_alert_cut"" v-show=""isPayment"">
  <div class=""close"" @click=""isPayment = isWxQrCode = isPaymentSuccess = false""></div>
  <div class=""alert_input"">
    金额: ¥{fee:N2}元
  </div>
  <div class=""alert_textarea"">
    <textarea v-model=""message"" placeholder=""留言""></textarea>
  </div>
  <div class=""pay_list"">
    <p>支付方式</p>
    <ul>
        <li v-show=""(isAlipayPc && !isMobile) || (isAlipayMobi && isMobile)"" :class=""{{ pay_cut: channel === 'alipay' }}"" @click=""channel = 'alipay'"" class=""channel_alipay""><b></b></li>
        <li v-show=""isWeixin"" :class=""{{ pay_cut: channel === 'weixin' }}"" @click=""channel = 'weixin'"" class=""channel_weixin""><b></b></li>
        <li v-show=""isJdpay"" :class=""{{ pay_cut: channel === 'jdpay' }}"" @click=""channel = 'jdpay'"" class=""channel_jdpay""><b></b></li>
    </ul>
    <div class=""mess_text""></div>
    <a href=""javascript:;"" @click=""pay"" class=""pay_go"">立即支付</a>
  </div>
</div>
<div class=""detail_alert detail_alert_cut"" v-show=""isWxQrCode"">
  <div class=""close"" @click=""isPayment = isWxQrCode = isPaymentSuccess = false""></div>
  <div class=""pay_list"">
    <p style=""text-align: center""> 打开手机微信，扫一扫下面的二维码，即可完成支付</p>
    {weixinName}
    <p style=""margin-left: 195px;margin-bottom: 80px;""><img :src=""qrCodeUrl"" style=""width: 200px;height: 200px;""></p>
  </div>
</div>
<div class=""detail_alert detail_alert_cut"" v-show=""isPaymentSuccess"">
  <div class=""close"" @click=""isPayment = isWxQrCode = isPaymentSuccess = false""></div>
  <div class=""pay_list"">
    <p style=""text-align: center"">支付成功，谢谢支持</p>
    <div class=""mess_text""></div>
    <a href=""javascript:;"" @click=""weixinPaiedClose"" class=""pay_go"">关闭</a>
  </div>
</div>
";

            var elementId = "el-" + Guid.NewGuid();
            var vueId = "v" + Guid.NewGuid().ToString().Replace("-", string.Empty);
            var styleUrl = Main.ApiCollection.PluginApi.GetPluginUrl("assets/css/style.css");
            var jqueryUrl = Main.ApiCollection.PluginApi.GetPluginUrl("assets/js/jquery.min.js");
            var vueUrl = Main.ApiCollection.PluginApi.GetPluginUrl("assets/js/vue.min.js");
            var deviceUrl = Main.ApiCollection.PluginApi.GetPluginUrl("assets/js/device.min.js");
            var apiPayUrl = Main.ApiCollection.PluginApi.GetPluginApiUrl(nameof(ApiPay));
            var apiPaySuccessUrl = Main.ApiCollection.PluginApi.GetPluginApiUrl(nameof(ApiPaySuccess));
            var successUrl = Main.ApiCollection.ParseApi.GetCurrentUrl(context) + "?isPaymentSuccess=" + true;
            var apiWeixinIntervalUrl = Main.ApiCollection.PluginApi.GetPluginApiUrl(nameof(ApiWeixinInterval));
            var apiGetUrl = Main.ApiCollection.PluginApi.GetPluginApiUrl(nameof(ApiGet));

            var paymentApi = new PaymentApi(context.SiteId);

            var html = $@"
<link rel=""stylesheet"" type=""text/css"" href=""{styleUrl}"" />
<script type=""text/javascript"" src=""{jqueryUrl}""></script>
<script type=""text/javascript"" src=""{vueUrl}""></script>
<script type=""text/javascript"" src=""{deviceUrl}""></script>
<span id=""{elementId}"">
    {template}
</span>
<script type=""text/javascript"">
    var match = location.search.match(new RegExp(""[\?\&]isPaymentSuccess=([^\&]+)"", ""i""));
    var isPaymentSuccess = (!match || match.length < 1) ? false : true;
    var {vueId} = new Vue({{
        el: '#{elementId}',
        data: {{
            isUserLoggin: false,
            isForceLogin: false,
            loginUrl: '{loginToPaymentUrl}',
            message: '',
            isAlipayPc: {paymentApi.IsAlipayPc.ToString().ToLower()},
            isAlipayMobi: {paymentApi.IsAlipayMobi.ToString().ToLower()},
            isWeixin: {paymentApi.IsWeixin.ToString().ToLower()},
            isJdpay: {paymentApi.IsJdpay.ToString().ToLower()},
            isMobile: device.mobile(),
            channel: 'alipay',
            isPayment: false,
            isWxQrCode: false,
            isPaymentSuccess: isPaymentSuccess,
            qrCodeUrl: ''
        }},
        methods: {{
            open: function () {{
                if (this.isForceLogin && !this.isUserLoggin) {{
                    location.href = this.loginUrl;
                }} else {{
                    this.isPayment = true;
                }}
            }},
            weixinInterval: function(orderNo) {{
                var $this = this;
                var interval = setInterval(function(){{
                    $.ajax({{
                        url : ""{apiWeixinIntervalUrl}"",
                        type: ""POST"",
                        data: JSON.stringify({{orderNo: orderNo}}),
                        contentType: ""application/json; charset=utf-8"",
                        dataType: ""json"",
                        success: function(data)
                        {{
                            if (data.isPaied) {{
                                clearInterval(interval);
                                $this.isPayment = $this.isWxQrCode = false;
                                $this.isPaymentSuccess = true;
                            }}
                        }},
                        error: function (err)
                        {{
                            var err = JSON.parse(err.responseText);
                            console.log(err.message);
                        }}
                    }});
                }}, 3000);
            }},
            weixinPaiedClose: function() {{
                this.isPayment = this.isWxQrCode = this.isPaymentSuccess = false;
                var redirectUrl = '{redirectUrl}';
                if (redirectUrl) {{
                    location.href = '{redirectUrl}';
                }}
            }},
            pay: function () {{
                var $this = this;
                var data = {{
                    siteId: {context.SiteId},
                    productId: '{productId}',
                    productName: '{productName}',
                    fee: {fee:N2},
                    channel: this.channel,
                    message: this.message,
                    isMobile: this.isMobile,
                    successUrl: '{successUrl}'
                }};
                $.ajax({{
                    url : ""{apiPayUrl}"",
                    type: ""POST"",
                    data: JSON.stringify(data),
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function(charge)
                    {{
                        if ($this.channel === 'weixin') {{
                            $this.isPayment = false;
                            $this.isWxQrCode = true;
                            $this.qrCodeUrl = charge.qrCodeUrl;
                            $this.weixinInterval(charge.orderNo);
                        }} else {{
                            document.write(charge);
                        }}
                    }},
                    error: function (err)
                    {{
                        var err = JSON.parse(err.responseText);
                        console.log(err.message);
                    }}
                }});
            }}
        }}
    }});
    
    match = location.search.match(new RegExp(""[\?\&]orderNo=([^\&]+)"", ""i""));
    var orderNo = (!match || match.length < 1) ? '' : decodeURIComponent(match[1]);
    if (isPaymentSuccess) {{
        $(document).ready(function(){{
            $.ajax({{
                url : ""{apiPaySuccessUrl}"",
                type: ""POST"",
                data: JSON.stringify({{
                    orderNo: orderNo
                }}),
                contentType: ""application/json; charset=utf-8"",
                dataType: ""json"",
                success: function(data)
                {{
                    var redirectUrl = '{redirectUrl}';
                    if (redirectUrl) location.href = '{redirectUrl}';
                }},
                error: function (err)
                {{
                    var err = JSON.parse(err.responseText);
                    console.log(err.message);
                }}
            }});
        }});
    }} else {{
        $.ajax({{
            url : ""{apiGetUrl}"",
            type: ""POST"",
            data: JSON.stringify({{
                siteId: '{context.SiteId}'
            }}),
            contentType: ""application/json; charset=utf-8"",
            dataType: ""json"",
            success: function(data)
            {{
                {vueId}.isUserLoggin = data.isUserLoggin;
                {vueId}.isForceLogin = data.isForceLogin;
            }},
            error: function (err)
            {{
                var err = JSON.parse(err.responseText);
                console.log(err.message);
            }}
        }});
    }}
</script>
";

            stlAnchor.InnerHtml = Main.ApiCollection.ParseApi.ParseInnerXml(context.InnerXml, context);
            stlAnchor.HRef = "javascript:;";
            stlAnchor.Attributes["onclick"] = $"{vueId}.open()";

            return Utils.GetControlRenderHtml(stlAnchor) + html;
        }
    }
}
