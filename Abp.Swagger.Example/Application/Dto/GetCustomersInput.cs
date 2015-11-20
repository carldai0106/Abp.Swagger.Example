using Abp.Runtime.Validation;

namespace Abp.Swagger.Application.Dto
{
    public class GetCustomersInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string Filter { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "FirstName";
            }
        }
    }
}
