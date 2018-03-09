<%@ Page Language="C#" Inherits="SS.Payment.Pages.PageIntegrationPayJdpay" %>
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

      <div class="card-box">
        <div class="m-t-0 header-title">
          京东支付集成
        </div>
        <p class="text-muted font-13 m-b-25">
          在此设置京东支付
        </p>

        <div class="form-group">
          <label class="col-form-label">是否启用京东支付</label>
          <asp:DropDownList id="DdlIsEnabled" AutoPostBack="true" OnSelectedIndexChanged="DdlIsEnabled_SelectedIndexChanged" class="form-control"
            runat="server"></asp:DropDownList>
        </div>

        <asp:PlaceHolder id="PhSettings" runat="server">

          <div class="form-group">
            <label class="col-form-label">商户号
              <asp:RequiredFieldValidator ControlToValidate="TbMerchant" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMerchant" ValidationExpression="[^']+" ErrorMessage=" *"
                ForeColor="red" Display="Dynamic" />
            </label>
            <asp:TextBox id="TbMerchant" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">
              <a href="https://biz.jd.com" target="_blank">京东金融</a> - 商户号</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">MD5 密钥
              <asp:RequiredFieldValidator ControlToValidate="TbMd5Key" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMd5Key" ValidationExpression="[^']+" ErrorMessage=" *"
                ForeColor="red" Display="Dynamic" />
            </label>
            <asp:TextBox id="TbMd5Key" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">
              <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 密钥设置 - 京东支付密钥设置 - MD5 密钥</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">DES密钥
              <asp:RequiredFieldValidator ControlToValidate="TbDesKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDesKey" ValidationExpression="[^']+" ErrorMessage=" *"
                ForeColor="red" Display="Dynamic" />
            </label>
            <asp:TextBox id="TbDesKey" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">
              <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 密钥设置 - 京东支付密钥设置 - DES密钥</small>
          </div>

          <div class="form-group">
            <label class="col-form-label">商户RSA公钥
              <asp:RequiredFieldValidator ControlToValidate="TbPublicKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPublicKey" ValidationExpression="[^']+" ErrorMessage=" *"
                ForeColor="red" Display="Dynamic" />
            </label>
            <asp:TextBox id="TbPublicKey" TextMode="MultiLine" Rows="6" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">
              <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 密钥设置 - 京东支付密钥设置 - 商户RSA公钥</small>
            </small>
          </div>

          <div class="form-group">
            <label class="col-form-label">商户RSA私钥
              <asp:RequiredFieldValidator ControlToValidate="TbPrivateKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbPrivateKey" ValidationExpression="[^']+" ErrorMessage=" *"
                ForeColor="red" Display="Dynamic" />
            </label>
            <asp:TextBox id="TbPrivateKey" TextMode="MultiLine" Rows="15" class="form-control" runat="server"></asp:TextBox>
            <small class="form-text text-muted">
              <a href="https://biz.jd.com" target="_blank">京东金融</a> - 安全中心 - 密钥设置 - 京东支付密钥设置 - 商户RSA私钥</small>
            </small>
          </div>

        </asp:PlaceHolder>

        <hr />

        <div class="text-center">
          <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <asp:Button class="btn m-l-10" Text="返 回" OnClick="BtnReturn_OnClick" CausesValidation="false" runat="server" />
        </div>
      </div>

    </form>
  </body>

  </html>