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
using Glass.Sitecore.Mapper.FieldTypes;
using Sitecore.Data.Fields;
using Sitecore.Data;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Configuration;
using Sitecore.Links;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreFieldLinkHandler : AbstractSitecoreField
    {

        public override object GetValue(global::Sitecore.Data.Items.Item item,  ISitecoreService service)
        {

            var itemField = base.GetField(item);

            if (itemField == null || itemField.Value.Trim().IsNullOrEmpty()) return null;
            
            

            Link link = new Link();
            LinkField field = new LinkField(itemField);

            switch (field.LinkType)
            {
                case "anchor":
                    link.Url = field.Anchor;
                    link.Type = LinkType.Anchor;
                    break;
                case "external":
                    link.Url = field.Url;
                    link.Type = LinkType.External;
                    break;
                case "mailto":
                    link.Url = field.Url;
                    link.Type = LinkType.MailTo;
                    break;
                case "javascript":
                    link.Url = field.Url;
                    link.Type = LinkType.JavaScript;
                    break;
                case "media":
                    if (field.TargetItem == null) 
                        link.Url = string.Empty;
                    else
                    {
                        global::Sitecore.Data.Items.MediaItem media =
                            new global::Sitecore.Data.Items.MediaItem(field.TargetItem);
                        link.Url = global::Sitecore.Resources.Media.MediaManager.GetMediaUrl(media);
                        link.Type = LinkType.Media;
                        link.TargetId = field.TargetID.Guid;
                    }
                    break;
                case "internal":
                    if (field.TargetItem == null) link.Url = string.Empty;
                    else link.Url = LinkManager.GetItemUrl(field.TargetItem);
                    link.Type = LinkType.Internal;
                    link.TargetId = field.TargetID.Guid;

                    break;
                default:
                    return null;
                    break;

            }
            

            link.Anchor = field.Anchor;
            link.Class = field.Class;
            link.Text = field.Text;
            link.Title = field.Title;
            link.Target = field.Target;
            link.Query = field.QueryString;

            return link;
        }

        public override void SetValue(global::Sitecore.Data.Items.Item item, object value, ISitecoreService service)
        {

            Link link = value as Link;

            var itemField = base.GetField(item);

            if (itemField == null) return;

            LinkField field = new LinkField(itemField);
            if (link == null || link.Type == LinkType.NotSet)
            {
                field.Clear();
                return;
            }


            switch (link.Type)
            {
                case LinkType.Internal:
                    field.LinkType = "internal";
                    if (field.TargetID.Guid != link.TargetId)
                    {
                        if (link.TargetId == Guid.Empty)
                        {
                            ItemLink iLink = new ItemLink(item.Database.Name, item.ID, field.InnerField.ID, field.TargetItem.Database.Name, field.TargetID, field.TargetItem.Paths.FullPath);
                            field.RemoveLink(iLink);
                        }
                        else
                        {
                            ID newId = new ID(link.TargetId);
                            Item target = item.Database.GetItem(newId);
                            if (target != null)
                            {
                                field.TargetID = newId;
                                ItemLink nLink = new ItemLink(item.Database.Name, item.ID, field.InnerField.ID, target.Database.Name, target.ID, target.Paths.FullPath);
                                field.UpdateLink(nLink);
                                field.Url = LinkManager.GetItemUrl(target);
                            }
                            else throw new MapperException("No item with ID {0}. Can not update Link field".Formatted(newId));
                        }

                    }
                    break;
                case LinkType.Media:
                    field.LinkType = "media";
                    if (field.TargetID.Guid != link.TargetId)
                    {
                        if (link.TargetId == Guid.Empty)
                        {
                            ItemLink iLink = new ItemLink(item.Database.Name, item.ID, field.InnerField.ID, field.TargetItem.Database.Name, field.TargetID, field.TargetItem.Paths.FullPath);
                            field.RemoveLink(iLink);
                        }
                        else
                        {
                            ID newId = new ID(link.TargetId);
                            Item target = item.Database.GetItem(newId);
                            global::Sitecore.Data.Items.MediaItem media = new global::Sitecore.Data.Items.MediaItem(target);

                            if (target != null)
                            {
                                field.TargetID = newId;
                                ItemLink nLink = new ItemLink(item.Database.Name, item.ID, field.InnerField.ID, target.Database.Name, target.ID, target.Paths.FullPath);
                                field.UpdateLink(nLink);
                                field.Url = global::Sitecore.Resources.Media.MediaManager.GetMediaUrl(media);
                            }
                            else throw new MapperException("No item with ID {0}. Can not update Link field".Formatted(newId));
                        }

                    }
                    break;
                case LinkType.External:
                    field.LinkType = "external";
                    field.Url = link.Url;
                    break;
                case LinkType.Anchor:
                    field.LinkType = "anchor";
                    field.Url = link.Anchor;
                    break;
                case LinkType.MailTo:
                    field.LinkType = "mailto";
                    field.Url = link.Url;
                    break;
                case LinkType.JavaScript:
                    field.LinkType = "javascript";
                    field.Url = link.Url;
                    break;


            }



            if (!link.Anchor.IsNullOrEmpty())
                field.Anchor = link.Anchor;
            if (!link.Class.IsNullOrEmpty())
                field.Class = link.Class;
            if (!link.Text.IsNullOrEmpty())
                field.Text = link.Text;
            if (!link.Title.IsNullOrEmpty())
                field.Title = link.Title;
            if (!link.Query.IsNullOrEmpty())
                field.QueryString = link.Query;
            if (!link.Target.IsNullOrEmpty())
                field.Target = link.Target;


        }

        public override string SetFieldValue(object value,  ISitecoreService service)
        {
            throw new NotImplementedException();
        }

        public override Type TypeHandled
        {
            get { return typeof(FieldTypes.Link); }
        }

        public override object GetFieldValue(string fieldValue, Item item, ISitecoreService service)
        {
            throw new NotImplementedException();
        }
    }
}
