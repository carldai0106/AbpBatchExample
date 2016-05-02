using System.Web.Mvc;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Security;
using Abp.Web.Mvc.Controllers;

namespace Abp.Web.Mvc.Extensions
{
    /// <summary>
    /// Types of message
    /// </summary>
    public enum MessageTypes
    {
        /// <summary>
        /// Error
        /// </summary>
        Error,
        /// <summary>
        /// Information
        /// </summary>
        Information
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ControllerExtension
    {
        /// <summary>
        /// Output error messages to ModelState
        /// </summary>
        /// <param name="controller">Current Controller</param>
        public static void OutputModelMessage(this AbpController controller)
        {
            if (controller.TempData["ModelState"] != null)
            {
                controller.ViewBag.HasMessage = true;
                var dics = controller.TempData["ModelState"] as ModelStateDictionary;
                if (dics != null)
                {
                    var msd = dics.Dereference<ModelStateDictionary>();
                    foreach (var item in msd)
                    {
                        if (item.Value != null)
                        {
                            foreach (var error in item.Value.Errors)
                            {
                                var strKey = item.Key;
                                if (string.IsNullOrEmpty(strKey))
                                {
                                    strKey = Md5Cipher.GetMd5Str(error.ErrorMessage);
                                }

                                if (!controller.ModelState.ContainsKey(strKey))
                                {
                                    controller.ModelState.AddModelError(strKey, error.ErrorMessage);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="type"></param>
        public static void InputModelError(this AbpController controller, MessageTypes type = MessageTypes.Error)
        {
            controller.TempData["MessageType"] = type == MessageTypes.Error ? "error" : "";
            controller.TempData["ModelState"] = controller.ModelState;
        }

        /// <summary>
        /// Add error message to ModelState
        /// </summary>
        /// <param name="controller">Current Controller</param>
        /// <param name="message">Message</param>
        /// <param name="type"></param>
        public static void AddModelMessage(this AbpController controller, string message, MessageTypes type = MessageTypes.Error)
        {
            controller.ModelState.AddModelError("", message);
            controller.InputModelError(type);
        }

        /// <summary>
        /// Add error message to ModelState
        /// </summary>
        /// <param name="controller">Current Controller</param>
        /// <param name="key">The Key</param>
        /// <param name="message">Message</param>
        /// <param name="type"></param>
        public static void AddModelMessage(this AbpController controller, string key, string message, MessageTypes type = MessageTypes.Error)
        {
            controller.ModelState.AddModelError(key, message);
            controller.InputModelError(type);
        }
       
        /// <summary>
        /// 初始化分页实体类
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="records">总记录数</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="showButtons">需要显示的分页按钮数</param>
        /// <returns></returns>
        public static PagingDto NewPager(this AbpController controller, int pageIndex, int records, int pageSize = 0, int showButtons = 7)
        {
            var pm = GetPager(pageIndex, records, pageSize, showButtons);
            controller.TempData["_PagerEntity"] = pm;
            controller.TempData["_PageIndex"] = pageIndex;
            controller.ViewBag.PageModel = pm;
            return pm;
        }

        public static PagingDto GetPager(int pageIndex, int records, int pageSize = 0, int showButtons = 7)
        {
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var pm = new PagingDto();

            if (records % pageSize == 0)
            {
                pm.TotalPages = records / pageSize;
                pm.RecordsOfCurrentPage = records - ((pm.TotalPages - 1) * pageSize);
            }
            else
            {
                pm.TotalPages = (records / pageSize) + 1;
                pm.RecordsOfCurrentPage = pageSize - ((pm.TotalPages * pageSize) - records);
            }

            pm.First = 1;
            pm.Last = pm.TotalPages;
            pm.Next = pageIndex + 1;
            pm.Prev = pageIndex - 1;
            pm.Current = pageIndex;
            pm.ShowButtons = showButtons;

            return pm;
        }

        /// <summary>
        /// 当执行删除时检查页面的记录数，如果删除的条数等于当前页面的条数，需要将页面索引减1，否则当前页会是空白。
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="records">删除执行后受影响的记录数</param>
        /// <returns></returns>
        public static int CheckPageIndexWhenDeleted(this AbpController controller, int records)
        {
            var page = controller.TempData["_PageIndex"].ToString();
            var pageIndex = 0;
            int.TryParse(page, out pageIndex);
            var pm = controller.TempData["_PagerEntity"] as PagingDto;
            if (pm != null)
            {
                var recordsOfCurrentPage = pm.RecordsOfCurrentPage;
                if (recordsOfCurrentPage == records)
                {
                    pageIndex--;
                    if (pageIndex <= 0)
                    {
                        pageIndex = 1;
                    }
                }
            }

            if (pageIndex <= 0)
                pageIndex = 1;

            return pageIndex;
        }
    }
}
