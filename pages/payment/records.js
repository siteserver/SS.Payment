var $api = axios.create({
  baseURL:
    utils.getQueryString("apiUrl") +
    "/" +
    utils.getQueryString("pluginId") +
    "/pages/records/",
  withCredentials: true
});

var data = {
  siteId: utils.getQueryInt('siteId'),
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  page: 1,
  recordInfoList: [],
  count: null,
  pages: null,
  pageOptions: null
};

var methods = {
  apiGet: function (page) {
    var $this = this;

    if ($this.pageLoad) {
      utils.loading(true);
    }

    $api.get('', {
      params: {
        siteId: this.siteId,
        page: page
      }
    }).then(function (response) {
      var res = response.data;

      if ($this.pageLoad) {
        utils.loading(false);
        utils.up();
      } else {
        $this.pageLoad = true;
      }

      $this.recordInfoList = res.value;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.page = res.page;
      $this.pageOptions = [];
      for (var i = 1; i <= $this.pages; i++) {
        $this.pageOptions.push(i);
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  apiDelete: function (recordId) {
    var $this = this;

    utils.loading(true);
    $api.delete('', {
      params: {
        siteId: this.siteId,
        page: this.page,
        recordId: recordId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.page = res.page;
      $this.pageOptions = [];
      for (var i = 1; i <= $this.pages; i++) {
        $this.pageOptions.push(i);
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  getChannel: function(recordInfo) {
    if (recordInfo.channel === 'AliPay') {
      return '支付宝';
    } else if (recordInfo.channel === 'WxPay') {
      return '微信支付';
    }
    return '';
  },

  btnDeleteClick: function (recordId) {
    var $this = this;

    utils.alertDelete({
      title: '删除支付记录',
      text: '此操作将删除支付记录，确定吗？',
      callback: function () {
        $this.apiDelete(recordId);
      }
    });
  },

  btnNavClick: function(pageName) {
    location.href = utils.getPageUrl(pageName);
  },

  btnFirstPageClick: function () {
    if (this.page === 1) return;
    this.apiGet(1);
  },

  btnPrevPageClick: function () {
    if (this.page - 1 <= 0) return;
    this.apiGet(this.page - 1);
  },

  btnNextPageClick: function () {
    if (this.page + 1 > this.pages) return;
    this.apiGet(this.page + 1);
  },

  btnLastPageClick: function () {
    if (this.page + 1 > this.pages) return;
    this.apiGet(this.pages);
  },

  btnPageSelectClick(option) {
    this.apiGet(option);
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet(1);
  }
});
