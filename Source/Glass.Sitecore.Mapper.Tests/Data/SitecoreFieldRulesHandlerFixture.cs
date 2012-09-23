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
using NUnit.Framework;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.Data;
using Sitecore.Data;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Tests.Domain;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.InsertOptions;

namespace Glass.Sitecore.Mapper.Tests.Data
{
    [TestFixture]
    public class SitecoreFieldRulesHandlerFixture
    {
        Database _db;
        Guid _itemId;
        SitecoreFieldRulesHandler _handler;

        [SetUp]
        public void Setup()
        {
            Context context = new Context(
                new AttributeConfigurationLoader(
                    "Glass.Sitecore.Mapper.Tests.Data.SitecoreFieldRulesHandlerFixture,  Glass.Sitecore.Mapper.Tests",
                    "Glass.Sitecore.Mapper.Tests.Domain,  Glass.Sitecore.Mapper.Tests"));

            _db = global::Sitecore.Configuration.Factory.GetDatabase("master");
            _itemId = new Guid("{B17E8A5F-D2C8-4083-AB8C-E9D22012C446}");
            _handler = new SitecoreFieldRulesHandler();
        }

        [Test]
        public void GetValue_ReturnsValidRules()
        {
            //Assign
            Item item = _db.GetItem(new ID(_itemId));
            Type propertyType = typeof(RuleList<>).MakeGenericType(new Type[] {typeof (TestRuleContext)});
            SitecoreProperty property = new SitecoreProperty()
            {
                Attribute = new SitecoreFieldAttribute(),
                Property = new FakePropertyInfo(propertyType, "Rules")
            };

            _handler.ConfigureDataHandler(property);

            //Act
            var result = _handler.GetValue(item, null) as RuleList<TestRuleContext>;

            //Assert
            Assert.IsNotNull(result);
            Assert.True(result.Count == 2);
        }

        public class TestRuleContext : InsertOptionsRuleContext
        {

        }
    }
}
