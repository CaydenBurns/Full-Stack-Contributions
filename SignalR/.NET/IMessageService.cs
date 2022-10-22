using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Messages;
using Sabio.Models.Requests.Messages;
using System;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface IMessageService
    {
        ChatMessage AddMessage(MessageAddRequest model, int userId);
        void AutomatedMessage(RestaurantMessageAddRequest model, int userId);
        ChatMessage UpdateMessage(MessageUpdateRequest model, int userId);
        OutboundMessageIds DeleteMessage(int id, int userId);
        List<MessageConvoList> GetByRecipientId(int recipientId);
        List<MessageConvoList> GetBySenderId(int userId);
        List<MessagesNameAndAvatar> GetBySenderIdAndRecipientId(int recipientId, int userId);
    }
}