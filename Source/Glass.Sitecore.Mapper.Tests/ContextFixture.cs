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
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Glass.Sitecore.Mapper.Data;

namespace Glass.Sitecore.Mapper.Tests
{
    [TestFixture]
    public class ContextFixture
    {
        Context _context;

        [SetUp]
        public void Setup()
        {

            
            
        }


        #region Load

        [Test]
        public void GetContext_ReturnsContextWithLoadedClass()
        {
            //Act
            AttributeConfigurationLoader loader = new AttributeConfigurationLoader(
              new string[] { "Glass.Sitecore.Mapper.Tests.ContextFixtureNS, Glass.Sitecore.Mapper.Tests" }
              );
            _context = new Context(loader, new AbstractSitecoreDataHandler[] { });
            InstanceContext instance = Context.GetContext();

            //Assert
            Assert.AreEqual(2, instance.Classes.Count());

            
        }

        [Test]
        [ExpectedException(typeof(MapperException))]
        public void GetContext_MutlipleConfigLoadersSameNamespace_ThrowsException()
        {
            //Act
            AttributeConfigurationLoader loader1 = new AttributeConfigurationLoader(
              new string[] { "Glass.Sitecore.Mapper.Tests.ContextFixtureNS, Glass.Sitecore.Mapper.Tests" }
              );

            AttributeConfigurationLoader loader2 = new AttributeConfigurationLoader(
              new string[] { "Glass.Sitecore.Mapper.Tests.ContextFixtureNS, Glass.Sitecore.Mapper.Tests" }
              );
            _context = new Context(loader1, loader2);
            InstanceContext instance = Context.GetContext();

            //Assert
            Assert.AreEqual(2, instance.Classes.Count());
        }

        [Test]
        public void GetContext_MutlipleConfigLoadersDifferentNamespace_ReturnsContextWithLoadedClasses()
        {
            //Act
            AttributeConfigurationLoader loader1 = new AttributeConfigurationLoader(
              new string[] { "Glass.Sitecore.Mapper.Tests.ContextFixtureNS, Glass.Sitecore.Mapper.Tests" }
              );

            AttributeConfigurationLoader loader2 = new AttributeConfigurationLoader(
              new string[] { "Glass.Sitecore.Mapper.Tests.ContextFixtureNS2, Glass.Sitecore.Mapper.Tests" }
              );
            _context = new Context(loader1, loader2);
            InstanceContext instance = Context.GetContext();

            //Assert
            Assert.AreEqual(4, instance.Classes.Count());
        }

        #endregion

    }

    namespace ContextFixtureNS
    {

        [SitecoreClass]
        public class ContextFixtureTest1
        {
        }
        [SitecoreClass]
        public class ContextFixtureTest2
        {
        }
    }

    namespace ContextFixtureNS2
    {

        [SitecoreClass]
        public class ContextFixtureTest1
        {
        }
        [SitecoreClass]
        public class ContextFixtureTest2
        {
        }
    }

}
