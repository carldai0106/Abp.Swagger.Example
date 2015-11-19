using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Swagger.Application.Dto;
using Abp.Swagger.Core;
using Abp.UI;
using Abp.AutoMapper;

namespace Abp.Swagger.Application
{
    public class CustomerAppService : ApplicationService, ICustomerAppService
    {
        private static IList<CustomerEntity> list;

        public CustomerAppService()
        {
            list = new List<CustomerEntity>();

            for (var i = 1; i < 10; i++)
            {
                list.Add(new CustomerEntity
                {
                    Id = i,
                    FirstName = "FirstName " + i,
                    LastName = "LastName " + i
                });
            }
        }

        public async Task<GetCustomerForEditOutput> GetCustomer(NullableIdInput input)
        {
            if (input == null || input.Id == null)
                throw new UserFriendlyException("Id is invalid.");

            var info = list.FirstOrDefault(x => x.Id == input.Id);

            var output = info.MapTo<GetCustomerForEditOutput>();

            return output;
        }

        public async Task<IList<CustomerListDto>> GetCustomers()
        {
            return list.MapTo<List<CustomerListDto>>();
        }

        public async Task<IList<CustomerListDto>> GetCustomerList()
        {
            return list.MapTo<List<CustomerListDto>>();
        }
    }
}
