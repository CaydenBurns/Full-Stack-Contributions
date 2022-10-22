using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Stripe
{
    public class SubscriptionSessionAddrequest
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string SubscriptionId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]

        public int UserId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateEnded { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string IsActive { get; set; }

    }
}
