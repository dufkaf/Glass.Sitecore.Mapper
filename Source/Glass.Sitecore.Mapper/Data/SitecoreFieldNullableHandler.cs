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
using Sitecore.Data.Items;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreFieldNullableHandler<TNullable, THandler> : AbstractSitecoreField
        where THandler : AbstractSitecoreField, new()
    {
        private readonly THandler _baseHandler;

        public SitecoreFieldNullableHandler()
        {
            _baseHandler = new THandler();
        }

        public override object GetFieldValue(string fieldValue, Item item, ISitecoreService service)
        {
#if NET40
            if (string.IsNullOrWhiteSpace(fieldValue))
#else
            if (string.IsNullOrEmpty(fieldValue))
#endif
            {
                return null;
            }
            return _baseHandler.GetFieldValue(fieldValue, item, service);
        }

        public override string SetFieldValue(object value, ISitecoreService service)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return _baseHandler.SetFieldValue(value, service);
        }

        public override Type TypeHandled
        {
            get { return typeof(TNullable); }
        }

        
    }
}
