using Sabio.Models.Domain.Stripe;
using Sabio.Models.Requests.Stripe;
using Sabio.Models.Requests.StripeCustomers;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface IStripeAccountLinkService
    {
        AccountLink Create(string accountId);

        Account CreateAccount(int userId);

        string SubscriptionSessionCreate(CreateSubscriptionRequest model);

        List<StripeSubscription> SelectAllSubscriptions();

        Customer CreateCustomerAccount();

        int AddStripeCustomer(string customerId, int userId);
        string CheckoutSessionCreate(List<Product> prods);
        int InsertPaymentAccount(string accoundId, int userId);

        Session RetrieveSession(string sessionId,int user);

        Session RetrieveCheckoutSession(string sessionId);
    }
}
