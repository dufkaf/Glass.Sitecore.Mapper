using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.Proxies;
using System.Reflection;

namespace Glass.Sitecore.Mapper
{
    public class ClassManager
    {

        public ClassManager()
        {
            TypeInferrer = new DefaultTypeInferrer();
        }

        public DefaultTypeInferrer TypeInferrer { get; set; }

        public virtual object CreateClass(ISitecoreService service, bool isLazy, bool inferType, Type type, Item item, params object[] constructorParameters)
        {
            //check there is an item to create a class from
            if (item == null) return null;
            //check that there are some constructor arguments 

            SitecoreClassConfig config = null;

            if (!inferType)
            {
                //this retrieves the class based on return type
                config = Context.StaticContext.GetSitecoreClass(type);
            }
            else
            {
                //this retrieves the class by inferring the type from the template ID
                //if ths return type can not be found then the system will try to create a type 
                //base on the return type
                config = TypeInferrer.InferrerType(item, type);

            }

            //if the class should be lazy loaded or is an interface then load using a proxy
            if (isLazy || config.Type.IsInterface)
            {
                return ProxyGenerator.CreateProxy(config, service, item, inferType);
            }
            else
            {

                ConstructorInfo constructor = config.Type.GetConstructor(constructorParameters == null || constructorParameters.Count() == 0 ? Type.EmptyTypes : constructorParameters.Select(x => x.GetType()).ToArray());

                if (constructor == null) throw new MapperException("No constructor for class {0} with parameters {1}".Formatted(config.Type.FullName, string.Join(",", constructorParameters.Select(x => x.GetType().FullName).ToArray())));

                Delegate conMethod = config.CreateObjectMethods[constructor];
                object t = conMethod.DynamicInvoke(constructorParameters);
                ReadFromItem(service, t, item, config);
                return t;
            }
        }
        public  void ReadFromItem(ISitecoreService service, object target, Item item, SitecoreClassConfig config)
        {
            foreach (var handler in config.DataHandlers)
            {
                handler.SetProperty(target, item, service);
            }
        }
    }
}
