using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Abp.Swagger.Application.Dto
{
    public class PagedAndSortedBaseInputDto
    {
        public string FieldName { get; set; }
    }

    [Serializable]
    public class PagedAndSortedInputDto :PagedAndSortedBaseInputDto,
        IInputDto, IPagedResultRequest, ISortedResultRequest
    {
        [Range(1, 1000)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        [Range(0, int.MaxValue)]
        public int PageIndex { get; set; }

        public string Sorting { get; set; }

        public PagedAndSortedInputDto()
        {
            MaxResultCount = 10;
        }
    }
}
