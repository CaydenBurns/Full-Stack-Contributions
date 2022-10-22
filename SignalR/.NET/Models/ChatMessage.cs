using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain
{
    public class ChatMessage
    {
        public string SenderName { get; set; }
        public Uri SenderAvatar { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
        public DateTime DateSent { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
    }
}