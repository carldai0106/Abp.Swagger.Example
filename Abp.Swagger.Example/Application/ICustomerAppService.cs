using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        Task<GetCustomerForEditOutput> GetCustomer([Description("Plese enter customer id.")]NullableIdInput input);

        [HttpPost, OpenWebApi]
        [WebApiDescription("Customer", "Get customer by id.")]
        Task<GetCustomerForEditOutput> GetCustomerById([Description("Plese enter customer id."), Required]int input);

        [HttpGet, OpenWebApi]
        [WebApiDescription("Customer", "Get all customer.")]
        Task<IList<CustomerListDto>> GetCustomers();

        Task<IList<CustomerListDto>> GetCustomerList();

        [HttpPost, OpenWebApi]
        [WebApiDescription("Customer", "Get customers by input.")]
        Task<PagedResultOutput<CustomerListDto>> GetCustomerToList([Description("Get customer input.")]GetCustomersInput input);
    }
}
