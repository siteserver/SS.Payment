using System;
using System.Web;
using System.Web.UI.HtmlControls;
using SiteServer.Plugin;

namespace SS.Payment.Core
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
        public const string AttributeIsForceLogin = "isForceLogin";

        public static string Parse(IParseContext context)
        {
            var stlAnchor = new HtmlAnchor();

            var productId = string.Empty;
            var productName = string.Empty;
            decimal fee = 0;
            var loginUrl = string.Empty;
            var redirectUrl = string.Empty;
            var weixinName = string.Empty;
            var isForceLogin = false;

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];
                if (Utils.EqualsIgnoreCase(name, AttributeProductId))
                {
                    productId = Context.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeProductName))
                {
                    productName = Context.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeFee))
                {
                    value = Context.ParseApi.ParseAttributeValue(value, context);
                    decimal.TryParse(value, out fee);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeLoginUrl))
                {
                    loginUrl = Context.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeRedirectUrl))
                {
                    redirectUrl = Context.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeWeixinName))
                {
                    weixinName = Context.ParseApi.ParseAttributeValue(value, context);
                }
                else if (Utils.EqualsIgnoreCase(name, AttributeIsForceLogin))
                {
                    isForceLogin = Utils.ToBool(Context.ParseApi.ParseAttributeValue(value, context));
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
            var currentUrl = Context.ParseApi.GetCurrentUrl(context);
            var loginToPaymentUrl = $"{loginUrl}?redirectUrl={HttpUtility.UrlEncode(currentUrl)}";

            if (string.IsNullOrEmpty(productName) || fee <= 0) return string.Empty;

            if (!string.IsNullOrEmpty(weixinName))
            {
                weixinName = $@"<p style=""text-align: center"">{weixinName}</p>";
            }

            string template = $@"
<div class=""mask1_bg mask1_bg_cut"" v-show=""isPayment || isWxQrCode || isPaymentSuccess"" @click=""isPayment = isWxQrCode = isPaymentSuccess = false"" style=""display: none""></div>
<div class=""detail_alert detail_alert_cut"" v-show=""isPayment"" style=""display: none"">
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
        <li v-show=""isAliPay"" :class=""{{ pay_cut: channel === 'AliPay' }}"" @click=""channel = 'AliPay'"" class=""channel_alipay""><b></b></li>
        <li v-show=""isWxPay"" :class=""{{ pay_cut: channel === 'WxPay' }}"" @click=""channel = 'WxPay'"" class=""channel_weixin""><b></b></li>
    </ul>
    <div class=""mess_text""></div>
    <a href=""javascript:;"" @click=""pay"" class=""pay_go"">立即支付</a>
  </div>
</div>
<div class=""detail_alert detail_alert_cut"" v-show=""isWxQrCode"" style=""display: none"">
  <div class=""close"" @click=""isPayment = isWxQrCode = isPaymentSuccess = false""></div>
  <div class=""pay_list"">
    <p style=""text-align: center""> 打开手机微信，扫一扫下面的二维码，即可完成支付</p>
    {weixinName}
    <p style=""margin-left: 195px;margin-bottom: 80px;""><img :src=""qrCodeUrl"" style=""width: 200px;height: 200px;""></p>
  </div>
</div>
<div class=""detail_alert detail_alert_cut"" v-show=""isPaymentSuccess"" style=""display: none"">
  <div class=""close"" @click=""isPayment = isWxQrCode = isPaymentSuccess = false""></div>
  <div class=""pay_list"">
    <p style=""text-align: center"">支付成功，谢谢支持</p>
    <div class=""mess_text""></div>
    <a href=""javascript:;"" @click=""weixinPayedClose"" class=""pay_go"">关闭</a>
  </div>
</div>
";

            var apiUrl = Context.Environment.ApiUrl;

            var elementId = "el-" + Guid.NewGuid();
            var vueId = "v" + Guid.NewGuid().ToString().Replace("-", string.Empty);
            var styleUrl = UrlUtils.GetAssetsUrl("css/style.css");
            var jqueryUrl = UrlUtils.GetAssetsUrl("js/jquery.min.js");
            var vueUrl = UrlUtils.GetAssetsUrl("js/vue.min.js");
            var deviceUrl = UrlUtils.GetAssetsUrl("js/device.min.js");
            //var apiPayUrl = $"{apiUrl}/{nameof(ApiPay)}";
            //var apiPaySuccessUrl = $"{apiUrl}/{nameof(ApiPaySuccess)}";
            //var successUrl = Context.ParseApi.GetCurrentUrl(context) + "?isPaymentSuccess=" + true;
            //var apiWeixinIntervalUrl = $"{apiUrl}/{nameof(ApiWeixinInterval)}";
            //var apiGetUrl = $"{apiUrl}/{nameof(ApiGet)}";
            var apiPayUrl = UrlUtils.GetPayUrl(apiUrl);
            var apiPaySuccessUrl = UrlUtils.GetPaySuccessUrl(apiUrl);
            var successUrl = UrlUtils.GetSuccessUrl(context);
            var apiWxPayIntervalUrl = UrlUtils.GetWxPayIntervalUrl(apiUrl);
            var apiStatusUrl = UrlUtils.GetStatusUrl(apiUrl);

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
            isForceLogin: {isForceLogin.ToString().ToLower()},
            loginUrl: '{loginToPaymentUrl}',
            message: '',
            isAliPay: {paymentApi.IsAliPay.ToString().ToLower()},
            isWxPay: {paymentApi.IsWxPay.ToString().ToLower()},
            isMobile: device.mobile(),
            channel: 'AliPay',
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
            wxPayInterval: function(orderNo) {{
                var $this = this;
                var interval = setInterval(function(){{
                    $.ajax({{
                        url : ""{apiWxPayIntervalUrl}"",
                        xhrFields: {{
                            withCredentials: true
                        }},
                        type: ""POST"",
                        data: JSON.stringify({{orderNo: orderNo}}),
                        contentType: ""application/json; charset=utf-8"",
                        dataType: ""json"",
                        success: function(data)
                        {{
                            if (data.isPayed) {{
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
            weixinPayedClose: function() {{
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
                    xhrFields: {{
                        withCredentials: true
                    }},
                    type: ""POST"",
                    data: JSON.stringify(data),
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function(charge)
                    {{
                        if ($this.channel === 'WxPay') {{
                            $this.isPayment = false;
                            $this.isWxQrCode = true;
                            $this.qrCodeUrl = charge.qrCodeUrl;
                            $this.wxPayInterval(charge.orderNo);
                        }} else {{
                            location.href = charge;
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
                xhrFields: {{
                    withCredentials: true
                }},
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
            url : ""{apiStatusUrl}"",
            xhrFields: {{
                withCredentials: true
            }},
            type: ""POST"",
            data: JSON.stringify({{
                siteId: '{context.SiteId}'
            }}),
            contentType: ""application/json; charset=utf-8"",
            dataType: ""json"",
            success: function(data)
            {{
                {vueId}.isUserLoggin = data.isUserLoggin;
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

            stlAnchor.InnerHtml = Context.ParseApi.Parse(context.StlInnerHtml, context);
            stlAnchor.HRef = "javascript:;";
            stlAnchor.Attributes["onclick"] = $"{vueId}.open()";

            return Utils.GetControlRenderHtml(stlAnchor) + html;
        }

        public static void ApiRedirect(string successUrl)
        {
            Utils.Redirect(successUrl);
        }
    }
}
