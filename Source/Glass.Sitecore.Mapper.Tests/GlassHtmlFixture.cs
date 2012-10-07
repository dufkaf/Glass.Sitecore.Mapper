using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections.Specialized;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Glass.Sitecore.Mapper.Tests.GlassHtmlFixtureNs;

namespace Glass.Sitecore.Mapper.Tests
{
    [TestFixture]
    public class GlassHtmlFixture
    {
        Context _context;
        ISitecoreService _service;
        string _editablePath;

        [SetUp]
        public void Setup()
        {
            _context = new Context(
                new AttributeConfigurationLoader("Glass.Sitecore.Mapper.Tests.GlassHtmlFixtureNs, Glass.Sitecore.Mapper.Tests")
                );

            _service = new SitecoreService("master");
            _editablePath = "/sitecore/content/GlassHtml/Editable";
        }

        #region RenderImage
        [Test]
        public void RenderImage_RendersImageWithAttributes()
        {
            GlassHtml html = new GlassHtml("master");

            //Assign
            FieldTypes.Image img = new FieldTypes.Image();
            img.Alt = "Some alt test";
            img.Src = "/cats.jpg";
            img.Class = "classy";

            NameValueCollection attrs = new NameValueCollection();
            attrs.Add("style", "allStyle");
            
            //Act
            var result = html.RenderImage(img, attrs);

            //Assert
            Assert.AreEqual("<img src='/cats.jpg' style='allStyle' class='classy' alt='Some alt test' />", result);

        }

        #endregion

        #region RenderLink

        [Test]
        public void RenderLink_RendersAValidaLink()
        {

            //Assign
            GlassHtml html = new GlassHtml("master");

            FieldTypes.Link link = new FieldTypes.Link();
            link.Class = "classy";
            link.Anchor = "landSighted";
            link.Target = "xMarksTheSpot";
            link.Text = "Click here";
            link.Title = "You should click here";
            link.Url = "/yourpage";

            NameValueCollection attrs = new NameValueCollection();
            attrs.Add("style", "got some");

            //Act
            var result = html.RenderLink(link, attrs);

            //Assert
            Assert.AreEqual("<a href='/yourpage#landSighted' title='You should click here' target='xMarksTheSpot' class='classy' style='got some' class='classy' target='xMarksTheSpot' title='You should click here' >Click here</a>", result);


        }
        #endregion

        #region Method - Editable

        [Test]
        public void Editable_RootInterface_NoExceptions()
        {
            //Assign
            IBase item = _service.GetItem<IBase>(_editablePath);
            GlassHtml html = new GlassHtml(_service);
            global::Sitecore.Context.Site = global::Sitecore.Configuration.Factory.GetSite("website");
                 

            //Act
            html.Editable(item, x => x.SingleLineText);
        }

        [Test]
        public void Editable_InterfaceWithSuperInterface_NoExceptions()
        {
            //Assign
            ISubType1 item = _service.GetItem<ISubType1>(_editablePath);
            GlassHtml html = new GlassHtml(_service);
            global::Sitecore.Context.Site = global::Sitecore.Configuration.Factory.GetSite("website");


            //Act
            html.Editable(item, x => x.SingleLineText);
        }

        [Test]
        public void Editable_InterfaceWithSuperInterfaceWithSameName_NoExceptions()
        {
            //Assign
            GlassHtmlFixtureNs.Inner.ISubType1 item = _service.GetItem<GlassHtmlFixtureNs.Inner.ISubType1>(_editablePath);
            GlassHtml html = new GlassHtml(_service);
            global::Sitecore.Context.Site = global::Sitecore.Configuration.Factory.GetSite("website");


            //Act
            html.Editable(item, x => x.SingleLineText);
            html.Editable(item, x => x.RichText);
        }

        [Test]
        public void Editable_ConcreteType_NoExceptions()
        {
            //Assign
            global::Sitecore.Context.Site = global::Sitecore.Configuration.Factory.GetSite("website");

            Concrete1 item = _service.GetItem<Concrete1>(_editablePath);
            GlassHtml html = new GlassHtml(_service);


            //Act
            html.Editable(item, x => x.SingleLineText);
            html.Editable(item, x => x.Text);
        }

        [Test]
        public void Editable_ConcreteAccessingFirstChild_NoExceptions()
        {
            //Assign
            global::Sitecore.Context.Site = global::Sitecore.Configuration.Factory.GetSite("website");

            Concrete1 item = _service.GetItem<Concrete1>(_editablePath);
            GlassHtml html = new GlassHtml(_service);


            //Act
            html.Editable(item, x => x.Children.First().SingleLineText);
            html.Editable(item, x => x.Children.First().Text);
        }

        [Test]
        public void Editable_ConcreteMissingField_NoExceptions()
        {
            //Assign
            global::Sitecore.Context.Site = global::Sitecore.Configuration.Factory.GetSite("website");

            Concrete2 item = _service.GetItem<Concrete2>(_editablePath);
            GlassHtml html = new GlassHtml(_service);


            //Act
            html.Editable(item, x => x.MissingField);
        }

        [Test]
        [ExpectedException(typeof(MapperException))]
        public void Editable_ConcreteNoId_NoExceptions()
        {
            //Assign
            global::Sitecore.Context.Site = global::Sitecore.Configuration.Factory.GetSite("website");

            Concrete3 item = _service.GetItem<Concrete3>(_editablePath);
            GlassHtml html = new GlassHtml(_service);


            //Act
            html.Editable(item, x => x.MissingField);
        }

        #endregion


    }

    namespace GlassHtmlFixtureNs
    {
        [SitecoreClass]
        public interface IBase
        {
            [SitecoreId]
            Guid Id { get; set; }

            [SitecoreField]
            string SingleLineText { get; set; }
        }

        [SitecoreClass]
        public interface ISubType1: IBase{
            
        }

        [SitecoreClass]
        public class Concrete1
        {
            [SitecoreId]
            public virtual Guid Id { get; set; }

            [SitecoreField]
            public virtual string SingleLineText { get; set; }

            [SitecoreField("RichText")]
            public virtual string Text { get; set; }

            [SitecoreChildren]
            public virtual IEnumerable<Concrete1> Children { get; set; }

        }

        [SitecoreClass]
        public class Concrete2
        {
            [SitecoreId]
            public virtual Guid Id { get; set; }

            [SitecoreField]
            public virtual string MissingField { get; set; }

        }

        [SitecoreClass]
        public class Concrete3
        {

            [SitecoreField]
            public virtual string MissingField { get; set; }

        }
        namespace Inner
        {
            [SitecoreClass]
            public interface ISubType1 : GlassHtmlFixtureNs.ISubType1
            {
                [SitecoreField]
                string RichText { get; set; }
            }
        }
    }
}
