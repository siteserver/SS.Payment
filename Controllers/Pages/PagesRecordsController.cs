using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Payment.Core;

namespace SS.Payment.Controllers.Pages
{
    [RoutePrefix("pages/records")]
    public class PagesRecordsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var recordRepository = new RecordRepository();
                var siteId = request.GetQueryInt("siteId");
                var page = request.GetQueryInt("page", 1);
                const int perPage = 30;

                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId)) return Unauthorized();

                var totalCount = recordRepository.GetPayedCount(siteId);
                var pages = Convert.ToInt32(Math.Ceiling((double)totalCount / perPage));
                if (pages == 0) pages = 1;
                if (page > pages) page = pages;

                var recordInfoList = recordRepository.GetPayedRecordInfoList(siteId, page, perPage);

                return Ok(new
                {
                    Value = recordInfoList,
                    Count = totalCount,
                    Pages = pages,
                    Page = page
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var recordRepository = new RecordRepository();
                var siteId = request.GetQueryInt("siteId");
                var page = request.GetQueryInt("page", 1);
                var recordId = request.GetQueryInt("recordId", 1);
                const int perPage = 30;

                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId)) return Unauthorized();

                recordRepository.Delete(recordId);

                var totalCount = recordRepository.GetPayedCount(siteId);
                var pages = Convert.ToInt32(Math.Ceiling((double)totalCount / perPage));
                if (pages == 0) pages = 1;
                if (page > pages) page = pages;

                var recordInfoList = recordRepository.GetPayedRecordInfoList(siteId, page, perPage);

                return Ok(new
                {
                    Value = recordInfoList,
                    Count = totalCount,
                    Pages = pages,
                    Page = page
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
