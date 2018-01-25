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
    public class Main : IPlugin
    {
        public static DatabaseType DatabaseType { get; private set; }
        public static string ConnectionString { get; private set; }
        public static IDataApi DataApi { get; private set; }
        public static IConfigApi ConfigApi { get; private set; }
        public static IParseApi ParseApi { get; private set; }
        public static IFilesApi FilesApi { get; private set; }
        public static IAdminApi AdminApi { get; private set; }

        public static Dao Dao { get; private set; }
        public static RecordDao RecordDao { get; private set; }

        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public static ConfigInfo GetConfigInfo(int publishmentSystemId)
        {
            if (!ConfigInfoDict.ContainsKey(publishmentSystemId))
            {
                ConfigInfoDict[publishmentSystemId] = ConfigApi.GetConfig<ConfigInfo>(publishmentSystemId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[publishmentSystemId];
        }

        public void Startup(IContext context, IService service)
        {
            DatabaseType = context.Environment.DatabaseType;
            ConnectionString = context.Environment.ConnectionString;
            DataApi = context.DataApi;
            ConfigApi = context.ConfigApi;
            ParseApi = context.ParseApi;
            FilesApi = context.FilesApi;
            AdminApi = context.AdminApi;

            Dao = new Dao(ConnectionString, DataApi);
            RecordDao = new RecordDao(ConnectionString, DataApi);

            service
                .AddDatabaseTable(RecordDao.TableName, RecordDao.Columns)
                .AddSiteMenu(siteId => new Menu
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
                })
                .AddStlElementParser(StlPayment.ElementName, StlPayment.Parse)
                .AddJsonPost((request, name) =>
                {
                    if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiGet)))
                    {
                        return StlPayment.ApiGet(request);
                    }
                    if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiPay)))
                    {
                        return StlPayment.ApiPay(request);
                    }
                    if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiPaySuccess)))
                    {
                        return StlPayment.ApiPaySuccess(request);
                    }
                    if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiWeixinInterval)))
                    {
                        return StlPayment.ApiWeixinInterval(request);
                    }

                    return null;
                })
                .AddHttpGet((request, name) =>
                {
                    if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiQrCode)))
                    {
                        return StlPayment.ApiQrCode(request);
                    }

                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                })
                .AddHttpPost((request, name, id) =>
                {
                    if (Utils.EqualsIgnoreCase(name, nameof(StlPayment.ApiWeixinNotify)))
                    {
                        return StlPayment.ApiWeixinNotify(request, id);
                    }

                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                });
        }
    }
}