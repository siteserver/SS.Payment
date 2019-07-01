using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Payment.Core;

namespace SS.Payment.Controllers.Pages
{
    [RoutePrefix("pages/settings")]
    public class PagesSettingsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            var request = Context.AuthenticatedRequest;
            var siteId = request.GetQueryInt("siteId");

            if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId))
            {
                return Unauthorized();
            }

            return Ok(new
            {
                Value = Main.GetConfigInfo(siteId)
            });
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId))
                {
                    return Unauthorized();
                }

                var configInfo = request.GetPostObject<ConfigInfo>();

                Main.SetConfigInfo(siteId, configInfo);

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
