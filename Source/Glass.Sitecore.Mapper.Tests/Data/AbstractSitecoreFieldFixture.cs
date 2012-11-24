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
using Glass.Sitecore.Mapper.Data;
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Glass.Sitecore.Mapper.Tests.Data
{
    [TestFixture]
    public class AbstractSitecoreFieldFixture
    {
        AbstractSitecoreFieldFixtureNS.TestClass testClass;
        Database _database;

        [SetUp]
        public void Setup()
        {
            testClass = new Glass.Sitecore.Mapper.Tests.Data.AbstractSitecoreFieldFixtureNS.TestClass();
            _database = global::Sitecore.Configuration.Factory.GetDatabase("master");
        }

        [Test]
        public void CanSetValue_FieldIsWritable_ReturnsTrue()
        {
            //Assign
            SitecoreProperty property = new SitecoreProperty(){
                Property = new FakePropertyInfo(typeof(string)),
                 Attribute = new SitecoreFieldAttribute(){
                        
                }
            };
            testClass.ConfigureDataHandler(property);
            //Act
            var result = testClass.CanSetValue;
               
            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanSetValue_FieldIsReadOnly_ReturnsFalse()
        {
            //Assign
            SitecoreProperty property = new SitecoreProperty()
            {
                Property = new FakePropertyInfo(typeof(string)),
                Attribute = new SitecoreFieldAttribute()
                {
                    ReadOnly = true
                }
            };

            testClass.ConfigureDataHandler(property);


            //Act
            var result = testClass.CanSetValue;

            //Assert
            Assert.IsFalse(result);
        }

      

        [Test]
        public void GetField_UsingFieldName_InvaliFieldNameReturnsNull()
        {
            //Assign

            AbstractSitecoreFieldFixtureNS.TestClass attr = new AbstractSitecoreFieldFixtureNS.TestClass();
            attr.FieldName = "no field here";

           
                var item = _database.GetItem("/sitecore/content/Data/AbstractSitecoreField/Item1");

                //Act
                var field = attr.GetField(item);

                //Assert
                Assert.IsNull(field);
            

        }

        [Test]
        public void GetField_UsingFieldId_InvalidIdReturnsNull()
        {
            //Assign

            AbstractSitecoreFieldFixtureNS.TestClass attr = new AbstractSitecoreFieldFixtureNS.TestClass();
            attr.FieldId = new ID("{AAAAAAAA-604A-4F0A-BEDD-A346567075D3}");

            var item = _database.GetItem("/sitecore/content/Data/AbstractSitecoreField/Item1");

             //have to use the security disable because the call item.Templates requires the role manager that  isn't available.
            using (new SecurityDisabler())
            {
                //Act
                var field = attr.GetField(item);

                //Assert
                Assert.IsNull(field);
            }

        }

        [Test]
        public void GetField_UsingFieldIdAndName_FieldIDIsUsedOverName()
        {
            //Assign

            AbstractSitecoreFieldFixtureNS.TestClass attr = new AbstractSitecoreFieldFixtureNS.TestClass();
            attr.FieldId = new ID("{9D57BA8A-604A-4F0A-BEDD-A346567075D3}");
            attr.FieldName = "randomvalue";

            var item = _database.GetItem("/sitecore/content/Data/AbstractSitecoreField/Item1");

            //Act
            var field = attr.GetField(item);

            //Assert
            Assert.AreEqual("Test Single", field.Value);


        }
    }

    namespace AbstractSitecoreFieldFixtureNS
    {
        public class TestClass : AbstractSitecoreField
        {
            public object SetValue { get; set; }

            public override object GetFieldValue(string fieldValue, global::Sitecore.Data.Items.Item item , ISitecoreService service)
            {
                return SetValue;
            }

            public override string SetFieldValue(object value, ISitecoreService service)
            {
                throw new NotImplementedException();
            }

            public override Type TypeHandled
            {
                get { throw new NotImplementedException(); }
            }

            public Field GetField(Item item)
            {
                return base.GetField(item);
            }
        }
    }
}
