using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Messages
{
    public class MessagesNameAndAvatar : Messages
    {
        public Uri SenderAvatar { get; set; }
        public string SenderName { get; set; }
        public Uri RecipientAvatar { get; set; }
        public string RecipientName { get; set; }
    }
}