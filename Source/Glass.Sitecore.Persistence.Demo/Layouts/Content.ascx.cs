﻿/*
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Sitecore.Persistence.Demo.Application.Domain;
using System.Web.UI.HtmlControls;

namespace Glass.Sitecore.Persistence.Demo.Layouts
{
    public partial class Content : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ISitecoreContext context = new SitecoreContext();

            DemoClass item = context.GetCurrentItem<DemoClass>();
            title.Text = item.Title;
            body.Text = item.Body;

            links.DataSource = item.Links;
            links.DataBind();
        }
    }
}