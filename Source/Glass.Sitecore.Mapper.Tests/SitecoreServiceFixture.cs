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
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.Tests.SitecoreServiceFixtureNS;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Glass.Sitecore.Mapper.Data;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.SecurityModel;
using Moq;
using Glass.Sitecore.Mapper.Tests.Domain;

namespace Glass.Sitecore.Mapper.Tests
{
    [TestFixture]
    public class SitecoreServiceFixture
    {
        SitecoreService _sitecore;
        Context _context;
        Database _db;

        [SetUp]
        public void Setup()
        {

            AttributeConfigurationLoader loader = new AttributeConfigurationLoader(
                new string []{
                    "Glass.Sitecore.Mapper.Tests.SitecoreServiceFixtureNS, Glass.Sitecore.Mapper.Tests",
                    "Glass.Sitecore.Mapper.Tests.Domain, Glass.Sitecore.Mapper.Tests"

                }
                );

            _context = new Context(loader, new AbstractSitecoreDataHandler[]{new SitecoreIdDataHandler() });

            _sitecore = new SitecoreService("master");
            _db = global::Sitecore.Configuration.Factory.GetDatabase("master");
        }

        #region GetItem
        [Test]
        public void GetItem_ByPath_ReturnsItem()
        {
            //Assign
            string path =  "/sitecore/content/Glass/Test1";
            //Act
            TestClass result = _sitecore.GetItem<TestClass>(path);

            //Assert
            Assert.IsNotNull(result);
        }
        [Test]
        public void GetItem_ById_ReturnsItem()
        {
            //Assign
            Guid id = new Guid("{8A317CBA-81D4-4F9E-9953-64C4084AECCA}");

            //Act
            TestClass result = _sitecore.GetItem<TestClass>(id);

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetItem_ByPath_ReturnsNullItemDoesNotExist()
        {
            //Assign
            string path = "/sitecore/content/Glass/DoesntExist";
            //Act
            TestClass result = _sitecore.GetItem<TestClass>(path);

            //Assert
            Assert.IsNull(result);
        }
        [Test]
        public void GetItem_ById_ReturnsNullItemDoesNotExist()
        {

            //Assign
            Guid id = new Guid("{99317CBA-81D4-4F9E-9953-64C4084AECC1}");
           
            //Act
            TestClass result = _sitecore.GetItem<TestClass>(id);

            //Assert
            Assert.IsNull(result);
        }
        #endregion

        #region Query

        [Test]
        public void Query_ReturnsSetOfClasses()
        {
            //Assign
            Item parent = _db.GetItem("/sitecore/content/glass");
            string query = "/sitecore/content/glass/*";

            //Act
            var result = _sitecore.Query<SitecoreServiceFixtureNS.TestClass>(query);

            //Assert
            Assert.AreEqual(parent.Children.Count, result.Count());
            Assert.IsTrue(parent.Children.Cast<Item>().All(x => result.Any(y => y.Id == x.ID.Guid)));

        }

        [Test]
        public void Query_IncorrectQuery_ReturnsEmptyEnumeration()
        {
            //Assign
            string query = "/sitecore/content/glass/notthere/*";

            //Act
            var result = _sitecore.Query<SitecoreServiceFixtureNS.TestClass>(query);

            //Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void Query_ReturnsSetOfClasses_Proxies()
        {
            //Assign
            Item parent = _db.GetItem("/sitecore/content/glass");
            string query = "/sitecore/content/glass/*";

            //Act
            var result = _sitecore.Query<SitecoreServiceFixtureNS.TestClass>(query, true);

            //Asserte
            Assert.AreNotEqual(typeof(SitecoreServiceFixtureNS.TestClass), result.First().GetType());
            Assert.IsTrue(result.First() is SitecoreServiceFixtureNS.TestClass);
            Assert.AreEqual(parent.Children.Count, result.Count());
            Assert.IsTrue(parent.Children.Cast<Item>().All(x => result.Any(y => y.Id == x.ID.Guid)));

        }

        

        #endregion

        #region QuerySingle

        [Test]
        public void QuerySingle_ReturnsSingleClasses()
        {
            //Assign
            Item parent = _db.GetItem("/sitecore/content/glass");
            string query = "/sitecore/content/glass/*";

            //Act
            var result = _sitecore.QuerySingle<SitecoreServiceFixtureNS.TestClass>(query);

            //Assert
            Assert.AreEqual(parent.Children[0].ID.Guid, result.Id);

        }
        [Test]
        public void QuerySingle_IncorrectQuery_ReturnsNull()
        {
            //Assign
            string query = "/sitecore/content/glass/notthere/*";

            //Act
            var result = _sitecore.QuerySingle<SitecoreServiceFixtureNS.TestClass>(query);

            //Assert
            Assert.IsNull(result);
        }


        #endregion

        #region Create

        [Test]
        [ExpectedException(typeof(MapperException))]
        public void Create_NoTemplateId_ThrowsException()
        {
            //Assign
            TestClass test3 = _sitecore.GetItem<TestClass>("/sitecore/content/Glass/Test1/Test3");

            using (new SecurityDisabler())
            {
                //Act
                _sitecore.Create<TestClass, TestClass>(test3, "Test4");


                //Assert
                //N/A
            }
        }

        [Test]
        public void Create_CreatesAnItem()
        {
            //Assign
            TestClass test3 = _sitecore.GetItem<TestClass>("/sitecore/content/Glass/Test1/Test3");

            using (new SecurityDisabler())
            {
                //Act
                CreateClass newItem = _sitecore.Create<CreateClass, TestClass>(test3, "Test4");


                //Assert
                Item item = _db.GetItem("/sitecore/content/Glass/Test1/Test3/Test4");
                Assert.IsNotNull(item);
                Assert.AreNotEqual(item.ID, newItem.Id);

                try
                {
                    //Clean up
                    item.Delete();
                }
                catch (NullReferenceException ex)
                {
                    //this expection is thrown by Sitecore.Tasks.ItemEventHandler.OnItemDeleted
                }
            }
        }

        [Test]
        public void Create_CreatesAnItem_PrePopulates()
        {
            //Assign
            TestClass test3 = _sitecore.GetItem<TestClass>("/sitecore/content/Glass/Test1/Test3");
            CreateClass preClass = new CreateClass() { SingleLineText = "some test data" };

            using (new SecurityDisabler())
            {


                //Act
                CreateClass newItem = _sitecore.Create<CreateClass, TestClass>(test3, "Test5", preClass);


                //Assert
                Item item = _db.GetItem("/sitecore/content/Glass/Test1/Test3/Test5");
                Assert.IsNotNull(item);
                Assert.AreNotEqual(item.ID, newItem.Id);
                Assert.AreEqual(preClass.SingleLineText, item["SingleLineText"]);

                try
                {
                    //Clean up
                    item.Delete();
                }
                catch (NullReferenceException ex)
                {
                    //this expection is thrown by Sitecore.Tasks.ItemEventHandler.OnItemDeleted
                }
            }
        }

        [Test]
        public void Create_CreatesAnItem_PrePopulates_NewMethod()
        {
            //Assign
            TestClass test3 = _sitecore.GetItem<TestClass>("/sitecore/content/Glass/Test1/Test3");
            CreateClass preClass = new CreateClass() { SingleLineText = "some test data", Name = "Test6" };

            using (new SecurityDisabler())
            {


                //Act
                CreateClass newItem = _sitecore.Create<CreateClass, TestClass>(test3, preClass);


                //Assert
                Item item = _db.GetItem("/sitecore/content/Glass/Test1/Test3/Test6");


                Assert.AreEqual(preClass, newItem);
                Assert.AreEqual(item.ID.Guid, newItem.Id);
                Assert.IsNotNull(item);
                Assert.AreNotEqual(item.ID, newItem.Id);
                Assert.AreEqual(preClass.SingleLineText, item["SingleLineText"]);

                try
                {
                    //Clean up
                    item.Delete();
                }
                catch (NullReferenceException ex)
                {
                    //this expection is thrown by Sitecore.Tasks.ItemEventHandler.OnItemDeleted
                }
            }
        }

        
        #endregion

        #region Delete

        [Test]
        public void Delete_DoesDeleteItem()
        {
            //Assign
            string parentPath = "/sitecore/content/SitecoreService/Delete_DoesDeleteItem";
            string itemName = "Test4";
            Item parent = _db.GetItem(parentPath);
            Guid templateId = new Guid("{1D0EE1F5-21E0-4C5B-8095-EDE2AF3D3300}");
            Item child = null;

            using (new SecurityDisabler())
            {
                 parent.DeleteChildren();
                 child = parent.Add(itemName, new TemplateID(new ID(templateId)));
            }

            Assert.IsNotNull(child);
            Guid childId = child.ID.Guid;

            TestClass childClass = _sitecore.GetItem<TestClass>("/sitecore/content/SitecoreService/Delete_DoesDeleteItem/Test4");
            Assert.IsNotNull(childClass);
            Assert.AreEqual(childId, childClass.Id);

            //Act
            using (new SecurityDisabler())
            {
                try
                {
                    _sitecore.Delete<TestClass>(childClass);
                }
                catch (NullReferenceException ex)
                {
                    //we need to catch a null reference exception raised by the Sitecore.Tasks.ItemEventHandler.OnItemDeleted
                }
            }

            //Assert

            Item check = _db.GetItem("/sitecore/content/SitecoreService/Delete_DoesDeleteItem/Test4");
            Assert.IsNull(check);


        }

        #endregion

        #region Method - CreateClass
        
        [Test]
        public void CreateClass_CreateAClassWithSingleConstructor()
        {
            //Assign
            string path =  "/sitecore/content/Glass/Test1";
            int param1 = 10;
            Item item1 = _db.GetItem(path);
            
            //Act
            TestClassOneParamConstructor result = _sitecore.CreateClass<TestClassOneParamConstructor, int>(false, false, item1, param1);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Param1);
        }

        [Test]
        public void CreateClass_CreateAClassWithTwoParametersConstructor()
        {
            //Assign
            string path = "/sitecore/content/Glass/Test1";
            int param1 = 10;
            string param2 = "Hello world";

            Item item1 = _db.GetItem(path);

            //Act
            TestClassTwoParamConstructor result = _sitecore.CreateClass<TestClassTwoParamConstructor, int, string>(false, false, item1, param1, param2);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Param1);
            Assert.AreEqual("Hello world", result.Param2);
        }

        [Test]
        public void CreateClass_CreateAClassWithThreeParametersConstructor()
        {
            //Assign
            string path = "/sitecore/content/Glass/Test1";
            int param1 = 10;
            string param2 = "Hello world";
            bool param3 = true;

            Item item1 = _db.GetItem(path);

            //Act
            TestClassThreeParamConstructor result = _sitecore.CreateClass<TestClassThreeParamConstructor, int, string, bool>(false, false, item1, param1, param2, param3);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Param1);
            Assert.AreEqual("Hello world", result.Param2);
            Assert.AreEqual(true, result.Param3);
        }

        [Test]
        public void CreateClass_CreateAClassWithFourParametersConstructor()
        {
            //Assign
            string path = "/sitecore/content/Glass/Test1";
            int param1 = 10;
            string param2 = "Hello world";
            bool param3 = true;
            float param4 = 0.5f;

            Item item1 = _db.GetItem(path);

            //Act
            TestClassFourParamConstructor result = _sitecore.CreateClass<TestClassFourParamConstructor, int, string, bool, float>(false, false, item1, param1, param2, param3, param4);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Param1);
            Assert.AreEqual("Hello world", result.Param2);
            Assert.AreEqual(true, result.Param3);
            Assert.AreEqual(0.5f, result.Param4);
        }

        [Test]
        public void CreateClass_CreateAClassWithMultipleParametersConstructor()
        {
            //Assign
            string path = "/sitecore/content/Glass/Test1";
            int param1 = 10;
            string param2 = "Hello world";
            bool param3 = true;
            float param4 = 0.5f;

            Item item1 = _db.GetItem(path);

            //Act
            TestClassMultipleParamConstructor result1 = _sitecore.CreateClass<TestClassMultipleParamConstructor, int, string, bool, float>(false, false, item1, param1, param2, param3, param4);
            TestClassMultipleParamConstructor result2 = _sitecore.CreateClass<TestClassMultipleParamConstructor, int, string, bool>(false, false, item1, param1, param2, param3);

            //Assert
            Assert.IsNotNull(result1);
            Assert.AreEqual(10, result1.Param1);
            Assert.AreEqual("Hello world", result1.Param2);
            Assert.AreEqual(true, result1.Param3);
            Assert.AreEqual(0.5f, result1.Param4);


            Assert.IsNotNull(result1);
            Assert.AreEqual(10, result1.Param1);
            Assert.AreEqual("Hello world", result1.Param2);
            Assert.AreEqual(true, result1.Param3);
        }


        [Test]
        [ExpectedException(typeof(MapperException))]
        public void CreateClass_CreateAClassWithFourParametersConstructor_WrongOrder()
        {
            //Assign
            string path = "/sitecore/content/Glass/Test1";
            int param1 = 10;
            string param2 = "Hello world";
            bool param3 = true;
            float param4 = 0.5f;

            Item item1 = _db.GetItem(path);

            //Act
            TestClassFourParamConstructor result = _sitecore.CreateClass<TestClassFourParamConstructor, int, string, float, bool>(false, false, item1, param1, param2, param4, param3);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Param1);
            Assert.AreEqual("Hello world", result.Param2);
            Assert.AreEqual(true, result.Param3);
            Assert.AreEqual(0.5f, result.Param4);
        }

        [Test]
        [ExpectedException(typeof(MapperException))]
        public void CreateClass_CreateAClassWithTwoParametersConstructorParametersIncorrectOrder()
        {
            //Assign
            string path = "/sitecore/content/Glass/Test1";
            int param1 = 10;
            string param2 = "Hello world";

            Item item1 = _db.GetItem(path);

            //Act
            TestClassTwoParamConstructor result = _sitecore.CreateClass<TestClassTwoParamConstructor, string, int>(false, false, item1, param2, param1);

            //Assert
            //expected exception


        }

        #endregion

        #region Method - Move

        [Test]
        public void Move_MovesItemFromParent1ToParent2()
        {
            //Assign
            string currentPath = "/sitecore/content/SitecoreService/Move/Parent1/Child";
            string parent1Path = "/sitecore/content/SitecoreService/Move/Parent1";
            string parent2Path = "/sitecore/content/SitecoreService/Move/Parent2";
            string newPath = "/sitecore/content/SitecoreService/Move/Parent2/Child";


            var parent1 = _sitecore.GetItem<EmptyTemplate1>(parent1Path);
            var parent2 = _sitecore.GetItem<EmptyTemplate1>(parent2Path);
            var child = _sitecore.GetItem<EmptyTemplate1>(currentPath);
            var childNew = _sitecore.GetItem<EmptyTemplate1>(newPath);

            //not item should exist at target
            Assert.IsNull(childNew);

            //Act
            using (new SecurityDisabler())
            {
                try
                {

                    _sitecore.Move(child, parent2);
                    childNew = _sitecore.GetItem<EmptyTemplate1>(newPath);

                    //Assert
                    Assert.IsNotNull(childNew);

                }
                finally
                {
                    Item item = _db.GetItem(newPath);
                    Item oldParent = _db.GetItem(parent1Path);
                    if (item != null)
                        item.MoveTo(oldParent);

                }

            }



        }


        #endregion

    }
    namespace SitecoreServiceFixtureNS
    {
        [SitecoreClass]
        public class TestClassOneParamConstructor
        {

            public TestClassOneParamConstructor(int param1)
            {
                Param1 = param1;
            }

            public int Param1 { get; set; }

            [SitecoreId]
            public virtual Guid Id { get; set; }
        }

        [SitecoreClass]
        public class TestClassTwoParamConstructor
        {

            public TestClassTwoParamConstructor(int param1, string param2)
            {
                Param1 = param1;
                Param2 = param2;
            }

            public int Param1 { get; set; }
            public string Param2 { get; set; }

            [SitecoreId]
            public virtual Guid Id { get; set; }
        }

        [SitecoreClass]
        public class TestClassThreeParamConstructor
        {

            public TestClassThreeParamConstructor(int param1, string param2, bool param3)
            {
                Param1 = param1;
                Param2 = param2;
                Param3 = param3;
            }

            public int Param1 { get; set; }
            public string Param2 { get; set; }
            public bool Param3{get;set;}

            [SitecoreId]
            public virtual Guid Id { get; set; }
        }

        [SitecoreClass]
        public class TestClassFourParamConstructor
        {

            public TestClassFourParamConstructor(int param1, string param2, bool param3, float param4)
            {
                Param1 = param1;
                Param2 = param2;
                Param3 = param3;
                Param4 = param4;
            }

            public int Param1 { get; set; }
            public string Param2 { get; set; }
            public bool Param3 { get; set; }
            public float Param4 { get; set; }

            [SitecoreId]
            public virtual Guid Id { get; set; }
        }

        [SitecoreClass]
        public class TestClassMultipleParamConstructor
        {

            public TestClassMultipleParamConstructor(int param1, string param2, bool param3)
            {
                Param1 = param1;
                Param2 = param2;
                Param3 = param3;
            }

            public TestClassMultipleParamConstructor(int param1, string param2, bool param3, float param4)
            {
                Param1 = param1;
                Param2 = param2;
                Param3 = param3;
                Param4 = param4;
            }

            public int Param1 { get; set; }
            public string Param2 { get; set; }
            public bool Param3 { get; set; }
            public float Param4 { get; set; }

            [SitecoreId]
            public virtual Guid Id { get; set; }
        }

        [SitecoreClass]
        public class TestClass
        {
            [SitecoreId]
            public virtual Guid Id { get; set; }

           
        }

        [SitecoreClass(TemplateId = "{1D0EE1F5-21E0-4C5B-8095-EDE2AF3D3300}")]
        public class CreateClass
        {
            [SitecoreId]
            public virtual Guid Id { get; set; }

            [SitecoreField]
            public virtual string SingleLineText { get; set; }

            [SitecoreInfo(SitecoreInfoType.Name)]
            public virtual string Name { get; set; }
        }
        

    }
}
