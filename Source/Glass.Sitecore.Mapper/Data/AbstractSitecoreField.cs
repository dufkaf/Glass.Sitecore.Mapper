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
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;

namespace Glass.Sitecore.Mapper.Data
{
    public abstract class AbstractSitecoreField : AbstractSitecoreDataHandler
    {

        public ID FieldId { get; set; }

        public string FieldName { get; set; }

        protected bool ReadOnly { get; set; }

        protected SitecoreFieldSettings Setting { get; set; }

        public abstract object GetFieldValue(string fieldValue, Item item, ISitecoreService service);


        public abstract string SetFieldValue(object value, ISitecoreService service);


        protected Field GetField(Item item)
        {
            Field field = null;
            if (ID.IsNullOrEmpty(FieldId))
            {
                field =  item.Fields[FieldName];
            }

            else if(item.Fields.Contains(FieldId) || item.Template.GetField(FieldId) != null)

            {
                field = item.Fields[FieldId];
                FieldName = field.Name;
            }
         

            return field;
        }

        public override void SetValue(Item item, object value, ISitecoreService service)
        {
            var field = GetField(item);
            if (field == null) return;

            string fieldValue = SetFieldValue(value,  service);

            field.Value = fieldValue;
            
        }

        public override  object GetValue(Item item, ISitecoreService service)
        {
            var field = GetField(item);

            string fieldValue = field == null ? string.Empty : field.Value ;

            return GetFieldValue(fieldValue, item, service);
        }

        public override  bool WillHandle(Glass.Sitecore.Mapper.Configuration.SitecoreProperty property, IEnumerable<AbstractSitecoreDataHandler> datas, Dictionary<Type, SitecoreClassConfig> classes)
        {
            return property.Attribute is SitecoreFieldAttribute && property.Property.PropertyType == TypeHandled;
        }

        public override bool CanSetValue
        {
            get
            {
                return !ReadOnly;
            }
        }


        public abstract Type TypeHandled { get; }

        public override void ConfigureDataHandler(SitecoreProperty scProperty)
        {
            SitecoreFieldAttribute attr = scProperty.Attribute as SitecoreFieldAttribute;
            
            if (attr != null && !attr.FieldName.IsNullOrEmpty()) FieldName = attr.FieldName;
            else FieldName = scProperty.Property.Name;

            ReadOnly = attr.ReadOnly;

            Setting = attr.Setting;

            if (attr.FieldId.IsNotNullOrEmpty())
            {
                Guid id = Guid.Empty;

                if (Utility.GuidTryParse(attr.FieldId, out id)) {
                    FieldId = new ID(id);
                }
                else
                    throw new MapperException("The field Id {0} on property {1} in class {2} isn't in the correct format".Formatted(
                        attr.FieldId, scProperty.Property.Name, scProperty.Property.ReflectedType.FullName));


            }
            
            base.ConfigureDataHandler(scProperty);
        }
     

    }
}
