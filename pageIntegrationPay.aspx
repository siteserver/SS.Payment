<%@ Page Language="C#" Inherits="SS.Payment.Pages.PageIntegrationPay" %>
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
    <div class="m-l-15 m-r-15 m-t-15">

      <div class="card-box">
        <div class="m-t-0 header-title">
          支付集成设置
        </div>
        <p class="text-muted font-13 m-b-25">
          如已有渠道参数可直接进行参数填写，如尚未获得参数可交由我们代为申请。
        </p>

        <table class="table table-hover m-0">
          <thead>
            <tr>
              <th>支付渠道</th>
              <th>应用场景</th>
              <th>状态</th>
            </tr>
          </thead>
          <tbody>
            <tr style="cursor: pointer" onclick="location.href='<%=PageIntegrationPayAlipayPcUrl%>'">
              <td>
                <div>
                  <img src="./assets/images/channel_alipay.gif">支付宝电脑网站支付</div>
              </td>
              <td>
                <div class="m-t-15">PC 端网页</div>
              </td>
              <td>
                <div class="m-t-15">
                  <asp:Literal id="LtlAlipayPc" runat="server" />
                </div>
              </td>
            </tr>
            <tr style="cursor: pointer" onclick="location.href='<%=PageIntegrationPayAlipayMobiUrl%>'">
              <td>
                <div>
                  <img src="./assets/images/channel_alipay.gif">支付宝手机网站支付</div>
              </td>
              <td>
                <div class="m-t-15">移动网页</div>
              </td>
              <td>
                <div class="m-t-15">
                  <asp:Literal id="LtlAlipayMobi" runat="server" />
                </div>
              </td>
            </tr>
          </tbody>
          <tbody>
            <tr style="cursor: pointer" onclick="location.href='<%=PageIntegrationPayWeixinUrl%>'">
              <td>
                <div>
                  <img src="./assets/images/channel_weixin.gif">微信公众号支付</div>
              </td>
              <td>
                <div class="m-t-15">PC 端网页/移动网页</div>
              </td>
              <td>
                <div class="m-t-15">
                  <asp:Literal id="LtlWeixin" runat="server" />
                </div>
              </td>
            </tr>
          </tbody>
          <tbody>
            <tr style="cursor: pointer" onclick="location.href='<%=PageIntegrationPayJdpayUrl%>'">
              <td>
                <div>
                  <img src="./assets/images/channel_jdpay.gif">京东支付</div>
              </td>
              <td>
                <div class="m-t-15">PC 端网页/移动网页</div>
              </td>
              <td>
                <div class="m-t-15">
                  <asp:Literal id="LtlJdpay" runat="server" />
                </div>
              </td>
            </tr>
          </tbody>
        </table>


      </div>

      <asp:Literal id="LtlScript" runat="server" />

    </div>
  </body>

  </html>