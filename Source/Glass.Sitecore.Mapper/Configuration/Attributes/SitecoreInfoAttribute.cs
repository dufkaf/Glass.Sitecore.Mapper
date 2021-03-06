﻿/*
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

namespace Glass.Sitecore.Mapper.Configuration.Attributes
{
    /// <summary>
    /// Used to map item information to a class property
    /// </summary>
    public class SitecoreInfoAttribute:AbstractSitecorePropertyAttribute
    {
        /// <summary>
        /// The type of information that should populate the property
        /// </summary>
        public SitecoreInfoType Type{get;set;}


        /// <summary>
        /// UrlOptions, use in conjunction with SitecoreInfoType.Url
        /// </summary>
        public SitecoreInfoUrlOptions UrlOptions { get; set; }


        
        /// <summary>
        /// Indicates that the property should be populated with data about the item such as URL, ID, etc.
        /// </summary>
        /// <param name="type">The type of information that should populate the property</param>
        public SitecoreInfoAttribute(SitecoreInfoType type)
        {
            Type = type;
        }

        internal SitecoreInfoAttribute()
        {
            Type = SitecoreInfoType.NotSet;
        }
    }
}
