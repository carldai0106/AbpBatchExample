using System;

namespace Abp.Application.Services.Dto
{
    [Serializable]
    public class PagingDto
    {
        /// <summary>
        /// Total pages
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// First page
        /// </summary>
        public int First { get; set; }
        /// <summary>
        /// Last page
        /// </summary>
        public int Last { get; set; }
        /// <summary>
        /// Previous page
        /// </summary>
        public int Prev { get; set; }
        /// <summary>
        /// Next page
        /// </summary>
        public int Next { get; set; }
        /// <summary>
        /// Current page
        /// </summary>
        public int Current { get; set; }
        /// <summary>
        /// How many buttons you want to show
        /// </summary>
        public int ShowButtons { get; set; }
        /// <summary>
        /// Container Id, Assigned an Id to wrapper the html of paging
        /// </summary>
        public string ContainerId { get; set; }
        /// <summary>
        /// Recourds of current page
        /// </summary>
        public int RecordsOfCurrentPage { get; set; }
    }
}
