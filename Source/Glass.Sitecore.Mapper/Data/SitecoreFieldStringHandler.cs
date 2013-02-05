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
using Sitecore.Web.UI.WebControls;
using Glass.Sitecore.Mapper.Configuration;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Configuration.Attributes;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreFieldStringHandler: AbstractSitecoreField
    {

        public override object GetValue(global::Sitecore.Data.Items.Item item, ISitecoreService service)
        {
            var field = base.GetField(item);

            if (field == null)
                return string.Empty;

            if (field.Type.StartsWith("Rich Text") && Setting != SitecoreFieldSettings.RichTextRaw)
            {
                FieldRenderer renderer = new FieldRenderer();
                renderer.Item = item;
                renderer.FieldName = field.Name;
                renderer.Parameters = string.Empty;
                return renderer.Render();
            }
            else return field.Value ;
        }



        public override void SetValue( Item item, object value, ISitecoreService service)
        {
            var field = base.GetField(item);

            if (field == null)
            {
                return;
            }
            else if (field.Type.StartsWith("Rich Text") && Setting != SitecoreFieldSettings.RichTextRaw)
            {
                throw new NotSupportedException("It is not possible to save data from a rich text field when the data isn't raw."
                    + "Set the SitecoreFieldAttribute setting property to SitecoreFieldSettings.RichTextRaw for property {0} on type {1}".Formatted(Property.Name, Property.ReflectedType.FullName));
            }
            else
            {
                if (value != null)
                {
                    string fieldValue = value.ToString();
                    field.Value = fieldValue;
                }
                else
                {
                    field.Value = string.Empty;
                }
            }
        }

        public override string SetFieldValue(object value, ISitecoreService service)
        {
            throw new NotImplementedException();
        }
        public override object GetFieldValue(string fieldValue, Item item, ISitecoreService service)
        {
            throw new NotImplementedException();
        }
        public override Type TypeHandled
        {
            get { return typeof(System.String); }
        }
    }
}
