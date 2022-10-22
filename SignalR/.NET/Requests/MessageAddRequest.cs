using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Messages
{
    public class MessageAddRequest
    {
        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Message { get; set; }
        // Subject is nullable due to drafts
        [StringLength(100, MinimumLength = 2)]
        public string Subject { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int RecipientId { get; set; }
    }
}