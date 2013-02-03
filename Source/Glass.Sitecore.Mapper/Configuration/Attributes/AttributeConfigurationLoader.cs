/*
   Copyright 2011 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Glass.Sitecore.Mapper.Configuration.Attributes
{
    public class AttributeConfigurationLoader : AbstractConfigurationLoader
    {

        Guid _id;

        IEnumerable<string> _namespaces;

        /// <summary>
        /// Load types from certian namespaces only or specify just assemblies
        /// </summary>
        /// <param name="namespaces"></param>
        public AttributeConfigurationLoader(params string [] namespaces)
        {
            _namespaces = namespaces;
            _id = Guid.NewGuid();
        }

        #region AbstractConfigurationLoader Members

        public override IEnumerable<SitecoreClassConfig> Load()
        {
            IDictionary<Type, SitecoreClassConfig> classes;

            if (_namespaces == null || !_namespaces.Any())
            {
                classes = LoadAllClasses();
            }
            else
            {
                classes = LoadNamespaceClasses();
            }

            classes.ForEach(x => x.Value.Properties = GetProperties(x.Value.Type));

            return classes.Select(x => x.Value);
        }


        private IDictionary<Type, SitecoreClassConfig> LoadAllClasses()
        {
            var classes = new Dictionary<Type, SitecoreClassConfig>();

            AppDomain domain = AppDomain.CurrentDomain;
            Assembly[] assemblies = domain.GetAssemblies();

            foreach (var assem in assemblies)
            {
                try
                {
                    //don't load assemblies from the GAC
                    if (assem.GlobalAssemblyCache)
                        continue;

                    //sssem.SecurityRuleSet\

                    Func<Type, bool> namespacePredicate = (x) => true;

                    var namespaceClasses = ProcessAssembly(assem, namespacePredicate);

                    namespaceClasses.ForEach(cls =>
                    {
                        //stops duplicates being added
                        if (!classes.ContainsKey(cls.Type))
                        {
                            classes.Add(cls.Type, cls);
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw new MapperException("Could not load assembly {0}".Formatted(assem.Location), ex);
                }

            };

            return classes;
        }


        public virtual IDictionary<Type, SitecoreClassConfig> LoadNamespaceClasses()
        {
            var classes =  new Dictionary<Type, SitecoreClassConfig>();
            foreach (string space in _namespaces)
            {
                string[] parts = space.Split(',');



                string assembly = parts.Length > 1 ? parts[1] : parts[0];

                Assembly assem = Assembly.Load(assembly);

                string namesp =  parts.Length > 1 ? parts[0] : string.Empty;


                Func<Type, bool> namespacePredicate = null;

                if (namesp.IsNullOrEmpty()) namespacePredicate = (x) => true;
                else namespacePredicate = (x) => x != null && x.Namespace != null && (x.Namespace.Equals(namesp) || x.Namespace.StartsWith(namesp + "."));



                var namespaceClasses = ProcessAssembly(assem, namespacePredicate);


                namespaceClasses.ForEach(cls =>
                {
                    //stops duplicates being added
                    if (!classes.ContainsKey(cls.Type))
                    {
                        classes.Add(cls.Type, cls);
                    }
                });

            };

            return classes;
        }
      

        #endregion


        public static  IEnumerable<SitecoreProperty> GetProperties(Type type)
        {
            IEnumerable<PropertyInfo> properties = Utility.GetAllProperties(type);

            return properties.Select(x =>
            {
               return GetProperty(x);
            }).Where(x=>x != null).ToList();
            
        }

        public static SitecoreProperty GetProperty(PropertyInfo info)
        {
            //we have to get the property definition fromt he declaring type so that 
            //we can set private setters.
            var finalInfo = Utility.GetProperty(info.DeclaringType, info.Name);

            var attr = GetSitecoreAttribute(finalInfo);

            

            //if we can't get a Sitecore attribute from current property we search down the 
            // inheritence chain to find the first declared attribute.
            if (attr == null)
            {
                var interfaces = finalInfo.DeclaringType.GetInterfaces();
                string propertyName = finalInfo.Name;

                foreach (var inter in interfaces)
                {
                    finalInfo = inter.GetProperty(propertyName);
                    if (finalInfo != null)
                        attr = GetSitecoreAttribute(finalInfo);

                    if (attr != null) break;
                }

               
            }
                                                  
            if (attr != null)
            {
                return new SitecoreProperty()
                {
                    Attribute = attr,
                    Property = finalInfo
                };
            }
            else return null;
        }
        public static AbstractSitecorePropertyAttribute GetSitecoreAttribute(PropertyInfo info)
        {
            var attrs = info.GetCustomAttributes(true);
            var attr = attrs.FirstOrDefault(y => y is AbstractSitecorePropertyAttribute) as AbstractSitecorePropertyAttribute;
            return attr;
        }




        private IEnumerable<SitecoreClassConfig> ProcessAssembly(Assembly assem, Func<Type, bool> predicate)
        {

            if (assem != null)
            {
                try
                {
                    return assem.GetTypes().Select(x =>
                    {
                        if (predicate(x))
                        {
                            IEnumerable<object> attrs = x.GetCustomAttributes(true);
                            SitecoreClassAttribute  attr = attrs.FirstOrDefault(y => y is SitecoreClassAttribute) as SitecoreClassAttribute;

                            if (attr != null)
                            {
                                var config = new SitecoreClassConfig()
                                {
                                    Type = x,
                                    ClassAttribute = attr,

                                };
                                //TODO need to wrap in exception handler
                                if (!attr.BranchId.IsNullOrEmpty()) config.BranchId = new Guid(attr.BranchId);
                                if (!attr.TemplateId.IsNullOrEmpty()) config.TemplateId = new Guid(attr.TemplateId);

                                return config;
                            }
                        }
                        return null;
                    }).Where(x => x != null).ToList();

                }
                catch (Exception ex)
                {
                    //throw new MapperException("Failed to load types {0}".Formatted(ex.LoaderExceptions.First().Message), ex);
                    //don't mind assemblies that contain dynamic types...
                }
            }
            return new List<SitecoreClassConfig>();
        }



        public override Guid Id
        {
            get { return _id ; }
        }
    }
}
