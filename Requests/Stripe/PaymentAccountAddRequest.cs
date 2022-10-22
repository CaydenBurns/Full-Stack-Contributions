using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Stripe
{
    public class PaymentAccountAddRequest
    {
        [Required]
        [Range(1, int.MaxValue)]

        public int VendorId { get; set; }
        [Required]
        public string AccountId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int PaymentTypeId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int CreatedBy { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int ModifiedBy { get; set; }
    }
}
