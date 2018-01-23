using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using SiteServer.Plugin;
using SiteServer.Plugin.Features;
using SS.Payment.Core;
using SS.Payment.Model;
using SS.Payment.Pages;
using SS.Payment.Parse;
using SS.Payment.Provider;

namespace SS.Payment
{
    public class Main : PluginBase, ITable, IMenu, IParse, IWebApi
    {
        public static IContext Context { get; private set; }

        public static Dao Dao { get; private set; }
        public static RecordDao RecordDao { get; private set; }

        public override Action<IContext> PluginActive => context =>
        {
            Context = context;

            Dao = new Dao(context);
            RecordDao = new RecordDao(context);
        };

        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public static ConfigInfo GetConfigInfo(int publishmentSystemId)
        {
            if (!ConfigInfoDict.ContainsKey(publishmentSystemId))
            {
                ConfigInfoDict[publishmentSystemId] = Context.ConfigApi.GetConfig<ConfigInfo>(publishmentSystemId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[publishmentSystemId];
        }

        public override Dictionary<string, List<TableColumn>> Tables => new Dictionary
            <string, List<TableColumn>>
            {
                {
                    RecordDao.TableName, RecordDao.Columns
                }
            };

        public override Func<int, Menu> SiteMenu => publishmentSystemId => new Menu
        {
            Text = "快速支付",
            IconClass = "ion-card",
            Menus = new List<Menu>
            {
                new Menu
                {
                    Text = "快速支付记录",
                    Href = $"{nameof(PageRecords)}.aspx"
                },
                new Menu
                {
                    Text = "快速支付设置",
                    Href = $"{nameof(PageSettings)}.aspx"
                }
            }
        };

        public override Dictionary<string, Func<IParseContext, string>> ElementsToParse => new Dictionary<string, Func<IParseContext, string>>
        {
            {StlPayment.ElementName, StlPayment.Parse }
        };

        public override Func<IRequestContext, string, object> JsonPostWithName
            => (context, name) =>
            {
                if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiGet)))
                {
                    return StlPayment.ApiGet(context);
                }
                if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiPay)))
                {
                    return StlPayment.ApiPay(context);
                }
                if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiPaySuccess)))
                {
                    return StlPayment.ApiPaySuccess(context);
                }
                if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiWeixinInterval)))
                {
                    return StlPayment.ApiWeixinInterval(context);
                }

                return null;
            };

        public override Func<IRequestContext, string, HttpResponseMessage> HttpGetWithName => (context, name) =>
        {
            if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiQrCode)))
            {
                return StlPayment.ApiQrCode(context);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        };

        public override Func<IRequestContext, string, string, HttpResponseMessage> HttpPostWithNameAndId => (context, name, id) =>
        {
            if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiWeixinNotify)))
            {
                return StlPayment.ApiWeixinNotify(context, id);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        };
    }
}