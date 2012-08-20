using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Sitecore.Mapper.Dashboard.Web;
using Glass.Sitecore.Mapper.Dashboard.Model.Data;
using Glass.Sitecore.Mapper.Dashboard.Model;
using System.Collections;

namespace Glass.Sitecore.Mapper.Dashboard.Controllers
{
    public class QueryController : AbstractController
    {
        SitecoreService _service = new SitecoreService("master");

        public AbstractView Index()
        {
            return new HtmlView("Html.Query.htm");
        }
       


        public AbstractView Paths(string term)
        {
            IEnumerable<PathQuery> result = new PathQuery[0];

            term = term.ToLower();

            if (term.StartsWith("/sitecore"))
            {
                ISitecoreService service = new SitecoreService("master");

                if (term.EndsWith("/"))
                {
                    result = service.Query<PathQuery>(term + "*");
                }
                else
                {
                    var parts = term.Split('/');

                    var query = parts.Take(parts.Length - 1).Aggregate((x, y) => "{0}/{1}".Formatted(x, y));

                    result = service.Query<PathQuery>(query + "/*");

                    result = result.Where(x => x.value.ToLower().StartsWith(term));
                        
                }
            }

            return new JsonView(result);
        }

        public AbstractView Query(string cls, string path)
        {
            if (cls.IsNullOrEmpty() || path.IsNullOrEmpty()) return null;

            global::Sitecore.Context.Site = global::Sitecore.Configuration.Factory.GetSite("website");

            var types = GlassContext.Classes.Where(x => x.Key.FullName == cls);
            if (types.Any()) {
                var type = types.First();
                var item = _service.Database.GetItem(path);

                List<GlassQueryResult> results = new List<GlassQueryResult>();
                foreach (var handler in type.Value.DataHandlers)
                {
                    var value = handler.GetValue(item, _service);
                    var result = new GlassQueryResult();
                    result.Title = handler.Property.Name;
                    result.Value = GetValue(value);

                    

                    results.Add(result);
                }


                return new JsonView(results);

            
            }
            else return null;



        }

        private string GetValue(object value)
        {

            if (value == null) return string.Empty;
            Type resultType = value.GetType();

            

            if (GlassContext.Classes.ContainsKey(resultType))
            {
               return ProcessGlassType(value, resultType);
            }
                //this handles proxies
            else if(GlassContext.Classes.ContainsKey(resultType.BaseType)){
                return ProcessGlassType(value, resultType.BaseType);
            }
                //handles proxied interfaces
            else if(resultType.GetInterfaces().Any(x=>GlassContext.Classes.ContainsKey(x))) {
                var interfaceType = resultType.GetInterfaces().First(x=>GlassContext.Classes.ContainsKey(x));
                return ProcessGlassType(value, interfaceType);
            }
            else if (value is IEnumerable && resultType.IsGenericType)
            {
                var collection = value as IEnumerable;

                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("The enumeration contains {0} items of type {1}", collection.Cast<object>().Count(), resultType.GetGenericArguments().First().Name);

                if (collection.Cast<object>().Any())
                {

                    sb.AppendFormat("<ul>{0}</ul>",
                         collection
                            .Cast<object>()
                            .Select(x => "<li>{0}</li>".Formatted(GetValue(x)))
                            .Aggregate((x, y) => x + y));


                    
                }
                return sb.ToString();
            }
            else
            {
                return System.Web.HttpUtility.HtmlEncode(value.ToString());
            }



        }

        private string ProcessGlassType(object value, Type resultType)
        {
            var glassDetails = GlassContext.Classes[resultType];
            if (glassDetails.IdProperty != null)
            {
                Guid id = (Guid)glassDetails.IdProperty.Property.GetValue(value, null);
                ItemData data = _service.GetItem<ItemData>(id);

                if(data != null)
                    return "<a class='queryLink' path='{0}' cls='{1}' href='#'>{2}</a> - <a href='/details.gls?cls={1}'>{1}</a>".Formatted(data.Path, resultType.FullName, data.Path);
                else
                    return System.Web.HttpUtility.HtmlEncode(value.ToString());

            }
            else
                return System.Web.HttpUtility.HtmlEncode(value.ToString());
        }
    }
}
