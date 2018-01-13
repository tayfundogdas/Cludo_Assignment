using Business;
using Cludo_Assignment.Helpers;
using Cludo_Assignment.Models;
using Lucene.Net.Store;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cludo_Assignment.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(TwitterSearch model)
        {
            //search hashtag on twitter
            if (!string.IsNullOrWhiteSpace(model.HashTag))
            {
                var index = new RAMDirectory();
                SessionHelper.SetIndex(index);
                TwitterFlow flow = new TwitterFlow(SessionHelper.GetIndex());
                var searchResult = flow.SearchHashTagOnTwitter(model.HashTag);
                //for local testing only!
                //var searchResult = flow.SearchHashTagOnTwitter();
                model.TwitterResult = searchResult;
                flow.IndexSearchResult(searchResult);
                model.HashTag = null;
            }

            //filter search result
            if (!string.IsNullOrWhiteSpace(model.Filter) && SessionHelper.GetIndex() != null && string.IsNullOrWhiteSpace(model.HashTag))
            {
                TwitterFlow flow = new TwitterFlow(SessionHelper.GetIndex());
                var filterResult = flow.FilterSearchResult(model.Filter);
                model.TwitterResult = filterResult;
                model.Filter = null;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveSelected(List<int> selectedIds)
        {
            if (SessionHelper.GetIndex() != null)
            {
                TwitterFlow flow = new TwitterFlow(SessionHelper.GetIndex());
                return Json(new
                {
                    success = flow.SaveSearchResultToFile(selectedIds),
                },
                JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = false,
            },
JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}