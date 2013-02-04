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
using System.Collections;
using Glass.Sitecore.Mapper.Proxies;

namespace Glass.Sitecore.Mapper
{
    public class Enumerable<T> : IEnumerable<T> where T : class
    {
        private Func<IEnumerable<Item>> _getItems;
        private ISitecoreService _service;
        
#if NET40
        private Lazy<IList<T>> _lazyItemList;
#else
        private IList<T> _lazyItemList;
        bool _loaded = false;
#endif

        private bool _isLazy = true;
        private bool _inferType = false;

        public Enumerable(Func<IEnumerable<Item>> getItems, ISitecoreService service, bool isLazy, bool inferType)
        {
            _getItems = getItems;
            _service = service;
            _isLazy = isLazy;
            _inferType = inferType;

#if NET40
            _lazyItemList = new Lazy<IList<T>>(LoadItems);
#endif
        }



        private IList<T> LoadItems()
        {

            Type type = typeof (T);
            var itemList = Utility.CreateGenericType(typeof (List<>), new Type[] {type}) as IList<T>;

            if (_getItems == null) throw new NullReferenceException("No function to return items");

            var items = _getItems.Invoke();

            if (items != null)
            {
                foreach (Item item in items.Where(x => x != null))
                {
                    var result = _service.CreateClass<T>(_isLazy, _inferType, item);
                    itemList.Add(result);
                }
            }

            return itemList;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
#if NET40
            return _lazyItemList.Value.GetEnumerator();
#else
            if (! _loaded) _lazyItemList = LoadItems();
            return _lazyItemList.GetEnumerator();
#endif
        }



        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}