using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using SiteServer.Plugin;
using SS.Payment.Core;
using SS.Payment.Model;
using SS.Payment.Pages;
using SS.Payment.Parse;
using SS.Payment.Provider;

namespace SS.Payment
{
    public class Main : PluginBase
    {
        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public static IApiCollection ApiCollection { get; set; }

        public static ConfigInfo GetConfigInfo(int siteId)
        {
            if (!ConfigInfoDict.ContainsKey(siteId))
            {
                ConfigInfoDict[siteId] = ApiCollection.ConfigApi.GetConfig<ConfigInfo>(siteId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[siteId];
        }

        public static void SetConfigInfo(int siteId, ConfigInfo configInfo)
        {
            ConfigInfoDict[siteId] = configInfo;
            ApiCollection.ConfigApi.SetConfig(siteId, configInfo);
        }

        public override void Startup(IService service)
        {
            ApiCollection = this;

            Dao.Init(ConnectionString, DataApi);
            RecordDao.Init(ConnectionString, DataApi);

            service
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "在线支付",
                    IconClass = "ion-card",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "在线支付记录",
                            Href = $"{nameof(PageRecords)}.aspx"
                        },
                        new Menu
                        {
                            Text = "支付集成设置",
                            Href = $"{nameof(PageIntegrationPay)}.aspx"
                        }
                    }
                })
                .AddDatabaseTable(RecordDao.TableName, RecordDao.Columns)
                .AddStlElementParser(StlPayment.ElementName, StlPayment.Parse);

            service.ApiPost += Service_ApiPost;
            service.ApiGet += Service_ApiGet;
        }

        private object Service_ApiGet(object sender, ApiEventArgs args)
        {
            var request = args.Request;

            if (Utils.EqualsIgnoreCase(args.Action, nameof(StlPayment.ApiQrCode)))
            {
                return StlPayment.ApiQrCode(request);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        private object Service_ApiPost(object sender, ApiEventArgs args)
        {
            var request = args.Request;

            if (Utils.EqualsIgnoreCase(args.Action, nameof(StlPayment.ApiGet)))
            {
                return StlPayment.ApiGet(request);
            }
            if (Utils.EqualsIgnoreCase(args.Action, nameof(StlPayment.ApiPay)))
            {
                return StlPayment.ApiPay(request);
            }
            if (Utils.EqualsIgnoreCase(args.Action, nameof(StlPayment.ApiPaySuccess)))
            {
                return StlPayment.ApiPaySuccess(request);
            }
            if (Utils.EqualsIgnoreCase(args.Action, nameof(StlPayment.ApiWeixinInterval)))
            {
                return StlPayment.ApiWeixinInterval(request);
            }
            if (Utils.EqualsIgnoreCase(args.Action, nameof(StlPayment.ApiWeixinNotify)))
            {
                return StlPayment.ApiWeixinNotify(request, args.Id);
            }
            if (Utils.EqualsIgnoreCase(args.Action, nameof(StlPayment.ApiRedirect)))
            {
                var successUrl = request.GetPostString("successUrl");
                StlPayment.ApiRedirect(successUrl);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}