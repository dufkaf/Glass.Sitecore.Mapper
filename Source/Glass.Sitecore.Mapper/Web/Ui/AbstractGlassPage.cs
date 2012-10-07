using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Sitecore.Data.Items;

namespace Glass.Sitecore.Mapper.Web.Ui
{
    public class AbstractGlassPage : Page
    {
        ISitecoreContext _sitecoreContext;
        GlassHtml _glassHtml;

        /// <summary>
        /// Represents the current Sitecore context
        /// </summary>
        public ISitecoreContext SitecoreContext
        {
            get
            {
                return _sitecoreContext ?? (_sitecoreContext = new SitecoreContext());
            }
            set
            {
                _sitecoreContext = value;
            }
        }

        /// <summary>
        /// Access to rendering helpers
        /// </summary>
        protected virtual GlassHtml GlassHtml
        {
            get
            {
                return _glassHtml ?? (_glassHtml = new GlassHtml(SitecoreContext));
            }
        }

        /// <summary>
        /// Returns either the item specified by the DataSource or the current context item
        /// </summary>
        public Item LayoutItem
        {
            get
            {
                return global::Sitecore.Context.Item;
            }
        }
    }
}
