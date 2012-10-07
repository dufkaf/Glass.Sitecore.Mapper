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
using System.Globalization;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreFieldLongHandler : AbstractSitecoreField
    {
        public override object GetFieldValue(string fieldValue, Item item, ISitecoreService service)
        {
            if (fieldValue.IsNullOrEmpty()) return 0;
            long lValue = 0;
            if (long.TryParse(fieldValue, NumberStyles.Any, CultureInfo.InvariantCulture, out lValue)) return lValue;
            throw new MapperException("Could not convert value to long.");
        }

        public override string SetFieldValue(object value, ISitecoreService service)
        {
            return value.ToString();
        }

        public override Type TypeHandled
        {
            get { return typeof(long); }
        }
    }
}
