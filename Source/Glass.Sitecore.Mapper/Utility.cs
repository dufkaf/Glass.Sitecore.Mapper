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
using Sitecore.Data.Items;
using System.Collections;
using Glass.Sitecore.Mapper.Proxies;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Linq.Expressions;
using Sitecore.Links;
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.Data;
using System.IO;
using System.Web;

namespace Glass.Sitecore.Mapper
{
    public static class Utility
    {
        public static Type GetGenericArgument(Type type)
        {
            Type[] types = type.GetGenericArguments();
            if (types.Count() > 1)
                throw new MapperException("Type {0} has more than one generic argument".Formatted(type.FullName));
            if (types.Count() == 0)
                throw new MapperException("The type {0} does not contain any generic arguments".Formatted(type.FullName));
            return types[0];
        }

        public static Type GetGenericOuter(Type type)
        {
            return type.GetGenericTypeDefinition();
        }

        /// <summary>
        /// Will call the add method on a list via reflection to add the items
        /// </summary>
        /// <param name="items">Items to add to the list</param>
        /// <param name="list">The list to call the add method on</param>
        public static void CallAddMethod(IEnumerable<object> items, object list)
        {
            MethodInfo addMethod = list.GetType().GetMethod("Add");

            items.ForEach(x =>
                              {
                                  addMethod.Invoke(list, new object[] {x});
                              });
        }

        public static object InvokeMethod(string methodName, object target, params object[] parameters)
        {
            MethodInfo method = target.GetType().GetMethod(methodName);

            return method.Invoke(target, parameters);
        }



        /// <summary>
        /// Creates a generic type via reflection
        /// </summary>
        /// <param name="type">The generic type to create e.g. List&lt;&gt;</param>
        /// <param name="arguments">The list of subtypes for the generic type, e.g string in List&lt;string&gt;</param>
        /// <returns></returns>
        public static object CreateGenericType(Type type, Type[] arguments)
        {
            return CreateGenericType(type, arguments, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="type">The generic type to create e.g. List&lt;&gt;</param>
        /// <param name="arguments">The list of subtypes for the generic type, e.g string in List&lt;string&gt;</param>
        /// <param name="parameters"> List of parameters to pass to the constructor.</param>
        /// <returns></returns>
        public static object CreateGenericType(Type type, Type[] arguments, params object[] parameters)
        {
            Type genericType = type.MakeGenericType(arguments);
            object obj;
            if (parameters != null && parameters.Count() > 0)
                obj = Activator.CreateInstance(genericType, parameters);
            else
                obj = Activator.CreateInstance(genericType);
            return obj;

        }






        /// <summary>
        /// Checks if a method is a set property method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsSetMethod(MethodInfo info)
        {
            return info.IsSpecialName && info.Name.StartsWith("set_");
        }

        /// <summary>
        /// Checks if a method is a get property method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsGetMethod(MethodInfo info)
        {
            return info.IsSpecialName && info.Name.StartsWith("get_");
        }

        /// <summary>
        /// Returns a PropertyInfo based on a link expression, it will pull the first property name from the linq express.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(Type type, Expression expression)
        {
            string name = "";

            if (expression.NodeType == ExpressionType.Convert)
            {
                Expression operand = (expression as UnaryExpression).Operand;
                name = operand.CastTo<MemberExpression>().Member.Name;

            }
            else if (expression.NodeType == ExpressionType.Call)
            {
                name = expression.CastTo<MethodCallExpression>().Method.Name;
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                name = expression.CastTo<MemberExpression>().Member.Name;
            }

            PropertyInfo info = type.GetProperty(name);

            //if we don't find the property straight away then it is probably an interface
            //and we need to check all inherited interfaces.
            if (info == null)
            {
                info = GetAllProperties(type).FirstOrDefault(x => x.Name == name);
            }

            return info;
        }

        /// <summary>
        /// Gets all properties on a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetAllProperties(Type type)
        {
            List<Type> typeList = new List<Type>();
            typeList.Add(type);

            if (type.IsInterface)
            {
                typeList.AddRange(type.GetInterfaces());
            }

            List<PropertyInfo> propertyList = new List<PropertyInfo>();

            foreach (Type interfaceType in typeList)
            {
                foreach (PropertyInfo property in interfaceType.GetProperties(Flags))
                {
                    propertyList.Add(property);
                }
            }

            return propertyList.ToArray();
        }

        public static BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy |
                                           BindingFlags.Instance;

        public static PropertyInfo GetProperty(Type type, string name)
        {
            return type.GetProperty(name, Flags);
        }


        /// <summary>
        /// Converts a NameValueCollection in to HTML attributes
        /// </summary>
        /// <param name="attributes">A list of atrributes to convert</param>
        public static string ConvertAttributes(NameValueCollection attributes)
        {

            if (attributes == null || attributes.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            foreach (var key in attributes.AllKeys)
            {
                sb.AppendFormat("{0}='{1}' ".Formatted(key, attributes[key] ?? ""));
            }

            return sb.ToString();
        }


        public static UrlOptions CreateUrlOptions(SitecoreInfoUrlOptions urlOptions)
        {
            UrlOptions defaultUrl = UrlOptions.DefaultOptions;

            if (urlOptions == 0) return defaultUrl;

            //check for any default overrides
            defaultUrl.AddAspxExtension = (urlOptions & SitecoreInfoUrlOptions.AddAspxExtension) ==
                                          SitecoreInfoUrlOptions.AddAspxExtension
                                              ? true
                                              : defaultUrl.AddAspxExtension;
            defaultUrl.AlwaysIncludeServerUrl = (urlOptions & SitecoreInfoUrlOptions.AlwaysIncludeServerUrl) ==
                                                SitecoreInfoUrlOptions.AlwaysIncludeServerUrl
                                                    ? true
                                                    : defaultUrl.AlwaysIncludeServerUrl;
            defaultUrl.EncodeNames = (urlOptions & SitecoreInfoUrlOptions.EncodeNames) ==
                                     SitecoreInfoUrlOptions.EncodeNames
                                         ? true
                                         : defaultUrl.EncodeNames;
            defaultUrl.ShortenUrls = (urlOptions & SitecoreInfoUrlOptions.ShortenUrls) ==
                                     SitecoreInfoUrlOptions.ShortenUrls
                                         ? true
                                         : defaultUrl.ShortenUrls;
            defaultUrl.SiteResolving = (urlOptions & SitecoreInfoUrlOptions.SiteResolving) ==
                                       SitecoreInfoUrlOptions.SiteResolving
                                           ? true
                                           : defaultUrl.SiteResolving;
            defaultUrl.UseDisplayName = (urlOptions & SitecoreInfoUrlOptions.UseUseDisplayName) ==
                                        SitecoreInfoUrlOptions.UseUseDisplayName
                                            ? true
                                            : defaultUrl.UseDisplayName;


            if ((urlOptions & SitecoreInfoUrlOptions.LanguageEmbeddingAlways) ==
                SitecoreInfoUrlOptions.LanguageEmbeddingAlways)
                defaultUrl.LanguageEmbedding = LanguageEmbedding.Always;
            else if ((urlOptions & SitecoreInfoUrlOptions.LanguageEmbeddingAsNeeded) ==
                     SitecoreInfoUrlOptions.LanguageEmbeddingAsNeeded)
                defaultUrl.LanguageEmbedding = LanguageEmbedding.AsNeeded;
            else if ((urlOptions & SitecoreInfoUrlOptions.LanguageEmbeddingNever) ==
                     SitecoreInfoUrlOptions.LanguageEmbeddingNever)
                defaultUrl.LanguageEmbedding = LanguageEmbedding.Never;

            if ((urlOptions & SitecoreInfoUrlOptions.LanguageLocationFilePath) ==
                SitecoreInfoUrlOptions.LanguageLocationFilePath)
                defaultUrl.LanguageLocation = LanguageLocation.FilePath;
            else if ((urlOptions & SitecoreInfoUrlOptions.LanguageLocationQueryString) ==
                     SitecoreInfoUrlOptions.LanguageLocationQueryString)
                defaultUrl.LanguageLocation = LanguageLocation.QueryString;

            return defaultUrl;

        }

        public static string ConstructQueryString(NameValueCollection parameters)
        {
            var sb = new StringBuilder();

            foreach (String name in parameters)
                sb.Append(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name]), "&"));

            if (sb.Length > 0)
                return sb.ToString(0, sb.Length - 1);

            return String.Empty;
        }

        /// <summary>
        /// Gets an embedded manifest resource in the assembly
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Stream GetResource(string name)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            return stream;
            //TextReader reader =   new StreamReader(stream);
            //string content = reader.ReadToEnd();
            //reader.Close();
            //reader.Dispose();
            //return content;
        }

        public static void WriteToResponse(HttpResponse response, Stream content)
        {
            byte[] data = new byte[content.Length];
            content.Read(data, 0, data.Length);

            response.OutputStream.Write(data, 0, data.Length);
        }

        public static bool GuidTryParse(string input, out Guid output)
        {
#if NET40
          return Guid.TryParse(input, out output);
#else
            try
            {
                output = new Guid(input);
                return true;
            }
            catch (Exception ex)
            {
                output = Guid.Empty;
                return false;
            }
#endif
        }
    

    public static IEnumerable<AbstractSitecoreDataHandler> GetDefaultDataHanlders()
        {

            //load default handlers
           return new List<AbstractSitecoreDataHandler>(){
                new SitecoreChildrenHandler(),
                new SitecoreFieldBooleanHandler(),
                new SitecoreFieldClassHandler(),
                new SitecoreFieldDateTimeHandler(),
                new SitecoreFieldDecimalHandler(),
                new SitecoreFieldDoubleHandler(),
                new SitecoreFieldEnumHandler(),
                new SitecoreFieldFileHandler(),
                new SitecoreFieldFloatHandler(),
                new SitecoreFieldGuidHandler(),
                new SitecoreFieldIEnumerableHandler(),
                new SitecoreFieldImageHandler(),
                new SitecoreFieldIntegerHandler(),
                new SitecoreFieldLongHandler(),
                new SitecoreFieldLinkHandler(),
                new SitecoreFieldNameValueCollectionHandler(),
                new SitecoreFieldStreamHandler(),
                new SitecoreFieldStringHandler(),
                new SitecoreFieldTriStateHandler(),
                new SitecoreFieldRulesHandler(),
                new SitecoreIdDataHandler(),
                new SitecoreInfoHandler(),
                new SitecoreParentHandler(),
                new SitecoreQueryHandler(),
                new SitecoreItemHandler(),
                new SitecoreLinkedHandler(),
                new SitecoreFieldNullableHandler<DateTime?, SitecoreFieldDateTimeHandler>(),
                new SitecoreFieldNullableHandler<decimal?, SitecoreFieldDecimalHandler>(),
                new SitecoreFieldNullableHandler<double?, SitecoreFieldDoubleHandler>(),
                new SitecoreFieldNullableHandler<float?, SitecoreFieldFloatHandler>(),
                new SitecoreFieldNullableHandler<Guid?, SitecoreFieldGuidHandler>(),
                new SitecoreFieldNullableHandler<int?, SitecoreFieldIntegerHandler>()
            };
        }

        
    }
}
