using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Sitecore.Rules;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreFieldRulesHandler : AbstractSitecoreField
    {
        public override bool CanSetValue
        {
            get
            {
                return false;
            }
        }

        public override bool WillHandle(Configuration.SitecoreProperty property, IEnumerable<AbstractSitecoreDataHandler> datas, Dictionary<Type, Configuration.SitecoreClassConfig> classes)
        {
            if (!(property.Attribute is SitecoreFieldAttribute))
            {
                return false;
            }
            var type = property.Property.PropertyType;
            if (!type.IsGenericType)
            {
                return false;
            }
            var baseType = type.GetGenericTypeDefinition();
            if (baseType == typeof(RuleList<>))
            {
                return true;
            }
            return false;
        }

        public override object GetValue(global::Sitecore.Data.Items.Item item, ISitecoreService service)
        {
            Type ruleFactory = typeof (RuleFactory);
            var method = ruleFactory.GetMethod("GetRules",
                                                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                                                null,
                                                new Type[] { typeof(global::Sitecore.Data.Fields.Field) },
                                                null);
            Type[] genericArgs = this.Property.PropertyType.GetGenericArguments();
            method = method.MakeGenericMethod(genericArgs);
            object rules = method.Invoke(null, new object[] { this.GetField(item) });
            return rules;
        }

        public override void SetValue(global::Sitecore.Data.Items.Item item, object value, ISitecoreService service)
        {
            throw new NotImplementedException();
        }

        public override object GetFieldValue(string fieldValue, global::Sitecore.Data.Items.Item item, ISitecoreService service)
        {
            throw new NotImplementedException();
        }

        public override string SetFieldValue(object value, ISitecoreService service)
        {
            throw new NotImplementedException();
        }

        public override Type TypeHandled
        {
            get { throw new NotImplementedException(); }
        }
    }
}
