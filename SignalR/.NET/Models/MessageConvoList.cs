using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Messages
{
    public class MessageConvoList
    {
        public Uri RecipientAvatar { get; set; }
        public string RecipientName { get; set; }
        public List<Messages> Conversation { get; set; }
        public int UserId { get; set; }
    }
}