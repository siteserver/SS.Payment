<%@ Page Language="C#" Inherits="SS.Payment.Pages.PageRecords" %>
  <%@ Register TagPrefix="ctrl" Namespace="SS.Payment.Controls" Assembly="SS.Payment" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
      <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
      <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
      <script src="assets/plugin-utils/js/jquery.min.js"></script>
      <script src="assets/plugin-utils/js/bootstrap.min.js"></script>
      <script src="assets/js/sweetalert.min.js"></script>
    </head>

    <body>
      <form class="m-l-15 m-r-15 m-t-15" runat="server">

        <div class="row">
          <div class="col-sm-12">
            <div class="card-box">
              <h4 class="text-dark  header-title m-t-0">
                <asp:Literal id="LtlPageTitle" runat="server"></asp:Literal>
              </h4>
              <p class="text-muted m-b-25 font-13"></p>
              <asp:Literal id="LtlMessage" runat="server" />

              <table class="table table-bordered table-hover m-0">
                <thead>
                  <tr class="info thead">
                    <th>标题</th>
                    <th>留言</th>
                    <th>支付方式</th>
                    <th>金额</th>
                    <th>状态</th>
                    <th class="text-center" style="width:160px;">时间</th>
                    <th width="20" class="text-center">
                      <input onclick="var checked = this.checked;$(':checkbox').each(function(){$(this)[0].checked = checked;checked ? $(this).parents('tr').addClass('success') : $(this).parents('tr').removeClass('success')});"
                        type="checkbox" />
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <asp:Repeater ID="RptContents" runat="server">
                    <itemtemplate>
                      <tr onClick="$(this).toggleClass('success');$(this).find(':checkbox')[0].checked = $(this).hasClass('success');">
                        <td>
                          <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                        </td>
                        <td>
                          <asp:Literal ID="ltlMessage" runat="server"></asp:Literal>
                        </td>
                        <td>
                          <asp:Literal ID="ltlChannel" runat="server"></asp:Literal>
                        </td>
                        <td>
                          <asp:Literal ID="ltlAmount" runat="server"></asp:Literal>
                        </td>
                        <td>
                          <asp:Literal ID="ltlStatus" runat="server"></asp:Literal>
                        </td>
                        <td class="text-center">
                          <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                        </td>
                        <td class="text-center">
                          <input type="checkbox" name="idCollection" value='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />
                        </td>
                      </tr>
                    </itemtemplate>
                  </asp:Repeater>
                </tbody>


              </table>

              <div class="m-b-25"></div>

              <ctrl:sqlPager id="SpContents" runat="server" class="table table-pager" />
              <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />

            </div>
          </div>
        </div>

      </form>
    </body>

    </html>