var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/settings/",
  withCredentials: true
});

var data = {
  siteId: utils.getQueryInt("siteId"),
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  configInfo: null
};

var methods = {
  apiGetConfig: function() {
    var $this = this;

    if ($this.pageLoad) utils.loading(true);
    $api
      .get("", {
        params: {
          siteId: $this.siteId
        }
      })
      .then(function(response) {
        var res = response.data;

        $this.configInfo = res.value;
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.pageLoad = true;
        utils.loading(false);
      });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post('?siteId=' + this.siteId, this.configInfo).then(function (response) {
      var res = response.data;

      $this.pageType ='';
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnAddClick: function(item) {
    location.href = utils.getPageUrl("advertisementAdd.html");
  },

  btnEditClick: function(item) {
    location.href =
      utils.getPageUrl("advertisementAdd.html") + "&advertisementId=" + item.id;
  },

  btnDeleteClick: function(item) {
    var $this = this;

    utils.alertDelete({
      title: "删除漂浮广告",
      text: "此操作将删除漂浮广告" + item.advertisementName + "，确定吗？",
      callback: function() {
        $this.apiDelete(item);
      }
    });
  },

  btnNavClick: function(pageName) {
    location.href = utils.getPageUrl(pageName);
  },

  btnTestClick: function(type) {
    window.open(
      "test.html?apiUrl=" +
        utils.getQueryString("apiUrl") +
        "&pluginId=" +
        utils.getQueryString("pluginId") +
        "&siteId=" +
        this.siteId +
        "&type=" +
        type
    );
  },

  btnBackClick: function() {
    location.href = utils.getPageUrl('settings.html');
  },
  
  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiSubmit();
      }
    });
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
