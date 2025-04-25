using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entities
{
    [Table("ConversationDetail")]
    [PrimaryKey(nameof(GroupId))]
    public class ConversationDetail
    {
        public string GroupId { get; set; }

        [StringLength(10)]
        public string Type { get; set; }

        public DateTime DateCreated { get; set; }

        public string? Title { get; set; }

        public int? MemberNum { get; set; }
    }
}
