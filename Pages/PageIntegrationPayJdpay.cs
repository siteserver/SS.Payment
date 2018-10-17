using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Payment.Core;
using SS.Payment.Model;

namespace SS.Payment.Pages
{
    public class PageIntegrationPayJdpay : Page
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;

        public TextBox TbMerchant;
        public TextBox TbMd5Key;
        public TextBox TbDesKey;
        public TextBox TbPublicKey;
        public TextBox TbPrivateKey;

        private ConfigInfo _configInfo;
        private int _siteId;

        public static string GetRedirectUrl(int siteId)
        {
            return $"{nameof(PageIntegrationPayJdpay)}.aspx?siteId={siteId}";
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
            Utils.SelectSingleItem(DdlIsEnabled, _configInfo.IsJdpay.ToString());

            PhSettings.Visible = _configInfo.IsJdpay;

            TbMerchant.Text = _configInfo.JdpayMerchant;
            TbMd5Key.Text = _configInfo.JdpayMd5Key;
            TbDesKey.Text = _configInfo.JdpayDesKey;
            TbPublicKey.Text = _configInfo.JdpayPublicKey;
            TbPrivateKey.Text = _configInfo.JdpayPrivateKey;
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = Utils.ToBool(DdlIsEnabled.SelectedValue);
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            _configInfo.IsJdpay = Utils.ToBool(DdlIsEnabled.SelectedValue);
            //if (_configInfo.IsJdpay)
            //{
            //    try
            //    {
            //        AlipaySignature.RSASignCharSet("test", TbPrivateKey.Text, "utf-8", false, "RSA2");
            //    }
            //    catch (Exception ex)
            //    {
            //        SwalError("应用私钥格式不正确!", ex.Message);
            //        return;
            //    }
            //}

            _configInfo.JdpayMerchant = TbMerchant.Text;
            _configInfo.JdpayMd5Key = TbMd5Key.Text;
            _configInfo.JdpayDesKey = TbDesKey.Text;
            _configInfo.JdpayPublicKey = TbPublicKey.Text;
            _configInfo.JdpayPrivateKey = TbPrivateKey.Text;

            Main.SetConfigInfo(_siteId, _configInfo);

            Utils.Redirect(PageIntegrationPay.GetRedirectUrl(_siteId));
        }

        public void BtnReturn_OnClick(object sender, EventArgs e)
        {
            Utils.Redirect(PageIntegrationPay.GetRedirectUrl(_siteId));
        }
    }
}
