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
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using System.Reflection;

namespace Glass.Sitecore.Mapper.Data
{
    public abstract class AbstractSitecoreDataHandler : ICloneable
    {
        /// <summary>
        /// Sets the property on the target object using data from the Sitecore Item
        /// </summary>
        /// <param name="target">The object to set the value on</param>
        /// <param name="item">The item to pull the data from</param>
        /// <param name="context">The current instance context</param>
        public void SetProperty(object target, Item item, ISitecoreService service)
        {
            object value = null;

            value = this.GetValue(item, service);
            if(value != null)
                Property.SetValue(target, value, null);
        }
        /// <summary>
        /// Reads a property from an object and sets the value on the Sitecore Item
        /// </summary>
        /// <param name="target">The object to read the value from</param>
        /// <param name="item">The item to write the value to</param>
        /// <param name="context">The instance context operating on the target</param>
        public void ReadProperty(object target, Item item, ISitecoreService service)
        {
            
            object value = Property.GetValue(target, null);
            this.SetValue(item, value, service);
        }


        /// <summary>
        /// The class property the handler is setting
        /// </summary>
        public PropertyInfo Property
        {
            get;
            set;
        }

        /// <summary>
        /// Reads settings from the property and sets up the data handler.
        /// </summary>
        public virtual void ConfigureDataHandler(SitecoreProperty scProperty)
        {
            Property = scProperty.Property;
        }

        /// <summary>
        /// Determines if the Data Handler will handle the passed in Property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="datas"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        public abstract bool WillHandle(SitecoreProperty property, IEnumerable<AbstractSitecoreDataHandler> datas, Dictionary<Type, SitecoreClassConfig> classes);

        /// <summary>
        /// Reads an item from the Sitecore and returns it as the requested return type
        /// </summary>
        /// <param name="target">The object that the value will be set on</param>
        /// <param name="item">The Sitecore item to work on</param>
        /// <param name="context">The context used to load the target object</param>
        /// <returns></returns>
        public abstract object GetValue(Item item, ISitecoreService service);

        /// <summary>
        /// Sets a value on the Sitecore item
        /// </summary>
        /// <param name="target">The object that the value was read from</param>
        /// <param name="item">The Sitecore item to write the value to</param>
        /// <param name="value">The value to be set</param>
        /// <param name="context">The context used to load the target object</param>
        public abstract void SetValue(Item item, object value, ISitecoreService service);

        /// <summary>
        /// Indicates the data handler can be used to set a value on a Sitecore item.
        /// </summary>
        public abstract bool CanSetValue { get; }


        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

       

        
    }
}
