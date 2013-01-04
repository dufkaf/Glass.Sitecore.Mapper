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
using System.Web;
using Glass.Sitecore.Mapper.FieldTypes;
using Sitecore.Data.Fields;
using Sitecore.Links;
using Sitecore.Data;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Configuration;
using Sitecore.Resources.Media;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreFieldFileHandler : AbstractSitecoreField
    {

        public override object GetValue(global::Sitecore.Data.Items.Item item, ISitecoreService service)
        {

            var itemField = base.GetField(item);

            if (itemField == null) return null;

            FileField field = new FileField(itemField);
            File file = new File();
            if (field.MediaItem != null)
            {
                MediaItem mediaItem = new MediaItem(field.MediaItem);

                file.Src = HttpUtility.UrlPathEncode(MediaManager.GetMediaUrl(field.MediaItem));
                file.Size = mediaItem.Size;
                file.Title = mediaItem.Title;
                file.Extension = mediaItem.Extension;
            }
            file.Id = field.MediaID.Guid;

            return file;
        }

        public override void SetValue(global::Sitecore.Data.Items.Item item, object value, ISitecoreService service)
        {

            File  file = value as File;

            Field itemField = base.GetField(item);

            if (itemField == null) return;

            FileField field = new FileField(item.Fields[FieldName]);

            if (file == null)
            {
                field.Clear();
                return;
            }

            if (field.MediaID.Guid != file.Id)
            {
                if (file.Id == Guid.Empty)
                {
                    ItemLink link = new ItemLink(item.Database.Name, item.ID, field.InnerField.ID, field.MediaItem.Database.Name, field.MediaID, field.MediaItem.Paths.FullPath);
                    field.RemoveLink(link);
                }
                else
                {
                    ID newId = new ID(file.Id);
                    Item target = item.Database.GetItem(newId);
                    if (target != null)
                    {
                        field.MediaID = newId;
                        ItemLink link = new ItemLink(item.Database.Name, item.ID, field.InnerField.ID, target.Database.Name, target.ID, target.Paths.FullPath);
                        field.UpdateLink(link);
                    }
                    else throw new MapperException("No item with ID {0}. Can not update File Item field".Formatted(newId));
                }
            }

        }

        public override object GetFieldValue(string fieldValue, global::Sitecore.Data.Items.Item item,  ISitecoreService service)
        {
            throw new NotImplementedException();
        }

        public override string SetFieldValue(object value,ISitecoreService service)
        {
            throw new NotImplementedException();
        }

        public override Type TypeHandled
        {
            get { return typeof(File); }
        }
    }
}
