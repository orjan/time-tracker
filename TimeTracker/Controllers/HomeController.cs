using System;
using System.Web.Mvc;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    public class HomeController : DocumentController
    {
        public ActionResult Index()
        {
            /*
            DocumentSession.Store(new TimeLog
                                      {
                                          StartTime = DateTime.Now,
                                          EndTime = DateTime.Now.AddHours(2)
                                      });
            */
            return Content(Principal.Id.ToString());
        }
    }
}