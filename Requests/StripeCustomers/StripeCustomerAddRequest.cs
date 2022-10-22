using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.StripeCustomers
{
    public class StripeCustomerAddRequest
    {
        public int UserId { get; set; }

        public string CustomerId { get; set; }
    }
}
