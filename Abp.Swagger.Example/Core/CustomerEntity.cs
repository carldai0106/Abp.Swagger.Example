using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace Abp.Swagger.Core
{
    public class CustomerEntity : FullAuditedAndTenantEntity
    {
        public const int FirstNameMaxLength = 16;
        public const int LastNameMaxLength = 16;
        public const int TelphoneMaxLength = 16;
        public const int EmailMaxLength = 64;
        public const int AddressMaxLength = 256;
        public const int CityMaxLength = 32;

        [Required]
        [StringLength(FirstNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(LastNameMaxLength)]
        public string LastName { get; set; }

        [StringLength(AddressMaxLength)]
        public string Address { get; set; }

        [StringLength(CityMaxLength)]
        public string City { get; set; }

        [StringLength(CityMaxLength)]
        public string State { get; set; }

        [StringLength(TelphoneMaxLength)]
        public string Telephone { get; set; }

        [StringLength(EmailMaxLength)]
        public string Email { get; set; }
    }
}
