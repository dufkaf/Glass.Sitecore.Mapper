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
using Sitecore.Data;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Configuration;
using Sitecore.Globalization;
using System.Collections;
using Sitecore.Links;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Sitecore.SecurityModel;

namespace Glass.Sitecore.Mapper
{
    public class SitecoreService : ISitecoreService
    {
        public InstanceContext InstanceContext { get; private set; }

        /// <summary>
        /// The database used by the Sitecore service, used by some internal processed.
        /// </summary>
        public Database Database
        {
            get
            {
                return _database;
            }
        }

        global::Sitecore.Data.Database _database;
        LinkDatabase _linkDb;

        public SitecoreService(string database)
            : this(global::Sitecore.Configuration.Factory.GetDatabase(database))
        {

        }
        public SitecoreService(global::Sitecore.Data.Database database)
        {
            InstanceContext = Context.GetContext();
            _linkDb = global::Sitecore.Configuration.Factory.GetLinkDatabase();
            _database = database;
        }



        #region ISitecoreService Members

        /// <summary>
        /// Query Sitecore for a set of items. Concrete classes are created
        /// </summary>
        /// <typeparam name="T">The type to return the items as</typeparam>
        /// <param name="query">The query to execute</param>
        /// <returns>Sitecore items as concrete classes of the specified type</returns>
        public IEnumerable<T> Query<T>(string query) where T : class
        {
            return Query<T>(query, false);
        }

        /// <summary>
        /// Query Sitecore for a set of items. Proxy classes are created.
        /// </summary>
        /// <typeparam name="T">The type to return the items as</typeparam>
        /// <param name="query">The query to execute</param>
        /// <returns>Sitecore items as proxy classes of the specified type</returns>
        public IEnumerable<T> Query<T>(string query, bool isLazy) where T : class
        {
            Item[] items = _database.SelectItems(query);
            return CreateClasses(isLazy, typeof(T), () => { return items; }) as IEnumerable<T>;
        }

        /// <summary>
        /// Query Sitecore for a single item. 
        /// </summary>
        /// <typeparam name="T">The type to return the items as</typeparam>
        /// <param name="query">The query to execute</param>
        /// <returns>Sitecore item as the specified type</returns>
        public T QuerySingle<T>(string query) where T : class
        {
            Item item = _database.SelectSingleItem(query);
            return CreateClass<T>(false, false, item);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <param name="path">The path to the Sitecore item</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T>(string path) where T : class
        {
            Item item = _database.GetItem(path);
            return CreateClass<T>(false, false, item);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K>(string path, K param1) where T : class
        {
            Item item = _database.GetItem(path);
            return CreateClass<T, K>(false, false, item, param1);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L>(string path, K param1, L param2) where T : class
        {
            Item item = _database.GetItem(path);
            return CreateClass<T, K, L>(false, false, item, param1, param2);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M>(string path, K param1, L param2, M param3) where T : class
        {
            Item item = _database.GetItem(path);
            return CreateClass<T, K, L, M>(false, false, item, param1, param2, param3);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <typeparam name="N">The type of the fourth constructor parameter</typeparam>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <param name="param4">The value of the fourth parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M, N>(string path, K param1, L param2, M param3, N param4) where T : class
        {
            Item item = _database.GetItem(path);
            return CreateClass<T, K, L, M, N>(false, false, item, param1, param2, param3, param4);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T>(string path, Language language) where T : class
        {
            Item item = _database.GetItem(path, language);
            return CreateClass<T>(false, false, item);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K>(string path, Language language, K param1) where T : class
        {
            Item item = _database.GetItem(path, language);
            return CreateClass<T, K>(false, false, item, param1);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L>(string path, Language language, K param1, L param2) where T : class
        {
            Item item = _database.GetItem(path, language);
            return CreateClass<T, K, L>(false, false, item, param1, param2);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M>(string path, Language language, K param1, L param2, M param3) where T : class
        {
            Item item = _database.GetItem(path, language);
            return CreateClass<T, K, L, M>(false, false, item, param1, param2, param3);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <typeparam name="N">The type of the fourth constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <param name="param4">The value of the fourth parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M, N>(string path, Language language, K param1, L param2, M param3, N param4) where T : class
        {
            Item item = _database.GetItem(path, language);
            return CreateClass<T, K, L, M, N>(false, false, item, param1, param2, param3, param4);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T>(string path, Language language, global::Sitecore.Data.Version version) where T : class
        {
            Item item = _database.GetItem(path, language, version);
            return CreateClass<T>(false, false, item);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K>(string path, Language language, global::Sitecore.Data.Version version, K param1) where T : class
        {
            Item item = _database.GetItem(path, language, version);
            return CreateClass<T, K>(false, false, item, param1);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L>(string path, Language language, global::Sitecore.Data.Version version, K param1, L param2) where T : class
        {
            Item item = _database.GetItem(path, language, version);
            return CreateClass<T, K, L>(false, false, item, param1, param2);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M>(string path, Language language, global::Sitecore.Data.Version version, K param1, L param2, M param3) where T : class
        {
            Item item = _database.GetItem(path, language, version);
            return CreateClass<T, K, L, M>(false, false, item, param1, param2, param3);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <typeparam name="N">The type of the fourth constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="path">The path to the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <param name="param4">The value of the fourth parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M, N>(string path, Language language, global::Sitecore.Data.Version version, K param1, L param2, M param3, N param4) where T : class
        {
            Item item = _database.GetItem(path, language, version);
            return CreateClass<T, K, L, M, N>(false, false, item, param1, param2, param3, param4);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T>(Guid id) where T : class
        {
            Item item = _database.GetItem(new ID(id));
            return CreateClass<T>(false, false, item);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K>(Guid id, K param1) where T : class
        {
            Item item = _database.GetItem(new ID(id));
            return CreateClass<T, K>(false, false, item, param1);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L>(Guid id, K param1, L param2) where T : class
        {
            Item item = _database.GetItem(new ID(id));
            return CreateClass<T, K, L>(false, false, item, param1, param2);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M>(Guid id, K param1, L param2, M param3) where T : class
        {
            Item item = _database.GetItem(new ID(id));
            return CreateClass<T, K, L, M>(false, false, item, param1, param2, param3);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <typeparam name="N">The type of the fourth constructor parameter</typeparam>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <param name="param4">The value of the fourth parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M, N>(Guid id, K param1, L param2, M param3, N param4) where T : class
        {
            Item item = _database.GetItem(new ID(id));
            return CreateClass<T, K, L, M, N>(false, false, item, param1, param2, param3, param4);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <param name="language">The language of the item to return</param>        
        /// <param name="id">The ID of the Sitecore item</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T>(Guid id, Language language) where T : class
        {
            Item item = _database.GetItem(new ID(id), language);
            return CreateClass<T>(false, false, item);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>        
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K>(Guid id, Language language, K param1) where T : class
        {
            Item item = _database.GetItem(new ID(id), language);
            return CreateClass<T, K>(false, false, item, param1);

        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>        
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L>(Guid id, Language language, K param1, L param2) where T : class
        {
            Item item = _database.GetItem(new ID(id), language);
            return CreateClass<T, K, L>(false, false, item, param1, param2);

        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>        
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M>(Guid id, Language language, K param1, L param2, M param3) where T : class
        {
            Item item = _database.GetItem(new ID(id), language);
            return CreateClass<T, K, L, M>(false, false, item, param1, param2, param3);

        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <typeparam name="N">The type of the fourth constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>        
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <param name="param4">The value of the fourth parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M, N>(Guid id, Language language, K param1, L param2, M param3, N param4) where T : class
        {
            Item item = _database.GetItem(new ID(id), language);
            return CreateClass<T, K, L, M, N>(false, false, item, param1, param2, param3, param4);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T>(Guid id, Language language, global::Sitecore.Data.Version version) where T : class
        {
            Item item = _database.GetItem(new ID(id), language, version);
            return CreateClass<T>(false, false, item);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K>(Guid id, Language language, global::Sitecore.Data.Version version, K param1) where T : class
        {
            Item item = _database.GetItem(new ID(id), language, version);
            return CreateClass<T, K>(false, false, item, param1);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L>(Guid id, Language language, global::Sitecore.Data.Version version, K param1, L param2) where T : class
        {
            Item item = _database.GetItem(new ID(id), language, version);
            return CreateClass<T, K, L>(false, false, item, param1, param2);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M>(Guid id, Language language, global::Sitecore.Data.Version version, K param1, L param2, M param3) where T : class
        {
            Item item = _database.GetItem(new ID(id), language, version);
            return CreateClass<T, K, L, M>(false, false, item, param1, param2, param3);
        }

        /// <summary>
        /// Retrieve a Sitecore item as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the Sitecore item as</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <typeparam name="N">The type of the fourth constructor parameter</typeparam>
        /// <param name="language">The language of the item to return</param>
        /// <param name="version">The version of the item to return</param>
        /// <param name="id">The ID of the Sitecore item</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <param name="param4">The value of the fourth parameter of the constructor</param>
        /// <returns>The Sitecore item as the specified type</returns>
        public T GetItem<T, K, L, M, N>(Guid id, Language language, global::Sitecore.Data.Version version, K param1, L param2, M param3, N param4) where T : class
        {
            Item item = _database.GetItem(new ID(id), language, version);
            return CreateClass<T, K, L, M, N>(false, false, item, param1, param2, param3, param4);
        }

        /// <summary>
        /// Saves a class back to Sitecore.
        /// </summary>
        /// <typeparam name="T">The type being saved. The type must have a property with the SitecoreIdAttribute.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="updateStatistics">if set to <c>true</c> [update statistics].</param>
        public void Save<T>(T target, bool updateStatistics = true) where T : class
        {
            Item item = GetItemFromSitecore<T>(target);

            item.Editing.BeginEdit();
            WriteToItem<T>(target, item);
            item.Editing.EndEdit(updateStatistics, false);
            _linkDb.UpdateReferences(item);

        }

        /// <summary>
        /// Adds a version of the item
        /// </summary>
        /// <typeparam name="T">The type being added. The type must have a property with the SitecoreIdAttribute.</typeparam>
        /// <param name="target">The class to add a version to</param>
        /// <returns></returns>
        public T AddVersion<T>(T target) where T : class
        {
            Item item = GetItemFromSitecore<T>(target);

            Item newVersion = item.Versions.AddVersion();

            return CreateClass<T>(false, false, newVersion);

        }

        /// <summary>
        /// Retrieves an item based on the passed in class based on the version and language properties on the class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        private Item GetItemFromSitecore<T>(T target)
        {
            Guid guid = InstanceContext.GetClassId(typeof(T), target);

            SitecoreClassConfig config = InstanceContext.GetSitecoreClass(typeof(T));
            Language language = null;
            int versionNumber = -1;

            if (config.LanguageProperty != null)
                language = config.LanguageProperty.Property.GetValue(target, null) as Language;
            if (config.VersionProperty != null)
            {
                versionNumber = (int)config.VersionProperty.Property.GetValue(target, null);

            }
            if (language != null && versionNumber > 0)
            {
                return _database.GetItem(new ID(guid), language, new global::Sitecore.Data.Version(versionNumber));
            }
            else if (language != null)
            {
                return _database.GetItem(new ID(guid), language);
            }
            else
            {
                return _database.GetItem(new ID(guid));
            }
        }

        /// <summary>
        /// Creates a new Sitecore item.
        /// </summary>
        /// <typeparam name="T">The type of the new item to create. This type must have either a TemplateId or BranchId defined on the SitecoreClassAttribute or fluent equivalent</typeparam>
        /// <typeparam name="K">The type of the parent item</typeparam>
        /// <param name="parent">The parent of the new item to create. Must have the SitecoreIdAttribute or fluent equivalent</param>
        /// <param name="newItem">New item to create, must have the attribute SitecoreInfoAttribute of type SitecoreInfoType.Name or the fluent equivalent</param>
        /// <param name="updateStatistics">if set to <c>true</c> [update statistics].</param>
        /// <returns></returns>
        /// <exception cref="MapperException">You are trying to create an item on a class that doesn't have an empty ID value</exception>
        public T Create<T, K>(K parent, T newItem, bool updateStatistics = true)
            where T : class
            where K : class
        {
            try
            {
                Guid id = InstanceContext.GetClassId(typeof(T), newItem);
                if (id != Guid.Empty) throw new MapperException("You are trying to create an item on a class that doesn't have an empty ID value");
            }
            catch (SitecoreIdException ex)
            {
                //we can swallow this exception for now
                //should look to do this beeter
            }


            Guid parentId = Guid.Empty;
            try
            {
                parentId = InstanceContext.GetClassId(typeof(K), parent);
            }
            catch (SitecoreIdException ex)
            {
                throw new MapperException("Failed to get parent ID", ex);
            }


            if (parentId == Guid.Empty)
                throw new MapperException("Guid for parent is empty");

            Item pItem = GetItemFromSitecore<K>(parent);
            if (pItem == null)
                throw new MapperException("Could not find parent item with ID {0}".Formatted(parentId));

            SitecoreClassConfig scClass = InstanceContext.GetSitecoreClass(typeof(T));

            var nameProperty = scClass.Properties.Where(x => x.Attribute is SitecoreInfoAttribute)
                .Cast<SitecoreProperty>().FirstOrDefault(x => x.Attribute.CastTo<SitecoreInfoAttribute>().Type == SitecoreInfoType.Name);

            if (nameProperty == null)
                throw new MapperException("Type {0} does not have a property with SitecoreInfoType.Name".Formatted(typeof(T).FullName));

            string name = string.Empty;

            try
            {
                name = nameProperty.Property.GetValue(newItem, null).ToString();
            }
            catch
            {
                throw new MapperException("Failed to get item name");
            }

            if (name.IsNullOrEmpty())
                throw new MapperException("New class has no name");


            Guid templateId = scClass.TemplateId;
            Guid branchId = scClass.BranchId;

            Item item = null;

            // Changed to use the branch id as default. This need to be done as we can't have an empty ID on the item
            if (branchId != Guid.Empty)
            {
                item = pItem.Add(name, new BranchId(new ID(branchId)));
            }
            else if (templateId != Guid.Empty)
            {
                item = pItem.Add(name, new TemplateID(new ID(templateId)));
            }
            else
            {
                throw new MapperException("Type {0} does not have a Template ID or Branch ID".Formatted(typeof(T).FullName));
            }

            if (item == null) throw new MapperException("Failed to create item");

            //write new data to the item

            item.Editing.BeginEdit();
            WriteToItem<T>(newItem, item);
            item.Editing.EndEdit(updateStatistics, false);

            //then read it back
            InstanceContext.ClassManager.ReadFromItem(this, newItem, item, scClass);
            return newItem;
            //   return CreateClass<T>(false, false, item);

        }






        #region OBSOLETE

        /// <summary>
        /// Create a blank Sitecore item
        /// </summary>
        /// <typeparam name="T">The type to create</typeparam>
        /// <typeparam name="K">The type of the parent</typeparam>
        /// <param name="parent">The parent item. This item must have been load by Glass Sitecore Mapper</param>
        /// <param name="name">The name of the item</param>
        /// <returns></returns>
        [Obsolete("Use Create<T,K>(K parent, T newItem)")]
        public T Create<T, K>(K parent, string name)
            where T : class
            where K : class
        {
            return Create<T, K>(parent, name, null);
        }


        /// <summary>
        /// Create an item with pre-populated data
        /// </summary>
        /// <typeparam name="T">The type to create</typeparam>
        /// <typeparam name="K">The type of the parent</typeparam>
        /// <param name="parent">The parent item. This item must have been load by Glass Sitecore Mapper</param>
        /// <param name="name">The name of the item</param>
        /// <param name="data">The data to pre-populate the item with</param>
        /// <param name="updateStatistics">if set to <c>true</c> [update statistics].</param>
        /// <returns></returns>
        /// <exception cref="MapperException">Failed to get parent ID</exception>
        [Obsolete("Use Create<T,K>(K parent, T newItem)")]
        public T Create<T, K>(K parent, string name, T data, bool updateStatistics = true)
            where T : class
            where K : class
        {

            //check that the data is not null and if it has an ID check that it is empty
            if (data != null)
            {


            }

            Guid guid = Guid.Empty;
            try
            {
                guid = InstanceContext.GetClassId(typeof(K), parent);
            }
            catch (SitecoreIdException ex)
            {
                throw new MapperException("Failed to get parent ID", ex);
            }


            if (guid == Guid.Empty)
                throw new MapperException("Guid for parent is empty");

            Item pItem = _database.GetItem(new ID(guid));
            if (pItem == null)
                throw new MapperException("Could not find parent item");

            SitecoreClassConfig scClass = InstanceContext.GetSitecoreClass(typeof(T));

            Guid templateId = scClass.TemplateId;
            Guid branchId = scClass.BranchId;

            Item item = null;

            if (templateId != Guid.Empty)
            {
                item = pItem.Add(name, new TemplateID(new ID(templateId)));
            }
            else if (branchId != Guid.Empty)
            {
                item = pItem.Add(name, new BranchId(new ID(branchId)));
            }
            else
            {
                throw new MapperException("Type {0} does not have a Template ID or Branch ID".Formatted(typeof(T).FullName));
            }



            if (item == null)
                throw new MapperException("Failed to create child with name {0} and parent {1}".Formatted(name, item.Paths.FullPath));

            //if we have data save it to the item
            if (data != null)
            {
                item.Editing.BeginEdit();
                WriteToItem<T>(data, item);
                item.Editing.EndEdit(updateStatistics, false);
            }
            return CreateClass<T>(false, false, item);

        }

        #endregion

        /// <summary>
        /// Delete an item from Sitecore
        /// </summary>
        /// <typeparam name="T">The type being deleted. The type must have a property with the SitecoreIdAttribute.</typeparam>
        /// <param name="item">The class to delete</param>
        public void Delete<T>(T item) where T : class
        {
            Guid guid = Guid.Empty;
            try
            {
                guid = InstanceContext.GetClassId(typeof(T), item);
            }
            catch (SitecoreIdException ex)
            {
                throw new MapperException("Failed to get item ID", ex);
            }

            if (guid == Guid.Empty)
                throw new MapperException("Guid for item is empty");

            Item scItem = _database.GetItem(new ID(guid));

            if (scItem == null)
                throw new MapperException("Item not found");

            scItem.Delete();
        }


        /// <summary>
        /// Creates a class from the specified item
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="isLazy">If true creates a proxy for the class</param>
        /// <param name="item">The item to load data from</param>
        /// <param name="inferType">Infer the type to be loaded from the template</param>
        /// <returns>The item as the specified type</returns>
        public T CreateClass<T>(bool isLazy, bool inferType, Item item) where T : class
        {
            return (T)CreateClass(isLazy, inferType, typeof(T), item);
        }

        /// <summary>
        /// Creates a class from the specified item
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="isLazy">If true creates a proxy for the class</param>
        /// <param name="item">The item to load data from</param>
        /// <param name="inferType">Infer the type to be loaded from the template</param>
        /// <returns>The item as the specified type</returns>
        public object CreateClass(bool isLazy, bool inferType, Type type, Item item)
        {
            //we have to add null to the list of parameters otherwise we get a stack overflow
            return InstanceContext.ClassManager.CreateClass(this, isLazy, inferType, type, item, null);
        }



        /// <summary>
        /// Create a collection of classes from the specified type
        /// </summary>
        /// <param name="isLazy">If true creates a proxy for each class</param>
        /// <param name="type">The type to return</param>
        /// <param name="getItems">A function that returns the list of items to load</param>
        /// <returns>An enumerable of the items as the specified type</returns>
        public IEnumerable CreateClasses(bool isLazy, Type type, Func<IEnumerable<Item>> getItems)
        {
            return CreateClasses(isLazy, false, type, getItems);
        }

        /// <summary>
        /// Create a collection of classes from the specified type
        /// </summary>
        /// <param name="isLazy">If true creates a proxy for each class</param>
        /// <param name="type">The type to return</param>
        /// <param name="getItems">A function that returns the list of items to load</param>
        /// <param name="inferType">Infer the type to be loaded from the item template</param>
        /// <returns>An enumerable of the items as the specified type</returns>
        public IEnumerable CreateClasses(bool isLazy, bool inferType, Type type, Func<IEnumerable<Item>> getItems)
        {
            return Utility.CreateGenericType(typeof(Enumerable<>), new Type[] { type }, getItems, this, isLazy, inferType) as IEnumerable;
        }

        /// <summary>
        /// Creates a class from the specified item with a single constructor parameter
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <param name="isLazy">If true creates a proxy for the class</param>
        /// <param name="item">The item to load data from</param>
        /// <param name="inferType">Infer the type to be loaded from the template</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <returns>The item as the specified type</returns>
        public T CreateClass<T, K>(bool isLazy, bool inferType, Item item, K param1)
        {
            return (T)InstanceContext.ClassManager.CreateClass(this, isLazy, inferType, typeof(T), item, param1);

        }

        /// <summary>
        /// Creates a class from the specified item with a two constructor parameter
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <param name="isLazy">If true creates a proxy for the class</param>
        /// <param name="item">The item to load data from</param>
        /// <param name="inferType">Infer the type to be loaded from the template</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <returns>The item as the specified type</returns>
        public T CreateClass<T, K, L>(bool isLazy, bool inferType, Item item, K param1, L param2)
        {
            return (T)InstanceContext.ClassManager.CreateClass(this, isLazy, inferType, typeof(T), item, param1, param2);
        }

        /// <summary>
        /// Creates a class from the specified item with a two constructor parameter
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <param name="isLazy">If true creates a proxy for the class</param>
        /// <param name="item">The item to load data from</param>
        /// <param name="inferType">Infer the type to be loaded from the template</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <returns>The item as the specified type</returns>
        public T CreateClass<T, K, L, M>(bool isLazy, bool inferType, Item item, K param1, L param2, M param3)
        {
            return (T)InstanceContext.ClassManager.CreateClass(this, isLazy, inferType, typeof(T), item, param1, param2, param3);
        }

        /// <summary>
        /// Creates a class from the specified item with a two constructor parameter
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <typeparam name="K">The type of the first constructor parameter</typeparam>
        /// <typeparam name="L">The type of the second constructor parameter</typeparam>
        /// <typeparam name="M">The type of the third constructor parameter</typeparam>
        /// <typeparam name="N">The type of the fourth constructor parameter</typeparam>
        /// <param name="isLazy">If true creates a proxy for the class</param>
        /// <param name="item">The item to load data from</param>
        /// <param name="inferType">Infer the type to be loaded from the template</param>
        /// <param name="param1">The value of the first parameter of the constructor</param>       
        /// <param name="param2">The value of the second parameter of the constructor</param>
        /// <param name="param3">The value of the third parameter of the constructor</param>
        /// <param name="param4">The value of the fourth parameter of the constructor</param>
        /// <returns>The item as the specified type</returns>
        public T CreateClass<T, K, L, M, N>(bool isLazy, bool inferType, Item item, K param1, L param2, M param3, N param4)
        {
            return (T)InstanceContext.ClassManager.CreateClass(this, isLazy, inferType, typeof(T), item, param1, param2, param3, param4);
        }


        public void Move<T, K>(T item, K newParent)
        {
            Guid itemId = Guid.Empty;
            try
            {
                itemId = InstanceContext.GetClassId(typeof(T), item);
            }
            catch (SitecoreIdException ex)
            {
                throw new MapperException("Failed to get item ID", ex);
            }

            Guid newParentId = Guid.Empty;
            try
            {
                newParentId = InstanceContext.GetClassId(typeof(K), newParent);
            }
            catch (SitecoreIdException ex)
            {
                throw new MapperException("Failed to get new parent ID", ex);
            }

            Item scItem = Database.GetItem(new ID(itemId));
            Item scNewParent = Database.GetItem(new ID(newParentId));

            scItem.MoveTo(scNewParent);



        }

        #endregion

        #region Private Methods



        public void WriteToItem<T>(T target, Item item)
        {
            var scClass = InstanceContext.GetSitecoreClass(typeof(T));

            foreach (var handler in scClass.DataHandlers)
            {
                if (handler.CanSetValue)
                {
                    handler.ReadProperty(target, item, this);
                }
            }
        }



        #endregion




    }
}
