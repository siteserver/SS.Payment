<%@ Page Language="C#" Inherits="SS.Payment.Pages.PageSettings" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body>
    <div style="padding: 20px 0;">

      <div class="container">
        <form id="form" runat="server" class="form-horizontal">

          <div class="row">
            <div class="card-box">
              <div class="row">
                <div class="col-lg-10">
                  <h4 class="m-t-0 header-title"><b>快速支付设置</b></h4>
                  <p class="text-muted font-13 m-b-30">
                    在此设置快速支付功能
                  </p>
                </div>
              </div>

              <asp:Literal id="LtlMessage" runat="server" />

              <div class="form-horizontal">

                <div class="form-group">
                  <label class="col-sm-3 control-label">登录选项</label>
                  <div class="col-sm-3">
                    <asp:DropDownList ID="DdlIsForceLogin" class="form-control" runat="server">
                      <asp:ListItem Text="须登录后购买" Value="True" Selected="True" />
                      <asp:ListItem Text="直接购买无须登录" Value="False" />
                    </asp:DropDownList>
                  </div>
                  <div class="col-sm-6">

                  </div>
                </div>

                <div class="form-group m-b-0">
                  <div class="col-sm-offset-3 col-sm-9">
                    <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
                  </div>
                </div>

              </div>
            </div>
          </div>

        </form>
      </div>
    </div>
  </body>

  </html>