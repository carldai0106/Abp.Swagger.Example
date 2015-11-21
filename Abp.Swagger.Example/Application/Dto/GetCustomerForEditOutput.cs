using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Swagger.Core;

namespace Abp.Swagger.Application.Dto
{
    [AutoMap(typeof(CustomerEntity))]
    public class GetCustomerForEditOutput : IOutputDto
    {
        [Description("Customer Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(CustomerEntity.FirstNameMaxLength)]
        [Description("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(CustomerEntity.LastNameMaxLength)]
        [Description("Last Name")]
        public string LastName { get; set; }

        [StringLength(CustomerEntity.AddressMaxLength)]
        [Description("Address")]
        public string Address { get; set; }

        [StringLength(CustomerEntity.CityMaxLength)]
        [Description("City")]
        public string City { get; set; }

        [StringLength(CustomerEntity.CityMaxLength)]
        [Description("State")]
        public string State { get; set; }

        [StringLength(CustomerEntity.TelphoneMaxLength)]
        [Description("Telphone")]
        public string Telephone { get; set; }

        [StringLength(CustomerEntity.EmailMaxLength)]
        [Description("Email")]
        public string Email { get; set; }
    }
}
