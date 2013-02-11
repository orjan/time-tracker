using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracker.Indexes;

namespace TimeTracker.Controllers
{
    public class StatusController : DocumentController
    {
        //
        // GET: /Status/

        public ActionResult Index()
        {
            IOrderedQueryable<TotalWorkByUserAndDay.Result> logs =
                DocumentSession.Query<TotalWorkByUserAndDay.Result, TotalWorkByUserAndDay>()
                               .Where(x => x.UserId == Principal.Id)
                               .OrderByDescending(x => x.Date);

            return View(logs);
        }

    }
}
