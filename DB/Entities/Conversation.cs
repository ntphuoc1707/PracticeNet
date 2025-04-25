using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.Entities
{
    [Table("Conversation")]
    [PrimaryKey(nameof(GroupId),nameof(UserId))]
    public class Conversation
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }

    }
}
