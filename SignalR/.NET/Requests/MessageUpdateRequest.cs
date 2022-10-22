using System;
using System.ComponentModel.DataAnnotations;


namespace Sabio.Models.Requests.Messages
{
    public class MessageUpdateRequest : MessageBodyAddRequest, IModelIdentifier
    {
        public int Id { get; set; }
    }
}