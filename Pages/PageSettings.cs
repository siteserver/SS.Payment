using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Payment.Core;
using SS.Payment.Model;

namespace SS.Payment.Pages
{
    public class PageSettings : Page
    {
        public Literal LtlMessage;
        public DropDownList DdlIsForceLogin;
        public TextBox TbExpressCost;

        private int _siteId;
        private ConfigInfo _configInfo;

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);

            if (!Main.Instance.AdminApi.IsSiteAuthorized(_siteId))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
                return;
            }

            _configInfo = Main.GetConfigInfo(_siteId);

            if (IsPostBack) return;

            Utils.SelectListItems(DdlIsForceLogin, _configInfo.IsForceLogin.ToString());
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            _configInfo.IsForceLogin = Convert.ToBoolean(DdlIsForceLogin.SelectedValue);

            Main.SetConfigInfo(_siteId, _configInfo);
            LtlMessage.Text = Utils.GetMessageHtml("快速支付设置修改成功！", true);
        }
    }
}