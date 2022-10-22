using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.StripeCustomers
{
    public class StripeCustomer
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string CustomerId { get; set; }

    }
}
