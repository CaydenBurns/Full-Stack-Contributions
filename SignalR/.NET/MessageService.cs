using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Messages;
using Sabio.Models.Requests.Messages;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Sabio.Services
{
    public class MessageService : IMessageService
    {
        IDataProvider _data = null;

        public MessageService(IDataProvider data)
        {
            _data = data;

        }

        public ChatMessage AddMessage(MessageAddRequest model, int userId)
        {
            ChatMessage message = null;

            string procName = "Messages_Insert";
            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col, userId);
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);

            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                message = MapMessageSent(reader, ref index);

            });
            return message;
        }

        public void AutomatedMessage(RestaurantMessageAddRequest model, int userId)
        {

            string procName = "Messages_Insert";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                SqlParameter id = new SqlParameter("@Id", SqlDbType.Int);
                id.Direction = ParameterDirection.Output;
                col.Add(id);
                AddCommonParamsAutomated(model, col, userId);
            }, returnParameters: null);

        }


        public ChatMessage UpdateMessage(MessageUpdateRequest model, int userId)
        {
            ChatMessage message = null;
            string procName = "Messages_Update";
            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Message", model.Message);
                col.AddWithValue("@Subject", model.Subject);
                col.AddWithValue("@Id", model.Id);
                col.AddWithValue("@SenderId", userId);
            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                message = MapMessageSent(reader, ref index);

            });
            return message;
        }
        public OutboundMessageIds DeleteMessage(int id, int userId)
        {
            OutboundMessageIds message = null;
            string procName = "Messages_Delete_ById";
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
                col.AddWithValue("@SenderId", userId);
            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                message = MapOutBoundIds(reader, ref index);

            });
            return message;
        }

        #region Get Messages Between Sender/Recipient 

        public List<MessageConvoList> GetByRecipientId(int userId)
        {
            string procName = "Messages_SelectByRece";
            List<MessageConvoList> list = null;
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@RecipientId", userId);

            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                MessageConvoList message = MapConversation(reader, ref index);
                if (list == null)
                {
                    list = new List<MessageConvoList>();
                }
                list.Add(message);
            });
            return list;
        }

        public List<MessageConvoList> GetBySenderId(int userId)
        {
            string procName = "Message_Select_BySenderId";
            List<MessageConvoList> list = null;
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@SenderId", userId);

            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                MessageConvoList message = MapConversation(reader, ref index);
                if (list == null)
                {
                    list = new List<MessageConvoList>();
                }
                list.Add(message);
            });
            return list;
        }

        public List<MessagesNameAndAvatar> GetBySenderIdAndRecipientId(int userId, int recipientId)
        {
            string procName = "Messages_Select_Conversation";
            List<MessagesNameAndAvatar> list = null;
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@SenderId", userId);
                col.AddWithValue("@RecipientId", recipientId);

            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                MessagesNameAndAvatar message = MapMessageNameAndAvatar(reader, ref index);
                if (list == null)
                {
                    list = new List<MessagesNameAndAvatar>();
                }
                list.Add(message);
            });
            return list;
        }

        #endregion

        #region Mapping

        private static ChatMessage MapMessageSent(IDataReader reader, ref int i)
        {
            ChatMessage message = new ChatMessage();
            message.SenderName = reader.GetSafeString(i++);
            message.SenderAvatar = reader.GetSafeUri(i++);
            message.Message = reader.GetSafeString(i++);
            message.Id = reader.GetSafeInt32(i++);
            message.DateSent = reader.GetSafeDateTime(i++);
            message.SenderId = reader.GetSafeInt32(i++);
            message.RecipientId = reader.GetSafeInt32(i++);
            return message;
        }
        private static OutboundMessageIds MapOutBoundIds(IDataReader reader, ref int i)
        {
            OutboundMessageIds message = new OutboundMessageIds();
            message.Id = reader.GetSafeInt32(i++);
            message.RecipientId = reader.GetSafeInt32(i++);
            return message;
        }

        private static void AddCommonParams(MessageAddRequest model, SqlParameterCollection col, int userId)
        {

            col.AddWithValue("@Message", model.Message);
            col.AddWithValue("@Subject", model.Subject);
            col.AddWithValue("@RecipientId", model.RecipientId);
            col.AddWithValue("@SenderId", userId);
        }

        private static void AddCommonParamsAutomated(RestaurantMessageAddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@Message", model.Message);
            col.AddWithValue("@Subject", model.Subject);
            col.AddWithValue("@RecipientId", userId);
            col.AddWithValue("@SenderId", model.SenderId);
        }

        private static Messages MapMessage(IDataReader reader, ref int i)
        {
            Messages message = new Messages();
            message.Id = reader.GetSafeInt32(i);
            message.Message = reader.GetSafeString(++i);
            message.Subject = reader.GetSafeString(++i);
            message.RecipientId = reader.GetSafeInt32(++i);
            message.DateSent = reader.GetSafeDateTime(++i);
            message.DateRead = reader.GetSafeDateTime(++i);
            return message;
        }

        private static MessagesNameAndAvatar MapMessageNameAndAvatar(IDataReader reader, ref int i)
        {
            MessagesNameAndAvatar message = new MessagesNameAndAvatar();
            message.SenderAvatar = reader.GetSafeUri(i);
            message.SenderName = reader.GetSafeString(++i);
            message.RecipientAvatar = reader.GetSafeUri(++i);
            message.RecipientName = reader.GetSafeString(++i);
            message.Id = reader.GetSafeInt32(++i);
            message.Message = reader.GetSafeString(++i);
            message.Subject = reader.GetSafeString(++i);
            message.RecipientId = reader.GetSafeInt32(++i);
            message.DateSent = reader.GetSafeDateTime(++i);
            message.DateRead = reader.GetSafeDateTime(++i);
            return message;
        }
        private static MessageConvoList MapConversation(IDataReader reader, ref int i)
        {
            MessageConvoList message = new MessageConvoList();
            message.RecipientAvatar = reader.GetSafeUri(i);
            message.RecipientName = reader.GetSafeString(++i);
            message.Conversation = reader.DeserializeObject<List<Messages>>(++i);
            message.UserId = reader.GetSafeInt32(++i);
            return message;
        }

        #endregion
    }
}