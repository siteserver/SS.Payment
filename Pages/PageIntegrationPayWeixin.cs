using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Payment.Core;
using SS.Payment.Model;

namespace SS.Payment.Pages
{
    public class PageIntegrationPayWeixin : Page
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;
        public TextBox TbAppId;
        public TextBox TbAppSecret;
        public TextBox TbMchId;
        public TextBox TbKey;

        private ConfigInfo _configInfo;
        private int _siteId;

        public static string GetRedirectUrl(int siteId)
        {
            return Main.Instance.PluginApi.GetPluginUrl($"{nameof(PageIntegrationPayWeixin)}.aspx?siteId={siteId}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);
            _configInfo = Main.Instance.GetConfigInfo(_siteId);

            if (!Main.Instance.AdminApi.IsSiteAuthorized(_siteId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (IsPostBack) return;

            Utils.AddListItems(DdlIsEnabled, "开通", "不开通");
            Utils.SelectSingleItem(DdlIsEnabled, _configInfo.IsWeixin.ToString());

            PhSettings.Visible = _configInfo.IsWeixin;

            TbAppId.Text = _configInfo.WeixinAppId;
            TbAppSecret.Text = _configInfo.WeixinAppSecret;
            TbMchId.Text = _configInfo.WeixinMchId;
            TbKey.Text = _configInfo.WeixinKey;
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = Utils.ToBool(DdlIsEnabled.SelectedValue);
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            _configInfo.IsWeixin = Utils.ToBool(DdlIsEnabled.SelectedValue);
            _configInfo.WeixinAppId = TbAppId.Text;
            _configInfo.WeixinAppSecret = TbAppSecret.Text;
            _configInfo.WeixinMchId = TbMchId.Text;
            _configInfo.WeixinKey = TbKey.Text;

            Main.Instance.SetConfigInfo(_siteId, _configInfo);

            Utils.Redirect(PageIntegrationPay.GetRedirectUrl(_siteId));
        }

        public void BtnReturn_OnClick(object sender, EventArgs e)
        {
            Utils.Redirect(PageIntegrationPay.GetRedirectUrl(_siteId));
        }
    }
}
