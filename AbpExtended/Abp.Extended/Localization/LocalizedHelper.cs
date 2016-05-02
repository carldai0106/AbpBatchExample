using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Abp.Application.Services.Dto;

namespace Abp.Localization
{
    public class LocalizedHelper
    {
        public static IList<CountryDto> GetCountries()
        {
            var list = new List<CountryDto>();

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var region in cultures.Select(culture => 
                new RegionInfo(culture.LCID)).Where(
                region => list.Count(x => x.EnglishName == region.EnglishName) == 0))
            {
                list.Add(new CountryDto
                {
                    Name = region.Name,
                    EnglishName = region.EnglishName,
                    DisplayName = region.DisplayName
                });
            }

            return list;
        }
    }
}
