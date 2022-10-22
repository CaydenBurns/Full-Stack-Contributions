using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Messages;
using Sabio.Models.Requests.Messages;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Api.Hubs;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessageApiController : BaseApiController
    {
        private IMessageService _service = null;
        private IAuthenticationService<int> _auth = null;
        private IUserProfileService _profileService = null;
        private readonly IHubContext<ChatHub, IChatHub> _chatHub = null;
        public MessageApiController(IMessageService service, ILogger<MessageApiController> logger, IAuthenticationService<int> authService, IUserProfileService profileService, IHubContext<ChatHub, IChatHub> chatHub) : base(logger)
        {
            _service = service;
            _auth = authService;
            _chatHub = chatHub;
            _profileService = profileService;
        }

        [HttpPost]
        public ActionResult<SuccessResponse> AddMessage(MessageAddRequest model)
        {
            int userId = _auth.GetCurrentUserId();
            BaseResponse response = null;
            int code = 201;
            try
            {
                ChatMessage newMsg = _service.AddMessage(model, userId);

                if (newMsg == null)
                {
                    code = 404;
                    response = new ErrorResponse("Message not created.");

                    return NotFound404(response);
                }
                else
                {
                    response = new SuccessResponse();
                    _chatHub.Clients.Users(userId.ToString(), model.RecipientId.ToString()).OnMessageAdd(newMsg);
                }

            }
            catch (Exception ex)
            {
                code = 500;

                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);
        }

        [HttpPost("automated")]

        public ActionResult<SuccessResponse> AutomatedMessage(RestaurantMessageAddRequest model)
        {
            int userId = _auth.GetCurrentUserId();
            BaseResponse response = null;
            int code = 201;
            try
            {
                _service.AutomatedMessage(model, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> UpdateMessage(MessageUpdateRequest model)
        {
            int code = 200;
            int userId = _auth.GetCurrentUserId();

            BaseResponse response = null;
            try
            {
                ChatMessage newMsg = _service.UpdateMessage(model, userId);

                if (newMsg == null)
                {
                    code = 404;
                    response = new ErrorResponse("Message not found.");

                    return NotFound404(response);
                }
                else
                {
                    response = new SuccessResponse();
                    _chatHub.Clients.Users(userId.ToString(), newMsg.RecipientId.ToString()).OnMessageUpdate(newMsg);
                }

            }

            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> DeleteMessage(int id)
        {
            int code = 200;
            int userId = _auth.GetCurrentUserId();
            BaseResponse response = null;
            try
            {
                OutboundMessageIds delInfo = _service.DeleteMessage(id, userId);
                if (delInfo == null)
                {
                    code = 404;
                    response = new ErrorResponse("Message not found.");

                    return NotFound404(response);
                }
                else
                {
                    response = new SuccessResponse();
                    _chatHub.Clients.Users(userId.ToString(), delInfo.RecipientId.ToString()).OnMessageDelete(delInfo);
                }

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        #region Get Messages Between Sender/Recipient

        [HttpGet("recipient")]
        public ActionResult<ItemsResponse<Messages>> GetByRecipientId()
        {
            int code = 200;
            BaseResponse response = null;
            int userId = _auth.GetCurrentUserId();

            try
            {
                List<MessageConvoList> list = _service.GetByRecipientId(userId);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Message(s) not found.");

                    return NotFound404(response);
                }
                else
                {
                    response = new ItemsResponse<MessageConvoList> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;

                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpGet("sender")]
        public ActionResult<ItemsResponse<MessageConvoList>> GetBySenderId(int userId)
        {
            int code = 200;
            BaseResponse response = null;
            IUserAuthData user = _auth.GetCurrentUser();

            try
            {
                List<MessageConvoList> list = _service.GetBySenderId(user.Id);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Message(s) not found.");

                    return NotFound404(response);
                }
                else
                {
                    response = new ItemsResponse<MessageConvoList> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;

                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpGet("conversation/{recipientId:int}")]
        public ActionResult<ItemsResponse<MessagesNameAndAvatar>> GetBySenderIdAndRecipientId(int recipientId)
        {
            int code = 200;
            BaseResponse response = null;
            int userId = _auth.GetCurrentUserId();

            try
            {
                List<MessagesNameAndAvatar> list = _service.GetBySenderIdAndRecipientId(userId, recipientId);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Message(s) not found.");

                    return NotFound404(response);
                }
                else
                {
                    response = new ItemsResponse<MessagesNameAndAvatar> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;

                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        #endregion
    }
}