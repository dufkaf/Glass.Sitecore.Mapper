#if NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Sitecore.Mapper.Dashboard.Model;
using System.Web.Script.Serialization;
using Glass.Sitecore.Mapper.Dashboard.Web;

namespace Glass.Sitecore.Mapper.Dashboard.Controllers
{
    public class ListController : AbstractController
    {

        public AbstractView Index()
        {
            return new HtmlView("Html.List.htm");
        }

        public AbstractView Classes()
        {
            var classes = GlassContext.Classes.OrderBy(x => x.Key.FullName).Select(y => new GlassClassSummary() { Name = y.Key.FullName });
            return new JsonView(classes);
        }
    }
}
#endif