using System;

namespace Abp.Application.Services.Dto
{
    [Serializable]
    public class CountryDto
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}
