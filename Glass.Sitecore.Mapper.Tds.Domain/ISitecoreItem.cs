using System;
using System.Collections.Generic;
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Sitecore.Globalization;

namespace Glass.Demo.Application.Models
{
    [SitecoreClass]
    public interface ISitecoreItem {
		
        [SitecoreId]
        Guid Id{ get; }

        [SitecoreInfo(SitecoreInfoType.Name)]
        string Name { get; }

        [SitecoreInfo(SitecoreInfoType.Path)]
        string Path { get; }

        [SitecoreInfo(SitecoreInfoType.Language)]
        Language Language{ get; }

        [SitecoreInfo(SitecoreInfoType.Version)]
        int Version { get; }

        [SitecoreInfo(SitecoreInfoType.TemplateId)]
        Guid TemplateId { get; }

        [SitecoreParent(IsLazy = true, InferType = true)]
        ISitecoreItem Parent { get; }

        [SitecoreChildren(IsLazy = true, InferType = true)]
        IEnumerable<ISitecoreItem> Children { get; }
    }
}