using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Swagger.Core;


namespace Abp.Swagger.Application.Dto
{
    [AutoMapFrom(typeof(CustomerEntity))]
    public class CustomerListDto : EntityDto<int>
    {
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
       
        public string Address { get; set; }
      
        public string City { get; set; }
      
        public string State { get; set; }
      
        public string Telephone { get; set; }
       
        public string Email { get; set; }
    }
}
