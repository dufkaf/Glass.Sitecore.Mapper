using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Linq.Expressions;
using Glass.Sitecore.Mapper.RenderField;

namespace Glass.Sitecore.Mapper.Web.Ui
{
    public class GlassUserControl<T> : AbstractGlassUserControl where T:class
    {
        protected override void OnLoad(EventArgs e)
        {
            Model = SitecoreContext.CreateClass<T>(false, false, LayoutItem);
            base.OnLoad(e);
        }

        /// <summary>
        /// Model to render on the sublayout
        /// </summary>
        public T Model { get; set; }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model => model.Title where Title is field name.
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string Editable(Expression<Func<T, object>> field)
        {

            if (field == null) throw new NullReferenceException("No field set");

            if (Model == null) throw new NullReferenceException("No model set");

            try
            {
                var result = GlassHtml.Editable<T>(Model, field);
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model =&gt; model.Title where Title is field name.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public string Editable(Expression<Func<T, object>> field, string parameters)
        {
            if (field == null) throw new NullReferenceException("No field set");
            if (Model == null) throw new NullReferenceException("No model set");

            try
            {
                var result = GlassHtml.Editable<T>(Model, field, parameters);
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model => model.Title where Title is field name.
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string Editable(Expression<Func<T, object>> field, AbstractParameters parameters)
        {

            if (field == null) throw new NullReferenceException("No field set");

            if (Model == null) throw new NullReferenceException("No model set");

            try
            {
                var result = GlassHtml.Editable<T>(Model, field, parameters);
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model => model.Title where Title is field name.
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string Editable(Expression<Func<T, object>> field, Expression<Func<T, string>> standardOutput)
        {

            if (field == null) throw new NullReferenceException("No field set");

            if (standardOutput == null) throw new NullReferenceException("No standardoutput set");

            if (Model == null) throw new NullReferenceException("No model set");

            try
            {
                var result = GlassHtml.Editable<T>(Model, field, standardOutput);
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        /// <summary>
        /// Makes a field editable via the Page Editor. Use the Model property as the target item, e.g. model => model.Title where Title is field name.
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string Editable(Expression<Func<T, object>> field, Expression<Func<T, string>> standardOutput, AbstractParameters parameters)
        {

            if (field == null) throw new NullReferenceException("No field set");

            if (standardOutput == null) throw new NullReferenceException("No standardoutput set");

            if (Model == null) throw new NullReferenceException("No model set");

            try
            {
                var result = GlassHtml.Editable<T>(Model, field, standardOutput, parameters);
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

       
    }
}
