using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Stripe
{
    public class CreateSubscriptionRequest
    {
        public string CustomerId { get; set; }

        public string PriceId { get; set; }
    }
}
