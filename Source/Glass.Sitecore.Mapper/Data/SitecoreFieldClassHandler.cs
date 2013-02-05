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
using Glass.Sitecore.Mapper.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreFieldClassHandler:AbstractSitecoreField
    {
        public bool IsLazy { get; set; }
        public bool InferType { get; set; }


        public override bool WillHandle(Glass.Sitecore.Mapper.Configuration.SitecoreProperty property, IEnumerable<AbstractSitecoreDataHandler> datas, Dictionary<Type, SitecoreClassConfig> classes)
        {
            if (!(property.Attribute is SitecoreFieldAttribute)) return false;
            return classes.ContainsKey(property.Property.PropertyType);
        }



        public override object GetFieldValue(string fieldValue, Item item, ISitecoreService service)
        {
            Item target = null;

            if (fieldValue.IsNullOrEmpty()) return null;

            Guid id = Guid.Empty;
            if (Utility.GuidTryParse(fieldValue, out id)) {
                target = item.Database.GetItem(new ID(id), item.Language);
            }
            else
            {
                target = item.Database.GetItem(fieldValue, item.Language);
            }

            if (target == null) return null;
            return service.CreateClass(IsLazy, InferType, Property.PropertyType, target);

        }

        public override string SetFieldValue(object value, ISitecoreService service)
        {
            if (value == null) return "";
            return service.InstanceContext.GetClassId(Property.PropertyType, value).ToString("B").ToUpper();
        }

        public override Type TypeHandled
        {
            get { throw new NotImplementedException(); }
        }
        public override void ConfigureDataHandler(SitecoreProperty scProperty)
        {
     
            base.ConfigureDataHandler(scProperty);
            IsLazy = (Setting & SitecoreFieldSettings.DontLoadLazily) != SitecoreFieldSettings.DontLoadLazily;
            InferType = (Setting & SitecoreFieldSettings.InferType) == SitecoreFieldSettings.InferType;
        }
      
        
    }
}
