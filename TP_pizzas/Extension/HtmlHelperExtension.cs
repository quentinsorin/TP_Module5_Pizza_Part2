using System.Web.Mvc;

namespace TP_Pizza.Extension
{
    public static class HtmlHelperExtension
    {
        public static MvcHtmlString CustomSubmit<TModel>(this HtmlHelper<TModel> html, string libelle)
        {
            return new MvcHtmlString($"<div class=\"form-group\"><div class=\"col-md-offset-2 col-md-10\"><input type=\"submit\" value=\"{libelle}\" class=\"btn btn-default\" /></div></div>");
        }
    }
}