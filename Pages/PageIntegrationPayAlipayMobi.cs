using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aop.Api.Util;
using SS.Payment.Core;
using SS.Payment.Model;

namespace SS.Payment.Pages
{
    public class PageIntegrationPayAlipayMobi : Page
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;
        public DropDownList DdlIsMApi;

        public PlaceHolder PhMApi;
        public TextBox TbPid;
        public TextBox TbMd5;

        public PlaceHolder PhOpenApi;
        public TextBox TbAppId;
        public TextBox TbPublicKey;
        public TextBox TbPrivateKey;

        private ConfigInfo _configInfo;
        private int _siteId;

        public static string GetRedirectUrl(int siteId)
        {
            return $"{nameof(PageIntegrationPayAlipayMobi)}.aspx?siteId={siteId}";
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);
            _configInfo = Main.GetConfigInfo(_siteId);

            if (!Main.Request.AdminPermissions.HasSitePermissions(_siteId, Main.PluginId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (IsPostBack) return;

            Utils.AddListItems(DdlIsEnabled, "开通", "不开通");
            Utils.SelectSingleItem(DdlIsEnabled, _configInfo.IsAlipayMobi.ToString());

            Utils.AddListItems(DdlIsMApi, "手机网站支付（mapi）", "手机网站支付（openapi）");
            Utils.SelectSingleItem(DdlIsMApi, _configInfo.AlipayMobiIsMApi.ToString());

            PhSettings.Visible = _configInfo.IsAlipayMobi;

            TbAppId.Text = _configInfo.AlipayMobiAppId;
            TbPid.Text = _configInfo.AlipayMobiPid;
            TbMd5.Text = _configInfo.AlipayMobiMd5;
            TbPublicKey.Text = _configInfo.AlipayMobiPublicKey;
            TbPrivateKey.Text = _configInfo.AlipayMobiPrivateKey;

            PhMApi.Visible = _configInfo.AlipayMobiIsMApi;
            PhOpenApi.Visible = !PhMApi.Visible;
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = Utils.ToBool(DdlIsEnabled.SelectedValue);
        }

        public void DdlIsMApi_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhMApi.Visible = Utils.ToBool(DdlIsMApi.SelectedValue);
            PhOpenApi.Visible = !PhMApi.Visible;
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            _configInfo.IsAlipayMobi = Utils.ToBool(DdlIsEnabled.SelectedValue);
            if (_configInfo.IsAlipayMobi)
            {
                try
                {
                    AlipaySignature.RSASignCharSet("test", TbPrivateKey.Text, "utf-8", false, "RSA2");
                }
                catch (Exception ex)
                {
                    Utils.SwalError(Page, "应用私钥格式不正确!", ex.Message);
                    return;
                }
            }

            _configInfo.AlipayMobiIsMApi = Utils.ToBool(DdlIsMApi.SelectedValue);
            _configInfo.AlipayMobiAppId = TbAppId.Text;
            _configInfo.AlipayMobiPid = TbPid.Text;
            _configInfo.AlipayMobiMd5 = TbMd5.Text;
            _configInfo.AlipayMobiPublicKey = TbPublicKey.Text;
            _configInfo.AlipayMobiPrivateKey = TbPrivateKey.Text;

            Main.SetConfigInfo(_siteId, _configInfo);

            Utils.Redirect(PageIntegrationPay.GetRedirectUrl(_siteId));
        }

        public void BtnReturn_OnClick(object sender, EventArgs e)
        {
            Utils.Redirect(PageIntegrationPay.GetRedirectUrl(_siteId));
        }
    }
}
