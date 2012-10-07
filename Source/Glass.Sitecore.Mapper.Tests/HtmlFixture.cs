using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Glass.Sitecore.Mapper.Data;
using Glass.Sitecore.Mapper.Tests.HtmlFixtureNS;
using NUnit.Framework;
using System.Collections.Specialized;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Sites;

namespace Glass.Sitecore.Mapper.Tests
{
    [TestFixture]
    public class HtmlFixture
    {
        private Context _context;
        private ISitecoreService _sitecore;
        private Database _db;
        private Guid _itemId;
        private Item _item;
        private GlassHtml _html;
        private string _sltTextOriginal;
        private string _mltTextOriginal;
        private string _intOriginal;
        private string _numberOriginal;


        private const string _sltContent = "Kitty";
        private const string _mltContent = "Dog";
        private const string _intContent = "42";
        private const string _numberContent = "1.21";

        [SetUp]
        public void Setup()
        {
            var loader = new AttributeConfigurationLoader(
               new string[] { "Glass.Sitecore.Mapper.Tests.HtmlFixtureNS, Glass.Sitecore.Mapper.Tests" }
               );
            _context = new Context(loader, new AbstractSitecoreDataHandler[] { });

            _db = global::Sitecore.Configuration.Factory.GetDatabase("master");
            _sitecore = new SitecoreService(_db);
            _itemId = new Guid("{5864308D-A91A-4E74-B8CA-7F27372CBB73}");
            _item = _db.GetItem(new ID(_itemId));
            _html = new GlassHtml(_sitecore);
            _sltTextOriginal = _item["SingleLineText"];
            _mltTextOriginal = _item["MultiLineText"];
            _intOriginal = _item["Integer"];
            _numberOriginal = _item["Number"];

            using (new SecurityDisabler())
            {
                _item.Editing.BeginEdit();
                _item["SingleLineText"] = _sltContent;
                _item["MultiLineText"] = _mltContent;
                _item["Integer"] = _intContent;
                _item["Number"] = _numberContent;
                _item.Editing.EndEdit();
            }

        }

        [TearDown]
        public void TearDown()
        {
            using (new SecurityDisabler())
            {
                _item.Editing.BeginEdit();
                _item["SingleLineText"] = _sltTextOriginal;
                _item["MultiLineText"] = _mltTextOriginal;
                _item["Integer"] = _intOriginal;
                _item["Number"] = _numberOriginal;
                _item.Editing.EndEdit();
            }
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

        /// <summary>
        /// Basic test of property/field resolution
        /// </summary>
        [Test]
        public void RenderField()
        {
            var obj = _sitecore.CreateClass<Super>(false, false, _item);
            string rendered = null;
            using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext("website")))
            {
                rendered = _html.Editable<Super>(obj, x => x.MultiLineText);
            }
            Assert.AreEqual(_mltContent, rendered);
        }

        /// <summary>
        /// Ensure that SingleLineText can be resolved back to Mid when accessed from Super
        /// (and that we don't attempt to resolve from Base!)
        /// </summary>
        [Test]
        public void RenderFieldComplexInheritance()
        {
            var obj = _sitecore.CreateClass<Super>(false, false, _item);
            string rendered = null;
            using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext("website")))
            {
                rendered = _html.Editable<Super>(obj, x => x.SingleLineText);
            }
            Assert.AreEqual(_sltContent, rendered);
        }

        /// <summary>
        /// Ensure that when we override the field mapping, we resolve properly
        /// </summary>
        [Test]
        public void RenderFieldOverride()
        {
            var obj = _sitecore.CreateClass<Super>(false, false, _item);
            string rendered = null;
            using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext("website")))
            {
                rendered = _html.Editable<Super>(obj, x => x.Integer);
            }
            Assert.AreEqual(_numberContent, rendered);
        }

    }

    namespace HtmlFixtureNS
    {
        public class Base
        {
            public virtual string SingleLineText { get; set; }
        }

        [SitecoreClass]
        public class Mid : Base
        {
            [SitecoreId]
            public virtual Guid ID { get; set; }

            [SitecoreField]
            public override string SingleLineText { get; set; }

            [SitecoreField]
            public virtual string Integer { get; set; }
        }

        [SitecoreClass]
        public class Super : Mid
        {
            [SitecoreField]
            public virtual string MultiLineText { get; set; }

            [SitecoreField(FieldName = "Number")]
            public override string Integer { get; set; }
        }
    }
}
