using System;

namespace Abp.Application.Services.Dto
{
    [Serializable]
    public class CountryDto
    {
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string DisplayName { get; set; }
    }
}
