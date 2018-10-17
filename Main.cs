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
        public static string PluginId { get; private set; }

        public static IRequest Request => Context.Request;

        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public static ConfigInfo GetConfigInfo(int siteId)
        {
            if (!ConfigInfoDict.ContainsKey(siteId))
            {
                ConfigInfoDict[siteId] = Context.ConfigApi.GetConfig<ConfigInfo>(PluginId, siteId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[siteId];
        }

        public static void SetConfigInfo(int siteId, ConfigInfo configInfo)
        {
            ConfigInfoDict[siteId] = configInfo;
            Context.ConfigApi.SetConfig(PluginId, siteId, configInfo);
        }

        public override void Startup(IService service)
        {
            PluginId = Id;

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

            service.RestApiPost += Service_RestApiPost;
            service.RestApiGet += Service_RestApiGet;
        }

        private object Service_RestApiGet(object sender, RestApiEventArgs args)
        {
            var request = args.Request;

            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiQrCode)))
            {
                return StlPayment.ApiQrCode(request);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        private object Service_RestApiPost(object sender, RestApiEventArgs args)
        {
            var request = args.Request;

            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiGet)))
            {
                return StlPayment.ApiGet(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiPay)))
            {
                return StlPayment.ApiPay(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiPaySuccess)))
            {
                return StlPayment.ApiPaySuccess(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiWeixinInterval)))
            {
                return StlPayment.ApiWeixinInterval(request);
            }
            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiWeixinNotify)))
            {
                return StlPayment.ApiWeixinNotify(request, args.RouteId);
            }
            if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiRedirect)))
            {
                var successUrl = request.GetPostString("successUrl");
                StlPayment.ApiRedirect(successUrl);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}