using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Payment.Core;

namespace SS.Payment
{
    public class Main : PluginBase
    {
        public static string PluginId { get; private set; }

        public static ConfigInfo GetConfigInfo(int siteId)
        {
            var globalConfigInfo = Context.ConfigApi.GetConfig<ConfigInfo>(PluginId, 0);
            if (globalConfigInfo == null)
            {
                globalConfigInfo = new ConfigInfo();
                Context.ConfigApi.SetConfig(PluginId, 0, globalConfigInfo);
            }

            return Context.ConfigApi.GetConfig<ConfigInfo>(PluginId, siteId) ?? globalConfigInfo;
        }

        public static void SetConfigInfo(int siteId, ConfigInfo configInfo)
        {
            Context.ConfigApi.SetConfig(PluginId, siteId, configInfo);
            Context.ConfigApi.SetConfig(PluginId, 0, configInfo);
        }

        public override void Startup(IService service)
        {
            PluginId = Id;

            var repository = new RecordRepository();

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
                            Href = "pages/payment/records.html"
                        },
                        new Menu
                        {
                            Text = "在线支付设置",
                            Href = "pages/payment/settings.html"
                        }
                    }
                })
                .AddDatabaseTable(repository.TableName, repository.TableColumns)
                .AddStlElementParser(StlPayment.ElementName, StlPayment.Parse);

            //service.RestApiPost += Service_RestApiPost;
        }

        //private object Service_RestApiPost(object sender, RestApiEventArgs args)
        //{
        //    var request = args.Request;

        //    if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiGet)))
        //    {
        //        return StlPayment.ApiGet(request);
        //    }
        //    if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiPay)))
        //    {
        //        return StlPayment.ApiPay(request);
        //    }
        //    if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiPaySuccess)))
        //    {
        //        return StlPayment.ApiPaySuccess(request);
        //    }
        //    if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiWeixinInterval)))
        //    {
        //        return StlPayment.ApiWeixinInterval(request);
        //    }
        //    if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiWeixinNotify)))
        //    {
        //        return StlPayment.ApiWeixinNotify(request, args.RouteId);
        //    }
        //    if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlPayment.ApiRedirect)))
        //    {
        //        var successUrl = request.GetPostString("successUrl");
        //        StlPayment.ApiRedirect(successUrl);
        //    }

        //    return new HttpResponseMessage(HttpStatusCode.NotFound);
        //}
    }
}