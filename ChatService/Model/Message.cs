namespace ChatService.Model
{
    public class Message
    {
        public string SenderId { get;set; }
        public string ReceiverId { get; set; }

        public string GroupId { get; set; }

        public DateTime DateCreate { get; set; }
        public string Content { get; set; }
    }
}
