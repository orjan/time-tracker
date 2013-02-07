using System.Web.Mvc;
using Raven.Client;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    public abstract class DocumentController : Controller
    {
        public IDocumentSession DocumentSession { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DocumentSession = MvcApplication.DocumentStore.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            using (DocumentSession)
            {
                if (filterContext.Exception != null)
                    return;

                if (DocumentSession != null)
                    DocumentSession.SaveChanges();
            }
        }

        public CustomPrincipal Principal
        {
            get
            {
                if (HttpContext.User is CustomPrincipal)
                    return (CustomPrincipal) HttpContext.User;
                return null;
            }
        }
    }
}