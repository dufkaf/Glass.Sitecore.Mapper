using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NET40
using System.Web.Script.Serialization;
#endif
namespace Glass.Sitecore.Mapper.Dashboard.Web
{
    public class JsonView: AbstractView
    {
        object _model;

        public JsonView(object model)
        {
            _model = model;
        }

        public override void Response(System.Web.HttpResponse response)
        {
#if NET40
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var output = new StringBuilder();
            serializer.Serialize(_model, output);

            response.ContentType = "application/json";
            response.Write(output.ToString());
#else
            throw new NotImplementedException("ToDo: use JSON.NET for <4.0");
#endif
        }
    }
}
