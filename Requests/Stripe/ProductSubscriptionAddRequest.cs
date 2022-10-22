using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Stripe
{
    public class ProductSubscriptionAddRequest
    {
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string SubscriptionId { get; set; }
    }
}
