using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Messages
{
    public class Messages
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public int RecipientId { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateRead { get; set; }
    }
}