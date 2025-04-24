using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.Entities
{
    [Table("Conversation")]
    public class Conversation
    {
        [Key]
        public string GroupId { get; set; }
        [Key]
        public string SenderId { get; set; }
        [Key]
        public string ReceiverId { get; set; }
        public string? Title { get; set; }
        public int? MemberNum { get; set; }

        [Key]
        [StringLength(10)]
        public string Type { get; set; }
        public string? Status { get; set; }
    }
}
