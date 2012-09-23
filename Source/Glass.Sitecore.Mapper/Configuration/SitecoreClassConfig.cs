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
using Glass.Sitecore.Mapper.Data;
using System.Reflection;

namespace Glass.Sitecore.Mapper.Configuration
{
    public class SitecoreClassConfig 
    {
        public SitecoreClassConfig()
        {
            CreateObjectMethods = new Dictionary<ConstructorInfo, Delegate>();
        }

        public Type Type { get; set; }
        public Guid TemplateId { get; set; }
        public Guid BranchId { get; set; }

        public  IEnumerable<AbstractSitecoreDataHandler> DataHandlers { get; set; }
        internal Dictionary<ConstructorInfo, Delegate> CreateObjectMethods { get; set; }

        public IEnumerable<SitecoreProperty> Properties { get; set; }
        public SitecoreClassAttribute ClassAttribute {get;set;}
        public SitecoreProperty IdProperty { get; set; }
        public SitecoreProperty LanguageProperty { get; set; }
        public SitecoreProperty VersionProperty { get; set; }
    }
}
