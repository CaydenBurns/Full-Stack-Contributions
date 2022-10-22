using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Messages
{
    public class OutboundMessageIds
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
    }
}