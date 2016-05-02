using System.ComponentModel.DataAnnotations;
namespace Abp.Application.Services.Dto
{
    public class PagedInputDto : IInputDto, IPagedResultRequest
    {
        [Range(1, AbpExtendedConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public PagedInputDto()
        {
            MaxResultCount = AbpExtendedConsts.DefaultPageSize;
        }
    }
}
