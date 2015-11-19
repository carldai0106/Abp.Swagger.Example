using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Swagger.Core;

namespace Abp.Swagger.Application.Dto
{
    [AutoMap(typeof(CustomerEntity))]
    public class GetCustomerForEditOutput : IOutputDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CustomerEntity.FirstNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(CustomerEntity.LastNameMaxLength)]
        public string LastName { get; set; }

        [StringLength(CustomerEntity.AddressMaxLength)]
        public string Address { get; set; }

        [StringLength(CustomerEntity.CityMaxLength)]
        public string City { get; set; }

        [StringLength(CustomerEntity.CityMaxLength)]
        public string State { get; set; }

        [StringLength(CustomerEntity.TelphoneMaxLength)]
        public string Telephone { get; set; }

        [StringLength(CustomerEntity.EmailMaxLength)]
        public string Email { get; set; }
    }
}
