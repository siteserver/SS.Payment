var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/test/",
  withCredentials: true
});
var $paymentApi = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/payment/",
  withCredentials: true
});

var data = {
  siteId: utils.getQueryInt("siteId"),
  type: utils.getQueryString("type"),
  pageLoad: false,
  pageAlert: null,
  html: null,
  wxPayQrCodeUrl: null,
  wxPayOrderNo: null,
  wxPayInterval: null,
  wxPaySuccess: false
};

var methods = {
  apiGetConfig: function() {
    var $this = this;

    if ($this.pageLoad) utils.loading(true);
    $api
      .get("", {
        params: {
          siteId: $this.siteId,
          type: $this.type
        }
      })
      .then(function(response) {
        var res = response.data;
        if (res.value) {
          location.href = res.value;
        } else {
          $this.wxPayQrCodeUrl = res.wxPayQrCodeUrl;
          $this.wxPayOrderNo = res.wxPayOrderNo;
          $this.apiWxPayInterval();
        }
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.pageLoad = true;
        utils.loading(false);
      });
  },

  apiWxPayInterval: function() {
    var $this = this;
    $this.wxPayInterval = setInterval(function() {
      utils.loading(false);
      $paymentApi
        .get("WxPayInterval", {
          params: {
            orderNo: $this.wxPayOrderNo
          }
        })
        .then(function(response) {
          var res = response.data;

          if (res.value) {
            clearInterval($this.wxPayInterval);
            $this.wxPaySuccess = true;
          }
        })
        .catch(function(error) {
          $this.pageAlert = utils.getPageAlert(error);
        })
        .then(function() {
          utils.loading(false);
        });
    }, 3000);
  }
};

new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function() {
    this.apiGetConfig();
  }
});
