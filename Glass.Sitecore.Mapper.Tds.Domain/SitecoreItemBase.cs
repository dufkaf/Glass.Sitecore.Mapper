using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.FieldTypes;
using Sitecore.Globalization;


namespace Glass.Demo.Application.Models
{
    public abstract class SitecoreItemBase : ISitecoreItem
    {
		[SitecoreId]
		public virtual Guid Id { get; protected set;}

        [SitecoreInfo(SitecoreInfoType.Name)]
        public virtual string Name { get; protected set; }

        [SitecoreInfo(SitecoreInfoType.Path)]
        public virtual string Path { get; protected set; }

		[SitecoreInfo(SitecoreInfoType.Language)]
        public virtual Language Language { get; protected set; }

        [SitecoreInfo(SitecoreInfoType.Version)]
        public virtual int Version { get; protected set; }

        [SitecoreInfo(SitecoreInfoType.TemplateId)]
        public virtual Guid TemplateId { get; protected set; }

        [SitecoreParent(IsLazy = true, InferType = true)]
        public virtual ISitecoreItem Parent { get; protected set; }

        [SitecoreChildren(IsLazy = true, InferType = true)]
        public virtual IEnumerable<ISitecoreItem> Children { get; protected set; }
    }
}
