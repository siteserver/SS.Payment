using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Payment.Controls;
using SS.Payment.Core;
using SS.Payment.Model;

namespace SS.Payment.Pages
{
	public class PageRecords : Page
	{
	    public Literal LtlMessage;

        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnDelete;

        private int _siteId;

        public static string GetRedirectUrl(int siteId)
        {
            return Plugin.FilesApi.GetPluginUrl($"{nameof(PageRecords)}.aspx?siteId={siteId}");
        }

		public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);

            if (!Plugin.AdminApi.IsSiteAuthorized(_siteId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["delete"]) &&
                !string.IsNullOrEmpty(Request.QueryString["idCollection"]))
            {
                var array = Request.QueryString["idCollection"].Split(',');
                var list = array.Select(s => Convert.ToInt32(s)).ToList();
                Plugin.RecordDao.Delete(list);
                LtlMessage.Text = Utils.GetMessageHtml("删除成功！", true);
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 20;
            SpContents.SelectCommand = Plugin.RecordDao.GetSelectString(_siteId);
            SpContents.SortField = nameof(RecordInfo.Id);
            SpContents.SortMode = "DESC";
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            SpContents.DataBind();

            BtnDelete.Attributes.Add("onclick", Utils.ReplaceNewline($@"
var ids = [];
$(""input[name='idCollection']:checked"").each(function () {{
    ids.push($(this).val());}}
);
if (ids.length > 0){{
    {Utils.SwalWarning("删除记录", "此操作将删除所选记录，确定吗？", "取 消", "删 除",
                $"location.href='{GetRedirectUrl(_siteId)}&delete={true}&idCollection=' + ids.join(',')")}
}} else {{
    {Utils.SwalError("请选择需要删除的记录！", string.Empty)}
}}
;return false;", string.Empty));

        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var message = Utils.EvalString(e.Item.DataItem, nameof(RecordInfo.Message));
            var productName = Utils.EvalString(e.Item.DataItem, nameof(RecordInfo.ProductName));
            var fee = Utils.EvalDecimal(e.Item.DataItem, nameof(RecordInfo.Fee));
            var channel = Utils.EvalString(e.Item.DataItem, nameof(RecordInfo.Channel));
            var isPaied = Utils.EvalBool(e.Item.DataItem, nameof(RecordInfo.IsPaied));
            var addDate = Utils.EvalDateTime(e.Item.DataItem, nameof(RecordInfo.AddDate));

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlMessage = (Literal)e.Item.FindControl("ltlMessage");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlAmount = (Literal)e.Item.FindControl("ltlAmount");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");

            ltlTitle.Text = productName;
            ltlMessage.Text = message;
            ltlAmount.Text = fee.ToString("N2");
            ltlChannel.Text = Utils.GetChannelName(channel);
            ltlStatus.Text = isPaied ? "已支付" : "未支付";
            ltlAddDate.Text = addDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
