using Microsoft.Extensions.Options;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.AppSettings;
using Sabio.Models.Domain.Stripe;
using Sabio.Models.Requests.Stripe;
using Sabio.Models.Requests.StripeCustomers;
using Sabio.Services.Interfaces;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class StripeAccountLinkService : IStripeAccountLinkService
    {
        private readonly Domain _domain;
        private IDataProvider _data = null;
        private IAuthenticationService<int> _authService = null;
        private AppKeys _appKeys;

        public StripeAccountLinkService(IDataProvider data, IAuthenticationService<int> 
            authService, IOptions<AppKeys> appKeys, IOptions<Domain> domain)
        {
            _data = data;
            _authService = authService;
            _appKeys = appKeys.Value;
            _domain = domain.Value;
        }

        public AccountLink Create(string accId)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeSecretKey;
            var domain = _domain.Url;

            var options = new AccountLinkCreateOptions
            {
                Account = accId,
                RefreshUrl = domain + "/signup/account/reauth",
                ReturnUrl = domain,
                Type = "account_onboarding",
            };
            var service = new AccountLinkService();
            AccountLink account = service.Create(options);
            return account;
        }

        public int InsertPaymentAccount(string accoundId, int userId)
        {
            int id = 0;
            string procName = "[dbo].[PaymentAccounts_Insert]";
            var model = new PaymentAccountAddRequest();

            _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@VendorId", userId);
                col.AddWithValue("@AccountId", accoundId);
                col.AddWithValue("@PaymentTypeId", 1);
                col.AddWithValue("@CreatedBy", userId);
                col.AddWithValue("@ModifiedBy", userId);
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            },
             returnParameters: delegate (SqlParameterCollection returnCollection)
             {
                 object oId = returnCollection["@Id"].Value;
                 int.TryParse(oId.ToString(), out id);
             });
            return id;

        }

        public Session RetrieveSession(string sessionId, int user)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeSecretKey;
            var service = new SessionService();
            Session sessionDetails = service.Get(sessionId);

            SubscriptionService subscriptionService = new SubscriptionService();
            var subObject = subscriptionService.Get(sessionDetails.SubscriptionId);
            SubscriptionSessionAddrequest subSession = new SubscriptionSessionAddrequest();
            subSession.CustomerId = sessionDetails.CustomerId;
            subSession.IsActive = "True";
            subSession.SubscriptionId = sessionDetails.SubscriptionId;
            subSession.DateEnded = sessionDetails.ExpiresAt;
            subSession.UserId = user;
            subSession.ProductId = subObject.Items.Data[0].Price.ProductId;
            SubscriptionInsert(subSession);
            AddStripeCustomer(subSession.ProductId, user);
            CreateProductSubscripton(subSession.ProductId, subSession.SubscriptionId);
            return sessionDetails;
        }


        public Session RetrieveCheckoutSession(string sessionId)
        {

            StripeConfiguration.ApiKey = _appKeys.StripeSecretKey;
            var service = new SessionService();
            Session sessionDetails = service.Get(sessionId);
            return sessionDetails;
        }

        public int CreateProductSubscripton(string productId, string subId)
        {
            int id = 0;
            ProductSubscriptionAddRequest subAdd = new ProductSubscriptionAddRequest();
            string procName = "[dbo].[stripeProductSubscription_Insert]";
            _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                subAdd.ProductId = productId;
                subAdd.SubscriptionId = subId;
                col.AddWithValue("@ProductId", subAdd.ProductId);
                col.AddWithValue("@SubscriptionId", subAdd.SubscriptionId);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            },
             returnParameters: delegate (SqlParameterCollection returnCollection)
             {
                 object oId = returnCollection["@Id"].Value;
                 int.TryParse(oId.ToString(), out id);
             });
            return id;

        }

        public int AddStripeCustomer(string customerId, int userId)
        {
            int id = 0;
            StripeCustomerAddRequest customer = new StripeCustomerAddRequest();
            string procName = "[dbo].[StripeCustomers_Insert]";
            _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                customer.UserId = userId;
                customer.CustomerId = customerId;
                col.AddWithValue("userId", customer.UserId);
                col.AddWithValue("@CustomerId", customer.CustomerId);
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            },
             returnParameters: delegate (SqlParameterCollection returnCollection)
             {
                 object oId = returnCollection["@Id"].Value;
                 int.TryParse(oId.ToString(), out id);
             });
            return id;
        }

        public int SubscriptionInsert(SubscriptionSessionAddrequest sub)
        {
            int id = 0;
            string procName = "[dbo].[Subscriptions_Insert]";
            _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@SubscriptionId", sub.SubscriptionId);
                col.AddWithValue("@IsActive", sub.IsActive);
                col.AddWithValue("@CustomerId", sub.CustomerId);
                col.AddWithValue("@DateEnded", sub.DateEnded);
                col.AddWithValue("@UserId", sub.UserId);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            },
             returnParameters: delegate (SqlParameterCollection returnCollection)
             {
                 object oId = returnCollection["@Id"].Value;
                 int.TryParse(oId.ToString(), out id);
             });
            return id;

        }

        public Account CreateAccount(int userId)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeSecretKey;
            var options = new AccountCreateOptions { Type = "express" };

            var service = new AccountService();
            Account account = service.Create(options);
            InsertPaymentAccount(account.Id, userId);
            return account;
        }
        public List<StripeSubscription> SelectAllSubscriptions()
        {
            List<StripeSubscription> list = null;
            string procName = "[dbo].[StripeSubscription_SelectAll]";
            _data.ExecuteCmd(procName, inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    StripeSubscription subscription = MapSingleSubscription(reader);
                    if (list == null)
                    {
                        list = new List<StripeSubscription>();
                    }
                    list.Add(subscription);
                });
            return list;


        }

        public string SubscriptionSessionCreate(CreateSubscriptionRequest model)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeSecretKey;
            var domain = _domain.Url;
            List<SessionLineItemOptions> list = new List<SessionLineItemOptions>();
            SessionLineItemOptions sessionOptions = new SessionLineItemOptions();
            sessionOptions.Price = model.PriceId;
            sessionOptions.Quantity = 1;
            list.Add(sessionOptions);
            SessionCreateOptions options = new SessionCreateOptions();
            options.Mode = "subscription";
            options.SuccessUrl = domain + "/subscriptions/success?session_id={CHECKOUT_SESSION_ID}";
            options.CancelUrl = domain + "/subscriptions/cancelled";
            options.LineItems = list;
            options.Currency = "usd";
            options.Customer = model.CustomerId;
            var service = new SessionService();
            Session session = service.Create(options);
            return session.Id;
        }

        public Customer CreateCustomerAccount()
        {
            StripeConfiguration.ApiKey = _appKeys.StripeSecretKey;
            var options = new CustomerCreateOptions
            {
                Description = "My First Test Customer (created for API docs at https://www.stripe.com/docs/api)",

            };
            var service = new CustomerService();
            var newCustomer = service.Create(options);
            return newCustomer;
        }
        public string CheckoutSessionCreate(List<Product> prods)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeSecretKey;

            var domain = _domain.Url;

            SessionCreateOptions options = new SessionCreateOptions();
            List<SessionLineItemOptions> list = new List<SessionLineItemOptions>();
            foreach (Product prod in prods)
            {
                SessionLineItemOptions sessionOptions = new SessionLineItemOptions();
                sessionOptions.Price = prod.DefaultPriceId;
                sessionOptions.Quantity = 1;
                list.Add(sessionOptions);
            }
            options.LineItems = list;
            options.Mode = "payment";
            options.SuccessUrl = domain + "/checkout/success?session_id={CHECKOUT_SESSION_ID}";
            options.CancelUrl = domain + "/checkout/cancelled";
            var service = new SessionService();
            Session session = service.Create(options);
            return session.Id;
        }
        private static void AddCustomerParams(StripeCustomerAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@CustomerId", model.CustomerId);
            col.AddWithValue("@UserId", model.UserId);
        }
        private static StripeSubscription MapSingleSubscription(IDataReader reader)
        {
            StripeSubscription subscription = new StripeSubscription();
            int startingIndex = 0;
            subscription.Id = reader.GetSafeInt32(startingIndex++);
            subscription.ProductId = reader.GetSafeString(startingIndex++);
            subscription.Name = reader.GetSafeString(startingIndex++);
            subscription.PriceId = reader.GetSafeString(startingIndex++);
            subscription.imageUrl = reader.GetSafeString(startingIndex++);
            return subscription;
        }

        private static void AddSubParams(CreateSubscriptionRequest model, SqlParameterCollection paramCollection)
        {
            paramCollection.AddWithValue("@PriceId", model.PriceId);
            paramCollection.AddWithValue("@CustomerId", model.CustomerId);

        }


    }
}
