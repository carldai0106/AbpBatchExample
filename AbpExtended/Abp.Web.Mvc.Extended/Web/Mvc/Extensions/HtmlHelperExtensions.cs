using System;
using System.Text;
using System.Web.Mvc;
using Abp.Application.Services.Dto;

namespace Abp.Web.Mvc.Extensions
{
    public static class HtmlHelperExtensions
    {
        #region paging css
       /* .pagination {
    margin: 0 0 20px 0;
    text-align: center;
    display: block;
    border-radius: 0;
}


    .pagination ul {
        display: inline-block;
        *display: inline;
        margin-bottom: 0;
        margin-left: 0;
        -webkit-border-radius: 4px;
        -moz-border-radius: 4px;
        border-radius: 4px;
        *zoom: 1;
        -webkit-box-shadow: 0 1px 2px rgba(0,0,0,0.05);
        -moz-box-shadow: 0 1px 2px rgba(0,0,0,0.05);
        box-shadow: 0 1px 2px rgba(0,0,0,0.05);
    }

        .pagination ul > li {
            display: inline;
        }

            .pagination ul > li > a, .pagination ul > li > span {
                float: left;
                padding: 4px 16px;
                line-height: 26px;
                text-decoration: none;
                background-color: #fff;
                border: 1px solid #ddd;
                border-left-width: 0;
                font-size: 16px;
            }

                .pagination ul > li > a:hover, .pagination ul > li > a:focus, .pagination ul > .active > a, .pagination ul > .active > span {
                    background: #1763a7;
                    border: 1px solid #1763a7;
                    border-left: none;
                    color: #fff;
                }

        .pagination ul > .active > a, .pagination ul > .active > span {
            color: #fff;
            cursor: default;
            background: #1763a7;
            border: 1px solid #1763a7;
            border-left: none;
        }

        .pagination ul > .disabled > span, .pagination ul > .disabled > a, .pagination ul > .disabled > a:hover, .pagination ul > .disabled > a:focus {
            color: #999;
            cursor: not-allowed;
            background-color: transparent;
        }

        .pagination ul > li:first-child > a, .pagination ul > li:first-child > span {
            border-left-width: 1px;
            -webkit-border-bottom-left-radius: 4px;
            border-bottom-left-radius: 4px;
            -webkit-border-top-left-radius: 4px;
            border-top-left-radius: 4px;
            -moz-border-radius-bottomleft: 4px;
            -moz-border-radius-topleft: 4px;
        }

        .pagination ul > li:last-child > a, .pagination ul > li:last-child > span {
            -webkit-border-top-right-radius: 4px;
            border-top-right-radius: 4px;
            -webkit-border-bottom-right-radius: 4px;
            border-bottom-right-radius: 4px;
            -moz-border-radius-topright: 4px;
            -moz-border-radius-bottomright: 4px;
        }*/
        #endregion

        public static MvcHtmlString Pager(this HtmlHelper html, PagingDto model, string url)
        {
            var sbHtml = new StringBuilder();

            if (model == null)
                return MvcHtmlString.Create(sbHtml.ToString());

            if (model.TotalPages <= 1) return MvcHtmlString.Create(sbHtml.ToString());

            var containerId = string.Empty;
            if (!string.IsNullOrEmpty(model.ContainerId))
            {
                containerId = String.Format("id=\"{0}\"", model.ContainerId);
            }

            sbHtml.AppendFormat("<div class=\"pagination\" {0}>", containerId);
            sbHtml.Append("<ul>");
            if (model.Current == 1)
            {
                sbHtml.Append("<li class=\"disabled\"><span><i class=\"fa fa-angle-double-left\"></i></span></li>");
            }
            else
            {
                sbHtml.AppendFormat("<li><a href=\"{0}\" title=\"First\"><i class=\"fa fa-angle-double-left\"></i></a></li>", (url + model.First));
            }

            if (model.Prev <= 0)
            {
                sbHtml.Append("<li class=\"disabled\"><span><i class=\"fa fa-angle-left\"></i></span></li>");
            }
            else
            {
                sbHtml.AppendFormat("<li> <a href=\"{0}\" title=\"Previous\"><i class=\"fa fa-angle-left\"></i></a></li>", (url + model.Prev));
            }
            var showButtons = model.ShowButtons;

            var begin = showButtons / 2;

            var start = model.Current - begin;
            if (start > model.TotalPages - showButtons)
            {
                start = model.TotalPages - showButtons;
            }
            if (start <= 0)
            {
                start = 1;
            }

            var end = start + showButtons;
            if (end > model.TotalPages)
            {
                end = model.TotalPages;
            }

            for (var i = start; i <= end; i++)
            {
                if (i == model.Current)
                {
                    sbHtml.AppendFormat("<li class=\"active\"><span>{0}</span></li>", i);
                }
                else
                {
                    sbHtml.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", (url + i), i);
                }
            }

            

            if (model.Next > model.Last)
            {
                sbHtml.Append("<li class=\"disabled\"><span><i class=\"fa fa-angle-right\"></i></span></li>");
            }
            else
            {
                sbHtml.AppendFormat("<li><a href=\"{0}\" title=\"Next\"><i class=\"fa fa-angle-right\"></i></a></li>", (url + model.Next));
            }
            if (model.TotalPages == model.Current)
            {
                sbHtml.Append("<li class=\"disabled\"><span><i class=\"fa fa-angle-double-right\"></i></span></li>");
            }
            else
            {
                sbHtml.AppendFormat("<li><a href=\"{0}\" title=\"Last\"><i class=\"fa fa-angle-double-right\"></i></a></li>", (url + model.Last));
            }

            sbHtml.Append("</ul>");
            sbHtml.Append("</div>");

            return MvcHtmlString.Create(sbHtml.ToString());
        }
    }
}
