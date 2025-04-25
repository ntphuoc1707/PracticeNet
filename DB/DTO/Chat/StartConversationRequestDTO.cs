using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.DTO.Chat
{
    public class StartConversationRequestDTO
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }
}
