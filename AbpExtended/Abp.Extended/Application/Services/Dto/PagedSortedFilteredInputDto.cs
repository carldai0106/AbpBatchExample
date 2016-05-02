namespace Abp.Application.Services.Dto
{
    public class PagedSortedFilteredInputDto : PagedInputDto, ISortedResultRequest
    {
        public string Sorting { get; set; }
        public string Filter { get; set; }
    }
}
