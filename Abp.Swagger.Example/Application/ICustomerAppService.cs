using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Swagger.Application.Dto;

namespace Abp.Swagger.Application
{
    [WebApiDescription("Customer", "WebApi for Customer.")]
    public interface ICustomerAppService : IApplicationService
    {
        [HttpPost, OpenWebApi]
        [WebApiDescription("Customer", "Get customer by id.")]
        Task<GetCustomerForEditOutput> GetCustomer(NullableIdInput input);

        [HttpGet, OpenWebApi]
        Task<IList<CustomerListDto>> GetCustomers();

        Task<IList<CustomerListDto>> GetCustomerList();
    }
}
