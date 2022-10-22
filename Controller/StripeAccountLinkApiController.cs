using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sabio.Models.AppSettings;
using Sabio.Services.Interfaces;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using Stripe;
using Sabio.Models.Domain.Stripe;
using Sabio.Models.Requests.Stripe;
using System.Collections.Generic;
using Sabio.Models.Requests.StripeCustomers;
using Sabio.Models;
using Stripe.TestHelpers;
using Stripe.Checkout;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/stripe")]
    [ApiController]
    public class StripeAccountLinkApiController : BaseApiController
    {
        private AppKeys _appKeys;
        private IStripeAccountLinkService _service = null;
        private IAuthenticationService<int> _authService;

        public StripeAccountLinkApiController(IOptions<AppKeys> appKeys, IStripeAccountLinkService expressAcc,
            IAuthenticationService<int> authService, ILogger<StripeAccountLinkApiController> logger) : base(logger)
        {
            _appKeys = appKeys.Value;
            _service = expressAcc;
            _authService = authService;
        }

        [HttpPost("signup/account")]
        public ActionResult<ItemResponse<AccountLink>> CreateAccount([FromBody] string accountId)
        {
            BaseResponse response = null;
            int code = 201;
            try
            {
                AccountLink account = _service.Create(accountId);
                response = new ItemResponse<AccountLink> { Item = account };

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Exception Error : , {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpPost("signup")]
        public ActionResult<ItemResponse<Account>> CreateAccount()
        {
            BaseResponse response = null;
            int code = 201;
            try
            {
                IUserAuthData user = _authService.GetCurrentUser();
                Stripe.Account account = _service.CreateAccount(user.Id);
                response = new ItemResponse<Account> { Item = account };

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Exception Error : , {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpPost("subscriptions")]
        public ActionResult<ItemResponse<string>> Create(CreateSubscriptionRequest model)
        {
            BaseResponse response = null;
            int code = 201;
            try
            {

                string sessionId = _service.SubscriptionSessionCreate(model);
                response = new ItemResponse<string> { Item = sessionId };

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Exception Error : , {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpGet("subscriptions")]
        public ActionResult<ItemsResponse<StripeSubscription>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<StripeSubscription> list = _service.SelectAllSubscriptions();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<StripeSubscription> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("subscriptions/session")]
        public ActionResult<ItemResponse<Session>> getSession(string sessionId)
        {
            int code = 200;
            IUserAuthData user = _authService.GetCurrentUser();
            BaseResponse response = null;
            try
            {
                Session session = _service.RetrieveSession(sessionId, user.Id);
                response = new ItemResponse<Session> { Item = session };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("checkout/session")]
        public ActionResult<ItemResponse<Session>> getCheckoutSession(string sessionId)
        {
            int code = 200;

            BaseResponse response = null;
            try
            {
                Session session = _service.RetrieveCheckoutSession(sessionId);
                response = new ItemResponse<Session> { Item = session };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }


        [HttpPost("customers")]
        public ActionResult<ItemResponse<string>> Create(Customer Newcustomer)
        {
            BaseResponse response = null;
            int code = 201;
            try
            {
                Customer CustomerId = _service.CreateCustomerAccount();
                response = new ItemResponse<Customer> { Item = CustomerId };

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Exception Error : , {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpPost("checkout")]
        public ActionResult<ItemResponse<string>> CreateCheckoutSession(List<Product> prods)
        {
            BaseResponse response = null;
            int code = 200;
            try
            {
                string sessionId = _service.CheckoutSessionCreate(prods);
                response = new ItemResponse<string> { Item = sessionId };
                code = 200;
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Exception Error : , {ex.Message}");
            }
            return StatusCode(code, response);
        }

    }
}
